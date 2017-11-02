using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Network.Bgo.DataStructure;

namespace Assets.CSharpCode.Network.Bgo
{
    public static class BgoActionFormater
    {
        private static readonly TtaCivilopedia Civilopedia = TtaCivilopedia.GetCivilopedia("2.0");

        public static void Format(BgoGame game, String html)
        {
            //如果当前是EventResolve，那么没有CardRow
            if (game.CurrentPhase == TtaPhase.EventResolution)
            {
                game.PossibleActions.RemoveAll(a => a.ActionType == PlayerActionType.TakeCardFromCardRow);

                var mc = BgoRegexpCollections.ExtractResolveingEvent.Match(html);
                var card = Civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), mc.Groups[2].Value), mc.Groups[1].Value);

                foreach (
                    var action in
                        game.PossibleActions.Where(action => action.ActionType == PlayerActionType.Unknown).ToList())
                {
                    String bgoStr = action.Data[0].ToString();
                    String optValue = action.Data[1].ToString();

                    if (bgoStr == "----- Select an action -----")
                    {
                        continue;
                    }

                    action.ActionType = PlayerActionType.ResolveEventOption;

                    //获得CurrentActionCard
                    action.Data[0] = card;
                    action.Data[1] = bgoStr;
                    action.Data[2] = optValue;
                }
            }else if (game.CurrentPhase == TtaPhase.SendColonists)
            {
                game.PossibleActions.RemoveAll(a => a.ActionType == PlayerActionType.TakeCardFromCardRow);

                foreach (
                    var action in
                        game.PossibleActions.Where(action => action.ActionType == PlayerActionType.Unknown).ToList())
                {
                    String bgoStr = action.Data[0].ToString();
                    String optValue = action.Data[1].ToString();

                    if (bgoStr != "Send task force")
                    {
                        continue;
                    }

                    action.ActionType = PlayerActionType.SendColonists;
                    
                    var mc = BgoRegexpCollections.ExtractSendColonists.Match(html);
                    var cardName = mc.Groups[1].Value;
                    cardName = cardName.Substring(0, cardName.IndexOf("(")).Trim();
                    var card = Civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), mc.Groups[2].Value), cardName);

                    action.Data[0] =card;
                    action.Data[1] = Convert.ToInt32(mc.Groups[3].Value);
                    action.Data[2] = new List<CardInfo>();

                    var mapdict=new List<BgoAdditionalFormTuple>();
                    var saMc = BgoRegexpCollections.ExtractSendColoinstsSacrifice.Matches(html);
                    foreach (Match m in saMc)
                    {
                        BgoAdditionalFormTuple tuple=new BgoAdditionalFormTuple();
                        tuple.formValue = m.Groups[1].Value;
                        tuple.formName = m.Groups[2].Value;
                        var tupCard = Civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), mc.Groups[6].Value), mc.Groups[8].Value.Trim());
                        tuple.card = tupCard;
                        mapdict.Add(tuple);
                    }

                    action.Data[3] = bgoStr;
                    action.Data[4] = optValue;
                    action.Data[5] = mapdict;
                }
            }
            else if (game.CurrentPhase == TtaPhase.Colonize)
            {
                game.PossibleActions.RemoveAll(a => a.ActionType == PlayerActionType.TakeCardFromCardRow);

                var mc = BgoRegexpCollections.ExtractResolveingEvent.Match(html);
                var cardName = mc.Groups[1].Value;
                cardName = cardName.Substring(0, cardName.IndexOf("(")).Trim();
                var card = Civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), mc.Groups[2].Value), cardName);

                foreach (
                    var action in
                        game.PossibleActions.Where(action => action.ActionType == PlayerActionType.Unknown).ToList())
                {
                    String bgoStr = action.Data[0].ToString();
                    String optValue = action.Data[1].ToString();

                    if (bgoStr == "----- Select an action -----")
                    {
                        continue;
                    }

                    action.ActionType = PlayerActionType.ColonizeBid;

                    //获得CurrentActionCard
                    action.Data[0] = card;
                    action.Data[1] = bgoStr;
                    action.Data[2] = optValue;
                }
            }

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
                    action.Data[0] = foodCost;
                    action.Data[1] = bgoStr;
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
                        action.Data[0] = Civilopedia.GetCardInfoByName(wonderName);
                        action.Data[1] = stageCount;
                        action.Data[2] = resCost;
                        action.Data[3] = 1;
                        action.Data[4] = optValue;
                    }
                    else if (bgoStr.Contains("("))
                    {
                        //Building
                        String cardName = bgoStr.CutBetween("Build ", " (");
                        int resCost = Convert.ToInt32(bgoStr.CutBetween("(", "R)"));
                        action.ActionType = PlayerActionType.BuildBuilding;
                        action.Data[0] = Civilopedia.GetCardInfoByName(cardName);
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
                    action.Data[0] = Civilopedia.GetCardInfoByName(card1);
                    action.Data[1] = Civilopedia.GetCardInfoByName(card2);
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
                        var card = Civilopedia.GetCardInfoByName((Age) Enum.Parse(typeof (Age), age), cardName);
                        action.ActionType= PlayerActionType.PlayActionCard;
                        
                        action.Internal = IfCardIsInternal(card);

                        action.Data[0] = card;
                        action.Data[1] = optValue;
                    }else if (bgoStr.StartsWith("Play event"))
                    {
                        //Play event Pestilence
                        //Play event Developed Territory II

                        if (bgoStr.EndsWith(" A") || bgoStr.EndsWith(" I") || bgoStr.EndsWith(" II") ||
                            bgoStr.EndsWith(" III"))
                        {
                            var conlonySplit = bgoStr.Split(" ".ToCharArray());
                            var conlonyName = "";
                            for (var j = 2; j < conlonySplit.Length - 1; j++)
                            {
                                conlonyName += conlonySplit[j] + " ";
                            }
                            conlonyName=conlonyName.Trim(' ');

                            var card = Civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof(Age), conlonySplit[conlonySplit.Length-1]), conlonyName);
                            action.ActionType = PlayerActionType.PlayColony;
                            action.Data[0] = card;
                            action.Data[1] = optValue;
                        }
                        else
                        {
                            //Play event Pestilence
                            var card = Civilopedia.GetCardInfoByName(bgoStr.CutAfter("Play event "));
                            action.ActionType = PlayerActionType.PlayEvent;
                            action.Data[0] = card;
                            action.Data[1] = optValue;
                        }
                    }
                }
                else if (bgoStr.StartsWith("Revolution"))
                {
                    //Revolution! Change to Constitutional Monarchy (6S)
                    var resCost = Convert.ToInt32(bgoStr.CutBetween("(", "S)"));
                    var card = Civilopedia.GetCardInfoByName(bgoStr.CutBetween("Change to ", " ("));
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
                        action.Data[0] = Civilopedia.GetCardInfoByName(leader);
                        action.Data[1] = optValue;
                    }
                    else
                    {
                        //MA elect
                    }

                }
                else if (bgoStr.StartsWith("Discover"))
                {
                    var resCost = Convert.ToInt32(bgoStr.CutBetween("(", "S)"));
                    var card = Civilopedia.GetCardInfoByName(bgoStr.CutBetween("Discover ", " ("));
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
                    var card = Civilopedia.GetCardInfoByName(bgoStr.CutAfter(":  "));
                    action.Data[0] = card;
                    action.Data[1] = optValue;
                }
                else if (bgoStr.StartsWith("Adopt tactics"))
                {
                    //Adopt tactics:  Heavy Cavalry
                    action.ActionType = PlayerActionType.AdoptTactic;
                    var card = Civilopedia.GetCardInfoByName(bgoStr.CutAfter(":  "));
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
                if (subDropdown.Success)
                {
                    foreach (Match match in BgoRegexpCollections.ExtractActions.Matches(subDropdown.Groups[1].Value))
                    {
                        //8;9 A/ Agriculture
                        var optValue = match.Groups[1].Value;
                        var cardName = match.Groups[2].Value;
                        var cardAge = cardName.CutBefore("/");
                        cardName = cardName.CutAfter("/ ");

                        var card = Civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof (Age), cardAge), cardName);

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

            #region 侵略/战争一拆多

            var aggression =
                game.PossibleActions.Where(
                    action =>
                        action.ActionType == PlayerActionType.Unknown && (
                            action.Data[0].ToString().EndsWith(" A") || action.Data[0].ToString().EndsWith(" I") ||
                            action.Data[0].ToString().EndsWith(" II") || action.Data[0].ToString().EndsWith(" III")))
                    .ToList();

            foreach (var action in aggression)
            {
                game.PossibleActions.Remove(action);
                
                String bgoStr = action.Data[0].ToString();
                String optValue = action.Data[1].ToString();

                var cardName = bgoStr.Substring(0,bgoStr.LastIndexOf(" ", StringComparison.Ordinal)).Replace("Declare ","").Trim();
                var cardAge = bgoStr.Substring(bgoStr.LastIndexOf(" ", StringComparison.Ordinal)+1);
                var card = Civilopedia.GetCardInfoByName(
                    (Age)Enum.Parse(typeof(Age), cardAge)
                    , cardName);

                var advNum = optValue.CutAfter(";");
                var subDropdown = BgoRegexpCollections.ExtractWarAggressionPactTargetList("listeAdversaires" + advNum).Match(html);
                if (subDropdown.Success)
                {
                    foreach (Match match in BgoRegexpCollections.ExtractWarAggressionPactTarget.Matches(subDropdown.Groups[1].Value))
                    {
                        //var childId = match.Groups[1].Value;
                        var childOpt = match.Groups[2].Value;
                        var playername = match.Groups[3].Value.CutBefore(" -");
                        var maRequired = BgoRegexpCollections.ExtractImage.Matches(match.Groups[4].Value).Count;
                        game.PossibleActions.Add(new BgoPlayerAction
                        {
                            ActionType = PlayerActionType.Aggression,
                            Data = new Dictionary<int, object>
                                {
                                    {0,card },
                                    {1,playername},
                                    {2,maRequired },
                                    {3,optValue },
                                    {4,"adversaire"+advNum },
                                    {5,childOpt }
                                }
                        });
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
            
        }

        public static void FormatInternalAction(List<PlayerAction> actions, String html)
        {

            foreach (
                var action in
                    actions.Where(action => action.ActionType == PlayerActionType.Unknown).ToList())
            {
                String bgoStr = action.Data[0].ToString();
                String optValue = action.Data[1].ToString();
                String idCarte = action.Data[2].ToString();

                if (bgoStr.StartsWith("Discover"))
                {
                    //Discover Irrigation - 3
                    var resCost = Convert.ToInt32(bgoStr.CutAfter("- "));
                    var card = Civilopedia.GetCardInfoByName(bgoStr.CutBetween("Discover ", " -"));
                    action.ActionType = PlayerActionType.DevelopTechCard;
                    action.Data[0] = card;
                    action.Data[1] = resCost;
                    action.Data[2] = optValue;
                    action.Data[3] = idCarte;
                }
                if (bgoStr.StartsWith("Build"))
                {
                    //Build Philosophy - 1
                    var price = Convert.ToInt32(bgoStr.CutAfter("- "));
                    var card = Civilopedia.GetCardInfoByName(bgoStr.CutBetween("Build ", " -"));
                    action.ActionType = PlayerActionType.BuildBuilding;
                    action.Data[0] = card;
                    action.Data[1] = price;
                    action.Data[2] = optValue;
                    action.Data[3] = idCarte;
                }
                if (bgoStr.StartsWith("Upgrade"))
                {
                    //Upgrade Philosophy to Alchemy - 1
                    var price = Convert.ToInt32(bgoStr.CutAfter("- "));
                    var card1 = Civilopedia.GetCardInfoByName(bgoStr.CutBetween("Upgrade ", " to"));
                    var card2 = Civilopedia.GetCardInfoByName(bgoStr.CutBetween(" to ", " -"));
                    action.ActionType = PlayerActionType.UpgradeBuilding;
                    action.Data[0] = card1;
                    action.Data[1] = card2;
                    action.Data[2] = price;
                    action.Data[3] = optValue;
                    action.Data[4] = idCarte;
                }
            }
        }

        public static bool IfCardIsInternal(CardInfo info)
        {
            if (info.CardType != CardType.Action)
            {
                return false;
            }
            var effectList = new List<CardEffect>();
            foreach (var immediateEffect in info.ImmediateEffects)
            {
                PutItIntoList(immediateEffect,effectList);
            }

            if (effectList.Exists(e => e.FunctionId == CardEffectType.ChooseOne
                                       || e.FunctionId == CardEffectType.E408||
                                       e.FunctionId == CardEffectType.E402 ||
                                       e.FunctionId == CardEffectType.E403 ||
                                       e.FunctionId == CardEffectType.E404 ||
                                       e.FunctionId == CardEffectType.E405))
            {
                return true;
            }

            return false;
        }

        private static void PutItIntoList(CardEffect effect, List<CardEffect> list)
        {
            list.Add(effect);

            if (effect.FunctionId == CardEffectType.ChooseOne)
            {
                var choose = effect as ChooseOneCardEffect;
                if (choose == null)
                {
                    return;
                }

                foreach (var e in choose.Candidate)
                {
                    PutItIntoList(e, list);
                }
            }
        }
    }
}