using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PNUnfolding
{
    /// <summary>
    /// Author: Natalia Nikitina
    /// Command classes for undo/redo
    /// PAIS Lab, 2014
    /// </summary>
    public class Command
    {
        public static Stack<Command> ExecutedCommands = new Stack<Command>();
        public static Stack<Command> CanceledCommands = new Stack<Command>();
        public Command()
        {
            PNEditorControl.isSomethingChanged = true;
        }
    }

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

    public class AddArcCommand : Command
    {
        public VArc newArc;
        public bool isNonOriented;
        public AddArcCommand(VArc thisArc)
        {
            newArc = thisArc;
        }
    }

    public class AddTokensCommand : Command
    {
        public VPlace markedPlace;
        public object senderFigure;
        public int oldNumber, newNumber;
        public AddTokensCommand(VPlace marked, int oldNum, int newNum)
        {
            markedPlace = marked;
            oldNumber = oldNum;
            newNumber = newNum;
        }
    }

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
