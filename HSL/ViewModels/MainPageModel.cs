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

using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace HSL.Models
{
    [DataContract]
    public class MainPageModel : INotifyPropertyChanged
    {
        private String _fromTextBoxText;
        private String _toTextBoxText;
        private String _fromCoords;
        private String _toCoords;
        private String _date;
        private String _time;
        private String _timetype;
        private Boolean _isFromTextBox;
        private List<String> _fromToCoordsList;
        private int _pivotSelectedIndex;


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [DataMember]
        public string fromTextBoxText
        {
            get { return _fromTextBoxText; }
            set
            {
                _fromTextBoxText = value;
                NotifyPropertyChanged("fromTextBoxText");
            }
        }

        [DataMember]
        public String toTextBoxText
        {
            get { return _toTextBoxText; }
            set
            {
                _toTextBoxText = value;
                NotifyPropertyChanged("toTextBoxText");
            }
        }


        [DataMember]
        public String fromCoords
        {
            get { return _fromCoords; }
            set
            {
                _fromCoords = value;
                NotifyPropertyChanged("fromCoords");
            }
        }


        [DataMember]
        public String toCoords
        {
            get { return _toCoords; }
            set
            {
                _toCoords = value;
                NotifyPropertyChanged("toCoords");
            }
        }

        [DataMember]
        public String date
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyPropertyChanged("date");
            }
        }

        [DataMember]
        public String time
        {
            get { return _time; }
            set
            {
                _time = value;
                NotifyPropertyChanged("time");
            }
        }

        [DataMember]
        public String timetype
        {
            get { return _timetype; }
            set
            {
                _timetype = value;
                NotifyPropertyChanged("timetype");
            }
        }

        [DataMember]
        public Boolean isFromTextBox
        {
            get { return _isFromTextBox; }
            set
            {
                _isFromTextBox = value;
                NotifyPropertyChanged("isFromTextBox");
            }
        }

        [DataMember]
        public List<String> fromToCoordsList
        {
            get { return _fromToCoordsList; }
            set
            {
                _fromToCoordsList = value;
                NotifyPropertyChanged("fromToCoordsList");
            }
        }

        
        [DataMember]
        public int pivotSelectedIndex
        {
            get { return _pivotSelectedIndex; }
            set
            {
                _pivotSelectedIndex = value;
                NotifyPropertyChanged("pivotSelectedIndex");
            }
        }

    }
}
