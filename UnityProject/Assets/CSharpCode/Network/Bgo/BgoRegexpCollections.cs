using System.Text.RegularExpressions;

namespace Assets.CSharpCode.Network.Bgo
{
    public static class BgoRegexpCollections
    {
        /// <summary>
        /// 这个可以在My games页面匹配游戏列表，1是id，2是版本，3是nat，4是名字<b/>
        /// </summary>
        //<td rowspan="1" class="tabPartiesTexteC tabPartiesFond\d">(\d*?)</td><td rowspan="1" class="tabPartiesTexteFinG tabPartiesFond\d">([\s\S]*?)<[\s\S]*?nat=(\d)">([\s\S]*?)<
        public static readonly Regex ListGamesInMyGamePage = new Regex(@"<td rowspan=""1"" class=""tabPartiesTexteC tabPartiesFond\d"">(\d*?)</td><td rowspan=""1"" class=""tabPartiesTexteFinG tabPartiesFond\d"">([\s\S]*?)<[\s\S]*?nat=(\d)"">([\s\S]*?)<");
        /// <summary>
        /// 这个Regex可以查找P标签
        /// </summary>
        public static readonly Regex FindP = new Regex(@"<p.*?>(.*?)</p>");
        /// <summary>
        /// 这个用于分析UL(也就是鼠标悬停提示）box内的东西
        /// </summary>
        //p class=.ageCarte ageCarte20.>([^<]*?)<[\s\S]*?p class=.tta_event1 nomCarte nomCarteMilitaire.>([^<]*?)<
        public static readonly Regex FindCardInfoFromUnorderedList = new Regex(@"p class=.ageCarte ageCarte20.>([^<]*?)<[\s\S]*?p class=.tta_event1 nomCarte nomCarteMilitaire.>([^<]*?)<");

        //happy.png
        //unhappy.png
        public static readonly Regex ExtractPlayerNameAndResourceHappy = new Regex(@"/happy.png");
        public static readonly Regex ExtractPlayerNameAndResourceUnhappy = new Regex(@"/unhappy.png");
        //missing
        public static readonly Regex ExtractGovenrmentAndActionPointsMissing = new Regex(@"/missing");

        /// <summary>
        /// 这个可以测试我的名字，但是要注意需要替换&nbsp;
        /// </summary>
        public static readonly Regex ExtractMyName = new Regex(@"connect_ok[^<]*?<[^>]*?>([^<]*?)<");

        /// <summary>
        /// 这个可以匹配卡牌列. Group 2 是 post url, 3 是 idNote ，4是a的class，5是 age， 6是card name
        /// 8是需要的CA，没有CA可能是免费拿
        /// postUrl为空就是拿不了的卡
        /// 4的Class里，有carteEnMain的就是可以放回去的卡
        /// </summary>
        //(<form method="post" id="piocherCarte\d*?" action="([\S]*?)">)?<input type="hidden" name="idNote" value="([\S]*?)"[\s\S]{0,300}?<a class="([\s\S]*?)" onClick="[\s\S]*?">[\s\S]*?<p class="ageCarte ageCarte1x">(\S*?)</p>[\s\S]*?<p class="nomCarte">([\s\S]*?)(<br />)?</p>[\s\S]*?<p>([\s\S]*?)</p>
        public static readonly Regex ExtractCardRow = new Regex(@"(<form method=""post"" id=""piocherCarte\d*?"" action=""([\S]*?)"">)?<input type=""hidden"" name=""idNote"" value=""([\S]*?)""[\s\S]{0,300}?<a class=""([\s\S]*?)"" onClick=""[\s\S]*?"">[\s\S]*?<p class=""ageCarte ageCarte1x"">(\S*?)</p>[\s\S]*?<p class=""nomCarte"">([\s\S]*?)(<br />)?</p>[\s\S]*?<p>([\s\S]*?)</p>");

        //<a onClick="\S*?"><img class="imageLeader" \S*?  alt="([\s\S]*?)"
        //Group 1是当前领袖

        //<div id="Plateau(\d)"( style="\S*?")? class="plateau plateau\d"( style="\S*?")?>
        //寻找玩家面板位置，前面的是玩家面板，后面两个是Journal和Chat，1是玩家编号，2为空的才是玩家，3为空的是系统版
        public static readonly Regex ExtractPlayerPlate = new Regex(@"<div id=""Plateau(\d)""( style=""\S*?"")? class=""plateau plateau\d""( style=""\S*?"")?>");

