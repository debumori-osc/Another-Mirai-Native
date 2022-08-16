using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Forms;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native
{
    /// <summary>
    /// 托盘帮助类
    /// </summary>
    public static class NotifyIconHelper
    {
        public static NotifyIcon Instance { get; set; }
        private static string Icon = "AAABAAEAICAAAAEAGACoDAAAFgAAACgAAAAgAAAAQAAAAAEAGAAAAAAAAAAAAMQOAADEDgAAAAAAAAAAAAD///////+enp5+fYF1dHZ1dHZ/foCAf4F/foB7enx7enyAf4GAf4GAf4GBgIJ4fX55fn19goF9goF9gYJ9gYJ4fH18e32Af4OBgISBgIR9fH59fH6BgIJyc3H///////////9+fYGBgIJ+gIF2eHh2eHh8gYB/gYJ9gYJ4fH19fH5/gISBgISBgISBgIJ9fH59fH6CgYOBgIKBgYGBgYF9fX1+fX+CgYODgoSBg4R9f4B8fn9/g4SBg4SBg4P///+dnZ2BgISBgITFxMa4t7m3trjGxsbGxsbFxcW+vr6/vsDGxsbGxsbGxsbGxce/v7+/v7/GxsbGxsbGxsbGxsa/wL6/wL7Gx8XGx8XGxsa/wL6/wL7FxcWEhISEhIR6enqCgYOBg4TBw8TGxsa5ubm5ubnFxcXGxsbHx8fAwMC/v7/Hx8fGxsbGxsbGxsbAwMDAwMDGxsbGxsbGxsbGxsbBwcHAwMDGxsbGxsbGxsbAwMDAv8HGxsbGxsaChIWChIWDgoaDgoTGx8XGxsa6urq6urrHx8fIyMjHx8fAwMDAwMDHx8fIyMjIyMjIyMjAwb/Awb/IyMjIyMjIyMjIyMjCwsLCwsLIyMjJycnJycnCwsLCwsLJyMrJyMqGhoaGhYeAhIWChIXJyMrJyMq8u728u73IyMjJyMrJyMrCwsLDw8PJycnJycnJycl+fX9+fn5+fn7ExMTJycnJycnJycnDw8PDw8PKy8nJycnJycnDxMLDxMLJycnJycmFhoqHhoqEg4WEg4XJysjJycm9vb29vb3JycnJycnJycnDw8PDw8PJycnJycnJycmAf4GAf4GAf4F5eHrJycnJycnJycnExMTDw8PKysrKysrLy8vDw8PDw8PLy8vLy8uHhoiJiIqFh4iDh4jLzMrLy8u9vb29vb3KysrLy8vLy8uBgILFxcXLy8uFhYWBgIKBgIKBgIKBgIKBgIKBgILMzMzMy83CwsKCgYPKysrMzMzMzMzFxcXGxsbMzMzMzcuJiIyJiIyIh4mHhojMzcvMzMy/vsC/v7/Ly8vMzcuDgoaDgoaDgoaDgoaDgoaDgoaDgoaDgoaDgoaDgoaDgoaDgoaDgoaDgoaDgoaDgobMzMzMzMzHx8fGxsbNzc3MzMyLioyLi4uJiIqJiIrMzcvMzMy/v7+/wL7Nzc2AhIWChIWChIWChIWChIWChIWChIWChIWChIWChIWChIWChIWChIWChIWChIWChIWChIXOzs7Ozs7GxsbGxsbPz8/Pz8+KjI2IjI2JiIqKiYvPz8/Pz8/BwcHAwMDNzc3T09OEhoeEhoeEhoeEhoeEhofOzs7Pz8/IyMjIyMjOzs60tLSEhoeEhoeEhoeEhoeEhofPz8/Pz8/IyMjIyMjPz8/Pz8+NjY2NjY2KiYuLiozQ0c/Q0NDBwcHCwsLPz8/Q0NCHh4eHh4eHh4fR0s7Pz8/Pz8/////+/v7R0dHP0M7Pz8/Pz8+Hh4eHh4eHh4eIiIjPz8/Pz8/JycnJycnR0dHQ0NCNjI6OjY+Mi4+Mi4/Pz8/R0dHCwsLDw8PQ0c+IhoyJiIyJiIzMzcvR0dH////////////////////////////R0dHR0dGJiIyJiIyJiIzR0tDR0dHJycnJycnR0dHR0dGPjpKPjpCOjY+NjI7R0tDR0dHExMTDw8PR0dGKjI2KjI2KjI3Ly8v////////////////+/v7////////////39/fT09OKiYuKjI2KjI3HycnS0dPMy83My83S0dPS0tKQj5GRkJKLj5CLj5DS0tLS0dPFxsSNjI6NjY2NjY2NjY3Ly8vMzcv///////+PjpCNjY2NjY2NjY2QkJD////////U1NTNzc2NjY2NjY2NjY2NjY3Dw8PMzMzU1NTU1NSOkpOOkpOPj4+QkJDU1NTU1NTGxsaLj5CLj5CLj5CLj5DMzcv+/v7///////+Lj5CLj5CLj5CLj5CLj5D////////T09POzs6Lj5CLj5CLj5CLj5CLj5DOzs7V1dXV1dWTkpSTkpSQj5GSkZPV1tTV1dXHx8eQj5GQj5GQj5GQj5HP0M7+/v7///+QkJCQj5GQj5GQj5GQj5GQj5H////////a2trQ0NCQj5GQj5GQj5GQj5GQj5HP0M7V1dXV1dWUlJSRk5OSkZOSkZPV1tTu7u7e3t6SkZOSkZOSkZOSkZPk5+X+/v7////9/f2QkpOQkpOQkpOQkpOQkpP///7////t7e3n5+eSkZOSkZOSkZOSkZPm5efn5+ft7e3W1taRlZaTlZaSkpKTk5Pv7+/t7e3f39/e3t7t7e2TlJKTlJKTlJLn5+f///////+Sk5GSk5GSk5GSk5GUlZP////////t7e2Tk5OTlJKTlJLx8vDt7e3n5+fn5+ft7e3t7e2WlZeWlZeRk5SPk5Tv8O7v7+/f39/e3t7v7++SlJWTlZaTlZbn5+f////////////8/fv6/P3+//3////////8/Pzv7++TlZaTlZaTlZbv8O7v7+/n5+fo6Ojv7+/v7++SlpeSlpeVlJaVlJbv8O7v7+/g4ODg4ODu7u7v7++Vl5iVl5iVl5jv7+/////////////////////////////w8PCSlJWVl5iVl5iVl5jw8PDw8PDp6enp6enw8PDw8PCYmJiYmJiWlZeXlpjw8e/w8PDh4eHh4uDv8O7n5+eYmJiYmJiYmJiYmJjw8PDw8PD////////l5eXw8PDw8PCYl5mYmJiYmJiYmJiYl5nw8PDw8PDn6enp6enw8PDw8PCZmJqamZuWlZmWlZfw8PDw8PDi4uLi4uLw8PCampqamZuamZuamZuamZuamZuampqbm5uUlJSdnZ2ampqamZuamZuamZuamZuamZuamZvx8fHw8PDp6enp6enw8PDw8PCXnJqXnJuZmJqZmJrw8e/w8PDi4uLh4eHx8fHx8fGbm5ubm5ubm5ubm5ubm5ubm5ubm5ubm5ubm5ubm5ubm5ubm5ubm5ubm5ubm5ubm5vy8vLy8vLq6urq6ury8vLy8vKbm5ubm5uZmJqZmJry8/Hy8vLj4+Pj4+Py8vLy8vLz8/Ocm53q6ury8vKlpaWcm52cm52cm52cm52cm52cm53w8e/y8vLq6uqcm53x8fHy8vLy8vLq6urq6ury8vLy8vKcm52cm52bmpybmpzy8/Hy8vLj4+Pj4+Px8fHy8vLy8vLq6urq6ury8vLy8vLy8vKcn52cn52cn52Ul5Xz8/Py8vLz8/Pr6+vq6ury8vLz8/Pz8/Pq6urq6urz8/Pz8/Oanp+anp+bm5ucnJzz8/Pz8/Pk5OTk5OTy8vLz8/Pz8/Pr6+vs7Ozz8/Pz8/Pz8/Ofn5+fn5+fn5/x8vDz8/Pz8/Pz8/Ps7Ozs7Ozz8/Pz8/Pz8/Ps7Ozs7Ozz8/Pz8/Ocnp6bnZ2cm52cm53z9PLz8/Pk5OTk5OTz8/Pz8/Pz8/Pr6+vr6+vz8/Pz8/Pz8/Pz8/Ps7Ozs7Ozz8/Pz8/P09PTz8/Pr6+vs7Ozz8/P09PTz8/Ps7Ozs7Ozz8/Pz8/Ofn5+fn5+dn5+boJ/x8vDz8/Pw8PDw8PD+/v7////////29vb29vb////////////////29vb29vb////////////////39/f29vb+/v7////////29vb29vbz8/Pz8/OhoKKhoKKTk5Oenp6fn5/19fXw8PDw8PD+/v7////////29vb29vb////////////////29vb29vb////////////////39/f29vb+/v7////////29vb29vb19fWfn5+eoqOho6T///+fn5+fn5+fnqCWlZeXlpigoKChoKKhoKKcnJycm52hoKKioaOgoqOgoqOYnJ2YnJ2goqOeoqOgoqOfoaGenp6enZ+ioqKio6GioqKenZ+enZ+ioqKioqKioqL///////////+an56goqOUmJmWmJmeoqOeoqOgoqKbnZ6enZ+io6Gio6GioqKioqKenZ+enZ+io6GioqKioqKioqKenp6enZ+ioqKioqKioqKen52en52ioqKkpKT///////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
        public static void Init()
        {
            Instance.Icon = new Icon(new MemoryStream(Convert.FromBase64String(Icon)));
            Instance.ContextMenu = new ContextMenu();
            ContextMenu menu = Instance.ContextMenu;
            menu.MenuItems.Add(new MenuItem() { Text = Helper.NickName, Name = "UserName" });
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(new MenuItem() { Text = "应用", Name = "PluginMenu" });
            menu.MenuItems.Add(new MenuItem() { Text = "日志", Tag = "LogForm" });
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(new MenuItem() { Text = "显示悬浮窗", Tag = "Displaywindow", Checked = FloatWindow.Instance.ShowFlag });
            menu.MenuItems.Add(new MenuItem() { Text = "窗口置顶", Tag = "TopMost", Checked = FloatWindow.Instance.TopMostFlag });
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(new MenuItem() { Text = "重载应用", Tag = "ReLoad" });
            menu.MenuItems.Add(new MenuItem() { Text = "退出", Tag = "Quit" });

            menu.MenuItems[3].Click += MenuItem_Click;
            menu.MenuItems[5].Click += MenuItem_Click;
            menu.MenuItems[6].Click += MenuItem_Click;
            menu.MenuItems[8].Click += MenuItem_Click;
            menu.MenuItems[9].Click += MenuItem_Click;
        }
        public static void LoadMenu(JObject json)//初始化,遍历json的menu节点
        {
            MenuItem menu = Instance.ContextMenu.MenuItems.Find("PluginMenu", false).First();
            MenuItem menuItem = new()//一级菜单,插件的名称
            {
                Name = json["name"].ToString(),
                Text = json["name"].ToString()
            };
            if (!json.ContainsKey("menu"))
                return;
            foreach (var item in JArray.Parse(json["menu"].ToString()))
            {
                MenuItem childmenu = new()//二级菜单,窗口的名称
                {
                    Text = item["name"].ToString(),
                    Tag = new KeyValuePair<string, string>(json["name"].ToString(), item["name"].ToString())//插件名称与窗口函数名称,保存于这个菜单的tag中
                };
                menuItem.MenuItems.Add(childmenu);//加入二级子菜单
                childmenu.Click += MenuItem_Click;
            }
            menu.MenuItems.Add(0, menuItem);//加入一级子菜单
        }

        private delegate int Type_Menu();//窗口事件均为无参数
        private static Type_Menu menu;

        private static void MenuItem_Click(object sender, EventArgs e)
        {
            var targetItem = sender as MenuItem;
            try
            {
                switch (targetItem.Tag)
                {
                    case "Quit":
                        Quit();
                        return;
                    case "ReLoad":
                        PluginManagment.Instance.ReLoad();
                        return;
                    case "LogForm":
                        LogForm.Instance.Show();
                        return;
                    case "Displaywindow":
                        targetItem.Checked = !targetItem.Checked;
                        FloatWindow.Instance.ShowFlag = targetItem.Checked;
                        return;
                    case "TopMost":
                        targetItem.Checked = !targetItem.Checked;
                        FloatWindow.Instance.TopMostFlag = targetItem.Checked;
                        return;
                }
                KeyValuePair<string, string> pair = (KeyValuePair<string, string>)(sender as MenuItem).Tag;
                CQPlugin c = PluginManagment.Instance.Plugins.Find(x => x.appinfo.Name == pair.Key);//从已加载的插件寻找这个名称的插件
                if (c.Enable is false)
                {
                    MessageBox.Show("插件未启用, 请打开菜单之前启用此插件", "无法调用菜单", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string menuname = string.Empty;
                foreach (var item in JArray.Parse(JObject.Parse(c.json)["menu"].ToString()))//遍历此插件的json的menu节点,寻找窗口函数
                {
                    if (item["name"].ToString() == pair.Value)
                    { menuname = item["function"].ToString(); break; }
                }
                menu = (Type_Menu)c.dll.Invoke(menuname, typeof(Type_Menu));//将函数转换委托
                menu();//调用
            }
            catch (Exception exc)
            {
                MessageBox.Show($"拉起菜单发生错误，错误信息:{exc.Message}\n{exc.StackTrace}", "菜单错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void ShowNotifyIcon()
        {
            Instance.Visible = true;
        }
        public static void HideNotifyIcon()
        {
            Instance.Visible = false;
        }
        public static void RemoveMenu(string pluginName)
        {
            var item = Instance.ContextMenu.MenuItems.Find(pluginName, true).First();
            Instance.ContextMenu.MenuItems[2].MenuItems.Remove(item);
        }
        public static void ClearAppMenu()
        {
            Instance.ContextMenu.MenuItems[2].MenuItems.Clear();
        }
        public static void Quit()
        {
            PluginManagment.Instance.CallFunction(FunctionEnums.Disable);
            PluginManagment.Instance.CallFunction(FunctionEnums.Exit);
            PluginManagment.Instance.UnLoad();
            HideNotifyIcon();
            Environment.Exit(0);
        }
        public static void AddManageMenu()
        {
            MenuItem menu = Instance.ContextMenu.MenuItems.Find("PluginMenu", false).First();
            menu.MenuItems.Add("-");
            MenuItem manage = new MenuItem { Text = "插件管理", Tag = "PluginManage" };
            manage.Click += Manage_Click;
            menu.MenuItems.Add(manage);
        }
        [STAThread]
        private static void Manage_Click(object sender, EventArgs e)
        {
            PluginForm form = new();
            form.Show();
        }
        public static void ReStart()
        {
            HideNotifyIcon();
            Application.Restart();
        }
    }
}
