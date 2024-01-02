﻿using System.Diagnostics;
using nanoFramework.Json;
using nanoFramework.WebServer;
using OperBlock.Web;

namespace OperBlock
{
    public class WebController
    {


        public static void Init()
        {
            using var webServer = new WebServer(80, HttpProtocol.Http);

            webServer.Start();
            webServer.CommandReceived += WebServerOnCommandReceived;
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

            var methods = typeof(ControllerOper).GetMethods();

            foreach (var method in methods)
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