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
using System.ComponentModel;

namespace HSL.ViewModels
{
    public class RouteItemViewModel : INotifyPropertyChanged
    {
        //icon for one route
        private String _imagePath;
        public String ImagePath
        { 
            get
            {
                return _imagePath;
            }
            set
            {
                if (value != _imagePath)
                {
                    _imagePath = value;
                    NotifyPropertyChanged("ImagePath");
                }
            }
            
          }

        // show time & distance & bus code 
        private String _timeLengthCode;
        public String TimeLengthCode 
        { 
            get
            {
                return _timeLengthCode;
            }
            set
            {
                if (value != _timeLengthCode)
                {
                    _timeLengthCode = value;
                    NotifyPropertyChanged("TimeLengthCode");
                }
            }

        }

        // show time & stop name & stop code
        private String _timeStopsCode;
        public String TimeStopsCode
        {
            get
            {
                return _timeStopsCode;
            }
            set
            {
                if(value != _timeStopsCode)
                {
                    _timeStopsCode = value;
                    NotifyPropertyChanged("TimeStopsCode");
                }
            }

        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
