using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using System.Security.Cryptography;

namespace block_io_lib
{
    class Key: NBitcoin.Key
    {
        public Key(byte[] data, int count = -1, bool fCompressedIn = true) : base(data, count = -1, fCompressedIn = true)
        {
            
        }
        //public Key FromWif()
        //{
        //    Network.GetNetworks();
        //}
    }
}
