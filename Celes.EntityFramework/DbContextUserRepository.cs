using Celes.Common;
using System;
using System.Data.Entity;
using System.Linq;
using Crypt = BCrypt.Net.BCrypt;

namespace Celes.EntityFramework
{
	public sealed class DbContextUserRepository : IUserRepository
	{
		private readonly DbContext _dbContext;

		public DbContextUserRepository(DbContext dbContext)
		{
			if (dbContext == null)
			{
				throw new ArgumentNullException("dbContext");
			}

			_dbContext = dbContext;
		}

		private DbSet<User> Users { get { return _dbContext.Set<User>(); } }

		bool IUserRepository.IsEmpty()
		{
			return !Users.Any();
		}

		bool IUserRepository.ValidateCredentials(string userName, string password)
		{
			var user = Users.FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
			if (user == null)
			{
				return false;
			}

			return Crypt.Verify(password, user.PasswordHash);
		}

		void IUserRepository.CreateUser(string userName, string password)
		{
			if (string.IsNullOrWhiteSpace(userName))
			{
			    throw new ArgumentNullException("userName");
			}
			
			if (string.IsNullOrEmpty(password))
			{
			    throw new ArgumentNullException("password");
			}

			if (password.Length < 7)
			{
				throw new ArgumentException("The password must have at least 7 characters.");
			}
			
			var userExists = Users.Any(u => u.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
			if (userExists)
			{
				throw new InvalidOperationException(string.Format("There is already a user named '{0}'.", userName));
			}

			Users.Add(new User
			{
				UserName = userName,
				PasswordHash = Crypt.HashPassword(password),
			});

			_dbContext.SaveChanges();
		}
	}
}
