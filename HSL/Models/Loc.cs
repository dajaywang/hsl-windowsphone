using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace HSL.Models
{
    public class Loc
    {
        public Coord coord;
        public String arrTime;
        public String depTime;
        public String name;
        public String code;
        public String shortCode;
        public String stopAddress;

        public Loc(JToken token)
        {

            var coordToken = token["coord"];
            double x = coordToken.Value<double>("x");
            double y = coordToken.Value<double>("y");
            coord = new Coord(y, x);
            arrTime = token.Value<string>("arrTime");
            depTime = token.Value<string>("depTime");
            name = token.Value<string>("name");
            code = token.Value<string>("code");
            shortCode = token.Value<string>("shortCode");
            stopAddress = token.Value<string>("stopAddress");
            System.Diagnostics.Debug.WriteLine("in Loc name:" + name);
        }
    }
}
