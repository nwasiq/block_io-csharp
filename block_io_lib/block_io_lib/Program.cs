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
            string api_key = "6094-2139-1c8c-21b1";
            string pin = "";
            Key testKey;

            // Key tests

            //Program.RunKeyTests(WifTestStr, PassphraseStr, DataToSignStr);

            // Withdraw

            //BlockIo test = new BlockIo(api_key, pin);
            //Console.WriteLine(test.Withdraw("{amounts: 0.0001, to_addresses:'2Mx7Wqey9Pg3PfH6iXff5avNB8havXLbKq9'}").Data);

            // Sweep

            BlockIo test = new BlockIo(api_key, pin);
            string wif = "cTYLVcC17cYYoRjaBu15rEcD5WuDyowAw562q2F1ihcaomRJENu5";
            string sweepArgs = "{ private_key: '" + wif + "', to_address: 'QhSWVppS12Fqv6dh3rAyoB18jXh5mB1hoC', from_address: 'tltc1qpygwklc39wl9p0wvlm0p6x42sh9259xdjl059s'}";
            
            test.SweepFromAddress(sweepArgs);

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
