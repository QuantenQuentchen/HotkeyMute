using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace Ahhhhhhhhh_Jesus_Christus_warum_steht_da_nicht_legacy
{
    internal class LookUpTableV3
    {
        //LookUp Declaration
        public static Dictionary<UInt32, HashSet<AudioSessionControl>> LookUp = new();

        public static void Add(UInt32 PID, AudioSessionControl sessionControl)
        {
            //Basic Appending based on PID
            if (LookUp.TryGetValue(PID, out HashSet<AudioSessionControl>? outHashSet))
            {
                outHashSet.Add(sessionControl);
                Debug.WriteLine($"Added{sessionControl.GetSessionIdentifier} to LookUp with pID {PID}");
            }
            else
            {
                LookUp.Add(PID, new HashSet<AudioSessionControl> { sessionControl });
                Debug.WriteLine($"Added{sessionControl.GetSessionIdentifier} to LookUp with pID {PID} as new");
            }
            //Adding References for PPID
            UInt32? PPID = ProcessHelper.GetParentId(PID);
            if (PPID != null)
            {
                if (LookUp.TryAdd(PPID!.Value, LookUp[PID]))
                {
                    Debug.WriteLine($"PPID: {PPID}, pointing to {PID}");
                }
            }
        }

        public static void Remove(UInt32 PID)
        {
            //Basic Removal based on PID
            LookUp.Remove(PID);
            //Removing References for PPID
            UInt32? PPID = ProcessHelper.GetParentId(PID);
            if (PPID != null)
            {
                LookUp.Remove(PPID!.Value);
            }
        }

        public static HashSet<AudioSessionControl>? Get(UInt32 PPID)
        {
            if (LookUp.TryGetValue(PPID, out HashSet<AudioSessionControl>? sessions))
            {
                return sessions;
            }
            else
            {
                ProcessHelper.GetParentId(PPID);
                if (LookUp.TryGetValue(PPID, out sessions))
                {
                    return sessions;
                }
                else
                {
                    return null;
                }
            }
        }

        public static void PrintEntries()
        {
            foreach (var pair in LookUp)
            {
                Debug.WriteLine($"{pair.Key} : [");
                foreach (var session in pair.Value)
                {
                    Debug.WriteLine($"    {session.GetProcessID} : {session.GetSessionIdentifier}, {session.DisplayName}");
                }
                Debug.WriteLine("]");
            }
        }

        public static void Populate()
        {
            var DeviceEnum = new MMDeviceEnumerator();
            var Devices = DeviceEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            foreach (var Device in Devices)
            {
                var sessionManager2 = Device.AudioSessionManager;
                var sessionEnumerator = sessionManager2.Sessions;
                for (int i = 0; i < sessionEnumerator.Count; i++)
                {
                    //Retrieve Audio Session
                    AudioSessionControl sessionControl = sessionEnumerator[i];

                    //Retrieve Associated PID
                    UInt32 ProcID = (UInt32)sessionEnumerator[i].GetProcessID;
                    //Use All Encompassing Add Method
                    Add(ProcID, sessionControl);
                }
            }
        }
    }
}
