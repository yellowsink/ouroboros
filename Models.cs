// ReSharper disable once CheckNamespace

namespace Ouroboros.Models;

public record AuthIndexModel(string ReturnTo, string ReturnToPretty);

public record AuthNotRegisteredModel(string Login);

public record RegisterIndexModel(AuthedUser User, string TrimmedNK);

public record DashboardModel(
	AuthedUser                                                            User,
	IEnumerable<Headscale.HeadscaleNode>                                  YourNodes,
	IEnumerable<Headscale.HeadscaleNode>                                  OtherNodes,
	Dictionary<int, (Headscale.HeadscaleRoute, Headscale.HeadscaleRoute)> YourExitNodes,
	Dictionary<int, (Headscale.HeadscaleRoute, Headscale.HeadscaleRoute)> OtherExitNodes,
	IEnumerable<Headscale.HeadscaleRoute>     YourOtherRoutes,
	IEnumerable<Headscale.HeadscaleRoute>     OtherOtherRoutes);