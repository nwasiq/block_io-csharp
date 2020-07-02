using NBitcoin;
using System;
using System.Runtime.InteropServices.ComTypes;

namespace block_io_lib
{
    class Program
    {
        static void Main(string[] args)
        {

            /**
             * TESTS
             */

            string WifTestStr = "L1cq4uDmSKMiViT4DuR8jqJv8AiiSZ9VeJr82yau5nfVQYaAgDdr";
            string PassphraseStr = "deadbeef";
            string DataToSignStr = "e76f0f78b7e7474f04cc14ad1343e4cc28f450399a79457d1240511a054afd63";
            string api_key = "27d6-fc3a-1606-e6d4";
            string pin = "Was1qWas1q";
            Key testKey;

            // Key tests

            Program.RunKeyTests(WifTestStr, PassphraseStr, DataToSignStr);

            // Withdraw

            BlockIo test = new BlockIo(api_key, pin);
            
            Console.WriteLine(test.WithdrawFromAddresses("{amounts: 0.1, from_addresses:['2N8pEWg9ZPyxa2yioZWDYAzNFyTnYp6TkHF'], to_addresses:['my9gXk65EzZUL962MSJadPXJFmJzPDc1WT']}").Data);

            // Sweep

            //BlockIo test = new BlockIo(api_key, pin);
            //string wif = "cUhedoiwPkprm99qfUKzixsrpN3w6wT2XrrMjqo3Yh1tHz8ykVKc";
            //string from_address = "my9gXk65EzZUL962MSJadPXJFmJzPDc1WT";
            //string sweepArgs = "{ private_key: '" + wif + "', to_address: '2N8pEWg9ZPyxa2yioZWDYAzNFyTnYp6TkHF'}";
            //
            //Console.WriteLine(test.SweepFromAddress(sweepArgs).Data);

        }

        public static void RunKeyTests(string WifTestStr, string PassphraseStr, string DataToSignStr) 
        {
            // Key From WIF

            Key testKey = new Key().ExtractKeyFromPassphrase(PassphraseStr);
            string privateKey = testKey.ToHex();
            string publicKey = testKey.PubKey.ToHex();
            string signature = Helper.SignInputs(testKey, DataToSignStr, publicKey);

            Console.WriteLine("Private Key From Passphrase: " + privateKey);
            Console.WriteLine("Public Key From Passphrase: " + publicKey);
            Console.WriteLine("Signature From Passphrase: " + signature);

            /////////////////////////////////

            Console.WriteLine("");
            Console.WriteLine("");

            // Key From Passphrase

            testKey = new Key().FromWif(WifTestStr);
            privateKey = testKey.ToHex();
            publicKey = testKey.PubKey.ToHex();
            signature = Helper.SignInputs(testKey, DataToSignStr, publicKey);

            Console.WriteLine("Private Key From WIF: " + privateKey);
            Console.WriteLine("Public Key From WIF: " + publicKey);
            Console.WriteLine("Signature From WIF: " + signature);
        }
    }
}
