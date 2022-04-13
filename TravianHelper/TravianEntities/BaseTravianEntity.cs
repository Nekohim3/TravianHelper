using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using Newtonsoft.Json;

namespace TravianHelper.TravianEntities
{
    public class BaseTravianEntity : NotificationObject
    {
        private string _desc;
        [JsonIgnore]
        public string Desc
        {
            get => _desc;
            set
            {
                _desc = value;
                RaisePropertyChanged(() => Desc);
            }
        }

        private DateTime _updateTime = DateTime.MinValue;

        [JsonIgnore]
        public DateTime UpdateTime
        {
            get => _updateTime;
            set
            {
                _updateTime = value;
                RaisePropertyChanged(() => UpdateTime);
            }
        }

        private long _updateTimeStamp;

        [JsonIgnore]
        public long UpdateTimeStamp
        {
            get => _updateTimeStamp;
            set
            {
                _updateTimeStamp = value;
                if (_updateTimeStamp > 0)
                    _updateTimeStamp /= 1000;
                RaisePropertyChanged(() => UpdateTimeStamp);
            }
        }
    }
}
