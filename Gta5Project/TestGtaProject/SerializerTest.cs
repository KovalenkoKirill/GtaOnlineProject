using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataContact.Authorization;
using DataContact;
using GtaServer;

namespace TestGtaProject
{
    [TestClass]
    public class SerializerTest
    {
        [TestMethod]
        public void Serialize()
        {
            string login = "dev";
            string password = "dev";
            AuthorizationRequest request = new AuthorizationRequest()
            {
                Name = login,
                Password = password,
                GameVersion = ((GameVersion)(byte)0)
            };
            StandardPackage<AuthorizationRequest> requestPack = new StandardPackage<AuthorizationRequest>(request, null);
            byte[] bytes = requestPack.Serialize();
        }
        [TestMethod]
        public void Desirialize()
        {
            string login = "dev";
            string password = "dev";
            AuthorizationRequest request = new AuthorizationRequest()
            {
                Name = login,
                Password = password,
                GameVersion = ((GameVersion)(byte)0)
            };
            StandardPackage<AuthorizationRequest> requestPack = new StandardPackage<AuthorizationRequest>(request, null);
            byte [] bytes = requestPack.Serialize();
            StandardPackage<AuthorizationRequest> requesetPack2 = StandardPackage<AuthorizationRequest>.GetStandardPackage(bytes);
            ServerPlayer player = new ServerPlayer();
            Client client = new Client(null, player);
            StandardPackage<AuthorizationResponse> response = new StandardPackage<AuthorizationResponse>(
                new AuthorizationResponse()
                {
                    Session = "",
                    Success = false,
                    Error = "wrong username or password",
                    Player = new Player()
                    {
                        DisplayName = "dev",
                        Name = "dev",
                        Health = 100
                    }
                }, null);
            byte[] bytesResponse = response.Serialize();
            StandardPackage<AuthorizationResponse> response2 = StandardPackage<AuthorizationResponse>.GetStandardPackage(bytesResponse);
        }

    }
}
