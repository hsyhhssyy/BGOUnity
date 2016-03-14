using System.IO;
using System.Net;
using System.Text;
using Assets.CSharpCode.Network;

namespace LibraryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //http://boardgaming-online.com/
            HttpWebRequest wr = (HttpWebRequest) WebRequest.Create("http://www.boardgaming-online.com/");
            wr.Method = "POST";
            wr.ContentType = "application/x-www-form-urlencoded";

            byte[] byteArray = Encoding.UTF8.GetBytes(@"identifiant=hsyhhssyy&mot_de_passe=hsy12345&souvenir=on");

            //http://boardgaming-online.com/index.php?cnt=202&pl=7309370&nat=1

            wr.ContentLength = byteArray.Length;
            Stream newStream = wr   .GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);//写入参数
            newStream.Close();

            var wb=wr.GetResponse();
            var cookie = wb.Headers;
            var phpsessionid = cookie["Set-Cookie"].Split(";".ToCharArray())[0].Split("=".ToCharArray())[1];
            
            wr = (HttpWebRequest)WebRequest.Create("http://boardgaming-online.com/index.php?cnt=202&pl=7309370&nat=1");
            wr.Method = "GET";
            wr.ContentType = "application/x-www-form-urlencoded";

            wr.Headers["Cookie"] = "PHPSESSID="+phpsessionid;
            wb = wr.GetResponse();
            TextReader tr = new StreamReader(wb.GetResponseStream());
            string str=tr.ReadToEnd();

            BoardAnalyzer.AnalyzeBoard(str);
        }
    }
}
