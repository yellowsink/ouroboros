using System.Diagnostics;

namespace Ouroboros;

/// <summary>
/// Handles talking to Headscale
/// </summary>
public static class Headscale
{
	public static async Task<string> InvokeHeadscale(params string[] args)
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
}