using System.Collections.Generic;

namespace PNUnfolding
{
    public class DragCommand : Command
    {
        public List<PetriNetNode> figuresBeforeDrag;
        public List<PetriNetNode> figuresAfterDrag;

        public DragCommand(List<PetriNetNode> beforeDrag, List<PetriNetNode> afterDrag)
        {
            figuresBeforeDrag = beforeDrag;
            figuresAfterDrag = afterDrag;
        }
    }
}
