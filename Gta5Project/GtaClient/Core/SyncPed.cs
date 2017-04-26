using DataContact;
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient
{
    public class SyncPed
    {
        const float hRange = 200f;

        public DateTime _lastUpdate { get; private set; }

        public DateTime _lastUpdatePedInfoTime = DateTime.Now;

        public PedInfo PedInfo
        {
            get
            {
                return lastPedInfo;
            }
            set
            {
                NewPedInfo = value;
                _lastUpdatePedInfoTime = DateTime.Now;
            }
        }


        private int _relGroup;

        public uint PedId { get; private set; }

        PedInfo lastPedInfo;

        PedInfo NewPedInfo;

        Ped Character;

        public bool PedIsInit = false;

        public SyncPed(PedInfo ped)
        {
            this.PedId = ped.PedId;
            this.NewPedInfo = ped;
            this.lastPedInfo = ped;
        }
        
        public void Remove()
        {
            if(Character!= null)
            {
                Character.Delete();
            }
        }


        public void Sync()
        {
            if (lastPedInfo == NewPedInfo) return;
            if (!PedIsInit)
            {
                _relGroup = World.AddRelationshipGroup("SYNCPED");
                World.SetRelationshipBetweenGroups(Relationship.Neutral, _relGroup, Game.Player.Character.RelationshipGroup);
                World.SetRelationshipBetweenGroups(Relationship.Neutral, Game.Player.Character.RelationshipGroup, _relGroup);

                if (Character == null || !Character.Exists() || !Character.IsInRangeOf(NewPedInfo.Position.ToVector(), hRange) || Character.Model.Hash != NewPedInfo.PedModelHash || (Character.IsDead && NewPedInfo.Health > 0))
                {
                    if (Character != null) Character.Delete();

                    Character = World.CreatePed(new Model((int)NewPedInfo.PedModelHash), NewPedInfo.Position.ToVector(), NewPedInfo.Quaternion.Z);
                    if (Character == null) return;

                    Character.BlockPermanentEvents = true;
                    Character.IsInvincible = true;
                    Character.CanRagdoll = false;
                    Character.RelationshipGroup = _relGroup;
                    if (NewPedInfo.flags == PedFlags.IsPlayer)
                    {
                        Character.AddBlip();
                        if (Character.CurrentBlip == null) return;
                        Character.CurrentBlip.Color = BlipColor.White;
                        Character.CurrentBlip.Scale = 0.8f;
                        SetBlipNameFromTextFile(Character.CurrentBlip, "Player");
                    }
                }
                PedIsInit = true;
            }
            if (Character.Weapons.Current.Hash != (WeaponHash)NewPedInfo.WeaponHash)
            {
                var wep = Character.Weapons.Give((WeaponHash)NewPedInfo.WeaponHash, 9999, true, true);
                Character.Weapons.Select(wep);
            }

            if (!lastPedInfo.flags.HasFlag(PedFlags.IsJumping) && NewPedInfo.flags.HasFlag(PedFlags.IsJumping))
            {
                Character.Task.Jump();
            }
            var dest = NewPedInfo.Position.ToVector();
            Character.FreezePosition = false;

            const int threshold = 50;
            if (NewPedInfo.flags != PedFlags.IsJumping && NewPedInfo.flags != PedFlags.IsAiming && NewPedInfo.flags != PedFlags.IsShooting)
            {
                float distance = World.GetDistance(Character.Position, dest);
                if(distance>0.5f)
                {
                    Character.Task.RunTo(dest, true, 10);
                }
                else
                {
                    Character.PositionNoOffset = dest;
                    Character.Quaternion = this.NewPedInfo.Quaternion.ToQuaternion();
                }
                //if (!Character.IsInRangeOf(dest, 0.5f))
                //{
                //    Character.Task.RunTo(dest, true, 10);
                //}
                //if (!Character.IsInRangeOf(dest, 200f))
                //{
                //    Character.PositionNoOffset = dest - new Vector3(0, 0, 1f);
                //    Character.Quaternion = this.NewPedInfo.Quaternion.ToQuaternion();
                //}

            }
            this.lastPedInfo = NewPedInfo;
            _lastUpdate = DateTime.Now;
        }
        public void SetBlipNameFromTextFile(Blip blip, string text)
        {
            Function.Call(Hash._0xF9113A30DE5C6670, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._0xBC38B49BCB83BC9B, blip);
        }
    }
}
