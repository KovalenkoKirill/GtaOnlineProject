using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GtaServer;
using TestGtaProject.forServer;
using TestConfiguration = TestGtaProject.forServer.TestConfiguration;
using System.Threading;

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
