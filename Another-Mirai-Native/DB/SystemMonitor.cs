using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.DB
{
    public class SystemMonitor
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int RowID { get; set; }
        public long time { get; set; }
        public double cpu { get; set; }
        public double memory { get; set; }
        public int msgSpeed { get; set; }
    }
}
