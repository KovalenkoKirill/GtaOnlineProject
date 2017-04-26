using DataContact;
using GTA;
using GTA.Math;
using GTA.Native;
using GtaClient;
using GtaClient.Core;
using GtaServer.DataContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if TEST
using MainClient = TestGtaProject.FakeClient;
#else
using MainClient = GtaClient.MainClient;
#endif

namespace GtaClient
{
    public class PlayerSynchronization: IHandler,IDisposable
    {
        public const int UpdateInterval = 10;

        public Thread SyncThread;

        MainClient client;

        bool IsWork = true;

        LocalPed currentPlayer;

        public List<OtherServerClients> otherClient = new List<OtherServerClients>();

        public PlayerSynchronization(MainClient client)
        {
            this.client = client;
            PedInfo playerServer = client.Player.PedInfo;
            GameTaskFactory.Factory.Invoke(() => {
                Ped player = Game.Player.Character;
                
                player.Weapons.Give((WeaponHash)playerServer.WeaponHash, 999, true, true);
                player.Quaternion = playerServer.Quaternion.ToQuaternion();
                player.PositionNoOffset = playerServer.Position.ToVector();
                Game.Player.ChangeModel(new Model((int)playerServer.PedModelHash));
                currentPlayer = new LocalPed(player, true);
            });
            SyncThread = new Thread(SyncTask);
            SyncThread.Start();
        }

        public void Dispose()
        {
            try
            {
                
                IsWork = false;
                foreach(var otheClient in this.otherClient)
                {
                    otheClient.RemoveAllPeds();
                }
                this.SyncThread.Abort();
            }
            catch { }
        }

        void SyncTask()
        {
            DateTime lastUpdate = DateTime.Now.AddSeconds(-1);
            MainClient.Instance.Logger.Info("PlayerSynchronization thread start");
            while (IsWork)
            {
                if(DateTime.Now < lastUpdate )
                {
                    Thread.Sleep(1);
                    continue;
                }
                try
                {
                    List<PedInfo> currentNpcPeds;
                    GameTaskFactory.Factory.Invoke(() => {
                        currentNpcPeds = GetLocalNpcPeds();
                        this.currentPlayer.Update(Game.Player.Character);
                        foreach(var client in otherClient)
                        {
                            client.RenderAll();
                            
                        }
                    });
                    foreach (var client in otherClient)
                    {
                        client.CreateResponse<PedInfo>(currentPlayer.info, PacketType.pedInfo);
                        foreach (var npc in this.localPeds)
                        {
                            client.CreateResponse<PedInfo>(npc.info, PacketType.pedInfo);
                        }
                    }
                    MainClient.Instance.SendPackage(currentPlayer.info, Lidgren.Network.NetDeliveryMethod.Unreliable);
                    lastUpdate = DateTime.Now.AddMilliseconds(UpdateInterval);
                }
                catch (Exception ex)
                {
                    MainClient.Instance.Logger.Exception("PlayerSynchronization", ex);
                }
                
            }
        }


        public Task HandlePackage<T>(StandardPackage<T> package)
        {
            OtherServerClients client = this.otherClient.Where(x => x.Session == package.Session).SingleOrDefault();
            if(client != null)
            {
                client.AddPedInfo(package.data as PedInfo);
            }
            return null;
        }

        

        private class LocalPed
        {

            public int pedHandle { get; private set; }

            public PedInfo info { get; set; }

            public bool isPlayer = false;

            public LocalPed(Ped gamePed,bool isPlayer)
            {
                this.pedHandle = gamePed.Handle;
                
                this.isPlayer = isPlayer;
                info = new PedInfo();
                info.PedId = (uint)new Random().Next(0, int.MaxValue);
            }

            public void Update(Ped ped)
            {
                bool aiming = Game.IsControlPressed(0, GTA.Control.Aim);
                bool shooting = Function.Call<bool>(Hash.IS_PED_SHOOTING, ped.Handle);

                Vector3 aimCoord = new Vector3();
                if (aiming || shooting)
                    aimCoord = ScreenRelToWorld(GameplayCamera.Position, GameplayCamera.Rotation,
                        new Vector2(0, 0));

                info.Health = ped.Health;
                info.Position = ped.Position.ToLVector();
                info.AimCoords = aimCoord.ToLVector();
                info.Quaternion = ped.Quaternion.ToLQuaternion();
                info.PedModelHash = (uint)ped.Model.Hash;
                info.WeaponHash = (uint)ped.Weapons.Current.Hash;
                info.flags = aiming ? info.flags | PedFlags.IsAiming : info.flags | ~PedFlags.IsAiming;
                info.flags = shooting ? info.flags | PedFlags.IsShooting : info.flags | ~PedFlags.IsShooting;
                info.flags = Function.Call<bool>(Hash.IS_PED_JUMPING, ped.Handle) ?
                info.flags | PedFlags.IsJumping :
                info.flags | ~PedFlags.IsJumping;
                info.flags = Function.Call<int>(Hash.GET_PED_PARACHUTE_STATE, Game.Player.Character.Handle) == 2 ?
                info.flags | PedFlags.IsParachuteOpen : info.flags | ~PedFlags.IsParachuteOpen;
                if (isPlayer)
                {
                    info.flags |= PedFlags.IsPlayer;
                }
                else
                {
                    info.flags |= ~PedFlags.IsPlayer;
                }
            }
        }

        private List<LocalPed> localPeds = new List<LocalPed>();

