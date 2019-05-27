using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp.Models
{
    /// <summary>
    /// Contains list of cards.
    /// </summary>
    class CardDeck
    {
        // create protected cardlist
        protected List<Card> _cardList;

        // random to get cards from deck
        private static Random rnd;

        protected const int MAX_SUIT_COUNT = 4;
        protected const int MAX_CARD_VALUE = 13;

        public Random Randomizer
        {
            get { return rnd; }
        }

        public List<Card> CardList
        {
            get { return _cardList; }
            set { _cardList = value; }
        }

        // Constructor for CardDeck
        public CardDeck()
        {
            _cardList = new List<Card>();
            // spawn a card deck
            SpawnDeck();
            // shuffle the card deck
            ShuffleDeck();
        }

        static CardDeck()
        {
            rnd = new Random();
        }

        // Cards left in deck
        public int CardCount
        {
            get { return _cardList.Count; }
        }

        // Create full deck of cards
        protected virtual void SpawnDeck()
        {
            // loop through suits
            for (byte value = 1; value <= MAX_CARD_VALUE; value++)
            {
                // loop through cards
                for (int i = 1; i <= MAX_SUIT_COUNT; i++)
                {
                    //obtain the suit for the current index
                    CardSuit suit = (CardSuit)i;

                    //create the card
                    Card card = new Card(value, suit);

                    //add the card to the list of the deck
                    _cardList.Add(card);
                }
            }
        }

        // Fisher-Yates shuffle for cards shuffle
        public void ShuffleDeck()
        {
            for (int cardIndex = 0; cardIndex < CardCount; cardIndex++)
            {
                int swapIndex = Randomizer.Next(cardIndex, CardCount);
                Card card = _cardList[cardIndex];
                _cardList[cardIndex] = _cardList[swapIndex];
                _cardList[swapIndex] = card;
            }
        }
        
        // draws a card
        public Card DrawCard(Boolean faceUp)
        {
            Card card = _cardList[0];
            _cardList.Remove(card);
            if (faceUp)
                card.FaceUp = true;
            return card;
        }


        public Card[] DrawCards(int amount, Boolean faceup)
        {
            Card[] cards = _cardList.Take(amount).ToArray();
            _cardList.RemoveRange(0, amount);
            if (faceup)
            {
                foreach (Card card in cards)
                {
                    card.FaceUp = true;
                }
            }
            return cards;
        }
    }
}
