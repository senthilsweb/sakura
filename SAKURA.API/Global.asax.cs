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
using SAKURA.API.WebApi;
using System;
using System.Web.Http.Filters;
using System.Web.Http;
using Spring.Context;
using Spring.Context.Support;
using Spring.Web.Mvc;
using System.Collections.Generic;

namespace SAKURA.API
{
    /// <summary>
    /// Application Start class where basic plumbing are setup
    ///
    /// </summary>
    /// <remarks>"SpringMvcApplication" is to implement method execution time in unobtrusive way using Sping AOP otherwise
    /// "System.Web.HttpApplication" implementaiton is sufficient.
    /// </remarks>
    /// <reference>https://github.com/serra/stackoverflow/tree/master/spring-questions/q9114762_aop_on_mvc_controllers</reference>
    public class Global : SpringMvcApplication
    {
        /// <summary>
        /// Application Start function where basic plumbing are setup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            //Register for web api**********************************
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //Register for web api**********************************

            //Register for http filters**********************************            
            FilterConfig.RegisterGlobalFilters(GlobalConfiguration.Configuration.Filters);
            //Register for http filters**********************************

            //[Notes about this implementation]
            // Register Spring Context files. [Note: This is required to compute web controller's method execution time in unobtrusive way using Sping AOP]
            // The implementation is not straight forward for MVC Controlleres hence following the approcah given in the below stackoverflow answer and implementation

            string baseDir = AppDomain.CurrentDomain.BaseDirectory + Utilities.Centroid.Config_Files.Spring.Base_Directory + "aspect\\";
            string configs = Utilities.Centroid.Config_Files.Spring.Configs_For_ApiController;
            string[] param = new string[configs.Split(',').Length];
            int i = 0;
            foreach (string file in configs.Split(','))
            {
                param.SetValue(baseDir + file.Trim(), i);
                i++;
            }
           
                IApplicationContext context = new Spring.Context.Support.XmlApplicationContext(param);
                ContextRegistry.RegisterContext(context);
           
            
        }

        /// <summary>
        /// Dummy implementation
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private IApplicationContext XmlApplicationContext(string p1, string p2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Required for
        /// </summary>
        /// <returns></returns>
        protected override System.Web.Http.Dependencies.IDependencyResolver BuildWebApiDependencyResolver()
        {
            //TODO: Write details about the significance of this resolver
            var resolver = base.BuildWebApiDependencyResolver();
            var springResolver = resolver as SpringWebApiDependencyResolver;
            return resolver;
        }
    }
}