using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Native
{
    public class PluginManagment
    {
        public static PluginManagment Instance { get; set; }
        public List<CQPlugin> Plugins { get; set; }
        public Dictionary<IntPtr, AppDomain> AppDomains { get; set; }

        public PluginManagment()
        {
            Instance = this;
        }


        internal void ReLoad()
        {
            throw new NotImplementedException();
        }

        internal static void CallFunction(object disable)
        {
            throw new NotImplementedException();
        }

        internal static void UnLoad()
        {
            throw new NotImplementedException();
        }

        internal void Init()
        {
            throw new NotImplementedException();
        }
    }
}
