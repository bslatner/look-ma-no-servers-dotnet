using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelloWorldAspNet.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }

        public string Greeting { get; set; }

        public IActionResult OnGet()
        {
            Name = "";
            Greeting = "";
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Greeting = "";
                return Page();
            }

            Greeting = $"Hello, {Name}!";

            return Page();
        }
    }
}
