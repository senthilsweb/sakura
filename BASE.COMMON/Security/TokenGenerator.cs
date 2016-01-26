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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace BASE.COMMON.SECURITY
{
	/// <summary>
	/// Generates  auth tokens for the client application.
	/// </summary>
	public class TokenGenerator
    {
		
        private static int TOKEN_VERSION = 0;
        private string _secret;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="Secret">The  Secret for your client application.</param>
        public TokenGenerator(string secret)
        {
            _secret = secret;
        }

        /// <summary>
        /// Creates an authentication token containing arbitrary auth data.
        /// </summary>
        /// <param name="data">Arbitrary data that will be passed to the  Rules API, once a client authenticates.  Must be able to be serialized to JSON with <see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>.</param>
        /// <returns>The auth token.</returns>
        public string CreateToken(Dictionary<string, object> data)
        {
            return CreateToken(data, new TokenOptions());
        }

        /// <summary>
        /// Creates an authentication token containing arbitrary auth data and the specified options.
        /// </summary>
        /// <param name="data">Arbitrary data that will be passed to the  Rules API, once a client authenticates.  Must be able to be serialized to JSON with <see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>.</param>
        /// <param name="options">A set of custom options for the token.</param>
        /// <returns>The auth token.</returns>
        public string CreateToken(Dictionary<string, object> data, TokenOptions options)
        {
            var dataEmpty = (data == null || data.Count == 0);
            if (dataEmpty && (options == null || (!options.admin && !options.debug)))
            {
                throw new Exception("data is empty and no options are set.  This token will have no effect on .");
            }

            var claims = new Dictionary<string, object>();
            claims["v"] = TOKEN_VERSION;
            claims["iat"] = secondsSinceEpoch(DateTime.UtcNow);

            var isAdminToken = (options != null && options.admin);
            
            // TODO: refresh validate token
            //validateToken(data, isAdminToken);

            if (!dataEmpty)
            {
                claims["d"] = data;
            }

            // Handle options.
            if (options != null)
            {
                if (options.expires.HasValue)
                    claims["exp"] = secondsSinceEpoch(options.expires.Value);
                if (options.notBefore.HasValue)
                    claims["nbf"] = secondsSinceEpoch(options.notBefore.Value);
                if (options.admin)
                    claims["admin"] = true;
                if (options.debug)
                    claims["debug"] = true;
            }

            var token = computeToken(claims);
            if (token.Length > 1024)
            {
                throw new Exception("Generated token is too long. The token cannot be longer than 1024 bytes.");
            }

            return token;
        }

        private string computeToken(Dictionary<string, object> claims)
        {
            return JWT.JsonWebToken.Encode(claims, this._secret, JWT.JwtHashAlgorithm.HS256);
        }

		public string deserializeTokenAsJson(string token)
		{
			return JWT.JsonWebToken.Decode(token, this._secret);
		}
		public object deserializeTokenAsObject(string token)
		{
			return JWT.JsonWebToken.DecodeToObject(token, this._secret);
		}

        private static long secondsSinceEpoch(DateTime dt)
        {
            TimeSpan t = dt.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (long)t.TotalSeconds;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="isAdminToken"></param>
        private static void validateToken(Dictionary<string, object> data, Boolean isAdminToken)
        {
            var containsUid = (data != null && data.ContainsKey("uid"));
            if ((!containsUid && !isAdminToken) || (containsUid && !(data["uid"] is string)))
            {
                throw new Exception("Data payload must contain a \"uid\" key that must not be a string.");
            }
            else if (containsUid && data["uid"].ToString().Length > 256)
            {
                throw new Exception("Data payload must contain a \"uid\" key that must not be longer than 256 characters.");
            }			
        }
    }
}
