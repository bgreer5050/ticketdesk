﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TicketDesk.Web.Client.Annotations;
using TicketDesk.Web.Identity;
using TicketDesk.Web.Identity.Model;

namespace TicketDesk.Web.Client.Models
{

    /// <summary>
    /// Handles security migration functions for upgrades from TD 2.x databases.
    /// </summary>
    public static class LegacySecurityMigrator
    {
        [UsedImplicitly]
        private class LegacyUser
        {
            internal Guid UserId { get; set; }
            internal string Email { get; set; }
            internal string Password { get; set; }
            internal string Comment { get; set; }
            internal int PasswordFormat { get; set; }
        }

        /// <summary>
        /// Migrates the users and roles from a legacy database to the new TD 2.5 schema.
        /// </summary>
        /// <param name="context">The identity database context</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        /// <returns><c>true</c> if users migrated, <c>false</c> otherwise.</returns>
        public static bool MigrateSecurity(TicketDeskIdentityContext context, TicketDeskUserManager userManager, TicketDeskRoleManager roleManager)
        {
            EnsureRolesExist(roleManager);
            var appId =
                context.Database.SqlQuery<Guid>(
                    "select ApplicationId from aspnet_Applications where ApplicationName = 'TicketDesk'").First().ToString();
            var users = context.Database.SqlQuery<LegacyUser>(
                "select UserId, Email, Password, PasswordFormat, Comment from aspnet_Membership where ApplicationId = '" + appId + "' and IsApproved = 1 and IsLockedOut = 0").ToList();
            const string roleQuery = "SELECT r.RoleName FROM aspnet_UsersInRoles u inner join aspnet_Roles r on u.RoleId = r.RoleId WHERE u.UserId = @userId and r.ApplicationId = @appId";
            
            foreach (var user in users)
            {
                var newUser = new TicketDeskUser
                {
                    UserName = user.Email,
                    Email = user.Email,
                    DisplayName = user.Comment,
                };

                IdentityResult result = null;
                result = user.PasswordFormat == 0 ?
                    userManager.Create(newUser, user.Password) :
                    userManager.Create(newUser);

                if (result.Succeeded)
                {
                    var rolesForUser =
                        context.Database.SqlQuery<string>(roleQuery, 
                        new SqlParameter("userId", user.UserId),
                        new SqlParameter("appId", appId));
                    var newRoles = new List<string>();
                    foreach (var role in rolesForUser)
                    {
                        switch (role.ToLowerInvariant())
                        {
                            case "administrators":
                                newRoles.Add("TdAdministrators");
                                break;
                            case "helpdesk":
                                newRoles.Add("TdHelpDeskUsers");
                                break;
                            case "ticketsubmitters":
                                newRoles.Add("TdInternalUsers");
                                break;
                            default:
                                newRoles.Add("TdPendingUsers");
                                break;
                        }
                    }
                    userManager.AddToRoles(newUser.Id, newRoles.ToArray());
                }
            }
            return true;
        }

        /// <summary>
        /// Ensures the correct set of TD standard roles exist.
        /// </summary>
        /// <param name="roleManager">The role manager.</param>
        private static void EnsureRolesExist(TicketDeskRoleManager roleManager)
        {
            roleManager.Create(new IdentityRole("TdAdministrators"));
            roleManager.Create(new IdentityRole("TdHelpDeskUsers"));
            roleManager.Create(new IdentityRole("TdInternalUsers"));
            roleManager.Create(new IdentityRole("TdPendingUsers"));
        }
    }
}