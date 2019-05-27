using System;
using System.Diagnostics;

namespace BlackJackApp.Models
{
    // Enum with the card suits
    public enum CardSuit
    {
        Diamonds,
        Clubs,
        Hearts,
        Spades
    }

    /// <summary>
    /// Card with value and suit
    /// </summary>
    public class Card
    {
        // value of card
        private byte _value;
        // suit of card
        CardSuit _suit;
        // faceup boolean
        Boolean _faceUp;

        /// <summary>
        /// Contructor for card object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="suit"></param>
        public Card(byte value, CardSuit suit)
        {
            _value = value;
            _suit = suit;
            _faceUp = false;
        }

        // Numeric value of card
        public byte Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The card name based on its value. 1 is Ace, 11 is Jack etc.
        /// </summary>
        /// <returns></returns>
        public string CardName
        {
            get
            {
                // switch case for card name based on value
                // 1 = Ace
                // 11 = jack
                // 12 = Queen
                // 13 = King
                string cardName;
                switch (_value)
                {
                    case 1:
                        cardName = "Ace";
                        break;

                    case 11:
                        cardName = "Jack";
                        break;

                    case 12:
                        cardName = "Queen";
                        break;

                    case 13:
                        cardName = "King";
                        break;

                    default:
                        cardName = _value.ToString();
                        break;
                }

                return cardName;
            }
        }

        // Determines name of suit
        public string SuitName
        {
            get
            {
                // switch case to determine the name of the suit
                switch (_suit)
                {
                    case CardSuit.Diamonds:
                    case CardSuit.Clubs:
                    case CardSuit.Hearts:
                    case CardSuit.Spades:
                        return _suit.ToString();

                    default:
                        Debug.Assert(false, "Suit could not be determined.");
                        return "N/A";
                }
            }
        }

        // card face up
        public bool FaceUp
        {
            get
            {
                return _faceUp;
            }

            set
            {
                _faceUp = value;
            }
        }

        // get image filename
        public string GetFileName()
        {
            string value = _value.ToString();
            if(value.Length == 1)
            {
                value = $"0{value}";
            }
            return $"{_suit.ToString().ToLower()[0].ToString()}{value}.png";
        }
    }
}

