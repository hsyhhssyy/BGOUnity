using System.Linq;
using NHibernate;
using TtaCommonLibrary.Entities.FileStorageModel;

namespace TtaWcfServer.ServerApi.FileTransfer
{
    public class FileTransferApi
    {
        public static void ListFile()
        {

        }

        public static UploadedFileToken GetToken(ISession session, string guid)
        {
            var query =
                session.CreateQuery("from UploadedFileToken where guid=? and end_date is null");
            query.SetString(0, guid);

            var token = query.List<UploadedFileToken>().FirstOrDefault();

            return token;

        }

        public static void AddToken(ISession hibernateSession, UploadedFileToken token)
        {
            hibernateSession.Save(token);
        }
    }
}