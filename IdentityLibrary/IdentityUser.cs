namespace IdentityLibrary
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;

    /// <summary>
    /// Class that implements the ASP.NET Identity
    /// IUser interface
    /// </summary>
    public class IdentityUser : IdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>, IUser, IUser<string>
    {
        public IdentityUser()
        {
            Id = Guid.NewGuid().ToString();
        }
        public IdentityUser(string userName) : this()
        {
            UserName = userName;
        }

        public virtual string RoleID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }
    }
}