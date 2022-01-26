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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace NEA_A_Lvl_Project
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    static class IO //Class of all the object for the login window
    {      
        public static TextBox createlogintb { get; set; } = new TextBox() { Text = "Username", Height = 13, Width = 150 };
        public static TextBox createloginpasstb { get; set; } = new TextBox() { Text = "Password", Height = 13, Width = 150 };
        public static TextBox createloginpasschecktb { get; set; } = new TextBox() { Text = "Type Password Again", Height = 13, Width = 150 };
        public static PasswordBox createloginpb { get; set; } = new PasswordBox() { Height = 32, Width = 150, Visibility = Visibility.Hidden };
        public static PasswordBox createloginpbcheck { get; set; } = new PasswordBox() { Height = 32, Width = 150, Visibility = Visibility.Hidden };
        public static Button createloginenterbutton { get; set; } = new Button() { Height = 31, Width = 30, Background = Brushes.Gray};   
    }
    public partial class LoginWindow : Window
    {
        Label incorrectlabel = new Label() { Content = "Username or Password is Incorrect", Height = 32, Width = 250, Foreground = Brushes.Red, Background = Brushes.Transparent, FontWeight = FontWeights.SemiBold, VerticalContentAlignment = VerticalAlignment.Center }; //lable for when username/password is incorrect
        Label logincorrectlabel = new Label() { Content = "Login Created Successfully!", Height = 32, Width = 250, Foreground = Brushes.LimeGreen, Background = Brushes.Transparent, FontWeight = FontWeights.SemiBold, VerticalContentAlignment = VerticalAlignment.Center }; //lable for when username/password is incorrect
        DispatcherTimer labeltimer = new DispatcherTimer(); //Timer for lables appearing on screen for messages e.g. error messages
        Image[] textimages = new Image[2]; //Image array for the title
        private bool logincreated = false;


        public LoginWindow()
        {
            InitializeComponent();
            LoginWindowModel loginModel = new LoginWindowModel();
            Title = "Transport App";
            ClearCanvas();
            LoadUI();
            Server.Init(); //initiaties connection server *placeholder*

            logintextBox.GotFocus += LogintextBox_GotFocus;
            logintextBox.LostFocus += LogintextBox_LostFocus;
            loginpasswordBox.LostFocus += LoginpasswordBox_LostFocus;
            loginpasswordtextBox.GotFocus += LoginpasswordtextBox_GotFocus; 
            //^^^ All of theses events to make the Username and Password text appear in the text/password boxes

            LoginWin.KeyDown += LoginWin_KeyDown;      
        }

        private void ClearCanvas() //Clears the Canvas of the Login Create objects
        {
            myC.Children.Remove(IO.createloginenterbutton);
            myC.Children.Remove(IO.createloginpasschecktb);
            myC.Children.Remove(IO.createloginpasstb);
            myC.Children.Remove(IO.createloginpb);
            myC.Children.Remove(IO.createloginpbcheck);
            myC.Children.Remove(IO.createlogintb);
            myC.Children.Remove(textimages[0]);
            myC.Children.Remove(textimages[1]);
            returnbutton.Visibility = Visibility.Hidden;
        }
        private void LoadUI()
        {
            if (myC.Children.Contains(incorrectlabel))
            {
                myC.Children.Remove(incorrectlabel);
            }
            textimages[0] = new Image() { Source = new BitmapImage(Settings.transporttext) };
            textimages[1] = new Image() { Source = new BitmapImage(Settings.apptext) }; //gets the source from the url class
            Background = new ImageBrush { ImageSource = new BitmapImage(Settings.background) };
            for (int i = 0; i < 2; i++) //Sets the positions of the "Transport App" and adds them to the canvas
            {
                Canvas.SetLeft(textimages[i], 185 + (i * 310));
                Canvas.SetTop(textimages[i], 120);
                if (!myC.Children.Contains(textimages[i]))
                {
                    myC.Children.Add(textimages[i]);
                }
            }
            Canvas.SetLeft(incorrectlabel, 300);
            Canvas.SetTop(incorrectlabel, 320); //setting position of the incorrect label
            Canvas.SetLeft(logincorrectlabel, 320);
            Canvas.SetTop(logincorrectlabel, 320); //setting position of correct label
            //myC.Children.Add(logincorrectlabel); //myC.Children.Add(incorrectlabel);
            //incorrectlabel.Content = "Username or Password is Incorrect"; //resets the incorrect label Content
            if (!myC.Children.Contains(logintextBox))
            {
                Canvas.SetLeft(logintextBox, 325);
                Canvas.SetTop(logintextBox, 245);
                myC.Children.Add(logintextBox);
            }
            if (!myC.Children.Contains(loginpasswordtextBox))
            {
                Canvas.SetLeft(loginpasswordtextBox, 325);
                Canvas.SetTop(loginpasswordtextBox, 282);
                myC.Children.Add(loginpasswordtextBox);
            }
            if (!myC.Children.Contains(loginpasswordBox))
            {
                Canvas.SetLeft(loginpasswordBox, 325);
                Canvas.SetTop(loginpasswordBox, 282);
                myC.Children.Add(loginpasswordBox);
            }
            if (!myC.Children.Contains(loginenterbutton))
            {
                Canvas.SetLeft(loginenterbutton, 480);
                Canvas.SetTop(loginenterbutton, 283);
                myC.Children.Add(loginenterbutton);
            }
            if (!myC.Children.Contains(logincreatebutton))
            {
                Canvas.SetLeft(logincreatebutton, 325);
                Canvas.SetTop(logincreatebutton, 349);
                myC.Children.Add(logincreatebutton);
            }
            //^^Adds all the login input objects to the canvas
        }
        private void LoadCreateLoginUI(TextBox textbox, int left, int top)//Adds Textboxes to the canvas
        {
            Canvas.SetLeft(textbox, left);
            Canvas.SetTop(textbox, top);
            if (!myC.Children.Contains(textbox))
            {
                myC.Children.Add(textbox);
            }
        }
        private void LoadCreateLoginUIpass(PasswordBox passbox, int left, int top)//Adds Passwordboxes to the canvas
        {
            Canvas.SetLeft(passbox, left);
            Canvas.SetTop(passbox, top);
            if (!myC.Children.Contains(passbox))
            {
                myC.Children.Add(passbox);
            }
        }

        private void incorrectlabelshow(string error)//When there is an error show the error lable for 1.5s with suitable error message
        {
            labeltimer.Tick += labeltimer_Tick;
            labeltimer.Interval = new TimeSpan(0, 0, 0, 0, 1500);
            labeltimer.Start();
            incorrectlabel.Content = error;
            if (!myC.Children.Contains(incorrectlabel))
            {
                myC.Children.Add(incorrectlabel);
            }
        }
        private void logincorrectlabelshow(string text)//When login is successfully created show the correct lable for 1.5s with suitable message
        {
            labeltimer.Tick += labeltimer_Tick;
            labeltimer.Interval = new TimeSpan(0, 0, 0, 0, 1500);
            labeltimer.Start();
            logincorrectlabel.Content = text;
            if (!myC.Children.Contains(logincorrectlabel))
            {
                myC.Children.Add(logincorrectlabel);
            }
        }
        private void labeltimer_Tick(object sender, EventArgs e)
        {
            if (myC.Children.Contains(incorrectlabel)) //if the incorrect label already exists, remove it and stop the timer
            {
                myC.Children.Remove(incorrectlabel);
                labeltimer.Stop();
            }
            if (myC.Children.Contains(logincorrectlabel)) //if the login label already exists, remove it and stop the timer
            {
                myC.Children.Remove(logincorrectlabel);
                labeltimer.Stop();
            }
        }

        private void LoginWin_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Enter();
            }
            //if(e.Key == Key.OemMinus) //for testing at the moment 
            //{
            //    encryptlogindata(logintextBox.Text, loginpasswordBox.Password); //encrypts data from the username and password into 2 files
            //}

        }

        private void loginenterbutton_Click(object sender, RoutedEventArgs e)
        {
            Enter();
        }

        private void Enter()
        {
            Keyboard.ClearFocus(); //makes sure no input boxes are focused
            if (myC.Children.Contains(loginenterbutton))
            {
                if (string.IsNullOrEmpty(logintextBox.Text) || string.IsNullOrEmpty(loginpasswordBox.Password)) //Checks if user has actual entered username/password
                {
                    incorrectlabelshow("Please enter a Username/Password");
                }
                else
                {
                    LoginWindowModel.containsincorrectchar = checkloginchar(); //Checks if login is acceptable
                    LoginWindowModel.Loginenter(logintextBox.Text, loginpasswordBox.Password); //Checks if login exists and will set Settings.loggedin = true if so
                    if (Settings.loggedin == true)
                    {
                        loginpasswordBox.Clear(); //clears passwordbox once logged in
                        logintextBox.Clear(); //clears textbox once logged in
                        LoginWin.Hide(); //hides the loginwin
                    }
                    else
                    {
                        incorrectlabelshow("Username or Password is Incorrect");
                    }
                }
            }
            else if (myC.Children.Contains(IO.createloginenterbutton))
            {
                CreateLoginCheck();
                if (logincreated == true)
                {
                    resetcreatelogintb();
                    ClearCanvas();
                    LoadUI();
                }
            }
        }

        private bool checkloginchar() //checks loginboxes for incorrect chars
        {
            if (logintextBox.Text.Contains(",") || logintextBox.Text.Contains("/") || loginpasswordBox.Password.Contains(",") || loginpasswordBox.Password.Contains("/")) //Cannot enter comma or slash due to how user login data is stored
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void LoginpasswordtextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(loginpasswordBox.Password))
            {
                loginpasswordtextBox.Visibility = Visibility.Hidden; //hides the text "password" when password text box becomes focused
                loginpasswordBox.Visibility = Visibility.Visible; //shows the empty password box
            }
            Keyboard.ClearFocus(); //Clears the focus on the text box
            loginpasswordBox.Focus(); //Focuses the password box
        }      
        private void LoginpasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(loginpasswordBox.Password))
            {
                loginpasswordBox.Visibility = Visibility.Hidden; //hides password box when the password box loses focus
                loginpasswordtextBox.Visibility = Visibility.Visible; //shows the writing "password" -- only does this if the password box is empty  
            }      
        }

        private void LogintextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(logintextBox.Text) || logintextBox.Text.ToLower() == "username")
            {
                logintextBox.Clear(); //clears textbox to allow user to type
            }           
        }
        private void LogintextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(logintextBox.Text)) //checks if username is empty when textbox loses focus
            {
                logintextBox.Text = "Username"; //if it is empty show the text "username"
            }
        }

        private void logincreatebutton_Click(object sender, RoutedEventArgs e)
        {
            if (myC.Children.Contains(incorrectlabel))
            {
                myC.Children.Remove(incorrectlabel);
            }
            myC.Children.Remove(logincreatebutton);
            myC.Children.Remove(loginenterbutton);
            myC.Children.Remove(logintextBox);
            myC.Children.Remove(loginpasswordBox);
            myC.Children.Remove(loginpasswordtextBox);            
            LoadCreateLoginUI(IO.createlogintb, 325, 208);          
            LoadCreateLoginUI(IO.createloginpasstb, 325, 245);
            LoadCreateLoginUI(IO.createloginpasschecktb, 325, 282);           
            LoadCreateLoginUIpass(IO.createloginpb, 325, 245);
            LoadCreateLoginUIpass(IO.createloginpbcheck, 325, 282);
            Canvas.SetLeft(IO.createloginenterbutton, 480); Canvas.SetTop(IO.createloginenterbutton, 283);           
            if (!myC.Children.Contains(IO.createloginenterbutton))
            {
                myC.Children.Add(IO.createloginenterbutton);
            }
            returnbutton.Visibility = Visibility.Visible;
            for (int i = 0; i < 2; i++) //Repositions the "Transport App" text
            {
                Canvas.SetLeft(textimages[i], 185 + (i * 310));
                Canvas.SetTop(textimages[i], 100);
            }           

            IO.createlogintb.GotFocus += Createlogintb_GotFocus;
            IO.createlogintb.LostFocus += Createlogintb_LostFocus;
            IO.createloginpb.LostFocus += Createloginpb_LostFocus;
            IO.createloginpasstb.GotFocus += Createloginpasstb_GotFocus;
            IO.createloginpbcheck.LostFocus += Createloginpbcheck_LostFocus;
            IO.createloginpasschecktb.GotFocus += Createloginpasschecktb_GotFocus;
            IO.createloginenterbutton.Click += Createloginenterbutton_Click;
            returnbutton.Click += Returnbutton_Click;
        }

        private void Returnbutton_Click(object sender, RoutedEventArgs e)
        {
            resetcreatelogintb();
            ClearCanvas();
            LoadUI();
        }
        private void resetcreatelogintb()
        {
            IO.createlogintb.Text = "Username";
            IO.createloginpb.Clear();
            IO.createloginpbcheck.Clear();
            IO.createloginpb.Focus();
            IO.createloginpbcheck.Focus();
            Keyboard.ClearFocus();
        }

        private void Createloginenterbutton_Click(object sender, RoutedEventArgs e)
        {
            CreateLoginCheck();
            if (logincreated == true)
            {
                resetcreatelogintb();
                ClearCanvas();
                LoadUI();
            }

        }
        private void CreateLoginCheck() // Allows for a login to be created 
        {
            string check = "error";
            if (String.IsNullOrEmpty(IO.createlogintb.Text) || IO.createlogintb.Text.ToLower() == "username")
            {
                incorrectlabelshow("Must contain a Username");
            }
            else if (String.IsNullOrEmpty(IO.createloginpb.Password))
            {
                incorrectlabelshow("Must contain a Password");
            }
            else if (String.IsNullOrEmpty(IO.createloginpbcheck.Password))
            {
                incorrectlabelshow("Please type Password again");
            }
            else
            {
                if (IO.createlogintb.Text.Contains(",") || IO.createlogintb.Text.Contains("/") || IO.createloginpb.Password.Contains(",") || IO.createloginpb.Password.Contains("/") || IO.createloginpbcheck.Password.Contains(",") || IO.createloginpbcheck.Password.Contains(","))
                {
                    incorrectlabelshow("Cannot use , or /");
                }
                else
                {
                    if (IO.createloginpb.Password != IO.createloginpbcheck.Password)
                    {
                        check = "passerror";
                    }
                    else
                    {
                        check = LoginWindowModel.CreateLogin(IO.createlogintb.Text + ',' + IO.createloginpb.Password);
                    }
                }
            }
            if (check == "success")
            {
                logincorrectlabelshow("Login Successfully created!");
                logincreated = true;
            }
            else if (check == "fail")
                incorrectlabelshow("Login Creation Failed");
            else if (check == "exists")
                incorrectlabelshow("Please choose a different Username");        
            else if (check == "passerror")
                incorrectlabelshow("Passwords do not match");
            else
            {
                incorrectlabelshow("An Error Occured");
                throw new NotImplementedException();
            }
        }

        private void Createloginpasschecktb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IO.createloginpbcheck.Password))
            {
                IO.createloginpasschecktb.Visibility = Visibility.Hidden; //hides the text "password" when password text box becomes focused
                IO.createloginpbcheck.Visibility = Visibility.Visible; //shows the empty password box
            }
            Keyboard.ClearFocus(); //Clears the focus on the text box
            IO.createloginpbcheck.Focus(); //Focuses the password box
        }
        private void Createloginpbcheck_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IO.createloginpbcheck.Password))
            {
                IO.createloginpbcheck.Visibility = Visibility.Hidden; //hides password box when the password box loses focus
                IO.createloginpasschecktb.Visibility = Visibility.Visible; //shows the writing "password" -- only does this if the password box is empty  
            }
        }

        private void Createloginpasstb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IO.createloginpb.Password))
            {
                IO.createloginpasstb.Visibility = Visibility.Hidden; //hides the text "password" when password text box becomes focused
                IO.createloginpb.Visibility = Visibility.Visible; //shows the empty password box
            }
            Keyboard.ClearFocus(); //Clears the focus on the text box
            IO.createloginpb.Focus(); //Focuses the password box
        }
        private void Createloginpb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IO.createloginpb.Password))
            {
                IO.createloginpb.Visibility = Visibility.Hidden; //hides password box when the password box loses focus
                IO.createloginpasstb.Visibility = Visibility.Visible; //shows the writing "password" -- only does this if the password box is empty  
            }
        }

        private void Createlogintb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IO.createlogintb.Text)) //checks if username is empty when textbox loses focus
            {
                IO.createlogintb.Text = "Username"; //if it is empty show the text "username"
            }
        }
        private void Createlogintb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IO.createlogintb.Text) || IO.createlogintb.Text.ToLower() == "username")
            {
                IO.createlogintb.Clear(); //clears textbox to allow user to type
            }
        }

        private void ReturnMainButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWin.Hide();
        }
    }
}
