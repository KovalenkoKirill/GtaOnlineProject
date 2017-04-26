using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GtaServer;
using TestGtaProject.forServer;
using TestConfiguration = TestGtaProject.forServer.TestConfiguration;
using System.Threading;
using System.Threading.Tasks;
using DataContact;

namespace TestGtaProject
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void LoadServer()
        {
            TestConfiguration configuration = new TestConfiguration();
            Server server = new Server(configuration);
            server.Start();
            Assert.AreEqual(server.State, ServerState.Started);
        }

        [TestMethod]
        public void ClientCreate()
        {
            Client client = new Client(null, null);
            if (string.IsNullOrEmpty(client.Session)) throw new Exception("Сессия клиента пуста");
        }
        [TestMethod]
        public void DistanseCheck()
        {
            LVector3 vector1 = new LVector3(-10, 0, 0);
            LVector3 vector2 = new LVector3(10, 0, 0);
            float dist = Distanse(vector1, vector2);
        }

        public float Distanse(LVector3 vector1, LVector3 vector2)
        {
            return  (float)Math.Sqrt(Math.Pow((vector1.X - vector2.X),2) +
                Math.Pow((vector1.Y - vector2.Y),2) 
                + Math.Pow((vector1.Z - vector2.Z),2));
        }


        [TestMethod]
        public void ClientTest()
        {
            TestConfiguration configuration = new TestConfiguration();;
            for (int i = 0; i < 100; i++)
            {
                int port = i + 5050;
                Task.Factory.StartNew(() => {
                    FakeClient client = new FakeClient(new ClientConfiguration()
                    {
                        ServerAdress = "127.0.0.1",
                        ServerPort = configuration.serverPort
                    }, configuration.logger);
                    client.clientPort = port;
                    client.Connect();
                    client.Autorization("dev", "dev");
                });
            }
            Thread.Sleep(20000);
        }
        [TestMethod]
        public void ClientConnection()
        {
            TestConfiguration configuration = new TestConfiguration();
            Server server = new Server(configuration);
            server.Start();

            FakeClient client = new FakeClient(new ClientConfiguration()
            {
                ServerAdress = "127.0.0.1",
                ServerPort = configuration.serverPort,
            },configuration.logger);
            client.Connect();
            client.Autorization("dev", "dev");
            Thread.Sleep(20000);
        }
    }
}
