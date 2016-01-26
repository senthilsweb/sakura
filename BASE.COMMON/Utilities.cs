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
using BASE.COMMON.Factory;
using BASE.COMMON.Logging;
using Centroid;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RazorEngine;
using RazorEngine.Templating;
using RestSharp;
using Spring.Core.IO;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BASE.COMMON
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Utilities
	{
        #region "Inject Encript Wrapper through Ioc"

        private static IEncriptWrapper _encriptWrapper;
        private static IEncriptWrapper EncriptWrapper { get { return _encriptWrapper ?? (_encriptWrapper = new GenericIocExposer<IEncriptWrapper>().GetInstance); } }

        #endregion "Inject Encript Wrapper through Ioc"

		//Centroid is a tool for loading configuration values declared in JSON, and accessing those configuration values using object properties.
		//http://www.nuget.org/packages/Centroid/
		public static dynamic Centroid
		{
			get
			{
				//TODO : Get the file path from configuration / settings (App.config / web.config) or follow some convetion to dynamically resolve the path & name.
                return Config.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\config\dev.json"));
			}
		}

		public static dynamic ErrorMessages
		{
			get
			{
				//TODO : Get the file path from configuration / settings (App.config / web.config) or follow some convetion to dynamically resolve the path & name.
                return Config.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\message\en.json"));
			}
		}

		
		

		#region "Encryption and Decryption"

		/// <summary>
		/// Decrypt a crypted string.
		/// </summary>
		/// <param name="cryptedString">The crypted string.</param>
		/// <returns>The decrypted string.</returns>
		/// <exception cref="ArgumentNullException">This exception will be thrown
		/// when the crypted string is null or empty.</exception>
		public static string Decrypt(string cryptedString)
		{
            return EncriptWrapper.Decrypt(cryptedString);
        }

		/// <summary>
		/// Encrypt a string.
		/// </summary>
		/// <param name="originalString">The original string.</param>
		/// <returns>The encrypted string.</returns>
		/// <exception cref="ArgumentNullException">This exception will be
		/// thrown when the original string is null or empty.</exception>
		public static string Encrypt(string originalString)
		{
            return EncriptWrapper.Encrypt(originalString); 
        }

		#endregion "Encryption and Decryption"

		#region "Serialization & De-serialization"

		/// <summary>
		/// Method to De-serialize the binary data to Object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <returns></returns>
		public static T DeserializeObject<T>(byte[] data) where T : class
		{
			//Initializing the memory stream
			MemoryStream ms = new MemoryStream();
			//Writing the Byte array data into Memory stream
			ms.Write(data, 0, data.Length);
			ms.Position = 0;
			//Initializing the Binary formatter
			BinaryFormatter bf = new BinaryFormatter();
			//Deserializing the memory stream byte array data into an object and storing into a variable
			T deserializedObj = (T)bf.Deserialize(ms);
			return deserializedObj;
		}

		/// <summary>
		/// Method to Serialize the Object to binary data
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static byte[] SerializeObject<T>(T obj) where T : class
		{
			//Initializing the memory stream
			MemoryStream ms = new MemoryStream();
			//Initializing the Binary formatter
			BinaryFormatter bf = new BinaryFormatter();
			//Seralizing the object and placing the data into memory stream
			bf.Serialize(ms, obj);
			ms.Position = 0;
			//declaring byte array variable to read data from memory stream
			byte[] serializedObj = new byte[ms.Length];
			//reading the data in memory stream and converting it into byte Array
			ms.Read(serializedObj, 0, (int)ms.Length);
			ms.Close();
			return serializedObj;
		}

		#endregion "Serialization & De-serialization"

		#region "JSON Serialization & De-serialization using NEWTONSOFT"

		/// <summary>
		/// Method called to serialize a dynamic object
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static dynamic JsonSerialize(dynamic request)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(request);
		}

		/// <summary>
		/// Method called to serialize a dynamic object with datetime in a format
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static dynamic JsonSerializeWithDateFormatter(dynamic request)
		{
			return JsonConvert.SerializeObject(request, new IsoDateTimeConverter());
		}

		/// <summary>
		/// Method called to De-serialize a dynamic object
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static dynamic JsonDeSerialize(dynamic request)
		{
			return JsonConvert.DeserializeObject(request);
		}

		/// <summary>
		/// Method called to De-serialize a dynamic object
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static dynamic stringToJObject(string request)
		{
			return JObject.Parse(request);
		}

		#endregion "JSON Serialization & De-serialization using NEWTONSOFT"

		#region "Datatype conversions from string"

		public static Decimal StrToDecimal(string value)
		{
			return StrToDecimal(value, 0);
		}

		public static Decimal StrToDecimal(string value, Decimal defaultData)
		{
			Decimal result;
			if (!Decimal.TryParse(value, out result))
			{
				result = defaultData;
			}
			return result;
		}

		public static Double StrToDouble(string value)
		{
			return StrToDouble(value, 0);
		}

		public static Double StrToDouble(string value, Double defaultData)
		{
			Double result;
			if (!Double.TryParse(value, out result))
			{
				result = defaultData;
			}
			return result;
		}
		public static Boolean StrToBoolean(string value)
		{
			return StrToBoolean(value, false);
		}

		public static Boolean StrToBoolean(string value, Boolean defaultData)
		{
			Boolean result;
			if (!Boolean.TryParse(value, out result))
			{
				
				int intdata;
				if (Int32.TryParse(value, out intdata))
				{
					
					result = (intdata != 0);
				}
				else
				{
					result = defaultData;
				}
			}
			return result;
		}
		public static int StrToInt(string value)
		{
			return StrToInt(value, 0);
		}

		public static int StrToInt(string value, int defaultData)
		{
			int result;
			if (!Int32.TryParse(value, out result))
			{
				result = defaultData;
			}
			return result;
		}

		public static long StrToLong(string value)
		{
			try
			{
				long result;
				if (!long.TryParse(value, out result))
				{
					result = 0;
				}
				return result;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public static short StrToShort(string value)
		{
			try
			{
				short result;
				if (!short.TryParse(value, out result))
				{
					result = 0;
				}
				return result;
			}
			catch (Exception)
			{
				return 0;
			}
		}

        public static string CnvStr(object target)
        {
            if (target == null)
            {
                return string.Empty;
            }
            if (target == DBNull.Value)
            {
                return string.Empty;
            }
            try
            {
                string ret = target.ToString();
                return ConvProperString(ret);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
		/// Replaces single quote with two single quotes and also preserves Leading and Trailing spaces [indentations]
		/// </summary>
		/// <param name="input">string</param>
		/// <returns>string</returns>
		public static string ConvProperString(string input)
		{
			string wk = string.Empty;
			wk = Regex.Replace(input, @"^(?<1>\r\n)*(?<3>.*?)\s*$", @"$3", RegexOptions.Multiline);
			return wk;
		}

		public static string GetAsDBString(string input)
		{
			string result;
			result = string.IsNullOrEmpty(input) ? "" : input.Replace("'", "''");
			return "'" + result + "'";
		}

		#endregion "Datatype conversions from string"

		#region "Random number & string generation"

		public static String GetRandom(int num)
		{
			if (num < 1) throw new ArgumentOutOfRangeException("num", "Exception");
			var randoms = new byte[num + 1];

			var rng = new RNGCryptoServiceProvider();

			rng.GetBytes(randoms);

			var inputNumber = new StringBuilder();
			for (int i = 0; i < num; i++)
			{
				inputNumber.Append((randoms[i] % 10).ToString());
			}
			return inputNumber.ToString();
		}

		public static string GetRandomAlphaNumeric(int padLength)
		{
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var random = new Random();
			var result = new string(Enumerable.Repeat(chars, padLength)
						.Select(s => s[random.Next(s.Length)])
						.ToArray());
			return result;
		}

		/// <summary>
		/// Random unique strings like the ones being generated by MSDN library
		/// </summary>
		/// <remarks>http://stackoverflow.com/questions/730268/unique-random-string-generation/730418#730418</remarks>
		/// <returns></returns>
		public static string UniqueRandomString()
		{
			Guid g = Guid.NewGuid();
			string GuidString = Convert.ToBase64String(g.ToByteArray());
			GuidString = GuidString.Replace("=", "");
			GuidString = GuidString.Replace("+", "");
			return GuidString;
		}

		#endregion "Random number & string generation"

        #region Making HTTP calls

        /// <summary>
        /// Makes HTTP GET call to apiUrl
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns>response string</returns>
        public static string HttpGet(string apiUrl) {
            string responseFromServer = string.Empty;
            try {
                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(apiUrl);

                // Set the Method property of the request to GET.
                request.Method = "GET";

                // Get the response.
                WebResponse response = request.GetResponse();

                // Log the HTTP response status.
                UtilsFactory.Logger.Log("In Utilities.HttpGet: " + ((HttpWebResponse)response).StatusDescription, LogType.Debug);

                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);

                // Read the content.
                responseFromServer = reader.ReadToEnd();

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();

            }
            catch (Exception ex) {
                UtilsFactory.Logger.Log("In Utilities.HttpGet: " + ex.Message, LogType.Error);
                throw;
            }
            // return the content.
            return responseFromServer;
        }

        #endregion Making HTTP calls

        /// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static dynamic AttachCommonFields(dynamic input, string action)
		{
            //Map the input dynamic object to dictionary
            var dict = (IDictionary<string, object>)input;
            //Remove the MethodType property which is not required in the data Saving
            if (dict.ContainsKey("MethodType")) dict.Remove("MethodType"); //<Senthil, 07-Jul-2015> Included "ContainsKey" check.

            int userId = 1;
            try
            {
                if (dict.ContainsKey("UserID"))
                {
                    userId = input.UserID;
                    dict.Remove("UserID"); //<Senthil, 07-Jul-2015> Included "ContainsKey" check.
                }

                //Get UserId from HttpHeader.
                var header = HttpContext.Current.Request.Headers["x-user"];
                if (header != null && !string.Equals("null", (string)header))
                {
                    userId = (int)Utilities.StrToLong((string)Utilities.JsonDeSerialize(header).Id);
                    userId = (userId == 0) ? (int)Utilities.StrToLong(Utilities.Centroid.Defaults.AnonUser) : userId;
                }

            }
            catch (Exception ex)
            {
                UtilsFactory.Logger.Log(ex.Message, LogType.Error);
            }

            //Update common fields.
            switch (action.ToLower())
			{
				case "post":
					if (dict.ContainsKey("Id"))	dict.Remove("Id"); //<Senthil, 07-Jul-2015> Included "ContainsKey" check.
					dict.Add("Code", GetRandomAlphaNumeric(25)); //Auto generate code
					dict.Add("CreatedBy", userId);
					dict.Add("CreatedAt", DateTime.UtcNow);
					break;
				default:
					dict.Add("ModifiedBy", userId);
					dict.Add("ModifiedAt", DateTime.UtcNow);
					break;
			}

            return dict;
		}

		/// <summary>
		/// This generic method is used to check if property exist in dynamic object
		/// </summary>
		/// <param name="dynamicObject">any dynamic object to compare</param>
		/// <param name="propertyToCheck">name of the property To Check if exists</param>
		/// <returns>Boolean</returns>
		/// <remarks></remarks>
		public static Boolean CheckIfPropertyExistInDynamicObject(dynamic dynamicObject, string propertyToCheck)
		{
			try
			{
				if (dynamicObject.GetType().FullName.Equals("Newtonsoft.Json.Linq.JObject"))
				{
					return ((IDictionary<string, Newtonsoft.Json.Linq.JToken>)dynamicObject).ContainsKey(propertyToCheck);
				}
				else
				{
					return ((IDictionary<string, Object>)dynamicObject).ContainsKey(propertyToCheck);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.Write(ex);
				return false;
			}
			
		}

		/// <summary>
		/// </summary>
		/// <param name="input"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static bool RemoveProperties(dynamic input, string propertyName)
		{
			var dict = (IDictionary<string, object>)input;
			dict.Remove(propertyName);
			return dict.Remove(propertyName);
		}

		public static string ReadTextFile(string path)
		{
			IResource res = new FileSystemResource(path);
			if (!res.File.Exists) return string.Empty;
			var reader = new StreamReader(res.InputStream);
			return reader.ReadToEnd();
		}

        /// <summary>
        /// Check if setting exist in dynamic object
        /// </summary>
        public static bool IsSettingsExist(dynamic settings, string name)
        {
            return ((IDictionary<string, object>)settings).ContainsKey(name);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static dynamic BindDataInTemplate(string template, string key, Object data)
		{
			return Engine.Razor.RunCompile(template, key , null, data);
		}

        /// <summary>
        /// Convert string to Title Case.
        /// </summary>
        /// <param name="input">Not in Title Case string</param>
        /// <returns>Title case string</returns>
        public static string ToTitleCase(string input)
        {
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            return myTI.ToTitleCase(input);
        }

		/// <summary>
		/// </summary>
		/// <param name="key"></param>		
		/// <returns>Http header string value for a given header key name</returns>
		public static string HttpHeader(string key)
		{
			return Utilities.CnvStr(HttpContext.Current.Request.Headers[key]);
		}

        /// <summary>
        /// Send REST request.
        /// </summary>
        /// <param name="key"></param>		
        /// <returns>Response</returns>
        public static dynamic SendPost(string serviceUrl, dynamic data)
        {
            dynamic response = new ExpandoObject();

            try
            {
                var client = new RestClient(serviceUrl);
                var request = new RestRequest(Method.POST);

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("accept", "application/json");
                request.AddHeader("content-type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddBody(data);
                response.Data = client.Execute<dynamic>(request).Data;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Success = false;
                response.Message = ex.Message;
            }
            
            return response;
        }
    }
}