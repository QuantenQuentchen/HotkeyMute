using KeyboardUtils;
using System.Windows.Input;

namespace Ahhhhhhhhh_Jesus_Christus_warum_steht_da_nicht_legacy
{
    internal class HotkeyManager
    {
        private static int? scId = null;
        public static void RegisterHotkey(List<Key> HKList, Action onHotkey)
        {
            if (scId != null)
            {
                GlobalKeyboardHook.Instance.UnHook(scId.Value);
            }
            //List<Key> HKList = new() { Key.LeftCtrl, Key.M };

            scId = GlobalKeyboardHook.Instance.Hook(
            HKList,
            () => onHotkey.Invoke(), out var msg
            );
        }

    }
}
