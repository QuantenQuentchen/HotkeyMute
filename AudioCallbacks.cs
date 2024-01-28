using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Diagnostics;

namespace Ahhhhhhhhh_Jesus_Christus_warum_steht_da_nicht_legacy
{
    internal class AudioCallbacks
    {
        private static readonly Dictionary<UInt32, HashSet<NAudioEventCallbacks>> Callbacks = new();

        public static void ProcessEventRegister(Process process)
        {
            try
            {
                process.EnableRaisingEvents = true;
                process.Exited += AudioProcessExitedCallback;
            }
            catch
            {
                Debug.WriteLine($"Permission Error in {process}");
            }

        }

        public static void ProcessEventUnregister(Process process)
        {
            try
            {
                process.EnableRaisingEvents = false;
                process.Exited -= AudioProcessExitedCallback;
            }
            catch
            {
                Debug.WriteLine($"Permission Error in {process}");
            }
        }

        public static void EventUnregister(UInt32 Pid)
        {
            if (Callbacks.ContainsKey(Pid))
            {
                HashSet<AudioSessionControl>? Sessions = LookUpTableV3.Get(Pid);
                if (Sessions == null) { return; }
                foreach (var session in Sessions)
                {
                    foreach (var callback in Callbacks[Pid])
                    {
                        session.UnRegisterEventClient(callback);
                    }
                }
                Callbacks.Remove(Pid);
            }
        }

        public static void EventRegisterBulk()
        {
            foreach (var Device in DeviceUtils.Devices)
            {
                Device.AudioSessionManager.OnSessionCreated += SessionCreated;
                var sessionManager2 = Device.AudioSessionManager;
                var sessionEnumerator = sessionManager2.Sessions;
                for (int i = 0; i < Device.AudioSessionManager.Sessions.Count; i++)
                {
                    var session = sessionEnumerator[i];
                    var callback = new NAudioEventCallbacks(session.GetProcessID);
                    if (Callbacks.TryGetValue(session.GetProcessID, out HashSet<NAudioEventCallbacks>? outHashSet))
                    {
                        outHashSet.Add(callback);
                    }
                    else
                    {
                        HashSet<NAudioEventCallbacks>? callbacks = new()
                        {
                            callback
                        };
                        Callbacks.Add(session.GetProcessID, callbacks);
                    }
                    session.RegisterEventClient(callback);
                }
            }
            foreach (var Pid in LookUpTableV3.LookUp.Keys)
            {
                try
                {
                    Process? Proc = Process.GetProcessById((Int32)Pid);
                    if (Proc == null) { continue; }
                    ProcessEventRegister(Proc);
                }
                catch
                {
                    Debug.WriteLine($"Permission Error in {Pid}");
                    continue;
                }

            }
            Debug.WriteLine("EventRegistered");
        }

        public static void EventUnregisterBulk()
        {
            foreach (var Device in DeviceUtils.Devices)
            {
                Device.AudioSessionManager.OnSessionCreated -= SessionCreated;
                var sessionManager2 = Device.AudioSessionManager;
                var sessionEnumerator = sessionManager2.Sessions;
                for (int i = 0; i < Device.AudioSessionManager.Sessions.Count; i++)
                {
                    var session = sessionEnumerator[i];
                    var sessionPID = session.GetProcessID;
                    if (Callbacks.ContainsKey(sessionPID))
                    {
                        foreach (var callback in Callbacks[sessionPID])
                        {
                            session.UnRegisterEventClient(callback);
                        }
                        Callbacks.Remove(sessionPID);
                    }
                }
            }
        }

        private static void SessionCreated(object sender, IAudioSessionControl NewSession)
        {
            Debug.WriteLine($"SessionCreated:{NewSession}");
            AudioSessionControl audioSessionControl = new AudioSessionControl(NewSession);
            LookUpTableV3.Add(audioSessionControl.GetProcessID, audioSessionControl);
            Debug.WriteLine($"Session Typcasted:{audioSessionControl}");
        }


        //Callback Classes:

        //Now this is fucking frustrating thx Bill
        private static void AudioProcessExitedCallback(object sender, EventArgs e)
        {
            Process? proc = sender as Process;
            UInt32? PId = (UInt32)proc.Id;
        }

        private class NAudioEventCallbacks : IAudioSessionEventsHandler
        {
            private readonly uint Pid;
            public NAudioEventCallbacks(UInt32 Pid)
            {
                this.Pid = Pid;
            }

            public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex) { }

            public void OnDisplayNameChanged(string displayName) { }

            public void OnGroupingParamChanged(ref Guid groupingId) { }

            public void OnIconPathChanged(string iconPath) { }

            public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
            {
                Debug.WriteLine($"Session Disconnected:{disconnectReason}, {this.Pid}");
                try
                {
                    //LookUpTable.LookUp.Remove(this.Pid);
                }
                catch
                {
                    Debug.WriteLine("Error removing from LookUp");
                }
            }

            public void OnStateChanged(AudioSessionState state)
            {
                Debug.WriteLine($"Session State Changed:{state}, {this.Pid}");
            }

            public void OnVolumeChanged(float volume, bool isMuted)
            {
                Debug.WriteLine(this.Pid);
            }
        }

    }
}
