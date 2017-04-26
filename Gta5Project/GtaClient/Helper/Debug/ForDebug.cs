using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;

namespace GtaClient
{
#if DEBUG
    public class ForDebug
    {
        bool init = false;
        public ForDebug()
        {
            MainClient.Instance.Logger.Info("ForDebug enter");
            GameTaskFactory.Factory.Invoke(() =>
            {
                try
                {
                    Ped character = GTA.Game.Player.Character;
                    Game.Player.ChangeModel(new Model(PedHash.Migrant01SMM));
                    MainClient.Instance.Logger.Info($"x:{character.Position.X} y:{character.Position.Y} z: {character.Position.Z}");
                }
                catch (Exception ex)
                {
                    MainClient.Instance.Logger.Exception("ForDebuf", ex);
                }
            });

        }
    }
#endif
}
