using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp.Models
{
    /// <summary>
    /// Player class containing a cardlist called hand and the player name
    /// </summary>
    class Player
    {
        // create local variables
        private List<Card> _hand;
        protected String _name;

        // cardlist
        internal List<Card> Hand
        {
            get
            {
                return _hand;
            }

            set
            {
                _hand = value;
            }
        }

        public Player(String name)
        {
            _name = name;
            _hand = new List<Card>();

        }

        // name
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        // turn cards face up
        public void TurnCardsFaceUp()
        {
            foreach(Card card in Hand)
            {
                card.FaceUp = true;
            }
        }
    }
}
