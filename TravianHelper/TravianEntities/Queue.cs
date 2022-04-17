using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;

namespace TravianHelper.TravianEntities
{
    public class VillageQueue : BaseTravianEntity
    {
        private List<Queue> _queueList = new List<Queue>();

        public List<Queue> QueueList
        {
            get => _queueList;
            set
            {
                _queueList = value;
                RaisePropertyChanged(() => QueueList);
            }
        }

        private List<FreeSlot> _freeSlotLIst = new List<FreeSlot>();

        public List<FreeSlot> FreeSlotLIst
        {
            get => _freeSlotLIst;
            set
            {
                _freeSlotLIst = value;
                RaisePropertyChanged(() => FreeSlotLIst);
            }
        }

        public VillageQueue(dynamic d, dynamic time)
        {
            QueueList.Clear();
            FreeSlotLIst.Clear();
            if (d.queues["1"].Count != 0)
                QueueList.Add(new Queue(1, d.queues["1"][0].buildingType, d.queues["1"][0].locationId, d.queues["1"][0].finished));
            if (d.queues["2"].Count != 0)
                QueueList.Add(new Queue(2, d.queues["2"][0].buildingType, d.queues["2"][0].locationId, d.queues["2"][0].finished));
            if (d.queues["4"].Count != 0)
                foreach (var x in d.queues["4"])
                    QueueList.Add(new Queue(4, x.buildingType, x.locationId, x.finished));
            if (d.queues["5"].Count != 0)
                QueueList.Add(new Queue(1, d.queues["5"][0].buildingType, d.queues["5"][0].locationId, d.queues["5"][0].finished));

            UpdateTime      = DateTime.Now;
            UpdateTimeStamp = time;
        }
    }

    public class Queue : NotificationObject
    {
        private int _queueId;

        public int QueueId
        {
            get => _queueId;
            set
            {
                _queueId = value;
                RaisePropertyChanged(() => QueueId);
            }
        }

        private int _buildingType;

        public int BuildingType
        {
            get => _buildingType;
            set
            {
                _buildingType = value;
                RaisePropertyChanged(() => BuildingType);
            }
        }

        private int _buildingLocation;

        public int BuildingLocation
        {
            get => _buildingLocation;
            set
            {
                _buildingLocation = value;
                RaisePropertyChanged(() => BuildingLocation);
            }
        }

        private int _finishTime;

        public int FinishTime
        {
            get => _finishTime;
            set
            {
                _finishTime = value;
                RaisePropertyChanged(() => FinishTime);
            }
        }

        public Queue(dynamic queueId, dynamic buildingType, dynamic buildingLocation, dynamic finishTime)
        {
            QueueId = queueId;
            BuildingType = buildingType;
            BuildingLocation = buildingLocation;
            FinishTime = finishTime;
        }
    }

    public class FreeSlot : NotificationObject
    {
        private int _idSlot;

        public int IdSlot
        {
            get => _idSlot;
            set
            {
                _idSlot = value;
                RaisePropertyChanged(() => IdSlot);
            }
        }

        private int _freeCount;

        public int FreeCount
        {
            get => _freeCount;
            set
            {
                _freeCount = value;
                RaisePropertyChanged(() => FreeCount);
            }
        }

        public FreeSlot(dynamic idSlot, dynamic freeCount)
        {
            IdSlot = idSlot;
            FreeCount = freeCount;
        }
    }

}
