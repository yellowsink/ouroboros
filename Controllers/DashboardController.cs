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
	
	private async Task<bool> RouteIsOwned(uint id)
	{
		var routes = await Headscale.RoutesList();
		var route  = routes.FirstOrDefault(n => n.Id == id);
		return await NodeIsOwned(route?.Node?.Id ?? -1);
	}
	
	public async Task<IActionResult> Index()
	{ 
		var user = AuthedUser.FromCtx(HttpContext);
		if (user == null)
			return Redirect("/ouroboros/auth");
		
		var nodesT = Headscale.NodesList();
		var routes = await Headscale.RoutesList();
		var nodes  = await nodesT;

		var yourNodes  = nodes.Where(n => n.User?.Name == user.HeadscaleName).ToArray();
		var otherNodes = nodes.Where(n => n.User?.Name != user.HeadscaleName).ToArray();

		var yourRoutes  = routes.Where(r => yourNodes.Any(n => r.Node?.Id == n.Id)).ToArray();
		var otherRoutes = routes.Where(r => yourNodes.All(n => r.Node?.Id != n.Id)).ToArray();

		var yourExitNodes = FindExitNodes(yourNodes,  yourRoutes);
		var otherExitNodes = FindExitNodes(otherNodes, otherRoutes);
		
		var yourOtherRoutes = yourRoutes.Where(
			r => !yourExitNodes.Any(n => n.Value.Item1.Id == r.Id || n.Value.Item2.Id == r.Id));
		
		var otherOtherRoutes = otherRoutes.Where(
			r => !otherExitNodes.Any(n => n.Value.Item1.Id == r.Id || n.Value.Item2.Id == r.Id));

		return View(
			new DashboardModel(
				user,
				yourNodes,
				otherNodes,
				yourExitNodes,
				otherExitNodes,
				yourOtherRoutes,
				otherOtherRoutes));

		Dictionary<int, (Headscale.HeadscaleRoute, Headscale.HeadscaleRoute)> FindExitNodes(
			IEnumerable<Headscale.HeadscaleNode> nodes, IReadOnlyList<Headscale.HeadscaleRoute> routes)
		{
			var target = new Dictionary<int, (Headscale.HeadscaleRoute, Headscale.HeadscaleRoute)>();

			foreach (var node in nodes)
			{
				// find matching routes
				// double enumeration? honestly, bleh. its fine.
				var r4 = routes.FirstOrDefault(r => r.Node?.Id == node.Id && r.Prefix == "0.0.0.0/0");
				var r6 = routes.FirstOrDefault(r => r.Node?.Id == node.Id && r.Prefix == "::/0");

				if (r4?.Advertised == true && r6?.Advertised == true)
					target[node.Id] = (r4, r6);
			}

			return target;
		}
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
	
	[HttpPost]
	public async Task<IActionResult> EnableRoute(uint id)
	{
		if (!await RouteIsOwned(id)) return Unauthorized();
		
		await Headscale.RouteEnable(id);
		return Redirect("/ouroboros/dashboard");
	}
	
	[HttpPost]
	public async Task<IActionResult> DisableRoute(uint id)
	{
		if (!await RouteIsOwned(id)) return Unauthorized();
		
		await Headscale.RouteDisable(id);
		return Redirect("/ouroboros/dashboard");
	}
	
	[HttpPost]
	public async Task<IActionResult> EnableENode(uint id4, uint id6)
	{
		if (!await RouteIsOwned(id4)) return Unauthorized();
		if (!await RouteIsOwned(id6)) return Unauthorized();
		
		// technically only need to set one, but lets be safe
		await Headscale.RouteEnable(id4);
		await Headscale.RouteEnable(id6);
		return Redirect("/ouroboros/dashboard");
	}
	
	[HttpPost]
	public async Task<IActionResult> DisableENode(uint id4, uint id6)
	{
		if (!await RouteIsOwned(id4)) return Unauthorized();
		if (!await RouteIsOwned(id6)) return Unauthorized();
		
		// technically only need to set one, but lets be safe
		await Headscale.RouteDisable(id4);
		await Headscale.RouteDisable(id6);
		return Redirect("/ouroboros/dashboard");
	}
	
	[HttpPost]
	public async Task<IActionResult> DeleteRoute(uint id)
	{
		if (!await RouteIsOwned(id)) return Unauthorized();
		
		await Headscale.RouteDelete(id);
		return Redirect("/ouroboros/dashboard");
	}
}