        private List<PedInfo> GetLocalNpcPeds()
        {
            Ped[] peds = World.GetAllPeds();
            int syncGroup = World.AddRelationshipGroup("SYNCPED");

            List<PedInfo> pedsInfo = new List<PedInfo>();

            List<LocalPed> curentLocalPeds = new List<LocalPed>();

            int PlayerId = Game.Player.Character.Handle;

            foreach (Ped ped in peds)
            {
                if (ped.Handle == PlayerId) continue;
                if (syncGroup == ped.RelationshipGroup) continue;
                LocalPed local = localPeds.Where(x => x.pedHandle == ped.Handle).SingleOrDefault();
                if(local == null)
                {
                    local = new LocalPed(ped, false);
                }
                local.Update(ped);
                pedsInfo.Add(local.info);
                curentLocalPeds.Add(local);
            }
            this.localPeds = curentLocalPeds;
            return pedsInfo;
            
        }

        #region Math

        public static bool WorldToScreenRel(Vector3 worldCoords, out Vector2 screenCoords)
        {
            var num1 = new OutputArgument();
            var num2 = new OutputArgument();

            if (!Function.Call<bool>(Hash._WORLD3D_TO_SCREEN2D, worldCoords.X, worldCoords.Y, worldCoords.Z, num1, num2))
            {
                screenCoords = new Vector2();
                return false;
            }
            screenCoords = new Vector2((num1.GetResult<float>() - 0.5f) * 2, (num2.GetResult<float>() - 0.5f) * 2);
            return true;
        }

        public static Vector3 ScreenRelToWorld(Vector3 camPos, Vector3 camRot, Vector2 coord)
        {
            var camForward = RotationToDirection(camRot);
            var rotUp = camRot + new Vector3(10, 0, 0);
            var rotDown = camRot + new Vector3(-10, 0, 0);
            var rotLeft = camRot + new Vector3(0, 0, -10);
            var rotRight = camRot + new Vector3(0, 0, 10);

            var camRight = RotationToDirection(rotRight) - RotationToDirection(rotLeft);
            var camUp = RotationToDirection(rotUp) - RotationToDirection(rotDown);

            var rollRad = -DegToRad(camRot.Y);

            var camRightRoll = camRight * (float)Math.Cos(rollRad) - camUp * (float)Math.Sin(rollRad);
            var camUpRoll = camRight * (float)Math.Sin(rollRad) + camUp * (float)Math.Cos(rollRad);

            var point3D = camPos + camForward * 10.0f + camRightRoll + camUpRoll;
            Vector2 point2D;
            if (!WorldToScreenRel(point3D, out point2D)) return camPos + camForward * 10.0f;
            var point3DZero = camPos + camForward * 10.0f;
            Vector2 point2DZero;
            if (!WorldToScreenRel(point3DZero, out point2DZero)) return camPos + camForward * 10.0f;

            const double eps = 0.001;
            if (Math.Abs(point2D.X - point2DZero.X) < eps || Math.Abs(point2D.Y - point2DZero.Y) < eps) return camPos + camForward * 10.0f;
            var scaleX = (coord.X - point2DZero.X) / (point2D.X - point2DZero.X);
            var scaleY = (coord.Y - point2DZero.Y) / (point2D.Y - point2DZero.Y);
            var point3Dret = camPos + camForward * 10.0f + camRightRoll * scaleX + camUpRoll * scaleY;
            return point3Dret;
        }

        public static Vector3 RotationToDirection(Vector3 rotation)
        {
            var z = DegToRad(rotation.Z);
            var x = DegToRad(rotation.X);
            var num = Math.Abs(Math.Cos(x));
            return new Vector3
            {
                X = (float)(-Math.Sin(z) * num),
                Y = (float)(Math.Cos(z) * num),
                Z = (float)Math.Sin(x)
            };
        }

        public static Vector3 DirectionToRotation(Vector3 direction)
        {
            direction.Normalize();

            var x = Math.Atan2(direction.Z, direction.Y);
            var y = 0;
            var z = -Math.Atan2(direction.X, direction.Y);

            return new Vector3
            {
                X = (float)RadToDeg(x),
                Y = (float)RadToDeg(y),
                Z = (float)RadToDeg(z)
            };
        }

        public static double DegToRad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        public static double RadToDeg(double deg)
        {
            return deg * 180.0 / Math.PI;
        }

        public static double BoundRotationDeg(double angleDeg)
        {
            var twoPi = (int)(angleDeg / 360);
            var res = angleDeg - twoPi * 360;
            if (res < 0) res += 360;
            return res;
        }

        public static Vector3 RaycastEverything(Vector2 screenCoord)
        {
            var camPos = GameplayCamera.Position;
            var camRot = GameplayCamera.Rotation;
            const float raycastToDist = 100.0f;
            const float raycastFromDist = 1f;

            var target3D = ScreenRelToWorld(camPos, camRot, screenCoord);
            var source3D = camPos;

            Entity ignoreEntity = Game.Player.Character;
            if (Game.Player.Character.IsInVehicle())
            {
                ignoreEntity = Game.Player.Character.CurrentVehicle;
            }

            var dir = (target3D - source3D);
            dir.Normalize();
            var raycastResults = World.Raycast(source3D + dir * raycastFromDist,
                source3D + dir * raycastToDist,
                (IntersectOptions)(1 | 16 | 256 | 2 | 4 | 8)// | peds + vehicles
                , ignoreEntity);

            if (raycastResults.DitHitAnything)
            {
                return raycastResults.HitCoords;
            }

            return camPos + dir * raycastToDist;
        }
        #endregion
    }
}
