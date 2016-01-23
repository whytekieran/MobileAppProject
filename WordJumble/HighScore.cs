using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordJumble
{
    class HighScore
    {
        //Class which represents HighScore elements in the high score tables of the game database
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public int Score { get; set; }
        public string Name { get; set; }
    }
}
