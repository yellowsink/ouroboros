using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Ouroboros;

// config code adapted from the uwu radio constants loader
// https://github.com/uwu/radio/blob/master/UwuRadio.Server/Constants.cs

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Config
{
#if DEBUG
	private const string CfgPath = "cfg.debug.json";
#else
	private const string CfgPath = "cfg.json";
#endif

	public static readonly Config C = JsonSerializer.Deserialize<Config>(File.OpenRead(CfgPath))!;

	public bool hs_is_remote { get; init; } = false;
	
	/// <summary>
	/// (REQUIRED if headscale_is_remote)The address to attempt to reach the headscale server at
	/// </summary>
	public string? hs_address { get; init; }
	
	/// <summary>
	/// (REQUIRED if headscale_is_remote) The API key to use to connect to the headscale server
	/// </summary>
	public string? hs_api_key { get; init; }

	/// <summary>
	/// (OPTIONAL) The path to the headscale CLI binary to use
	/// </summary>
	public string hs_bin_path { get; init; } = "headscale";
	
	/// <summary>
	/// The Github OAuth2 Client ID
	/// </summary>
	public string gh_client_id { get; init; }
	
	/// <summary>
	/// The Github OAuth2 Client Secret
	/// </summary>
	public string gh_client_secret { get; init; }
	
	/// <summary>
	/// A map of github user IDs to headscale users
	/// </summary>
	// ReSharper disable once CollectionNeverUpdated.Global
	public Dictionary<string, string> user_map { get; init; }
}