        //>([^>]+?)<ul id="indJoueur">((<li class="ind\S*? ind">[\s\S]*?){7})</ul>
        //1是两个玩家的名字，和他们的面板顺序一致 2是当前资源，用下面的regex来找，注意这个不在PlayerPlate里
        public static readonly Regex ExtractPlayerNameAndResource = new Regex(@">([^>]+?)<ul id=""indJoueur"">((<li class=""ind\S*? ind"">[\s\S]*?){7})</ul>");

        //class="ind(\S*?) ind">([\s\S]*?<)(/li>)?(li|/ul)
        //这个regex先把这7个切出来，然后再用下面的regex具体分析,1是资源类型，2是值
        public static readonly Regex ExtractPlayerNameAndResourceCutResourceOut = new Regex(@"class=""ind(\S*?) ind"">([\s\S]*?<)(/li>)?(li|/ul)");

        #region 分析资源用的Regex
        //(\d+?)( \|[\s\S]*?>([+-]?\d*?))?<
        //分析资源，1是当前值，3是加值
        public static readonly Regex ExtractPlayerNameAndResourceNormal = new Regex(@"(\d+?)( \|[\s\S]*?>([+-]?\d*?))?<");
        //(\d+?)(\+\d+?)?( \|[\s\S]*?>([+-]?\d*?))?<
        //分析资源，1是当前值，2是特殊当前值，4是加值
        public static readonly Regex ExtractPlayerNameAndResourceSpecial = new Regex(@"(\d+?)(\+\d+?)?( \|[\s\S]*?>([+-]?\d*?))?<");
        #endregion

        //<div id="statusBar" class="statusActive">([\s\S]*?)</div>
        //这个先把statusbar切出来，然后再分析

        #region 分析建筑面板

        /// <summary>
        /// 这个Regex会把BuildingBoard切出来，1就是table中间的内容，不含table标签
        /// </summary>
        //<p class="commentaire">&nbsp;</p><table class="tableau2"[\s\S]*?>([\s\S]*?)</table>
        public static readonly Regex ExtractBuildingBoard=new Regex(@"<p class=""commentaire"">&nbsp;</p><table class=""tableau2""[\s\S]*?>([\s\S]*?)</table>");
        /// <summary>
        /// 这个会把BuildingBoard的row切出来，注意第一行是表头，用FindP能切开表头
        /// </summary>
        //<tr>([\s\S]*?)(?=(</tr>|<tr>|$))
        public static readonly Regex ExtractBuildingBoardRow = new Regex(@"<tr>([\s\S]*?)(?=(</tr>|<tr>|$))");
        /// <summary>
        /// 这个能切开单元格，对于空白单元格，其1的内容就是"&amp;nbsp;"。<br/>
        /// 第一列是Age：&lt;p class="ageBatiments">Age&amp;nbsp;A&lt;/p>，可以用FindP来找Age。<br/>
        /// 对于切出来的单元格，通过FindP，可以找到两行，第一行是建筑物名字，第二行是当前建筑的数量(需要用下面的Regex来数）
        /// </summary>
        //<td.*?>([\s\S]*?)(?=(</td>|<td>))
        public static readonly Regex ExtractBuildingBoardCell=new Regex(@"<td.*?>([\s\S]*?)(?=(</td>|<td>))");
        /// <summary>
        /// 这个Regex可以确定建筑物的数量
        /// </summary>
        public static readonly Regex ExtractBuildingBoardBuidingCount = new Regex(@"<img class=""icon");
        /// <summary>
        /// 这个可以确定单元格里资源的数量
        /// </summary>
        //<img class="icon
        public static readonly Regex ExtractBuildingBoardResourceCount = new Regex(@"<img class=""icone15");
        /// <summary>
        /// 这个确认造价和产出，第一个1是造价，第二个1是产出
        /// </summary>
        //[^\d](\d*?)&nbsp;<img[^>]*? class="iconeTexte".*? />
        public static readonly Regex ExtractBuildingBoardPriceAndProduction = new Regex(@"[^\d](\d*?)&nbsp;<img[^>]*? class=""iconeTexte"".*? />");
        #endregion

