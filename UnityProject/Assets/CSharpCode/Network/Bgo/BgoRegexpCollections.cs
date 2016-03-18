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
        /// 这个可以匹配卡牌列. Group 2 是 post url, 3 是 idNote ，4是 age， 5 是card name
        /// </summary>
        //(<form method="post" id="piocherCarte\d*?" action="([\S]*?)">)?<input type="hidden" name="idNote" value="([\S]*?)"[\s\S]{0,300}?<a class="[\s\S]*?" onClick="[\s\S]*?">[\s\S]*?<p class="ageCarte ageCarte1x">(\S*?)</p>[\s\S]*?<p class="nomCarte">([\s\S]*?)(<br />)?</p>
        public static readonly Regex ExtractCardRow = new Regex(@"(<form method=""post"" id=""piocherCarte\d*?"" action=""([\S]*?)"">)?<input type=""hidden"" name=""idNote"" value=""([\S]*?)""[\s\S]{0,300}?<a class=""[\s\S]*?"" onClick=""[\s\S]*?"">[\s\S]*?<p class=""ageCarte ageCarte1x"">(\S*?)</p>[\s\S]*?<p class=""nomCarte"">([\s\S]*?)(<br />)?</p>");

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
        //happy.png
        //unhappy.png
        public static readonly Regex ExtractPlayerNameAndResourceHappy = new Regex(@"/happy.png");
        public static readonly Regex ExtractPlayerNameAndResourceUnhappy = new Regex(@"/unhappy.png");
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
    }
}
