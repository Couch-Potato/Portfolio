using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    public class Game
    {
        public List<Level> Levels = new List<Level>();

        public Level SelectedLevel = new Level();

        public void Render()
        {
            SelectedLevel.Render();
        }
        public void Update(GameTime timeSpan)
        {
            SelectedLevel.Update(timeSpan);
        }
      

    }
    
}
