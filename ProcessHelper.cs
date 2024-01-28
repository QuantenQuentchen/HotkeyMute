using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ahhhhhhhhh_Jesus_Christus_warum_steht_da_nicht_legacy
{
    internal class ProcessHelper
    {
        //DLL Invokeing and Shit
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);


        /// <summary>
        /// A utility class to determine a process parent.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ParentProcessUtilities
        {
            // These members must match PROCESS_BASIC_INFORMATION
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved2_0;
            internal IntPtr Reserved2_1;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;
            /// <summary>
            /// Gets the parent process of the current process.
            /// </summary>
            /// <returns>An instance of the Process class.</returns>
            public static Process? GetParentProcess()
            {
                return GetParentProcess(Process.GetCurrentProcess().Handle);
            }

            /// <summary>
            /// Gets the parent process of specified process.
            /// </summary>
            /// <param name="id">The process id.</param>
            /// <returns>An instance of the Process class.</returns>
            public static Process? GetParentProcess(int id)
            {
                try
                {
                    Process process = Process.GetProcessById(id);
                    return GetParentProcess(process.Handle);
                }
                catch { return null; }
            }

            /// <summary>
            /// Gets the parent process of a specified process.
            /// </summary>
            /// <param name="handle">The process handle.</param>
            /// <returns>An instance of the Process class.</returns>
            public static Process? GetParentProcess(IntPtr handle)
            {
                ParentProcessUtilities pbi = new();
                int returnLength;
                int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
                if (status != 0)
                    throw new Win32Exception(status);

                try
                {
                    return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
                }
                catch (ArgumentException)
                {
                    // not found
                    return null;
                }
            }
        }

        public static uint GetProcessByWindowHandle(IntPtr handle)
        {
            _ = GetWindowThreadProcessId(handle, out uint pid);
            return pid;
        }

        public static uint? GetParentId(UInt32 id)
        {
            Process? processParent = ParentProcessUtilities.GetParentProcess((Int32)id);
            if (processParent != null)
            {
                return (uint)processParent.Id;
            }
            else
            {
                return null;
            }

        }
        // The GetWindowThreadProcessId function retrieves the identifier of the thread
        // that created the specified window and, optionally, the identifier of the
        // process that created the window.

        // Returns the name of the process owning the foreground window.
        public static Process? GetForegroundProcess()
        {
            //This shit is impossibly Slow
            IntPtr hwnd = GetForegroundWindow();

            // The foreground window can be NULL in certain circumstances, 
            // such as when a window is losing activation.
            // This should never be an Issue, possibly, maybe, hopefully.
            // Try replacing it with hwnd.zero() or something soon...
            /*
            if (hwnd == null)
              return null;
            */

            _ = GetWindowThreadProcessId(hwnd, out uint pid);
            //Hold up, why am I searching for the Process here? I just need the ID, right?

            foreach (Process p in Process.GetProcesses())
            {
                if (p.Id == pid)
                    return p;
            }
            //That should be way faster...
            try
            {
                return Process.GetProcessById((int)pid);
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }

        public static IntPtr? GetWindowHandle(UInt32 PID)
        {
            Process proc;
            try
            {
                proc = Process.GetProcessById((int)PID);
                proc.Refresh();
                return proc.MainWindowHandle;
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }

        public static uint? GetForegroundProcessId()
        {
            IntPtr? hwnd = GetForegroundWindow();

            // The foreground window can be NULL in certain circumstances, 
            // such as when a window is losing activation.
            // This should never be an Issue, possibly, maybe, hopefully.
            // Try replacing it with hwnd.zero() or something soon...

            if (hwnd == null)
            {
                return null;
            }


            _ = GetWindowThreadProcessId(hwnd!.Value, out uint pid);
            return pid;
        }
        public static Process? FastForegroundProcess()
        {
            if (GetForegroundProcessId() is uint pid)
            {
                try
                {
                    return Process.GetProcessById((int)pid);
                }
                catch (System.ArgumentException)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
