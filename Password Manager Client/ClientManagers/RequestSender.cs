using System;

namespace Password_Manager_Client
{
    using System.Net;
    using System.IO;
    using System.Text;

    class RequestSender
    {
        static private string authKey;

        static public void SetAuthorization(string key)
        {
            var token = EncryptionManager.HashData(key);
            authKey = "Bearer " + Convert.ToBase64String(token);
            Console.WriteLine("Your authorization token: " + authKey);
        }

        static public WebRequest CreatePOSTRequest(string URL, byte[] data)
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

        static public WebRequest CreateGETRequest(string URL)
        {
            var req = (HttpWebRequest)WebRequest.Create(URL);
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
