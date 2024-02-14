using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ouroboros.Pages;

public class DashboardModel : PageModel
{
	public AuthedUser AuthedUser = null!; // safety: if page is rendered, user was set in ongetasync
	
	public IEnumerable<Headscale.HeadscaleNode> YourNodes = [];
	public IEnumerable<Headscale.HeadscaleNode> OtherNodes = [];
    
	public async Task<IActionResult> OnGetAsync()
	{
		var userSer = HttpContext.Session.Get("user");
		if (userSer == null)
			return Redirect("/ouroboros/auth");

		AuthedUser = JsonSerializer.Deserialize<AuthedUser>(userSer)!;
		if (AuthedUser == null!)
			return Redirect("/ouroboros/auth");
		
		var nodes = await Headscale.NodesList();

		YourNodes  = nodes.Where(n => n.User?.Name == AuthedUser.HeadscaleName);
		OtherNodes = nodes.Where(n => n.User?.Name != AuthedUser.HeadscaleName);

		return Page();
	}
}