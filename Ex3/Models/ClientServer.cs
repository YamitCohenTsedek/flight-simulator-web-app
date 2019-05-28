using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ex3.Models
{
    public class ClientServer
    {
        using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulator.Model
    {
        public class CommandClient : IDisposable
        {
            TcpClient client;

            private CommandClient()
            {
            }

            public void Start()
            {
                int simulatorPort = ApplicationSettingsModel.Instance.FlightCommandPort;
                string simulatorIp = ApplicationSettingsModel.Instance.FlightServerIP;
                client = new TcpClient(simulatorIp, simulatorPort);
            }

            private static CommandClient instance = null;
            public static CommandClient Instance
            {
                get
                {
                    if (instance == null)
                        instance = new CommandClient();
                    return instance;
                }
            }

            public void Send(string command)
            {
                if (client != null)
                {
                    StreamWriter writer = new StreamWriter(client.GetStream());
                    writer.WriteLine(command);
                    writer.Flush();
                }

            }

            public void Dispose()
            {
                client.Close();
            }
        }
    }

}
}