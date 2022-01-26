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
using Newtonsoft.Json;
using ModernWpf;

namespace NEA_A_Lvl_Project //https://download.geonames.org/export/!!!!!
{
    static class urls
    {
        private static string drive = "G:/Computing/";

        public static Uri background { get; set; } = new Uri(drive + "/NEA A Lvl Project/background.jpg"); //Background Image Path
        public static Uri transporttext { get; set; } = new Uri(drive + "/NEA A Lvl Project/transport_text.png"); //Login "Transport" Text Path
        public static Uri apptext { get; set; } = new Uri(drive + "/NEA A Lvl Project/app_text.png"); //Login "App" Text Path
        public static Uri bus { get; set; } = new Uri(drive + "/NEA A Lvl Project/bus.jpeg");
        public static Uri publictrans { get; set; } = new Uri(drive + "/NEA A Lvl Project/public.png");
        public static Uri train { get; set; } = new Uri(drive + "/NEA A Lvl Project/train.png");
        public static Uri roadmap { get; set; } = new Uri(drive + "/NEA A Lvl Project/roadmap.png");

        public static Uri pfp { get; set; } = new Uri(drive + "/NEA A Lvl Project/pfp.png");
        public static Ellipse profile { get; set; } = new Ellipse { Width = 40, Height = 40, Fill = new ImageBrush { ImageSource = new BitmapImage(pfp) }, Stroke = Brushes.Black, StrokeThickness = 2 };
        public static Label usernamelable { get; set; } = new Label { Width = 50, Height = 20, Content = Stats.Username, Foreground = Brushes.Black, VerticalContentAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Center  };
        public static Button logoutbutton { get; set; } = new Button { Width = 90, Height = 34, Content = "Sign Out", BorderBrush = Brushes.Black, BorderThickness = new Thickness(1.5) };

        public static string transportapidomain = "http://transportapi.com/v3"; //http://transportapi.com/v3/uk/places.json?query=euston&type=train_station&app_id=62fd3e78&app_key=49c60d51ca16ad51d9793143750eeb34
        public static string apiid { get; set; } = "62fd3e78"; //"62fd3e78"
        public static string appkey { get; set; } = "49c60d51ca16ad51d9793143750eeb34"; //"49c60d51ca16ad51d9793143750eeb34
        public static string currentstation { get; set; } = "euston";
        public static string timetabletype { get; set; } = "train_station";
        public static string stationcode { get; set; } = "RUG";
        public static DataGrid datagrid { get; set; } = new DataGrid() { Height = 350, Width = 700, Visibility = Visibility.Hidden};
        public static List<string> specificdata = new List<string>();
        public static List<Stations> stations { get; set; } = new List<Stations>();
        public static List<string> namesofstations { get; set; } = new List<string>();
        //public static string domain = 

        public static string usernamedata { get; set; } = "usernameData.txt"; //encrypted username data file path
        public static string passworddata { get; set; } = "passwordData.txt"; //encrypted password data file path
        public const int port = 8001;
        public const string serverIP = "127.0.0.1";
        public static bool loggedin = false;

        public static T downloadjsondata<T>(string url) where T : new()
        {
            var request = WebRequest.Create(url); //Initiates a Webrequest to "Transport API"
            IWebProxy webProxy = WebRequest.DefaultWebProxy; 
            webProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.Proxy = webProxy;
            string data;
            var response = (HttpWebResponse)request.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                data = sr.ReadToEnd();
            }
            return !string.IsNullOrEmpty(data) ? JsonConvert.DeserializeObject<T>(data) : new T();
        }
    }
}
