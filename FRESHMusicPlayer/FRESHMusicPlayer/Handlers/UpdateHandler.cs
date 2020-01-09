using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Squirrel;
namespace FRESHMusicPlayer.Handlers
{
    class UpdateHandler
    {
        public static async Task ProgramUpdate()
        {
            using (var mgr = UpdateManager.GitHubUpdateManager(@"https://github.com/Royce551/FRESHMusicPlayer/releases", null, null, null, Properties.Settings.Default.General_PreRelease))
            {
                await mgr.Result.UpdateApp();
            }
        }
    }
}
