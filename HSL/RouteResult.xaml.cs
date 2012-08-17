using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using System.Runtime.Serialization.Json;
using HSL.Models;
using System.IO;
using System.Collections.ObjectModel;
using HSL.ViewModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace HSL
{
    public partial class routeResult : PhoneApplicationPage
    {
        public List<RouteItemViewModel> items1 { get; set; }
        public List<RouteItemViewModel> items2 { get; set; }
        public List<RouteItemViewModel> items3 { get; set; }

        //which panorama page we are in
        int panoramaSelectedIndex;

        //save teh coords, transfer to MapDetail page               
        List<String> locsCoords1 = new List<string>();
        List<String> locsCoords2 = new List<string>();
        List<String> locsCoords3 = new List<string>();

        //Constructor
        public routeResult()
        {
            InitializeComponent();

            items1 = new List<RouteItemViewModel>();
            items2 = new List<RouteItemViewModel>();
            items3 = new List<RouteItemViewModel>();
        }

        String routingURL = "http://api.reittiopas.fi/hsl/prod/?user=dajie&pass=482826&epsg_in=wgs84&epsg_out=wgs84&request=route&";


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            String fromCoords;
            String toCoords;
            String date;
            String time;
            String timetype;

            NavigationContext.QueryString.TryGetValue("fromCoords", out fromCoords);
            NavigationContext.QueryString.TryGetValue("toCoords", out toCoords);
            NavigationContext.QueryString.TryGetValue("date", out date);
            NavigationContext.QueryString.TryGetValue("time", out time);
            NavigationContext.QueryString.TryGetValue("timetype", out timetype);
            System.Diagnostics.Debug.WriteLine(fromCoords + "," + toCoords + "," + date + "," + time + "," + timetype);

            //web client
            try
            {
                WebClient wc = new WebClient();
                Uri routingUri = new Uri(routingURL + "from=" + fromCoords + "&to=" + toCoords + "&date=" + date + "&time=" + time + "&timetype=" + timetype);
                System.Diagnostics.Debug.WriteLine(routingUri);
                
                //show process bar
                //performanceProgressBar1.IsIndeterminate = true;

                wc.DownloadStringAsync(routingUri);
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(routingRequestCompleted);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        void routingRequestCompleted(Object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //deserialize joson into objects
                List<RouteList> routeResults = new List<RouteList>();

                if (e.Result.Trim().Length != 0)
                {
                    JArray json = JArray.Parse(e.Result);

                    foreach (var token in json)
                    {
                        System.Diagnostics.Debug.WriteLine("Json length:" + json.Count);
                        var routeArray = token as JArray;
                        if (routeArray != null)
                        {
                            var routeList = new RouteList { routes = new Route[routeArray.Count] };
                            System.Diagnostics.Debug.WriteLine("Route length1:" + routeArray.Count);
                            for (int i = 0; i < routeList.routes.Length; ++i)
                            {
                                routeList.routes[i] = new Route(routeArray[i]);
                            }
                            routeResults.Add(routeList);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("route Results length:" + routeResults.Count);
                }

                ////Route1
                Route oneRoute = routeResults[0].routes[0];
                
                //save 3 parts in List to show 1 Route. Length for 3 parts are the same.
                String totalLengthDuration1; 
                List<String> legLengthDurationNoList1 = new List<String>();
                List<String> legTypeList1 = new List<String>();
                List<String> locsList1 = new List<String>();

                
                //0, the total statics, not include in the following 3 parts
                totalLengthDuration1 = "Length: " + String.Format("{0:0.00}", oneRoute.length/1000.0) + "Km   Duration:" + String.Format("{0:0.0}",oneRoute.duration/60) + "Min" + Environment.NewLine;

                for ( int j = 0; j < oneRoute.legs.Length; ++j)
                {
                    Leg oneLeg = oneRoute.legs[j];

                    //1
                    if (oneLeg.type == "walk")
                    {
                        legTypeList1.Add("/Images/walking.png");
                    }
                    else if (oneLeg.type == "1" || oneLeg.type == "3" || oneLeg.type == "4" || oneLeg.type == "5" || oneLeg.type == "22" || oneLeg.type == "25" || oneLeg.type == "36" || oneLeg.type == "39")
                    {
                        legTypeList1.Add("/Images/bus.png");
                    }
                    else if (oneLeg.type == "2")
                    {
                        legTypeList1.Add("/Images/tram.png");
                    }
                    else if (oneLeg.type == "6")
                    {
                        legTypeList1.Add("/Images/metro.png");
                    }
                    else if (oneLeg.type == "7")
                    {
                        legTypeList1.Add("/Images/ferry.png");
                    }
                    else if (oneLeg.type == "12")
                    {
                        legTypeList1.Add("/Images/train.png");
                    }


                    //2
                    String realCode1 = "";
                    if (oneLeg.code != null)
                    {
                        realCode1 = oneLeg.code.Substring(4) != "" ? oneLeg.code.Substring(1,4) : oneLeg.code.Substring(1,3);

                    }
                    legLengthDurationNoList1.Add(String.Format("{0:0.00}", oneLeg.length / 1000.0) + "Km    " + String.Format("{0:0.0}", oneLeg.duration / 60) + "Min    " + realCode1);

                    
                    //3
                    String locsTemp = "";
                    for( int k = 0; k < oneLeg.locs.Length; ++k)
                    {
                        Loc oneLoc = oneLeg.locs[k];
                        if (oneLoc.code != null)
                        {
                            locsTemp += DateTime.ParseExact(oneLoc.depTime, "yyyyMMddHHmm", null).ToShortTimeString() + "    " + oneLoc.name + "(" + oneLoc.shortCode + ")" + Environment.NewLine;
                            locsCoords1.Add(oneLoc.coord.y + "," + oneLoc.coord.x);
                        }
                    }
                    locsList1.Add(locsTemp);
                }

                //save into View Model Item choose 1 item from each List of 3 parts
                System.Diagnostics.Debug.WriteLine("---:" + legTypeList1.Count + "," + legLengthDurationNoList1.Count + "," + locsList1.Count);
                for(int i = 0; i < legTypeList1.Count; ++i)
                {
                    RouteItemViewModel oneRouteItem = new RouteItemViewModel();
                    oneRouteItem.ImagePath = legTypeList1[i];
                    oneRouteItem.TimeLengthCode = legLengthDurationNoList1[i];
                    oneRouteItem.TimeStopsCode = locsList1[i];
                    items1.Add(oneRouteItem);
                }

                //remove processbar
                performanceProgressBar1.IsIndeterminate = false;

                //give value
                this.Route1Total.Text = totalLengthDuration1;
                this.Route1List.ItemsSource = items1;


                //// Route2
                Route oneRoute2 = routeResults[1].routes[0];

                List<String> legLengthDurationNoList2 = new List<String>();
                List<String> legTypeList2 = new List<String>();
                List<String> locsList2 = new List<String>();
                String totalLengthDuration2; 

                //1
                totalLengthDuration2 = "Length: " + String.Format("{0:0.00}",oneRoute.length/1000.0) + "Km   Duration" + String.Format("{0:0.0}",oneRoute.duration/60) + "Min" + Environment.NewLine;


                for (int j = 0; j < oneRoute2.legs.Length; ++j)
                {
                    //2
                    Leg oneLeg = oneRoute2.legs[j];
                    if (oneLeg.type == "walk")
                    {
                        legTypeList2.Add("/Images/walking.png");
                    }
                    else if (oneLeg.type == "1" || oneLeg.type == "3" || oneLeg.type == "4" || oneLeg.type == "5" || oneLeg.type == "22" || oneLeg.type == "25" || oneLeg.type == "36" || oneLeg.type == "39")
                    {
                        legTypeList2.Add("/Images/bus.png");
                    }
                    else if (oneLeg.type == "2")
                    {
                        legTypeList2.Add("/Images/tram.png");
                    }
                    else if (oneLeg.type == "6")
                    {
                        legTypeList2.Add("/Images/metro.png");
                    }
                    else if (oneLeg.type == "7")
                    {
                        legTypeList2.Add("/Images/ferry.png");
                    }
                    else if (oneLeg.type == "12")
                    {
                        legTypeList2.Add("/Images/train.png");
                    }

                    //3
                    String realCode2 = "";
                    if (oneLeg.code != null)
                    {
                        realCode2 = oneLeg.code.Substring(4) != "" ? oneLeg.code.Substring(1, 4) : oneLeg.code.Substring(1, 3);

                    }
                    legLengthDurationNoList2.Add(String.Format("{0:0.00}", oneLeg.length / 1000.0) + "Km    " + String.Format("{0:0.0}", oneLeg.duration / 60) + "Min    " + realCode2);

                    String locsTemp = "";
                    for (int k = 0; k < oneLeg.locs.Length; ++k)
                    {
                        Loc oneLoc = oneLeg.locs[k];
                        if (oneLoc.code != null)
                        {
                            locsTemp += DateTime.ParseExact(oneLoc.depTime, "yyyyMMddHHmm", null).ToShortTimeString() + "    " + oneLoc.name + "(" + oneLoc.shortCode + ")" + Environment.NewLine;
                            locsCoords2.Add(oneLoc.coord.y + "," + oneLoc.coord.x);
                        }
                    }
                    locsList2.Add(locsTemp);
                }

                System.Diagnostics.Debug.WriteLine("---:" + legTypeList2.Count + "," + legLengthDurationNoList2.Count + "," + locsList2.Count);
                for (int i = 0; i < legTypeList2.Count; ++i)
                {
                    RouteItemViewModel oneRouteItem2 = new RouteItemViewModel();
                    oneRouteItem2.ImagePath = legTypeList2[i];
                    oneRouteItem2.TimeLengthCode = legLengthDurationNoList2[i];
                    oneRouteItem2.TimeStopsCode = locsList2[i];
                    items2.Add(oneRouteItem2);
                }

                this.Route2Total.Text = totalLengthDuration2;
                this.Route2List.ItemsSource = items2;


                ////Route3
                Route oneRoute3 = routeResults[2].routes[0];

                List<String> legLengthDurationNoList3 = new List<String>();
                List<String> legTypeList3 = new List<String>();
                List<String> locsList3 = new List<String>();
                String totalLengthDuration3; 

                totalLengthDuration3 = "Length: " + String.Format("{0:0.00}",oneRoute.length/1000.0) + "Km   Duration" + String.Format("{0:0.0}",oneRoute.duration/60) + "Min" + Environment.NewLine;

                for (int j = 0; j < oneRoute3.legs.Length; ++j)
                {
                    Leg oneLeg = oneRoute3.legs[j];
                    if (oneLeg.type == "walk")
                    {
                        legTypeList3.Add("/Images/walking.png");
                    }
                    else if (oneLeg.type == "1" || oneLeg.type == "3" || oneLeg.type == "4" || oneLeg.type == "5" || oneLeg.type == "22" || oneLeg.type == "25" || oneLeg.type == "36" || oneLeg.type == "39")
                    {
                        legTypeList3.Add("/Images/bus.png");
                    }
                    else if (oneLeg.type == "2")
                    {
                        legTypeList3.Add("/Images/tram.png");
                    }
                    else if (oneLeg.type == "6")
                    {
                        legTypeList3.Add("/Images/metro.png");
                    }
                    else if (oneLeg.type == "7")
                    {
                        legTypeList3.Add("/Images/ferry.png");
                    }
                    else if (oneLeg.type == "12")
                    {
                        legTypeList3.Add("/Images/train.png");
                    }

                    //for length and duration
                    String realCode3 = "";
                    if (oneLeg.code != null)
                    {
                        realCode3 = oneLeg.code.Substring(4) != "" ? oneLeg.code.Substring(1, 4) : oneLeg.code.Substring(1, 3);

                    }
                    legLengthDurationNoList3.Add(String.Format("{0:0.00}", oneLeg.length / 1000.0) + "Km    " + String.Format("{0:0.0}", oneLeg.duration / 60) + "Min    " + realCode3);

                    String locsTemp = "";
                    for (int k = 0; k < oneLeg.locs.Length; ++k)
                    {
                        Loc oneLoc = oneLeg.locs[k];
                        if (oneLoc.code != null)
                        {
                            locsTemp += DateTime.ParseExact(oneLoc.depTime, "yyyyMMddHHmm", null).ToShortTimeString() + "    " + oneLoc.name + "(" + oneLoc.shortCode + ")" + Environment.NewLine;
                            locsCoords3.Add(oneLoc.coord.y + "," + oneLoc.coord.x);
                        }
                    }
                    locsList3.Add(locsTemp);
                }



                System.Diagnostics.Debug.WriteLine("---:" + legTypeList3.Count + "," + legLengthDurationNoList3.Count + "," + locsList3.Count);
                for (int i = 0; i < legTypeList3.Count; ++i)
                {
                    RouteItemViewModel oneRouteItem3 = new RouteItemViewModel();
                    oneRouteItem3.ImagePath = legTypeList3[i];
                    oneRouteItem3.TimeLengthCode = legLengthDurationNoList3[i];
                    oneRouteItem3.TimeStopsCode = locsList3[i];
                    items3.Add(oneRouteItem3);
                }

                this.Route3Total.Text = totalLengthDuration3;
                this.Route3List.ItemsSource = items3;

            }
        }

        private void mainPanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panoramaSelectedIndex = mainPanorama.SelectedIndex;

            System.Diagnostics.Debug.WriteLine("the panorama selected index:" + panoramaSelectedIndex);

        }

        private void ApplicationBarIconButton_Click_Map(object sender, EventArgs e)
        {
            if (panoramaSelectedIndex == 0)
            {
                string urlWithData = string.Format("/MapDetailPage.xaml?");

                urlWithData += "&stopCoords=" + String.Join("|", locsCoords1.ToArray());
                System.Diagnostics.Debug.WriteLine("url to MapDetailPage:", urlWithData);

                NavigationService.Navigate(new Uri(urlWithData, UriKind.Relative));
            }
            else if (panoramaSelectedIndex == 1)
            {
                string urlWithData = string.Format("/MapDetailPage.xaml?");

                urlWithData += "&stopCoords=" + String.Join("|", locsCoords2.ToArray());
                System.Diagnostics.Debug.WriteLine("url to MapDetailPage:", urlWithData);

                NavigationService.Navigate(new Uri(urlWithData, UriKind.Relative));
 
            }
            else if (panoramaSelectedIndex == 2)
            {
                string urlWithData = string.Format("/MapDetailPage.xaml?");

                urlWithData += "&stopCoords=" + String.Join("|", locsCoords3.ToArray());
                System.Diagnostics.Debug.WriteLine("url to MapDetailPage:", urlWithData);

                NavigationService.Navigate(new Uri(urlWithData, UriKind.Relative));

            }

        }

    }
}