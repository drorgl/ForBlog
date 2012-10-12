using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using WCFServer.Services;
using System.ServiceModel.Description;

namespace WCFServer
{
    class Program
    {
        /// <summary>
        /// Main for starting all the ServiceHosts in the project
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            List<ServiceHost> hosts = new List<ServiceHost>();
            hosts.Add(new ServiceHost(typeof(ServiceDemo)));
            hosts.Add(new ServiceHost(typeof(SimpleUsernameServiceDemo)));
            hosts.Add(new ServiceHost(typeof(AAServiceDemo)));
            hosts.Add(new ServiceHost(typeof(CertificateServiceDemo)));
            hosts.Add(new ServiceHost(typeof(InvokerServiceDemo)));


            Helper.Log("Services:");

            foreach (var host in hosts)
            {
                host.Open();
                Helper.Log("At {0}", string.Join(", ", host.BaseAddresses.Select(i => i.AbsoluteUri).ToArray()));
            }

            Console.WriteLine("Press <Enter> to stop the service.");
            Console.ReadLine();

            foreach (var host in hosts)
            {
                // Close the ServiceHosts.
                host.Close();
            }

        }
    }
}
