using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJackApp.Models;

namespace BlackJackApp.GameLogic
{
    abstract class CardGame
    {
        protected int _minBet;
        private CardDeck _cardDeck;

        public CardGame(int bet)
        {
            _cardDeck = new CardDeck();
            _minBet = bet;
        }

        public CardDeck CardDeck
        {
            get
            {
                return _cardDeck;
            }

            set
            {
                _cardDeck = value;
            }
        }

        public abstract void Restart();
    }
}
