using NAudio.CoreAudioApi;

namespace Ahhhhhhhhh_Jesus_Christus_warum_steht_da_nicht_legacy
{
    internal class DeviceUtils
    {
        //This entire class becomes worth it once someone figures out how to switch an AudioSessionControl to a different AudioEndpoint.
        public static MMDeviceEnumerator DeviceEnum = new();//Devices Mutable after Initialization (e.g. Headset plugged in) Registering Change Callbacks just to update the Dictionary is probably not worth it atm may cut a few ms MAY.
        public static MMDeviceCollection Devices = DeviceEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active); //Also Mutable after Initialization and negligible performance impact called ONCE.
        public static Dictionary<Guid, MMDevice> DeviceDict = new(); //Never used. But I'm keeping it here just in case I finally can switch AudioEndpoints.
    }
}
