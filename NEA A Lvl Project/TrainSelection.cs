using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace NEA_A_Lvl_Project
{
    public class TrainSelection : SelectionWindow
    {
        public TrainSelection() : base()
        {
            IsGoingVia = TrainSearchData.IsGoingVia;
            searchProgressBar.Value = 0;
            using (StreamReader streamReader = new StreamReader("Names.txt"))
            {
                do
                {
                    Settings.namesofstations.Add(streamReader.ReadLine());
                }
                while (!streamReader.EndOfStream);
            }
            selectWindow.Closing += SelectWindow_Closing;
        }

        private void SelectWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TrainSearchData.IsAllDataEntered)
            {
                int i = !TrainSearchData.IsGoingVia ? i = 2 : i = 3; //i = number of stations to search
                int[] stationnums = new int[i];
                Stationdata[] stationdata = new Stationdata[i];
                Tuple<int[], Stationdata[]> stationnumbertuples = StationNumbers(i);
                stationnums = stationnumbertuples.Item1;
                stationdata = stationnumbertuples.Item2;
                if (stationnums[0] <= 1 && stationnums[1] <= 1)
                {
                    e.Cancel = false;
                    TrainSearchData.StationCodeStart = stationdata[0].member[0].station_code; //Gets start stations station code
                    TrainSearchData.StationCodeEnd = stationdata[1].member[0].station_code; //Gets end stations station code
                    TrainSearchData.IsTrainSearchReady = true;
                    //close window
                }
                else if (stationnums[0] == 0 || stationnums[1] == 0)
                {
                    e.Cancel = true;
                    incorrectlabelshow("One of your stations dosn't exist");
                }
                else
                {
                    e.Cancel = true;
                    incorrectlabelshow("Pick a Station");
                    MultipleStations(stationnums, stationdata); //!!!!
                }
            }

        }
        private void MultipleStations(int[] stationnumbers, Stationdata[] stationdata)
        {
            fromstationcB.Items.Clear();
            tostationcB.Items.Clear();
            fromstationcB.Text = TrainSearchData.From;
            Settings.stations.Clear();
            for (int i = 0; i < stationdata.Length; i++)
            {
                foreach (StationDetails detail in stationdata[i].member) //NEEDS BREAKPOINTING
                {
                    string station = "";
                    foreach (string name in Settings.namesofstations)
                    {
                        if (detail.name.Contains(name))
                        {
                            station = TrainModel.TrimStationData(name, station);
                            if (string.IsNullOrWhiteSpace(station))
                            {
                                station = name;
                            }
                            else
                            {
                                station = name + " " + station;
                            }
                            break;
                        }
                        station = detail.name;
                    }
                    if (i == 0)
                    { 
                        fromstationcB.Items.Add(station);

                    }
                    else if (i == 1)
                    {
                        tostationcB.Items.Add(station);
                    }
                    else if(i == 2)
                    {
                        viacB.Items.Add(station);
                    }
                    else
                    {
                        throw new Exception(); // test
                    }

                    //Settings.stations[Settings.stations.Count - 1].stationBox.Checked += StationBox_Checked;
                }
            }
            fromstationcB.StaysOpenOnEdit = true;
            fromstationcB.Focus();
        }
        private Tuple<int[],Stationdata[]> StationNumbers(int stationsentered)
        {
            int[] stationnums = new int[stationsentered];
            Stationdata[] stationdata = new Stationdata[stationsentered];
            searchProgressBar.Minimum = 0;
            searchProgressBar.Maximum = stationsentered;
            searchProgressBar.Visibility = Visibility.Visible;
            for (int i = 0; i < stationsentered; i++)
            {
                if (i == 0)
                    stationdata[i] = TrainModel.GetTrainStationdata(TrainSearchData.From);
                else if (i == 1)
                    stationdata[i] = TrainModel.GetTrainStationdata(TrainSearchData.To);
                else if (i == 2)
                    stationdata[i] = TrainModel.GetTrainStationdata(TrainSearchData.Via);
                else
                    throw new NotImplementedException();
                stationnums[i] = stationdata[i].member.Count;
                searchProgressBar.Value = i;
            }
            Duration duration = new Duration(TimeSpan.FromSeconds(2000));
            DoubleAnimation doubleanimation = new DoubleAnimation(200.0, duration);
            searchProgressBar.BeginAnimation(ProgressBar.ValueProperty, doubleanimation); //!
            //if (searchProgressBar.Maximum == stationsentered)
                //searchProgressBar.Visibility = Visibility.Hidden;
            return Tuple.Create(stationnums, stationdata);
        }
    }
    public static class TrainSearchData
    {
            public static string From { get; set; }
            public static string To { get; set; }
            public static DateTime DateandTime { get; set; }
            public static bool IsDeparting { get; set; }
            public static bool IsAllDataEntered { get; set; } = false;
            public static string Via { get; set; }
            public static bool IsGoingVia { get; set; } = false;
            public static bool IsTrainSearchReady { get; set; } = false;
            public static List<string> ServiceNumbersOfMatchingTrains { get; set; } = new List<string>();
            public static string StationCodeStart { get; set; }
            public static string StationCodeEnd { get; set; }
            public static List<Journey> journeydata { get; set; } = new List<Journey>();
            public static void ClearData()
            {
                From = default;
                To = default;
                DateandTime = default;
                IsDeparting = default;
                IsAllDataEntered = false;
                Via = default;
                IsGoingVia = false;
                IsTrainSearchReady = false;
                ServiceNumbersOfMatchingTrains.Clear();
                StationCodeStart = default;
                StationCodeEnd = default;
                journeydata.Clear();
            }
    }
    
}
