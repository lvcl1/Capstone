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
        int enemyhealth, playerhealth;
        Random random = new Random();
        Player player;
        Enemy enemy, demonlord;
        DispatcherTimer timer = new DispatcherTimer();
        public MainPage()
        {
            InitializeComponent();
            player = new Player(5, 5, 5, 5, 0, 0, 1, 3);
            playerhealth = player.Health;
            btnheal.Content = "heal: " + playerhealth + "/" + player.Health;
            demonlord = new Enemy(100, 50, 50, 20, 150, 250, "demon lord");
            timer.Tick += Timer_Tick;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled&&sender==btn)
            {
                timer.Stop();
            }
            else
            {
                text.Text = "" + random.Next(0, 1000);
            }
            //btn.IsEnabled = false;
            //choices.Visibility = Visibility.Visible;
        }

        private void btnchoice1_Click(object sender, RoutedEventArgs e)
        {
            //choices.Visibility = Visibility.Collapsed;
            //btn.IsEnabled = true;
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
                        text.Text = text.Text + "\nwith the demon lord dead you win the game";
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
                        text.Text = text.Text + "\nwith the demon lord dead you win the game";
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
            nav.Visibility = Visibility.Collapsed;
            hud.Visibility = Visibility.Collapsed;
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
            nav.Visibility = Visibility.Collapsed;
            hud.Visibility = Visibility.Collapsed;
            settingsmenu.Visibility = Visibility.Visible;
        }

        private void btnheal_Click(object sender, RoutedEventArgs e)
        {
            playerhealth = player.Health;
            btnheal.Content = "heal: " + playerhealth + "/" + player.Health;
            btntooutside.IsEnabled = true;
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
    }
}
