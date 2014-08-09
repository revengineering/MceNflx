using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Net;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Threading;

namespace MceNflx
{
    /// <summary>
    /// Acts as a dirt-simple web server that serves up the SearchTaxonomy.xml for all requests.
    /// </summary>
    public partial class MceSearchTaxonomyService : ServiceBase
    {
        const string RegistryKeyRoot = "HKEY_LOCAL_MACHINE\\";
        const string RegistryKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensions\\InternetTV\\";

        int _port;
        string _appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        HttpListener _listener;
        string _originalRegistryValue;
        Thread _thread;

        public MceSearchTaxonomyService()
        {
            InitializeComponent();
        }
        
        protected override void OnStart(string[] args)
        {
            var settings = new Properties.Settings();

            // Get an available port #.
            _port = settings.PortNumber;

            // Construct the root URL to this service.
            string rootUrl = string.Format("http://localhost:{0}/", _port);

            // Backup anything that happened to be stored in the SearchTaxonomyPath registry entry.
            _originalRegistryValue = (string)Registry.GetValue(RegistryKeyRoot + RegistryKeyPath, "SearchTaxonomyPath", null);

            // Put our root URL into the registry entry.
            Registry.SetValue(RegistryKeyRoot + RegistryKeyPath, "SearchTaxonomyPath", rootUrl);

            string filePath = Path.Combine(_appPath, "SearchTaxonomy.xml");

            // Start the HttpListener, to listen for requests.
            _listener = new HttpListener();
            _listener.Prefixes.Add(rootUrl);

            _thread = new Thread((ThreadStart) delegate() {
                _listener.Start();
                while(true)
                {
                    try
                    {
                        var context = _listener.GetContext();
                        var response = context.Response;
                        try
                        {
                            var bytes = File.ReadAllBytes(filePath);
                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.ContentType = "text/xml";
                            response.ContentLength64 = bytes.Length;
                            response.OutputStream.Write(bytes, 0, bytes.Length);
                            response.OutputStream.Flush();
                        }
                        catch(Exception e)
                        {
                            response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            response.ContentType = "text/plain";
                            using(var sw = new StreamWriter(response.OutputStream))
                            {
                                sw.WriteLine("Error: " + e.Message);
                            }
                        }
                        response.Close();
                    }
                    catch(HttpListenerException) { break; }
                    catch(InvalidOperationException) { break; }
                }
            });
            _thread.Start();
        }

        protected override void OnStop()
        {
            if(_listener != null && _listener.IsListening)
            {
                _listener.Stop();
            }
            _thread.Join();
            
            // Restore previous registry value.
            if(_originalRegistryValue != null)
            {
                Registry.SetValue(RegistryKeyRoot + RegistryKeyPath, "SearchTaxonomyPath", _originalRegistryValue);
            }
            else
            {
                using(var key = Registry.LocalMachine.OpenSubKey(RegistryKeyPath, true))
                {
                    key.DeleteValue("SearchTaxonomyPath");
                    key.Close();
                }
            }
            
        }
    }
}
