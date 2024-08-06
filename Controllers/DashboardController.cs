using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Ouroboros.Models;

namespace Ouroboros.Controllers;

public class DashboardController : Controller
{
	private async Task<bool> NodeIsOwned(int id)
	{
		var username = AuthedUser.FromCtx(HttpContext)?.HeadscaleName;
		if (username == null) return false;
		
		var nodes = await Headscale.NodesList();
		var node  = nodes.FirstOrDefault(n => n.Id == id);
		return node?.User?.Name == username;
	}
	
	public async Task<IActionResult> Index()
	{ 
		var user = AuthedUser.FromCtx(HttpContext);
		if (user == null)
			return Redirect("/ouroboros/auth");
		
		var nodes = await Headscale.NodesList();

		var yourNodes  = nodes.Where(n => n.User?.Name == user.HeadscaleName);
		var otherNodes = nodes.Where(n => n.User?.Name != user.HeadscaleName);

		return View(new DashboardModel(user, yourNodes, otherNodes));
	}

	[HttpPost]
	public async Task<IActionResult> DeleteNode(int id)
	{
		if (!await NodeIsOwned(id)) return Unauthorized();
        
		var res = await Headscale.NodeDelete(id);
		// TODO: test!
		return res
				   ? Redirect("/ouroboros/dashboard")
				   : StatusCode(500, "500: Could not remove node.");
	}
	
	[HttpPost]
	public async Task<IActionResult> ExpireNode(int id)
	{
		if (!await NodeIsOwned(id)) return Unauthorized();
		
		await Headscale.NodeExpire(id);
		return Redirect("/ouroboros/dashboard");
	}
	
	[HttpPost]
	public async Task<IActionResult> RenameNode(int id, string name)
	{
		if (!await NodeIsOwned(id)) return Unauthorized();
		
		await Headscale.NodeRename(id, name);
		return Redirect("/ouroboros/dashboard");
	}
}