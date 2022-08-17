using Another_Mirai_Native.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.DB
{
    public static class LogHelper
    {
        public delegate void AddLogHandler(int logid, LogModel log);
        public static event AddLogHandler LogAdded;
        public delegate void UpdateLogStatusHandler(int logid, string status);
        public static event UpdateLogStatusHandler LogStatusUpdated;
        public static string GetLogFileName()
        {
            var fileinfo = new DirectoryInfo($@"logs\{Helper.QQ}").GetFiles("*.db");
            string filename = "";
            foreach (var item in fileinfo)
            {
                if (item.Name.StartsWith($"logv2_{DateTime.Now:yyMM}"))
                {
                    filename = item.Name;
                    break;
                }
            }
            return string.IsNullOrWhiteSpace(filename) ? $"logv2_{DateTime.Now:yyMMdd}.db" : filename;
        }
        public static string GetLogFilePath()
        {
            if (Directory.Exists($@"logs\{Helper.QQ}") is false)
                Directory.CreateDirectory($@"logs\{Helper.QQ}");
            return Path.Combine(Environment.CurrentDirectory, $@"logs\{Helper.QQ}", GetLogFileName());
        }
        private static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new(new ConnectionConfig()
            {
                ConnectionString = $"data source={GetLogFilePath()}",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
            });
            return db;
        }
        public static void CreateDB()
        {
            using (var db = GetInstance())
            {
                string DBPath = GetLogFilePath();
                db.DbMaintenance.CreateDatabase(DBPath);
                db.CodeFirst.InitTables(typeof(LogModel));
            }
            WriteLog(LogLevel.InfoSuccess, "运行日志", $"日志数据库初始化完毕{DateTime.Now:yyMMdd}。");
        }
        public static string GetTimeStampString(long Timestamp)
        {
            DateTime time = Helper.TimeStamp2DateTime(Timestamp);
            StringBuilder sb = new();
            //sb.Append($"{(time.AddDays(1).Day == DateTime.Now.Day ? "昨天" : "今天")} ");
            sb.Append($"{time:MM/dd HH:mm:ss}");
            return sb.ToString();
        }
        public static int WriteLog(LogLevel level, string logOrigin, string type, string messages, string status = "")
        {
            LogModel model = new()
            {
                detail = messages,
                id = 0,
                source = logOrigin,
                priority = (int)level,
                name = type,
                time = Helper.TimeStamp,
                status = status
            };
            return WriteLog(model);
        }
        public static int WriteLog(LogModel model)
        {
            if (File.Exists(GetLogFilePath()) is false)
            {
                CreateDB();
            }
            if (!string.IsNullOrWhiteSpace(model.detail) && string.IsNullOrWhiteSpace(model.name))
            {
                model.name = "异常捕获";
                model.detail = model.name;
            }
            using (var db = GetInstance())
            {
                int logid = db.Insertable(model).ExecuteReturnIdentity();
                model.id = logid;
                LogAdded?.Invoke(logid, model);
                return logid;
            }
        }
        public static int WriteLog(int level, string logOrigin, string type, string messages, string status = "")
        {
            LogLevel loglevel = (LogLevel)Enum.Parse(typeof(LogLevel), Enum.GetName(typeof(LogLevel), level));
            return WriteLog(loglevel, logOrigin, type, messages, status);
        }
        public static int WriteLog(LogLevel level, string type, string message, string status = "")
        {
            return WriteLog(level, "AMN框架", type, message, status);
        }
        /// <summary>
        /// 以info为等级，"AMN框架"为来源，"提示"为类型写出一条日志
        /// </summary>
        /// <param name="messages">日志内容</param>
        public static int WriteLog(string messages, string status = "")
        {
            return WriteLog(LogLevel.Info, "AMN框架", "提示", messages, status);
        }

        public static LogModel GetLogByID(int id)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<LogModel>().First(x => x.id == id);
            }
        }
        public static void UpdateLogStatus(int id, string status)
        {
            using (var db = GetInstance())
            {
                db.Updateable<LogModel>().SetColumns(x => x.status == status).Where(x => x.id == id)
                  .ExecuteCommand();
                LogStatusUpdated?.Invoke(id, status);
            }
        }
        public static List<LogModel> GetDisplayLogs(int priority)
        {
            using (var db = GetInstance())
            {
                var c = db.SqlQueryable<LogModel>($"select * from log where priority>= {priority} order by id desc limit {Helper.MaxLogCount}").ToList();
                c.Reverse();
                return c;
            }
        }
        public static LogModel GetLastLog()
        {
            using (var db = GetInstance())
            {
                return db.SqlQueryable<LogModel>("select * from log order by id desc limit 1").First();
            }
        }
    }
}
