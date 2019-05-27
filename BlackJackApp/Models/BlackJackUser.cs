using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackApp.Models
{
    /// <summary>
    /// Creates a blackjack player, inherits from user class
    /// </summary>
    class BlackJackUser : User
    {
        // game money
        private int _gameMoney;

        // player score
        private int _score;

        // soft hand value
        private int _softHandValue;

        // ace count of current hand
        private int _aceCount;

        /// <summary>
        /// blackjack user
        /// </summary>
        /// <param name="name"></param>
        /// <param name="money"></param>
        /// <param name="loses"></param>
        /// <param name="wins"></param>
        /// <param name="gameMoney"></param>
        /// <param name="bet"></param>
        public BlackJackUser(string name, int money, int loses, int wins, int gameMoney, int bet) : base(name, money, loses, wins)
        {
            _gameMoney = gameMoney;
            _score = 0;
            _softHandValue = 0;
            _aceCount = 0;
        }

        // game money of player can be modified
        public int GameMoney
        {
            get { return _gameMoney; }
            set { _gameMoney = value; }
        }

        // score of player can be modified
        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }

        // softhand value of player can be modified
        public int SoftHandValue
        {
            get { return _softHandValue; }
            set { _softHandValue = value; }
        }

        // ace count of player can be modified
        public int AceCount
        {
            get { return _aceCount; }
            set { _aceCount = value; }
        }

        /// <summary>
        /// read values from file if game played before
        /// </summary>
        /// <param name="reader"></param>
        public void Load(StreamReader reader)
        {
            //set the values of the field variables to the files contents
            _name = reader.ReadLine();
            _money = int.Parse(reader.ReadLine());
            _gameMoney = int.Parse(reader.ReadLine());
            _numWins = int.Parse(reader.ReadLine());
            _numLoses = int.Parse(reader.ReadLine());
        }

        /// <summary>
        /// Write player data to file
        /// </summary>
        /// <param name="writer"></param>
        public void Save(StreamWriter writer)
        {
            //write the values of the field variables to the file, line by line
            writer.WriteLine(_name);
            writer.WriteLine(_money);
            writer.WriteLine(_gameMoney);
            writer.WriteLine(_numWins);
            writer.WriteLine(_numLoses);
        }
    }
}
