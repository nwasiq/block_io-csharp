using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace block_io_lib
{
    class BlockIo
    {
        public dynamic Options;
        public string Key { get; set; }
        public int Version { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string Pin { get; set; }
        public string AesKey { get; set; }
        public Key ECKey { get; set; }

        public int DefaultVersion { get; set; } = 2;
        public string DefaultServer { get; set; } = "";
        public string DefaultPort { get; set; } = "";
        public string Host { get; set; } = "block.io";

        public BlockIo(string Config, string Pin = null, int Version = -1, string Options = null)
        {
            this.Options = JsonConvert.DeserializeObject("{allowNoPin: false}");
            dynamic ConfigObj;
            try
            {
                //config is an obj

                ConfigObj = JsonConvert.DeserializeObject(Config);
                Key = ConfigObj.api_key;

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
                            //Options is a string, not an obj
                        }
                    }
                }
            }
        }
    }
}
