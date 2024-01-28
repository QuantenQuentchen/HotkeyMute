using KeyboardUtils;
using System.Windows.Input;

namespace Ahhhhhhhhh_Jesus_Christus_warum_steht_da_nicht_legacy
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 

        [STAThread]
        static void Main()
        {
            //Application.SetCompatibleTextRenderingDefault(false);
            //ApplicationConfiguration.Initialize();

            LookUpTableV3.Populate();
            //AudioCallbacks.EventRegisterBulk();
            List<Key> HotKeyList = new() { Key.LeftCtrl, Key.M };
            HotkeyManager.RegisterHotkey(HotKeyList, SoundControll.SoundController);

            Application.Run(new Form1());
        }
    }
}