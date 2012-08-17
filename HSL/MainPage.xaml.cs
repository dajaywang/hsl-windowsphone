using System;
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
using Microsoft.Phone.Controls.Maps;
using Microsoft.Devices;
using HSL.Models;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.ComponentModel;
using Microsoft.Phone.Shell;
using System.IO;
using System.Device.Location;


namespace HSL
{
    public partial class MainPage : PhoneApplicationPage
    {
        //URLs
        String geoCodingURL = "http://api.reittiopas.fi/hsl/prod/?user=dajie&pass=482826&epsg_out=wgs84&request=geocode&key=";
        String stopsInAreaURL = "http://api.reittiopas.fi/hsl/prod/?user=dajie&pass=482826&request=stops_area&epsg_in=wgs84&epsg_out=wgs84&center_coordinate=";

        //navigating related
        MainPageModel mainPageModel;
        Boolean _isNewPageInstance = false;

        //from or to address related 
        Boolean isFromTextBox;
        int itemIndex;
        List<String> coordsList;

        //////search parameters
        ////Geocoding
        //key: use textBox.text directly
        ////Routing
        //fromTextBoxText;
        //toTextBoxText;
        String fromCoords;
        String toCoords;
        String date;
        String time;
        String timetype;
        ////Nearby
        String locationX;
        String LocationY;

        //search result data
        List<Geocoding> fromToAddressRequestResult;
        List<StopsInArea> stopsInAreaRequestResult;

        //Location
        GeoCoordinateWatcher watcher;

        //
        int pivotSelectedIndex;


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _isNewPageInstance = true;
            mainPageModel = new MainPageModel();


            DateTime dateTemp = (DateTime)datePicker1.Value;
            date = dateTemp.ToString("yyyyMMdd");
            System.Diagnostics.Debug.WriteLine("date in Constructor: " + date + ", " + dateTemp.ToString());

            DateTime timeTemp = (DateTime)timePicker1.Value;
            time = timeTemp.ToString("HHmm");
            System.Diagnostics.Debug.WriteLine("time in Constructor: " + time + ", " + timeTemp.ToString());
           
            timetype = "departure";

            isFromTextBox = true;
            System.Diagnostics.Debug.WriteLine("isFromTextBox in constructor: " + isFromTextBox);

            fromToAddressRequestResult = new List<Geocoding>();
            stopsInAreaRequestResult = new List<StopsInArea>();

            coordsList = new List<String>();

