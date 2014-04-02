using Celes.Mvc4.Models;
using System.Security.Principal;

namespace Celes.Mvc4.Services
{
	public interface IAuthorizationProvider
	{
		bool IsAuthorizedToView(IPrincipal user, IContentInfo contentInfo);
	}
}
