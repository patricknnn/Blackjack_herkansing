using Windows.UI.Xaml.Controls;
namespace BlackJackApp.Presentation
{
    /// <summary>
    /// Class used to represent an item in the navigation menu SplitView
    /// </summary>
    public class NavMenuItem
    {
        public string Label { get; set; }
        public Symbol Symbol { get; set; }

        public char SymbolAsChar
        {
            get { return (char)Symbol; }
        }
    }
}
