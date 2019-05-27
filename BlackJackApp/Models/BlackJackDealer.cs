using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp.Models
{

    /// <summary>
    /// Blackjack Dealer, inherits from the Player class
    /// </summary>
    class BlackJackDealer : Player
    {
        // dealer score
        private int _score;

        // soft hand value
        private int _softHandValue;

        // ace count of current hand
        private int _aceCount;

        /// <summary>
        /// Blackjack dealer
        /// </summary>
        /// <param name="name"></param>
        public BlackJackDealer(string name) : base(name)
        {
            _score = 0;
            _softHandValue = 0;
            _aceCount = 0;
        }

        // score of dealer can be modified
        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        // softhand value of dealer can be modified
        public int SoftHandValue
        {
            get { return _softHandValue; }
            set { _softHandValue = value; }
        }

        // ace count of dealer can be modified
        public int AceCount
        {
            get { return _aceCount; }
            set { _aceCount = value; }
        }
    }
}
