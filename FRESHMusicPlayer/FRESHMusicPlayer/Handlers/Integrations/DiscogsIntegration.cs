using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FRESHMusicPlayer.Forms;
using System.Windows.Forms;
using System.Drawing;
namespace FRESHMusicPlayer.Handlers.Integrations
{
    class DiscogsIntegration : Integration
    {
        private readonly string Key = "rYhrWVjHmbqOhVijxBtk";
        private readonly string Secret = "TaUMdjJnmmcjGttJbegdmRyOHyqQxljK";

        public override IntegrationData FetchMetadata(string query)
        {   
            var json = JObject.Parse(Player.HttpClient.GetStringAsync($"https://api.discogs.com/database/search?q={{{query}}}&{{track}}&per_page=1&key={Key}&secret={Secret}").Result);
            IntegrationData data = new IntegrationData
            {                                               // Reference for json format - https://www.discogs.com/developers#page:database,header:database-search
                Album = json.SelectToken("results[0].title").ToString(),
                AlbumArt = Image.FromStream(Player.HttpClient.GetStreamAsync(json.SelectToken("results[0].cover_image").ToString()).Result),
                Genre = json.SelectToken("results[0].genre").ToString(),
                Year = (int)json.SelectToken("results[0].year")
            };
            return data;
        }
    }
}
