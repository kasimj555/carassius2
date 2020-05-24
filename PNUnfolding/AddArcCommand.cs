namespace PNUnfolding
{
    public class AddArcCommand : Command
    {
        public VArc newArc;
        public bool isNonOriented;
        public AddArcCommand(VArc thisArc)
        {
            newArc = thisArc;
        }
    }
}
