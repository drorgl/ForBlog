/*
 Idea from http://stackoverflow.com/questions/2885487/problem-passing-elmah-log-id-to-custom-error-page-in-asp-net
 
 Changelog:
 2012-06-01 - Dror Gluska - Made into an easy to use class.
 
 Example usage:
 1. in global.asax:
         protected void ErrorLog_Logged(object sender, ErrorLoggedEventArgs args)
        {
            var result = ElmahErrorHandling.ProcessError(sender, args);
            if (!string.IsNullOrEmpty(result))
                Response.Redirect(result);
        }
 
 *** if you want the errors to still be handled by elmah, you have to comment the following line in void RegisterGlobalFilters(GlobalFilterCollection filters)
    filters.Add(new HandleErrorAttribute());
 
 2. in web.config, defaultRedirect should be an error page accepting id as the elmah error id:
    <customErrors mode="On" defaultRedirect="~/Demo/ShowError" />
 3. in error page:
    @MvcElmahErrorDemo.AppHelpers.ElmahErrorHandling.GetError(ViewBag.id)
 
 
 GetError will either diplay a link (if executed locally) or an error id the admin/developers can later check against the elmah error log.
 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Elmah;
using System.Configuration;
using System.Web.Mvc;

namespace MvcElmahErrorDemo.AppHelpers
{
    /// <summary>
    /// Elmah Error handling helper class
    /// </summary>
    public class ElmahErrorHandling
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public static Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

        /// <summary>
        /// CustomErrors configuration section
        /// </summary>
        public static CustomErrorsSection customErrorsSection = (CustomErrorsSection)config.GetSection("system.web/customErrors");

        /// <summary>
        /// Handlers configuration section
        /// </summary>
        public static HttpHandlersSection handlers = (HttpHandlersSection)config.GetSection("system.web/httpHandlers");

        /// <summary>
        /// Process errors caught in Global.asax
        /// <para>should be inside protected void ErrorLog_Logged(object sender, ErrorLoggedEventArgs args)</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns>null if no custom error should be displayed or a link if redirect is required to the specific location</returns>
        public static string ProcessError(object sender, ErrorLoggedEventArgs args)
        {
            if (customErrorsSection != null)
            {
                switch (customErrorsSection.Mode)
                {
                    case CustomErrorsMode.Off:
                        break;
                    case CustomErrorsMode.On:
                        return customErrorsSection.DefaultRedirect + "?id=" + args.Entry.Id;
                    case CustomErrorsMode.RemoteOnly:
                        if (!HttpContext.Current.Request.IsLocal)
                            return customErrorsSection.DefaultRedirect + "?id=" + args.Entry.Id;
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Get path to Elmah error handler
        /// <para>e.g. elmah.axd</para>
        /// </summary>
        /// <returns></returns>
        public static string GetElmahHandler()
        {
            if ((handlers != null) && (handlers.Handlers != null))
                for (int i = 0; i < handlers.Handlers.Count; i++)
                    if (handlers.Handlers[i].Type.IndexOf("elmah", StringComparison.InvariantCultureIgnoreCase) != -1)
                        return handlers.Handlers[i].Path;
                
            return null;
        }

        /// <summary>
        /// Gets absolute path to elmah error log
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetHandlerLink(string id)
        {
            return VirtualPathUtility.ToAbsolute(string.Format("~/{0}/detail?id={1}",GetElmahHandler(), id));
        }

        /// <summary>
        /// Gets Error string according to configuration and id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetErrorString(string id)
        {
            if (customErrorsSection != null)
            {
                switch (customErrorsSection.Mode)
                {
                    case CustomErrorsMode.Off:
                        return string.Format("<a href='{0}'>{1}</a>",GetHandlerLink(id),id);
                    case CustomErrorsMode.On:
                    case CustomErrorsMode.RemoteOnly:
                        if (HttpContext.Current.Request.IsLocal)
                            return string.Format("<a href='{0}'>{1}</a>", GetHandlerLink(id),id);
                        else
                            return id;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// returns MvcHtmlString for error link/text
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MvcHtmlString GetError(string id)
        {
            return new MvcHtmlString(GetErrorString(id));
        }
    }

}