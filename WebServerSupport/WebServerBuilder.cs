using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.UI.WebControls;
using RouteParameter = System.Web.Http.RouteParameter;

namespace WebServerSupport
{
    public class WebServerBuilder
    {
        public static WebServerBuilder Instance { get; set; } = new WebServerBuilder();
        public WebServerBuilder()
        {
            Instance = this;
        }
        public static void Main()
        {
            Instance.BuildServer(Instance.Port);
        }
        public IDisposable WebServer { get; set; }
        public int Port { get; set; } = 3080;
        public string BasePath { get; set; } = "";
        public bool PortConflict { get; set; } = false;
        public delegate void ListeningPortChangedHandler(int port);
        public event ListeningPortChangedHandler ListeningPortChanged;
        public void BuildServer(int port)
        {
            if (WebServer != null)
            {
                WebServer.Dispose();
                Console.WriteLine("Webserver Rebuilt.");
            }
            Port = port;
            try
            {
                WebServer = WebApp.Start<Startup>($"http://127.0.0.1:{port}");
                Console.WriteLine($"Running a http server on port {port}");
            }
            catch (System.Reflection.TargetInvocationException)
            {
                PortConflict = true;
                if (port - Port > 15000)
                {
                    throw new Exception("不能建立本地Web服务器");
                }
                if (port > 65530)
                {
                    throw new Exception("端口数值溢出，建议重新修改端口起始值后重启程序");
                }
                BuildServer(port + 1);
                return;
            }
            if (PortConflict)
            {
                ListeningPortChanged?.Invoke(port);
            }
            // Console.ReadLine();
        }
    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            app.UseWebApi(config);
        }
    }
    // test: 
    // ui => image upload => websocket saveImg => convert to local img url => build cqcode => send to plugin
    // plugin => download pic by url => handle
    // rich text:
    // if contain image, load image first, websocket return img placeholder(url), ui save img as placeholder
    // ui send text, amn parse text and img, build cqcode
    // web:
    // ui => image upload => websocket saveImg
    // amn => load img => base64 => build messagechains => send to mirai
    public class ImageController : ApiController
    {
        public object Get(string id)
        {
            string path = Path.Combine(WebServerBuilder.Instance.BasePath, id);
            if (File.Exists(path))
            {
                return new { content = File.ReadAllText(path) };
            }
            else
            {
                return new { content = "empty" };
            }
        }
    }
}
