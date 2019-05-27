using BlackJackApp.GameLogic;
using BlackJackApp.Serializer;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace BlackJackApp.Presentation
{
    /// <summary>
    /// Class used with the Blackjack Page
    /// </summary>
    public sealed partial class BlackJack : CardPage
    {

        // game
        private BlackJackGame _game;

        // dealer card timer
        private DispatcherTimer _DealerTimer;

        // game started
        private bool _started;

        // first timer tick
        private bool _firstTick;

        // serializer
        private BlackJackTextSerializer _serializer;

        /// <summary>
        /// Constructor
        /// </summary>
        public BlackJack()
        {
            this.InitializeComponent();
            _game = new BlackJackGame();
            _serializer = new BlackJackTextSerializer();

            LoadGame();

            // initialize timer
            _DealerTimer = new DispatcherTimer();
            _DealerTimer.Interval = TimeSpan.FromMilliseconds(500);
            _DealerTimer.Tick += OnDealerCardTimerTick;

            _uiDealerScoreBorder.Visibility = Visibility.Collapsed;
            
            // disable all buttons
            _btnStart.IsEnabled = false;
            _btnHold.IsEnabled = false;
            _btnHit.IsEnabled = false;
            _btnDouble.IsEnabled = false;

            // started set to false
            _started = false;
        }

        /// <summary>
        /// Scaling the grid
        /// </summary>
        public override Grid MainGrid
        {
            get
            {
                return _grid;
            }
        }

        /// <summary>
        /// Method for starting round
        /// </summary>
        private async void StartRound()
        {
            // start the round
            _game.StartRound();

            _firstTick = true;
            _started = true;

            // disable buttons
            _btnOpenPane.IsEnabled = false;
            _btnStart.IsEnabled = false;
            _uiPlayerChip.IsEnabled = false;

            // set visuals
            _btnOpenPane.Visibility = Visibility.Collapsed;
            _uiDealerScoreBorder.Visibility = Visibility.Collapsed;
            _txtDealerTotal.Text = "";

            // draw
            DrawCards(_canvasPlayerHand, _game.Player.Hand, Alignment.Center);
            DrawCards(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);

            // if the player has a soft-hand value
            if (_game.Player.SoftHandValue != 0)
            {
                if (_game.Player.SoftHandValue == 21)
                {
                    _txtPlayerTotal.Text = _game.Player.SoftHandValue.ToString();
                }
                
                else
                {
                    _txtPlayerTotal.Text = $"{_game.Player.Score.ToString()}/{_game.Player.SoftHandValue.ToString()}";
                }
            }

            // if the player does not have a soft-hand value
            else
            {
                _txtPlayerTotal.Text = _game.Player.Score.ToString();
            }

            // draw cards on screen
            DrawCardsOnScreen();

            // check for blackjack
            CheckBlackJack();

            if (_game.DetermineBlackJack() == "none")
            {
                // enable buttons
                _btnHold.IsEnabled = true;
                _btnHit.IsEnabled = true;

                if (_game.Player.GameMoney >= _game.Player.Bet)
                {
                    // enable double down button
                    _btnDouble.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Method for round end
        /// </summary>
        /// <param name="reason"></param>
        private async void EndRound(string reason)
        {
            // stop the dealer card timer
            _DealerTimer.Stop();

            // disable buttons
            _btnHold.IsEnabled = false;
            _btnHit.IsEnabled = false;
            _btnDouble.IsEnabled = false;

            _uiFinalTextBorder.Visibility = Visibility.Visible;

            _txtDisplayOutcome.Text = reason;

            await Task.Delay(TimeSpan.FromMilliseconds(1500));

            _started = false;

            // clear the canvas
            _canvasDealerHand.Children.Clear();
            _canvasPlayerHand.Children.Clear();
            _btnOpenPane.IsEnabled = true;
            _btnOpenPane.Visibility = Visibility.Visible;

            // update visuals
            _txtGameMoney.Text = (_game.Payout().ToString());
            _txtChipCount.Text = $"Chip Total: {_game.Player.GameMoney.ToString()}";

            // player has chips
            if (_game.Player.GameMoney != 0)
            {
                _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                _uiPlayerChips.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                _uiPlayerChip.IsEnabled = true;
                _txtDisplayOutcome.Text = "Please make a bet";
            }

            // player does not have chips
            else
            {
                _txtDisplayOutcome.Text = "Buy chips from the menu";
                _txtGameMoney.Text = "";
            }

            // save game
            SaveGame();

            // update visuals
            _uiBetArea.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            _uiBetArea.Fill = new SolidColorBrush();
            _uiDealerScoreBorder.Visibility = Visibility.Collapsed;
            _game.Player.Bet = 0;
            _txtBet.Text = "";
            _txtPlayerTotal.Text = "";
        }

        /// <summary>
        /// Method for drawing screen
        /// </summary>
        protected override void DrawCardsOnScreen()
        {
            if (_started)
            {
                DrawCards(_canvasPlayerHand, _game.Player.Hand, Alignment.Center);
                DrawCards(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);
            }
        }

        /// <summary>
        /// Method for blackjack
        /// </summary>
        private async void CheckBlackJack()
        {
            // blackjack
            if (_game.DetermineBlackJack() != "none")
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                // reveal dealer cards
                _game.Dealer.TurnCardsFaceUp();
                DrawCards(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);
                _uiDealerScoreBorder.Visibility = Visibility.Visible;

                // player has a soft-hand value
                if (_game.Player.SoftHandValue != 0)
                {
                    _game.Player.Score = _game.Player.SoftHandValue;
                }

                // dealer has a soft hand value
                if (_game.Dealer.SoftHandValue != 0)
                {
                    _game.Dealer.Score = _game.Dealer.SoftHandValue;
                }

                // update the score 
                _txtPlayerTotal.Text = _game.Player.Score.ToString();
                _txtDealerTotal.Text = _game.Dealer.Score.ToString();

                // delay
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                _uiFinalTextBorder.Visibility = Visibility.Visible;

                if (_game.DetermineBlackJack() == "player")
                {
                    EndRound("Player has BLACKJACK");
                }
                else if (_game.DetermineBlackJack() == "dealer")
                {
                    EndRound("Dealer has BLACKJACK");
                }
                else if (_game.DetermineBlackJack() == "draw")
                {
                    EndRound("Draw");
                }
            }
        }

        /// <summary>
        /// Method to save game
        /// </summary>
        private void SaveGame()
        {
            _game.Save(_serializer);
        }

        /// <summary>
        /// Method to load game
        /// </summary>
        private async void LoadGame()
        {
            bool wait = false;

            // check for data file
            if (File.Exists($"{_serializer.DirectoryPath}\\{_serializer.FilePath}"))
            {
                // load the game from file
                _game.Load(_serializer);
                _uiFinalTextBorder.Visibility = Visibility.Visible;

                // check for name
                if (_game.Player.Name != "")
                {
                    _txtDisplayOutcome.Text = $"Hello there, {_game.Player.Name.ToUpper()}";
                }
                else
                {
                    _txtDisplayOutcome.Text = $"Hello there";
                }

                // update visuals
                _txtNameInput.Text = _game.Player.Name;
                _uiEditProfilePane.IsPaneOpen = false;
                _txtChipCount.Text = $"CHIP TOTAL: {_game.Player.GameMoney.ToString()}";
                _txtTotalMoney.Text = $"${_game.Player.Money.ToString()}";

                // delay for message read
                wait = true;

            }

            // no file found
            else
            {
                // delay
                await Task.Delay(TimeSpan.FromMilliseconds(10));
                _uiEditProfilePane.IsPaneOpen = true;
                _btnAddFunds.IsEnabled = true;
                _btnAddGameMoney.IsEnabled = false;
            }

            // update labels
            _txtGameMoney.Text = _game.Player.GameMoney.ToString();
            _txtTotalMoney.Text = $"${_game.Player.Money.ToString()}";

            // no name
            if (_game.Player.Name == "")
            {
                _txtNameInput.PlaceholderText = "Your name...";
            }

            // no money
            if (_game.Player.Money == 0)
            {
                _btnAddGameMoney.IsEnabled = false;
            }

            // no chips
            if (_game.Player.GameMoney == 0)
            {
                _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                _uiPlayerChips.Fill = new SolidColorBrush();
                _txtGameMoney.Text = "";
            }

            // wait
            if (wait == true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(3000));

                // no chips
                if (_game.Player.GameMoney == 0)
                {
                    _txtDisplayOutcome.Text = "Buy chips from the user menu";
                }
                else
                {
                    _txtDisplayOutcome.Text = "Please place a bet";
                }

            }

            // no file
            else
            {
                _txtDisplayOutcome.Text = "Buy chips from the user menu";
            }

        }

        /// <summary>
        /// Event Handler for chip stack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerChipClick(object sender, PointerRoutedEventArgs e)
        {
            // player has chips
            if (_game.Player.GameMoney != 0)
            {
                // update visuals
                _txtDisplayOutcome.Text = "";
                _uiFinalTextBorder.Visibility = Visibility.Collapsed;

                _game.Player.Bet += 5;
                _txtBet.Text = _game.Player.Bet.ToString();
                _game.Player.GameMoney -= 5;

                // update text labels
                _txtGameMoney.Text = _game.Player.GameMoney.ToString();
                _txtChipCount.Text = $"Chip Total: {_game.Player.GameMoney.ToString()}";
                _uiBetArea.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                _uiBetArea.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

                _btnStart.IsEnabled = true;

                // no money
                if (_game.Player.GameMoney == 0)
                {
                    _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    _uiPlayerChips.Fill = new SolidColorBrush();
                    _txtGameMoney.Text = "";

                    // no cash
                    if (_game.Player.Money == 0)
                    {
                        _btnAddGameMoney.IsEnabled = false;
                    }
                    _uiPlayerChip.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Event Handler for bets
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBetAreaClick(object sender, PointerRoutedEventArgs e)
        {
            // bet placed but game did not start
            if (_game.Player.Bet != 0 && _started == false)
            {
                _game.Player.Bet -= 5;
                _txtBet.Text = _game.Player.Bet.ToString();
                
                // no chips
                if (_game.Player.GameMoney == 0)
                {
                    _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    _uiPlayerChips.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    _uiPlayerChip.IsEnabled = true;
                }

                // add 10 to the chip total of the player
                _game.Player.GameMoney += 5;

                //update the controls
                _txtGameMoney.Text = _game.Player.GameMoney.ToString();
                _txtChipCount.Text = $"Chip Total: {_game.Player.GameMoney.ToString()}";

                // bet is 0
                if (_game.Player.Bet == 0)
                {
                    _txtBet.Text = "";
                    _uiBetArea.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    _uiBetArea.Fill = new SolidColorBrush();
                    _txtDisplayOutcome.Text = "Place a bet";
                    _uiFinalTextBorder.Visibility = Visibility.Visible;
                    _btnStart.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Event Handler for start button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            StartRound();
        }

        /// <summary>
        /// Event Handler for hit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHitClick(object sender, RoutedEventArgs e)
        {
            if (_game.Hit() == true)
            {
                // double not allowed
                _btnDouble.IsEnabled = false;

                // draw player hand
                DrawCards(_canvasPlayerHand, _game.Player.Hand, Alignment.Center);

                // player has a soft-hand value
                if (_game.Player.SoftHandValue != 0)
                {
                    // show both score options
                    _txtPlayerTotal.Text = $"{_game.Player.Score.ToString()}/{_game.Player.SoftHandValue.ToString()}";
                }

                // no soft-hand value
                else
                {
                    _txtPlayerTotal.Text = _game.Player.Score.ToString();
                }

                // score of player == 21
                if (_game.Player.Score == 21)
                {
                    // disable buttons
                    _btnHold.IsEnabled = false;
                    _btnHit.IsEnabled = false;

                    // start dealer card timer
                    _DealerTimer.Start();
                }

                // player score > 21
                if (_game.Player.Score > 21)
                {
                    EndRound("Player busted!, Dealer wins!");
                }
            }
        }

        /// <summary>
        /// Event Handler for hold button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHoldClick(object sender, RoutedEventArgs e)
        {
            // start dealer timer
            _DealerTimer.Start();

            // disable buttons
            _btnHold.IsEnabled = false;
            _btnHit.IsEnabled = false;
            _btnDouble.IsEnabled = false;

            if (_game.Player.SoftHandValue > _game.Player.Score)
            {
                _txtPlayerTotal.Text = _game.Player.SoftHandValue.ToString();
            }

        }

        /// <summary>
        /// Event Handler for dealer card timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDealerCardTimerTick(object sender, object e)
        {
            // first tick
            if (_firstTick == true)
            {
                // cards face up
                _game.Dealer.TurnCardsFaceUp();
                DrawCards(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);

                // show dealer's score
                _uiDealerScoreBorder.Visibility = Visibility.Visible;

                // dealer has a soft-hand value
                if (_game.Dealer.SoftHandValue != 0)
                {
                    // show score and soft-hand value
                    _txtDealerTotal.Text = $"{_game.Dealer.Score.ToString()}/{_game.Dealer.SoftHandValue.ToString()}";
                }

                // dealer does not have soft-hand value
                else
                {
                    // show the score of the player
                    _txtDealerTotal.Text = _game.Dealer.Score.ToString();
                }

                // set first tick to false
                _firstTick = false;


            }

            // not first tick
            else
            {
                // dealer soft-hand value is 17 or above
                if (_game.Dealer.SoftHandValue >= 17)
                {
                    // dealer score == soft-hand value
                    _game.Dealer.Score = _game.Dealer.SoftHandValue;
                    _txtDealerTotal.Text = _game.Dealer.Score.ToString();
                }

                // dealer score is between 17 and 21
                if (_game.Dealer.Score >= 17 && _game.Dealer.Score <= 21)
                {
                    // end round 
                    EndRound(_game.DetermineWinner());
                }

                // dealer's score < 17
                else
                {
                    if (_game.Hold() == true)
                    {
                        // dealer has a soft-hand value
                        if (_game.Dealer.SoftHandValue != 0)
                        {
                            // show the score and soft-hand score of the dealer
                            _txtDealerTotal.Text = $"{_game.Dealer.Score.ToString()}/{_game.Dealer.SoftHandValue.ToString()}";
                        }

                        // no soft-hand score
                        else
                        {
                            // show score
                            _txtDealerTotal.Text = _game.Dealer.Score.ToString();
                        }
                        
                        // draw the cards
                        DrawCards(_canvasDealerHand, _game.Dealer.Hand, Alignment.Center);

                        // dealer score > 21
                        if (_game.Dealer.Score > 21)
                        {
                            // busted
                            EndRound("Dealer busted!, Player wins!");
                        }
                    }

                    // dealer score > 17
                    else
                    {
                        //stop card timer
                        _DealerTimer.Stop();

                        // score <= 17
                        if (_game.Dealer.Score <= 21)
                        {
                            //end the round
                            EndRound(_game.DetermineWinner());
                        }
                    }
                }
            }



        }

        /// <summary>
        /// Event Handler for double button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnDoubleClick(object sender, RoutedEventArgs e)
        {
            // check funds
            if (_game.Double() == true)
            {
                // double
                _txtBet.Text = _game.Player.Bet.ToString();
                if (_game.Player.GameMoney != 0)
                {
                    _txtGameMoney.Text = _game.Player.GameMoney.ToString();
                }

                // cant double
                else
                {
                    _txtGameMoney.Text = "";
                    _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    _uiPlayerChips.Fill = new SolidColorBrush();
                    _uiPlayerChip.IsEnabled = false;

                }

                // update chip total
                _txtChipCount.Text = $"Chip Total: {_game.Player.GameMoney.ToString()}";

                // disable buttons
                _btnHold.IsEnabled = false;
                _btnHit.IsEnabled = false;
                _btnDouble.IsEnabled = false;

                // draw cards
                DrawCards(_canvasPlayerHand, _game.Player.Hand, Alignment.Center);

                // soft-hand != 0
                if (_game.Player.SoftHandValue != 0)
                {
                    _txtPlayerTotal.Text = $"{_game.Player.Score.ToString()}/{_game.Player.SoftHandValue.ToString()}";
                    _game.Player.Score = _game.Player.SoftHandValue;
                }

                // no soft-hand value
                else
                {
                    _txtPlayerTotal.Text = _game.Player.Score.ToString();
                }

                //delay
                await Task.Delay(TimeSpan.FromMilliseconds(500));

                // player's score > 21
                if (_game.Player.Score > 21)
                {
                    // end the round
                    EndRound("Player busted!, Dealer Wins!");
                }

                // player score !> 21
                else
                {
                    // start dealer timer
                    _DealerTimer.Start();
                }
                _txtPlayerTotal.Text = _game.Player.Score.ToString();
            }
        }

        /// <summary>
        /// Event Handler for edit user button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditUserClick(object sender, RoutedEventArgs e)
        {
            _uiEditProfilePane.IsPaneOpen = true;
            _uiEditProfilePane.DisplayMode = SplitViewDisplayMode.Overlay;
        }

        /// <summary>
        /// Event Handler for add funds button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddFundsClick(object sender, RoutedEventArgs e)
        {
            _game.Player.Money += 5;

            // update cash text
            _txtTotalMoney.Text = $"${(_game.Player.Money).ToString()}";

            // button disabled?
            if (_btnAddGameMoney.IsEnabled == false)
            {
                // enable it
                _btnAddGameMoney.IsEnabled = true;
            }
        }

        /// <summary>
        /// Event Handler for add chips button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddChips(object sender, RoutedEventArgs e)
        {
            if (_txtDisplayOutcome.Text == "Buy chips from the user menu")
            {
                _txtDisplayOutcome.Text = "Please place a bet";
            }

            // no chips
            if (_game.Player.GameMoney == 0)
            {
                _uiPlayerChips.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                _uiPlayerChips.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                _uiPlayerChip.IsEnabled = true;
            }

            // add 5 to the chip stack
            _game.Player.GameMoney += 5;

            // subtract 5 from the player's cash total
            _game.Player.Money -= 5;

            // update the cash total
            _txtTotalMoney.Text = $"${(_game.Player.Money).ToString()}";

            // update the chip count
            _txtChipCount.Text = $"Chip Total: {_game.Player.GameMoney.ToString()}";
            _txtGameMoney.Text = _game.Player.GameMoney.ToString();

            // no money left
            if (_game.Player.Money == 0)
            {
                _btnAddGameMoney.IsEnabled = false;

            }

        }

        /// <summary>
        /// Event handler for done click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDoneClick(object sender, RoutedEventArgs e)
        {
            _uiEditProfilePane.IsPaneOpen = false;
        }

        /// <summary>
        /// Event handler for name change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNameChanged(object sender, TextChangedEventArgs e)
        {
            _game.Player.Name = _txtNameInput.Text;
        }

        /// <summary>
        /// Event handler for closing the pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPaneClosed(SplitView sender, object args)
        {
            SaveGame();
        }
    }
}