        //<p class="nomModule">[\s\S]*?<span class="infoModule">Age([\s\S]*?) round (\d*?)</span></p>
        public static readonly Regex ExtractAgeAndRound = new Regex(@"<p class=""nomModule"">[\s\S]*?<span class=""infoModule"">Age([\s\S]*?) round (\d*?)</span></p>");

        //<p class="titre3">Current[\s\S]*?((<p class="ageDosCarte">(.*?)</p>)|(<p class="ageCarte ageCarte20">(.*?)</p>[\s\S]*?class="nomCarte">(.*?)<br))[\s\S]*?<p class="nombre">(.*?)</p>
        /// <summary>
        /// 查找当前事件，由于贞德，这个的使用方法比较特别。
        /// 有两种可能
        /// 1、无贞德，Group3是当前牌时代（I），7是当前事件分布数字（1+2+3）
        /// 2、有贞德，Group3为空，5是当前牌时代，6是当前牌名字，7是分布数字
        /// </summary>
        public static readonly Regex ExtractCurrentEvent=new Regex(@"<p class=""titre3"">Current[\s\S]*?((<p class=""ageDosCarte"">(.*?)</p>)|(<p class=""ageCarte ageCarte20"">(.*?)</p>[\s\S]*?class=""nomCarte"">(.*?)<br))[\s\S]*?<p class=""nombre"">(.*?)</p>");

        //<p class="titre3">Future[\s\S]*?<p class="ageDosCarte">(.*?)</p>[\s\S]*?<p class="nombre">(.*?)</p>
        //未来事件，2是分布数字，注意如果group1的Length>1（暂定），那么就表示里面是img，就是那个none.img
        public static readonly Regex ExtractFutureEvent=new Regex(@"<p class=""titre3"">Future[\s\S]*?<p class=""ageDosCarte"">(.*?)</p>[\s\S]*?<p class=""nombre"">(.*?)</p>");

        //<p class="titre3">Civil[\s\S]*?<p class="ageDosCarte">(.*?)</p>[\s\S]*?<p class="nombre">(.*?)</p>
        //内政卡剩余，3是分布数字，注意如果group2的Length>4（暂定），那么就表示里面是img，就是那个none.img
        public static readonly Regex ExtractCivilCardRemains = new Regex(@"<p class=""titre3"">Civil[\s\S]*?<p class=""ageDosCarte"">(.*?)</p>[\s\S]*?<p class=""nombre"">(.*?)</p>");
        //军事卡剩余，3是分布数字，注意如果group2的Length>4（暂定），那么就表示里面是img，就是那个none.img
        //<p class="titre3">Military[\s\S]*?<p class="ageDosCarte">(.*?)</p>[\s\S]*?<p class="nombre">(.*?)</p>
        public static readonly Regex ExtractMilitryCardRemains = new Regex(@"<p class=""titre3"">Military[\s\S]*?<p class=""ageDosCarte"">(.*?)</p>[\s\S]*?<p class=""nombre"">(.*?)</p>");


        //<a [^>]*?><img class="imageLeader".*? alt="(.*?)"
        //查找玩家的领袖，没发现就是没领袖
        public static readonly Regex ExtractLeader=new Regex(@"<a [^>]*?><img class=""imageLeader"".*? alt=""(.*?)""");
        
        //<div class="worker_pool">([\s\S]*?)</div>
        /// <summary>
        /// 闲置工人
        /// </summary>
        public static readonly Regex ExtractWorkerPool=new Regex(@"<div class=""worker_pool"">([\s\S]*?)</div>");

