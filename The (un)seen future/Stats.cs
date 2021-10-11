using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The__un_seen_future
{
    class Stats
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int XP { get; set; }
        public int Gold { get; set; }
        public Stats(int health, int speed, int attack, int defense, int xp, int gold)
        {
            Health = health;
            Speed = speed;
            Attack = attack;
            Defense = defense;
            XP = xp;
            Gold = gold;
        }
    }
}
