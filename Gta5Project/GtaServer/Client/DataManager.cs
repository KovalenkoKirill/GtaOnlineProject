using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GtaServer
{
    /// <summary>
    /// Класс для сохранения состояния игроков.
    /// Пока сохраняет в файл json. В дальнейшем будет прикручена база
    /// </summary>
    public class DataManager : IDisposable
    {
        const string Player_File_Name = @"Player.json";

        const int Interval = 1000;

        public List<ServerPlayer> Players = new List<ServerPlayer>();

        volatile bool isWork = false;

        Thread syncThread;

        public DataManager()
        {
            if(!File.Exists(Player_File_Name))
            {
                File.Create(Player_File_Name);
            }
        }

        public ServerPlayer GetServerPlayer(string login)
        {
            return Players.Where(x => x.Login == login).SingleOrDefault();
        }

        public ServerPlayer GetDefault(string Default)
        {
            ServerPlayer player = Players.Where(x => x.Login == Default).SingleOrDefault();
            string serializeDefault = JsonConvert.SerializeObject(player);
            return JsonConvert.DeserializeObject<ServerPlayer>(serializeDefault);
        }

        public void Dispose()
        {
            Stop();
            
            try
            {
                syncThread?.Abort();
            }
            catch { }
        }

        private void SyncThread()
        {
            while(isWork)
            {
                try
                {
                    foreach (var client in Server.Instanse.clients)
                    {
                        ServerPlayer serverClient = Players.Where(x => x.Login == client.Player.Login)
                            .SingleOrDefault();
                        if (serverClient == null)
                        {
                            Players.Add(client.Player);
                        }
                    }
                    WriteFIle(JsonConvert.SerializeObject(Players));
                }catch(Exception ex)
                {
                    Server.Instanse.Logger.Exception("DataManager.SyncThread", ex);
                }
                Thread.Sleep(Interval);
            }
        }

        public void Stop()
        {
            isWork = false;
            try
            {
                syncThread?.Abort();
            }
            catch { }
        }

        public void Start()
        {
            isWork = true;
            try
            {
                Players = JsonConvert.DeserializeObject<ServerPlayer[]>(ReadFile()).ToList();
                if (Players == null) Players = new List<ServerPlayer>();
                this.syncThread = new Thread(SyncThread);
                syncThread.Start();
            }
            catch(Exception ex)
            {
                Server.Instanse.Logger.Exception("DataManager.Start", ex);
            }
        }

        void WriteFIle(string content)
        {
            using (StreamWriter writer = new StreamWriter(Player_File_Name))
            {
                writer.Write(content);
            }
        }
        string ReadFile()
        {
            using (StreamReader reader = new StreamReader(Player_File_Name))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
