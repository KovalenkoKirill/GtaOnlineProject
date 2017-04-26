using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact
{
    [Flags]
    public enum PedFlags : long
    {
        IsInVehicle,
        isAfk,
        IsPlayer,
        IsJumping,
        IsShooting,
        IsAiming,
        IsParachuteOpen
    }
}
