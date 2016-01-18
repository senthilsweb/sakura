#region Header
//************************************************************************************
// Name: LoggingAdvice
// Description: LoggingAdvice
// Created On:  02-Aug-2011
// Created By:  Swathi
// Last Modified On:27-Jan-2015
// Last Modified By:Chaitra
// Last Modified Reason:Included Base.Utils in the "Simplicity" project
//*************************************************************************************
#endregion Header
using System;
using System.Text;
using AopAlliance.Intercept;


namespace Base.Utils.Aspects
{
    internal class LoggingAdvice : IMethodInterceptor
    {  
        #region IMethodInterceptor Members

        public object Invoke(IMethodInvocation invocation)
        {

            DateTime sTime = DateTime.Now;
            object rval = invocation.Proceed();
            DateTime eTime = DateTime.Now;
            TimeSpan executionTime = eTime.Subtract(sTime);
            string logMessage = string.Format("[{0}.{1}] execution time={2}", invocation.TargetType.FullName, invocation.Method.Name,executionTime.TotalSeconds.ToString());
            Base.Utils.Factory.UtilsFactory.Logger.Log(logMessage, Base.Utils.Logging.LogType.Info);
            return rval;
        }

        #endregion

        private string GetMethodSignature(IMethodInvocation invocation)
        {
            var sb = new StringBuilder();
            sb.Append(invocation.Method.DeclaringType.Name)
                .Append('.')
                .Append(invocation.Method.Name)
                .Append('(');

            string separator = ", ";
            if (invocation.Arguments != null)
                for (int i = 0;
                     i < invocation.Arguments.Length;
                     i++)
                {
                    if (i == invocation.Arguments.Length - 1)
                    {
                        separator = "";
                    }
                    sb.Append(invocation.Arguments[i])
                        .Append(separator);
                }
            sb.Append(')');

            return sb.ToString();
        }
    }
}