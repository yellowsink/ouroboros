using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ouroboros.Pages.Auth;

public class IndexModel : PageModel
{
	public string ReturnTo = null!; // safety: definitely assigned in OnGet
	public string ReturnToPretty = null!;
	
	public void OnGet(string? nodeKey)
	{
		if (nodeKey == null)
		{
			// to dashboard
			ReturnTo       = Convert.ToBase64String("/ouroboros/dashboard"u8);
			ReturnToPretty = "to access the dashboard";
		}
		else
		{
			ReturnTo       = Convert.ToBase64String(Encoding.UTF8.GetBytes($"/register/nodekey:{nodeKey}"));
			ReturnToPretty = "to add a node";
		}
	}
}