using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Core.Model;

namespace PNUnfolding
{
    public class VArc
    {
        private Arc syncArc;
        public Arc SyncArc => syncArc;
        public bool IsBound => syncArc != null;

        public void BindSyncArc(Arc sync)
        {
            syncArc = sync;
            Sync();
        }

        public void Sync()
        {
            if (syncArc == null)
                return;
            syncArc.Id = Id;
            syncArc.Weight = int.Parse(weight);
        }

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                Sync();
            }
        }

        public static int Counter;

        public PetriNetNode From { get; set; }
        public PetriNetNode To { get; set; }
        public VArc() { }
        public VArc(PetriNetNode first, PetriNetNode second)
        {
            //TODO Здесь очень большая опасность в случае сетей Петри!
            //TODO Можно засунуть неправильную сеть
            //TODO сейчас проверяю при создании сети вообще
            //Debug.Assert(((first is Place) && (second is Place)) || ((first is Transition) && (second is Transition)), "first and second must be of different types");

            From = first;
            To = second;
            Counter++;//какой ужас! в конструкторе!!!
            Id = "arc" + Counter;
            Weight = "1";
        }

        public bool DeleteOrNot;
        public bool IsSelect;
        private string weight;

        public string Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                Sync();
            }
        }
        public Label WeightLabel = null; //todo Это на самом деле view, нужно убрать отсюда
        public bool IsDirected;
        private string _id;

        public void DetectIdMatches(IList<VArc> allArcs)
        {
            if (allArcs.Count < 1 || Id.Length < 3) return;
            if (InvalidArcId(Id)) return;

            var numb = 0;
            foreach (var a in allArcs)
                if (Id == a.Id)
                {
                    numb++;
                    Id = a.Id + "-" + numb;
                }
        }

        public void AddToThisArcsLists() //todo ЗАЧЕм ОНО вообще нужно?!
        {
            if (!From.ThisArcs.Contains(this))
                From.ThisArcs.Add(this);
            if (!To.ThisArcs.Contains(this))
                To.ThisArcs.Add(this);
        }

        public int TimesExistInList(List<VArc> arcs)
        {
            return arcs.Count(arc => Equals(arc));
        }

        private static bool InvalidArcId(string id)
        {
            var tmp = id.Substring(0, 3);
            return tmp != "arc" || tmp != "tra";
        }
    }
}