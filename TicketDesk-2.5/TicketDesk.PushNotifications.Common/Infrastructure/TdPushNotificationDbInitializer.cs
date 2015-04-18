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

using System.Data.Entity;
using TicketDesk.PushNotifications.Common.Migrations;

namespace TicketDesk.PushNotifications.Common
{
    public class TdPushNotificationDbInitializer : MigrateDatabaseToLatestVersion<TdPushNotificationContext, Configuration>
    {
        //no implementation, defined here to simplify and unify naming conventions and usage patterns 
    }
}
