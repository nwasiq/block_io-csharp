using NBitcoin;
using NBitcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace block_io_lib
{
    public class BlockIoResponse <T>
    {
        public string Status { get; set; }
        public T Data { get; set; }
    }
}
