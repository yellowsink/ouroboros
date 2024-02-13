using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ouroboros.Pages;

public class DashboardModel : PageModel
{
	public Headscale.HeadscaleNode[] NodesToShow = [];
    
	public async Task<PageResult> OnGetAsync()
	{
		// TODO: check user name and only show their devices

		NodesToShow = await Headscale.NodesList();

		return Page();
	}
}