using Another_Mirai_Native.DB;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Instrumentation;
using System.Threading;
using static Another_Mirai_Native.WsServer;

namespace Another_Mirai_Native
{
    public static class UsageMonitor
    {
        private static string MonitorFile { get; set; } = @"logs\monitor.db";
        public static bool ExitFlag { get; set; }
        private static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new(new ConnectionConfig()
            {
                ConnectionString = $"data source={MonitorFile}",
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
                string DBPath = MonitorFile;
                db.DbMaintenance.CreateDatabase(DBPath);
                db.CodeFirst.InitTables(typeof(SystemMonitor));
            }
        }
        public static void StartRecord()
        {
            new Thread(() =>
            {
                while (!ExitFlag)
                {
                    Thread.Sleep(1000);

                    if (DeviceInformation.Instance == null || DeviceInformation.Instance.TotalMemory == 0) continue;
                    var totalMemory = DeviceInformation.Instance.TotalMemory / 1024;
                    var leftMemory = DeviceInformation.Instance.MemoryCounter.NextValue();
                    var cpu = DeviceInformation.Instance.CpuCounter.NextValue();
                    if (cpu > 100) cpu = 100;
                    var log = new SystemMonitor
                    {
                        cpu = cpu,
                        memory = (1 - leftMemory / totalMemory) * 100,
                        msgSpeed = Helper.MsgSpeed.Count,
                        time = Helper.TimeStamp
                    };
                    using (var db = GetInstance())
                    {
                        db.Insertable(log).ExecuteCommand();
                    }
                    if (new FileInfo(MonitorFile).Length / 1024 / 1024 > 1)
                    {
                        File.Delete(MonitorFile);
                        CreateDB();
                    }
                }
            }).Start();
        }
        public static List<SystemMonitor> GetMinuteMonitor()
        {
            using (var db = GetInstance())
            {
                DateTime dt = DateTime.Now;
                return db.Queryable<SystemMonitor>().OrderByDescending(x => x.time).Take(60).ToList();
            }
        }
    }
}
