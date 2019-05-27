using BlackJackApp.Models;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace BlackJackApp.Serializer
{
    class BlackJackTextSerializer
    {

        // store directory path
        private string _dirPath;

        // player
        private BlackJackUser _player;

        // store path of file
        private string _filePath;

        /// <summary>
        /// Constructor
        /// </summary>
        public BlackJackTextSerializer()
        {
            //initializes the player to null
            _player = null;

            // set the file path
            _filePath = $"{_dirPath}/playerstats.dat";
            
            //set the directory path
            _dirPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "BlackJackSaves");

            // if the directory doesn't exist, create one
            if (Directory.Exists(_dirPath) == false)
            {
                // create task, this can run in background
                Task create_directory = Task.Factory.StartNew(() => {
                    Directory.CreateDirectory(_dirPath);
                });
            }
        }

        // for acces outside of class
        public BlackJackUser Player
        {
            get { return _player; }
            set { _player = value; }
        }

        // for acces outside of class
        public string FilePath
        {
            get { return _filePath; }
        }

        // for acces outside of class
        public string DirectoryPath
        {
            get { return _dirPath; }
        }

        // load data from file
        public void Load()
        {
            // access the directory path
            string[] directoryPath = Directory.GetFiles(_dirPath);
           
            // create load task
            Task load = Task.Factory.StartNew(() => {
                // loop through files in directory path
                foreach (string file in directoryPath)
                {
                    // read the file
                    using (StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open)))
                    {
                        _player.Load(reader);
                    }
                }

            });
            load.Wait();
        }

        // save data to file
        public void Save()
        {
            //stores the path of the file in a variable
            string filePath = $"{_dirPath}/playerstats.dat";
            // create save task
            Task save = Task.Factory.StartNew(() => {
                //open the file and write to it
                using (StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.Create)))
                {
                    _player.Save(writer);
                }
            });
        }
    }
}
