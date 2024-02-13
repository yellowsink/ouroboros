using System.Diagnostics;
using System.Text.Json;

namespace Ouroboros;

/* wrapping progress list (only APIs we need):
 * [ ] nodes:
 *   [x] list
 *   [ ] delete
 *   [ ] expire
 *   [ ] register
 *   [ ] rename
 *   [ ] tag
 * [ ] preauthkeys:
 *   [ ] create
 *   [ ] expire
 *   [ ] list
 * [ ] routes:
 *   [ ] delete
 *   [ ] disable
 *   [ ] enable
 *   [ ] list
 */

/// <summary>
/// Handles talking to Headscale
/// </summary>
public static class Headscale
{
	private static readonly JsonSerializerOptions Jso = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
	};
	
	private static async Task<string> Invoke(params string[] args)
	{
		var psi = new ProcessStartInfo(Config.C.hs_bin_path, args)
		{
			RedirectStandardOutput = true
		};
		if (Config.C.hs_is_remote)
		{
			psi.Environment.Add("HEADSCALE_CLI_ADDRESS", Config.C.hs_address);
			psi.Environment.Add("HEADSCALE_CLI_API_KEY", Config.C.hs_api_key); //{"HEADSCALE_CLI_INSECURE", "0"}
		}
		
		var proc = Process.Start(psi);
		await proc!.WaitForExitAsync();

		return await proc.StandardOutput.ReadToEndAsync();
	}
	
	public static async Task<HeadscaleNode[]> NodesList()
		=> JsonSerializer.Deserialize<HeadscaleNode[]>(await Invoke("nodes", "list", "-o", "json-line"), Jso)!;
    
	public struct HeadscaleTimestamp
	{
		public long  Seconds { get; set; }
		public ulong Nanos   { get; set; }

		public DateTime Parse() => new(Math.Max(DateTime.MinValue.Ticks / TimeSpan.TicksPerSecond, Seconds) * TimeSpan.TicksPerSecond + (long) (Nanos / 100));
	}

	public class HeadscaleUser
	{
		public string?            Id        { get; set; }
		public string?            Name      { get; set; }
		public HeadscaleTimestamp CreatedAt { get; set; }
	}

	public class HeadscaleNode
	{
		public int                Id                   { get; set; }
		public string?            MachineKey           { get; set; }
		public string?            NodeKey              { get; set; }
		public string?            DiscoKey             { get; set; }
		public string[]?          IpAddresses          { get; set; }
		public string?            Name                 { get; set; }
		public HeadscaleUser?     User                 { get; set; }
		public HeadscaleTimestamp LastSeen             { get; set; }
		public HeadscaleTimestamp LastSuccessfulUpdate { get; set; }
		public HeadscaleTimestamp Expiry               { get; set; }
		public HeadscaleTimestamp CreatedAt            { get; set; }
		public string?            GivenName            { get; set; }
		public bool               Online               { get; set; }
	}
}