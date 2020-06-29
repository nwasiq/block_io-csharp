using System;
using System.Collections.Generic;
using System.Text;

namespace block_io_lib.ResponseObject
{

    class SignatureData
    {
        public SignatureResponse signature_data { get; set; }
    }
    class SignatureResponse
    {
        public string reference_id { get; set; }
        public string unsigned_tx_hex { get; set; }
        public EncryptedPassphrase encrypted_passphrase { get; set; }
        public List<SigInput> inputs { get; set; }
    }

    class SigInput
    {
        public string data_to_sign { get; set; }
        public int input_no { get; set; }
        public int signatures_needed { get; set; }
        public List<Signer> signers { get; set; }
    }

    class Signer
    {
        public string signer_public_key { get; set; }
        public string signed_data { get; set; }
    }

    class EncryptedPassphrase
    {
        public string signer_public_key { get; set; }
        public string passphrase { get; set; }
    }
}
