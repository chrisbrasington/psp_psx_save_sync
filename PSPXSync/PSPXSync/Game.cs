using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSPXSync
{
    public class Game : IComparable
    {
        //public static List<GameID> GameIDs = new List<GameID>();
        public static Dictionary<string, Game> GameIDs = new Dictionary<string, Game>();

        public string Name;
        public string ID;

        public MemCard MemCard;

        public Game(string name, string id, string path)
        {
            this.Name = name;
            this.ID = id;
            MemCard = new MemCard(path);
        }
        public override string ToString()
        {
            if (this.Name.EndsWith("mcr"))
            {
                return $"{this.Name} [{this.MemCard.Modified}]" ;
            }

            if(this.Name == this.ID || this.ID == "")
            {
                return $"{this.Name}";
            }
            return $"{this.Name} ({this.ID}) [{this.MemCard.Modified}]";
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            return this.Name.CompareTo((obj as Game).Name);
        }

        public static void Load()
        {
            List<Game>  tempGameIDs = 
                Settings.ReadFromJsonFile<List<Game>>("ids.json");

            foreach(Game g in tempGameIDs)
            {
                GameIDs[g.ID] = g;
            }
        }
    }
}
