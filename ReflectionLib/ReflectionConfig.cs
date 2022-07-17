using System;
using System.Collections.Generic;
using System.Text;

namespace RaGae.ReflectionLib
{
    internal class ReflectionConfig
    {
        public string ReflectionPath { get; set; }
        public string FileSpecifier { get; set; }
        public IEnumerable<string> Files { get; set; }
    }
}