        #region 黄蓝点
        /// <summary>
        /// 这个可以拆出resource bank，然后用下面的Regex去匹配G1然后数匹配数即可
        /// </summary>
        //<div class="tta_element"><table class="tableau0".*?>([\s\S]*?)</table>
        //
        public static readonly Regex ExtractBlueMarker = new Regex(@"<div class=""tta_element""><table class=""tableau0"".*?>([\s\S]*?)</table>");
        //blue.gif
        public static readonly Regex ExtractBlueMarkerCounter = new Regex(@"blue.gif");
        /// <summary>
        /// 这个可以拆出worker bank，然后用下面的Regex去匹配G1然后数匹配数即可
        /// </summary>
        //<div class="tta_element"><table class="tableau2"[\s\S]*?<table class="tableau0".*?>([\s\S]*?)</table>
        public static readonly Regex ExtractYellowMarker = new Regex(@"<div class=""tta_element""><table class=""tableau2""[\s\S]*?<table class=""tableau0"".*?>([\s\S]*?)</table>");
        //blue.gif
        public static readonly Regex ExtractYellowMarkerCounter = new Regex(@"yellow.gif");
        #endregion

        #region 红白点
        /// <summary>
        /// 这个可以数出红白点外加当前政体.
        /// 1是政体，2是白点，3是红点，用/ca /missing /ma来数
        /// </summary>
        //<a .*?class="nomCarte tta_government2"><strong>(.*?)<[\s\S]*?br.*?>([\s\S]*?)br([\s\S]*?)/a
        public static readonly Regex ExtractGovenrmentAndActionPoints=new Regex(@"<a .*?class=""nomCarte tta_government2""><strong>(.*?)<[\s\S]*?br.*?>([\s\S]*?)br([\s\S]*?)/a");
        public static readonly Regex ExtractGovenrmentAndActionPointsCama = new Regex(@"/[c|m]a");
        #endregion

        /// <summary>
        /// 奇迹单元格，用下面的解析group1得到具体奇迹
        /// </summary>
        //<td class=[^>]*?wondersBox[\s\S]*?<table([\s\S]*?)</table></td>
        public static readonly Regex ExtractWonder=new Regex(@"<td class=[^>]*?wondersBox[\s\S]*?<table([\s\S]*?)</table></td>");

        /// <summary>
        /// 这个Reg分两种情况
        /// 1、Group3不为空，这表示是建造好的奇迹，3就是奇迹的名字
        /// 2、Group3为空，表示未完成奇迹，5是奇迹的名字，6是奇迹的建造情况
        /// </summary>
        //<a onClick="javascript:void.0.;"(([^>]*?><img class="imageMerveille" .*?alt="(.*?)")|([\s\S]*?p>([^>]*?)</p><p>([\s\S]*?)<p))
        public static readonly Regex ExtractWondeName=new Regex(@"<a onClick=""javascript:void.0.;""(([^>]*?><img class=""imageMerveille"" .*?alt=""(.*?)"")|([\s\S]*?p>([^>]*?)</p><p>([\s\S]*?)<p))");
        /// <summary>
        /// 这个用于拆出奇迹建造情况，Count是总步数，group1非数字就是造了，数字就是没造
        /// </summary>
        //nbsp;((<img [^>]*?>)|\d)(&|<)
        public static readonly Regex ExtractWondeBuildStatus=new Regex(@"nbsp;((<img [^>]*?>)|\d)(&|<)");
        /// <summary>
        /// 这个用于拆出ColonyBox （Group1）
        /// </summary>
        //td class=.colonyBox dataBox.[^<]*?<table([\s\S]*?)</table>
        public static readonly Regex ExtractColonyBox = new Regex(@"td class=.colonyBox dataBox.[^<]*?<table([\s\S]*?)</table>");

        
        /// <summary>
        /// 特殊科技单元格，用下面的解析group1得到具体科技
        /// </summary>
        //<td class=[^>]*?specTechsBox[\s\S]*?<table([\s\S]*?)</table></td>
        public static readonly Regex ExtractSpecialTech=new Regex(@"<td class=[^>]*?specTechsBox[\s\S]*?<table([\s\S]*?)</table></td>");

        /// <summary>
        /// Group1就是名字（不包含时代）
        /// </summary>
        //<p class=[^>]*?special1[^>]*?>(.*?)<
        public static readonly Regex ExtractSpecialTechName = new Regex(@"<p class=[^>]*?special1[^>]*?>(.*?)<");
        
