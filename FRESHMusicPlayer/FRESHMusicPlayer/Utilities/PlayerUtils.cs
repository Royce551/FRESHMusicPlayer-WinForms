using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRESHMusicPlayer;
namespace FRESHMusicPlayer.Utilities
{
    static class PlayerUtils
    {
        private static Random rng = new Random();

        public static List<string> ShuffleQueue(List<string> list)
        {
            List<string> listtosort = new List<string>();
            List<string> listtoreinsert = new List<string>();
            int number = 0;
            foreach (string x in list)
            {
                if (Player.QueuePosition < number) listtosort.Add(x);
                else listtoreinsert.Add(x);
                number++;
            }
            
            int n = listtosort.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = listtosort[k];
                listtosort[k] = listtosort[n];
                listtosort[n] = value;
            }
            foreach (string x in listtosort) listtoreinsert.Add(x);
            return listtoreinsert;
        }
    }
}
