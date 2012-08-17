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
    public class Leg
    {
        public Double length;
        public Double duration;
        public String type;
        public String code;
        public Loc[] locs;
        public String shape;

        public Leg(JToken token)
        {
            length = token.Value<double>("length");
            duration = token.Value<double>("duration");
            type = token.Value<string>("type");
            code = token.Value<string>("code");

            System.Diagnostics.Debug.WriteLine("in Leg:" + type + "--" + code);

            JArray locTokens = token["locs"] as JArray;
            System.Diagnostics.Debug.WriteLine("Loc length:" +locTokens.Count);

            if (locTokens != null)
            {
                locs = new Loc[locTokens.Count];
                for (int i = 0; i < locs.Length; ++i)
                {
                    locs[i] = new Loc(locTokens[i]);
                }
            }

            shape = token.Value<string>("shape");

        }
    }
}
