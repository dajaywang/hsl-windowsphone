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

namespace HSL.Models
{
    public class Geocoding
    {
        public String locType { get; set;}
        public Double locTypeId {get; set;}
        public String name {get; set;}
        public String matchedName {get; set;}
        public String lang {get; set;}
        public String city {get; set;}
        public String coords {get; set;}
        public String details { get; set;} //not string, but ok
    }
}
