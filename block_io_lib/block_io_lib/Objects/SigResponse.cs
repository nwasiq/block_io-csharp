using NBitcoin;
using NBitcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Text;

namespace block_io_lib
{
    class SigResponse
    {
        public string status { get; set; }
        public SigData data { get; set; }
    }

    class SigData
    {
        public string reference_id { get; set; }
        public string unsigned_tx_hex { get; set; }
        public EncryptedPassphrase encrypted_passphrase { get; set; }
        public List<SigInput> inputs { get; set; }
    }

    class SigInput
    {
        public uint256 data_to_sign { get; set; }
        public string input_no { get; set; }
        public string signatures_needed { get; set; }
        public List<Signer> signers { get; set; }
    }

    class Signer
    {
        public string signer_public_key { get; set; }
        public ECDSASignature signed_data { get; set; }
    }

    class EncryptedPassphrase
    {
        public string signer_public_key { get; set; }
        public string passphrase { get; set; }
    }
}
