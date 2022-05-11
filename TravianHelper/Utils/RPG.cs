using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TravianHelper.Utils
{
    public static class RPG
    {
        public static JObject BuildingDestroy(string session, int villageId, int locationId) =>
            new JObject(
                        new JProperty("action", "destroy"),
                        new JProperty("controller", "building"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("villageId", villageId),
                                                  new JProperty("locationId", locationId))));

        public static JObject BuildingUpgrade(string session, int villageId, int locationId, int buildingType) =>
            new JObject(
                        new JProperty("action", "upgrade"),
                        new JProperty("controller", "building"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("villageId", villageId),
                                                  new JProperty("locationId", locationId),
                                                  new JProperty("buildingType", buildingType))));

        public static JObject DialogAction(string session, int questId, int dialogId, string command, string input = "") =>
            new JObject(
                        new JProperty("action", "dialogAction"),
                        new JProperty("controller", "quest"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("questId", questId),
                                                  new JProperty("dialogId", dialogId),
                                                  new JProperty("command", command),
                                                  new JProperty("input", input))));

        public static JObject HeroAttribute(string session, int fightStrengthPoints, int attBonusPoints, int defBonusPoints, int resBonusPoints, int resBonusType) =>
            new JObject(
                        new JProperty("action", "addAttributePoints"),
                        new JProperty("controller", "hero"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("fightStrengthPoints", fightStrengthPoints),
                                                  new JProperty("attBonusPoints", attBonusPoints),
                                                  new JProperty("defBonusPoints", defBonusPoints),
                                                  new JProperty("resBonusPoints", resBonusPoints),
                                                  new JProperty("resBonusType", resBonusType))));

        public static JObject Trade(string session, int villageId, int offeredResource, int offeredAmount, int searchedResource, int searchedAmount, bool kingdomOnly) =>
            new JObject(
                        new JProperty("action", "createOffer"),
                        new JProperty("controller", "trade"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("villageId", villageId),
                                                  new JProperty("offeredResource", offeredResource),
                                                  new JProperty("offeredAmount", offeredAmount),
                                                  new JProperty("searchedResource", searchedResource),
                                                  new JProperty("searchedAmount", searchedAmount),
                                                  new JProperty("kingdomOnly", kingdomOnly))));

        public static JObject UseHeroItem(string session, int amount, int id, int villageId) =>
            new JObject(
                        new JProperty("action", "useItem"),
                        new JProperty("controller", "hero"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("amount", amount),
                                                  new JProperty("id", id),
                                                  new JProperty("villageId", villageId))));

        public static JObject NpcTrade(string session, int villageId, int r1, int r2, int r3, int r4) =>
            new JObject(
                        new JProperty("action", "bookFeature"),
                        new JProperty("controller", "premiumFeature"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("featureName", "NPCTrader"),
                                                  new JProperty("params",
                                                                new JObject(
                                                                            new JProperty("villageId", villageId),
                                                                            new JProperty("distributeRes",
                                                                                          new JObject(
                                                                                                      new JProperty("1", r1),
                                                                                                      new JProperty("2", r2),
                                                                                                      new JProperty("3", r3),
                                                                                                      new JProperty("4", r4))))))));

        public static JObject SetVillageName(string session, int villageId, string villageName) =>
            new JObject(
                        new JProperty("action", "updateName"),
                        new JProperty("controller", "village"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("villageId", villageId),
                                                  new JProperty("villageName", villageName))));

        public static JObject FinishBuild(string session, int villageId, int price, int queueType) =>
            new JObject(
                        new JProperty("action", "bookFeature"),
                        new JProperty("controller", "premiumFeature"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("featureName", "finishNow"),
                                                  new JProperty("params",
                                                                new JObject(
                                                                            new JProperty("price", price),
                                                                            new JProperty("queueType", queueType),
                                                                            new JProperty("villageId", villageId))))));

        public static JObject RecruitUnits(string session, int villageId, int locationId, int buildingType, string unitId, int count) =>
            new JObject(
                        new JProperty("action", "recruitUnits"),
                        new JProperty("controller", "building"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("villageId", villageId),
                                                  new JProperty("locationId", locationId),
                                                  new JProperty("buildingType", buildingType),
                                                  new JProperty("units",
                                                                new JObject(
                                                                            new JProperty(unitId, count))))));

        public static JObject CollectReward(string session, int villageId, int questId) =>
            new JObject(
                        new JProperty("action", "collectReward"),
                        new JProperty("controller", "quest"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("villageId", villageId),
                                                  new JProperty("questId", questId))));

        public static JObject SendHeroAdv(string session) =>
            JObject.FromObject(new
            {
                action = "dialogAction",
                controller = "quest",
                session,
                @params = new
                {
                    questId = 991,
                    dialogId = 0,
                    command = "activate"
                }
            });

        public static JObject ChooseTribe(string session, int tribeId) =>
            JObject.FromObject(new
            {
                action = "chooseTribe",
                controller = "player",
                session,
                @params = new
                {
                    tribeId
                }
            });

        public static JObject GetOasis(string session, int villageId) =>
            JObject.FromObject(new
                               {
                                   action     = "getOasisList",
                                   controller = "building",
                                   session,
                                   @params = new
                                             {
                                                 villageId
                                             }
                               });

        public static JObject SetOasis(string session, int villageId, int oasisId) =>
            JObject.FromObject(new
                               {
                                   action     = "useOasis",
                                   controller = "village",
                                   session,
                                   @params = new
                                             {
                                                 oasisId,
                                                 villageId
                                             }
                               });

        public static JObject SetBid(string session, int auctionId, int bidAmount) =>
            JObject.FromObject(new
                               {
                                   action     = "placeBid",
                                   controller = "auctions",
                                   session,
                                   @params = new
                                             {
                                                 auctionId,
                                                 bidAmount
                                             }
                               });

        public static JObject GetAuctions(string session, int page) =>
            new JObject(
                        new JProperty("action",     "getRunningAuctionPage"),
                        new JProperty("controller", "auctions"),
                        new JProperty("session",    session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("filterItemType", 0),
                                                  new JProperty("filterSlot", -2),
                                                  new JProperty("page",     page))));

        public static JObject Celeb(string session, int villageId) =>
            JObject.FromObject(new
                               {
                                   action     = "startCelebration",
                                   controller = "building",
                                   session,
                                   @params = new
                                             {
                                                 villageId,
                                                 type = 1
                                             }
                               });

        public static JObject SendTroops(string session, int villageId, int destVillageId, int movementType, bool redeployHero,
                                         string spyMission, int t1, int t2, int t3, int t4, int t5, int t6, int t7, int t8, int t9,
                                         int t10, int t11) =>
            new JObject(
                        new JProperty("action", "send"),
                        new JProperty("controller", "troops"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("destVillageId", destVillageId),
                                                  new JProperty("movementType", movementType),
                                                  new JProperty("redeployHero", redeployHero),
                                                  new JProperty("spyMission", spyMission),
                                                  new JProperty("villageId", villageId),
                                                  new JProperty("units",
                                                                new JObject(
                                                                            new JProperty("1", t1),
                                                                            new JProperty("2", t2),
                                                                            new JProperty("3", t3),
                                                                            new JProperty("4", t4),
                                                                            new JProperty("5", t5),
                                                                            new JProperty("6", t6),
                                                                            new JProperty("7", t7),
                                                                            new JProperty("8", t8),
                                                                            new JProperty("9", t9),
                                                                            new JProperty("10", t10),
                                                                            new JProperty("11", t11))))));

        public static JObject GetCache(string session, List<string> lst) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                JArray.Parse($"['{string.Join("','", lst)}']")))));

        public static JObject GetCache_Player(string session, int playerId) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"Player:{playerId}")))));

        public static JObject GetCache_VillageList(string session) =>
            new JObject(
                        new JProperty("action",     "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session",    session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray("Collection:Village:own"))))); 
        
        public static JObject GetCache_Voucher(string session, int playerId) =>
            new JObject(
                        new JProperty("action",     "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session",    session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"Voucher:{playerId}")))));

        public static JObject GetCache_All(string session) =>
            new JObject(
                        new JProperty("action", "getAll"),
                        new JProperty("controller", "player"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty(
                                                                "deviceDimension", "1920:1080"
                                                               ))));

        public static JObject GetCache_Hero(string session, int playerId) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"Hero:{playerId}")))));

        public static JObject GetCache_BuildingQueue(string session, int villageId) =>
            new JObject(
                        new JProperty("action",     "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session",    session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"BuildingQueue:{villageId}")))));

        public static JObject GetCache_UnitQueue(string session, int villageId) =>
            new JObject(
                        new JProperty("action",     "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session",    session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"UnitQueue:{villageId}")))));

        public static JObject GetCache_BuildingCollection(string session, int villageId) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"Collection:Building:{villageId}")))));

        public static JObject GetCache_Building(string session, int buildingId) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"Building:{buildingId}")))));

        public static JObject GetAvailableBuildingList(string session, int villageId, int locationId) =>
            new JObject(
                        new JProperty("action", "getBuildingList"),
                        new JProperty("controller", "building"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("villageId", villageId),
                                                  new JProperty("locationId", locationId))));

        public static JObject GetCache_MovingTroopsCollection(string session, int villageId) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"Collection:Troops:moving:{villageId}")))));

        public static JObject GetCache_StationaryTroopsCollection(string session, int villageId) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"Collection:Troops:stationary:{villageId}")))));

        public static JObject GetCache_Session(string session) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray($"Session:{session}")))));
        public static JObject GetCache_CollectionHeroItemOwn(string session) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray("Collection:HeroItem:own")))));
        public static JObject GetCache_Quest(string session) =>
            new JObject(
                        new JProperty("action", "get"),
                        new JProperty("controller", "cache"),
                        new JProperty("session", session),
                        new JProperty("params",
                                      new JObject(
                                                  new JProperty("names",
                                                                new JArray("Collection:Quest:")))));

        private static string[] GetVillageIds(int villageId)
        {
            var start = villageId - 32768 * 5 - 5;
            var arr = new List<string>();
            for (var i = 0; i < 10; i++)
                for (var j = 0; j < 10; j++)
                {
                    arr.Add($"MapDetails:{start + i * 32768 + j}");
                }
            return arr.ToArray();
        }

        public static JObject GetCache_MapDetails(string session, int villageId) =>
            JObject.FromObject(new
            {
                action = "get",
                controller = "cache",
                session,
                @params = new
                {
                    names = GetVillageIds(villageId)
                }
            });

        public static JObject SolvePuzzle(string session, JArray moves) =>
            JObject.FromObject(new
            {
                action = "solvePuzzle",
                controller = "quest",
                session,
                @params = new
                {
                    moves
                }
            });

        public static JObject GetPuzzle(string session) =>
        JObject.FromObject(new
        {
            action = "getPuzzle",
            controller = "quest",
            session,
            @params = new
            {

            }
        });
    }
}
