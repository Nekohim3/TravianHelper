using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianHelper.TravianEntities
{
    public class Quest : BaseTravianEntity
    {
        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        private int _finalStep;

        public int FinalStep
        {
            get => _finalStep;
            set
            {
                _finalStep = value;
                RaisePropertyChanged(() => FinalStep);
            }
        }

        private int _finishedSteps;

        public int FinishedSteps
        {
            get => _finishedSteps;
            set
            {
                _finishedSteps = value;
                RaisePropertyChanged(() => FinishedSteps);
            }
        }

        private int _progress;

        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                RaisePropertyChanged(() => Progress);
            }
        }

        private int _status;

        public int Status
        {
            get => _status;
            set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }
        
        public bool IsCompleted => FinishedSteps == FinalStep;

        public Quest(dynamic id, dynamic finalStep, dynamic finishedSteps, dynamic progress, dynamic status, dynamic time)
        {
            Id            = id;
            FinalStep     = finalStep;
            FinishedSteps = finishedSteps;
            Progress      = progress;
            Status        = status;

            UpdateTime      = DateTime.Now;
            UpdateTimeStamp = time;
        }
    }
}
