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
using System.IO;
using System.Windows.Threading;
using System.Globalization;

namespace NEA_A_Lvl_Project
{
    /// <summary>
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>       

    public partial class SelectionWindow : Window
    {
        private Label incorrectlabel = new Label() { Height = 32.0, Width = 250.0, Foreground = Brushes.Red, Background = Brushes.Transparent, FontWeight = FontWeights.SemiBold, VerticalContentAlignment = VerticalAlignment.Center};
        protected ComboBox viacB = new ComboBox() { Width = 124 };
        private DispatcherTimer labeltimer = new DispatcherTimer();
        public bool IsGoingVia;

        public SelectionWindow()
        {
            InitializeComponent();
            Title = "Train Timetable";
            Background = new ImageBrush { ImageSource = new BitmapImage(Settings.background) };
            foreach (string text in Settings.times)
            {
                timecomboBox.Items.Add(text);
            }
            deparrcomboBox.Items.Add("Leaving at:");
            deparrcomboBox.Items.Add("Arriving by:");
            enterButton.Content = "Get Times    >";
            Canvas.SetLeft(incorrectlabel, 125);
            Canvas.SetTop(incorrectlabel, 338);
            Canvas.SetLeft(viacB, 259);
            Canvas.SetTop(viacB, 109);
            fromstationcB.IsEditable = true;
            tostationcB.IsEditable = true;
            viacB.IsEditable = true;
            timecomboBox.SelectedIndex = 0;
            deparrcomboBox.SelectedIndex = 0;
            selectWindow.KeyDown += SelectWindow_KeyDown;
        }

        private void SelectWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ValidateData())
                {
                    SaveData();
                    selectWindow.Close();
                }
                else
                {
                    incorrectlabelshow("Not all inputs correct");
                }
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateData())
            {
                SaveData();
                selectWindow.Close();
            }
            else
            {
                incorrectlabelshow("Not all inputs correct");
            }
        }

        protected virtual void SaveData() //overriden for saving bus data
        {
            TrainSearchData.From = fromstationcB.Text;
            TrainSearchData.To = tostationcB.Text;
            TrainSearchData.DateandTime = datePicker.SelectedDate.Value;
            TimeSpan time = TimeSpan.Parse(timecomboBox.SelectedValue.ToString());
            TrainSearchData.DateandTime += time;
            if (deparrcomboBox.SelectedValue.ToString() == "Leaving at:")
            {
                TrainSearchData.IsDeparting = true;
            }
            else
            {
                TrainSearchData.IsDeparting = false;
            }
            if (IsGoingVia)
            {
                TrainSearchData.Via = viacB.Text;
                TrainSearchData.IsGoingVia = true;
            }
            TrainSearchData.IsAllDataEntered = true;
        }

        private bool ValidateData()
        {
            if (doAllFieldsContainData() && !IsGoingVia) //if the user is not going via a station and all fields contain data, it is valid
                return true;
            else if (doAllFieldsContainData() && IsGoingVia && !string.IsNullOrEmpty(viacB.Text)) //if the user is going via a station and the via Tb is not empty
                return true;
            else
                return false;

            
        }
        private bool doAllFieldsContainData()
        {
            if (!string.IsNullOrEmpty(fromstationcB.Text) && !string.IsNullOrEmpty(tostationcB.Text) && timecomboBox.SelectedValue != null && datePicker.SelectedDate != null && deparrcomboBox.SelectedValue != null)
                return true;
            else
                return false;           
        }

        protected void incorrectlabelshow(string error)
        {
            labeltimer.Tick += Labeltimer_Tick;
            labeltimer.Interval = new TimeSpan(0, 0, 0, 1); //1s
            labeltimer.Start();
            incorrectlabel.Content = error;
            if (!myC.Children.Contains(incorrectlabel))
            {
                myC.Children.Add(incorrectlabel);
            }
        }
        private void Labeltimer_Tick(object sender, EventArgs e)
        {
            if (myC.Children.Contains(incorrectlabel))
            {
                myC.Children.Remove(incorrectlabel);
                labeltimer.Stop();
            }
        }

        private void ViaButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGoingVia)
            {
                IsGoingVia = true;
                CreateViaTB();
            }
            else
            {
                IsGoingVia = false;
                RemoveViaTB();
            }
        }

        private void CreateViaTB()
        {
            if (!myC.Children.Contains(viacB))
            {
                Canvas.SetTop(viaButton, 78);
                viaButton.Content += ":";
                viaButton.Width += 2;
                myC.Children.Add(viacB);
            }
        }
        private void RemoveViaTB()
        {
            if (myC.Children.Contains(viacB))
            {
                myC.Children.Remove(viacB);
                string buttontext = viaButton.Content.ToString();
                viaButton.Content = buttontext.Remove(buttontext.Length -1, 1);
                viaButton.Width -= 2;
                Canvas.SetTop(viaButton, 93);
            }
        }
    }
    
}
