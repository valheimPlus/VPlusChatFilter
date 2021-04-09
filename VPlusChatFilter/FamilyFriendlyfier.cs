using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPlusChatFilter
{
    public static class FamilyFriendlyfier
    {
        public static ProfanityFilter.ProfanityFilter filter;

        public static void Initialise()
        {
            filter = new ProfanityFilter.ProfanityFilter();
        }

        public static string[] GetWordsFromFile(string filename)
        {
            string reader = "";
            using (StreamReader sr = new StreamReader(Path.Combine(BepInEx.Paths.PluginPath, "Filters", filename)))
            {
                string line;
                // Read and display lines from the file until the end of
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    reader += line;
                }
            }
            string[] words = reader.Split(',');
            return words;
        } 
    }
}
