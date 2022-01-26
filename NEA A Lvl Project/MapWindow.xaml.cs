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
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using System.Globalization;

namespace NEA_A_Lvl_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class Tags
    {
        public string crossing { get; set; }
        public string highway { get; set; }
        public string bus { get; set; }
        public string name { get; set; }

        [JsonProperty("naptan:AtcoCode")]
        public string NaptanAtcoCode { get; set; }

        [JsonProperty("naptan:Bearing")]
        public string NaptanBearing { get; set; }

        [JsonProperty("naptan:CommonName")]
        public string NaptanCommonName { get; set; }

        [JsonProperty("naptan:Indicator")]
        public string NaptanIndicator { get; set; }

        [JsonProperty("naptan:Landmark")]
        public string NaptanLandmark { get; set; }

        [JsonProperty("naptan:NaptanCode")]
        public string NaptanNaptanCode { get; set; }

        [JsonProperty("naptan:Notes")]
        public string NaptanNotes { get; set; }

        [JsonProperty("naptan:Street")]
        public string NaptanStreet { get; set; }

        [JsonProperty("naptan:verified")]
        public string NaptanVerified { get; set; }
        public string public_transport { get; set; }
        public string source { get; set; }
        public string alt_name { get; set; }
        public string railway { get; set; }
        public string gauge { get; set; }

        [JsonProperty("naptan:AltCommonName")]
        public string NaptanAltCommonName { get; set; }
        public string bench { get; set; }
        public string shelter { get; set; }
        public string note { get; set; }

        [JsonProperty("naptan:BusStopType")]
        public string NaptanBusStopType { get; set; }
        public string exit_to { get; set; }
        public string @ref { get; set; }

        [JsonProperty("crossing:island")]
        public string CrossingIsland { get; set; }

        [JsonProperty("traffic_signals:direction")]
        public string TrafficSignalsDirection { get; set; }
        public string bridge { get; set; }
        public string core { get; set; }
        public string junction { get; set; }
        public string layer { get; set; }
        public string lit { get; set; }
        public string maxspeed { get; set; }

        [JsonProperty("maxspeed:type")]
        public string MaxspeedType { get; set; }

        [JsonProperty("source:maxspeed")]
        public string SourceMaxspeed { get; set; }
    }
    public class Element
    {
        public string type { get; set; }
        public double id { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public Tags tags { get; set; }
        public List<object> nodes { get; set; }
        public List<Line> line { get; set; } = new List<Line>();
        //public List<System.Windows.Shapes.Path> path { get; set; } = new List<System.Windows.Shapes.Path>(); 
        public Ellipse ellipse { get; set; } = new Ellipse();
        public double length { get; set; }
        public List<double> ways { get; set; } = new List<double>(); //ways id that node is on
    }
    public class Root
    {
        public List<Element> elements { get; set; }
    }
    public class GeoData
    {
        public string type { get; set; }
        public string name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int accuracy { get; set; }
    }
    public class Geocode
    {
        public List<GeoData> member { get; set; }
    }
    public partial class MapWindow : Window //https://overpass-turbo.eu/
    {
        //resizing!!!
        public Root data { get; set; } = new Root();
        public MyVisualHost visualHost = new MyVisualHost();
        private VisualCollection _children;
        double latitude = 52.3709;//52.4560; //52.3709; //52.4560; //51.5072;
        double longitude = -1.2650;//-1.1992; //-1.2650; //-1.1992; //-0.1276;
        double horizontaldis = 0.08; //9km : 0.08
        double verticaldis = 0.04; //4.5km : 0.04
        double scaleX = 1;
        double scaleY = 1;
        Stopwatch timerapi = new Stopwatch(), timerload = new Stopwatch(), timerzoom = new Stopwatch();

        public MapWindow()
        {
            InitializeComponent();
            LoadMap();
            myC.Children.Add(visualHost);
            Background = Brushes.Black;
        }

        public void LoadMap()
        {
            //string location = Directory.GetCurrentDirectory();
            //Uri background = new Uri(location + @"\world.png");
            //Background = new ImageBrush { ImageSource = new BitmapImage(background)};
            searchButton.Click += SearchButton_Click;
            Uri url = new Uri("https://overpass-api.de/api/interpreter?data=[out:json][timeout:60][bbox:" +
                (latitude - (verticaldis / 2)) + "," + //south
                (longitude - (horizontaldis / 2)) + "," + //west
                (latitude + (verticaldis / 2)) + "," + //north
                (longitude + (horizontaldis / 2)) + //east
                "];(nwr[\"highway\"];);out body;>;out skel qt;"); //nwr[\"railway\"];
                                                                  //https://overpass-api.de/api/interpreter?data=[out:json][timeout:30];area[name=\"United Kingdom\"]->.country;area[\"name\"=\"Lutterworth\"]->.city;nwr(area.country)(area.city)[highway];(._;>;);out;");
                                                                  //            [out: json] [timeout:30];
                                                                  //              area["name" = "United Kingdom"]->.country;
                                                                  //              area["name" = "Lutterworth"]->.city;
                                                                  //              nwr(area.country)(area.city)[highway];
                                                                  //              (._;>;);
                                                                  //              out;
            timerapi.Start();
            data = Processes.GetData(url);
            timerapi.Stop();
            MessageBox.Show(timerapi.Elapsed.ToString());
            timerapi.Reset();
            timerload.Start();
            //data = process.GetRoadData("Lutterworth.json");
            Dictionary<double, Element> dic = new Dictionary<double, Element>();
            Point topleft = Processes.MercatorProj(latitude + (verticaldis / 2), longitude - (horizontaldis / 2)); //top left coord
            Point bottomright = Processes.MercatorProj(latitude - (verticaldis / 2), longitude + (horizontaldis / 2)); //bottom right coord
            double minx = topleft.X, miny = topleft.Y;
            double maxx = bottomright.X, maxy = bottomright.Y;

            double sfx = SystemParameters.PrimaryScreenWidth / (maxx - minx), sfy = SystemParameters.PrimaryScreenHeight / (maxy - miny); //world length
            sfx = sfy;
            for (int i = data.elements.Count - 1; i >= 0; i--) //Is backwards because nodes appear at the end of the json file
            {
                if (!dic.ContainsKey(data.elements[i].id)) //if the key dosnt already exist, add the element to a dictionary with its KEY: id
                {
                    dic.Add(data.elements[i].id, data.elements[i]);
                }
                if (data.elements[i].type == "node")
                {
                    if (data.elements[i].tags != null)
                    {
                        if (data.elements[i].tags.highway != null && data.elements[i].tags.highway == "bus_stop") //if the tag of the node != null, for every "bus stop" place a red circle
                        {
                            Point points = Processes.MercatorProj(data.elements[i].lat, data.elements[i].lon); //gets coords from lon/lat to display on 2d plane
                            //Ellipse rec = new Ellipse { Width = 10, Height = 10, Fill = Brushes.Red };
                            //Canvas.SetLeft(rec, (points.X - minx) * sfx);
                            //Canvas.SetTop(rec, (points.Y - miny) * sfy);
                            //myC.Children.Add(rec);
                            //data.elements[i].ellipse = rec;
                            points.X = (points.X - minx) * sfx;
                            points.Y = (points.Y - miny) * sfy;
                            //visualHost.Ellipse(Brushes.Red,points,4,4);
                        }
                    }
                }
                else if (data.elements[i].type == "way")
                {
                    double waylength = 0;
                    for (int j = 0; j < data.elements[i].nodes.Count - 1; j++)
                    {
                        long id1 = Convert.ToInt64(data.elements[i].nodes[j]); //gets the id of the first node
                        long id2 = Convert.ToInt64(data.elements[i].nodes[j + 1]); //gets the id of the second node
                        Point pos1 = new Point(0, 0), pos2 = new Point(0, 0);
                        double x1 = minx, y1 = miny, x2 = minx, y2 = miny;
                        if (dic.TryGetValue(id1, out var e))
                        {
                            pos1 = Processes.MercatorProj(e.lat, e.lon); //converts the lat/lon coords to Mercator points, to map it on a 2d plane
                            e.ways.Add(data.elements[i].id); //add current ways to list of ways that the node is on
                            if (e.ways.Count == 2) //if the node is on more than one way, it must be a JUNCTION - adds a pink circle to confirm this
                            {
                                Point points = Processes.MercatorProj(e.lat, e.lon);
                                //Ellipse rec = new Ellipse { Width = 10, Height = 10, Fill = Brushes.Magenta };
                                //Canvas.SetLeft(rec, (points.X - minx) * sfx);
                                //Canvas.SetTop(rec, (points.Y - miny) * sfy);
                                //myC.Children.Add(rec);
                                //e.ellipse = rec;
                                points.X = (points.X - minx) * sfx;
                                points.Y = (points.Y - miny) * sfy;
                                visualHost.Ellipse(Brushes.Magenta, points, 2, 2);
                            }
                        }
                        if (dic.TryGetValue(id2, out var f))
                        {
                            pos2 = Processes.MercatorProj(f.lat, f.lon); //converts the lat/lon coords to Mercator points, to map it on a 2d plane
                        }
                        if (e != null && f != null)
                            waylength += Processes.HaversineDist(e.lat, f.lat, e.lon, f.lon);

                        //Line line = new Line() //Creates a line using the first and second nodes coords - scales this accordingly using the sf to fit the screen
                        pos1.X = (pos1.X - minx) * sfx;
                        pos1.Y = (pos1.Y - miny) * sfy;
                        pos2.X = (pos2.X - minx) * sfx;
                        pos2.Y = (pos2.Y - miny) * sfy;

                        double thickness = 1;
                        Brush colour = Brushes.Black;
                        DashStyle dash = null;
                        if (data.elements[i].tags != null)//***
                        {
                            switch (data.elements[i].tags.highway)
                            {
                                default:
                                    thickness = 2;
                                    colour = Brushes.White;
                                    break;
                                //case "motorway_link":
                                case "motorway":
                                    thickness = 6;
                                    colour = Brushes.Blue;
                                    break;
                                //case "trunk_link":
                                case "trunk":
                                    thickness = 5;
                                    colour = Brushes.Green;
                                    break;
                                //case "primary_link":
                                case "primary":
                                    thickness = 4;
                                    colour = Brushes.Red;
                                    break;
                                //case "secondary_link":
                                case "secondary":
                                    thickness = 3;
                                    colour = Brushes.Yellow;
                                    break;
                                //case "tertiary_link":
                                case "tertiary":
                                    thickness = 2;
                                    colour = Brushes.Orange;
                                    break;
                                case "residential":
                                case "living_street":
                                case "service":
                                case "unclassified":
                                    thickness = 1;
                                    colour = Brushes.White;
                                    break;
                                case "line":
                                case "pedestrian":
                                    thickness = 1;
                                    colour = Brushes.Magenta;
                                    break;
                                case "bridleway":
                                case "track":
                                    thickness = 1;
                                    colour = Brushes.Brown;
                                    break;
                            }
                            switch (data.elements[i].tags.railway)
                            {
                                default:
                                    break;
                                case "rail":
                                    thickness = 2;
                                    dash = DashStyles.Dash;
                                    break;
                                case "light_rail":
                                case "tram":
                                case "subway":
                                    thickness = 1;
                                    colour = Brushes.Red;
                                    dash = DashStyles.Dot;
                                    break;
                                case "narrow_gauge":
                                    thickness = 0.5;
                                    dash = DashStyles.DashDot;
                                    break;
                            }
                            //data.elements[i].line.Add(line);//adds the related line to the data.elements[i]
                            visualHost.Line(colour, thickness / 2, pos1, pos2, dash);

                        }
                    }
                    //MessageBox.Show(waylength * 1000 + "m");
                    double waylengthInm = Math.Round(waylength, 2);
                    Point midpoint = new Point(0, 0);
                    if (dic.TryGetValue(Convert.ToInt64(data.elements[i].nodes[(data.elements[i].nodes.Count - 1) / 2]), out var p))
                    {
                        midpoint = Processes.MercatorProj(p.lat, p.lon);
                    }
                    double midx = (midpoint.X - minx) * sfx;
                    double midy = (midpoint.Y - miny) * sfx;
                    Label distlable = new Label() { Foreground = Brushes.White, FontWeight = FontWeights.DemiBold };
                    Canvas.SetLeft(distlable, midx);
                    Canvas.SetTop(distlable, midy);
                    distlable.Content = waylengthInm + "km";
                    if (waylength > 0.5)
                    {
                        //myC.Children.Add(distlable);
                    }
                }
            }
            //timerload.Stop();
            //MessageBox.Show(timerload.Elapsed.ToString());
            //timerload.Reset();
            MouseWheel += MyWin_MouseWheel;
            //PreviewMouseDown += MainWindow_PreviewMouseDown;
        }

        private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Transform(e.GetPosition(myC));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Geocode geocode = new Geocode();
            try
            {
                geocode = Processes.GetGeoData(SearchBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.Source + ex.StackTrace);
            }
            if (geocode.member == null || geocode.member.Count == 0)
            {
                MessageBox.Show("Input correct postcode");
                Close();
            }
            else
            {
                Label label = new Label() { Content = geocode.member[0].accuracy + "% Accurate", Foreground=Brushes.White};
                Canvas.SetLeft(label, 1700);
                Canvas.SetTop(label, 25);
                if(!myC.Children.Contains(label))myC.Children.Add(label);
                latitude = geocode.member[0].latitude; //should be only one postcode in the list, so should be first index
                longitude = geocode.member[0].longitude;
                visualHost.Clear();
                LoadMap();
            }
        }

        private void MyWin_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            timerzoom.Start();
            if (e.Delta >= 1)
            {
                ZoomIn(e.Delta, e.GetPosition(myC));
            }
            else if (e.Delta <= -1)
            {
                ZoomOut(-e.Delta, e.GetPosition(myC));
            }
            //timerzoom.Stop();
            //MessageBox.Show(timerzoom.Elapsed.ToString());
            //timerzoom.Reset();
        }
        public void ZoomIn(double sf, Point pos)
        {
            scaleX = 1.1;
            scaleY = 1.1;
            //ScaleTransform scale = new ScaleTransform(scaleX, scaleY, pos.X, pos.Y);
            //myC.RenderTransform = scale;
            //Point pos2 = Mouse.GetPosition(myC);
            //TranslateTransform transform = new TranslateTransform(pos2.X - pos.X, pos2.Y - pos.Y);
            //myC.RenderTransform = new TransformGroup() { Children = { transform, scale} };
            var matrixTrans = visualHost.RenderTransform as MatrixTransform;
            var matrix = matrixTrans.Matrix;
            matrix.ScaleAt(scaleX, scaleY, pos.X, pos.Y);
            matrixTrans = new MatrixTransform(matrix);
            visualHost.RenderTransform = matrixTrans;
        }
        public void ZoomOut(double sf, Point pos) //Could use Linear Interpolation to smooth
        {
            scaleX = 1 / 1.1;
            scaleY = 1 / 1.1;
            //ScaleTransform scale = new ScaleTransform(scaleX, scaleY, pos.X, pos.Y);
            //myC.RenderTransform = scale;
            //Point pos2 = Mouse.GetPosition(myC);
            //TranslateTransform transform = new TranslateTransform(pos2.X - pos.X, pos2.Y - pos.Y);
            //myC.RenderTransform = new TransformGroup() { Children = { transform, scale } };
            var matrixTrans = visualHost.RenderTransform as MatrixTransform;
            var matrix = matrixTrans.Matrix;
            matrix.ScaleAt(scaleX, scaleY, pos.X, pos.Y);
            matrixTrans = new MatrixTransform(matrix);
            visualHost.RenderTransform = matrixTrans;
        }
        public void Transform(Point pos)
        {
            scaleX = pos.X / 10;
            scaleY = pos.Y / 10;
            visualHost.RenderTransform = new TranslateTransform(scaleX + 25, scaleY + 25);
        }
    }
    public class MyVisualHost : FrameworkElement
    {
        // Create a collection of child visual objects.
        private readonly VisualCollection _children;

        public MyVisualHost()
        {
            _children = new VisualCollection(this);
        }
        public void Clear()
        {
            _children.Clear();
        }
        public void Ellipse(Brush colour, Point position, double radiusX, double radiusY)
        {
            _children.Add(CreateDrawingVisualEllipse(colour, position, radiusX, radiusY));
        }

        public void Rectangle(Brush colour, Point position, double width, double height)
        {
            _children.Add(CreateDrawingVisualRectangle(colour, position, width, height));
        }

        public void Line(Brush colour, double thickness, Point pos1, Point pos2, DashStyle dash)
        {
            _children.Add(CreateDrawingVisualLine(colour, thickness, pos1, pos2, dash));
        }

        // Create a DrawingVisual that contains a rectangle.
        private DrawingVisual CreateDrawingVisualRectangle(Brush colour, Point position, double width, double height)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Create a rectangle and draw it in the DrawingContext.
            Rect rect = new Rect(position, new Size(width, height));
            drawingContext.DrawRectangle(colour, null, rect);

            // Persist the drawing content.
            drawingContext.Close();

            return drawingVisual;
        }

        // Create a DrawingVisual that contains text.
        private DrawingVisual CreateDrawingVisualText()
        {
            // Create an instance of a DrawingVisual.
            System.Windows.Media.DrawingVisual drawingVisual = new System.Windows.Media.DrawingVisual();

            // Retrieve the DrawingContext from the DrawingVisual.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

#pragma warning disable CS0618 // 'FormattedText.FormattedText(string, CultureInfo, FlowDirection, Typeface, double, Brush)' is obsolete: 'Use the PixelsPerDip override'
            // Draw a formatted text string into the DrawingContext.
            drawingContext.DrawText(
                new FormattedText("Click Me!",
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    36, Brushes.Black),
                new Point(200, 116));
#pragma warning enable CS0618 // 'FormattedText.FormattedText(string, CultureInfo, FlowDirection, Typeface, double, Brush)' is obsolete: 'Use the PixelsPerDip override'

            // Close the DrawingContext to persist changes to the DrawingVisual.
            drawingContext.Close();

            return drawingVisual;
        }

        // Create a DrawingVisual that contains an ellipse.
        private DrawingVisual CreateDrawingVisualEllipse(Brush colour, Point position, double radiusX, double radiusY)
        {
            System.Windows.Media.DrawingVisual drawingVisual = new System.Windows.Media.DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawEllipse(colour, null, position, radiusX, radiusY);
            drawingContext.Close();

            return drawingVisual;
        }

        private DrawingVisual CreateDrawingVisualLine(Brush colour, double thickness, Point pos1, Point pos2, DashStyle dashed)
        {
            DrawingVisual drawingVisual = new System.Windows.Media.DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Pen pen = new Pen(colour, thickness) { DashStyle = dashed };

            drawingContext.DrawLine(pen, pos1, pos2);

            // Persist the drawing content.
            drawingContext.Close();

            return drawingVisual;
        }


        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount => _children.Count;

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }
    public static class Processes
    {
        private static T downloadjsondata<T>(Uri url)
        {
            try
            {
            WebRequest request = WebRequest.Create(url.ToString()); //Initiates a Webrequest to the url
            IWebProxy webProxy = WebRequest.DefaultWebProxy;
            webProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.Proxy = webProxy; //requests the data of Website
            string data;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse(); //recieves the response as a string
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                data = sr.ReadToEnd();
                sr.Close();
            }
            T info = (T)JsonConvert.DeserializeObject(data, typeof(T));
            return info;//converts the data from JSON format to string and returns
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static Root GetData(Uri url)
        {
            return downloadjsondata<Root>(url);
        }
        public static Geocode GetGeoData(string postcode)
        {
            Uri url = new Uri("https://transportapi.com/v3/uk/places.json?app_id="+ Settings.GetApiInfo("id")+"&app_key="+ Settings.GetApiInfo("key")+"&query="+ postcode + "&type=postcode");
            return downloadjsondata<Geocode>(url);
        }
        public static Root GetRoadData(string filename)
        {
            string data;
            using (StreamReader sr = new StreamReader(filename))
            {
                data = sr.ReadToEnd();
                sr.Close();
            }
            Root root = (Root)JsonConvert.DeserializeObject(data, typeof(Root));
            return root;//converts the data from JSON format to string if the is somthing there
        }
        public static Point MercatorProj(double lat, double lon)
        {
            double mapWidth = 100000;
            double mapHeight = 100000;

            // get x value
            double x = (lon + 180) * (mapWidth / 360);

            // convert from degrees to radians
            double latRad = lat * Math.PI / 180;

            // get y value
            double mercN = Math.Log(Math.Tan((Math.PI / 4) + (latRad / 2)));
            double y = (mapHeight / 2) - (mapWidth * mercN / (2 * Math.PI));
            return new Point(x, y);
        }
        private static double toRadians(double angle) //converts degrees to radian
        {
            return (angle * Math.PI) / 180;
        }
        public static double HaversineDist(double lat1, double lat2, double lon1, double lon2)
        {
            lat1 = toRadians(lat1); //converts all the lat/lon data to Radians
            lat2 = toRadians(lat2);
            lon1 = toRadians(lon1);
            lon2 = toRadians(lon2);

            // Haversine formula - uses the difference of the lat/lon and uses triganometry to calculate distance taking into account the earths curvature
            double dlon = lon2 - lon1;
            double dlat = lat2 - lat1;
            double angle = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlon / 2), 2);
            double theta = 2 * Math.Asin(Math.Sqrt(angle));

            double r = 6371; //radius of the earth approx (km) (miles = 3956)

            return (theta * r); //returns the result
        }
    }
}
