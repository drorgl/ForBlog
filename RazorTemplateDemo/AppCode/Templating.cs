/*
 Templating - Templating helper for rendering Razor templates
 
 Dror Gluska 2010
 
 */

using System.Web.Mvc;
using RazorTemplateDemo.Controllers;
using System.Web;
using System.IO;
using System;
using System.Text;
namespace RazorTemplateDemo.AppCode
{
    /// <summary>
    /// Templating Engine abstractions
    /// </summary>
    public class Templating
    {
        /// <summary>
        /// Concatenate a template with base content
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private static string GetViewContentFromTemplate(string template, Type model)
        {

            var baseContent =
@"@inherits RazorTemplateDemo.AppCode.RazorBaseWebViewPage<{0}>
@{
    Layout = null;
}{1}";
            return baseContent.Replace("{0}", model.FullName).Replace("{1}", template);
        }

        /// <summary>
        /// Generates a controller and context, hands them off to be rendered by the view engine and 
        /// returns the result string
        /// </summary>
        /// <param name="viewName">Template Name</param>
        /// <param name="model">Model for the view</param>
        /// <param name="viewData">ViewData</param>
        /// <returns>rendered string</returns>
        private static string Execute(string viewName, ViewDataDictionary viewData, object model)
        {
            var controller = new TemplatesController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new HttpContextWrapper(HttpContext.Current);
            controller.RouteData.DataTokens.Add("controller", "Templates");
            controller.RouteData.Values.Add("controller", "Templates");
            controller.ViewData = viewData;
            return RenderView(controller, viewName, model);
        }

        /// <summary>
        /// Retrieves View Directory
        /// </summary>
        private static string ViewDirectory
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/Views/Templates");
            }
        }

        /// <summary>
        /// Retrieves the view name by template and model
        /// </summary>
        private static string GetViewName(string template,Type modelType)
        {
            //gets the razor template from a text template
            var razortemplate = GetViewContentFromTemplate(template, modelType);

            //gets the hash string from the razor template
            string hashstring = BitConverter.ToString(BitConverter.GetBytes(razortemplate.GetHashCode()));

            //check if view exists in folder
            var files = Directory.GetFiles(ViewDirectory, hashstring + "*.cshtml");
            foreach (var file in files)
            {
                if (File.ReadAllText(file, Encoding.UTF8) == razortemplate)
                    return Path.GetFileNameWithoutExtension(file);
            }

            //if not, add it
            string filename = Path.Combine(ViewDirectory, hashstring + "_" + Guid.NewGuid().ToString() + ".cshtml");
            File.WriteAllText(filename, razortemplate,Encoding.UTF8);

            return Path.GetFileNameWithoutExtension(filename);
        }

        /// <summary>
        /// Renders a template with parameters to string
        /// </summary>
        /// <param name="template">template text to render</param>
        /// <param name="model">the model to give the template</param>
        /// <param name="parameters">the ViewData for the execution</param>
        /// <returns>rendered template</returns>
        public static string Render(string template, object model, ViewDataDictionary parameters)
        {
            //if empty
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            //if doesn't contain razor code
            if (template.IndexOf("@") == -1)
                return template;

            //get View filename
            string fileName = GetViewName(template, (model != null) ? model.GetType() : typeof(object));

            //Execute template
            return Execute(fileName, parameters, model);
        }

        /// <summary>
        /// Renders a PartialView to String
        /// </summary>
        private static string RenderView(Controller controller, string viewName, object model)
        {
            //origin http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/
            if (string.IsNullOrEmpty(viewName))
            {
                return string.Empty;
            }

            controller.ViewData.Model = model;
            try
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    IView viewResult = GetPartialView(controller, viewName);
                    ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult, controller.ViewData, controller.TempData, sw);
                    viewResult.Render(viewContext, sw);
                }
                return sb.ToString();

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Retrieves Partial View
        /// </summary>  
        private static IView GetPartialView(Controller controller, string viewName)
        {
            return ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName).View;
        }

    }
}