            //location
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                watcher.MovementThreshold = 20;
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                watcher.Start();
            }
        }

        ///PivotItem: Routes
        ///

        private void fromTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            isFromTextBox = true;
            System.Diagnostics.Debug.WriteLine("isFromTextBox in textBox: " + isFromTextBox);

        }

        private void toTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            isFromTextBox = false;
            System.Diagnostics.Debug.WriteLine("isFromTextBox" + isFromTextBox);
        }

        private void fromToTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    WebClient wc = new WebClient();
                    Uri geocodingUri;
                    if (isFromTextBox == true)
                    {
                        geocodingUri = new Uri(geoCodingURL + fromTextBox.Text);
                    }
                    else
                    {
                        geocodingUri = new Uri(geoCodingURL + toTextBox.Text);
                    }

                    System.Diagnostics.Debug.WriteLine(geocodingUri);

                    //remove all before another search 
                    coordsList.RemoveRange(0, coordsList.Count);
                    fromToAddressRequestResult.RemoveRange(0, fromToAddressRequestResult.Count);


                    wc.DownloadStringAsync(geocodingUri);

                    wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(geocodingRequestCompleted);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                } 

            }

        }

        void geocodingRequestCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                System.Diagnostics.Debug.WriteLine(e.Result);


                System.IO.MemoryStream mStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(e.Result));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Geocoding>));
                fromToAddressRequestResult = (List<Geocoding>)serializer.ReadObject(mStream);

                string urlWithData = string.Format("/FromToAddressResult.xaml?");
                List<string> nameList = new List<string>();

                if (fromToAddressRequestResult.Count > 0)
                {
                    for (int i = 0; i < fromToAddressRequestResult.Count; i++)
                    {
                        nameList.Add(fromToAddressRequestResult[i].name + "," + fromToAddressRequestResult[i].city);
                        coordsList.Add(fromToAddressRequestResult[i].coords);
                        /*
                        if (fromToAddressRequestResult[i].locTypeId == 10)
                        {

                        }
                        */

                        System.Diagnostics.Debug.WriteLine("coords added to coordsList:" + fromToAddressRequestResult[i].coords);
                    }
                }

                System.Diagnostics.Debug.WriteLine("PhoneApplicationService.Current.State.Count:" + PhoneApplicationService.Current.State.Count);

                urlWithData += "fromToAddress=" + String.Join("|", nameList.ToArray());
                urlWithData += "&fromToCoords=" + String.Join("|", coordsList.ToArray());
                urlWithData += "&latitude=" + locationX + "&longitude=" + LocationY;

                System.Diagnostics.Debug.WriteLine(urlWithData);

                NavigationService.Navigate(new Uri(urlWithData, UriKind.Relative));
            }
        }

        private void NearbyButton_Click(object sender, RoutedEventArgs e)
        {
            if (watcher.Status == GeoPositionStatus.Disabled)
            {
                //watcher.StatusChanged -= watcher_StatusChanged; 
                MessageBox.Show("Location services must be enabled on your phone.");
                return;
            }

            try
            {
                WebClient wc = new WebClient();
                Uri stopsInAreaUri;
                if (isFromTextBox == true)
                {
                    stopsInAreaUri = new Uri(stopsInAreaURL + LocationY + "," + locationX);
                }
                else
                {
                    stopsInAreaUri = new Uri(stopsInAreaURL + LocationY + "," + locationX);
                }
                System.Diagnostics.Debug.WriteLine(stopsInAreaUri);

                //remove all before another search 
                coordsList.RemoveRange(0, coordsList.Count);
                stopsInAreaRequestResult.RemoveRange(0, stopsInAreaRequestResult.Count);

                wc.DownloadStringAsync(stopsInAreaUri);

                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(stopsInAreaCompleted);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void stopsInAreaCompleted(Object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                System.Diagnostics.Debug.WriteLine(e.Result);


                System.IO.MemoryStream mStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(e.Result));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<StopsInArea>));
                stopsInAreaRequestResult = (List<StopsInArea>)serializer.ReadObject(mStream);

                //reuse the fromToAddress
                string urlWithData = string.Format("/FromToAddressResult.xaml?");
                List<string> nameList = new List<string>();

                if (stopsInAreaRequestResult.Count > 0)
                {
                    for (int i = 0; i < stopsInAreaRequestResult.Count; i++)
                    {
                        nameList.Add(stopsInAreaRequestResult[i].name + "," + stopsInAreaRequestResult[i].city);
                        coordsList.Add(stopsInAreaRequestResult[i].coords);

                        System.Diagnostics.Debug.WriteLine("coords added to coordsList:" + stopsInAreaRequestResult[i].coords);
                    }
                }

                System.Diagnostics.Debug.WriteLine("PhoneApplicationService.Current.State.Count:" + PhoneApplicationService.Current.State.Count);

                urlWithData += "fromToAddress=" + String.Join("|", nameList.ToArray());
                urlWithData += "&fromToCoords=" + String.Join("|", coordsList.ToArray());
                urlWithData += "&latitude=" + locationX + "&longitude=" + LocationY;

                System.Diagnostics.Debug.WriteLine(urlWithData);

                NavigationService.Navigate(new Uri(urlWithData, UriKind.Relative));
            }
        }

        private void datePicker1_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime dateTemp = (DateTime)datePicker1.Value;
            date = dateTemp.ToString("yyyyMMdd");
            System.Diagnostics.Debug.WriteLine("date in Constructor: " + date + ", " + dateTemp.ToString());
        }

        private void timePicker1_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime timeTemp = (DateTime)timePicker1.Value;
            time = timeTemp.ToString("HHmm");
            System.Diagnostics.Debug.WriteLine("time in Constructor: " + time + ", " + timeTemp.ToString());
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            timetype = "arrival";
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            timetype = "departure";
        }

        // location funtions
        //
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyPosititonChanged(e));
        }

        void MyPosititonChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            locationX = e.Position.Location.Latitude.ToString("0.000000");
            LocationY = e.Position.Location.Longitude.ToString("0.000000");

            /*
            if (mapFirstTime == true)
            {
                Pushpin p = new Pushpin();
                p.Template = this.Resources["pinMyLoc"] as ControlTemplate;
                p.Location = new GeoCoordinate(Convert.ToDouble(locationX), Convert.ToDouble(LocationY));
                routeMapControl.Items.Add(p);
                routeMap.SetView(p.Location, 14.0);

                mapFirstTime = false;
            }
            */
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            MyStatusChanged(e));
        }

        void MyStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.NoData:
                    MessageBox.Show("No Data Available");
                    break;
                case GeoPositionStatus.Ready:
                    //donothing
                    break;
                case GeoPositionStatus.Disabled:
                    MessageBox.Show("Location service is disabled");
                    break;
            }
        }

        //
        //

        //Navigation functions
        //
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                //CommitTextBoxWithFocus();

                // Save the ViewModel variable in the page's State dictionary.
                mainPageModel.fromTextBoxText = fromTextBox.Text;
                mainPageModel.toTextBoxText = toTextBox.Text;
                mainPageModel.fromToCoordsList = coordsList;
                mainPageModel.fromCoords = fromCoords;
                mainPageModel.toCoords = toCoords;
                mainPageModel.date = date;
                mainPageModel.time = time;
                mainPageModel.timetype = timetype;
                mainPageModel.isFromTextBox = isFromTextBox;
                mainPageModel.pivotSelectedIndex = pivotSelectedIndex;

                PhoneApplicationService.Current.State["mainPageModel"] = mainPageModel;

                System.Diagnostics.Debug.WriteLine("PhoneApplicationService.Current.State.Count:" + PhoneApplicationService.Current.State.Count);
            }

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            if (_isNewPageInstance)
            {
                
                System.Diagnostics.Debug.WriteLine(PhoneApplicationService.Current.State.Count);
                if (PhoneApplicationService.Current.State.Count > 0)
                {
                    if(PhoneApplicationService.Current.State.ContainsKey("mainPageModel"))
                    {
                        mainPageModel = (MainPageModel)PhoneApplicationService.Current.State["mainPageModel"];
                        fromTextBox.Text = (String)mainPageModel.fromTextBoxText;
                        toTextBox.Text = (String)mainPageModel.toTextBoxText;
                        coordsList = (List<String>)mainPageModel.fromToCoordsList;
                        fromCoords = (String)mainPageModel.fromCoords;
                        toCoords = (String)mainPageModel.toCoords;
                        date = (String)mainPageModel.date;
                        time = (String)mainPageModel.time;
                        isFromTextBox = (Boolean)mainPageModel.isFromTextBox;
                        pivotSelectedIndex = (int)mainPageModel.pivotSelectedIndex;

                        System.Diagnostics.Debug.WriteLine("isFromTextBox in Navigation: " + mainPageModel.isFromTextBox);
                    }
                    else
                        System.Diagnostics.Debug.WriteLine("no key ");
                }

                PhoneApplicationService.Current.State.Clear();
            }
            _isNewPageInstance = false;

            //Pivot Item: Route Search
            if (pivotSelectedIndex == 0)
            {
                //get value from one page's name
                string addressChosed = "";
                string itemIndexStr;
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out addressChosed))
                {
                    if (isFromTextBox == true)
                    {
                        fromTextBox.Text = addressChosed;
                    }
                    else
                    {
                        toTextBox.Text = addressChosed;
                    }
                }
                //get value from another name from same or another page 
                if (NavigationContext.QueryString.TryGetValue("itemIndex", out itemIndexStr))
                {
                    itemIndex = Convert.ToInt32(itemIndexStr);
                    System.Diagnostics.Debug.WriteLine("received itemIndex: " + itemIndex + "coordsList length: " + coordsList.Count);

                    if (isFromTextBox == true)
                    {
                        fromCoords = coordsList[itemIndex];
                        System.Diagnostics.Debug.WriteLine("fromCoords: " + fromCoords);
                    }
                    else
                    {
                        toCoords = coordsList[itemIndex];
                        System.Diagnostics.Debug.WriteLine("toCoords: " + toCoords);
                    }
                }
            }

            //Pivot Item:
            if (pivotSelectedIndex == 1)
            {
            }

            //Pivot Item:
            if (pivotSelectedIndex == 2)
            {
            }

            //Pivot Item:
            if (pivotSelectedIndex == 3)
            {
            }

            //Pivot Item:
            if (pivotSelectedIndex == 4)
            {
            }

        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            pivotSelectedIndex = mainPivot.SelectedIndex;
            System.Diagnostics.Debug.WriteLine("the pivot selected index:" + pivotSelectedIndex);

        }

        private void ApplicationBarIconButton_Click_Search(object sender, EventArgs e)
        {
            //Pivot Item: Route Search 
            if (pivotSelectedIndex == 0)
            {
                NavigationService.Navigate(new Uri("/RouteResult.xaml?fromCoords=" + fromCoords + "&toCoords=" + toCoords + "&date=" + date + "&time=" + time + "&timetype=" + timetype, UriKind.Relative));
            }

            //Pivot Item: Cycling
            if (pivotSelectedIndex == 1)
            {
                NavigationService.Navigate(new Uri("", UriKind.Relative));
            }

            //Pivot Item: Nearby
            if (pivotSelectedIndex == 2)
            {
                NavigationService.Navigate(new Uri("", UriKind.Relative));
            }

            //Pivot Item: Stops
            if (pivotSelectedIndex == 3)
            {
                NavigationService.Navigate(new Uri("", UriKind.Relative));
            }

            //Pivot Item: Lines
            if (pivotSelectedIndex == 4)
            {
                NavigationService.Navigate(new Uri("", UriKind.Relative));
            }
        }











    }
}