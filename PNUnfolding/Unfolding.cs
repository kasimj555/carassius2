using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Model;

namespace PNUnfolding
{
    /// <summary>
    /// Статичный класс с набором методов для развертки сети Петри.
    /// </summary>
    public static class Unfolding
    {
        /// <summary>
        /// Начальная сеть Петри.
        /// </summary>
        public static VPetriNet UNet = VPetriNet.Create();

        /// <summary>
        /// Развертнуая сеть Петри или же ветвящийся процесс.
        /// </summary>
        private static VPetriNet Result = VPetriNet.Create();

        /// <summary>
        /// Разметка столбцов узлов для построения развертки.
        /// </summary>
        public static int Table = 1;

        /// <summary>
        /// Разметка строк узлов для построения развертки.
        /// </summary>
        public static int Row = 1;

        /// <summary>
        /// Конкретный ход при построении развертки.
        /// </summary>
        private static int Turn = 0;

        /// <summary>
        /// Определяет требуется ли строить ветвящийся процесс.
        /// </summary>
        private static bool IsBranchingProcess;

        /// <summary>
        /// При ветвящемся процессе определяет глубину разветки (принимает -1 при построении finite prefix).
        /// </summary>
        private static int Depth;

        /// <summary>
        /// Условный словарь для хранения новых мест при построении развертки.
        /// </summary>
        private static VDictionary<VPlace> petriNewPlaces = new VDictionary<VPlace>();

        /// <summary>
        /// Условный словарь для хранения новых переходов при построении развертки.
        /// </summary>
        private static VDictionary<VTransition> petriNewTransitions = new VDictionary<VTransition>();

        /// <summary>
        /// Метод для проверки сети на 1-безопасность.
        /// </summary>
        /// <returns></returns>
        private static bool IsSafe()
        {
            return UNet.places.All(x => x.NumberOfTokens <= 1);
        }

        /// <summary>
        /// Метод для добавления начальных мест с токенами. При отсутсвии токенов развертка не будет соответсвенно строиться.
        /// </summary>
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

        /// <summary>
        /// Основной метод реализующий алгоритм Милана. Принимает глубину развертки (при значении -1 будет строиться finite prefix) и изначальную сеть.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="originalNet"></param>
        /// <returns>Возвращает соотвественно развертку сети.</returns>
        public static VPetriNet MilanAlgorithm(int depth, PetriNet originalNet)
        {
            if (depth == -1)
                IsBranchingProcess = false;
            else
            {
                IsBranchingProcess = true;
                Depth = depth;
            }

            Sync(originalNet);

            if (!IsSafe())
                throw new ArgumentException("This net is not 1-safe");

            AddFirstPlaces();

            Step();

            AddToResult();

            return Result;
        }

        /// <summary>
        /// Ход при построении развертки.
        /// </summary>
        private static void Step()
        {
            // При построении ветвящегося процесса проверяет на глубину. При достижении определенной глубины прерывает процесс.
            if (IsBranchingProcess)
                if (Turn >= Depth)
                    return;

            // Если все узлы неактивны прекрящает процесс.
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
                                    if (petriNewTransitions.GetKeys(arc.To as VTransition).All(x => ArcAlreadyExist(arc.From as VPlace, x)))
                                    {
                                        petriNewTransitions.Add(arc.To as VTransition);
                                        Result.arcs.Add(new VArc(place, petriNewTransitions.Last()));
                                        Result.arcs.Last().IsDirected = true;
                                    }
                                    else
                                    {
                                        foreach (var VTrn in petriNewTransitions.GetKeys(arc.To as VTransition))
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
                                if ((!IsBranchingProcess) && IsCutOfEvent(petriNewTransitions.Last()))
                                    petriNewTransitions.Last().IsActive = false;
                            }
                        }
                    }
                    place.IsActive = false;
                }
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
                                if ((!IsBranchingProcess) && IsCutOfEvent(petriNewPlaces.Last()))
                                    petriNewPlaces.Last().IsActive = false;
                            }
                        }
                    }
                    transition.IsActive = false;
                }
            }
            ++Turn;
            ++Table;
            Row = 1;
            Step();
        }

        /// <summary>
        /// Проверяется является ли данный узел cut-off event.  
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool IsCutOfEvent(PetriNetNode node)
        {
            foreach (var arc in Result.arcs)
            {
                if (arc.To == node)
                    try
                    {
                        return IsCutOfEvent(arc.From, node);
                    }
                    catch (Exception) { }
            }
            return false;
        }

        /// <summary>
        /// Подметод IsCutOfEvent(PetriNetNode node) который собственно и проверяет на cut-off event.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currentNode"></param>
        /// <returns></returns>
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
                    try
                    {
                        return IsCutOfEvent(arc.From, currentNode);
                    }
                    catch (Exception) { }
            }
            throw new Exception();
        }

        /// <summary>
        /// Метод определяющий содержится ли уже соединение идущее к данному переходу от места с первоночальной данному месту.
        /// </summary>
        /// <param name="place"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        private static bool ArcAlreadyExist(VPlace place, VTransition transition)
        {
            foreach (var vPlace in petriNewPlaces.GetKeys(place))
            {
                foreach (var arc in Result.arcs)
                {
                    if (arc.From == vPlace && arc.To == transition)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Добавление результата в разверную сеть Петри.
        /// </summary>
        private static void AddToResult()
        {
            foreach (var place in petriNewPlaces.Keys)
                Result.places.Add(place);
            foreach (var transition in petriNewTransitions.Keys)
                Result.transitions.Add(transition);
        }

        /// <summary>
        /// Обнуление параметров и синхронизация заданной сети с начальной сетью.
        /// </summary>
        /// <param name="net"></param>
        private static void Sync(PetriNet net)
        {
            Table = 1;
            Row = 1;
            Turn = 0;
            petriNewPlaces.Clear();
            petriNewTransitions.Clear();

            Clear(UNet);
            Clear(Result);

            (var places, var transitions, var arcs) = Util.ModelUtil.FromOriginalModel(net.MyClone());
            UNet.arcs.AddRange(arcs);
            UNet.places.AddRange(places);
            UNet.transitions.AddRange(transitions);
        }

        /// <summary>
        /// Чистка сети.
        /// </summary>
        /// <param name="net"></param>
        private static void Clear(VPetriNet net)
        {
            net.arcs.Clear();
            net.places.Clear();
            net.transitions.Clear();
        }
    }
}
