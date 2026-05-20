using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcGridGeneratorUi
{
    public static class AppSettings
    {
        // Path to AC installation
        public static string AssettoCorsaRoot { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\assettocorsa";

        // Path to Content Manager race grid presets
        public static string Presets { get; set; } = @"C:\Users\oculu\AppData\Local\AcTools Content Manager\Presets\Race Grids";
    }
}
