using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Ouroboros.Models;

namespace Ouroboros.Controllers;

public class DashboardController : Controller
{
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
		if (AuthedUser.FromCtx(HttpContext) == null) return Unauthorized();
        
		var res = await Headscale.NodeDelete(id);
		// TODO: test!
		return res
				   ? RedirectToAction("Index")
				   : StatusCode(500, "500: Could not remove node.");
	}
	
	[HttpPost]
	public async Task<IActionResult> ExpireNode(int id)
	{
		if (AuthedUser.FromCtx(HttpContext) == null) return Unauthorized();
		
		var res = await Headscale.NodeExpire(id);
		// TODO: wrong!
		return res
				   ? RedirectToAction("Index")
				   : StatusCode(500, "500: Could not expire node.");
	}
	
	[HttpPost]
	public async Task<IActionResult> RenameNode(int id, string name)
	{
		if (AuthedUser.FromCtx(HttpContext) == null) return Unauthorized();
		
		await Headscale.NodeRename(id, name);
		return RedirectToAction("Index");
	}
}