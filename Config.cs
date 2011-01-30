//#define SILENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tarnish
{
    public static class Config
    {
        public static bool Silent = 
        #if SILENT 
            true;
        #else
            false;
        #endif
    }
}