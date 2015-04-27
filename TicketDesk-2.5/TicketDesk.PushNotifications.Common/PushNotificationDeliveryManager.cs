﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using TicketDesk.PushNotifications.Common.Model;

namespace TicketDesk.PushNotifications.Common
{
    public class PushNotificationDeliveryManager
    {
        private static ICollection<IPushNotificationDeliveryProvider> _deliveryProviders;

        public static ICollection<IPushNotificationDeliveryProvider> DeliveryProviders
        {
            get
            {
                if (_deliveryProviders == null)
                {
                    _deliveryProviders = new List<IPushNotificationDeliveryProvider>();
                    using (var context = new TdPushNotificationContext())
                    {
                        var providerConfigs = context.TicketDeskPushNotificationSettings.DeliveryProviderSettings;
                        foreach (var prov in providerConfigs)
                        {
                            if (prov.IsEnabled)
                            {
                                var provType = Type.GetType(prov.ProviderAssemblyQualifiedName);
                                if (provType != null)
                                {
                                    var ci = provType.GetConstructor(new []{typeof(JToken)});
                                    if (ci != null)
                                    {
                                        _deliveryProviders.Add((IPushNotificationDeliveryProvider)ci.Invoke(new []{prov.ProviderConfigurationData}));
                                    }
                                }
                            }
                        }
                    }
                }
                return _deliveryProviders;
            }
            set { _deliveryProviders = value; }
        }


        public async Task SendNotification
        (
            int ticketId,
            string subscriberId,
            string destinationAddress,
            string destinationType
        )
        {
            using (var context = new TdPushNotificationContext())
            {
                
                var readyNote = await context.PushNotificationItems
                    .FirstOrDefaultAsync(n =>
                        (
                            n.TicketId == ticketId &&
                            n.SubscriberId == subscriberId &&
                            n.DestinationAddress == destinationAddress &&
                            n.DestinationType == destinationType
                        ) &&
                        (
                            n.DeliveryStatus == PushNotificationItemStatus.Scheduled ||
                            n.DeliveryStatus == PushNotificationItemStatus.Retrying)
                        );
                if (readyNote == null) { return; }

                await SendNotificationMessageAsync(context, readyNote);
                await context.SaveChangesAsync();
            }
        }



        public async Task SendNextReadyNotification()
        {
            using (var context = new TdPushNotificationContext())
            {
                //get the next notification that is ready to send
                var readyNote =
                    await context.PushNotificationItems.OrderBy(n => n.ScheduledSendDate).FirstOrDefaultAsync(
                        n =>
                            (n.DeliveryStatus == PushNotificationItemStatus.Scheduled || n.DeliveryStatus == PushNotificationItemStatus.Retrying) &&
                            n.ScheduledSendDate <= DateTimeOffset.Now);


                await SendNotificationMessageAsync(context, readyNote);
                await context.SaveChangesAsync();
            }
        }


        private async Task SendNotificationMessageAsync(TdPushNotificationContext context, PushNotificationItem readyNote)
        {
            var retryMax = context.TicketDeskPushNotificationSettings.RetryAttempts;
            var retryIntv = context.TicketDeskPushNotificationSettings.RetryIntervalMinutes;

            //find a provider for the notification destination type
            var provider = DeliveryProviders.FirstOrDefault(p => p.DestinationType == readyNote.DestinationType);
            if (provider == null)
            {
                //no provider
                readyNote.DeliveryStatus = PushNotificationItemStatus.NotAvailable;
                readyNote.ScheduledSendDate = null;
            }
            else
            {
                await provider.SendReadyMessageAsync(readyNote, retryMax, retryIntv);
            }
        }

        /*
        InProcess
         *  Create context
         *  Get one ready item
         *  Mark sending??/
         *  Generate email
         *  Send email
         *      fail
         *         Mark for retry
         *      success     
         *          Mark sent
         *  Clear context
         *      If two or more failues, break;
         *      Repeat until all are sent
        Azure
         *  WebJob Schedule Trigger
         *      Create context
         *      Get all ready items
         *      Queue all ready items
         *      Mark sending??
         *      Clear context - repeat until all are queued
         *  WebJob Queue Trigger Function
         *      Create context
         *      Check if sent
         *          Discard if sent
         *      Generate email
         *      Send email
         *          fail
         *              Mark for retry
         *          success     
         *              Mark sent
         *      
        */

    }
}
