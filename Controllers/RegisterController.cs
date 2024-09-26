using Microsoft.AspNetCore.Mvc;
using Ouroboros.Models;

namespace Ouroboros.Controllers;

public class RegisterController : Controller
{
	[Route("/register/{mKey}")]
	[HttpGet]
	public IActionResult Index(string? mKey)
	{
		if (mKey == null || !mKey.StartsWith("mkey:"))
			return BadRequest("not a valid mkey.");
		
		var trimmedNk = mKey["mkey:".Length..];
		
		var user = AuthedUser.FromCtx(HttpContext);
		if (user == null)
			return Redirect($"/ouroboros/auth?mkey={trimmedNk}");

		return View(new RegisterIndexModel(user, trimmedNk));
	}

	[HttpPost]
	public async Task<IActionResult> Add(string? mKey)
	{
		if (mKey == null) return BadRequest();
		var user = AuthedUser.FromCtx(HttpContext);
		if (user == null) return Unauthorized();

		await Headscale.NodeRegister(user.HeadscaleName, "mkey:" + mKey);
		return Redirect("/ouroboros/dashboard");
	}
}