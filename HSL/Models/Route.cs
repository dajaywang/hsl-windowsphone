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
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace HSL.Models
{
    public class Route
    {
        public Double length;
        public Double duration;
        public Leg[] legs;

        public Route(JToken token)
        {
            length = token.Value<double>("length");
            System.Diagnostics.Debug.WriteLine("in Route class:" + Convert.ToString(length));
            duration = token.Value<double>("duration");

            JArray legTokens = token["legs"] as JArray;
            System.Diagnostics.Debug.WriteLine("in Route class, leg length:" + legTokens.Count);
            if(legTokens != null)
            {
                legs = new Leg[legTokens.Count];
                for(int i = 0; i < legs.Length; ++i)
                {
                    legs[i] = new Leg(legTokens[i]);
                }
            }
        }
    }
}
