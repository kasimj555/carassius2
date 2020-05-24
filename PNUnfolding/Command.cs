using System.Collections.Generic;
using System.Windows.Controls;

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
}
