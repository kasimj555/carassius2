using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Model;

namespace PNUnfolding
{
    public class PetriNetNode : Node
    {
        private NodeBase syncedNode;
        public NodeBase SyncedNode => syncedNode;

        protected void BindSyncBase(NodeBase nb)
        {
            syncedNode = nb;
            Sync();
        }

        public virtual void Sync()
        {
            if(syncedNode == null)
                return;
            syncedNode.Position = (_coordX, _coordY);
            syncedNode.Label = _label;
            syncedNode.Id = _id;
        }

        public double CoordX
        {
            get { return _coordX; }
            set
            {
                _coordX = value;
                Sync();
            }
        }

        public double CoordY
        {
            get { return _coordY; }
            set
            {
                _coordY = value;
                Sync();
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                Sync();
            }
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

        public List<VArc> ThisArcs = new List<VArc>();    //todo здесь сделать интерфейс нормальный, инкапсулировать    

        public void ConnectTo(VArc arc)
        {
            if (arc.TimesExistInList(ThisArcs) == 0)
                ThisArcs.Add(arc);
        }

        public bool IsChecked;
        public int ModelNumber;
        public bool IsSelect;

        public int Column; //todo Вот эти штуки тоже невнятные
        public int Row; //todo Здесь надо бы как-то поменять, убрать куда-то
        public double SpaceCoefficient = 1; // это лейаут ... тоже почти представление
        public double XDistance, YDistance;
        public double NetForceX, NetForceY, VelocityX, VelocityY;
        private double _coordX;
        private double _coordY;
        private string _label;
        private string _id;

        protected PetriNetNode(double x, double y, string id)
        {
            CoordX = x;
            CoordY = y;
            Id = id;
            Label = "";
        }

        //TODO Временная мера для рефакторинга. Потом убрать вообще! Фигуры создавать нельзя
        public static PetriNetNode Create()
        {
            return new PetriNetNode(0, 0, "");
        }

        public void DetectIdMatches(List<PetriNetNode> figures)
        {
            if (figures.Count < 1 || Id.Length < 5) return;
            if (InvalidFigureId(Id)) return;

            var numb = 0;
            foreach (var f in figures)
                if (Id == f.Id && f != this)
                {
                    numb++;
                    Id = f.Id + "-" + numb;
                }
        }

        private static bool InvalidFigureId(string id)
        {
            var tmp = id.Substring(0, 4);
            return tmp != "plac" || tmp != "tran" || tmp != "node" || tmp != "stat";
        }

        /// <summary>
        /// Свойство определяющее является ли данный узел активным в разветке. (Используется в разветке сетей Петри)
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}