using System;

namespace Password_Manager_Client
{
    using System.Net;
    using System.IO;
    using System.Text;

    class RequestSender
    {
        static public WebRequest CreatePOSTRequest(string URL, byte[] data)
        {
            var req = (HttpWebRequest)WebRequest.Create(URL);

            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = data.Length;
            Stream streamToServer = req.GetRequestStream();
            SendData(streamToServer, data);

            return req;
        }

        static public WebRequest CreateGETRequest(string URL)
        {
            var req = (HttpWebRequest)WebRequest.Create(URL);
            req.Method = "GET";

            return req;
        }

        static public string GetRespond(WebRequest req)
        {
            var response = (HttpWebResponse)req.GetResponse();
            var responseString = new StreamReader(
                response.GetResponseStream()).ReadToEnd();
            return responseString;
        }

        static private void SendData(Stream streamToServer, byte[] data)
        {
            streamToServer.Write(data, 0, data.Length);
            streamToServer.Close();
        }

        static public byte[] ImportData(string dataAddr)
        {
            StreamReader reader = new StreamReader(dataAddr);
            var data = Encoding.UTF8.GetBytes(reader.ReadToEnd());
            return data;
        }  
    }
}
