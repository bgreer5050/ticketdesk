﻿// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (stephen@reddnet.net, http://www.reddnet.net)
//
// This file is distributed under the terms of the Microsoft Public 
// License (Ms-PL). See http://ticketdesk.codeplex.com/license
// for the complete terms of use. 
//
// For any distribution that contains code from this file, this notice of 
// attribution must remain intact, and a copy of the license must be 
// provided to the recipient.

using System;
using System.Collections.Generic;
using System.Linq;
using TicketDesk.Domain.Localization;
using TicketDesk.Domain.Model;

namespace TicketDesk.Domain.Models
{

    /// <summary>
    /// Represents a user's settings for a particular list view
    /// </summary>
    public class UserTicketListSetting
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTicketListSetting"/> class.
        /// </summary>
        /// <remarks>
        /// Required for serialization by the profile provider 
        /// </remarks>
        public UserTicketListSetting()
        { }


        /// <summary>
        /// Initializes a new instance of the <see cref="UserTicketListSetting" /> class with a default sort and filter.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <param name="listDisplayName">Display name of the list.</param>
        /// <param name="listMenuDisplayOrder">The list menu display order.</param>
        /// <param name="includeClosedTickets">if set to <c>true</c> [include closed tickets].</param>
        public UserTicketListSetting(string listName, string listDisplayName, int listMenuDisplayOrder, bool includeClosedTickets)
        {
            ListName = listName;
            ListDisplayName = listDisplayName;
            ListMenuDisplayOrder = listMenuDisplayOrder;
            SortColumns = new List<UserTicketListSortColumn>();
            SortColumns.Add(new UserTicketListSortColumn("LastUpdateDate", ColumnSortDirection.Descending));
            FilterColumns = new List<UserTicketListFilterColumn>();
            if (!includeClosedTickets)
            {
                FilterColumns.Add(new UserTicketListFilterColumn("TicketStatus", false, "Closed"));
            }
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTicketListSetting" /> class.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <param name="listDisplayName">Display name of the list.</param>
        /// <param name="listMenuDisplayOrder">The list menu display order.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="sortColumns">The sort columns.</param>
        /// <param name="filterColumns">The filter columns.</param>
        /// <param name="disabledFilterColumnNames">The disabled filter column names.</param>
        public UserTicketListSetting(string listName, string listDisplayName, int listMenuDisplayOrder, int itemsPerPage, List<UserTicketListSortColumn> sortColumns, List<UserTicketListFilterColumn> filterColumns, List<string> disabledFilterColumnNames)
        {
            ListName = listName;
            ListDisplayName = listDisplayName;
            ListMenuDisplayOrder = listMenuDisplayOrder;
            ItemsPerPage = itemsPerPage;
            SortColumns = sortColumns;
            FilterColumns = filterColumns;
            DisabledFilterColumnNames = (disabledFilterColumnNames) ?? new List<string>();
        }

        /// <summary>
        /// Gets the name of the list view.
        /// </summary>
        /// <value>The name of the list view.</value>
        public string ListName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the list view.
        /// </summary>
        /// <value>The display name of the list view.</value>
        public string ListDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the list display order.
        /// </summary>
        /// <value>The list display order.</value>
        public int ListMenuDisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the items per page.
        /// </summary>
        /// <value>The items per page.</value>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the sort columns.
        /// </summary>
        /// <value>The sort columns in the order they should be sorted by.</value>
        public ICollection<UserTicketListSortColumn> SortColumns { get; set; }

        /// <summary>
        /// Gets or sets the filter columns.
        /// </summary>
        /// <value>The filter columns.</value>
        public ICollection<UserTicketListFilterColumn> FilterColumns { get; set; }

        /// <summary>
        /// Gets or sets the disabled filter column names.
        /// </summary>
        /// <value>The disabled filter column names.</value>
        public IEnumerable<string> DisabledFilterColumnNames { get; set; }

        /// <summary>
        /// Modifies the filter settings.
        /// </summary>
        /// <param name="setting">The list settings which will be modified.</param>
        /// <param name="pageSize">Number of items to display on a single page.</param>
        /// <param name="TicketStatus">The current status filter setting.</param>
        /// <param name="owner">The owner filter setting.</param>
        /// <param name="assignedTo">The assigned to filter setting.</param>
        public void ModifyFilterSettings(int pageSize, string TicketStatus, string owner, string assignedTo)
        {
            ItemsPerPage = pageSize;

            if (!DisabledFilterColumnNames.Contains("TicketStatus"))
            {
                FilterColumns.ChangeTicketStatusFilter(TicketStatus);
            }
            if (!DisabledFilterColumnNames.Contains("Owner"))
            {
                FilterColumns.ChangeOwnerFilter(owner);
            }
            if (!DisabledFilterColumnNames.Contains("AssignedTo"))
            {
                FilterColumns.ChangeAssignedFilter(assignedTo);
            }
        }


        internal static List<UserTicketListSetting> GetDefaultListSettings(string userId)
        {
            
            var settings = new List<UserTicketListSetting>();

            var disableStatusColumn = new List<string> { "TicketStatus" };
            var disableOwnerColumn = new List<string> { "Owner" };

            var disOrder = 0;

            var mySortColumns = new List<UserTicketListSortColumn>();
            var myFilterColumns = new List<UserTicketListFilterColumn>();
            mySortColumns.Add(new UserTicketListSortColumn("LastUpdateDate", ColumnSortDirection.Descending));
            myFilterColumns.Add(new UserTicketListFilterColumn("TicketStatus", false, TicketStatus.Closed));
            myFilterColumns.Add(new UserTicketListFilterColumn("Owner", true, userId));
            settings.Add(new UserTicketListSetting("mytickets", TicketDeskDomainText.DefaultListNameMyTickets, disOrder++, 20, mySortColumns, myFilterColumns, disableOwnerColumn));


            var openSortColumns = new List<UserTicketListSortColumn>();
            var openFilterColumns = new List<UserTicketListFilterColumn>();
            openSortColumns.Add(new UserTicketListSortColumn("TicketStatus", ColumnSortDirection.Ascending));
            openSortColumns.Add(new UserTicketListSortColumn("LastUpdateDate", ColumnSortDirection.Descending));
            openFilterColumns.Add(new UserTicketListFilterColumn("TicketStatus", false, TicketStatus.Closed));
            settings.Add(new UserTicketListSetting("opentickets", TicketDeskDomainText.DefaultListNameOpenTickets, disOrder++, 20, openSortColumns, openFilterColumns, disableStatusColumn));


            var historyticketsSortColumns = new List<UserTicketListSortColumn>();
            var historyticketsFilterColumns = new List<UserTicketListFilterColumn>();
            historyticketsSortColumns.Add(new UserTicketListSortColumn("LastUpdateDate", ColumnSortDirection.Descending));
            historyticketsFilterColumns.Add(new UserTicketListFilterColumn("TicketStatus", true, TicketStatus.Closed));

            settings.Add(new UserTicketListSetting("historytickets", TicketDeskDomainText.DefaultListNameHistoryTickets, disOrder++, 20, historyticketsSortColumns, historyticketsFilterColumns, disableStatusColumn));

            return settings;
        }
        
    }
}

