using System;

namespace Password_Manager_Client
{
    using System.Net;
    using System.IO;
    using System.Text;

    class RequestSender
    {
        static private string authKey;
        private static readonly string url = "http://localhost:8000";

        static public void SetAuthorization(string token)
        {
            authKey = token;
        }

        static public void SendData(byte[] parsedData, string path)
        {
            var req = RequestSender.CreatePOSTRequest(
                url + path, parsedData);
            var respondStr = GetRespond(req);
            Console.WriteLine("Server respond: " + respondStr);
        }

        static private WebRequest CreatePOSTRequest(string URL, byte[] data)
        {
            var req = (HttpWebRequest)WebRequest.Create(URL);

            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = data.Length;
            req.Headers.Add("Authorization", authKey);
            Stream stream= req.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();
            return req;
        }

        static public WebRequest CreateGETRequest(string path)
        {
            var req = (HttpWebRequest)WebRequest.Create(url + path);
            req.Method = "GET";
            req.Headers.Add("Authorization", authKey);
            return req;
        }

        static public string GetRespond(WebRequest req)
        {
            var response = (HttpWebResponse)req.GetResponse();
            var responseString = new StreamReader(
                response.GetResponseStream()).ReadToEnd();
            return responseString;
        }
    }
}
