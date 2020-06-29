using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using System.Security.Cryptography;
using Base58Check;
using System.Linq;

namespace block_io_lib
{
    class Key: NBitcoin.Key
    {
        public Key(byte[] data, int count = -1, bool fCompressedIn = true) : base(data, count = -1, fCompressedIn = true)
        {
            
        }
        public Key FromWif(string PrivKey)
        {
            byte[] ExtendedKeyBytes = Base58CheckEncoding.Decode(PrivKey);
            bool Compressed = false;

            //skip the version byte
            ExtendedKeyBytes = ExtendedKeyBytes.Skip(1).ToArray();
            if (ExtendedKeyBytes.Length == 33)
            {
                if (ExtendedKeyBytes[32] != 0x01)
                {
                    throw new System.ArgumentException("Invalid compression flag", "PrivKey");
                }
                ExtendedKeyBytes = ExtendedKeyBytes.Take(ExtendedKeyBytes.Count() - 1).ToArray();
                Compressed = true;
            }

            if (ExtendedKeyBytes.Length != 32)
            {
                throw new System.ArgumentException("Invalid WIF payload length", "PrivKey");
            }

            return new Key(ExtendedKeyBytes, -1, Compressed);
        }
    }
}
