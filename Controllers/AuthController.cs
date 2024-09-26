using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Ouroboros.Models;

namespace Ouroboros.Controllers;

public class AuthController : Controller
{
	private HttpClient _http = new()
	{
		DefaultRequestHeaders =
		{
			{ "Accept", "application/json" },
			{ "User-Agent", "Ouroboros/1.0.0" }
		}
	};

	public async Task<IActionResult> Callback(string code, string state)
	{
		var user = await GetUser(await GetAccessToken(code));

		if (!Config.C.user_map.ContainsKey(user.id.ToString()))
			return View("NotRegistered", new AuthNotRegisteredModel(user.login));
		
		// keep track of the authentication for later
		HttpContext.Session.Set(
			"user",
			JsonSerializer.SerializeToUtf8Bytes(
				new AuthedUser(Config.C.user_map[user.id.ToString()], user.id, user.login, user.name)));
		
		// this is misuse of state but idc
		var then = Encoding.UTF8.GetString(Convert.FromBase64String(state));

		return Redirect(then);
	}
	
	[HttpPost]
	public IActionResult Logout(string then)
	{
		HttpContext.Session.Remove("user");
		return Redirect(then);
	}

	public IActionResult Index(string? mKey)
		=> View(
			new AuthIndexModel(
				Convert.ToBase64String(
					mKey == null
						? "/ouroboros/dashboard"u8
						: Encoding.UTF8.GetBytes($"/register/mkey:{mKey}")),
				mKey == null ? "to access the dashboard" : "to add a node"));
	
	private async Task<GhUserRes> GetUser(string accessToken)
	{
		using var userResp = await _http.SendAsync(
								 new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user")
								 {
									 Headers =
									 {
										 { "Authorization", $"Bearer {accessToken}" }
									 }
								 });

		return await userResp.Content.ReadFromJsonAsync<GhUserRes>();
	}

	private async Task<string> GetAccessToken(string code)
	{
		using var tokenResp = await _http.PostAsync(
								  "https://github.com/login/oauth/access_token",
								  new FormUrlEncodedContent(
								  [
									  // ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
									  new("client_id", Config.C.gh_client_id),
									  new("client_secret", Config.C.gh_client_secret),
									  new("code", code)
									  // ReSharper restore ArrangeObjectCreationWhenTypeNotEvident
								  ]));

		return (await tokenResp.Content.ReadFromJsonAsync<AccessTokenRes>()).access_token;
	}

	// ReSharper disable InconsistentNaming

	private struct AccessTokenRes
	{
		public string access_token { get; set; }
	}

	private struct GhUserRes
	{
		public int id { get; set; }

		public string login { get; set; }

		//public string  avatar_url { get; set; } // $"https://avatars.githubusercontent.com/u/{id}"
		public string? name { get; set; }
	}
}