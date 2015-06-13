﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Postal;
using TicketDesk.Domain.Model;

namespace TicketDesk.Web.Client.Models
{
    public class TicketEmail : Email
    {
        public Ticket Ticket { get; set; }

        public string SiteRootUrl { get; set; }
    }
}