        //手牌
        //<div id=.cartes_joueur\d.>[\s\S]*?<table class=.tableau\d. width=.90%.>([\s\S]*?)</table>
        public static readonly Regex ExtractHandCivilCard=new Regex(@"<div id=.cartes_joueur\d.>[\s\S]*?<table class=.tableau\d. width=.90%.>([\s\S]*?)</table>");
        //<a onClick=[^>]*?>([^-]*?)&nbsp;-&nbsp;([^-]*?)</a>
        public static readonly Regex ExtractHandCardName=new Regex(@"<a onClick=[^>]*?>([^-]*?)&nbsp;-&nbsp;([^-]*?)</a>");

        //<div id=.cartes_joueur\d.>[\s\S]*?<table class=.tableau\d. width=.90%.>[\s\S]*?<table class=.tableau\d. width=.90%.>([\s\S]*?)</table>
        public static readonly Regex ExtractHandMilitaryCard = new Regex(@"<div id=.cartes_joueur\d.>[\s\S]*?<table class=.tableau\d. width=.90%.>[\s\S]*?<table class=.tableau\d. width=.90%.>([\s\S]*?)</table>");
        //已打出的当前事件
        //已打出的未来事件
        public static readonly  Regex ExtractPlayedEvents=new Regex(@"Events&nbsp;played([\s\S]*?)</table>");
        
        //tdMidC[^>]*?>((<[\s\S]*?ageDosCarte.>([^<]*?)</p>[\s\S]*?)|(&nbsp[\s\S]*?))</td>
        public static readonly Regex ExtractPlayedEventsItem=new Regex(@"tdMidC[^>]*?>((<[\s\S]*?ageDosCarte.>([^<]*?)</p>[\s\S]*?)|(&nbsp[\s\S]*?))</td>");

        public static readonly Regex ExtractPlayedEventsVisible = new Regex(@"Current&nbsp;events&nbsp;played([\s\S]*?)Future events played([\s\S]*?)</table");


        /// <summary>
        /// 状态栏
        /// </summary>
        public static readonly Regex ExtractWarning = new Regex(@"div id=.statusBar.[^>]*?>([\s\S]*?)</div");
        
        public static readonly Regex ExtractWarningItem = new Regex(@"title=""([^""]*?)""");

        //class="tdMidC"><center><table class="tableau4"[^/]*?tactic0[\s\S]*?p class=.ageCarte ageCarte25.>([^<]*?)</[\s\S]*?nomCarte.>([^<]*?)<
        public static readonly Regex ExtractTactics = new Regex(@"class=.tdMidC.><center><table class=.tableau4.[^/]*?tactic0[\s\S]*?p class=.ageCarte ageCarte25.>([^<]*?)</[\s\S]*?nomCarte.>([^<]*?)<");
        //共享阵型框
        public static readonly Regex ExtractSharedTactics = new Regex(@"tta_tactic0 dataBox.>([\s\S]*?)<form");
        //p class=.ageCarte ageCarte25.>([^<]*?)</p>[^>]*?>[^>]*?nomCarte.>([^<]*?)<
        public static readonly Regex ExtractSharedTacticsItem = new Regex(@"p class=.ageCarte ageCarte25.>([^<]*?)</p>[^>]*?>[^>]*?nomCarte.>([^<]*?)<");

        //<option value=.(.*?).>(.*?)</option>
        public static readonly Regex ExtractActions = new Regex(@"<option value=.(.*?).>(.*?)</option>");

        //form id=.formAction. .*? action=.(.*?).>([\s\S]*?)</form>
        public static readonly Regex ExtractSubmitForm=new Regex(@"form id=.formAction. .*? action=.(.*?).>([\s\S]*?)</form>");
        //(<select.*?name="(.*?)")|(<input type="hidden"[^>]*?name="(.*?)"[^>]*?value="(.*?)")
        public static readonly Regex ExtractSubmitFormDetail = new Regex(@"(<select.*?name=""(.*?)"")|(<input type=""hidden""[^>]*?name=""(.*?)""[^>]*?value=""(.*?)"")");

        public static Regex ExtractSubDropDown(string dropdown)
        {
            return new Regex(@"<select.*?name="""+dropdown+@""".*?>([\s\S]*?)</select>");
        }

        public static readonly Regex ExtractGamePhase= new Regex(@"<td class=.titre3.>(.*?) Phase");

        public static readonly Regex ExtractActionChoice =new Regex(@"<input type=.radio. value=""(.*?)"" [\s\S]*?<label .*?>(.*?)<");
    }
}
