using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AutoMail
{
   public class WebClient1:WebClient
    {
        public string Url { get; set; }

        public string CallService(string RequestData)
        {
            try
            {
                //WebClient client = new WebClient();
                System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
                return base.UploadString(new Uri(Url),RequestData);
            }
            catch (Exception ex)
            {
                return "Error : " + ex.Message;

            }
        }
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 20 * 60 * 1000;
            return w;
        }

    }
}
