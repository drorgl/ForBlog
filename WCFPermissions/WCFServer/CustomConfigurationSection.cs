using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using WCFServer.Logging;
using System.Configuration;

namespace WCFServer
{

    /*
     Add section in configuration:
     <system.serviceModel>
      <extensions>
        <behaviorExtensions>
          <add name="customInspector" type="WCFServer.CustomConfigurationSection, WCFServer" />
        </behaviorExtensions>
      </extensions>
     
     Add in behavior configuration, where BehaviorName is the error handler or inspector type:
     <customInspector BehaviorName="WCFServer.Logging.LoggingBehavior,WCFServer" />
     
     */


    /// <summary>
    /// A "Generic" behavior configuration section that can add service behaviors based on configuration value BehaviorName
    /// </summary>
    public class CustomConfigurationSection : BehaviorExtensionElement
    {
        
        public const string ConfigBehaviorName = "BehaviorName";

        [ConfigurationProperty(ConfigBehaviorName)]
        public string BehaviorName
        {
            get
            {
                return (string)base[ConfigBehaviorName];
            }
            set
            {
                base[ConfigBehaviorName] = value;
            }
        }

        public override Type BehaviorType
        {
            get { return typeof(CustomConfigurationSectionBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new CustomConfigurationSectionBehavior((IServiceBehavior)Activator.CreateInstance(Type.GetType(this.BehaviorName)));
        }

    }
}
