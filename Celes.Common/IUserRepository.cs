
namespace Celes.Common
{
	public interface IUserRepository
	{
		/// <summary>
		/// Returns true of no users exist in the repository.
		/// </summary>
		bool IsEmpty();

		bool ValidateCredentials(string userName, string password);
		void CreateUser(string userName, string password);
	}
}
