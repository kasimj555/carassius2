namespace PNUnfolding
{
    public class ChangeNameCommand : Command
    {
        public PetriNetNode namedFigure;
        public string oldName, newName;
        public ChangeNameCommand(PetriNetNode figure, string oldN, string newN)
        {
            namedFigure = figure;
            oldName = oldN;
            newName = newN;
        }
    }
}
