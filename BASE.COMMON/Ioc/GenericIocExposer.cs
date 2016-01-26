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
using Spring.Context;
using System;
using System.Collections.Generic;
using System.IO;

namespace BASE.COMMON
{
	/// <summary>
	/// A generic call to load dependencies from Spring Configuration XML files. The approach used here is "Convention Over Configuration".
	/// It is implemented based on Service Locator Design Pattern
	/// </summary>
	/// <typeparam name="T">Instance of the requested class by name or Interface (Name of the class derived from Interface name)</typeparam>
	public sealed class GenericIocExposer<T> where T : class
	{
		#region "Class level private variables declaration"

		private T _service;
		private static IApplicationContext _context;

		#endregion "Class level private variables declaration"

		#region "Parameterless constructor"

		//TODO: Need to change this implementation
		public GenericIocExposer()
		{
			if(_context!=null) return;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory + Utilities.Centroid.Config_Files.Spring.Base_Directory + "ioc\\";
			string configs = Utilities.Centroid.Config_Files.Spring.Configs_For_All;
			string[] param = new string[configs.Split(',').Length];
			int i = 0;
			foreach (string file in configs.Split(','))
			{
				param.SetValue(baseDir + file.Trim(), i);
				i++;
			}
			_context = new Spring.Context.Support.XmlApplicationContext(param);
		}

		#endregion "Parameterless constructor"

		#region "Create instance using spring context"

		//private readonly string _objectId;
		private Type interfaceObject = typeof(T);

		// property to get instance of an object whose name is equivalent to Interface prefix with "I"
		public T GetInstance
		{
			get
			{
				//Remove the letter I from the interface passed in the Exposer
				var implObject = (interfaceObject.Name).Remove(0, 1);
				if (_service != null)
					return _service;
				_service = (T)_context.GetObject(implObject);
				if (_service == null)
					throw new TypeLoadException("Can not load facade/Dao Factory from container!");
				return _service;
			}
		}

		// property to get instance of an object whose name is not equivalent to Interface prefix with "I"
		public T GetInstanceByName(string objectName)
		{
			_service = (T)_context.GetObject(objectName);
			if (_service == null)
				throw new TypeLoadException("Can not load object requested '" + objectName + "' Factory from container!");
			return _service;
		}

		// property to get instance of an object whose name is not equivalent to Interface prefix with "I" and with constructor
		public T GetInstanceByNameAndWithConstructor(string objectName, string constructor)
		{
			_service = (T)_context.GetObject(objectName, new object[] { constructor });

			return _service;
		}

		// property to get instance of an object whose name is equivalent to Interface prefix with "I" and with constructor
		public T GetInstanceWithConstructor(string constructor)
		{
			//Remove the letter I from the interface passed in the Exposer
			var implObject = (interfaceObject.Name).Remove(0, 1);
			_service = (T)_context.GetObject(implObject, new object[] { constructor });

			return _service;
		}

		#endregion "Create instance using spring context"

		#region "Private utility to load all config files"

		/// <summary>
		/// Load all spring config files in comma separated string
		/// </summary>
		/// <remarks>
		/// TODO: This code needs to be optimized.
		/// </remarks>
		/// <returns>Comma separated string</returns>
		private string GetConfigFiles()
		{
			//TODO : Get the file path from configuration file
			List<string> files = new List<string>();
			string baseDir = AppDomain.CurrentDomain.BaseDirectory + @"Configs\";
			files.Add("\"" + Path.Combine(baseDir, "ioc.facade.xml") + "\"");
			files.Add("\"" + Path.Combine(baseDir, "ioc.data.xml") + "\"");
			files.Add("\"" + Path.Combine(baseDir, "ioc.framework.xml") + "\"");
			return string.Join(",", files);
		}

		#endregion "Private utility to load all config files"
	}
}