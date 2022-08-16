using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.CQCode.Core
{
	internal class Kernel32
	{
		[DllImport ("kernel32.dll", EntryPoint = "lstrlenA", CharSet = CharSet.Ansi)]
		public extern static int LstrlenA (IntPtr ptr);
	}
}
