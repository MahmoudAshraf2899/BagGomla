using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class TokenAccess
    {
        [JsonProperty("access_token")]
        public string Access_Token { get; set; }

        [JsonProperty("token_type")]
        public string Token_Type { get; set; }

        [JsonProperty("expires_in")]
        public string Expires_In { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty(".issued")]
        public DateTime Issued { get; set; }
        [JsonProperty(".expires")]
        public DateTime Expires { get; set; }
    }
}