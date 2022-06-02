using Prism.Services.Dialogs;
using System;

namespace MainModule.ViewModels.Dialogs
{
    public static class IDialogServiceExtensions
    {
        public static void ShowMessageDialog(this IDialogService dialogService, string message, Action<IDialogResult> callback)
        {
            var p = new DialogParameters();
            p.Add("message", message);

            dialogService.ShowDialog("MessageDialog", p, callback);
        }
    }
}
