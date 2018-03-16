//---------------------------------------------------------------
// Date: 2/19/2018
// Rights: 
// FileName: CdproViewModelBase.cs
//---------------------------------------------------------------

namespace CDP.UWP.Models
{
    using System;
    using Template10.Mvvm;
    using Windows.UI.Popups;

    /// <summary>
    /// Base implementation of a view model
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public abstract class CdproViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Throws the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        protected void Throw(Exception ex)
        {
            Throw(ex, ex.Message);
        }

        /// <summary>
        /// Throws the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        protected void Throw(Exception ex, string message)
        {
            ShowMessage(ex.Message);
            App.Logger.LogException(ex);
            throw ex;
        }

        /// <summary>
        /// Alerts the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected void Alert(string message)
        {
            ShowMessage(message);
            App.Logger.LogTrace(message);
        }



        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        private void ShowMessage(string message, string title = "Error")
        {
            this.Dispatcher.DispatchAsync(async () => 
            {
                var messageDialog = new MessageDialog(message)
                {
                    Title = title
                };
                await messageDialog.ShowAsync();
            });
        }
    }
}
