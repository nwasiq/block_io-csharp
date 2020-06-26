using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace block_io_lib
{
    class Program
    {
        static void Main(string[] args)
        {
            //string apiUrl = String.Format("https://jsonplaceholder.typicode.com/todos/1");
            //WebRequest requestObj = WebRequest.Create(apiUrl);
            //requestObj.Method = "GET";
            //HttpWebResponse responseObjGet = null;
            //responseObjGet = (HttpWebResponse)requestObj.GetResponse();
            //string result = null;
            //using (Stream stream = responseObjGet.GetResponseStream())
            //{
            //    StreamReader sr = new StreamReader(stream);
            //    result = sr.ReadToEnd();
            //    sr.Close();
            //}
            //Console.WriteLine("This is the result:" + JObject.Parse(result)["title"].ToString());


            //string encrypted = Helper.Encrypt("Wasiq", "12345678123456781234567812345678");
            //Console.WriteLine("This is what encrypt1 does " + encrypted);
            //string encrypted2 = Helper.Encrypt("Wasiq", "12345678123456781234567812345678");
            //Console.WriteLine("This is what encrypt2 does " + encrypted2);
            //Console.WriteLine("This is what decrypt does " + Helper.Decrypt(encrypted, "12345678123456781234567812345678"));
            //Console.WriteLine("This is what ExtractKey does " + Helper.ExtractKey(encrypted, "12345678123456781234567812345678"));
            //Console.WriteLine("This is what pintokey does: " + Helper.PinToKey("2794"));

            //string myJSON = "{\"name\":\"john\",\"age\":22,\"class\":\"mca\"}";
            //var myJsonObj = JsonConvert.DeserializeObject<dynamic>(myJSON);
            //Console.WriteLine(myJsonObj.name.GetType());
            //Key test = Helper.ExtractKeyFromEncryptedPassphrase("lalla", "llaalsdf");

            //string encryptionKey = Helper.PinToKey("50a0ffbc98bf8ea0c9322a02228a18e6");
            //Console.WriteLine("Calling pin to key gives this encryption key: " + encryptionKey );
            ////received key: +1sOM30JGfoIzrn+y/9qCH/JNFt6c1DPlLe0xoK0ins=
            //string encryptedData = Helper.Encrypt("block.io", encryptionKey);
            //Console.WriteLine("Calling encrypt data with received key: " + encryptedData);
            //Console.WriteLine("Calling decrypt data with received key. Decrypted data: " + Helper.Decrypt(encryptedData, encryptionKey));
            //string JsonString = "{ Age:  52}";
            //try
            //{
            //    dynamic stuff = JsonConvert.DeserializeObject("{allowNoPin: false}");
            //    //stuff.newParam = "param";
            //    //stuff = JsonConvert.DeserializeObject(JsonString);
            //    if (stuff.newParam == null)
            //    {
            //        Console.WriteLine("new param don't exist brah");
            //    }
            //    else
            //    {
            //        Console.WriteLine("new param DOES EXIST");
            //    }
            //    //Console.WriteLine(stuff);
            //}
            //catch(JsonReaderException ex)
            //{
            //    Console.WriteLine("An exception occurred, moving on.." + ex);
            //}

            BlockIo test = new BlockIo("{ api_key: '6094-2139-1c8c-21b1'}", "afusadfuhauidfhbzmxcv");
            //Console.WriteLine(test.GetNewAddress("{label: 'shibe2'}").Data);
            //Console.WriteLine(test.GetMyAddresses().Data);
            Console.WriteLine(test.Withdraw("{amounts: 0.0001, to_addresses:'2Mx7Wqey9Pg3PfH6iXff5avNB8havXLbKq9'}").Data);
            //"2NCUkmRWQzy82bwRTyDuWDyQ2zhWUVFBCZM",
            //string jsonString = "{addresses: ['2N8SB5MD5ev8tSKU363j9S9p5nZk111mFRZ', '2MsxwrZPN6pkMYxct8JvPKyU2sW2imtCUer']}";
            //var test2 = test.ValidateApiKey().Data;
            //Console.WriteLine(test2);
            //BlockIoResponse<> res = test.GetAddressBalance("user_id: 2").Data
        }

    }
}
