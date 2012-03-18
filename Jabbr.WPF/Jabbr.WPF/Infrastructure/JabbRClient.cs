using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using SignalR.Client.Transports;

namespace Jabbr.WPF
{
    public class JabbRClient : JabbR.Client.JabbRClient
    {
        private string _url = null;

        public JabbRClient(string url)
            : base(url, new LongPollingTransport())
        {
            _url = url;
        }

        public string Authenticate(string token)
        {
            CookieContainer cookieContainer = new CookieContainer();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url + "/Auth/Login.ashx");
            request.CookieContainer = cookieContainer;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            string data = "token=" + token;
            byte[] postBytes = Encoding.Default.GetBytes(data);

            request.ContentLength = postBytes.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(postBytes, 0, postBytes.Length);
            dataStream.Close();
            var response = request.GetResponse();
            response.Close();

            CookieCollection cookies = cookieContainer.GetCookies(new Uri(_url));
            string cookieValue = cookies[0].Value;

            JObject jsonObject = JObject.Parse(cookieValue);

            return (string)jsonObject["userId"];
        }
    }
}
