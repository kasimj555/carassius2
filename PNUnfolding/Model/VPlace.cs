using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using Core.Model;

namespace PNUnfolding
{
    public class VPlace : PetriNetNode
    {
        private Place syncPlace;
        public Place SyncPlace => syncPlace;
        public bool IsBound => syncPlace != null;
        public void BindSyncPlace(Place place)
        {
            syncPlace = place;
            BindSyncBase(place);
        }

        public override void Sync()
        {
            if (syncPlace == null)
                return;
            syncPlace.Tokens = _numberOfTokens;
            base.Sync();
        }
        public static int Counter { get; set; }

        public int NumberOfTokens
        {
            get { return _numberOfTokens; }
            set
            {
                _numberOfTokens = value;
                Sync();
            }
        }

        public VPlace(double x, double y, int numberOfTokens, string id)
            : base(x, y, id)
        {
            NumberOfTokens = numberOfTokens;
        }
        //todo: remove this counter, ужас
        public static VPlace Create(double x, double y)
        {
            Counter++;
            var idConstructionString = "";
            idConstructionString = "place" + Counter;

            return new VPlace(x, y, 0, idConstructionString);
        }

        public VPlace Copy()
        {
            return new VPlace(CoordX, CoordY, NumberOfTokens, Id);
        }

        public List<Ellipse> TokensList = new List<Ellipse>(); //todo И вот это тоже представление. Необходимо сделать отдельные классы для представления
        public Label NumberOfTokensLabel = new Label(); //TODO очень странная сущность здесь. убрать надо отсюда
        private int _numberOfTokens;
    }
}