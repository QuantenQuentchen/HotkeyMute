using Microsoft.Toolkit.Uwp.Notifications;

namespace Ahhhhhhhhh_Jesus_Christus_warum_steht_da_nicht_legacy
{
    internal class ToastUtil
    {
        public static void ShowToastNotification(string message)
        {
            new ToastContentBuilder()
                .AddText(message)
                .Show();
        }
    }
}
