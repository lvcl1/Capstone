using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace The__un_seen_future
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int enemyhealth, playerhealth, storytext, timesrejected, bossesdead;
        private bool Forestboss = true, Cryptboss = true, Swampboss = true, Dragondenboss = true, demonlord = true;//still alive
        Random random = new Random();
        Player player = null;
        Enemy enemy;
        DispatcherTimer timer = new DispatcherTimer();
        private static string[][] Story = new string[((int)Storylocation.KilledEpilogue)+1][];
        Storylocation storylocation = Storylocation.Prologue;
        List<Button> choicesbuttons = new List<Button>();
        public MainPage()
        {
            InitializeComponent();
            choicesbuttons.Add(btnchoice1);
            choicesbuttons.Add(btnchoice2);
            choicesbuttons.Add(btnchoice3);
            choicesbuttons.Add(btnchoice4);
            choicesbuttons.Add(btnchoice5);
            if (File.Exists(ApplicationData.Current.LocalFolder.Path+"\\savefile.txt")) load();
            else
            {
                player = new Player(5, 5, 5, 5, 0, 0, 1, 3);
                //player = new Player(500, 500, 500, 500, 0, 1000, 100, 300);
                playerhealth = player.Health;
                storymaker();
            }
            //btnrun_Click(null, null);
            timer.Tick += Timer_Tick;
            speaker.Text = "";
            //imgbackground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/town.png"));
            storytime(storylocation);
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
                //var buttons = choicesbuttons.ToArray();
                str = str.Substring(str.IndexOf("{") + 1, str.IndexOf("}") - str.IndexOf("{") - 1);
                var temp = str.Split(',');
                timer.Stop();
                for (int i = 0; i < temp.Length; i++)
                {
                    choicesbuttons[i].Visibility = Visibility.Visible;
                    choicesbuttons[i].Content = temp[i];
                }
            }
            else if (str.StartsWith("event{"))
            {
                str = str.Substring(str.IndexOf("{") + 1, str.IndexOf("}") - str.IndexOf("{") - 1);
                switch (str)
                {
                    case "getname":
                        //player.getname();
                        //storymaker();
                        break;
                    case "+1":
                        next();
                        break;
                    default:
                        break;
                }
                next();
            }
            else if (str.StartsWith("speaker{"))
            {
                int temp = str.IndexOf('}');
                speaker.Text = str.Substring(8, temp - 8);
                text.Text = str.Substring(temp + 2);
            }
            else if (str.StartsWith("type:"))
            {
                switch (str.Substring(str.IndexOf(":") + 1))
                {
                    case "bash":
                        player.Weapontype = Weapon.bash;
                        str = "a good bashing weapon to crush your foes into a paste";
                        break;
                    case "slash":
                        player.Weapontype = Weapon.slash;
                        str = "a good slashing weapon to slice your foes to bits";
                        break;
                    case "stab":
                        player.Weapontype = Weapon.stab;
                        str = "a good stabbing weapon to cleanly hit the vital points of your foes";
                        break;
                    case "fire":
                        player.Weaponenchant = Weapon.fire;
                        str = "fire to burn your foes";
                        break;
                    case "ice":
                        player.Weaponenchant = Weapon.ice;
                        str = "ice to freeze your foes";
                        break;
                    case "air":
                        player.Weaponenchant = Weapon.air;
                        str = "air to blow away your foes";
                        break;
                    case "earth":
                        player.Weaponenchant = Weapon.earth;
                        str = "earth to crush your foes";
                        break;
                    default:
                        str = "";
                        break;
                }
                text.Text = str;
                storytext++;
            }
            else if (str.StartsWith("add{"))
            {
                string str1 = str.Substring(str.IndexOf("{") + 1, str.IndexOf('}') - str.IndexOf("{") - 1),
                    str2=str.Substring(str.IndexOf('}')+1);
                if (int.TryParse(str1.Substring(str1.IndexOf('+') + 1), out int num))
                {
                    if (str1.StartsWith("player"))
                    {
                        str1 = str1.Substring(str1.IndexOf(".") + 1, str1.IndexOf('+') - str1.IndexOf('.') - 1);
                        switch (str1)
                        {
                            case "gold":
                                player.Gold += num;
                                break;
                            default:
                                break;
                        }
                    }
                    else if (str1.StartsWith("timesrejected"))
                    {
                        timesrejected += num;
                    }
                }
                text.Text = str2;
                //next();
            }
            else if (str.StartsWith("background:"))
            {
                str = str.Substring(str.IndexOf(":") + 1);
                imgbackground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/"+str+".png"));
                next();
            }
            else if (str.StartsWith("foreground:"))
            {
                str = str.Substring(str.IndexOf(":") + 1);
                imgforeground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/" + str + ".png"));
                next();
            }
            else
            {
                text.Text = str;
            }
            storytextcheck();
        }
        public void next()
        {
            storytext++;
            Button_Click(null, null);
            storytext--;
        }
        private void btnchoices_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(((Button)sender).Name[((Button)sender).Name.Length-1].ToString(),out int result);
            storytext += result - 1;
            choices.Visibility = Visibility.Collapsed;
            storytime(storylocation);
            Button_Click(null, null);
            int temp=0;
            foreach (Button button in choicesbuttons)
            {
                if (button.Visibility == Visibility.Visible) temp++;
                else break;
            }
            storytext += temp - 1 - result;
            storytextcheck();
            foreach (Button button in choicesbuttons)
            {
                button.Visibility = Visibility.Collapsed;
                button.Content = "";
            }
        }
        private void storytextcheck()
        {
            if (storytext < Story[(int)storylocation].Length)
            {
                storytext++;
                if (storytext == Story[(int)storylocation].Length)
                {
                    menus.Visibility = Visibility.Visible;
                    string temp = text.Text;
                    btntownload_Click(null, null);
                    text.Text = temp;
                    btn.IsEnabled = false;
                    btnskip.IsEnabled = false;
                    btnauto.IsEnabled = false;
                    storytext = 0;
                    save();
                }
            }
        }
        private void btntooutside_Click(object sender, RoutedEventArgs e)
        {
            nav.Visibility = Visibility.Collapsed;
            hud.Visibility = Visibility.Collapsed;
            combat.Visibility = Visibility.Collapsed;
            outside.Visibility = Visibility.Visible;
            imgbackground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/outside.png"));
        }
        private void btntocombat_Click(object sender, RoutedEventArgs e)
        {
            combat.Visibility = Visibility.Visible;
            outside.Visibility = Visibility.Collapsed;
            switch (random.Next(1,5))
            {
                case 1:
                    btntooutside_Click(sender, e);
                    text.Text = "you found nothing";
                    break;
                case 2:
                    btntooutside_Click(sender, e);
                    int num = random.Next(1, 10);
                    player.Gold += num;
                    text.Text = "you found " + num + " gold";
                    break;
                default:
                    switch (((Button)sender).Name)
                    {
                        case "btntoforest":
                            enemy = Enemy.NewForestEnemy();
                            if (player.Level >= 10 && Forestboss)
                            {
                                enemy = Enemy.NewForestEnemy(true);
                                //storytime(Storylocation.Forestboss);
                            }
                            imgbackground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/forest.png"));
                            break;
                        case "btntocrypt":
                            enemy = Enemy.NewCryptEnemy();
                            if (player.Level >= 20 && Cryptboss)
                            {
                                enemy = Enemy.NewCryptEnemy(true);
                                //storytime(Storylocation.Cryptboss);
                            }
                            imgbackground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/crypt.png"));
                            break;
                        case "btntoswap":
                            enemy = Enemy.NewSwampEnemy();
                            if (player.Level >= 30 && Swampboss)
                            {
                                enemy = Enemy.NewSwampEnemy(true);
                                //storytime(Storylocation.Swampboss);
                            }
                            break;
                        case "btntodragonden":
                            enemy = Enemy.NewDragondenEnemy();
                            if (player.Level >= 40 && Dragondenboss)
                            {
                                enemy = Enemy.NewDragondenEnemy(true);
                                //storytime(Storylocation.Dragondenboss);
                            }
                            break;
                        case "btntodemonlord":
                            enemy = Enemy.demonlord;
                            break;
                        default:
                            break;
                    }
                    enemyhealth = enemy.Health;
                    text.Text = enemy.Name + ": " + enemyhealth + "/" + enemy.Health;
                    break;
            }
        }
        private void btnrun_Click(object sender, RoutedEventArgs e)
        {
            switch (Speedcheck())
            {
                case 1:
                    break;
                default:
                    btntooutside_Click(sender, e);
                    text.Text = "you ran from the " + enemy.Name;
                    break;
            }
        }

        private void btntownload_Click(object sender, RoutedEventArgs e)
        {
            combat.Visibility = Visibility.Collapsed;
            outside.Visibility = Visibility.Collapsed;
            nav.Visibility = Visibility.Visible;
            hud.Visibility = Visibility.Visible;
            imgbackground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/town.png"));
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
            if (((byte)enemy.Weakness) == ((byte)player.Weapontype) || ((byte)enemy.Weakness) == ((byte)player.Weaponenchant))
            {
                enemydamage += (enemydamage / 10) + 1;
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
                btntownload_Click(null, null);
                if (enemyhealth <= 0 && playerhealth <= 0)
                {
                    player.XP += enemy.XP / 2;
                    player.Gold += enemy.Gold / 2;
                    text.Text = holder + "\nas you fall over you see the " + enemy.Name + " is dead and you received " + enemy.XP / 2 + " Xp and " + enemy.Gold / 2 + " Gold\n"
                        + player.XP + "/100 Xp " + player.Gold + " Gold";
                    btntooutside.IsEnabled = false;
                    bosscheck();
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
                    bosscheck();
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
        public void bosscheck()
        {
            if (enemy.Name.Equals("lion"))
            {
                text.Text = text.Text + "\nwith the king of beast dead you return to the palace";
                Forestboss = false;
                bossesdead++;
                btntocrypt.Visibility = Visibility.Visible;
                storytime(Storylocation.Forestcomplete);
            }
            else if (enemy.Name.Equals("lich"))
            {
                text.Text = text.Text + "\nwith the lord of the dead defeated you return to the palace";
                Cryptboss = false;
                bossesdead++;
                btntoswamp.Visibility = Visibility.Visible;
                storytime(Storylocation.Cryptcomplete);
            }
            else if (enemy.Name.Equals("witch"))
            {
                text.Text = text.Text + "\nwith the witch defeated you return to the palace";
                Swampboss = false;
                bossesdead++;
                btntodragonden.Visibility = Visibility.Visible;
            }
            else if (enemy.Name.Equals("demon lord"))
            {
                text.Text = text.Text + "\nwith the demon lord dead you return to the palace";
                if (timesrejected == bossesdead)
                {
                    storylocation = Storylocation.KilledEpilogue;//rejected the gold every time boss was killed
                }
                else
                {
                    storylocation = Storylocation.ControlledEpilogue;
                }
                btntodemonlord.Visibility = Visibility.Collapsed;
                storytime(storylocation);
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
            btntownload_Click(sender, e);
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
            text.Text = "you see the Receptionists handing out quests";
            imgbackground.Source= new BitmapImage(new Uri(BaseUri, "./Assets/adventurersguild.png"));
            imgforeground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/Receptionist.png"));
        }

        private void btnshop_Click(object sender, RoutedEventArgs e)
        {
            nav.Visibility = Visibility.Collapsed;
            shop.Visibility = Visibility.Visible;
            text.Text = "you see the shopkeep behind the shop counter";
            imgbackground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/shop.png"));
            imgforeground.Source = new BitmapImage(new Uri(BaseUri, "./Assets/shopkeeper.png"));
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
        Enemy questenemy = new Enemy(1, 1, 1, 1, 1, 1, "", "default");
        int questnum = -1, questreward;
        private void btngetquest_Click(object sender, RoutedEventArgs e)
        {
            btngetquest.Visibility = Visibility.Collapsed;
            btnviewquest.Visibility = Visibility.Visible;
            questenemy = Enemy.NewForestEnemy();
            questnum = random.Next(2, 6);
            questreward = questnum * 5;
            btnviewquest_Click(sender, e);
        }
        bool princessesforest = true, princessescrypt = true, princessesswamp = true, princessesdragonden = true;
        int princessesaffinity = 0;
        private void btntalktoprincesses_Click(object sender, RoutedEventArgs e)
        {
            if ((!Forestboss) && princessesforest)
            {
                princessesforest = false;
                princessesaffinity++;
                text.Text = "good job killing the lion";
            }
            else if ((!Cryptboss) && princessescrypt)
            {
                princessescrypt = false;
                princessesaffinity++;
                text.Text = "good job killing the lich";
            }
            else if ((!Swampboss) && princessesswamp)
            {
                princessesswamp = false;
                princessesaffinity++;
                text.Text = "good job killing the witch";
            }
            else if ((!Dragondenboss) && princessesdragonden)
            {
                princessesdragonden = false;
                princessesaffinity++;
                text.Text = "good job killing the dragon king";
            }
            else
            {
                string str;
                switch (random.Next(5))
                {
                    case 0:
                        str = "good luck " + player.Name;
                        break;
                    case 1:
                        str = "don't get to hurt " + player.Name;
                        break;
                    case 2:
                        str = "stay safe " + player.Name;
                        break;
                    case 3:
                        str = "talk to me if you need help";
                        break;
                    default:
                        str = "more to come";
                        break;
                }
                text.Text = str;
            }
        }

        private void save_Click(object sender, RoutedEventArgs e) { save(); }

        private void load_Click(object sender, RoutedEventArgs e) { load(); }

        private void delete_Click(object sender, RoutedEventArgs e) { if(File.Exists(ApplicationData.Current.LocalFolder.Path + "\\savefile.txt")) File.Delete(ApplicationData.Current.LocalFolder.Path + "\\savefile.txt"); }

        private void btnviewquest_Click(object sender, RoutedEventArgs e)
        {
            text.Text = "defeat " + questnum + " " + questenemy.Name + "s for " + questreward + "  gold" + "\n" + questenemy.Advice;
        }

        private void btnsubmitquest_Click(object sender, RoutedEventArgs e)
        {
            btngetquest.Visibility = Visibility.Visible;
            btnviewquest.Visibility = Visibility.Collapsed;
            btnsubmitquest.Visibility = Visibility.Collapsed;
            player.Gold += questreward;
            player.XP += questenemy.XP * 3;
            questnum = -1;
            questenemy.Name = "default";
        }

        private void btnweaponchange_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Content)
            {
                case "bash":
                    player.Weapontype = Weapon.bash;
                    break;
                case "slash":
                    player.Weapontype = Weapon.slash;
                    break;
                case "stab":
                    player.Weapontype = Weapon.stab;
                    break;
                case "fire":
                    player.Weaponenchant = Weapon.fire;
                    break;
                case "ice":
                    player.Weaponenchant = Weapon.ice;
                    break;
                case "air":
                    player.Weaponenchant = Weapon.air;
                    break;
                case "earth":
                    player.Weaponenchant = Weapon.earth;
                    break;
            }
        }

        private void btntownback_Click(object sender, RoutedEventArgs e)
        {
            imgbackground.Source= new BitmapImage(new Uri(BaseUri, "./Assets/town.png"));
            imgforeground.Source = null;
            btntownload_Click(sender, e);
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
            save();
        }
        private void sound(string path)
        {
            MediaElement media = new MediaElement();
            media.Source = new Uri(BaseUri, path);
            //media.Play();
        }
        private void save()
        {
            string path = ApplicationData.Current.LocalFolder.Path + "\\savefile.txt";
            File.WriteAllText(path, player.ToString() + "," + playerhealth + "|" + bossesdead + "," + Forestboss + "," + Cryptboss + "," + Swampboss + "," + Dragondenboss + "," + demonlord+"|"+
                (int)storylocation+","+princessesforest + "," +princessescrypt + "," +princessesswamp + "," +princessesdragonden + "," +princessesaffinity);
        }
        private void load()
        {
            if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\savefile.txt"))
            {
                playerhealth = 0;
                string text = File.ReadAllText(ApplicationData.Current.LocalFolder.Path + "\\savefile.txt");
                string[] readValue = text.Split("|");
                if (readValue.Length>=1)
                {
                    string[] savedplayer = readValue[0].Split(",");
                    player = new Player(int.Parse(savedplayer[0]), int.Parse(savedplayer[1]), int.Parse(savedplayer[2]), int.Parse(savedplayer[3]), int.Parse(savedplayer[4]),
                        int.Parse(savedplayer[5]), int.Parse(savedplayer[6]), int.Parse(savedplayer[7]), (Weapon)int.Parse(savedplayer[8]), (Weapon)int.Parse(savedplayer[9]), savedplayer[10]);
                    playerhealth = int.Parse(savedplayer[11]);
                    if (readValue.Length>=2)
                    {
                        string[] bosses = readValue[1].Split(",");
                        bossesdead = int.Parse(bosses[0]); Forestboss = bool.Parse(bosses[1]); Cryptboss = bool.Parse(bosses[2]); Swampboss = bool.Parse(bosses[3]); Dragondenboss = bool.Parse(bosses[4]); demonlord = bool.Parse(bosses[5]);
                        btntocrypt.Visibility = !Forestboss ? Visibility.Visible : Visibility.Collapsed; btntoswamp.Visibility = !Cryptboss ? Visibility.Visible : Visibility.Collapsed;
                        btntodragonden.Visibility = !Swampboss ? Visibility.Visible : Visibility.Collapsed; btntodemonlord.Visibility = !(Dragondenboss && demonlord) ? Visibility.Visible : Visibility.Collapsed;
                        if (readValue.Length>=3)
                        {
                            string[] story = readValue[2].Split(",");
                            storylocation = (Storylocation)int.Parse(story[0]); princessesforest = bool.Parse(story[1]); princessescrypt = bool.Parse(story[2]);
                            princessesswamp = bool.Parse(story[3]); princessesdragonden = bool.Parse(story[4]); princessesaffinity = int.Parse(story[5]);
                        }
                    }
                }
                storylocation = Storylocation.Load;
                storymaker(player.Name);
            }
        }
        private void storytime(Storylocation location)
        {
            storylocation = location;
            btn.IsEnabled = true;
            btnskip.IsEnabled = true;
            btnauto.IsEnabled = true;
            hud.Visibility = Visibility.Collapsed;
            combat.Visibility = Visibility.Collapsed;
            nav.Visibility = Visibility.Collapsed;
            adventurersguild.Visibility = Visibility.Collapsed;
            shop.Visibility = Visibility.Collapsed;
            menus.Visibility = Visibility.Collapsed;

        }
        /// <summary>
        /// use options{option1,options2...} for player options +numofoptions
        /// use event{event} for an event +1
        ///     use event{+1} in conjuntion with options
        ///         ie:"event{+1}","prompt","options{}"
        /// speaker{name of speaker} (what they're saying) +0
        /// background/foreground:(imagename) (must be .png) +1
        /// add{varname+num} (text to go with it) +0
        ///     num can be - but keep the +
        ///     
        /// when using any of these special cases make sure to have enough space infront of it
        /// </summary>
        public static void storymaker(string playername="")
        {
            //TODO I NEED NAMES
            //names
            string kingdom = "kingdom", princesses = "princesses", king = "king";
            Story[(int)Storylocation.Load] = new string[] { "welcome back" };
            Story[(int)Storylocation.Prologue] = new string[] { "background:darkness","any similarities to real life people or events are purely coincidence\nwelcome to The (un)seen future", "event{getname}",
                "speaker{" + playername + " thoughts} everyone is born with something that makes them special or different from the norm, it might not be completely unique but it's something you can take pride in.",
                "speaker{" + playername + " thoughts} often we are unable to see our own abilities always comparing ourselves to others, \"they can do it better\" or \"they can do it faster\", while not seeing that there are things you do better or faster.",
                "speaker{" + playername + " thoughts} some people will go there whole life with out knowing what their ability is because they never had an opportunity to use it, others find it early but reject it wanting to find their own path rather than one given to them by random chance, and everything in between.",
                "speaker{" + playername + " thoughts} you might have increased strength, vision, hearing, or an increase in some other aspest. or perhaps your able to do something others can't like having photographic memory or how I can see the future.",
                "speaker{" + playername + " thoughts} most would think that this ability is amazing how ever there's a catch, it only happens while I'm asleep in my dreams, until it happens i can't remeber the dream, and i have no control over what I see or when I see it",
                "speaker{" + playername + " thoughts} this might seem far-fetched, how do i know that i can see the future if I can't remember it, maybe it's just a bad case of deja vu?",
                "speaker{" + playername + " thoughts} but dreams and deja vu work with memories and past experiences and doesn't just make up random things, so bing able to see entire scenes with unknown things in unknown places play out then have the exact same scenes play out what else would this be",
                "speaker{" + playername + " thoughts} be it nothing more then dreams or precognition a couple nights ago I had a strange dream that was something that was stright out of a fantasy story yet it felt so real",
                "speaker{" + playername + " thoughts} but it was not the whole story, it kept jumping to random scenes, one second I was being called a hero by an unknown shadowy figure, the next fighting a demon, after that having a knife stabbed into my back surrounded by noble looking people",
                "speaker{" + playername + " thoughts} I woke up in a cold sweat but had forgotten the dream entirely as if wiped from my mind, but now it's back as if it was never gone becuse the first part is happen a shadowy figure is calling me a hero.",
                "foreground:princesses",
                "event{+1}","speaker{shadowy figure} THE SUMMONING WORKED! oh hero please save us from the demon lord",
                "options{who are you,where is this,whats going on}",
                "speaker{"+princesses+"} I am the princesses of the kingdom "+kingdom+", "+princesses,
                "speaker{"+princesses+"} the kingdom of "+kingdom+" my name is "+princesses,
                "speaker{"+princesses+"} you have been summoned to this world my name is "+princesses,
                "background:throneroom","you look around and see that you are now in a throne room",
                "speaker{"+princesses+"} before you set off on your quest choose your weapon",
                "event{+1}","speaker{"+princesses+"} which type of weapon do you want",
                "options{bash,slash,stab}",
                "type:bash","type:slash","type:stab",
                "",
                "event{+1}","speaker{"+princesses+"} which element do you want your weapon to have on your weapon",
                "options{fire,ice,air,earth}",
                "type:fire","type:ice","type:air","type:earth",
                "background:town",
                "foreground:none",
                "" };
            Story[(int)Storylocation.Forestboss] = new string[] { "" };
            Story[(int)Storylocation.Forestcomplete] = new string[] { "after retuning from the battle the king greets you", "speaker{" + king + "} well done hero on killing the king of the beast for your deed please accept this 250 gold", "event{+1}",
            "speaker{"+playername+"} thank you sir","options{accept,reject}","add{player.gold+250} you accepted the gold","add{timesrejected+1} you don't accept the gold","you take your leave"};
            Story[(int)Storylocation.Cryptboss] = new string[] { "test" };
            Story[(int)Storylocation.Cryptcomplete] = new string[] { "after retuning from the battle the king greets you", "speaker{" + king + "} well done hero on killing the lord of the dead for your deed please accept this 500 gold", "event{+1}",
            "speaker{"+playername+"} thank you sir","options{accept,reject}","add{player.gold+500} you accepted the gold","add{timesrejected+1} you don't accept the gold","you take your leave"};
            Story[(int)Storylocation.ControlledEpilogue] = new string[] { "after returning to the palace you receive your reward\nit takes some time but the king shows up and hands you a heavy bag of gold", "speaker{" + king + "} congratulations on beating the demon lord",
            "add{player.gold+1500}","you look in side the bag and see 1500 gold" };
            //,"speaker{dev} this ending you are a puppet of the kingdom controlled by money"};
            Story[(int)Storylocation.KilledEpilogue] = new string[] { "after returning to the palace you receive your reward\nwhile gitting your reward you feel a knife going in to your back stright to the heart","you have been betrayed by those you worked to save",
                "speaker{" + king + "} he didn't see it coming hahaha did he think we would keep someone we couldn't control alive when he has the power to defeat the demon lord?",
                "speaker{" + playername + " thoughts} so they feared my power and decided to get rid of me before anything bad would happen, what could i have done to cahanged this outcome" };
                //,"speaker{dev} this ending the the kingdom couldn't control you so they kill you to prevent you from doing anything"};
        }
    }
    public enum Storylocation
    {
        Load,Prologue, Forestboss, Forestcomplete, Cryptboss, Cryptcomplete, ControlledEpilogue, KilledEpilogue
    }
}
