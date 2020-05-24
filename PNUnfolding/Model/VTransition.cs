using Core.Model;

namespace PNUnfolding
{
    public class VTransition : PetriNetNode
    {
        private Transition syncTransition;
        public Transition SyncTransition => syncTransition;
        public bool IsBound => syncTransition != null;
        private int _priority;

        public void BindSyncTransition(Transition sync)
        {
            syncTransition = sync;
            syncTransition.Priority = Priority;
            BindSyncBase(sync);
        }

        public override void Sync()
        {
            if(syncTransition == null)
                return;
            syncTransition.Priority = _priority;
            base.Sync();
        }
        public int Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                Sync();
            }
        } // added by Khavanskikh Vladimir

        public static int Counter { get; set; }

        public VTransition(double x, double y, string id)
            : base(x, y, id)
        {
            Priority = 0; // added by Khavanskikh Vladimir
        }
        //todo:remove counter
        public static VTransition Create(double x, double y)
        {
            Counter++;
            return new VTransition(x, y, "transition" + Counter);
        }

        public VTransition Copy()
        {
            return new VTransition(CoordX, CoordY, Id);
        }
    }
}