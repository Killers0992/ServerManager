using CommandSystem;
using Exiled.API.Features;
using ServerManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerManager.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class LinkServerCommand : ICommand
    {
        public string Command { get; } = "linkserver";

        public string[] Aliases { get; } = new string[0];

        public string Description { get; } = "Link your server.";

        public string LinkServer(LinkServerModel model)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{MainClass.singleton.Config.ApiEndpoint}/servermanager/linkserver");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(Encoding.UTF8.GetString(Utf8Json.JsonSerializer.Serialize(model)));
            }
            var webResponse = request.GetResponse();
            var webStream = webResponse.GetResponseStream();
            var responseReader = new StreamReader(webStream);
            var response = responseReader.ReadToEnd();
            responseReader.Close();
            return response;
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Syntax: linkserver <token>";
                return false;
            }

            response = $"Response: " + LinkServer(new LinkServerModel()
            {
                ServerPort = Server.Port,
                Token = Guid.Parse(arguments.At(0))
            });
            return true;
        }
    }
}
