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
    public class Coord
    {
        public Double x { get; set; }
        public Double y { get; set; }

        public Coord(Double xValue, Double yValue)
        {
            x = xValue;
            y = yValue;
        }


    }
}
