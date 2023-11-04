using System;

namespace GTA
{
    internal class Events
    {
        public static Action<object, EventArgs> PlayerEnterVehicle { get; internal set; }

        internal class PlayerEnterVehicleEventArgs
        {
        }
    }
}