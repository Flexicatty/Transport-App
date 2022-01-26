using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModernWpf;
using Newtonsoft.Json;

namespace NEA_A_Lvl_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginWindow loginWindow = new LoginWindow();
        int inprogressnum = 0, inprogressnum2 = 0;
        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            ThemeManager.Current.AccentColor = Colors.Blue;

            Title = "Transport App";
            Background = new ImageBrush { ImageSource = new BitmapImage(Settings.background) };
            LoadMenu(); //loads the Menu objects
            Settings.SetTime();
            loginWindow.Closing += Login_Closing;
            loginWindow.IsVisibleChanged += Login_IsVisibleChanged;
            Closed += MainWindow_Closed;
            KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void LoadMenu()
        {
            Image[] textimages = new Image[2]; //Image array for the title
            textimages[0] = new Image() { Source = new BitmapImage(Settings.transporttext) };
            textimages[1] = new Image() { Source = new BitmapImage(Settings.apptext) }; //gets the source from the url class I created
            for (int i = 0; i < 2; i++) //Sets the positions of the "Transport App" and adds them to the canvas
            {
                Canvas.SetLeft(textimages[i], 185 + (i * 310));
                Canvas.SetTop(textimages[i], 10);
                myC.Children.Add(textimages[i]);
            }
            trainButton.Background = new ImageBrush { ImageSource = new BitmapImage(Settings.train)};
            busButton.Background = new ImageBrush { ImageSource = new BitmapImage(Settings.bus) };
            publicButton.Background = new ImageBrush { ImageSource = new BitmapImage(Settings.publictrans) };
            roadButton.Background = new ImageBrush { ImageSource = new BitmapImage(Settings.roadmap) };
            trainButton.MouseEnter += TrainButton_MouseEnter;
            trainButton.MouseLeave += TrainButton_MouseLeave;
            busButton.MouseEnter += BusButton_MouseEnter;
            busButton.MouseLeave += BusButton_MouseLeave;
            publicButton.MouseEnter += PublicButton_MouseEnter;
            publicButton.MouseLeave += PublicButton_MouseLeave;
            roadButton.MouseEnter += RoadButton_MouseEnter;
            roadButton.MouseLeave += RoadButton_MouseLeave;
        }

        

        private void Login_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Settings.loggedin == true) //if the user is logged in, display their profile picture and name
            {
                Settings.profile.MouseDown += Profile_MouseDown;
                Settings.profile.MouseEnter += Profile_MouseEnter;
                Settings.profile.MouseLeave += Profile_MouseLeave;
                Settings.logoutbutton.Click += Logoutbutton_Click;
                Canvas.SetLeft(Settings.profile, 740);
                Canvas.SetLeft(Settings.usernamelable, 734); Canvas.SetTop(Settings.usernamelable, 40);
                Settings.usernamelable.Content = Stats.GetUsername(); //Get the username from the stats
                if (!myC.Children.Contains(Settings.profile))                
                    myC.Children.Add(Settings.profile);                
                if (!myC.Children.Contains(Settings.usernamelable))
                    myC.Children.Add(Settings.usernamelable);
                if (!myC.Children.Contains(Settings.logoutbutton))
                    myC.Children.Add(Settings.logoutbutton);
                myC.Children.Remove(loginbutton);
            }
            mainWin.Show(); //When the login is correct, login window hidden, mainwindow shown
        }

        private void Logoutbutton_Click(object sender, RoutedEventArgs e)
        {
            myC.Children.Remove(Settings.profile);
            myC.Children.Remove(Settings.usernamelable);
            myC.Children.Remove(Settings.logoutbutton);
            if (!myC.Children.Contains(loginbutton))
            {
                myC.Children.Add(loginbutton);
            }      
            Settings.loggedin = false;
        }

        private void Profile_MouseLeave(object sender, MouseEventArgs e)
        {
            Settings.profile.Stroke = Brushes.Black; //when mouse leaves profile picture, return border colour back to normal
        }

        private void Profile_MouseEnter(object sender, MouseEventArgs e)
        {
            Settings.profile.Stroke = Brushes.LimeGreen; //when mouse enters profile picture, return 
        }
        int test = 0;
        private void Profile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.CurrentLocation != "profile") //if profile pic is pressed on another page/window, go to profile setup
            {
                myC.Children.Clear();
                Settings.CurrentLocation = "profile";
                ProfileSetup();
            }
            else if (Settings.CurrentLocation == "profile") //if profile pic is pressed in the profile page
            {

            }
        }
        private void ProfileSetup()
        {
            Settings.profile.Height = 100;
            Settings.profile.Width = 100;
            Canvas.SetLeft(Settings.profile, 350);
            Canvas.SetTop(Settings.profile, 50);
            if (!myC.Children.Contains(Settings.profile))
                myC.Children.Add(Settings.profile);

            Button returnbutton = new Button() { Content = "Return", BorderBrush = Brushes.Black, BorderThickness = new Thickness(1.5), Width = 90, Height = 34, HorizontalAlignment= HorizontalAlignment.Left};
            Canvas.SetLeft(returnbutton, 702);
            if (!myC.Children.Contains(returnbutton))
                myC.Children.Add(returnbutton);

            Label usernamelable = new Label() { Content = Stats.GetUsername(), Width = 100, Height = 34, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontWeight = FontWeights.SemiBold, FontSize = 20};
            Canvas.SetLeft(usernamelable, 350);
            Canvas.SetTop(usernamelable, 150);
            if (!myC.Children.Contains(usernamelable))
                myC.Children.Add(usernamelable);

            if (!myC.Children.Contains(Settings.StatsGrid))
                CreateStatsGrid();

            List<string> headerlist = new List<string>() { "Name:", "Number of Journeys:", "Total KM travelled:", "Total money spent:", "Total time spent on Journeys:"};
            List<string> statslist = Stats.GetDataAsList();
            TextBox nameBox = new TextBox();
            for (int i = 0; i < statslist.Count; i++)
            {
                TextBox txt = new TextBox() { Text = statslist[i], FontSize = 14, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, IsReadOnly = true};
                Grid.SetRow(txt, i);
                Grid.SetColumn(txt, 1);
                if (i == 0) //name is at the top
                {
                    nameBox = txt;
                    Settings.StatsGrid.Children.Add(nameBox);
                }
                else
                    Settings.StatsGrid.Children.Add(txt);
            }
            for (int i = 0; i < statslist.Count; i++)
            {
                Label lbl = new Label() { Content = headerlist[i], FontSize = 14, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetRow(lbl, i);
                Grid.SetColumn(lbl, 0);
                Settings.StatsGrid.Children.Add(lbl);
            }

            Button editnamebutton = new Button() { Content = "Edit", BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center};
            editnamebutton.Click += (sender,e) => Editnamebutton_Click(sender,e,nameBox);
            Grid.SetColumn(editnamebutton, 1); Grid.SetRow(editnamebutton, 0);
            Settings.StatsGrid.Children.Add(editnamebutton);
        }

        private void Editnamebutton_Click(object sender, RoutedEventArgs e, TextBox nameBox)
        {
            Button btn = (Button)sender;
            if(nameBox.IsReadOnly == true)
            {
                nameBox.IsReadOnly = false;
                nameBox.Focus();
                btn.Content = "Set";
            }
            else
            {
                nameBox.IsReadOnly = true;
                Stats.SetName(nameBox.Text);
                btn.Content = "Edit";
            }
        }

        private void CreateStatsGrid()
        {
            Settings.StatsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Settings.StatsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // Create Rows
            foreach (var item in Stats.GetDataAsList())
            {
                Settings.StatsGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(45) });
            }
            Canvas.SetTop(Settings.StatsGrid, 190);
            Canvas.SetLeft(Settings.StatsGrid, 200);
            myC.Children.Add(Settings.StatsGrid);
        }
        private void Login_Closing(object sender, EventArgs e)
        {
            Environment.Exit(0); //Once login window is closed program will end
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (Settings.loggedin == true)
            {
                string jsondata = JsonConvert.SerializeObject(Stats.GetData(), Formatting.Indented);
                string encrypteddata = LoginWindowModel.encryptdata(jsondata); //encrypt the username and password data using symmetrical key
                byte[][] publickey = Server.Getpublickey(); //gets public key from server *placeholder*
                byte[][] encryptedsymkey = LoginWindowModel.encryptkey(publickey); //encrypt the symmetrical encryption key using the asymmetrical public key
                Server.SaveStats(encryptedsymkey, encrypteddata); //
            }
            Server.Endcommunication(); //*placeholder*
            Environment.Exit(0); //Once Main Window is closed program will end
        }

        private void Loginbutton_Click(object sender, RoutedEventArgs e)
        {
            loginWindow.Show();
            mainWin.Hide(); //Want to hide the main window, when person wants to login                       
        }

        private void TrainButton_Click(object sender, RoutedEventArgs e)
        {
            myC.Children.Remove(Settings.profile); //disconnects profile from window so it can be used in other windows
            TrainTimetable trainTimetable = new TrainTimetable();
            mainWin.Hide();
            trainTimetable.Show();
            trainTimetable.Closed += TrainTimetableWindow_Closed;
        }

        private void TrainTimetableWindow_Closed(object sender, EventArgs e)
        {
            TrainSearchData.ClearData();
            mainWin.Show();
            if (!myC.Children.Contains(Settings.profile) && Settings.loggedin == true)
            {
                myC.Children.Add(Settings.profile);
            }
        }

        private void BusButton_Click(object sender, RoutedEventArgs e)
        {
            if (inprogressnum == 0)
            {
                bustext.Text = "Coming Soon";
                inprogressnum++;
            }
            else
            {
                bustext.Text = "Bus Timetable";
                inprogressnum = 0;
            }
            //myC.Children.Remove(Settings.profile); //disconnects profile from window so it can be used in other windows
            //TimetableWindow timetableWindow = new TimetableWindow();
            //timetableWindow.Closed += TimetableWindow_Closed;
            //timetableWindow.Show();
            //mainWin.Hide();
        }

        private void PublicButton_Click(object sender, RoutedEventArgs e) //WIP
        {
            if (inprogressnum2 == 0)
            {
                publictext.Text = "Coming Soon";
                inprogressnum2++;
            }
            else
            {
                publictext.Text = "Public Transport Planner";
                inprogressnum2 = 0;
            }

        }

        private void RoadButton_Click(object sender, RoutedEventArgs e)
        {
            MapWindow journeyMap = new MapWindow();
            mainWin.Hide();
            journeyMap.Show();
            journeyMap.Closed += JourneyMap_Closed;
        }

        private void JourneyMap_Closed(object sender, EventArgs e)
        {
            mainWin.Show();
            if (!myC.Children.Contains(Settings.profile) && Settings.loggedin == true)
            {
                myC.Children.Add(Settings.profile);
            }
        }

        private void RoadButton_MouseLeave(object sender, MouseEventArgs e)
        {
            roadtext.Opacity = 0;
        }
        private void RoadButton_MouseEnter(object sender, MouseEventArgs e)
        {
            roadtext.Opacity = 1;
        }
        private void PublicButton_MouseLeave(object sender, MouseEventArgs e)
        {
            publictext.Opacity = 0;
        }
        private void PublicButton_MouseEnter(object sender, MouseEventArgs e)
        {
            publictext.Opacity = 1;
        }
        private void BusButton_MouseLeave(object sender, MouseEventArgs e)
        {
            bustext.Opacity = 0;
        }
        private void BusButton_MouseEnter(object sender, MouseEventArgs e)
        {
            bustext.Opacity = 1;
        }
        private void TrainButton_MouseLeave(object sender, MouseEventArgs e)
        {
            traintext.Opacity = 0;
        }
        private void TrainButton_MouseEnter(object sender, MouseEventArgs e)
        {
            traintext.Opacity = 1;
        }


    }
}
