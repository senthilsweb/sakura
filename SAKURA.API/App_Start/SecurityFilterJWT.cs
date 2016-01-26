//The MIT License (MIT)

//Copyright (c) 2016 Senthilnathan Karuppaiah

// <author> </author>
// <date> </date>
// <summary> </summary>

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System.Web.Mvc;
using BASE.COMMON;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Net;
using System;
using BASE.COMMON.Logging;
using BASE.COMMON.Factory;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
using BASE.COMMON.SECURITY;
using System.Text.RegularExpressions;

namespace SAKURA.API.WebApi
{
    /// <summary>
    /// interceptor to check the presence of Authorization bearer token in the API request header.
    /// Also it has the following:
    /// 1) Presence of JWT Token in HTTP Header
    /// 2) Validity of the JWT Token
    /// 3) Plus future compatiability for user claims & policies
    /// </summary>
    public class SecurityFilterJWT : System.Web.Http.Filters.ActionFilterAttribute
    {
        /// <summary>
        /// This method is called for every API method before going to API controller
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try {

                //1) return if actionContext is null
                if (null == actionContext) { return; }

                //2) Exit if not enabled
                if (!Utilities.Centroid.api_security.enabled) return;
                
                //3) Check whether the API requested is whitelablled and if yes then exit.
                IList<String> whitelists = ((Utilities.Centroid.api_security.whitelists).RawConfig as JArray).ToObject<List<String>>().ConvertAll(d => d.ToLower());
                string whitelistedApisString = string.Join(", ", whitelists);
                string requestedPath = actionContext.Request.RequestUri.LocalPath.ToLower();

                requestedPath = TrimPathPrefix(requestedPath);
                if (whitelists.Contains(requestedPath))
                {
                    return;
                }
                               
                //4) check whether JWT Authorization (bearer token) token present in the request headers, if not present, set error message without executing controller
                if (!actionContext.Request.Headers.Contains(Utilities.Centroid.api_security.jwt_header_key))
                {
                    //actionContext.Request.Headers.GetValues("x-access-token") will gives x-access-token value if present on request header.
                    AddResponseHeaderDuringError(actionContext, Utilities.Centroid.api_security.un_auth_access_error_message
                        + " :: " + requestedPath
                        + " :: " + whitelistedApisString 
                        , 401);
                    return;
                }
				else
				{
					if (ValidateTokenExpiry(actionContext))
					{
                        AddResponseHeaderDuringError(actionContext, Utilities.Centroid.api_security.un_auth_expiry_error_message
                            + " :: " + requestedPath
                            + " :: " + whitelistedApisString
                            , 401);
                        return;
					}
				}
            }
            catch  (Exception ex) {
                UtilsFactory.Logger.Log(ex.StackTrace,LogType.Error);
            }           
        }

        /// <summary>
        /// Trims anything that comes before '/api/'. If '/api/' is not present, this function returns the input as-is.
        /// For example, "/SAKURA.API_deploy/api/roars/paginate" input => "/api/roars/paginate" output.
        /// For example, "/api/roars/paginate" input => "/api/roars/paginate" output.
        /// </summary>
        /// <param name="requestedPath"></param>
        /// <returns></returns>
        private string TrimPathPrefix(string requestedPath)
        {
            Match requestedPathMatch = Regex.Match(requestedPath, "/api/.*$");
            if (requestedPathMatch.Success)
            {
                requestedPath = requestedPathMatch.Value;
            }
            return requestedPath;
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
            actionContext.Response.Content = new StringContent( Newtonsoft.Json.JsonConvert.SerializeObject(model));
            actionContext.Response.ReasonPhrase =  Newtonsoft.Json.JsonConvert.SerializeObject(model);
            actionContext.Response.StatusCode = HttpStatusCode.Unauthorized;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionContext"></param>
		/// <returns></returns>
		private Boolean ValidateTokenExpiry(HttpActionContext actionContext)
		{
			Boolean result = false;

			string token = actionContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
			var resp = new TokenGenerator(Utilities.Centroid.jwt_secret_key).deserializeTokenAsObject(token) as IDictionary<string, object>;
			
			//check whether the current UTC time is greater than the expiry timestamp?
			result = ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds > BASE.COMMON.Utilities.StrToInt(BASE.COMMON.Utilities.CnvStr(resp["exp"]))); //session expired
			return result;
		}
    }
}