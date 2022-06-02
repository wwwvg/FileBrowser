using Prism.Services.Dialogs;
using System;

namespace MainModule.ViewModels.Dialogs
{
    public static class IDialogServiceExtensions
    {
        public static void ShowDeleteDialog(this IDialogService dialogService, string message, Action<IDialogResult> callback)
        {
            var p = new DialogParameters();
            p.Add("message", message);

            dialogService.ShowDialog("DeleteDialog", p, callback);
        }

        public static void ShowAddFolderDialog(this IDialogService dialogService, string message, Action<IDialogResult> callback)
        {
            var p = new DialogParameters();
            p.Add("message", message);

            dialogService.ShowDialog("AddFolderDialog", p, callback);
        }
    }
}
