using System;
using System.IO;
using System.Net;
using System.Web;

namespace block_io_lib
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiUrl = String.Format("https://jsonplaceholder.typicode.com/todos/1");
            WebRequest requestObj = WebRequest.Create(apiUrl);
            requestObj.Method = "GET";
            HttpWebResponse responseObjGet = null;
            responseObjGet = (HttpWebResponse)requestObj.GetResponse();
            string result = null;
            using (Stream stream = responseObjGet.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
                sr.Close();
            }
            Console.WriteLine("This is the result:"+  result);

        }
    }
}
