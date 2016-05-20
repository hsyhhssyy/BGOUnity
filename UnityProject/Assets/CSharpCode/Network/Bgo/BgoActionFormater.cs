using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;

namespace Assets.CSharpCode.Network.Bgo
{
    public static class BgoActionFormater
    {
        private static TtaCivilopedia civilopedia = TtaCivilopedia.GetCivilopedia("2.0");

        public static void Format(BgoGame game, String html)
        {
            //Unknown的话
            //Data[0]是opt的显示内容，就是文本
            //Data[1]是optValue（要传给后端的）

            foreach (
                var action in
                    game.PossibleActions.Where(action => action.ActionType == PlayerActionType.Unknown).ToList())
            {
                String bgoStr = action.Data[0].ToString();
                String optValue = action.Data[1].ToString();

                if (bgoStr.StartsWith("Increase"))
                {
                    int foodCost = Convert.ToInt32(bgoStr.CutBetween("(", "F)"));
                    action.ActionType = PlayerActionType.IncreasePopulation;
                    action.Data[0] = bgoStr;
                    action.Data[1] = foodCost;
                    action.Data[2] = optValue;
                }
                else if (bgoStr.StartsWith("Build"))
                {
                    //Build关键字的出现可能有多种情况。包括
                    //Build Iron (5R)
                    //Build free temple/Build free warrior.
                    //Build 1 stage of Library of Alexandria (1R)
                    //Build 4 stages of Library of Alexandria (1R)
                    if (bgoStr.IndexOf("stage", StringComparison.Ordinal) == 8 ||
                        bgoStr.IndexOf("stages", StringComparison.Ordinal) == 8)
                    {
                        //var keyword = bgoStr.Contains("stages") ? "stages" : "stage";
                        //Build X stage of Y (XR)
                        var resCost = Convert.ToInt32(bgoStr.CutBetween("(", "R)"));
                        var stageCount = Convert.ToInt32(bgoStr.CutBetween("Build ", " stage"));

                        var wonderName = bgoStr.CutBetween("of ", " (");
                        action.ActionType = PlayerActionType.BuildWonder;
                        action.Data[0] = civilopedia.GetCardInfoByName(wonderName);
                        action.Data[1] = stageCount;
                        action.Data[2] = resCost;
                        action.Data[3] = optValue;
                    }
                    else if (bgoStr.Contains("("))
                    {
                        //Building
                        String cardName = bgoStr.CutBetween("Build ", " (");
                        int resCost = Convert.ToInt32(bgoStr.CutBetween("(", "R)"));
                        action.ActionType = PlayerActionType.BuildBuilding;
                        action.Data[0] = civilopedia.GetCardInfoByName(cardName);
                        action.Data[1] = resCost;
                        action.Data[2] = optValue;
                    }
                    else
                    {
                        //以Build开头的其他无意义选项
                    }
                }else if (bgoStr.StartsWith("Upgrade"))
                {
                    //Upgrade Agriculture -> Irrigation (2R)
                    var card1 = bgoStr.CutBetween("Upgrade ", " ->");
                    var card2 = bgoStr.CutBetween("> ", " (");
                    var resCost= bgoStr.CutBetween("(", "R)");

                    action.ActionType = PlayerActionType.UpgradeBuilding;
                    action.Data[0] = civilopedia.GetCardInfoByName(card1);
                    action.Data[1] = civilopedia.GetCardInfoByName(card2);
                    action.Data[2] = resCost;
                    action.Data[3] = optValue;
                }
                else if (bgoStr.StartsWith("Play"))
                {
                    //Play关键字的出现可能有多种情况。包括
                    //Play A / Rich Land
                    //Play event Pestilence
                    //Play event Developed Territory II
                    if (bgoStr.Contains("/"))
                    {
                        //Play A / Rich Land
                        var age = bgoStr.CutBetween("Play "," /");
                        var cardName = bgoStr.CutAfter("/ ");
                        var card = civilopedia.GetCardInfoByName((Age) Enum.Parse(typeof (Age), age), cardName);
                        action.ActionType= PlayerActionType.PlayActionCard;
                        action.Data[0] = card;
                        action.Data[1] = optValue;
                    }
                }
                else if (bgoStr.StartsWith("Revolution"))
                {
                    //Revolution! Change to Constitutional Monarchy (6S)
                    var resCost = Convert.ToInt32(bgoStr.CutBetween("(", "S)"));
                    var card = civilopedia.GetCardInfoByName(bgoStr.CutBetween("Change to ", " ("));
                    action.ActionType = PlayerActionType.Revolution;
                    action.Data[0] = card;
                    action.Data[1] = resCost;
                    action.Data[2] = optValue;
                }
                else if (bgoStr.StartsWith("Elect"))
                {
                    //Elect leader: Moses
                    //Elect leader: Joan of Arc (for 1 MA)
                    if (!bgoStr.Contains("("))
                    {
                        var leader = bgoStr.CutAfter(": ");
                        action.ActionType = PlayerActionType.ElectLeader;
                        action.Data[0] = civilopedia.GetCardInfoByName(leader);
                        action.Data[1] = optValue;
                    }
                    else {
                        //MA elect
                    }

                }
                else if (bgoStr.StartsWith("Discover"))
                {
                    var resCost = Convert.ToInt32(bgoStr.CutBetween("(", "S)"));
                    var card = civilopedia.GetCardInfoByName(bgoStr.CutBetween("Discover ", " ("));
                    action.ActionType = PlayerActionType.DevelopTechCard;
                    action.Data[0] = card;
                    action.Data[1] = resCost;
                    action.Data[2] = optValue;
                }
                else if (bgoStr.StartsWith("Reset Action Phase"))
                {
                    action.ActionType = PlayerActionType.ResetActionPhase;
                }
                else if (bgoStr.StartsWith("Pass political phase"))
                {
                    action.ActionType = PlayerActionType.PassPoliticalPhase;
                }
                else if (bgoStr.StartsWith("End Action Phase"))
                {
                    action.ActionType = PlayerActionType.EndActionPhase;
                }
                else if (bgoStr.StartsWith("Set up new tactics"))
                {
                    //Set up new tactics:  Classic Army
                    action.ActionType = PlayerActionType.SetupTactic;
                    var card = civilopedia.GetCardInfoByName(bgoStr.CutAfter(":  "));
                    action.Data[0] = card;
                    action.Data[1] = optValue;
                }
                else if (bgoStr.StartsWith("Adopt tactics"))
                {
                    //Adopt tactics:  Heavy Cavalry
                    action.ActionType = PlayerActionType.AdoptTactic;
                    var card = civilopedia.GetCardInfoByName(bgoStr.CutAfter(":  "));
                    action.Data[0] = card;
                    action.Data[1] = optValue;
                }
            }

            //---删除多余 / 一拆多
            #region disband/destory

            var disband =
                game.PossibleActions.FirstOrDefault(
                    action =>
                        action.ActionType == PlayerActionType.Unknown &&
                        action.Data[0].ToString() == "Disband / Destroy");
            if (disband != null)
            {
                //为每个建筑物建立自己的disband/destory
                game.PossibleActions.Remove(disband);
                var subDropdown = BgoRegexpCollections.ExtractSubDropDown("unite").Match(html);
                if (subDropdown.Success == true)
                {
                    foreach (Match match in BgoRegexpCollections.ExtractActions.Matches(subDropdown.Groups[1].Value))
                    {
                        //8;9 A/ Agriculture
                        var optValue = match.Groups[1].Value;
                        var cardName = match.Groups[2].Value;
                        var cardAge = cardName.CutBefore("/");
                        cardName = cardName.CutAfter("/ ");

                        var card = civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof (Age), cardAge), cardName);

                        if (card.CardType == CardType.MilitaryTechAirForce ||
                            card.CardType == CardType.MilitaryTechArtillery ||
                            card.CardType == CardType.MilitaryTechCavalry ||
                            card.CardType == CardType.MilitaryTechInfantry)
                        {
                            game.PossibleActions.Add(new BgoPlayerAction()
                            {
                                ActionType = PlayerActionType.Disband,
                                Data = new Dictionary<int, object>
                                {
                                    {0,card },
                                    {1,disband.Data[1]},
                                    {2,optValue }

                                }
                            });
                        }
                        else
                        {
                            game.PossibleActions.Add(new BgoPlayerAction()
                            {
                                ActionType = PlayerActionType.Destory,
                                Data = new Dictionary<int, object>
                                {
                                    {0,card },
                                    {1,disband.Data[1]},
                                    {2,optValue }

                                }
                            });
                        }
                    }
                }
            }

            #endregion

            var selectaction =
                game.PossibleActions.FirstOrDefault(
                    action =>
                        action.ActionType == PlayerActionType.Unknown &&
                        action.Data[0].ToString() == "----- Select an action -----");
            if (selectaction != null)
            {
                game.PossibleActions.Remove(selectaction);
            }

            //标记internal
            foreach (var action in game.PossibleActions)
            {
                switch (action.ActionType)
                {
                        case PlayerActionType.PlayActionCard:
                        var card =(CardInfo)action.Data[0];

                        if (card.ImmediateEffects.FirstOrDefault(t=>t.FunctionId== CardEffectType.ChooseOne) != null)
                        {
                            action.Internal = true;
                        }
                        break;
                }
            }
        }

        public static void FormatInternalAction(List<PlayerAction> action, String html)
        {
            
        }
    }
}