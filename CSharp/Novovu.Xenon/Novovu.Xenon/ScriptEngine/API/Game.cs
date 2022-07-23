using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine.API
{
    public class Game
    {
        public List<Level> Levels = new List<Level>();

        public Level SelectedLevel;

        public Game(Engine.Game game)
        {
            foreach (Engine.Level lvl in game.Levels)
            {
                Levels.Add(new Level(lvl));
            }
            SelectedLevel = new Level(game.SelectedLevel);
        }

        
    }
}
