namespace GUC.GUI
{
    /// <summary>
    /// A implementation of <see cref="GUCTextBox"/> that hides the input by replacing the visual of the characters with a placeholder symbol.
    /// </summary>
    public class GUCPasswordTextBox : GUCTextBox
    {
        public GUCPasswordTextBox(int x, int y, int w, bool fixedBorders) : base(x, y, w, fixedBorders)
        {
        }

        protected override string OnUpdateVisualString(string displayString)
        {
            return string.Empty.PadRight(displayString.Length, '_');
        }
    }
}
