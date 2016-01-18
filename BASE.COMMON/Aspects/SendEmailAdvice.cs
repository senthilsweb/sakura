#region Header
/*
 ************************************************************************************
 Name: SendEmailAdvice
 Description:SendEmailAdvice
 Created On:  03-08-2012
 Created By:  Princeton
 Last Modified On:27-Jan-2015
 Last Modified By:Chaitra
 Last Modified Reason:Included Base.Utils in the "Simplicity" project
 ************************************************************************************
 */
#endregion Header

using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Net.Mail;
using System.Web;
using Spring.Aop;
using log4net;

using System.IO;

namespace Base.Utils.Aspects
{
    public class SendEmailAdvice : IAfterReturningAdvice
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SendEmailAdvice));

        /// <summary>
        /// /// 
        /// </summary>
        /// <param name="returnValue"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <param name="target"></param>
        public void AfterReturning(Object returnValue, MethodInfo method, Object[] args, Object target)
        {
            if (null != returnValue)
            {
                try
                {
                    dynamic obj =returnValue;
                    dynamic mailObject = obj.MailObject;
                    MailMessage message = new MailMessage();
                    ////string body = CommonUtils.GetEmailContents(mailObject.Template, mailObject.Content);
                    message = new MailMessage((string)mailObject.From, (string)mailObject.To,
                                                        (string)mailObject.Subject, (string)mailObject.Body);
                    if(mailObject.CC!=null){
                      var mailCC =(mailObject.CC).ToString().Split(',');
                            foreach (var cc in mailCC)
                            {
                                message.CC.Add(new MailAddress(cc));
                            }
                        }
                    var ma = new MailAddress((string)mailObject.From, (string)mailObject.DisplayName);
                    message.From = ma;
                   
                    var body = ((string)mailObject.Body).Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
                    //// send mail
                    Logger.Info(string.Format("{0}***{1}***{2}***{3}", (string)mailObject.From, (string)mailObject.To + " "+(string)mailObject.CC, (string)mailObject.Subject, body));
                    if (CommonUtils.StrToBoolean(AppSettings.Instance.EnableSMTPCall))
                    {
                        Mailer.SendMailMessageAsync(message);
                    }
                    //Logger.Info("Reference # :" + mailObject.ReferenceNo);
                }
                catch (Exception e)
                {
                    Logger.Error("Exception in Mail Sent:\n" + e.Message);
                }
            }
            else
            {
                Logger.Error("Mail Message is not created Yet");
            }
        }

        

    }
}
