using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.Crypto;
using Newtonsoft.Json;

namespace block_io_lib.Objects
{
    public class SignatureResponse
    {
        [JsonProperty("reference_id")]
        public string reference_id { get; set; }
        [JsonProperty("unsigned_tx_hex")]
        public string unsigned_tx_hex { get; set; }
        [JsonProperty("encrypted_passphrase")]
        public EncryptedPassphrase encrypted_passphrase { get; set; }
        [JsonProperty("inputs")]
        public IList<SigInput> inputs { get; set; }
    }

    public class SigInput
    {
        [JsonProperty("data_to_sign")]
        public uint256 data_to_sign { get; set; }
        [JsonProperty("input_no")]
        public string input_no { get; set; }
        [JsonProperty("signatures_needed")]
        public string signatures_needed { get; set; }
        [JsonProperty("signers")]
        public IList<Signer> signers { get; set; }
    }

    public class Signer
    {
        [JsonProperty("signer_public_key")]
        public string signer_public_key { get; set; }
        [JsonProperty("signed_data")]
        public ECDSASignature signed_data { get; set; }
    }

    public class EncryptedPassphrase
    {
        [JsonProperty("signer_public_key")]
        public string signer_public_key { get; set; }
        [JsonProperty("passphrase")]
        public string passphrase { get; set; }
    }
    
}
