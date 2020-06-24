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
    class BlockIo
    {
        private readonly RestClient RestClient;
        private readonly string ApiUrl;

        public dynamic Options;
        public string Key { get; set; }
        public int Version { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string Pin { get; set; }
        public string AesKey { get; set; }
        public Key ECKey { get; set; }

        public int DefaultVersion = 2;
        public string DefaultServer = "";
        public string DefaultPort = "";
        public string Host = "block.io";

        public string ERR_KEY_INV = "Error occurred validating key";
        public string ERR_PIN_MIS = "Missing 'pin', please supply as argument";
        public string ERR_PIN_INV = "Public key mismatch. Invalid Secret PIN detected.";
        public string ERR_PK_EXTR = "Could not extract privkey";
        public string ERR_WIF_MIS = "Missing mandatory private_key argument";
        public string ERR_WIF_INV = "Could not parse private_key as WIF";
        public string ERR_DEST_MIS ="Missing mandatory to_address argument";
        public string ERR_UNKNOWN = "Unknown error occured";

        public List<string> PASS_THROUGH_METHODS = new List<string>() {
          "get_balance", "get_new_address", "get_my_addresses", "get_address_received",
          "get_address_by_label", "get_address_balance", "create_user", "get_users",
          "get_user_balance", "get_user_address", "get_user_received",
          "get_transactions", "sign_and_finalize_withdrawal", "get_new_dtrust_address",
          "get_my_dtrust_addresses", "get_dtrust_address_by_label",
          "get_dtrust_transactions", "get_dtrust_address_balance",
          "get_network_fee_estimate", "archive_address", "unarchive_address",
          "get_my_archived_addresses", "archive_dtrust_address",
          "unarchive_dtrust_address", "get_my_archived_dtrust_addresses",
          "get_dtrust_network_fee_estimate", "create_notification", "disable_notification",
          "enable_notification", "get_notifications", "get_recent_notification_events",
          "delete_notification", "validate_api_key", "sign_transaction", "finalize_transaction",
          "get_my_addresses_without_balances", "get_raw_transaction", "get_dtrust_balance",
          "archive_addresses", "unarchive_addresses", "archive_dtrust_addresses", "unarchive_dtrust_addresses",
            "is_valid_address", "get_current_price", "get_account_info"
        };

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
        public string JsonToQuery(string jsonQuery)
        {
            string str = "?";
            str += jsonQuery.Replace(":", "=").Replace("{", "").
                        Replace("}", "").Replace(",", "&").
                            Replace("\"", "").Replace(" ", "").Replace("'", "");
            return str;
        }

        public void _withdraw(string Method, string Path, string args)
        {

        }

        public void _sweep(string Method, string Path, string args)
        {

        }

        public BlockIoResponse<dynamic> _request(string Method, string Path, string args="{}")
        {

            return GetNewAddress(Method, Path, args).Result;
        }

        public bool validate_key()
        {
            return true;
        }

        public string _constructPath(string Path, string Query = null)
        {
            //Query is a json string in format: "{name: John}"

            string QueryString = Query != null ? JsonToQuery(Query) : "";
            return Path + QueryString;
        }

        public async Task<BlockIoResponse<dynamic>> GetNewAddress(string Method, string Path, string args="{}")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = new RestRequest(Method == "POST" ? _constructPath(Path, "{api_key: '" + Key + "'}") : Path, (Method)Enum.Parse(typeof(Method), Method));

            if(Method == "POST")
            {
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddJsonBody(JsonConvert.DeserializeObject(args));
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
