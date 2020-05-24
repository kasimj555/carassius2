namespace PNUnfolding
{
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
}
