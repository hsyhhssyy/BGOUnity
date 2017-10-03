using System;
using System.IO;
using System.Web;
using HSYErpBase.NHibernate;
using NHibernate;
using TtaCommonLibrary.Entities.FileStorageModel;
using TtaPesistanceLayer.NHibernate;
using TtaWcfServer.ServerApi.FileTransfer;

namespace TtaWcfServer.Web
{
    /// <summary>
    /// FileUpload 的摘要说明
    /// </summary>
    public class FileUpload : IHttpHandler
    {
        string uploadFolder = @"C:\TtaUploads\";

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
            using (ISession hibernateSession = NHibernateHelper.CurrentHelper.OpenSession())
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