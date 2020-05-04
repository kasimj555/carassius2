using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Model;

namespace PNUnfolding
{
    public static class Unfolding
    {
        public static VPetriNet UNet = VPetriNet.Create();

        private static VPetriNet Result = VPetriNet.Create();

        public static PetriNet Basic { get; private set; }

        public static int Table = 1;

        public static int Row = 1;

        private static int Turn = 0;

        private static bool IsBranchingProcess;

        private static int Depth;

        private static int CurrentTurn = 0;

        private static VDictionary<VPlace> petriNewPlaces = new VDictionary<VPlace>();

        private static VDictionary<VTransition> petriNewTransitions = new VDictionary<VTransition>();

        private static bool IsSafe()
        {
            return UNet.places.All(x => x.NumberOfTokens <= 1);
        }

        private static void AddFirstPlaces()
        {
            foreach (var place in UNet.places)
                if (place.NumberOfTokens == 1)
                {
                    petriNewPlaces.Add(place);
                    petriNewPlaces.Last().NumberOfTokens = 1;
                    ++Row;
                }
            ++Table;
            Row = 1;
        }

        public static VPetriNet MilanAlgorithm(int depth)
        {
            if (depth == -1)
                IsBranchingProcess = false;
            else
            {
                IsBranchingProcess = true;
                Depth = depth;
            }

            Sync();

            if (!IsSafe())
                throw new ArgumentException("This net is not 1-safe");

            AddFirstPlaces();

            Step();

            AddToResult();

            return Result;
        }

