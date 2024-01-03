using System;
using System.Device;
using System.Device.Wifi;
using System.Diagnostics;
using System.Net;
using System.Threading;
using nanoFramework.Networking;

namespace OperBlock
{
    public static class WifiController
    {
        public static bool IsConnected { get; private set; }

        public static event Action? Connected;
        public static event Action? Disconnected;
        public static void Init()
        {
            new Thread(() =>
            {
                while (true)
                {
                    var ip = IPAddress.GetDefaultLocalAddress();
                    //Debug.WriteLine($"Wifi: {WifiNetworkHelper.Status} {ip}");

                    var isConnected = ip != IPAddress.Any;

                    if (IsConnected == false && isConnected == true)
                    {
                        Connected?.Invoke();
                    }
                    else if (IsConnected == true && isConnected == false)
                    {
                        Disconnected?.Invoke();
                    }

                    IsConnected = isConnected;

                    DelayHelper.DelayMilliseconds(100, true);
                }
            }).Start();

            bool success;
            CancellationTokenSource cs = new(15000);

            //success = WifiNetworkHelper.Reconnect();
            WifiNetworkHelper.Disconnect();
            success = WifiNetworkHelper.ConnectDhcp("OperBlock", "caravan.", token: cs.Token);

            if (success)
            {
                Connected?.Invoke();
                IsConnected = true;
            }
            else
            {
                Debug.WriteLine($"Can't get a proper IP address and DateTime, error: {WifiNetworkHelper.Status}.");
                if (WifiNetworkHelper.HelperException != null)
                {
                    Debug.WriteLine($"Exception: {WifiNetworkHelper.HelperException}");
                }

                return;
            }
        }
    }
}