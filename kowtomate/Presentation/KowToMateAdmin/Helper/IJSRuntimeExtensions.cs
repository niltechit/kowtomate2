using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace KowToMateAdmin.Helper
{
	public static class IJSRuntimeExtensions
	{
        public static ValueTask DisplayMessage(this IJSRuntime js, string message)
        {
            return js.InvokeVoidAsync("Swal.fire", message);
        }

        public static ValueTask SuccesMessage(this IJSRuntime js, string title, string message, SweetAlertTypeMessagee type)
        {
            return js.InvokeVoidAsync("Swal.fire", title, message, type.ToString());
        }

        public static ValueTask FailMessage(this IJSRuntime js, string title, string message, SweetAlertTypeMessagee type)
        {
            return js.InvokeVoidAsync("Swal.fire", title, message, type.ToString());
        }

        public static ValueTask<bool> Confirm(this IJSRuntime js, string title, string message, SweetAlertTypeMessagee type)
        {
            return js.InvokeAsync<bool>("CustomCofirm", title, message, type.ToString());
        }
        public static ValueTask<bool> DeleteConfirm(this IJSRuntime js, string title, string message, SweetAlertTypeMessagee type)
        {
            return js.InvokeAsync<bool>("DeleteConfirmation", title, message, type.ToString());
        } 
        public static ValueTask<bool> Confirmation(this IJSRuntime js, string title, string message, SweetAlertTypeMessagee type)
        {
            return js.InvokeAsync<bool>("CustomCofirmation", title, message, type.ToString());
        }
        public static ValueTask<bool> ReplaceConfirmation(this IJSRuntime js, string title, string message, SweetAlertTypeMessagee type)
        {
            return js.InvokeAsync<bool>("ReplaceConfirmation", title, message, type.ToString());
        } 
        public static ValueTask<bool> DeleteConfirmation(this IJSRuntime js, string title, string message, SweetAlertTypeMessagee type)
        {
            return js.InvokeAsync<bool>("Remove", title, message, type.ToString());
        }
        public static ValueTask<bool> YesConfirmation(this IJSRuntime js, string title, string message, SweetAlertTypeMessagee type)
        {
            return js.InvokeAsync<bool>("YesConfirmation", title, message, type.ToString());
        }
    }
    public enum SweetAlertTypeMessagee
    {
        warning, error, success, info, question
    }
}
