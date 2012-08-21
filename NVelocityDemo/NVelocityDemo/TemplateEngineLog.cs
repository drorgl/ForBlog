using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NVelocityDemo
{
     /// <summary>
    /// Template Engine Log
    /// </summary>
    public class TemplateEngineLog : NVelocity.Runtime.Log.ILogSystem
    {
        #region LogSystem Members

        private StringBuilder sbmessages = new StringBuilder();
       
        public string GetContents()
        {
            return sbmessages.ToString();
        }

        public void LogVelocityMessage(int level, string message)
        {
            sbmessages.Append(message);
        }

        public void Init(NVelocity.Runtime.IRuntimeServices rs)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
