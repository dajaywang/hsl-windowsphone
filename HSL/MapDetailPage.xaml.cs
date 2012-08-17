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
    public partial class mapDetailPage : PhoneApplicationPage
    {
        String[] coordsArray;

        public mapDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string stopCoords = "";
            if (NavigationContext.QueryString.TryGetValue("stopCoords", out stopCoords))
            {

                coordsArray = stopCoords.Split(new Char[] { '|' });

                System.Diagnostics.Debug.WriteLine("stopCoords:" + stopCoords);
            }

            for (int i = 0; i < coordsArray.Length; i++)
            {
                String coordStr = coordsArray[i];
                String[] coordTempArray = coordStr.Split(',');

                System.Diagnostics.Debug.WriteLine("coordStr:" + coordStr + "," + coordTempArray[0] + "," + coordTempArray[1]);


                Pushpin p = new Pushpin();
                p.Template = this.Resources["pinStop"] as ControlTemplate;
                p.Location = new GeoCoordinate(Convert.ToDouble(coordTempArray[1]), Convert.ToDouble(coordTempArray[0]));
                mapControl.Items.Add(p);

                //set the first one as the center
                if (i == coordsArray.Length/2)
                {
                    map1.SetView(p.Location, 13.0);
                }
            }

        }
    }
}