using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Assets.CSharpCode.UI;
using UnityEngine;

namespace Assets.CSharpCode.Helper
{
    class ExceptionHandle
    {
        static bool isExceptionHandlingSetup;
        public static void SetupExceptionHandling()
        {
            if (!isExceptionHandlingSetup)
            {
                isExceptionHandlingSetup = true;
                Application.logMessageReceived += HandleException;
                //Application.RegisterLogCallback(HandleException);
            }
        }
        static void HandleException(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                //Switch Scene
                SceneTransporter.LastError = condition+Environment.NewLine+stackTrace;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene/ErrorScene");
               Assets.CSharpCode.UI.Util.LogRecorder.Log("ExceptionReceived");
            }
        }
        internal static void ReportCrash(string message, string stack)
        {
            var errorMessage = new StringBuilder();

            errorMessage.AppendLine("Error Report " + Application.platform);

            errorMessage.AppendLine();
            errorMessage.AppendLine(message);
            errorMessage.AppendLine(stack);

            errorMessage.AppendFormat
            (
                @"{0} {1} {2} {3}
{4}, {5}, {6}x {7}
{8}x{9} {10}dpi FullScreen: {11}, {12}, {13} vmem: {14} Max Texture: {15}
Unity Version {16}",
                SystemInfo.deviceModel,
                SystemInfo.deviceName,
                SystemInfo.deviceType,
                SystemInfo.deviceUniqueIdentifier,

                SystemInfo.operatingSystem,
                SystemInfo.systemMemorySize,
                SystemInfo.processorCount,
                SystemInfo.processorType,

                Screen.currentResolution.width,
                Screen.currentResolution.height,
                Screen.dpi,
                Screen.fullScreen,
                SystemInfo.graphicsDeviceName,
                SystemInfo.graphicsDeviceVendor,
                SystemInfo.graphicsMemorySize,
                SystemInfo.maxTextureSize,
                Application.unityVersion
            );

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("youraddress@gmail.com");
            mail.To.Add("youraddress@gmail.com");
            mail.Subject = "Test Mail";
            mail.Body = "This is for testing SMTP mail from GMAIL";

            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential("youraddress@gmail.com", "yourpassword") as ICredentialsByHost;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback =
                (s, certificate, chain, sslPolicyErrors) => true;
            smtpServer.Send(mail);
           Assets.CSharpCode.UI.Util.LogRecorder.Log("success");
            
        }

    }
}
