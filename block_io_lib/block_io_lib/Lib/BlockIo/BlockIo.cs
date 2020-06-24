using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace block_io_lib
{
    /**
     * A note on args
     * 
     * args are in JSON format and need to be passed like this: 
     * "{param1: 'stringValue', param2: intValue, param3: ['this', 'is', 'a', 'list']}"
     * 
     * 
     * A note on lib method usage
     * 
     * BlockIo blockLib = new BlockIo("{ api_key: 'key' }");
     * var res = blockLib.GetBalance().Data;
     * var status = blockLib.GetBalance().Status;
     */

    public partial class BlockIo
    {
        private readonly RestClient RestClient;
        private readonly string ApiUrl;

        private dynamic Options;
        private string Key { get; set; }
        private int Version { get; set; }
        private string Server { get; set; }
        private string Port { get; set; }
        private string Pin { get; set; }
        private string AesKey { get; set; }
        private Key ECKey { get; set; }

        private int DefaultVersion = 2;
        private string DefaultServer = "";
        private string DefaultPort = "";
        private string Host = "block.io";

        public List<string> WITHDRAWAL_METHODS = new List<string>() {
          "withdraw", "withdraw_from_user", "withdraw_from_label",
          "withdraw_from_address", "withdraw_from_labels", "withdraw_from_addresses",
          "withdraw_from_users", "withdraw_from_dtrust_address", "withdraw_from_dtrust_addresses",
          "withdraw_from_dtrust_labels"
        };

        public List<string> SWEEP_METHODS = new List<string>() { "sweep_from_address" };

        public BlockIo(string Config, string Pin = null, int Version = 2, string Options = null)
        {
            this.Options = JsonConvert.DeserializeObject("{allowNoPin: false}");
            dynamic ConfigObj;
            try
            {
                //config is an obj

                ConfigObj = JsonConvert.DeserializeObject(Config);
                Key = ConfigObj.api_key;
                if (ConfigObj.version != null) this.Version = ConfigObj.version; else this.Version = this.DefaultVersion;
                if (ConfigObj.server != null) this.Server = ConfigObj.server; else this.Server = this.DefaultServer;
                if (ConfigObj.port != null) this.Port = ConfigObj.port; else this.Port = this.DefaultPort;

                if(ConfigObj.pin != null)
                {
                    this.Pin = ConfigObj.pin;
                    this.AesKey = Helper.PinToKey(this.Pin);
                }
                if(ConfigObj.options != null)
                {
                    this.Options = ConfigObj.options;
                    this.Options.allowNoPin = false;
                }
            }
            catch(Exception ex)
            {
                //config is a string, not an obj

                Key = Config;
                if (Version == -1) this.Version = this.DefaultVersion; else this.Version = Version;
                this.Server = this.DefaultServer;
                this.Port = this.DefaultPort;
                if (Pin != null)
                {
                    this.Pin = Pin;
                    this.AesKey = Helper.PinToKey(this.Pin);
                    if(Options != null)
                    {
                        try
                        {
                            this.Options = JsonConvert.DeserializeObject(Options);
                            this.Options.allowNoPin = false;
                        }
                        catch(Exception ex2)
                        {
                            //Options is a string, not an obj: Do nothing
                        }
                    }
                }
            }
            string ServerString = Server != "" ? Server + "." : Server;  // eg: 'dev.'
            string PortString = Port != "" ? ":" + Port : Port;

            ApiUrl = "https://" + ServerString + Host + PortString + "/api/v" + Version.ToString();

            RestClient = new RestClient(ApiUrl) { Authenticator = new BlockIoAuthenticator(Key) };
        }
        private string JsonToQuery(string jsonQuery)
        {
            jsonQuery = jsonQuery.Replace(":", "=").Replace("{", "").
                        Replace("}", "").Replace(",", "&").
                            Replace("\"", "").Replace(" ", "").Replace("'", "");

            int lastAmpercant = -1;

            for(int i=0; i<jsonQuery.Length; i++)
            {
                if (jsonQuery[i] == '&')
                {
                    lastAmpercant = i;
                    jsonQuery = jsonQuery.Remove(i, 1).Insert(i, ",");
                }
                if (jsonQuery[i] == '=')
                {
                    if (lastAmpercant != -1)
                    {
                        jsonQuery = jsonQuery.Remove(lastAmpercant, 1).Insert(lastAmpercant, "&");
                    }
                }
            }
            return jsonQuery.Replace("[", "").Replace("]", "");
        }


        private void _withdraw(string Method, string Path, string args)
        {

        }

        private void _sweep(string Method, string Path, string args)
        {

        }

        public BlockIoResponse<dynamic> ValidateKey()
        {
            return _request("GET", "get_balance").Result;
        }

        public string _constructPath(string Path, string Query = null)
        {
            //Query is a json string in format: "{name: John}"

            string QueryString = Query != null ? "?" + JsonToQuery(Query) : "";
            return Path + QueryString;
        }

        private async Task<BlockIoResponse<dynamic>> _request(string Method, string Path, string args="{}")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = new RestRequest(Method == "POST" ? _constructPath(Path, "{api_key: '" + Key + "'}") : Path, (Method)Enum.Parse(typeof(Method), Method));

            if(Method == "POST")
            {
                request.AddHeader("Content-Type", "application/json");
                string queryString = JsonToQuery(args);
                if(args != "{}")
                {
                    string[] querySegments = queryString.Split('&');
                    foreach (string segment in querySegments)
                    {
                        string[] parts = segment.Split('=');
                        if (parts.Length > 0)
                        {
                            string key = parts[0].Trim(new char[] { '?', ' ' });
                            string val = parts[1].Trim();
                            request.AddParameter(key, val, ParameterType.QueryString);
                        }
                    }
                }
            }
            else
            {
                dynamic qs = JsonConvert.DeserializeObject(args);
                qs.api_key = Key;
                request.AddJsonBody(qs);
            }
            var response = await RestClient.ExecuteGetAsync(request);
            CheckBadRequest(response);
            return GetData<BlockIoResponse<dynamic>>(response);
        }
        private void CheckBadRequest(IRestResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(response.Content);
            }
        }
        private T GetData<T>(IRestResponse response)
        {
            var data = JsonConvert.DeserializeObject<T>(response.Content);
            return data;
        }
    }
}
