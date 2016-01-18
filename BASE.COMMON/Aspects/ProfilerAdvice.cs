#region Header
//************************************************************************************
// Name:ProfilerAdvice
// Description:Profiler advice to check the elapsed time(of the application or method)
// Created On:  28-03-2011
// Created By:  Ranjitha
// Last Modified On:5-May-2011 / 27-Jan-2015
// Last Modified By: Karthiga Baskaran / Chaitra A U
// Last Modified Reason: Added method Name and Class Name to be displayed / Included Base.Utils in the "Simplicity"
//*************************************************************************************
#endregion Header
using System;
using Spring.Aop;
using System.Reflection;
using System.Text;
using AopAlliance.Intercept;
using log4net;


namespace Base.Utils.Aspects
{
    public class  ProfilerAdvice : IMethodInterceptor
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(ProfilerAdvice));

        #region IMethodInterceptor Members
        //Function to measure the execution time of a task. 
        public object Invoke(IMethodInvocation invocation)
        {
           // Get the start time i.e before invoking the method
            DateTime startTime = DateTime.Now;
            var className = invocation.Proxy.ToString();
            var methodName = invocation.Method.Name;
            var parameters = string.Empty;  //CommonUtils.JsonSerializer<object>(invocation.Arguments[0]); 
            object rval = null;
            //log4netadapter will also be invoked as it is injected using spring. This class doesnt require profiling.
            //So this class is eliminated and can be viewed in instrumentation log
            if (className.ToLower().Contains("log4netadapter"))// && invocation.Arguments.Length==3)
            {
                rval = invocation.Proceed();
                //className =  ((Type)(invocation.Arguments[0])).FullName;
                //methodName = string.Empty;
            }
            else //Start Profiling
            {
                Log.Info(String.Format("{0}***{1}***Start***{2}***{3}", className, methodName, startTime.ToShortDateString(), startTime.TimeOfDay));
                // Execute the task to be timed
                rval = invocation.Proceed();
                //Once invoking is done,keep the note of End Time
                DateTime stopTime = DateTime.Now;
                Log.Info(String.Format("{0}***{1}***End***{2}***{3}", className, methodName, stopTime.ToShortDateString(), stopTime.TimeOfDay));
                //Calculate the elapsed time from the start and stop time
                TimeSpan elapsedTime = stopTime - startTime;
                Log.Info(String.Format("{0}***{1}***For UI***{2}***{3}***{4}***{5}", className, methodName, parameters,startTime.TimeOfDay, stopTime.TimeOfDay, elapsedTime));
            }
           return rval;
        }

        #endregion
    }
}
