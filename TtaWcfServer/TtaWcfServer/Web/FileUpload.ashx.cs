using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.Web;
using TtaPesistanceLayer.NHibernate;

namespace RailwayERPWebService.Web
{
    /// <summary>
    /// FileUpload 的摘要说明
    /// </summary>
    public class FileUpload : IHttpHandler
    {
        string uploadFolder = @"C:\UploadFile\";

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Files.Count == 0)
            {
                return;
            }
            var file = context.Request.Files[0];
            var fileInfo = new FileInfo(file.FileName);
            var guid = Guid.NewGuid();
            String savePath = guid.ToString() + fileInfo.Extension;

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string filePath = Path.Combine(uploadFolder, savePath);
            file.SaveAs(filePath);

            UploadedFileToken token = new UploadedFileToken();
            token.Folder = uploadFolder;
            token.Guid = guid.ToString();
            token.OriginalName = fileInfo.Name;
            token.Type = fileInfo.Extension;
            using (ISession hibernateSession = NHibernateHelper.OpenSession())
            {
                FileTransferApi.AddToken(hibernateSession, token);
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(guid.ToString());
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}