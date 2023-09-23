using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using HtmlAgilityPack;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string Host_name = Dns.GetHostName();
            string Host_ip = Dns.GetHostByName(Host_name).AddressList[0].ToString();

            // UTF8Encoding utf8 = new UTF8Encoding();
            // WebClient webClient = new WebClient();
            // string externalIp = utf8.GetString(webClient.DownloadData("http://checkip.dyndns.org/"));
            // string ipAddress = externalIp.Substring(externalIp.IndexOf("Current IP Address: "));

            WebClient webClient = new WebClient();
            string ipAddress = webClient.DownloadString("https://ipinfo.io/ip");

            /*string marker = "Current IP Address: ";
            int startIndex = externalIp.IndexOf(marker) + marker.Length;
            int endIndex = externalIp.IndexOf("</body>", startIndex);
            string ipAddress = externalIp.Substring(startIndex, endIndex - startIndex);*/

            WriteToFile("Service is start at " + DateTime.Now + " " + Host_ip + " My public IP Address is " + ipAddress);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 60000; //number in milisecinds 
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            string Host_name = Dns.GetHostName();
            string Host_ip = Dns.GetHostByName(Host_name).AddressList[0].ToString();
            WriteToFile("Service is stop at " + DateTime.Now + " " +  Host_ip);

        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WebClient webClient = new WebClient();
            string ipAddress = webClient.DownloadString("https://api.ipify.org");

            string Host_name = Dns.GetHostName();
            string Host_ip = Dns.GetHostByName(Host_name).AddressList[0].ToString();
            WriteToFile("Service is recall at " + DateTime.Now + "  " +  Host_ip + "  " + ipAddress);
        }


            public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}

