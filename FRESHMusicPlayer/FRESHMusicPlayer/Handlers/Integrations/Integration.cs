using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace FRESHMusicPlayer.Handlers.Integrations
{
    
    class IntegrationData
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public int TrackNumber { get; set; }
        public int DiscNumber { get; set; }
        public int Bitrate { get; set; }
        public Image AlbumArt { get; set; }
        
    }
    abstract class Integration
    {
        /// <summary>
        /// Fetches metadata from the integration.
        /// </summary>
        /// <param name="query">The query for the integration to use. (Not all integrations use this)</param>
        /// <returns></returns>
        abstract public IntegrationData FetchMetadata(string query);
    }
}
