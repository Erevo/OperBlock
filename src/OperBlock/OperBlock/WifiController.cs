using System.Device.Wifi;
using nanoFramework.Networking;

namespace OperBlock
{
    public static class WifiController
    {
        public static void Init()
        {
            //success = WifiNetworkHelper.ConnectDhcp(MySsid, MyPassword, requiresDateTime: true, token: cs.Token);
            var success = WifiNetworkHelper.Reconnect();
        }
    }
}