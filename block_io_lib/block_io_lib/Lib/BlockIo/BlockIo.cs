using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using block_io_lib.ResponseObject;
using NBitcoin.Crypto;
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

        public BlockIo(string Config, string Pin = null, int Version = 2, string Options = null)
        {
            this.Options = JsonConvert.DeserializeObject("{allowNoPin: false}");
            this.Pin = Pin;
            this.AesKey = null;
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
                    this.AesKey = Helper.PinToAesKey(this.Pin);
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
                    this.AesKey = Helper.PinToAesKey(this.Pin);
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
                // need to handle ',' within a json array
                //'&' will always come after '=' if at all in a query string
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


        private Task<BlockIoResponse<dynamic>> _withdraw(string Method, string Path, string args)
        {
            BlockIoResponse<dynamic> res = null;
            try
            {
                dynamic argsObj = JsonConvert.DeserializeObject(args);
                string pin = argsObj.pin != null ? argsObj.pin : this.Pin;
                argsObj.pin = "";
                Task<BlockIoResponse<dynamic>> RequestTask = _request(Method, Path, args);
                 res = RequestTask.Result;
                if(res.Status == "fail" || res.Data.reference_id == null
                || res.Data.encrypted_passphrase == null || res.Data.encrypted_passphrase.passphrase == null) 
                    return RequestTask;

                if (pin == null)
                {
                    if(this.Options.allowNoPin == true)
                    {
                        return RequestTask;
                    }
                    throw new Exception("Public key mismatch. Invalid Secret PIN detected.");
                }
                
                string enrypted_passphrase = res.Data.encrypted_passphrase.passphrase;
                string aesKey = this.AesKey != null ? this.AesKey: Helper.PinToAesKey(pin);
                Key privKey = Helper.ExtractKeyFromEncryptedPassphrase(enrypted_passphrase, aesKey);
                string pubKey = privKey.PubKey.ToHex();
                if (pubKey != res.Data.encrypted_passphrase.signer_public_key.ToString())
                    throw new Exception("Public key mismatch. Invalid Secret PIN detected.");

                foreach(dynamic input in res.Data.inputs)
                {
                    foreach(dynamic signer in input.signers)
                    {
                        signer.signed_data = Helper.SignInputs(privKey, input.data_to_sign.ToString(), pubKey);
                    }
                }

                aesKey = "";
                privKey = null;
                return _request(Method, "sign_and_finalize_withdrawal", "{signature_data: " + res.Data + "}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private Task<BlockIoResponse<dynamic>> _sweep(string Method, string Path, string args)
        {
            return _request(Method, Path, args);
        }

        public BlockIoResponse<dynamic> ValidateKey()
        {
            return _request("GET", "get_balance").Result;
        }

        public string _constructPath(string Path, string Query = null)
        {
            //Query is a json string in format: "{name: 'John'}"

            string QueryString = Query != null ? "?" + JsonToQuery(Query) : "";
            return Path + QueryString;
        }

        private async Task<BlockIoResponse<dynamic>> _request(string Method, string Path, string args="{}")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = new RestRequest(Method == "POST" ? _constructPath(Path, "{api_key: '" + Key + "'}") : Path, (Method)Enum.Parse(typeof(Method), Method));

            if (Method == "POST" && Path != "sign_and_finalize_withdrawal")
            {
                dynamic dynArgs = JsonConvert.DeserializeObject(args);
                dynArgs.api_key = "";
                args = JsonConvert.SerializeObject(dynArgs);
                string queryString = JsonToQuery(args);
                if (args != "{}" )
                {
                    string[] querySegments = queryString.Split('&');
                    foreach (string segment in querySegments)
                    {
                        string[] parts = segment.Split('=');
                        if (parts.Length > 0)
                        {
                            string key = parts[0].Trim(new char[] { '?', ' ' });
                            string val = parts[1].Trim();
                            if(val != "") request.AddParameter(key, val, ParameterType.QueryString);
                        }
                    }
                }
            }
            else
            {
                SignatureData obj = JsonConvert.DeserializeObject<SignatureData>(args);
                Console.WriteLine("The SIGNATURE: " + obj.signature_data.inputs[0].signers[0].signed_data);
                //var json = JsonConvert.SerializeObject(obj);
                //Console.WriteLine("Created JSONS obj: " + JsonConvert.DeserializeObject(args));
                //qs.api_key = Key;
                request.AddJsonBody(obj);
            }
            var response = Path != "sign_and_finalize_withdrawal" ? await RestClient.ExecuteGetAsync(request) : await RestClient.ExecutePostAsync(request);
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
