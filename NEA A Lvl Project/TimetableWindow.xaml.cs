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
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

namespace NEA_A_Lvl_Project
{
    /// <summary>
    /// Interaction logic for TimetableWindow.xaml
    /// </summary>
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    //||
    public partial class TimetableWindow : Window
    {
        public TimetableWindow()
        {
            InitializeComponent();
            Background = new ImageBrush { ImageSource = new BitmapImage(Settings.background) };
            Canvas.SetTop(Settings.datagrid, 100);
            Canvas.SetLeft(Settings.datagrid, 50);
            if (!myC.Children.Contains(Settings.profile))
            {
                myC.Children.Add(Settings.profile);
            }
            if (!myC.Children.Contains(Settings.datagrid))
            {
                myC.Children.Add(Settings.datagrid);
            }

            Closing += TimetableWindow_Closing;
            Settings.datagrid.MouseLeftButtonUp += Datagrid_MouseLeftButtonUp;
            //data = Settings.downloadjsondata<Traindata>(Settings.transportapidomain + "/uk/train/station/RUG/live.json?app_id=" + Settings.apiid + "&app_key=" + Settings.appkey +"&darwin=false&train_status=passenger");

            //Settings.realtimeupdate.Tick += Realtimeupdate_Tick;
            //Settings.realtimeupdate.Interval = new TimeSpan(0, 1, 0); //realupdatetime
            //Settings.realtimeupdate.Start();

        }

        //private void Realtimeupdate_Tick(object sender, EventArgs e)
        //{
        //    Settings.times[0] = DateTime.UtcNow.ToLocalTime().ToString("HH:mm");
        //    int index = timecomboBox.SelectedIndex;
        //    timecomboBox.Items.Clear();
        //    foreach (var time in Settings.times)
        //    {
        //        timecomboBox.Items.Add(time);
        //    }
        //    timecomboBox.SelectedIndex = index;
        //}
        private void Datagrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Settings.specificdata.Add(Settings.datagrid.SelectedItem.ToString());
            foreach (var item in Settings.specificdata)
            {
                string remove = item.Replace("{", "").Replace("}", "");
                remove = Regex.Replace(remove, @"\s+", ""); //replaces all repeated spaces with one space
                MessageBox.Show(remove);
            }
            Settings.specificdata.Clear();
        }
        private void TimetableWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myC.Children.Remove(Settings.profile); //disconnects the profile picture from timetable window so it can be used in other windows
            //Settings.datagrid.Items.Clear();
            myC.Children.Remove(Settings.datagrid);
            Settings.realtimeupdate.Stop(); //stops the real time update
        }
    }
}
