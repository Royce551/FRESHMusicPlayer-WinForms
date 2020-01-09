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
            using (var mgr = new UpdateManager("C:\\Projects\\MyApp\\Releases"))
            {
                await mgr.UpdateApp();
            }
        }
    }
}
