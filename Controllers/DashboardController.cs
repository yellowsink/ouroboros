using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Ouroboros.Models;

namespace Ouroboros.Controllers;

public class DashboardController : Controller
{
	public async Task<IActionResult> Index()
	{ 
		var userSer = HttpContext.Session.Get("user");
		if (userSer == null)
			return Redirect("/ouroboros/auth");

		var user = JsonSerializer.Deserialize<AuthedUser>(userSer);
		if (user == null)
			return Redirect("/ouroboros/auth");
		
		var nodes = await Headscale.NodesList();

		var yourNodes  = nodes.Where(n => n.User?.Name == user.HeadscaleName);
		var otherNodes = nodes.Where(n => n.User?.Name != user.HeadscaleName);

		return View(new DashboardModel(user, yourNodes, otherNodes));
	}
}