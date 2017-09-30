using System;
using HSYErpBase.EntityDefinition.UserModel;

namespace HSYErpBase.EntityDefinition.SessionModel
{
    public class Session
    {
        public static Session CurrentSession { get; set; } = new Session();

        public String SessionString { get; set; }

        public User CurrentUser { get; set; }
    }
}