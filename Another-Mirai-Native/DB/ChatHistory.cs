using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.DB
{
    public class ChatHistory
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }
        public long GroupId { get; set; }
        public long QQId { get; set; }
        public string Name { get; set; }
        public string Msg { get; set; }
        public DateTime Time { get; set; }
    }
}
