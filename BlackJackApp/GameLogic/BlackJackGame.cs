using BlackJackApp.Serializer;
using BlackJackApp.Models;
using System.Threading.Tasks;

namespace BlackJackApp.GameLogic
{
    class BlackJackGame : CardGame
    {
        // player
        private BlackJackUser _player;

        // dealer
        private BlackJackDealer _dealer;

        /// <summary>
        /// Constructor
        /// </summary>
        public BlackJackGame() : base(10)
        {
            //default player and dealer
            _player = new BlackJackUser("", 0, 0, 0, 0, 0);
            _dealer = new BlackJackDealer("Roel de Blackjack Dealer");
        }

        // acces player from outside class
        public BlackJackUser Player
        {
            get { return _player; }
            set { _player = value; }
        }

        // acces dealer from outside class
        public BlackJackDealer Dealer
        {
            get { return _dealer; }
            set { _dealer = value; }
        }

        /// <summary>
        /// Method used to set the card value
        /// </summary>
        /// <param name="card"></param>
        /// <returns>value of the card</returns>
        private int SetCardValue(Card card)
        {
            // create task with int as return 
            var set_card_value = Task<int>.Factory.StartNew(() => {
                //initialize to 0
                int cardValue = 0;
                // if the card is a jack, queen, or king
                if (card.CardName == "Jack" || card.CardName == "Queen" || card.CardName == "King")
                {
                    //set the value to 10
                    cardValue = 10;
                }
                // else, default value
                else
                {
                    cardValue = card.Value;
                }
                //return the value
                return cardValue;
            });
            return set_card_value.Result;
        }

        /// <summary>
        /// Method used to determine if there is an ace in the initial hands of the dealer/player
        /// </summary>
        /// <param name="card1">the first card</param>
        /// <param name="card2">the second card</param>
        /// <returns>string value stating if there is an ace, and if there is it states which card in the hand has it</returns>
        private string InitialHandAceCheck(Card card1, Card card2)
        {
            // create task with string as return 
            var ace_check = Task<string>.Factory.StartNew(() => {
                // if only the first card is an ace
                if (SetCardValue(card1) == 1 && SetCardValue(card2) != 1)
                {
                    return "first";
                }

                // if only the second card is an ace
                else if (SetCardValue(card1) != 1 && SetCardValue(card2) == 1)
                {
                    return "second";
                }

                //if both cards are aces
                else if (SetCardValue(card1) == 1 && SetCardValue(card2) == 1)
                {
                    return "both";
                }

                //if there is no ace in the hand
                return "neither";
            });
            return ace_check.Result;
        }

        /// <summary>
        /// Method used to perform actions based on the outcome of the 'InitialHandAceCheck' method
        /// </summary>
        private void DetermineInitialAce()
        {
            // create task for player check
            Task player_ace_check = Task.Factory.StartNew(() => {
                //if the player has an ace as their first card only
                if (InitialHandAceCheck(_player.Hand[0], _player.Hand[1]) == "first")
                {
                    // add the soft ace value (11) to the value of the second card to get the soft-hand value
                    _player.SoftHandValue = SetCardValue(_player.Hand[1]) + 11;
                    _player.AceCount++;
                }

                //if the player has an ace as their second card only
                else if (InitialHandAceCheck(_player.Hand[0], _player.Hand[1]) == "second")
                {
                    //add the soft ace value (11) to the value of the first card to get the soft-hand value
                    _player.SoftHandValue = SetCardValue(_player.Hand[0]) + 11;
                    _player.AceCount++;
                }

                //if the player has two aces
                else if (InitialHandAceCheck(_player.Hand[0], _player.Hand[1]) == "both")
                {
                    //treat one of the aces as a soft ace, the other as a hard ace
                    _player.SoftHandValue = 1 + 11;
                    _player.AceCount++;
                }

                //if the player has no aces
                else
                {
                    //they have no soft-hand value
                    _player.SoftHandValue = 0;
                }
            });

            // create task for dealer check
            Task dealer_ace_check = Task.Factory.StartNew(() => {
                //if the dealer has an ace as their first card only
                if (InitialHandAceCheck(_dealer.Hand[0], _dealer.Hand[1]) == "first")
                {
                    //add the soft ace value (11) to the value of their second card to get the soft-hand value
                    _dealer.SoftHandValue = SetCardValue(_dealer.Hand[1]) + 11;
                    _dealer.AceCount++;
                }

                //if the dealer has an ace as their second card only
                else if (InitialHandAceCheck(_dealer.Hand[0], _dealer.Hand[1]) == "second")
                {
                    //add the soft ace value (11) to the value of the first card to get the soft-hand value
                    _dealer.SoftHandValue = SetCardValue(_dealer.Hand[0]) + 11;
                    _dealer.AceCount++;
                }

                //if the dealer has two aces
                else if (InitialHandAceCheck(_dealer.Hand[0], _dealer.Hand[1]) == "both")
                {
                    //treat one of the aces as a soft ace, the other as a hard ace
                    _dealer.SoftHandValue = 1 + 11;
                    _dealer.AceCount++;
                }

                //if the dealer has no aces
                else
                {
                    _dealer.SoftHandValue = 0;
                }
            });

            // await tasks
            player_ace_check.Wait();
            dealer_ace_check.Wait();
        }

