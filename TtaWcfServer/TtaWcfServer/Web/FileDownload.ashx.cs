using System;
using System.IO;
using System.Web;
using HSYErpBase.NHibernate;
using NHibernate;
using TtaPesistanceLayer.NHibernate;
using TtaWcfServer.ServerApi.FileTransfer;

namespace TtaWcfServer.Web
{
    /// <summary>
    /// FileDownload 的摘要说明
    /// </summary>
    public class FileDownload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var guid = context.Request.QueryString["guid"];
            if (String.IsNullOrEmpty(guid))
            {
                context.Response.StatusCode = 400;
                context.Response.End();
                return;
            }
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
            {
                //第一步查找是否有指定的文件
                var token = FileTransferApi.GetToken(hibernateSession, guid);
                if (token == null)
                {
                    context.Response.StatusCode = 404;
                    context.Response.End();
                    return;
                }

                string fileFullPath = Path.Combine(token.Folder, token.Guid + token.Type); //服务器文件路径
                if (!File.Exists(fileFullPath)) //判断文件是否存在
                {
                    context.Response.StatusCode = 404;
                    context.Response.End();
                    return;
                }
                try
                {
                    var mimeMapping = MimeMapping.GetMimeMapping(token.OriginalName);
                    context.Response.ContentType = mimeMapping??"application/octet-stream";
                    context.Response.AddHeader("Content-Disposition", $"attachment; filename=\"{token.OriginalName}\"");
                    context.Response.WriteFile(fileFullPath);     
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }
                catch (Exception)
                {
                    context.Response.StatusCode = 500;
                    context.Response.End();
                }
            }
        }

        public bool IsReusable => false;
    }
}