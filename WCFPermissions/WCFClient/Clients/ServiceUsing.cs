using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security;
using System.Reflection;
using System.ServiceModel.Description;
using System.Security.Cryptography.X509Certificates;

namespace WCFClient.Clients
{
    /// <summary>
    /// Service usage helpers
    /// </summary>
    class ServiceUsing
    {
        /// <summary>
        /// Use Service SVC and perform appropriate Close, Abort and Dispose
        /// </summary>
        /// <typeparam name="SVC">Service Client Object</typeparam>
        /// <param name="action">actions</param>
        public static void Use<SVC>(Action<SVC> action) where SVC: ICommunicationObject
        {
            SVC service = Activator.CreateInstance<SVC>();

            try
            {
                action(service);
                service.Close();
            }
            catch (Exception e)
            {
                service.Abort();
                throw e;
            }
            finally
            {
                ((IDisposable)service).Dispose();
            }
        }

        /// <summary>
        /// Use Service SVC with Certificate credentials and perform appropriate Close, Abort and Dispose
        /// </summary>
        /// <typeparam name="SVC">Service Client Object</typeparam>
        /// <param name="subjectName">Certificate subject name</param>
        /// <param name="storeLocation">Certificate Store Location</param>
        /// <param name="storeName">Certificate Store Name</param>
        /// <param name="action">actions</param>
        public static void Use<SVC>(string subjectName, StoreLocation storeLocation, StoreName storeName, Action<SVC> action) where SVC : ICommunicationObject
        {
            SVC service = Activator.CreateInstance<SVC>();

            //check if we need to apply a certificate to the service
            if (!string.IsNullOrEmpty(subjectName))
            {
                PropertyInfo pi = typeof(SVC).GetProperty("ClientCredentials");
                ClientCredentials cc = (ClientCredentials)pi.GetValue(service, null);
                cc.ClientCertificate.SetCertificate(subjectName,storeLocation,storeName);
            }


            try
            {
                action(service);
                service.Close();
            }
            catch (Exception e)
            {
                service.Abort();
                throw e;
            }
            finally
            {
                ((IDisposable)service).Dispose();
            }
        }


        /// <summary>
        /// Use Service SVC with username/password credentials and perform appropriate Close, Abort and Dispose
        /// </summary>
        /// <typeparam name="SVC">Service Client Object</typeparam>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="action">actions</param>
        public static void Use<SVC>(string username, string password, Action<SVC> action) where SVC : ICommunicationObject
        {
            SVC service = Activator.CreateInstance<SVC>();

            //check if we need to apply username/password to the service
            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            {
                PropertyInfo pi = typeof(SVC).GetProperty("ClientCredentials");
                ClientCredentials cc = (ClientCredentials)pi.GetValue(service, null);

                cc.UserName.UserName = username;
                cc.UserName.Password = password;
            }


            try
            {
                action(service);
                service.Close();
            }
            catch (Exception e)
            {
                service.Abort();
                throw e;
            }
            finally
            {
                ((IDisposable)service).Dispose();
            }
        }

    }
}
