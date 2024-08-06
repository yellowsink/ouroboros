using System.Text.Json;

namespace Ouroboros;

public record AuthedUser(string HeadscaleName, int GithubId, string GithubName, string? GithubPretty)
{
	public static AuthedUser? FromCtx(HttpContext ctx)
	{
		var userSer = ctx.Session.Get("user");
		if (userSer == null)
			return null;

		var user = JsonSerializer.Deserialize<AuthedUser>(userSer);
		return user;
	}
}