        private static void Step()
        {
            if (IsBranchingProcess)
                if (CurrentTurn >= Depth)
                    return;

            if (petriNewPlaces.Keys.All(x => !x.IsActive) && petriNewTransitions.Keys.All(x => !x.IsActive))
                return;

            if (Turn % 2 == 0)
            {
                foreach (var place in petriNewPlaces.Keys)
                {
                    if (place.IsActive)
                    {
                        foreach (var arc in UNet.arcs)
                        {
                            if (arc.From == petriNewPlaces[place])
                            {
                                if (petriNewTransitions.Exist(arc.To as VTransition))
                                {
                                    if(petriNewTransitions.GetKeys(arc.To as VTransition).All(x => ArcAlreadyExist(arc.From as VPlace, x)))
                                    {
                                        petriNewTransitions.Add(arc.To as VTransition);
                                        Result.arcs.Add(new VArc(place, petriNewTransitions.Last()));
                                        Result.arcs.Last().IsDirected = true;
                                    }
                                    else
                                    {
                                        foreach(var VTrn in petriNewTransitions.GetKeys(arc.To as VTransition))
                                        {
                                            if (!ArcAlreadyExist(arc.From as VPlace, VTrn))
                                            {
                                                Result.arcs.Add(new VArc(place, VTrn));
                                                Result.arcs.Last().IsDirected = true;
                                            }
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    petriNewTransitions.Add(arc.To as VTransition);
                                    Result.arcs.Add(new VArc(place, petriNewTransitions.Last()));
                                    Result.arcs.Last().IsDirected = true;
                                }
                                ++Row;
                                if (IsCutOfEvent(petriNewTransitions.Last()))
                                    petriNewTransitions.Last().IsActive = false;
                            }
                        }
                    }
                    place.IsActive = false;
                }
                //foreach (var vPlace in petriNewPlaces.Keys)
                //{
                //    if (vPlace.IsActive)
                //    {
                //        foreach (var transition in UNet.transitions)
                //        {
                //            List<VPlace> toTransition = ToThisTransition(transition);
                //            if (toTransition.Count == 0)
                //                continue;
                //            if (!toTransition.All(x => petriNewPlaces.Exist(x)))
                //                continue;
                //            if (!petriNewTransitions.Exist(transition))
                //            {
                //                petriNewTransitions.Add(transition);
                //                foreach (var toVtr in toTransition)
                //                {
                //                    Result.arcs.Add(new VArc(petriNewPlaces.GetKey(toVtr), petriNewTransitions.Last()));
                //                    ++Row;
                //                    petriNewPlaces.GetKey(toVtr).IsActive = false;
                //                }
                //            }
                //            else
                //            {
                //                foreach (var toVtr in toTransition)
                //                {
                //                    foreach (var place in petriNewPlaces.GetKeys(toVtr))
                //                    {
                //                        if (!ArcAlreadyExist(place, transition))
                //                        {
                //                            petriNewTransitions.Add(transition);
                //                            Result.arcs.Add(new VArc(toVtr, petriNewTransitions.Last()));
                //                            ++Row;
                //                        }
                //                        place.IsActive = false;
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            }
            else
            {
                foreach (var transition in petriNewTransitions.Keys)
                {
                    if (transition.IsActive)
                    {
                        foreach (var arc in UNet.arcs)
                        {
                            if (arc.From == petriNewTransitions[transition])
                            {
                                petriNewPlaces.Add(arc.To as VPlace);
                                Result.arcs.Add(new VArc(transition, petriNewPlaces.Last()));
                                Result.arcs.Last().IsDirected = true;
                                ++Row;
                                if (IsCutOfEvent(petriNewPlaces.Last()))
                                    petriNewPlaces.Last().IsActive = false;
                            }
                        }
                    }
                    transition.IsActive = false;
                }
            }
            ++Turn;
            ++Table;
            ++CurrentTurn;
            Row = 1;
            Step();
        }

        private static bool IsCutOfEvent(PetriNetNode node)
        {
            foreach (var arc in Result.arcs)
            {
                if (arc.To == node)
                    return IsCutOfEvent(arc.From, node);
            }
            return false;
        }

        private static bool IsCutOfEvent(PetriNetNode node, PetriNetNode currentNode)
        {
            if (node.GetType() == currentNode.GetType())
            {
                if (node is VPlace)
                {
                    if (petriNewPlaces[node as VPlace] == petriNewPlaces[currentNode as VPlace])
                        return true;
                }
                else
                {
                    if (petriNewTransitions[node as VTransition] == petriNewTransitions[currentNode as VTransition])
                        return true;
                }
            }
            foreach (var arc in Result.arcs)
            {
                if (arc.To == node)
                    return IsCutOfEvent(arc.From, currentNode);
            }
            return false;
        }

        public static List<VPlace> ToThisTransition(VTransition transition)
        {
            List<VPlace> list = new List<VPlace>();
            foreach (var arc in UNet.arcs)
            {
                if (arc.To == transition)
                {
                    list.Add(arc.From as VPlace);
                }
            }
            return list;
        }

        private static bool ArcAlreadyExist(VPlace place,VTransition transition)
        {
            foreach(var arc in Result.arcs)
            {
                if (arc.From == petriNewPlaces.GetKey(place) && arc.To == transition)
                    return true;
            }
            return false;
        }

        private static void AddToResult()
        {
            foreach (var place in petriNewPlaces.Keys)
                Result.places.Add(place);
            foreach (var transition in petriNewTransitions.Keys)
                Result.transitions.Add(transition);
        }

        private static void Sync()
        {
            Table = 1;
            Row = 1;
            Turn = 0;
            petriNewPlaces.Clear();
            petriNewTransitions.Clear();

            Basic = MainController.Self.Net.Clone();

            Clear(UNet);
            Clear(Result);

            (var places, var transitions, var arcs) = Util.ModelUtil.FromOriginalModel(Basic);
            UNet.arcs.AddRange(arcs);
            UNet.places.AddRange(places);
            UNet.transitions.AddRange(transitions);
        }

        private static void Clear(VPetriNet net)
        {
            net.arcs.Clear();
            net.places.Clear();
            net.transitions.Clear();
        }
    }
}
