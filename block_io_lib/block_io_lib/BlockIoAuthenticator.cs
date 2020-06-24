﻿using RestSharp;
using RestSharp.Authenticators;

namespace block_io_lib
{
    class BlockIoAuthenticator : IAuthenticator
    {
        private readonly string _apiKey;

        public BlockIoAuthenticator(string apiKey)
        {
            _apiKey = apiKey;
        }
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.Parameters.Add(new Parameter("api_key", _apiKey, ParameterType.QueryString));
        }
    }
}

