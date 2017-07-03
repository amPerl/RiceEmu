using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rice.Game;
using Rice.Server.Core;

namespace Rice
{
    public class Admin
    {
        private static HttpListener listener;

        public static void Initialize(Config config)
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8088/");
            listener.Start();

            listener.BeginGetContext(onGetContext, null);
        }

        private static void onGetContext(IAsyncResult result)
        {
            var ctx = listener.EndGetContext(result);
            switch (ctx.Request.RawUrl)
            {
                case "/status":
                case "/status/":
                    onStatus(ctx);
                    break;

                case "/favicon.ico":
                    break;

                default:
                    Log.WriteLine("Unhandled JSON API request: {0}", ctx.Request.RawUrl);
                    writeJsonResp(new { status = "error", error = "Invalid API call." }, ctx);
                    break;
            }
            listener.BeginGetContext(onGetContext, null);
        }

        private static void writeJsonResp(object obj, HttpListenerContext ctx)
        {
            var resp = ctx.Response;
            string str = JsonConvert.SerializeObject(obj);
            byte[] strBytes = Encoding.UTF8.GetBytes(str);

            resp.ContentLength64 = strBytes.Length;
            resp.OutputStream.Write(strBytes, 0, strBytes.Length);
            resp.OutputStream.Close();
        }

        private static void onStatus(HttpListenerContext ctx)
        {
            dynamic status = new ExpandoObject();
            status.status = "ok";
            status.playercount = RiceServer.GetPlayers().Length;

            var servers = new [] {RiceServer.Auth, RiceServer.Lobby, RiceServer.Game, RiceServer.Area, RiceServer.Ranking};
            status.servers = servers
                .Select(server => new
                {
                    name = server.Name,
                    clientcount = server.GetClients().Length
                });

            status.areas = RiceServer.GetAreas()
                .Where(a => a.GetPlayerCount() > 0)
                .Select(area => new
                {
                    id = area.ID,
                    playercount = area.GetPlayerCount(),
                    players = area.GetPlayers()
                        .Select(p => new
                        {
                            name = p.ActiveCharacter.Name,
                            level = p.ActiveCharacter.Level
                        })
                });

            writeJsonResp(status, ctx);
        }
    }
}
