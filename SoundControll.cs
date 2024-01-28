using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace Ahhhhhhhhh_Jesus_Christus_warum_steht_da_nicht_legacy
{
    internal class SoundControll
    {
        public static void SoundController()
        {
            UInt32? WinProcessId = ProcessHelper.GetForegroundProcessId();
            if (WinProcessId == null)
            {
                new Thread(() => ToastUtil.ShowToastNotification("No Process was found")).Start();
                return;
            }
            Process? WinProcess = Process.GetProcessById((int)WinProcessId);
            IntPtr? handle = ProcessHelper.GetWindowHandle((uint)WinProcessId);
            IntPtr? winHandle = ProcessHelper.GetForegroundWindow();
            Debug.WriteLine($"WinProcessId:{WinProcessId}, WinProcess:{WinProcess}\n HWND:{winHandle}, OverlayHWND:{handle}");
            String ToastStr;
            Debug.WriteLine($"WinProcessId:{WinProcessId}, WinProcess:{WinProcess}");
            if (WinProcess == null)
            {
                return;
            }
            bool? MuteState = MuteProcess(WinProcess);
            if (MuteState == null)
            {
                return;
            }
            if (MuteState!.Value)
            {
                ToastStr = $"Process:{WinProcess.ProcessName} was successfully Muted";
            }
            else
            {
                ToastStr = $"Process:{WinProcess.ProcessName} was successfully Unmuted";
            }
            new Thread(() => ToastUtil.ShowToastNotification(ToastStr)).Start();
        }

        private static bool? MuteProcess(Process process)
        {
            bool MuteState;
            UInt32? pID = (uint)process.Id;
            if (!pID.HasValue)
            {
                return null;
            }
            HashSet<AudioSessionControl>? windowSoundControllers = LookUpTableV3.Get(pID!.Value);
            if (windowSoundControllers == null)
            {
                return null;
            }

            var winSoundControllersEnumerator = windowSoundControllers.GetEnumerator();

            //Probably quite slow. Needs to be optimized. Or Refactored. Or something.
            winSoundControllersEnumerator.MoveNext();
            MuteState = winSoundControllersEnumerator.Current.SimpleAudioVolume.Mute;
            Debug.WriteLine($"PiD:{process.Id}, SoundControllers?:{windowSoundControllers}");
            foreach (var session in windowSoundControllers)
            {
                Debug.WriteLine($"{session}");
                Debug.WriteLine($"PiD:{process.Id}, SoundController:{session.DisplayName}");
                session.SimpleAudioVolume.Mute = MuteState != true;
            }
            return MuteState != true;
        }

    }
}
