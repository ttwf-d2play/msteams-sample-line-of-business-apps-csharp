using CrossVertical.Announcement.Models;
using CrossVertical.Announcement.Repository;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CrossVertical.Announcement.Helpers
{
    public class Common
    {
        public static Role GetUserRole(string emailId, Tenant tenatInfo)
        {
            var role = Role.User;
            if (tenatInfo.Moderators.Contains(emailId))
                role = Role.Moderator;
            if (tenatInfo.Admin == emailId)
                role = Role.Admin;
            return role;
        }

        public static async Task<List<Campaign>> GetMyAnnouncements(string emailId, string tenantId)
        {
            var tenant = await Cache.Tenants.GetItemAsync(tenantId);
            Role role = GetUserRole(emailId, tenant);

            var myTenantAnnouncements = new List<Campaign>();
            foreach (var announcementId in tenant.Announcements)
            {
                var announcement = await Cache.Announcements.GetItemAsync(announcementId);
                if (announcement != null)
                {
                    if (role == Role.Moderator || role == Role.Admin)
                    {
                        myTenantAnnouncements.Add(announcement);
                    }
                    else if (announcement.Recipients.Channels.Any(c => c.Members.Contains(emailId))
                          || announcement.Recipients.Groups.Any(g => g.Users.Any(u => u.Id == emailId)))
                    {
                        // Validate if user is part of this announcement.
                        myTenantAnnouncements.Add(announcement);
                    }
                }
            }
            return myTenantAnnouncements;
        }

    }
}