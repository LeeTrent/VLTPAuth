using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VLTPAuth.Pages
{
    public class IndexModel : PageModel
    {
        // public void OnGet()
        // {

        // }

        public IActionResult OnGet() 
        { 
          return RedirectToPage("/Account/Login", new { area = "Identity" }); 
        } 

        // public async Task<IActionResult> OnGetAsync()
        // {
        //   // https://stackoverflow.com/questions/51552561/razor-pages-web-app-net-core-2-1-how-to-handle-redirection-from-one-area-pa
        //   //https://www.learnrazorpages.com/advanced/areas#routing-and-url-generation 
           
        //     string url = Url.Page("/Account/Register", new { area = "Identity" });
        //     return RedirectToPage(url);
        // }
    }
}
