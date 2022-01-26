using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Globalization;

namespace NEA_A_Lvl_Project
{
    public class Departures
    {
        public List<All> all { get; set; }
    }
    public class Arrivals
    {
        public List<All> all { get; set; }
    }
    public class All
    {
        public string mode { get; set; }
        public string service { get; set; }
        public string train_uid { get; set; }
        public string platform { get; set; }
        public string @operator { get; set; }
        public string operator_name { get; set; }
        public string aimed_departure_time { get; set; }
        public string aimed_arrival_time { get; set; }
        public object aimed_pass_time { get; set; }
        public string origin_name { get; set; }
        public string destination_name { get; set; }
        public string source { get; set; }
        public string category { get; set; }
        public ServiceTimetable service_timetable { get; set; }
        public string status { get; set; }
        public string expected_arrival_time { get; set; }
        public string expected_departure_time { get; set; }
        public string best_arrival_estimate_mins { get; set; }
        public string best_departure_estimate_mins { get; set; }
    }
    public class ServiceTimetable
    {
        public string id { get; set; }
    }
    public class Traindata
    {
        public string date { get; set; }
        public string time_of_day { get; set; }
        public DateTime request_time { get; set; }
        public string station_name { get; set; }
        public string station_code { get; set; }
        public Departures departures { get; set; }
        public Arrivals arrivals { get; set; }
    }
    public class Stations
    {
        public string stationname { get; set; }
        public string stationID { get; set; }
    }
    public class Stationdata
    {
        public DateTime request_time { get; set; }
        public string source { get; set; }
        public string acknowledgements { get; set; }
        public List<StationDetails> member { get; set; }
    }
    public class StationDetails
    {
        public string type { get; set; }
        public string name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int accuracy { get; set; }
        public string station_code { get; set; }
        public string tiploc_code { get; set; }
    }

    public class Stop
    {
        public string station_code { get; set; }
        public string tiploc_code { get; set; }
        public string station_name { get; set; }
        public string stop_type { get; set; }
        public string platform { get; set; }
        public string aimed_departure_date { get; set; }
        public string aimed_departure_time { get; set; }
        public string aimed_arrival_date { get; set; }
        public string aimed_arrival_time { get; set; }
        public object aimed_pass_date { get; set; }
        public object aimed_pass_time { get; set; }
        public string expected_departure_date { get; set; }
        public string expected_departure_time { get; set; }
        public string expected_arrival_date { get; set; }
        public string expected_arrival_time { get; set; }
        public object expected_pass_date { get; set; }
        public object expected_pass_time { get; set; }
        public string status { get; set; }
    }
    public class ServiceData
    {
        public string service { get; set; }
        public string train_uid { get; set; }
        public string headcode { get; set; }
        public string train_status { get; set; }
        public string origin_name { get; set; }
        public string destination_name { get; set; }
        public object stop_of_interest { get; set; }
        public string date { get; set; }
        public string time_of_day { get; set; }
        public string mode { get; set; }
        public DateTime request_time { get; set; }
        public string category { get; set; }
        public string @operator { get; set; }
        public string operator_name { get; set; }
        public List<Stop> stops { get; set; }
    }
    public class Journey
    {
        public ServiceData data { get; set; }
        public DateTime departure { get; set; }
        public DateTime arrival { get; set; }
        public string start_station { get; set; }
        public string destination_station { get; set; }
        public TimeSpan duration { get; set; }
        public string departure_platform { get; set; }
        public string arrival_platform { get; set; }
        public string departure_status { get; set; }
        public double price { get; set; }

        public Journey(ServiceData data, DateTime departure, DateTime arrival, string start_station, string destination_station, string departure_platform, string arrival_platform, string departure_status, double price)
        {
            this.data = data;
            this.departure = departure;
            this.arrival = arrival;
            this.start_station = start_station;
            this.destination_station = destination_station;
            duration = DurationCalc(departure, arrival);
            this.departure_platform = departure_platform;
            this.arrival_platform = arrival_platform;
            this.departure_status = departure_status;
            this.price = price;
        }
        private TimeSpan DurationCalc(DateTime depature, DateTime arrival)
        {
            return arrival - depature; //calculates duration
        }
    }

    class TrainTimetable : TimetableWindow
    {
        Dictionary<int, string> trainid = new Dictionary<int, string>();
        public TrainTimetable() : base()
        {
            TrainSelection trainSelection = new TrainSelection();
            trainSelection.Show();
            trainSelection.Topmost = true;
            trainSelection.Closed += TrainSelection_Closed; 
            KeyDown += TrainTimetable_KeyDown;
            Closed += (sender, e) => TrainTimetable_Closed(sender, e, trainSelection);
        }

