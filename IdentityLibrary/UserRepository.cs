using IdentityLibrary.DataModel;
using System;
using System.Linq;

namespace IdentityLibrary
{
    public class UserRepository<T> where T : IdentityUser
    {
        private readonly DatabaseContext _databaseContext;

        public UserRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        internal T GeTByName(string userName)
        {
            var user = _databaseContext.AspNetUsers.SingleOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                T result = (T)Activator.CreateInstance(typeof(T));
                result.Id = user.Id;
                result.UserName = user.UserName;
                result.PasswordHash = user.PasswordHash;
                result.SecurityStamp = user.SecurityStamp;
                result.Email = result.Email;
                result.EmailConfirmed = user.EmailConfirmed;
                result.PhoneNumber = user.PhoneNumber;
                result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.LockoutEnabled = user.LockoutEnabled;
                result.LockoutEndDateUtc = user.LockoutEndDateUtc;
                result.AccessFailedCount = user.AccessFailedCount;
                return result;
            }
            return null;
        }

        internal T GeTByEmail(string email)
        {
            var user = _databaseContext.AspNetUsers.SingleOrDefault(u => u.Email == email);
            if (user != null)
            {
                T result = (T)Activator.CreateInstance(typeof(T));

                result.Id = user.Id;
                result.UserName = user.UserName;
                result.PasswordHash = user.PasswordHash;
                result.SecurityStamp = user.SecurityStamp;
                result.Email = result.Email;
                result.EmailConfirmed = user.EmailConfirmed;
                result.PhoneNumber = user.PhoneNumber;
                result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.LockoutEnabled = user.LockoutEnabled;
                result.LockoutEndDateUtc = user.LockoutEndDateUtc;
                result.AccessFailedCount = user.AccessFailedCount;
                return result;
            }
            return null;
        }

        internal T GeTByPhone(string Phone)
        {
            var user = _databaseContext.AspNetUsers.SingleOrDefault(u => u.PhoneNumber == Phone);
            if (user != null)
            {
                T result = (T)Activator.CreateInstance(typeof(T));

                result.Id = user.Id;
                result.UserName = user.UserName;
                result.PasswordHash = user.PasswordHash;
                result.SecurityStamp = user.SecurityStamp;
                result.Email = result.Email;
                result.EmailConfirmed = user.EmailConfirmed;
                result.PhoneNumber = user.PhoneNumber;
                result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.LockoutEnabled = user.LockoutEnabled;
                result.LockoutEndDateUtc = user.LockoutEndDateUtc;
                result.AccessFailedCount = user.AccessFailedCount;
                return result;
            }
            return null;
        }

        internal int Insert(T user)
        {
            _databaseContext.AspNetUsers.Add(new AspNetUsers
            {
                Id = user.Id,
                UserName = user.UserName,
                PasswordHash = user.PasswordHash,
                SecurityStamp = user.SecurityStamp,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc,
                AccessFailedCount = user.AccessFailedCount,
                Name = user.Name,
                Address = user.Address,
                RoleID = user.RoleID
            });

            return _databaseContext.SaveChanges();
        }

        /// <summary>
        /// Returns an T given the user’s id
        /// </summary>
        /// <param name=”userId”>The user’s id</param>
        /// <returns></returns>
        public T GeTById(string userId)
        {
            var user = _databaseContext.AspNetUsers.Find(userId);
            if (user != null)
            {
                T result = (T)Activator.CreateInstance(typeof(T));

                result.Id = user.Id;
                result.UserName = user.UserName;
                result.PasswordHash = user.PasswordHash;
                result.SecurityStamp = user.SecurityStamp;
                result.Email = result.Email;
                result.EmailConfirmed = user.EmailConfirmed;
                result.PhoneNumber = user.PhoneNumber;
                result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.LockoutEnabled = user.LockoutEnabled;
                result.LockoutEndDateUtc = user.LockoutEndDateUtc;
                result.AccessFailedCount = user.AccessFailedCount;
                return result;
            }
            return null;
        }

        /// <summary>
        /// Return the user’s password hash
        /// </summary>
        /// <param name=”userId”>The user’s id</param>
        /// <returns></returns>
        public string GetPasswordHash(string userId)
        {
            var user = _databaseContext.AspNetUsers.FirstOrDefault(u => u.Id == userId);
            var passHash = user != null ? user.PasswordHash : null;
            return passHash;
        }

        /// <summary>
        /// Updates a user in the Users table
        /// </summary>
        /// <param name=”user”></param>
        /// <returns></returns>
        public int Update(T user)
        {
            var result = _databaseContext.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
            if (result != null)
            {
                result.UserName = user.UserName;
                result.PasswordHash = user.PasswordHash;
                result.SecurityStamp = user.SecurityStamp;
                result.Email = result.Email;
                result.EmailConfirmed = user.EmailConfirmed;
                result.PhoneNumber = user.PhoneNumber;
                result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.LockoutEnabled = user.LockoutEnabled;
                result.LockoutEndDateUtc = user.LockoutEndDateUtc;
                result.AccessFailedCount = user.AccessFailedCount;
                result.RoleID = user.RoleID;
                return _databaseContext.SaveChanges();
            }
            return 0;
        }
    }
}