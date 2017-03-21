
using NetworkManager.Core.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;

namespace NetworkManager.Data
{
    public class BigBangInitializer : DropCreateDatabaseIfModelChanges<NetAppDbContext>
    {
        protected override void Seed(NetAppDbContext context)
        {
            Initialize(context);
            base.Seed(context);
        }

        public void Initialize(NetAppDbContext context)
        {
            try
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
                {
                    AllowOnlyAlphanumericUserNames = false
                };
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                if (!roleManager.RoleExists("Admin"))
                {
                    roleManager.Create(new IdentityRole("Admin"));
                }

                if (!roleManager.RoleExists("Member"))
                {
                    roleManager.Create(new IdentityRole("Member"));
                }

                var user = new ApplicationUser()
                {
                    Email = "john_dillinger@live.ru",
                    UserName = "john_dillinger@live.ru",
                    FirstName = "john_dillinger@live.ru",
                    LastName = "john_dillinger@live.ru"
                };

                var userResult = userManager.Create(user, "john_dillinger@live.ru");

                if (userResult.Succeeded)
                {
                    userManager.AddToRole<ApplicationUser, string>(user.Id, "Admin");
                }

                context.Commit();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