        private void TrainTimetable_Closed(object sender, EventArgs e, TrainSelection window)
        {
            window.Close();
        }

        private void Trainlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trainid.TryGetValue(trainlist.SelectedIndex, out string id))
            {
                ServiceData serviceData = new ServiceData();
                serviceData = TrainModel.GetServiceData(id, TrainSearchData.DateandTime);
                string calling = "Calling at: ";
                bool started = false, ended = false;
                foreach (var stop in serviceData.stops)
                {                
                    started = started ? true : stop.station_code == TrainSearchData.StationCodeStart; //if start = false then check if it need to be true
                    if (started)
                    {
                        if (!ended) calling += stop.station_name + " ";
                        ended = ended ? true : !(stop.station_code == TrainSearchData.StationCodeEnd);
                    }                    
                }
                MessageBox.Show(calling);
            }
        }

        private void TrainSelection_Closed(object sender, EventArgs e)
        {
            if (TrainSearchData.IsTrainSearchReady) //Run Train Search
              Displaytraindata(TrainModel.FindTrains());                         
            else
                timetableWin.Close();
            
        }

        private void TrainTimetable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //DisplayStations();
            }
        }

        private void Displaytraindata(Traindata data)
        {
            //Settings.datagrid.HorizontalContentAlignment = HorizontalAlignment.Center;
            //Settings.datagrid.ItemsSource = data.departures.all.Select(x => new {
            //    Arrives = x.expected_arrival_time,
            //    Departs = x.aimed_departure_time,
            //    Platform = x.platform,
            //    From = x.origin_name,
            //    At = data.station_name,
            //    To = x.destination_name,
            //    By = x.operator_name,
            //    Live = x.status,
            //    Estimated = x.best_arrival_estimate_mins + " mins"}).ToList();           
            int i = 0; 
            TrainSearchData.journeydata.Sort((x, y) => x.arrival.CompareTo(y.arrival));
            foreach (var journey in TrainSearchData.journeydata)
            { 
                    GroupBox train = new GroupBox()
                    {
                        Header = new Label()
                        {
                            Content = journey.departure.ToString("HH:mm") + "   -->   " + journey.arrival.ToString("HH:mm") + "\t\t\t\t" + "£" + string.Format("{0:0.00}", journey.price),
                            FontSize = 15,
                            FontWeight = FontWeights.SemiBold
                        }
                    };
                    train.Content = new TextBlock()
                    {
                    Text = journey.departure_status + Environment.NewLine
                    + "Platform: " + journey.departure_platform + "   " + "Arriving at Platform: " + journey.arrival_platform + Environment.NewLine
                    + journey.duration.ToString("c") + Environment.NewLine
                    + string.Concat(journey.start_station + "   -->   " + journey.destination_station) + Environment.NewLine
                    + "Press for stops"
                    };
                trainlist.Items.Add(train);
                trainid.Add(i, journey.data.train_uid);
                    i++;
            }
            trainlist.SelectionChanged += Trainlist_SelectionChanged;
        }
       
    }
    public class TrainModel
    {
        public static string TrimStationData(string originalname, string stationname) //NEEDS BREAKPOINTING
        {
            stationname = stationname.Replace(originalname, "").Replace("()", ""); //takes the orginal and clears it as well as any brackets
            stationname = Regex.Replace(stationname, "\\(.*\\)", ""); //
            stationname = Regex.Replace(stationname, "\\s+", " "); //Cuts any whitespaces with just one space
            stationname = stationname.Trim(); //removes any other spaces left over
            return stationname;
        }

        public static Traindata FindTrains() //using expected times
        {
            Random rnd = new Random(); //for the price **temp**
            bool direct = false;
            ServiceData serviceData = new ServiceData();
            Traindata departures = new Traindata();
            Traindata arrivals = new Traindata();
            if (TrainSearchData.IsDeparting)
            {   
                departures = GetTrainData(TrainSearchData.StationCodeStart, TrainSearchData.DateandTime, true, new TimeSpan(12, 0, 0)); //Searches for trains departing from start stations within 2hs
                arrivals = GetTrainData(TrainSearchData.StationCodeEnd, TrainSearchData.DateandTime, false, new TimeSpan(12, 0, 0)); //Searches trains arriving at end station within 4hs total
            }
            else
            {
                departures = GetTrainData(TrainSearchData.StationCodeStart, TrainSearchData.DateandTime - new TimeSpan(1,0,0), true, new TimeSpan(12, 0, 0)); //Searches for trains departing from start stations 4hs before
                arrivals = GetTrainData(TrainSearchData.StationCodeEnd, TrainSearchData.DateandTime - new TimeSpan(1,0,0), false, new TimeSpan(12, 0, 0)); //Searches Trains arriving 2hs before desired time
            }
            Dictionary<string, Dictionary<string, bool>> departureIDs = new Dictionary<string, Dictionary<string, bool>>();
            if (departures.departures.all.Count == 0)
                MessageBox.Show("No Trains Availiable"); //If there are no trains at all display error
            else
            {
                foreach (var departure in departures.departures.all)
                {
                    if (departureIDs.TryGetValue(departure.service, out Dictionary<string, bool> trainidDic))
                        trainidDic.Add(departure.train_uid, false);
                    else
                        departureIDs.Add(departure.service, new Dictionary<string, bool> { { departure.train_uid, false } });
                }
                foreach (var arrival in arrivals.arrivals.all)
                {
                    if (departureIDs.TryGetValue(arrival.service, out Dictionary<string, bool> trainidDic))
                    {
                        if (trainidDic.ContainsKey(arrival.train_uid))
                        {
                            //Log train as found (it arrives at the desitination station)
                            serviceData = GetServiceData(arrival.train_uid, TrainSearchData.DateandTime);
                            DateTime startDateTime = default;
                            string departureplatform = "", departurestatus = "";
                            for (int i = 0; i < serviceData.stops.Count; i++)
                            {
                                if (serviceData.stops[i].station_code == TrainSearchData.StationCodeStart && i < serviceData.stops.Count - 1)   //If the date/time the train departes is after/equal to the time user entered dont; also dont check last station as you cannot leave it
                                {
                                    departureplatform = serviceData.stops[i].platform;
                                    departurestatus = serviceData.stops[i].status;
                                    startDateTime = DateTime.Parse(string.Concat(
                                    serviceData.stops[i].expected_departure_date == null ? serviceData.stops[i].aimed_departure_date : serviceData.stops[i].expected_departure_date,
                                    " ",
                                    serviceData.stops[i].expected_departure_time == null ? serviceData.stops[i].aimed_departure_time : serviceData.stops[i].expected_departure_time)); //if expected time unavaliable, use aimed times
                                    int test = DateTime.Compare(startDateTime, TrainSearchData.DateandTime);
                                    //if (DateTime.Compare(startDateTime, TrainSearchData.DateandTime) < 0)// if in past then ignore
                                    //{
                                    //    startDateTime = default;
                                    //}
                                }
                                if (serviceData.stops[i].station_code == TrainSearchData.StationCodeEnd && startDateTime != default) //If a train calls at the end station and is arrives after start time, add that service number to a list
                                {
                                    string arrivalplatform = serviceData.stops[i].platform;
                                    DateTime arrivalDateTime = DateTime.Parse(string.Concat(
                                    serviceData.stops[i].expected_arrival_date == null ? serviceData.stops[i].aimed_arrival_date : serviceData.stops[i].expected_arrival_date,
                                    " ",
                                    serviceData.stops[i].expected_arrival_time == null ? serviceData.stops[i].aimed_arrival_time : serviceData.stops[i].expected_arrival_time)); ;

                                    Journey journey = new Journey(serviceData, startDateTime, arrivalDateTime, TrainSearchData.From, TrainSearchData.To, departureplatform, arrivalplatform, departurestatus, rnd.Next(10,46));
                                    TrainSearchData.journeydata.Add(journey);
                                    direct = true;
                                    break;
                                }
                            }                          
                        }
                    }
                }
                if (!direct)
                    MessageBox.Show("No Direct Trains"); //Give User Option to Search later trains
            }
            return departures;
        }
        //public static Traindata FindTrains() //using expected times
        //{
        //    bool direct = false;
        //    ServiceData serviceData = new ServiceData();
        //    TrainSearchData.StationCodeStart = GetTrainStationCode(TrainSearchData.From); //Gets start stations station code
        //    TrainSearchData.StationCodeEnd = GetTrainStationCode(TrainSearchData.To); //Gets end stations station code
        //    Traindata trains = GetTrainData(TrainSearchData.StationCodeStart, TrainSearchData.DateandTime); //Searches for trains departing from start stations
        //    if (trains.departures.all.Count == 0)
        //        MessageBox.Show("No Trains Availiable"); //If there are no trains at all display error
        //    else
        //    {
        //        foreach (var train in trains.departures.all) //Searches every train departing from the start station for stops
        //        {
        //            serviceData = GetServiceData(train.train_uid, TrainSearchData.DateandTime);
        //            DateTime startDateTime = default;
        //            string departureplatform = "", departurestatus = "";
        //            for (int i = 0; i < serviceData.stops.Count; i++)
        //            {


        //                if (serviceData.stops[i].station_code == TrainSearchData.StationCodeStart && i < serviceData.stops.Count - 1)   //If the date/time the train departes is after/equal to the time user entered dont; also dont check last station as you cannot leave it
        //                {
        //                    departureplatform = serviceData.stops[i].platform;
        //                    departurestatus = serviceData.stops[i].status;
        //                    startDateTime = DateTime.Parse(string.Concat(
        //                    serviceData.stops[i].expected_departure_date == null ? serviceData.stops[i].aimed_departure_date : serviceData.stops[i].expected_departure_date,
        //                    " ",
        //                    serviceData.stops[i].expected_departure_time == null ? serviceData.stops[i].aimed_departure_time : serviceData.stops[i].expected_departure_time)); //if expected time unavaliable, use aimed times
        //                    int test = DateTime.Compare(startDateTime, TrainSearchData.DateandTime);
        //                    if (DateTime.Compare(startDateTime, TrainSearchData.DateandTime) < 0)// if in past then ignore
        //                    {
        //                        startDateTime = default;
        //                    }
        //                }
        //                if (serviceData.stops[i].station_code == TrainSearchData.StationCodeEnd && startDateTime != default) //If a train calls at the end station and is arrives after start time, add that service number to a list
        //                {
        //                    string arrivalplatform = serviceData.stops[i].platform;
        //                    DateTime arrivalDateTime = DateTime.Parse(string.Concat(
        //                    serviceData.stops[i].expected_arrival_date == null ? serviceData.stops[i].aimed_arrival_date : serviceData.stops[i].expected_arrival_date,
        //                    " ",
        //                    serviceData.stops[i].expected_arrival_time == null ? serviceData.stops[i].aimed_arrival_time : serviceData.stops[i].expected_arrival_time)); ;

        //                    Journey journey = new Journey(serviceData, startDateTime, arrivalDateTime, TrainSearchData.From, TrainSearchData.To, departureplatform, arrivalplatform, departurestatus);
        //                    TrainSearchData.journeydata.Add(journey);
        //                    direct = true;
        //                    break;
        //                }
        //            }
        //            /// start time = scheduled time
        //            /// arrival time = expected time
        //            /// if !cancelled
        //            /// if expected_arrival => start time 
        //        }
        //        if (!direct)
        //            MessageBox.Show("No Direct Trains");
        //    }
        //    return trains;
        //}
        public static Traindata GetTrainData(string stationcode, DateTime date, bool IsDeparting, TimeSpan TimeWindow = default)
        {
            Traindata jsonData;

            if (TimeWindow == default)
                TimeWindow = new TimeSpan(2, 0, 0); //Sets default timespan to 2h
            jsonData = Settings.GetJsonData<Traindata>(string.Concat(new string[] {Settings.transportapidomain,"/uk/train/station/",stationcode,"/",date.ToString("yyyy-MM-dd"),"/", date.ToString("HH:mm"), "/timetable.json?app_id=",Settings.GetApiInfo("id"), "&app_key=",Settings.GetApiInfo("key"), "&darwin=false&train_status=passenger", !IsDeparting ? "&type=arrival" : "", "&to_offset=PT" , TimeWindow.ToString()}));        
            return jsonData;
        }

        public static ServiceData GetServiceData(string serviceID, DateTime date)
        {
            ServiceData jsonData;
            jsonData = Settings.GetJsonData<ServiceData>(string.Concat(new string[] { Settings.transportapidomain, "/uk/train/service/train_uid:",serviceID,"/", date.ToString("yyyy-MM-dd"), "/timetable.json?app_id=", Settings.GetApiInfo("id"), "&app_key=", Settings.GetApiInfo("key"), "&darwin=false&live=true" }));
            return jsonData;
        }

        public static Stationdata GetTrainStationdata(string stationname)
        {            
            return Settings.GetJsonData<Stationdata>(string.Concat(new string[]{Settings.transportapidomain,"/uk/places.json?query=",stationname,"&type=",Settings.timetabletype,"&app_id=",Settings.GetApiInfo("id"),"&app_key=",Settings.GetApiInfo("key")}));
        }

        public static string GetTrainStationCode(string stationname)
        {
            Stationdata stationdata = GetTrainStationdata(stationname);
            string stationcode = "";
            foreach (var s in stationdata.member)
            {
                stationcode = s.station_code;
            }
            return stationcode;
        }
    }

}
