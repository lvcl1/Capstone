using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace The__un_seen_future
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int enemyhealth, playerhealth, storytext;
        Random random = new Random();
        Player player;
        Enemy enemy, demonlord;
        DispatcherTimer timer = new DispatcherTimer();
        string[][] Story = new string[((int)Storylocation.epilogue)+1][];
        Storylocation storylocation = Storylocation.prologue;
        public MainPage()
        {
            InitializeComponent();
            //player = new Player(5, 5, 5, 5, 0, 0, 1, 3);
            player = new Player(500, 500, 500, 500, 0, 1000, 1, 3);
            playerhealth = player.Health;
            //btnrun_Click(null, null);
            demonlord = new Enemy(100, 50, 50, 20, 150, 250, "demon lord");
            timer.Tick += Timer_Tick;
            speaker.Text = "";
            storytime();
            storymaker();
            Button_Click(null, null);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string str = Story[(int)storylocation][storytext];
            speaker.Text = "";
            if (str.StartsWith("options{"))
            {
                btnskip.IsEnabled = false;
                btnauto.IsEnabled = false;
                btn.IsEnabled = false;
                menus.Visibility = Visibility.Visible;
                choices.Visibility = Visibility.Visible;
                str = str.Substring(str.IndexOf("{") + 1, str.IndexOf("}") - str.IndexOf("{") - 1);
                var temp = str.Split(',');
                timer.Stop();
                btnchoice1.Content = temp[0];
                btnchoice2.Content = temp[1];
                btnchoice3.Content = temp[2];
            }
            else if(str.StartsWith("event{"))
            {
                str = str.Substring(6, str.Length - 7);
                switch (str)
                {
                    case "getname":
                        //player.getname();
                        storymaker();
                        break;
                    default:
                        break;
                }
                storytext++;
                Button_Click(sender, e);
                storytext--;
            }
            else if (str.StartsWith("speaker{"))
            {
                int temp = str.IndexOf('}');
                speaker.Text = str.Substring(8, temp - 8);
                text.Text = str.Substring(temp + 2);
            }
            else
            {
                text.Text = str;
            }
            storytextcheck();
            //btn.IsEnabled = false;
            //choices.Visibility = Visibility.Visible;
        }
        private void btnchoices_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(((Button)sender).Name[((Button)sender).Name.Length-1].ToString(),out int result);
            storytext += result - 1;
            choices.Visibility = Visibility.Collapsed;
            menus.Visibility = Visibility.Collapsed;
            Button_Click(null, null);
            storytext += 2 - result;
            btnskip.IsEnabled = true;
            btnauto.IsEnabled = true;
            btn.IsEnabled = true;
            storytextcheck();
        }
        private void storytextcheck()
        {
            if (storytext < Story[(int)storylocation].Length)
            {
                storytext++;
                if (storytext == Story[(int)storylocation].Length)
                {
                    menus.Visibility = Visibility.Visible;
                    btnrun_Click(null, null);
                    btn.IsEnabled = false;
                    btnskip.IsEnabled = false;
                    btnauto.IsEnabled = false;
                    storytext = 0;
                }
            }
        }
        private void btntooutside_Click(object sender, RoutedEventArgs e)
        {
            nav.Visibility = Visibility.Collapsed;
            hud.Visibility = Visibility.Collapsed;
            combat.Visibility = Visibility.Visible;
            switch (random.Next(1,5))
            {
                case 1:
                    btnrun_Click(sender, e);
                    text.Text = "you found nothing";
                    break;
                case 2:
                    btnrun_Click(sender, e);
                    int num = random.Next(1, 10);
                    player.Gold += num;
                    text.Text = "you found " + num + " gold";
                    break;
                default:
                    enemy = Enemy.NewEnemy();
                    if (player.Level >= 50)
                    {
                        enemy = demonlord;
                    }
                    enemyhealth = enemy.Health;
                    text.Text = enemy.Name + ": " + enemyhealth + "/" + enemy.Health;
                    break;
            }
        }

        private void btnrun_Click(object sender, RoutedEventArgs e)
        {
            combat.Visibility = Visibility.Collapsed;
            nav.Visibility = Visibility.Visible;
            hud.Visibility = Visibility.Visible;
            text.Text = "";
            btnheal.Content = "heal: " + playerhealth + "/" + player.Health;
        }

        private void btnblock_Click(object sender, RoutedEventArgs e)
        {
            text.Text = text.Text + "\nyou blocked the attack and took no damage";
        }

        private void btnattack_Click(object sender, RoutedEventArgs e)
        {
            int enemydamage = player.Attack - enemy.Defense, playerdamage = enemy.Attack - player.Defense;
            string combataction;
            switch (Speedcheck())
            {
                case 1:
                    combataction = "the enemy got behide you and attacked";
                    playerdamage = (enemy.Attack * 2) - player.Defense;
                    enemydamage = 0;
                    break;
                case 3:
                    combataction = "the enemy was fast to attack but you managed to land a hit as well";
                    playerdamage = (int)((enemy.Attack * 1.5) - player.Defense);
                    break;
                case 2:
                    combataction = "you got behide the enemy and attacked";
                    enemydamage = (player.Attack * 2) - enemy.Defense;
                    playerdamage = 0;
                    break;
                case 4:
                    combataction = "you were fast to attack but the enemy managed to land a hit as well";
                    enemydamage = (int)((player.Attack * 1.5) - enemy.Defense);
                    break;
                default:
                    combataction = "both you and the enemy clashed in your attacks";
                    break;
            }
            enemyhealth -= enemydamage > 0 ? enemydamage : 0;
            playerhealth -= playerdamage > 0 ? playerdamage : 0;
            text.Text = combataction + "\n" + enemy.Name + ": " + enemyhealth + "/" + enemy.Health + "\n" + player.Name + ": " + playerhealth + "/" + player.Health;
            deadcheck();
        }
        private void deadcheck()
        {
            if (playerhealth <=0||enemyhealth<=0)
            {
                string holder = text.Text;
                btnrun_Click(null, null);
                if (enemyhealth <= 0 && playerhealth <= 0)
                {
                    player.XP += enemy.XP / 2;
                    player.Gold += enemy.Gold / 2;
                    text.Text = holder + "\nas you fall over you see the " + enemy.Name + " is dead and you received " + enemy.XP / 2 + " Xp and " + enemy.Gold / 2 + " Gold\n"
                        + player.XP + "/100 Xp " + player.Gold + " Gold";
                    btntooutside.IsEnabled = false;
                    if (enemy.Name.Equals("demon lord"))
                    {
                        text.Text = text.Text + "\nwith the demon lord dead you return to the palace";
                        storylocation = Storylocation.epilogue;
                        storytime();
                    }
                    if (enemy.Name.Equals(questenemy.Name))
                    {
                        questnum--;
                    }

                }
                else if (enemyhealth <= 0)
                {
                    player.XP += enemy.XP;
                    player.Gold += enemy.Gold;
                    text.Text = holder + "\n" + enemy.Name + " is dead and you received " + enemy.XP + " Xp and " + enemy.Gold + " Gold\n"
                        + player.XP + "/100 Xp " + player.Gold + " Gold";
                    if (enemy.Name.Equals("demon lord"))
                    {
                        text.Text = text.Text + "\nwith the demon lord dead you return to the palace";
                        storylocation = Storylocation.epilogue;
                        storytime();
                    }
                    if (enemy.Name.Equals(questenemy.Name))
                    {
                        questnum--;
                    }

                }
                else if (playerhealth <= 0)
                {
                    text.Text = holder + "\nyou blackout and await death. after some time you wake up in town with no idea how you got here";
                    btntooutside.IsEnabled = false;
                }
                Xpcheck();
            }
        }
        private int Speedcheck()
        {
            int playerspeed = random.Next(1, 100) + player.Speed, result, enemyspeed = random.Next(1, 100) + enemy.Speed;
            if (enemyspeed >= playerspeed * 2) result = 1;
            else if (playerspeed >= enemyspeed * 2) result = 2;
            else if (enemyspeed > playerspeed) result = 3;
            else if (playerspeed > enemyspeed) result = 4;
            else result = 0;
            return result;
        }

        private void btnplayer_Click(object sender, RoutedEventArgs e)
        {
            menus.Visibility = Visibility.Collapsed;
            playermenu.Visibility = Visibility.Visible;
            text.Text = "";
            Playername.Text = player.Name;  
            Playerhealth.Text = "Health: " + player.Health;
            Playerspeed.Text = "Speed: " + player.Speed;
            Playerattack.Text = "Attack: " + player.Attack;
            Playerdefense.Text = "Defense: " + player.Defense;
            Playergold.Text = "Gold: " + player.Gold;
            Playerxp.Text = "Xp: " + player.XP + "/100";
            Playerlevel.Text = "Level: " + player.Level;
            Playerpoints.Text = "Points: " + player.Points;
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            btnrun_Click(sender, e);
            menus.Visibility = Visibility.Visible;
            playermenu.Visibility = Visibility.Collapsed;
            settingsmenu.Visibility = Visibility.Collapsed;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (player.Points>0)
            {
                switch (((Button)sender).Name)
                {
                    case "addhealth":
                        player.Health += 5;
                        playerhealth += 5;
                        break;
                    case "addspeed":
                        player.Speed++;
                        break;
                    case "addattack":
                        player.Attack++;
                        break;
                    case "adddefense":
                        player.Defense++;
                        break;
                    default:
                        break;
                }
                player.Points--;
            }
            btnplayer_Click(sender, e);
        }
        public int autospeed=5;
        private void btnauto_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(autospeed);
            timer.Start();
        }
        private void btnskip_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timer.Interval = TimeSpan.FromSeconds(.25);
            timer.Start();
        }
        private void Timer_Tick(object sender, object e)
        {
            Button_Click(sender, null);
            if (menus.Visibility==Visibility.Visible)
            {
                timer.Stop();
            }
        }

        private void btnsettings_Click(object sender, RoutedEventArgs e)
        {
            menus.Visibility = Visibility.Collapsed;
            settingsmenu.Visibility = Visibility.Visible;
        }

        private void btnheal_Click(object sender, RoutedEventArgs e)
        {
            playerhealth = player.Health;
            btnheal.Content = "heal: " + playerhealth + "/" + player.Health;
            btntooutside.IsEnabled = true;
        }

        private void btnadventurersguild_Click(object sender, RoutedEventArgs e)
        {
            nav.Visibility = Visibility.Collapsed;
            adventurersguild.Visibility = Visibility.Visible;
            btnsubmitquest.Visibility = questnum == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnshop_Click(object sender, RoutedEventArgs e)
        {
            nav.Visibility = Visibility.Collapsed;
            shop.Visibility = Visibility.Visible;
        }

        private void btnupgrade_Click(object sender, RoutedEventArgs e)
        {
            if (player.Gold >= 100)
            {
                switch (((Button)sender).Name)
                {
                    case "btnupgradeweapon":
                        player.Attack++;
                        text.Text = "you upgraded your weapon by 1 point\nweapon is now " + player.Attack;
                        break;
                    case "btnupgradearmor":
                        player.Defense++;
                        text.Text = "you upgraded your armor by 1 point\narmor is now " + player.Defense;
                        break;
                    default:
                        break;
                }
                player.Gold -= 100;
            }
        }
        Enemy questenemy = new Enemy(1, 1, 1, 1, 1, 1, "default");
        int questnum = -1, questreward;
        private void btngetquest_Click(object sender, RoutedEventArgs e)
        {
            btngetquest.Visibility = Visibility.Collapsed;
            btnviewquest.Visibility = Visibility.Visible;
            questenemy = Enemy.NewEnemy();
            questnum = random.Next(2, 6);
            questreward = questnum * 5;
            btnviewquest_Click(sender, e);
        }

        private void btnviewquest_Click(object sender, RoutedEventArgs e)
        {
            text.Text = "defeat " + questnum + " " + questenemy.Name + "s for " + questreward + "  gold";
        }

        private void btnsubmitquest_Click(object sender, RoutedEventArgs e)
        {
            btngetquest.Visibility = Visibility.Visible;
            btnviewquest.Visibility = Visibility.Collapsed;
            btnsubmitquest.Visibility = Visibility.Collapsed;
            player.Gold += questreward;
            questnum = -1;
            questenemy.Name = "default";
        }

        private void btntownback_Click(object sender, RoutedEventArgs e)
        {
            btnrun_Click(sender, e);
            shop.Visibility = Visibility.Collapsed;
            adventurersguild.Visibility = Visibility.Collapsed;
        }

        private void Xpcheck()
        {
            if (player.XP>=100)
            {
                player.Level++;
                player.Points += 3;
                player.XP -= 100;
                text.Text = text.Text + "\nLEVEL UP!";
                Xpcheck();
            }
        }
        private void storytime()
        {
            btn.IsEnabled = true;
            btnskip.IsEnabled = true;
            btnauto.IsEnabled = true;
            combat.Visibility = Visibility.Collapsed;
            menus.Visibility = Visibility.Collapsed;

        }
        /// <summary>
        /// use options{option1,options2...} for player options +numofoptions
        /// use event{event} for an event +1
        /// speaker{name of speaker} (what they're saying) +0
        /// when using any of these special cases make sure to have enough space behide it
        /// </summary>
        public void storymaker()
        {
            //TODO I NEED NAMES
            //names
            string kingdom = "kingdom", princesses = "princesses", king = "king";
            Story[(int)Storylocation.prologue] = new string[] { "any similarities to real life people or events are purely coincidence\nwelcome to The (un)seen future", "event{getname}",
                "speaker{" + player.Name + " thoughts} everyone has something that makes them special or different from the norm, it might not be completely unique but it's something you can take pride in.",
                "speaker{" + player.Name + " thoughts} often we are unable to see our own abilities always conpering ourselves to others, \"they can do it better\" or \"they can do it faster\", while not seeing that there are things you do better or faster.",
                "speaker{" + player.Name + " thoughts} you might have increased strength, vision, hearing, or an increase in some other aspest. or perhaps your able to do something others can't like having photographic memory or how I can see the future.",
                "speaker{" + player.Name + " thoughts} most would think that this ability is amazing how ever there's a catch, it only happens while I'm asleep in my dreams, until it happens i can't remeber the dream, and i have no control over what I see or when I see it",
                "speaker{" + player.Name + " thoughts} this might seem far-fetched, how do i know that i can see the future if I can't rember it, maybe it's just a bad case of deja vu?",
                "speaker{" + player.Name + " thoughts} but dreams and deja vu work with memories and past experiences and doesn't just make up random things, so bing able to see entire scenes with unknown things in unknown places play out then have the exact same scenes play out what else would this be",
                "speaker{" + player.Name + " thoughts} be it nothing more then dreams or precognition a cupule nights ago I had a strange dream that was something that was stright out of a fantasy story yet it felt so real",
                "speaker{" + player.Name + " thoughts} but it was not the whole story, it kept jumping to random scenes, one second I was being called a hero by an unknown shadowy figure, the next fighting a demon, after that having a knife stabbed into my back surrounded by noble looking people",
                "speaker{" + player.Name + " thoughts} I woke up in a cold sweat but had forgotten the dream entirely as if wiped from my mind, but now it's back as if it was never gone becuse the first part is happen a shadowy figure is calling me a hero.",
                "speaker{shadowy figure} THE SUMMONING WORKED! oh hero please save us from the demon lord",
                "options{who are you,where is this,whats going on}",
                "I am the princesses of the kingdom "+kingdom+", "+princesses,
                "the kingdom of "+kingdom+" my name is "+princesses,
                "you have been summoned to this world my name is "+princesses };
            Story[(int)Storylocation.epilogue] = new string[] { "after returning to the palace you receive your reward\nwhile gitting your reward you feel a knife going in to your back stright to the heart","you have been betrayed by those you worked to save",
                "speaker{" + king + "} he didn't see it coming hahaha did he think we would keep him alive when he has the power to defeat the demon lord?",
                "speaker{" + player.Name + " thoughts} so they feared my power and decided to get rid of me before anything bad would happen, what could i have done to cahanged this outcome"};
        }
    }
    public enum Storylocation
    {
        prologue, epilogue
    }
}
