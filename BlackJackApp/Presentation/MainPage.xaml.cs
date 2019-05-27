using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BlackJackApp.Presentation;

namespace BlackJackApp
{
    /// <summary>
    /// Main Page
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            _frmContent.Navigate(typeof(SplashPage));
        }

        /// <summary>
        /// Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(InstructionsPage))
            {
                _txtPageTitle.Text = "Information";
                _lstAppNavigation.SelectedItem = _uiNavInstructions;
                _navSplitView.DisplayMode = SplitViewDisplayMode.Inline;
            }
            else if (e.SourcePageType == typeof(BlackJack))
            {
                _txtPageTitle.Text = "Blackjack Game";
                _lstAppNavigation.SelectedItem = _uiNavBlackJack;
                _navSplitView.DisplayMode = SplitViewDisplayMode.Inline;
                _navSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNavigationItemClicked(object sender, ItemClickEventArgs e)
        {
            NavMenuItem navMenuItem = e.ClickedItem as NavMenuItem;
            
            if (navMenuItem == _uiNavInstructions)
            {
                _frmContent.Navigate(typeof(InstructionsPage));
            }

            else if (navMenuItem == _uiNavBlackJack)
            {
                _frmContent.Navigate(typeof(BlackJack));
            }

        }
    }
}