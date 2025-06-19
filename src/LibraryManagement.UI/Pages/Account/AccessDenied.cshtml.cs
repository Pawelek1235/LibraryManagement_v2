using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        public void OnGet(string returnUrl = null)
        {
            // mo¿esz odczytaæ returnUrl, jeœli chcesz pokazaæ je w widoku
            // ViewData["ReturnUrl"] = returnUrl;
        }
    }
}
