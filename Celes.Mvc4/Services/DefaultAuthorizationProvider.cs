using Celes.Mvc4.Models;
using System.Security.Principal;

namespace Celes.Mvc4.Services
{
	public sealed class DefaultAuthorizationProvider : IAuthorizationProvider
	{
		public bool IsAuthorizedToView(IPrincipal user, IContentInfo contentInfo)
		{
			return true;
		}
	}
}
