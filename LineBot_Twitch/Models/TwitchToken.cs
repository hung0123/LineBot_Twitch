using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineBot_Twitch.Models
{
    public class TwitchToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string[] scope { get; set; }
        public string token_type { get; set; }

    }
}
