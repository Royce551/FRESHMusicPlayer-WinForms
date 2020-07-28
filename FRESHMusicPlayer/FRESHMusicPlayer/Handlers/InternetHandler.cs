using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FRESHMusicPlayer.Handlers
{
    public static class InternetHandler
    {
        public static HttpClient HttpClient;
        static InternetHandler()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"FRESHMusicPlayer/8.0.0 (https://github.com/Royce551/FRESHMusicPlayer)"); // hi i'm FMP
        }
    }
}
