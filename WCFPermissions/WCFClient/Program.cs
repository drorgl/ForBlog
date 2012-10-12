using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCFClient.Clients;
using WCFClient.ServiceDemoSVC;
using WCFClient.SimpleUsernameServiceDemoSVC;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Channels;
using WCFClient.AAServiceDemoSVC;
using WCFClient.CertificateServiceDemoSVC;
using WCFClient.InvokerServiceDemoSVC;

namespace WCFClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting and waiting for service to start..");
            Thread.Sleep(2000);

            Console.WriteLine("Executing ServiceDemoClient...");
            ServiceUsing.Use<ServiceDemoClient>((service) =>
            {
                service.DoWork();
            });

            Console.WriteLine("Executing ServiceDemoClient with a custom header...");
            ServiceUsing.Use<ServiceDemoClient>((service) =>
                {
                    using (OperationContextScope scope = new OperationContextScope((IContextChannel)service.InnerChannel))
                    {
                        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("CustomHeaderValue", "ns", "Value CustomHeaderValue"));

                        service.DoWork();
                    }
                });

            Console.WriteLine("Executing ServiceDemoClient, asking for an exception...");
            try
            {
                ServiceUsing.Use<ServiceDemoClient>((service) =>
                    {
                        service.ThrowException("hello");
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Executing ServiceDemoClient, asking for an custom fault...");
            try
            {
                ServiceUsing.Use<ServiceDemoClient>((service) =>
                {
                    service.ThrowCustomException("fault message");
                });
            }
            catch (FaultException<CustomFault> cf)
            {
                Console.WriteLine("Custom Fault received: {0}", cf.Detail.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine("Executing CertificateServiceDemoClient");
            ServiceUsing.Use<CertificateServiceDemoClient>((service) =>
            {
                service.DoWork();
            });



            Console.WriteLine("Executing AAServiceDemoClient...");
            ServiceUsing.Use<AAServiceDemoClient>("hello", "world", (service) =>
            {
                service.DoWork();
            });



            //should fail.
            Console.WriteLine("Executing SimpleUsernameServiceDemoClient, should fail since we're not passing client credentials");
            try
            {
                ServiceUsing.Use<SimpleUsernameServiceDemoClient>((service) =>
                    {
                        service.DoWork();
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            Console.WriteLine("Executing SimpleUsernameServiceDemoClient - should succeed executing TestRoleAPI since hello has the API role.");
            ServiceUsing.Use<SimpleUsernameServiceDemoClient>("hello", "world", (service) =>
            {
                service.DoWork();
                service.TestRoleAPI();
            });


            Console.WriteLine("Executing SimpleUsernameServiceDemoClient - should fail to execute TestRoleAdmin since hello doesn't have the Admin role.");
            try
            {
                ServiceUsing.Use<SimpleUsernameServiceDemoClient>("hello", "world", (service) =>
                {
                    service.TestRoleAdmin();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine("Executing InvokerServiceDemoClient - should succeed.");
            ServiceUsing.Use<InvokerServiceDemoClient>("hello", "world", (service) =>
            {
                service.DoWork();
            });

            Console.WriteLine("Executing InvokerServiceDemoClient - should fail.");
            try
            {
                ServiceUsing.Use<InvokerServiceDemoClient>("hello", "world", (service) =>
                {
                    service.DoWorkNoPermission();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
        }
    }
}
