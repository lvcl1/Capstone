using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The__un_seen_future
{
    class Enemy:Stats
    {
        public string Name { get; set; }
        public Enemy(int health, int speed, int attack, int defense, int xp, int gold, string name) : base(health, speed, attack, defense, xp, gold)
        {
            Name = name;
        }
        static Random random = new Random();
        static public Enemy NewEnemy()
        {
            Enemy enemy;
            int newenemy = random.Next(4);
            if (newenemy == 0)
            {
                enemy = new Enemy(5, 5, 5, 5, 5, 5,"goblin");
            }
            else if (newenemy == 1)
            {
                enemy = new Enemy(5, 5, 5, 5, 5, 5, "wolf");
            }
            else if (newenemy == 2)
            {
                enemy = new Enemy(5, 5, 5, 5, 5, 5, "skeleton");
            }
            else
            {
                enemy = new Enemy(5, 5, 5, 5, 5, 5, "slime");
            }
            return enemy;
        }
    }
}
