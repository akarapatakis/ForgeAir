using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace ForgeAir.Core.Shared
{
    public class SMSRequestsShared
    {

        public string RequestModelRegex { get; set; } = "^Request:\\s*(.*?)\\s*by\\s*(.*?)\\s*(?:-\\s*(.*))?$\r\n";
        public EventHandler onRequestReceived;
        public string phoneNumber { get; set; }
        public string messageUnfiltered { get; set; }
        public void RaiseOnRequestReceived() { onRequestReceived.Invoke(null, null); }
        public SMSRequestsShared() { 
            
        }

        private static SMSRequestsShared? instance;
        public static SMSRequestsShared Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SMSRequestsShared();
                }
                return instance;
            }
        }
    }
}
