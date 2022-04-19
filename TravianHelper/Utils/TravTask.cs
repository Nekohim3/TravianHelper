using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils
{
    public enum TaskState
    {
        Queue,
        InProgress,
        Finished,
        Error
    }

    public class TravTask : NotificationObject
    {
        private string _display;

        public string Display
        {
            get => _display;
            set
            {
                _display = value;
                RaisePropertyChanged(() => Display);
            }
        }

        private TaskState _state;

        public TaskState State
        {
            get => _state;
            set
            {
                _state = value;
                RaisePropertyChanged(() => State);
            }
        }

        private Account _account;

        public Account Account
        {
            get => _account;
            set
            {
                _account = value;
                RaisePropertyChanged(() => Account);
            }
        }

        private string _command;

        public TravTask(Account acc, string str)
        {
            Account  = acc;
            _command = str;
            CommandParse();
        }
        
        private void CommandParse(string str = "")
        {
            if(!string.IsNullOrEmpty(str))
                _command = str;

        }



        /// <summary>
        /// Для всего указывается деревня
        ///
        /// 
        /// Для Building:Type и Location не должны быть одновременно 0;
        /// Если Type == 0 то беред здание по Location. Если там пусто или не соответствует комманде(руины при комманде на улучшение или снос) то Error
        /// QueueType для FinishNow определяется автоматом
        ///
        /// Для CollectReward: Id или 0. При 0 собирает все.
        ///
        /// Для RecruitUnits Type и Location можно не указывать.
        /// римляне +10 к uid, галлы +20
        ///
        /// NpcTrade остальное поровну
        /// </summary>
        //BuildingUpgrade:vid:Type:Location(0 for find):BuildIfNotExist(0/1):FinishNow(0/1):Price(-1(item)/0(5min)/1(gold));Comment
        //BU:10:25:1:0:0;

        //BuildingDestroy:vid:Type:Location(0 for find):FinishNow:Price;Comment
        //BD:10:25:1:0;

        //BuildingCollect:vid:Type:Location(0 for find):FinishNow:Price;Comment
        //BC:10:25:1:0;

        //CollectReward:vid:Id;
        //CR:0

        //RecruitUnits:vid:Type:Location:UnitId:Amount
        //RU:29:19:11:15;

        //NpcTrade:vid:Wood:Clay:Iron:Crop
        //NT:5815:3790:3620:1345

        //UseItem:vid:ItemId:Amount;
        //UI:112:15

        //HeroAttribute:fightPoint:resPoint:resType
        //HA:0:4:1;

        //Wait:seconds
        //WT:700;

        //Trade:vid
        //TR:vid;

        //DialogAction:QuestId:DialogId:cmd:input
        //DA:1:1:setName:{AccountName};

        //ChooseTribe:tribeId
        //CT:2;

        //SetSettings
        //SS;

        //RegMail
        //RM;

        //GetLink
        //GL;

        //Activate
        //AT;

        //SendTroops:destVillageId:MoveType:redeployHero:c1:c2:c3:c4:c5:c6:c7:c8:c9:c10:c11 (-1 for max)
        //ST:536920065:3:0:12:0:0:0:0:0:0:0:0:0:1;

        //SolveMap
        //SM;

        //SendRobber:vid
        //SR:vid

        //Login
        //LG;

    }
}
