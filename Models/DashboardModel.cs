namespace Ouroboros.Models;

public record DashboardModel(AuthedUser User, IEnumerable<Headscale.HeadscaleNode> YourNodes, IEnumerable<Headscale.HeadscaleNode> OtherNodes);