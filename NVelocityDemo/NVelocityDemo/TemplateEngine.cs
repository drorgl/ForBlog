using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NVelocity.App;
using System.Diagnostics;

namespace NVelocityDemo
{
    public class TemplateEngine
    {
        /// <summary>
        /// Local dictionary holding the templates for the engine, without it, every 
        /// template will recompile on every execution making everything very slow.
        /// </summary>
        private static Dictionary<string, NVelocity.Template> m_templates = new Dictionary<string, NVelocity.Template>();


        /// <summary>
        /// Initialize the engine with required properties.
        /// </summary>
        /// <returns>Engine instance</returns>
        private static VelocityEngine GetEngine()
        {
            NVelocity.App.VelocityEngine engine = new NVelocity.App.VelocityEngine();

            //set log class type
            engine.SetProperty(NVelocity.Runtime.RuntimeConstants.RUNTIME_LOG_LOGSYSTEM_CLASS, typeof(TemplateEngineLog));

            //switch to local context, this way each macro/recursive execution uses its own variables.
            engine.SetProperty(NVelocity.Runtime.RuntimeConstants.VM_CONTEXT_LOCALSCOPE, "true");

            //allows #set to accept null values in the right hand side.
            engine.SetProperty(NVelocity.Runtime.RuntimeConstants.SET_NULL_ALLOWED, "true");

            //set template resource loader to strings
            engine.SetProperty("resource.loader", "string");
            engine.SetProperty("string.resource.loader.class", "NVelocity.Runtime.Resource.Loader.StringResourceLoader");
            //engine.SetProperty("string.resource.loader.repository.class", "NVelocity.Runtime.Resource.Util.StringResourceRepositoryImpl");

            //initialize engine.
            engine.Init();

            return engine;
        }

        /// <summary>
        /// Process a template in the dictionary's context
        /// </summary>
        /// <param name="context">Dictionary holding the key/values that can be accessed through the template, similar to ViewData</param>
        /// <param name="template">string template</param>
        /// <returns>rendered template + context</returns>
        public static string Process(IDictionary<string, object> context, string template)
        {
            //checks if template contains scripting elements.
            if ((template.IndexOf('$') == -1) && (template.IndexOf('#') == -1))
                return template;
            
            //gets a new engine instance
            var engine = GetEngine();

            //create a log object for this execution
            TemplateEngineLog log = new TemplateEngineLog();
            engine.SetProperty(NVelocity.Runtime.RuntimeConstants.RUNTIME_LOG_LOGSYSTEM, log);


            NVelocity.Template nvtemplate;
            if (!m_templates.TryGetValue(template, out nvtemplate))
            {
                //gets the string resource repository
                var repo = NVelocity.Runtime.Resource.Loader.StringResourceLoader.GetRepository();
                Guid guidcode = Guid.NewGuid();

                //puts a new template inside it
                repo.PutStringResource(guidcode.ToString(), template);

                nvtemplate = engine.GetTemplate(guidcode.ToString(), "UTF-8");

                //save in dictionary
                m_templates[template] = nvtemplate;
            }

            //create a context
            NVelocity.VelocityContext vcontext = new NVelocity.VelocityContext();

            //put default values
            vcontext.Put("null", null);
            vcontext.Put("true", true);

            //add the context values
            foreach (var key in context.Keys)
                vcontext.Put(key, context[key]);

            System.IO.StringWriter sw = new System.IO.StringWriter();

            //attempt to render the template
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                //merge uses a saved template
                nvtemplate.Merge(vcontext, sw);

                ////evaluate uses an unsaved template, recompiles on every execution, very slow.
                //if (engine.Evaluate(vcontext, sw, "", template) == false)
                //{
                //    throw new TemplateEngineException(log.GetContents());
                //}
                stopwatch.Stop();
                Debug.WriteLine("Engine took {0}ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return sw.ToString();
        }


    }
}
