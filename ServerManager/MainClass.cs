using Exiled.API.Features;
using MEC;
using ServerManager.Enums;
using ServerManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utf8Json.Resolvers;

namespace ServerManager
{
    public class MainClass : Plugin<PluginConfig>
    {
        public override string Author { get; } = "Killers0992";
        public override string Name { get; } = "ServerManager";
        public override string Prefix { get; } = "servermanager";
        public override Version RequiredExiledVersion { get; } = new Version(3, 0, 0);
        public override Version Version { get; } = new Version(1, 0, 1);

        public CoroutineHandle StatusUpdater;

        public static MainClass singleton;

        public override void OnEnabled()
        {
            singleton = this;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await UpdateServer(Config.ApiEndpoint, Config.Debug, new UpdateServerModel()
                    {
                        ExiledVersion = Exiled.Loader.Loader.Version.ToString(3),
                        OnlinePlayers = 0,
                        MaxPlayers = 25,
                        SLVersion = GameCore.Version.VersionString,
                        State = ServerState.Starting
                    });
                }
                catch(Exception ex)
                {
                    Log.Error(ex.ToString());
                }

            });

            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += Server_RoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += Server_RoundEnded;
            Exiled.Events.Handlers.Server.RestartingRound += Server_RestartingRound;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= Server_RoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= Server_RoundEnded;
            Exiled.Events.Handlers.Server.RestartingRound -= Server_RestartingRound;
            base.OnDisabled();
        }

        private void Server_WaitingForPlayers()
        {
            Task.Factory.StartNew(async () =>
            {
                await UpdateServer(Config.ApiEndpoint, Config.Debug, new UpdateServerModel()
                {
                    State = ServerState.WaitingForPlayers,
                    MaxPlayers = CustomNetworkManager.slots
                });
            });
            StatusUpdater = Timing.RunCoroutine(UpdateServerStatus());
        }

        private void Server_RoundStarted()
        {
            Task.Factory.StartNew(async () =>
            {
                await UpdateServer(Config.ApiEndpoint, Config.Debug, new UpdateServerModel()
                {
                    State = ServerState.RoundInProgress

                });
            });
        }

        private void Server_RoundEnded(Exiled.Events.EventArgs.RoundEndedEventArgs ev)
        {
            Task.Factory.StartNew(async () =>
            {
                await UpdateServer(Config.ApiEndpoint, Config.Debug, new UpdateServerModel()
                {
                    State = ServerState.RoundEnded

                });
            });
        }

        private void Server_RestartingRound()
        {
            Task.Factory.StartNew(async () =>
            {
                await UpdateServer(Config.ApiEndpoint, Config.Debug, new UpdateServerModel()
                {
                    State = ServerState.Restarting,
                    OnlinePlayers = 0
                });
            });
            if (StatusUpdater != null)
                Timing.KillCoroutines(StatusUpdater);
        }

        public static async Task<bool> UpdateServer(string endpoint, bool debug, UpdateServerModel model)
        {
            using (HttpClient client = new HttpClient())
            {
                var httpContent = new StringContent(Encoding.UTF8.GetString(Utf8Json.JsonSerializer.Serialize(model, Utf8JsonConfiguration.Resolver)), Encoding.UTF8, "application/json");

                var webRequest = await client.PostAsync($"{endpoint}/servermanager/updateserver", httpContent);

                if (!webRequest.IsSuccessStatusCode)
                {
                    Log.Error($"[UpdateServer] Web API connection error. " + webRequest.StatusCode + " - " + await webRequest.Content.ReadAsStringAsync());
                    return false;
                }

                string apiResponse = await webRequest.Content.ReadAsStringAsync();

                Log.Debug($"[UpdateServer] API Returned: {apiResponse}", debug);
                return true;
            }
        }

        public IEnumerator<float> UpdateServerStatus()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(5f);
                try
                {
                    Task.Factory.StartNew(async () =>
                    {
                        await UpdateServer(Config.ApiEndpoint, Config.Debug, new UpdateServerModel()
                        {
                            OnlinePlayers = Player.List.Count(),
                        });
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }
    }
}
