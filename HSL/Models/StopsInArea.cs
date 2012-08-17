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
    public class StopsInArea
    {
        public String code { get; set; }
        public String name { get; set; }
        public String city { get; set; }
        public String coords { get; set; }
        public Double dist { get; set; }
        public String codeShort { get; set; }
        public String address { get; set; }
    }
}
