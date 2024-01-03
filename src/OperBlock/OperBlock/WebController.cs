using System.Diagnostics;
using System.Reflection;
using System.Threading;
using nanoFramework.Json;
using nanoFramework.WebServer;
using OperBlock.Web;

namespace OperBlock
{
    public class WebController
    {
        private static WebServer _webServer = new WebServer(80, HttpProtocol.Http);

        private static MethodInfo[]? _methods;

        public static void Init()
        {
            try
            {
                _webServer = new WebServer(80, HttpProtocol.Http);
                if (WifiController.IsConnected)
                {
                    _webServer.Start();
                    _webServer.CommandReceived += WebServerOnCommandReceived;
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Test {ex.Message}");
            }

            WifiController.Connected += OnWifiControllerConnected;
            WifiController.Disconnected += OnWifiControllerDisconnected;
        }

        private static void OnWifiControllerConnected()
        {
            _webServer.Start();
            _webServer.CommandReceived += WebServerOnCommandReceived;
        }

        private static void OnWifiControllerDisconnected()
        {
            _webServer.Stop();
            _webServer.CommandReceived -= WebServerOnCommandReceived;
        }


        private static void WebServerOnCommandReceived(object obj, WebServerEventArgs e)
        {
            var rawUrl = e.Context.Request.RawUrl.TrimStart('/');

            var a = rawUrl.Split('/');
            if (a.Length < 1)
            {
                return;
            }

            var targetMethodName = a[0];

            if (_methods == null)
            {
                _methods = typeof(ControllerOper).GetMethods();
            }

            foreach (var method in _methods)
            {
                if (rawUrl.StartsWith(targetMethodName))
                {
                    var args = GetArguments(a);

                    var parameters = method.GetParameters();
                    if (parameters.Length < args.Length)
                    {
                        continue;
                    }

                    method.Invoke(null, args);
                    break;
                }
            }

            Debug.WriteLine(e.Context.Request.RawUrl);
        }

        private static string[] GetArguments(string[] args)
        {
            var result = new string[args.Length - 1];

            for (int i = 1; i < args.Length; i++)
            {
                result[i - 1] = args[i];
            }

            return result;
        }

    }
}