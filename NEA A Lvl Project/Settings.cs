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
using System.Net;
using System.Threading;
using System.IO;
using System.Windows.Threading;
using Newtonsoft.Json;
using ModernWpf;


namespace NEA_A_Lvl_Project //https://download.geonames.org/export/!!!!!
{
    static class Settings
    {
        private static string location = Directory.GetCurrentDirectory();

        public static Uri background { get; set; } = new Uri(location + @"\background.jpg"); //Background Image Path
        public static Uri transporttext { get; set; } = new Uri(location + @"\transport_text.png"); //Login "Transport" Text Path
        public static Uri apptext { get; set; } = new Uri(location + @"\app_text.png"); //Login "App" Text Path
        public static Uri bus { get; set; } = new Uri(location + @"\bus.jpeg");
        public static Uri publictrans { get; set; } = new Uri(location + @"\public.png");
        public static Uri train { get; set; } = new Uri(location + @"\train.png");
        public static Uri roadmap { get; set; } = new Uri(location + @"\roadmap.png");

        public static Uri pfp { get; set; } = new Uri(location + @"\pfp.png");
        public static Ellipse profile { get; set; } = new Ellipse { Width = 40, Height = 40, Fill = new ImageBrush { ImageSource = new BitmapImage(pfp) }, Stroke = Brushes.Black, StrokeThickness = 2 };
        public static Label usernamelable { get; set; } = new Label { Width = 50, Height = 20, Foreground = Brushes.Black, VerticalContentAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Center  };
        public static Button logoutbutton { get; set; } = new Button { Width = 90, Height = 34, Content = "Sign Out", BorderBrush = Brushes.Black, BorderThickness = new Thickness(1.5) };
        public static Grid StatsGrid { get; set; } = new Grid() { Width = 400, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
        public static string CurrentLocation { get; set; } = "menu"; //menu, profile, login, logincreate, trainselect, traintimetable, map

        public static string transportapidomain = "http://transportapi.com/v3"; //http://transportapi.com/v3/uk/places.json?query=euston&type=train_station&app_id=62fd3e78&app_key=49c60d51ca16ad51d9793143750eeb34
        private static string apiid = "62fd3e78"; //"62fd3e78"
        private static string appkey = "49c60d51ca16ad51d9793143750eeb34"; //"49c60d51ca16ad51d9793143750eeb34"
        public static string timetabletype { get; set; } = "train_station";
        public static string stationcode { get; set; } = "";
        public static DataGrid datagrid { get; set; } = new DataGrid() { Height = 350, Width = 700, Visibility = Visibility.Hidden};
        public static ListBox timetable = new ListBox() { Height = 350, Width = 700, Visibility = Visibility.Hidden};
        public static List<string> specificdata = new List<string>();
        public static List<Stations> stations { get; set; } = new List<Stations>();
        public static List<string> namesofstations { get; set; } = new List<string>();
        public static List<string> times { get; set; } = new List<string>();
        public static DispatcherTimer realtimeupdate { get; set; } = new DispatcherTimer();

        public static string usernamedata { get; set; } = "usernameData.txt"; //encrypted username data file path
        public static string passworddata { get; set; } = "passwordData.txt"; //encrypted password data file path
        public const int port = 8001;
        public const string serverIP = "127.0.0.1";
        public static bool loggedin = false;

        public static string GetApiInfo(string idOrKey)
        {
            using (StreamReader _reader = new StreamReader("api" + idOrKey.ToUpper() +".txt"))
            {
                return DecodeFrom64(_reader.ReadLine());
            }
        }
        private static void SetApiInfo(string idOrKey) //used for setting the api key and ID
        {
            using (StreamWriter _writer = new StreamWriter("api" + idOrKey.ToUpper() + ".txt"))
            {
                if (idOrKey.ToUpper() == "ID")
                {
                    _writer.Write(EncodeTo64(apiid));
                }
                else
                {
                    _writer.Write(EncodeTo64(appkey));
                }
            }
        }
        static private string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = Encoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        static private string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = Convert.FromBase64String(encodedData);
            string returnValue =
               Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        private static T downloadjsondata<T>(string url) where T : new()
        {
            var request = WebRequest.Create(url); //Initiates a Webrequest to "Transport API"
            IWebProxy webProxy = WebRequest.DefaultWebProxy; 
            webProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.Proxy = webProxy; //requests the data of Website
            string data = "";
            try
            {
                var response = (HttpWebResponse)request.GetResponse(); //recieves the response as a string 
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    data = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.GetType());
            }

            return !string.IsNullOrEmpty(data) ? JsonConvert.DeserializeObject<T>(data) : new T(); //converts the data from JSON format to string if the is somthing there
        }
        public static T GetJsonData<T>(string url) where T : new()
        {
            return Settings.downloadjsondata<T>(url);
        }
        public static void SetTime()
        {
            DateTime dateTime = new DateTime();
            dateTime.Add(new TimeSpan(0, 0, 0));
            Settings.times.Add(DateTime.UtcNow.ToLocalTime().ToString("HH:mm"));
            for (int i = 0; i < 48; i++)
            {
                Settings.times.Add(dateTime.ToString("HH:mm"));
                dateTime = dateTime.Add(new TimeSpan(0, 30, 0));
            }
        }
    }

}
