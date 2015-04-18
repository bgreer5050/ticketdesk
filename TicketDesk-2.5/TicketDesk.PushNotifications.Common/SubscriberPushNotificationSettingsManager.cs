﻿// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (stephen@reddnet.net, http://www.reddnet.net)
//
// This file is distributed under the terms of the Microsoft Public 
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use. 
//
// For any distribution that contains code from this file, this notice of 
// attribution must remain intact, and a copy of the license must be 
// provided to the recipient.

using System.Threading.Tasks;
using TicketDesk.PushNotifications.Common.Model;

namespace TicketDesk.PushNotifications.Common
{
    public class SubscriberPushNotificationSettingsManager
    {
        private TdPushNotificationContext NotificationContext { get; set; }
        public SubscriberPushNotificationSettingsManager(TdPushNotificationContext notificationContext)
        {
            NotificationContext = notificationContext;
        }

        public async Task<SubscriberPushNotificationSetting> GetSettingsForSubscriber(string subscriberId)
        {
            return await NotificationContext.SubscriberPushNotificationSettings.FindAsync(subscriberId);
        }

        public void AddSettingsForSubscriber(SubscriberPushNotificationSetting settings)
        {
            NotificationContext.SubscriberPushNotificationSettings.Add(settings);
        }
    }
}
