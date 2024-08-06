using System.Diagnostics;
using System.Text.Json;

namespace Ouroboros;

/* wrapping progress list (only APIs we need):
 * [x] nodes:
 *   [x] list
 *   [x] delete
 *   [x] expire
 *   [x] register
 *   [x] rename
 *   [x] tag
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
		var psi = new ProcessStartInfo(Config.C.hs_bin_path, args.Concat(["-o", "json-line"]))
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
		=> JsonSerializer.Deserialize<HeadscaleNode[]>(await Invoke("nodes", "list"), Jso)!;

	public static async Task<bool> NodeDelete(int id)
	{
		var res = await Invoke("nodes", "delete", "-i", id.ToString(), "--force");
		return res.Trim() == "{}";
	}
	
	public static async Task<HeadscaleNode> NodeExpire(int id)
		=> JsonSerializer.Deserialize<HeadscaleNode>(await Invoke("nodes", "expire", "-i", id.ToString()), Jso)!;

	public static async Task<HeadscaleNode> NodeRegister(string user, string nodekey)
		=> JsonSerializer.Deserialize<HeadscaleNode>(await Invoke("nodes", "register", "--user", user, "--key", nodekey), Jso)!;
	
	public static async Task<HeadscaleNode> NodeRename(int id, string name)
		=> JsonSerializer.Deserialize<HeadscaleNode>(await Invoke("nodes", "rename", name, "-i", id.ToString()), Jso)!;
    
	/*public static async Task<HeadscaleNode> NodeTags(int id, string[] tags)
	{
		var args = new [] {"node", "tags", "-i", id.ToString()}
				  .Concat(tags.SelectMany(t => new[] {"-t", "tag:" + t}))
				  .ToArray();
		
		return JsonSerializer.Deserialize<HeadscaleNode>(await Invoke(args), Jso)!;
	}*/

	public struct HeadscaleTimestamp
	{
		private const int NsecsPerTick = 100; // 1 tick = 100ns
		
		public long  Seconds { get; set; }
		public ulong Nanos   { get; set; }

		public DateTime Parse() => DateTime.UnixEpoch.AddSeconds(Seconds).AddTicks((long) Nanos / NsecsPerTick);
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
		public string[]?          ForcedTags           { get; set; }
		public string?            GivenName            { get; set; }
		public bool               Online               { get; set; }
	}
}