        /// <summary>
        /// Method used to determine if the dealer or player have blackjack
        /// </summary>
        /// <returns>string with result</returns>
        public string DetermineBlackJack()
        {
            // player has blackjack
            if (_player.SoftHandValue == 21 && _player.Hand.Count == 2)
            {
                // set the player score to 21
                _player.Score = 21;

                // dealer also has blackjack
                if (_dealer.SoftHandValue == 21 && _dealer.Hand.Count == 2)
                {
                    // set the dealer score to 21
                    _dealer.Score = 21;

                    // both the player and dealer got blackjack
                    return "draw";
                }

                // only the player got blackjack
                return "player";
            }

            //if the dealer has blackjack
            else if ((_dealer.SoftHandValue == 21 && _dealer.Hand.Count == 2) && _player.SoftHandValue != 21)
            {
                // set the dealer score to 21
                _dealer.Score = 21;

                // only the dealer has blackjack
                return "dealer";
            }

            //neither the player or dealer got blackjack
            return "none";
        }

        /// <summary>
        /// Method used to set the initial hands
        /// </summary>
        private void InitialHands()
        {
            // PLAYER //
            Task draw_player = Task.Factory.StartNew(() => {
                // draw 2 cards
                _player.Hand.AddRange(CardDeck.DrawCards(2, true));

                // loop through the player's hand
                for (int i = 0; i < _player.Hand.Count; i++)
                {
                    // add the value of the cards to the player's score
                    _player.Score += SetCardValue(_player.Hand[i]);
                }
            });

            // DEALER //
            Task draw_dealer = Task.Factory.StartNew(() => {
                // draw 2 cards for dealer, 1 faced down
                _dealer.Hand.Add(CardDeck.DrawCard(true));
                _dealer.Hand.Add(CardDeck.DrawCard(false));

                // loop through the dealer's hand
                for (int i = 0; i < _dealer.Hand.Count; i++)
                {
                    // add the value of the cards to the dealer's score
                    _dealer.Score += SetCardValue(_dealer.Hand[i]);
                }
            });

            // wait for draw tasks to finish
            draw_player.Wait();
            draw_dealer.Wait();

            // Determine if ace or blackjack
            DetermineInitialAce();
            DetermineBlackJack();
        }

        /// <summary>
        /// Method used to add player cards
        /// </summary>
        private void AddPlayerCards()
        {
            // draw card
            _player.Hand.Add(CardDeck.DrawCard(true));
            Card card = _player.Hand[_player.Hand.Count - 1];

            // card is an ace
            if (SetCardValue(card) == 1)
            {
                // if an ace is already being used as an 11
                if (_player.AceCount != 0 && _player.SoftHandValue != 0)
                {
                    // use 1 as value
                    _player.SoftHandValue += 1;
                }

                // first ace
                else
                {
                    _player.AceCount++;
                    if (_player.Score <= 10)
                    {
                        _player.SoftHandValue = _player.Score + 11;
                    }
                    else
                    {
                        _player.SoftHandValue = 0;
                    }
                }
            }

            // card is not ace
            else
            {
                // player already has a soft-hand value 
                if (_player.AceCount != 0 && _player.SoftHandValue != 0)
                {
                    _player.SoftHandValue += SetCardValue(card);

                    //if the soft score of the player is over 21
                    if (_player.SoftHandValue > 21)
                    {
                        _player.SoftHandValue = 0;
                    }
                }
            }
            // add the value of the card to the player's score
            _player.Score += SetCardValue(card);
        }

