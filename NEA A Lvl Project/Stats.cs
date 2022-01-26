using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_A_Lvl_Project
{

    public class Statsdata
    {
        public string Username { get; set; }
        public string name { get; set; }
        public decimal totalkmtravelled { get; set; }
        public decimal totalmoneyspent { get; set; }
        public int numofjourneys { get; set; }
        public TimeSpan timespenttravelling { get; set; }
        public Uri PfpUri { get; set; } 
    }
    public static class Stats
    {
        private static Statsdata statsdata = new Statsdata();
        public static string GetUsername()
        {
            return statsdata.Username;
        }
        public static void SetName(string name)
        {
            statsdata.name = name;
        }
        public static void SetData(Statsdata data)
        {
            statsdata = data;
        }
        public static Statsdata GetData() 
        {
            return statsdata;
        }
        public static List<string> GetDataAsList()
        {
            return new List<string>() {
                statsdata.name,
                statsdata.numofjourneys.ToString(),
                statsdata.totalkmtravelled.ToString() + " km",
                "£" + statsdata.totalmoneyspent.ToString(),
                string.Format("{0:%h} h {0:%m} mins", statsdata.timespenttravelling)};
        }
    }
}
