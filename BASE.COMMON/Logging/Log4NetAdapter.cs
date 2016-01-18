//
// <>.cs
//
// Authors:
// <Name> (<email>@* * * * * * * * * * .com)
//
// Copyright (C) 2015 * * * * * * * * * * 
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Web;
using System.IO;
using log4net.Core;

namespace BASE.COMMON.Logging
{
	/// <summary>
	/// 
	/// </summary>
    public class Log4NetAdapter : ILogger
	{
		#region "Class level variables declaration
		private readonly ILog _log;
        private readonly Dictionary<string, Level> _logLevels;
		#endregion

		#region "Constructor"
		public Log4NetAdapter()
        {
			//Load log4Net configuration
            var log4NetPath =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory + Utilities.Centroid.Config_Files.Spring.Base_Directory , @"logging\log4net.config"); 
            
            //Configure Log4net
            FileInfo fi = new FileInfo(log4NetPath);
            if (fi != null && fi.Exists)
                XmlConfigurator.Configure(fi);

			_log = LogManager.GetLogger("GlobalLogger");//TODO: Get the value from configuration
            _logLevels = new Dictionary<string, Level>
                             {
                                 {LogType.Debug.ToString(), Level.Debug},
                                 {LogType.Error.ToString(), Level.Error},
                                 {LogType.Info.ToString(), Level.Info},
                                 {LogType.Warn.ToString(), Level.Warn},
                                 {LogType.Fatal.ToString(), Level.Fatal}
                             };

        }
		#endregion

		#region ILogger Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="logType"></param>
		public void Log(string message, LogType logType)
        {
            //Create a LoggingEvent using the information required to show in the log file 
            //and pass it to Log() which actually writes it in the file
            var loggingEvent = new LoggingEvent(
                typeof(Log4NetAdapter),
                _log.Logger.Repository,
                _log.Logger.Name,
                _logLevels[logType.ToString()],
                message,
                null);
            _log.Logger.Log(loggingEvent);
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="logType"></param>
        public void Log(string message, Exception exception, LogType logType)
        {
            //Create a LoggingEvent using the information required to show in the log file 
            //and pass it to Log() which actually writes it in the file
            var loggingEvent = new LoggingEvent(
               typeof(Log4NetAdapter),
               _log.Logger.Repository,
               _log.Logger.Name,
               _logLevels[logType.ToString()],
               exception.Message,
               exception);
            _log.Logger.Log(loggingEvent);
        }

       
		/// <summary>
		/// Log the message using the caller information instead of log4netadapter
		/// </summary>
		/// <param name="caller"></param>
		/// <param name="message"></param>
		/// <param name="logType"></param>
        public void Log(Type caller, string message, LogType logType)
        {
            //Create a LoggingEvent using the information required to show in the log file 
            //and pass it to Log() which actually writes it in the file
            var loggingEvent = new LoggingEvent(
                caller,
                _log.Logger.Repository,
                caller.FullName,
                _logLevels[logType.ToString()],
                message,
                null);
            _log.Logger.Log(loggingEvent);
        }
        /// <summary>
		///Log the exception using the caller information instead of log4netadapter
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="logType"></param>
        public void Log(Type caller, string message, Exception exception, LogType logType)
        {

            //Create a LoggingEvent using the information required to show in the log file 
            //and pass it to Log() which actually writes it in the file
            var loggingEvent = new LoggingEvent(
               caller,
               _log.Logger.Repository,
               caller.FullName,
               _logLevels[logType.ToString()],
               exception.Message,
               exception);
            _log.Logger.Log(loggingEvent);
        }
        #endregion
    }
}
