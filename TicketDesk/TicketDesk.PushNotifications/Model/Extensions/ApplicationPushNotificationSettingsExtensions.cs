﻿// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public 
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use. 
//
// For any distribution that contains code from this file, this notice of 
// attribution must remain intact, and a copy of the license must be 
// provided to the recipient.

using System.Data.Entity;
using System.Linq;

namespace TicketDesk.PushNotifications.Model
{
    public static class ApplicationPushNotificationSettingsExtensions
    {
        public static ApplicationPushNotificationSetting GetTicketDeskSettings(this DbSet<ApplicationPushNotificationSetting> settings)
        {
            return settings.FirstOrDefault(s => s.ApplicationName == "CooperDesk");
        }
    }
}
