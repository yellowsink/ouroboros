using Microsoft.AspNetCore.Mvc;
using Ouroboros.Models;

namespace Ouroboros.Controllers;

public class RegisterController : Controller
{
	[Route("/register/{nodeKey}")]
	[HttpGet]
	public IActionResult Index(string? nodeKey)
	{
		if (nodeKey == null || !nodeKey.StartsWith("nodekey:"))
			return BadRequest("not a valid nodekey.");
		
		var trimmedNk = nodeKey[8..];
		
		var user = AuthedUser.FromCtx(HttpContext);
		if (user == null)
			return RedirectToAction("Index", "Auth", new { nodeKey = trimmedNk });

		return View(new RegisterIndexModel(user, trimmedNk));
	}

	[HttpPost]
	public async Task<IActionResult> Add(string? nodeKey)
	{
		if (nodeKey == null) return BadRequest();
		var user = AuthedUser.FromCtx(HttpContext);
		if (user == null) return Unauthorized();

		await Headscale.NodeRegister(user.HeadscaleName, "nodekey:" + nodeKey);
		return RedirectToAction("Index", "Dashboard");
	}
}