        /// <summary>
        /// Method used to add dealer cards
        /// </summary>
        private void AddDealerCards()
        {
            // draw a card
            _dealer.Hand.Add(CardDeck.DrawCard(true));
            Card card = _dealer.Hand[_dealer.Hand.Count - 1];
            
            // card drawn is an ace
            if (SetCardValue(card) == 1)
            {
                //if the dealer already has a soft-hand value
                if (_dealer.AceCount != 0 && _dealer.SoftHandValue != 0)
                {
                    // use 1 as value
                    _dealer.SoftHandValue += 1;
                }

                // dealer does not have an ace in hand
                else
                {
                    _dealer.AceCount++;

                    //if the dealer score is less than or equal to 10
                    if (_dealer.Score <= 10)
                    {
                        _dealer.SoftHandValue = _dealer.Score + 11;
                    }
                    else
                    {
                        _dealer.SoftHandValue = 0;
                    }
                }
            }

            // dealer did not draw ace
            else
            {
                // dealer has a soft-hand value
                if (_dealer.AceCount != 0 && _dealer.SoftHandValue != 0)
                {
                    _dealer.SoftHandValue += SetCardValue(card);
                    if (_dealer.SoftHandValue > 21)
                    {
                        _dealer.SoftHandValue = 0;
                    }
                }
               
            }

            // add the value of the card to the score of the dealer
            _dealer.Score += SetCardValue(card);
        }

        /// <summary>
        /// Method for starting round
        /// </summary>
        public void StartRound()
        {
            Restart();
            InitialHands();
        }
        
        /// <summary>
        /// Method for restarting 
        /// </summary>
        public override void Restart()
        {
            // create a new card deck and shuffle the cards
            CardDeck = new CardDeck();
            CardDeck.ShuffleDeck();

            // reset player 
            _player.Hand.Clear();
            _player.Score = 0;
            _player.SoftHandValue = 0;
            _player.AceCount = 0;

            //reset dealer
            _dealer.Hand.Clear();
            _dealer.Score = 0;
            _dealer.SoftHandValue = 0;
            _dealer.AceCount = 0;
        }

        /// <summary>
        /// Method that determiness the winner
        /// </summary>
        /// <returns>winner in string</returns>
        public string DetermineWinner()
        {
            // soft-hand value of the player is less than or equal to 21
            if (_player.SoftHandValue > _player.Score && _player.SoftHandValue <= 21)
            {
                _player.Score = _player.SoftHandValue;
            }

            // soft-hand value of the dealer is less than or equal to 21
            if (_dealer.SoftHandValue > _dealer.Score && _dealer.SoftHandValue <= 21)
            {
                _dealer.Score = _dealer.SoftHandValue;
            }

            // player score is greater than the dealer's score or the dealer busted
            if (_player.Score > _dealer.Score || _dealer.Score > 21)
            {
                if (_player.Score > 21)
                {
                    return "Dealer wins!";
                }
                return "Player Wins!";
            }

            // dealer score is greater than the player's score or the player busted
            else if (_dealer.Score > _player.Score || _player.Score > 21)
            {
                if (Dealer.Score > 21)
                {
                    return "Player Wins!";
                }
                return "Dealer Wins!";
            }

            // draw
            return "Its a draw!";
        }

        /// <summary>
        /// Method used to determine the payout 
        /// </summary>
        /// <returns>int payout</returns>
        public int Payout()
        {
            if (DetermineWinner() == "Player Wins!")
            {
                if (DetermineBlackJack() == "player")
                {
                     return _player.GameMoney += _player.Bet * 3;
                }
                return _player.GameMoney += _player.Bet * 2;
            }
            
            else if (DetermineWinner() == "Dealer Wins!")
            {
                return _player.GameMoney;
            }
            
            // draw
            else
            {
                return _player.GameMoney += _player.Bet;
            }
        }

        /// <summary>
        /// Method for hold
        /// </summary>
        /// <returns>boolean</returns>
        public bool Hold()
        {
            if (_player.SoftHandValue > _player.Score)
            {
                _player.Score = _player.SoftHandValue;
            }            
            
            // hit if score below 17
            if (_dealer.Score < 17)
            {
                AddDealerCards();
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Method for hits
        /// </summary>
        /// <returns>boolean</returns>
        public bool Hit()
        {
            if (_player.Score <= 21)
            {
                AddPlayerCards();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method for double
        /// </summary>
        /// <returns>boolean</returns>
        public bool Double()
        {
            if (Hit() == true && _player.GameMoney > 0)
            {
                _player.GameMoney -= _player.Bet;
                _player.Bet *= 2;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method for saving
        /// </summary>
        /// <param name="serializer"></param>
        public void Save(BlackJackTextSerializer serializer)
        {
            serializer.Player = _player;
            serializer.Save();
        }

        /// <summary>
        /// Method for loading
        /// </summary>
        /// <param name="_serializer"></param>
        public void Load(BlackJackTextSerializer _serializer)
        {
            _serializer.Player = _player;
            _serializer.Load();
            _player = _serializer.Player;
        }
    }
}
