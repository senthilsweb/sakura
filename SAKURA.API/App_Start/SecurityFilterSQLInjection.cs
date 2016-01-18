//
// SecurityFilterSQLInjection.cs
// Description : This filters any suspicious API requests which has SQL injection codes
// Authors:
// * * * * * * * * * 
//
// * * * * * * * * * 
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
using System.Web.Mvc;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Net;
using BASE.COMMON;
using System;
using BASE.COMMON.Factory;
using BASE.COMMON.Logging;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SAKURA.API.WebApi
{
    /// <summary>
    /// This interceptor is used to check whether delete keyword appears on query string of every RestApi call
    /// </summary>
    public class SecurityFilterSQLInjection : System.Web.Http.Filters.ActionFilterAttribute
    {
        /// <summary>
        /// This method is called for every API method before going Controller
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
			try
			 {

				 //1) return if actionContext is null
				 if (null == actionContext) { return; }

				 //2) Exit if not enabled
				 if (!Utilities.Centroid.api_security.enabled) return;

				 //3) Check whether the SQL INJECTION KEYWORD EXISTS.
				
				 IList<String> keywords = ((Utilities.Centroid.api_security.injection_keywords).RawConfig as JArray).ToObject<List<String>>().ConvertAll(d => d.ToLower());
				 
				 for (var i = 0; i < keywords.Count; ++i)
				 {
					 if (actionContext.Request.RequestUri.ParseQueryString().ToString().ToLower().IndexOf(keywords[i]) !=-1)
					 {
						 AddResponseHeaderDuringError(actionContext, Utilities.Centroid.api_security.un_auth_sql_injection_error_message, 401);
						 return;
					 }
				 }
			 }
			 catch (Exception ex)
			 {
				 UtilsFactory.Logger.Log(ex.StackTrace, LogType.Error);
			 }
        }


		/// <summary>
		/// This method sets the Status code and error message to response
		/// </summary>
		/// <param name="actionContext"></param>
		/// <param name="Error"></param>
		/// <param name="statuscode"></param>
		private void AddResponseHeaderDuringError(HttpActionContext actionContext, string Error, int statuscode)
		{
			//Return stringified JSON without execution of Controller.
			//Set the Response with Status Code Unauthorized
			actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
			//set model response Sucess as false and error message
			var model = new { Success = false, Message = Error };
			//set X-error as Error Message
			actionContext.Response.Headers.Add("x-error", Error);
			//Assign the model response to actioncontext and display response content as Stringified JSON
			actionContext.Response = actionContext.Request.CreateResponse();
			actionContext.Response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(model));
			actionContext.Response.ReasonPhrase = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			actionContext.Response.StatusCode = HttpStatusCode.Unauthorized;
		}
    }
}