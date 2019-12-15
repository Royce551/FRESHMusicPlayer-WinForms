using System.Collections.Generic;

namespace DatabaseFormat
{
    public class Format
    {
        public int Version { get; set; }
        public List<string> Songs { get; set; }
    }
}