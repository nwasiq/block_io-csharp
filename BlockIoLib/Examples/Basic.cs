using System;
using System.Collections.Generic;
using System.Text;

namespace BlockIoLib.Examples
{
    class Basic
    {
        private BlockIo blockIo;
        private string apiKey;
        private string pin;

        public Basic(string k, string p)
        {
            apiKey = k;
            pin = p;
            blockIo = new BlockIo(apiKey, pin);
        }

        public void RunBasicExample()
        {
            Console.WriteLine("Get New Address: " + blockIo.GetNewAddress("{label: 'testDest2'}").Data);
            Console.WriteLine("Withdraw from labels: " + blockIo.WithdrawFromLabels("{from_labels: 'default', to_label: 'testDest2', amount: 2.5}").Data);
            Console.WriteLine("Get Address Balance: " + blockIo.GetAddressBalance("{labels: ['default', 'testDest2']}").Data);
            Console.WriteLine("Get Sent Transactions: " + blockIo.GetTransactions("{type: 'sent'}").Data);
            Console.WriteLine("Get Received Transactions: " + blockIo.GetTransactions("{type: 'received'}").Data);
            Console.WriteLine("Get Current Price: " + blockIo.GetCurrentPrice("{base_price: 'BTC'}").Data);
        }
    }
}
