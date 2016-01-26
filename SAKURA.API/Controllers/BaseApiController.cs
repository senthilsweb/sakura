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
using BASE.COMMON;
using BASE.COMMON.Factory;
using BASE.COMMON.Logging;
using System;
using System.Web;
using System.Web.Http;

namespace SAKURA.API
{
	public abstract class BaseApiController : ApiController
	{
		public static readonly ILogger LogManager = UtilsFactory.Logger;
		public BaseApiController() {
			//LogManager.Log("BaseApiController", LogType.Info);
		}

        /// <summary>
        /// Returns User Id from Http Header if available else it returns -1
        /// </summary>
        public long UserId
        {
            get
            {
                long userId = Utilities.Centroid.Defaults.AnonUser;
                try
                {
                    var header = HttpContext.Current.Request.Headers["x-user"];
                    
                    if (header != null && !string.Equals("null", (string)header))
                    {
                        userId = Utilities.StrToLong((string)Utilities.JsonDeSerialize(header).Id);
                        userId = (userId == -1 || userId == 0) ? Utilities.Centroid.Defaults.AnonUser : userId;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Log(ex.StackTrace, LogType.Error);
                }                
                return userId;
            }
        }
	}
}