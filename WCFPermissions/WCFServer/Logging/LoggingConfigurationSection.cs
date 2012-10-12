using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace WCFServer.Logging
{
    /// <summary>
    /// Logging Extension Configuration Section
    /// </summary>
    public class LoggingConfigurationSection : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(LoggingBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new LoggingBehavior();
        }
    }
}
