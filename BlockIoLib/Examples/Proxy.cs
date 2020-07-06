using System;
using System.Collections.Generic;
using System.Text;

namespace BlockIoLib.Examples
{
    class Proxy
    {
        private BlockIo blockIo;
        private string apiKey;
        private string pin;
        private string proxyHost;
        private string proxyUser;
        private string proxyPass;
        private string proxyPort;
        public Proxy(string k, string p, string host, string user, string pass, string port)
        {
            apiKey = k;
            pin = p;
            proxyHost = host;
            proxyPass = pass;
            proxyPort = port;
            proxyUser = user;
            blockIo = new BlockIo(apiKey, pin, 2, "{proxy: {hostname: " + proxyHost + ", port: " + proxyPort + ", username: " + proxyUser + ", password: " + proxyPass + "}");
        }

        public void RunProxyExample()
        {
            Console.WriteLine("Get Balance: " + blockIo.GetBalance().Data);

            Console.WriteLine("Get New Address: " + blockIo.GetNewAddress("{label: 'testDest4'}").Data);
            Console.WriteLine("Withdraw from labels: " + blockIo.WithdrawFromLabels("{from_labels: 'default', to_label: 'testDest4', amount: 0.003}").Data);
            Console.WriteLine("Get Address Balance: " + blockIo.GetAddressBalance("{labels: ['default', 'testDest4']}").Data);
            Console.WriteLine("Get Sent Transactions: " + blockIo.GetTransactions("{type: 'sent'}").Data);
            Console.WriteLine("Get Received Transactions: " + blockIo.GetTransactions("{type: 'received'}").Data);
            Console.WriteLine("Get Current Price: " + blockIo.GetCurrentPrice("{base_price: 'BTC'}").Data);
        }
    }
}
