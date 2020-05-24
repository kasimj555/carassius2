using System.Collections.Generic;

namespace PNUnfolding
{
    public class ChangePriorityCommand : Command // added by Khavanskikh Vladimir
    {
        public int change;
        public List<VTransition> transitions;

        public ChangePriorityCommand(List<VTransition> transitions, int change)
        {
            this.transitions = transitions;
            this.change = change;
        }
    }
}
