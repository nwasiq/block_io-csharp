using BlockIoLib.Examples;
using NBitcoin;
using System;
using System.Runtime.InteropServices.ComTypes;

namespace BlockIoLib
{
    class Program
    {
        static void Main(string[] args)
        {

            /**
             * TESTS
             */

            string api_key = "6094-2139-1c8c-21b1";
            string pin = "";

            string host = "";
            string user = "";
            string pass = "";
            string port = "";

            Basic basicExample = new Basic(api_key, pin);
            //basicExample.RunBasicExample();

            Proxy proxyExample = new Proxy(api_key, pin, host, user, pass, port);
            proxyExample.RunProxyExample();
            // Withdraw

            //BlockIo test = new BlockIo(api_key, pin);
            //
            //Console.WriteLine(test.WithdrawFromAddresses("{amounts: 0.1, from_addresses:['2N8pEWg9ZPyxa2yioZWDYAzNFyTnYp6TkHF'], to_addresses:['my9gXk65EzZUL962MSJadPXJFmJzPDc1WT']}").Data);

            // Sweep

            //BlockIo test = new BlockIo(api_key, pin);
            //string wif = "cUhedoiwPkprm99qfUKzixsrpN3w6wT2XrrMjqo3Yh1tHz8ykVKc";
            //string from_address = "my9gXk65EzZUL962MSJadPXJFmJzPDc1WT";
            //string sweepArgs = "{ private_key: '" + wif + "', to_address: '2N8pEWg9ZPyxa2yioZWDYAzNFyTnYp6TkHF'}";
            
            //Console.WriteLine(test.SweepFromAddress(sweepArgs).Data);

        }
    }
}
