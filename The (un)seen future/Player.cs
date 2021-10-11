using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The__un_seen_future
{
    class Player : Stats
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Points { get; set; }
        public Player(int health, int speed, int attack, int defense, int xp, int gold, int level, int point) : base(health, speed, attack, defense, xp, gold)
        {
            getname();
            Level = level;
            Points = point;
        }
        private async void getname()
        {
            Name name = new Name();
            do
            {
                await name.ShowAsync();
                if (!(name.name+"").Trim().Equals(""))
                {
                    Name = name.name.Trim();
                }
            } while (Name == null);
        }
    }
}
