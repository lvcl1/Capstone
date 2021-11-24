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
        public Weapon Weapontype { get; set; }
        public Weapon Weaponenchant { get; set; }
        public Player(int health, int speed, int attack, int defense, int xp, int gold, int level, int point, Weapon weapontype = Weapon.Nothing, Weapon weaponenchant = Weapon.Nothing, string name = "") : base(health, speed, attack, defense, xp, gold)
        {
            if (name.Equals("")) Getname();
            else Name = name; 
            Level = level;
            Points = point;
            Weapontype = weapontype;
            Weaponenchant = weaponenchant;
        }
        private async void Getname()
        {
            if (Name==null)
            {
                Name name = new Name();
                do
                {
                    await name.ShowAsync();
                    if (!(name.name + "").Trim().Equals(""))
                    {
                        Name = name.name.Trim();
                    }
                } while (Name == null);
                MainPage.storymaker(Name);
            }
        }

        public override string ToString()
        {
            return Health + "," + Speed + "," + Attack + "," + Defense + "," + XP + "," + Gold + "," + Level + "," + Points + "," + (int)Weapontype + "," + (int)Weaponenchant + "," + Name;
        }
    }
    enum Weapon
    {
        Nothing, fire, ice, air, earth, bash, slash, stab

    }
}
