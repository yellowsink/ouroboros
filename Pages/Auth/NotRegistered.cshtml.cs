using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ouroboros.Pages.Auth;

public class NotRegisteredModel : PageModel
{
	public string? Login;
	
	public void OnGet(string login)
	{
		Login = login;
	}
}