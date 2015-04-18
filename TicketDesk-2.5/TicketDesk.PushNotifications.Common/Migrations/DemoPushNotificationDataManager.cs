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

using TicketDesk.PushNotifications.Common.Model;

namespace TicketDesk.PushNotifications.Common.Migrations
{
    public static class DemoPushNotificationDataManager
    {
        public static void RemoveAllPushNotificationData(TdPushNotificationContext context)
        {
            context.SubscriberPushNotificationSettings.RemoveRange(context.SubscriberPushNotificationSettings);
            context.PushNotificationItems.RemoveRange(context.PushNotificationItems);
            context.TicketDeskPushNotificationSettings = new ApplicationPushNotificationSetting();
            context.SaveChanges();
        }
        public static void SetupDemoPushNotificationData(TdPushNotificationContext context)
        {
            RemoveAllPushNotificationData(context);
            context.SaveChanges();


            context.SubscriberPushNotificationSettings.Add(new SubscriberPushNotificationSetting()
            {
                SubscriberId = "64165817-9cb5-472f-8bfb-6a35ca54be6a",
                IsEnabled = true,
                PushNotificationDestinations = new PushNotificationDestinationCollection()
                    {
                        new PushNotificationDestination()
                        {
                            SubscriberName = "Admin User",
                            DestinationAddress = "admin@example.com",
                            DestinationType = "email"
                        }
                    }
            });
            context.SubscriberPushNotificationSettings.Add(new SubscriberPushNotificationSetting()
            {
                SubscriberId = "72bdddfb-805a-4883-94b9-aa494f5f52dc",
                IsEnabled = true,
                PushNotificationDestinations = new PushNotificationDestinationCollection()
                    {
                        new PushNotificationDestination()
                        {
                            SubscriberName = "HelpDesk User",
                            DestinationAddress = "staff@example.com",
                            DestinationType = "email"
                        }
                    }
            });
            context.SubscriberPushNotificationSettings.Add(new SubscriberPushNotificationSetting()
            {
                SubscriberId = "17f78f38-fa68-445f-90de-38896140db28",
                IsEnabled = true,
                PushNotificationDestinations = new PushNotificationDestinationCollection()
                    {
                        new PushNotificationDestination()
                        {
                            SubscriberName = "Regular User",
                            DestinationAddress = "user@example.com",
                            DestinationType = "email"
                        }
                    }
            });
            context.SaveChanges();
        }
    }
}
