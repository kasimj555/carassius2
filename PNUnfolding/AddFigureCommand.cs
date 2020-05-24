using System.Windows.Shapes;

namespace PNUnfolding
{
    public class AddFigureCommand : Command
    {
        public PetriNetNode newFigure;
        public Shape shape;
        public AddFigureCommand(PetriNetNode thisFigure, Shape thisShape)
        {
            newFigure = thisFigure;
            shape = thisShape;
        }
    }
}
