using System;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Threading.Tasks;

namespace ColonyConcierge.Mobile.Customer
{
    public class AlertManager
    {
        public AlertManager()
        {
        }

        /// <summary>
        /// Shows the full screen spinner with title, default title is "Loading.."
        /// </summary>
        /// <param name="title">Title.</param>
        static public void ShowFullScreenSpinner(String title = "Loading..")
        {
            UserDialogs.Instance.ShowLoading(title, MaskType.Gradient);
        }

        /// <summary>
        /// Hides the full screen spinner.
        /// </summary>
        static public void HideFullScreenSpinner()
        {
            UserDialogs.Instance.HideLoading();
        }

        /// <summary>
        /// Displaies the alert.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="cancel">Cancel.</param>
        static public void ShowAlert(String title, String message, String cancel, Page contentPage)
        {
            Device.BeginInvokeOnMainThread(() => {
                contentPage.DisplayAlert(title, message, cancel ?? "Ok");
            });
        }

        /// <summary>
        /// Displaies the error alert.
        /// </summary>
        /// <param name="message">Message.</param>
        static public void ShowErrorAlert(String message, Page contentPage, String title = "Error")
        {
            AlertManager.ShowAlert(title, message, null, contentPage);
        }

        /// <summary>
        /// Displaies the warning alert.
        /// </summary>
        /// <param name="message">Message.</param>
        static public void ShowWarningAlert(String message, ContentPage contentPage)
        {
            AlertManager.ShowAlert("Warning", message, null, contentPage);
        }

        /// <summary>
        /// Shows the alert for aggregate exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <param name="contentPage">Content page.</param>
        static public void ShowAlertForAggregateException(AggregateException exception, Page contentPage, String title = null)
        {
            String errorsInformation = "";
            foreach (var ex in exception.InnerExceptions)
            {
                if (errorsInformation.Length != 0)
                {
                    errorsInformation += "; ";
                }
                errorsInformation += ex.Message;
            }
            AlertManager.ShowAlert(title ?? "Error", errorsInformation, "Ok", contentPage);
        }

        static public Task<string> Prompt(string message, Boolean isPassword = false)
        {
            var tcs = new TaskCompletionSource<string>();

            var promptConfig = new PromptConfig();

            if (isPassword)
            {
                promptConfig.InputType = InputType.Password;
            }

            promptConfig.Message = message;
            promptConfig.OnAction += (obj) =>
            {
                var result = obj as PromptResult;
                if (result.Ok)
                {
                    tcs.SetResult((obj as PromptResult).Text);
                }
            };
            UserDialogs.Instance.Prompt(promptConfig);

            return tcs.Task;
        }
    }
}
