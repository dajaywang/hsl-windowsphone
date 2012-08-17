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
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;

namespace HSL
{
    public partial class FromToAddressResult : PhoneApplicationPage
    {
        String[] addressArray;
        String[] coordsArray;

        public FromToAddressResult()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string fromToAddress = "";
            //var fromToAddress = NavigationContext.QueryString["fromToAddress"];
            if (NavigationContext.QueryString.TryGetValue("fromToAddress", out fromToAddress))
            {
                addressArray = fromToAddress.Split(new Char[] {'|'});
                fromToListBox.ItemsSource = addressArray;

                System.Diagnostics.Debug.WriteLine("fromToAddress:" + fromToAddress);
            }

            string fromToCoords = "";
            if (NavigationContext.QueryString.TryGetValue("fromToCoords", out fromToCoords))
            {

                coordsArray = fromToCoords.Split(new Char[] { '|' });

                System.Diagnostics.Debug.WriteLine("fromToCoords:" + fromToCoords);
            }

            string latitude = "";
            if (NavigationContext.QueryString.TryGetValue("latitude", out latitude))
            { }

            string longitude = "";
            if (NavigationContext.QueryString.TryGetValue("longitude", out longitude))
            {
                System.Diagnostics.Debug.WriteLine("latitude:" + latitude + ",longitude:" + longitude);

                Pushpin p = new Pushpin();
                p.Template = this.Resources["pinMyLoc"] as ControlTemplate;
                p.Location = new GeoCoordinate(Convert.ToDouble(latitude), Convert.ToDouble(longitude));
                mapControl.Items.Add(p);
            }

            for (int i = 0; i < coordsArray.Length; i++)
            {
                String coordStr = coordsArray[i];
                String [] coordTempArray = coordStr.Split(',');

                System.Diagnostics.Debug.WriteLine("coordStr:" + coordStr + "," + coordTempArray[0] + "," + coordTempArray[1]);


                Pushpin p = new Pushpin();
                p.Template = this.Resources["pinStop"] as ControlTemplate;
                p.Location = new GeoCoordinate(Convert.ToDouble(coordTempArray[1]), Convert.ToDouble(coordTempArray[0]));
                mapControl.Items.Add(p);

                //set the first one as the center
                if (i == 0)
                {
                    map1.SetView(p.Location, 14.0);
                }
            }

        }

        private void fromTolistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fromToListBox.SelectedIndex == -1)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/MainPage.xaml?selectedItem=" + addressArray[fromToListBox.SelectedIndex] + "&itemIndex=" + fromToListBox.SelectedIndex, UriKind.Relative));
            
            // Reset selected index to -1 (no selection)
            fromToListBox.SelectedIndex = -1;

        }

    }
}