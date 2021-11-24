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
        public Weakness Weakness { get; set; }
        public string Advice { get; set; }
        public static Enemy demonlord = new Enemy(100, 50, 50, 20, 150, 250, "demon lord", "");
        public Enemy(int health, int speed, int attack, int defense, int xp, int gold, string name, string advice, Weakness weakness = Weakness.NoWeakness) : base(health, speed, attack, defense, xp, gold)
        {
            Name = name;
            Advice = advice;
            Weakness = weakness;
        }
        static private Random random = new Random();
        static private Enemy enemy;
        static public Enemy NewForestEnemy(bool boss = false)
        {
            int newenemy = random.Next(4);
            if (boss) enemy = new Enemy(10, 10, 10, 10, 10, 10, "lion", "", Weakness.ice);
            else
            {
                switch (newenemy)
                {
                    case 0: enemy = new Enemy(4, 2, 3, 1, 5, 5, "goblin", "goblins aren't very strong and is weak to slashing damage", Weakness.slash);
                        break;
                    case 1: enemy = new Enemy(5, 5, 4, 1, 5, 5, "wolf", "wolves are fast and bites hard but use some fire and you'll be fine", Weakness.fire);
                        break;
                    case 2: enemy = new Enemy(3, 2, 2, 5, 5, 5, "slime", "slimes have squishy bodies making it hard to land a hit using ice will make it easier", Weakness.ice);
                        break;
                    default: enemy = new Enemy(2, 2, 3, 3, 5, 5, "skeleton", "skeletons can be found in most places and easily taken out with bashing damage", Weakness.bash);
                        break;
                }
            }
            return enemy;
        }
        static public Enemy NewCryptEnemy(bool boss = false)
        {
            int newenemy = random.Next(4);
            if (boss) enemy = new Enemy(15, 15, 15, 15, 15, 15, "lich", "", Weakness.fire);
            else
            {
                switch (newenemy)
                {
                    case 0: enemy = new Enemy(5, 1, 4, 3, 7, 7, "zombie", "zombies are strong but very slow, if you use fire you they burn easily", Weakness.fire);
                        break;
                    case 1: enemy = new Enemy(5, 2, 4, 5, 7, 7, "skeleton warrior", "skeleton warrior has a sword and heavy armor so it has high defense but slow", Weakness.bash);
                        break;
                    case 2: enemy = new Enemy(5, 5, 4, 2, 7, 7, "skeleton archer", "skeleton archer has a bow and light armor so low defense but fast", Weakness.bash);
                        break;
                    default: enemy = new Enemy(2, 2, 3, 3, 5, 5, "skeleton", "skeletons can be found in most places and easily taken out with bashing damage", Weakness.bash);
                        break;
                }
            }
            return enemy;
        }
        static public Enemy NewSwampEnemy(bool boss = false)
        { 
            int newenemy = random.Next(4);
            if (boss) enemy = new Enemy(20, 20, 20, 20, 20, 20, "witch", "");
            else
            {
                switch (newenemy)
                {
                    case 0: enemy = new Enemy(7, 6, 5, 6, 12, 12, "lizard man", "being cold blooded lizard man are weak to ice be carefull of there fast speed and hard scales", Weakness.ice);
                        break;
                    case 1: enemy = new Enemy(5, 5, 5, 15, 12, 12, "slime", "the slime's liquid like body makes it hard to get a good hit but using ice will make it more solid", Weakness.ice);
                        break;
                    case 2: enemy = new Enemy(5, 13, 5, 6, 12, 12, "Will-O’-Wisp", "a ghost fire ball that is small and fast making it hard to hit use air to blow it out", Weakness.air);
                        break;
                    default: enemy = new Enemy(2, 2, 3, 3, 5, 5, "skeleton", "skeletons can be found in most places and easily taken out with bashing damage", Weakness.bash);
                        break;
                }
            }
            return enemy;
        }
        static public Enemy NewDragondenEnemy(bool boss = false)
        {
            int newenemy = random.Next(4);
            if (boss) enemy = new Enemy(25, 25, 25, 25, 25, 25, "dragon king", "");
            else
            {
                switch (newenemy)
                {
                    case 0:
                        enemy = new Enemy(10, 12, 11, 12, 15, 15, "dragonwit", "use ice", Weakness.ice);
                        break;
                    case 1:
                        enemy = new Enemy(10, 12, 11, 12, 15, 15, "dragonwit", "use ice", Weakness.ice);
                        break;
                    case 2:
                        enemy = new Enemy(10, 12, 11, 12, 15, 15, "dragonwit", "use ice", Weakness.ice);
                        break;
                    default:
                        enemy = new Enemy(2, 2, 3, 3, 5, 5, "skeleton", "skeletons can be found in most places and easily taken out with bashing damage", Weakness.bash);
                        break;
                }
            }
            return enemy;
        }
    }
    enum Weakness
    {
        NoWeakness, fire, ice, air, earth, bash, slash, stab
    }
}
