//
// AccountController.cs
//
// Authors:
// 
//
// Copyright (C) 2015 
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#region "Using"

using BASE.COMMON;
using BASE.COMMON.Attributes;
using BASE.COMMON.Logging;
using BASE.COMMON.SECURITY;
using SAKURA.CORE;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web.Http;
using System.Web.Http.Cors;

#endregion "Using"

namespace SAKURA.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountController : BaseApiController
	{
		public AccountController()
			: base()
		{
			
		}

		#region "Inject User Facade through Ioc"

        //private static IUserFacade _userFacade;

        ////property holds the instance of UserDao instantiated through Spring IoC
        //private IUserFacade UserFacade { get { return _userFacade ?? (_userFacade = new GenericIocExposer<IUserFacade>().GetInstance); } }

		#endregion "Inject User Facade through Ioc"

		#region "Inject Serviceagents through Ioc"

        //private INotificationServiceAgent mailer;

        //private INotificationServiceAgent Mailer { get { return mailer ?? (mailer = new GenericIocExposer<INotificationServiceAgent>().GetInstanceByName("Mailer")); } }

		#endregion "Inject Serviceagents through Ioc"

		

		/// <summary>
		///
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		
		[HttpPost]
		[Route("api/account/Authenticate")]
		public virtual dynamic Authenticate(dynamic request)
		{
			dynamic response = new ExpandoObject();
			try
			{
               
				LogManager.Log(BASE.COMMON.Utilities.CnvStr(request.username) + " " + BASE.COMMON.Utilities.CnvStr("Sam"), BASE.COMMON.Logging.LogType.Info);
                LogManager.Log(BASE.COMMON.Utilities.CnvStr(request.username) + " " + BASE.COMMON.Utilities.CnvStr("Sam"), BASE.COMMON.Logging.LogType.Debug);
                LogManager.Log("Tec", LogType.Error);
				//LogManager.Log(request.password, BASE.COMMON.Logging.LogType.Info);
				//1)Authenticate user
                dynamic result = new ExpandoObject(); //UserFacade.Authenticate(request);
				//2)Authentication Successful
				if (result.Success)
				{
					//Issue JWT Auth Token
					var res = IssueAuthToken(result);
					response.UserViewModel = res.User;
					response.token = res.Token;
				}				
				response.Success = result.Success;
				response.Message = result.Message;
			}
			catch (Exception ex)
			{
				response.Success = false;
				response.Message = ex.Message;
			}
			return response;
		}

        //[HttpPost]
        //[Route("api/token/reissue")]
        //private dynamic Issue(string token)
        //{
        //    //ContainsKey("uid")
        //    var extractedToken = new TokenGenerator(Utilities.Centroid.jwt_secret_key).deserializeTokenAsObject(token) as IDictionary<string, object>;
        //    var user = extractedToken["d"] as IDictionary<string, object>;
        //    dynamic res = new ExpandoObject();
        //    //3)Create Payload for JWT. Payload should have "uid"
        //    var payload = new Dictionary<string, object>
        //        {
        //            { "uid", user["uid"] },
        //            { "FirstName", user["FirstName"]},
        //            { "Email", user["Email"]},
        //            { "Code", user["Code"]},
        //            { "CompanyCode", user["CompanyCode"]}
        //        };
        //    //4)Generate Claims for JWT
        //    var tokenOptions = new TokenOptions(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(Utilities.Centroid.api_security.token_timeout_in_seconds), false, true); 
        //    var tokenGenerator = new TokenGenerator(Utilities.Centroid.jwt_secret_key);
        //    var reissuedToken = tokenGenerator.CreateToken(payload, tokenOptions);
        //    LogManager.Log(reissuedToken, BASE.COMMON.Logging.LogType.Info);
        //    return reissuedToken;
        //}

        //[HttpPost]
        //[Route("api/account/signup")]
        //public dynamic Signup(ExpandoObject request)
        //{
        //    dynamic response = new ExpandoObject();
        //    try
        //    {
        //        dynamic req = request;
        //        req.request.MethodType = Request.Method.ToString();
        //        req.request.UserTypeCode = Utilities.Centroid.Defaults.User_Type_Code;
        //        req.request.SubscriptionTypeCode = Utilities.Centroid.Defaults.Subscription_Type_Code;
        //        req.request.CompanyCode = Utilities.Centroid.Defaults.Company_Code;
        //        if (!Utilities.CheckIfPropertyExistInDynamicObject(req.request, "SocialNetType"))
        //        {
        //            req.request.SocialNetType = Utilities.Centroid.Defaults.Source_Of_The_Registration;
        //        }

        //        var login = UserFacade.LoginNameCheck(req.request); //TODO: We might need to add company code check
        //        //If this user is not registered already then let let him to register.
        //        if (login.Success == true) //<Senthil, 07-JUL-2015> Added this condition to provide meaningful error message otherwise it was throwing DB unique constraint error.
        //        {
        //            //Save user into db
        //            var result = UserFacade.Save(req.request);
        //            if (result.Success)
        //            {
        //                var res = IssueAuthToken(result);
        //                //5) Build response with token
        //                response.Success = result.Success;
        //                response.Message = result.Message;
        //                response.UserViewModel = res.User;
        //                response.token = res.Token;
        //            }
        //            else
        //            {
        //                response.Success = false;
        //                response.Message = result.Message;
        //                response.FieldId = result.FieldId;
        //            }
        //        }
        //        else
        //        {
        //            response.Success = login.Success;
        //            response.Message = login.Message;
        //            response.FieldId = login.FieldId;
        //        }

        //        //
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Message = ex.Message;
        //    }
        //    //6) send response
        //    return response;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private dynamic IssueAuthToken(dynamic result)
        {
            //ContainsKey("uid")
            dynamic res = new ExpandoObject();
            UserViewModel user = new UserViewModel { Id = BASE.COMMON.Utilities.CnvStr(((IDictionary<string, Object>)result.User).ContainsKey("ID") ? result.User.ID : result.User.Id), FirstName = result.User.FirstName, Email = result.User.Email, Code = result.User.Code, CompanyCode = result.User.CompanyCode };
            //3)Create Payload for JWT. Payload should have "uid"
            var payload = new Dictionary<string, object>
                {
                    { "uid", user.Id },
                    { "FirstName", user.FirstName},
                    { "Email", user.Email},
                    { "Code", user.Code},
                    { "CompanyCode", user.CompanyCode}
                };
            //4)Generate Claims for JWT
            var tokenOptions = new TokenOptions(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(Utilities.Centroid.api_security.token_timeout_in_seconds), false, true);
            var tokenGenerator = new TokenGenerator(Utilities.Centroid.jwt_secret_key);
            var token = tokenGenerator.CreateToken(payload, tokenOptions);
            LogManager.Log(token, BASE.COMMON.Logging.LogType.Info);
            res.Token = token;
            res.User = result.User;
            return res;
        }

        ///// <summary>
        ///// request.FromName
        ///// request.FromEmail
        ///// request.ToName
        ///// request.ToEmail
        ///// request.Bcc
        ///// request.Cc
        ///// request.Subject
        ///// request.Body 
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("api/email/send")]
        //public dynamic Send(dynamic request)
        //{
        //    dynamic response = new ExpandoObject();
					
        //    try
        //    {
        //        if (!Utilities.CheckIfPropertyExistInDynamicObject(request, "FromName")) { throw new ArgumentException("FromName"); }
        //        if (!Utilities.CheckIfPropertyExistInDynamicObject(request, "FromEmail")) { throw new ArgumentException("FromEmail"); }
        //        if (!Utilities.CheckIfPropertyExistInDynamicObject(request, "ToName")) { throw new ArgumentException("ToName"); }
        //        if (!Utilities.CheckIfPropertyExistInDynamicObject(request, "ToEmail")) { throw new ArgumentException("ToEmail"); }				
        //        if (!Utilities.CheckIfPropertyExistInDynamicObject(request, "Subject")) { throw new ArgumentException("Subject"); }
        //        if (!Utilities.CheckIfPropertyExistInDynamicObject(request, "Body")) { throw new ArgumentException("Body"); }
        //        response = Mailer.Send(request);				
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        response.Message = String.Format("{0} is missing in the request", ex.Message);
        //        response.Success = false;
        //        LogManager.Log(response.Message, LogType.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.StackTrace;
        //        response.Success = false;
        //        LogManager.Log("Exception Message: " + ex.Message + "& Stack Trace:" + ex.StackTrace + " & Request Object : " + Utilities.JsonSerialize(request), LogType.Error);
        //    }
        //    return response;
        //}
	}
}