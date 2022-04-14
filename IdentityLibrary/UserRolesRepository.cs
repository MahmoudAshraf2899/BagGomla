using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLibrary
{
    internal class UserRolesRepository
    {
        private readonly DatabaseContext _databaseContext;

        public UserRolesRepository(DatabaseContext database)
        {
            _databaseContext = database;
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public IList<string> FindByUserId(string userId)
        {
            var User = _databaseContext.AspNetUsers.SingleOrDefault(c => c.Id == userId);
            var roles = new List<string>();
            if (User != null)
            {
                roles = _databaseContext.AspNetUserRoles.Include(r => r.AspNetRoles).Where(c => c.UserId == User.Id && !c.IsDeleted).Select(r => r.AspNetRoles.Id).ToList();
            }
            var userRole = _databaseContext.AspNetRoles.FirstOrDefault(r => r.IsDeleted == false && r.Id == User.RoleID && !roles.Contains(User.RoleID));
            if(userRole != null)
            {
                roles.Add(userRole.Id);
            }
            //Where(u => u.Id == userId).SelectMany(r => r.AspNetUserRoles).Select(c=>c.AspNetRoles);
            return roles;
        }

        internal async Task AddUserToRole<T>(T user, string roleId) where T : IdentityUser
        {
            _databaseContext.AspNetUserRoles.Add(new AspNetUserRoles() { RoleId = roleId, UserId = user.Id });
            await _databaseContext.SaveChangesAsync();
        }
    }
}