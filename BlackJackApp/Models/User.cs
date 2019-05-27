using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp.Models
{
    class User : Player
    {
        protected int _money;
        protected int _numWins;
        protected int _numLoses;
        protected int _bet;

        public User(String name, int money, int losses, int wins) : base(name)
        {
            _money = money;
            _numLoses = losses;
            _numWins = wins;
            _bet = 0;
            
        }
        
        public int Money
        {
            get { return _money; }
            set { _money = value; }
        }

        public int Bet
        {
            get { return _bet; }
            set { _bet = value; }
        }
    }
}
