using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace LagoVista.Client.Core.Net
{
    public class RestClient : IRestClient
    {
        HttpClient _client;

        public RestClient(HttpClient client)
        {
            _client = client;
        }

        

        
    }
}
