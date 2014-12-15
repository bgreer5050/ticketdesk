﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using TicketDesk.Domain.Model.Extensions;
using TicketDesk.Web.Identity;

namespace TicketDesk.Web.Client.Controllers
{
    [Authorize]
    public class TicketActivityController : Controller
    {

        private TicketDeskContext Context { get; set; }
        public TicketActivityController(TicketDeskContext context)
        {
            Context = context;
        }

        public async Task<ActionResult> LoadActivity(TicketActivity activity, int ticketId)
        {
            var ticket = await Context.Tickets.FindAsync(ticketId);
            Context.SecurityProvider.IsTicketActivityValid(ticket, activity);
            ViewBag.CommentRequired = activity.IsCommentRequired();
            ViewBag.Activity = activity;
            return PartialView("_ActivityForm", ticket); //PartialView(string.Format("_{0}", activity), ticket);
        }

        public async Task<ActionResult> ActivityButtons(int ticketId)
        {
            var ticket = await Context.Tickets.FindAsync(ticketId);
            var activities = Context.SecurityProvider.GetValidTicketActivities(ticket);
            return PartialView("_ActivityButtons", activities);
        }


        [HttpPost]
        public async Task<ActionResult> AddComment(int ticketId, string comment)
        {
            const TicketActivity activity = TicketActivity.AddComment;
            Action<Ticket> activityFn =
                t => t.TicketEvents.AddActivityEvent(Context.SecurityProvider.CurrentUserId, activity, comment);

            return await PerformActivity(ticketId, activityFn, activity);
        }

        [HttpPost]
        public async Task<ActionResult> Assign(int ticketId, string comment, string assignedTo, string priority)
        {
            const TicketActivity activity = TicketActivity.Assign;
            return await ChangeAssignment(ticketId, comment, assignedTo, priority, activity);
        }

        [HttpPost]
        public async Task<ActionResult> GiveUp(int ticketId, string comment, string priority)
        {
            const TicketActivity activity = TicketActivity.GiveUp;
            Action<Ticket> activityFn = t =>
            {
                t.AssignedTo = null;
                if (!string.IsNullOrEmpty(priority))
                {
                    if (t.Priority == priority)
                    {
                        priority = null;
                    }
                    else
                    {
                        t.Priority = priority;
                    }
                }
                t.TicketEvents.AddActivityEvent(Context.SecurityProvider.CurrentUserId, activity, comment, priority, null);
            };
            return await PerformActivity(ticketId, activityFn, activity);
        }

        [HttpPost]
        public async Task<ActionResult> Pass(int ticketId, string comment, string assignedTo, string priority)
        {
            const TicketActivity activity = TicketActivity.Pass;
            return await ChangeAssignment(ticketId, comment, assignedTo, priority, activity);
        }

        [HttpPost]
        public async Task<ActionResult> ReAssign(int ticketId, string comment, string assignedTo, string priority)
        {
            const TicketActivity activity = TicketActivity.ReAssign;
            return await ChangeAssignment(ticketId, comment, assignedTo, priority, activity);
        }

        [HttpPost]
        public async Task<ActionResult> TakeOver(int ticketId, string comment, string priority)
        {
            const TicketActivity activity = TicketActivity.TakeOver;
            Action<Ticket> activityFn = t =>
            {
                t.AssignedTo = Context.SecurityProvider.CurrentUserId;
                if (!string.IsNullOrEmpty(priority))
                {
                    if (t.Priority == priority)
                    {
                        priority = null;
                    }
                    else
                    {
                        t.Priority = priority;
                    }
                }
                t.TicketEvents.AddActivityEvent(Context.SecurityProvider.CurrentUserId, activity, comment, priority, null);
            };
            return await PerformActivity(ticketId, activityFn, activity);
        }
       

        private async Task<ActionResult> ChangeAssignment(int ticketId, string comment, string assignedTo, string priority, TicketActivity activity)
        {
            if (Context.SecurityProvider.CurrentUserId == assignedTo)//attempting to assign/reassign to self
            {
                return await TakeOver(ticketId, comment, priority);
            }
            Action<Ticket> activityFn =
                t =>
                {
                    t.AssignedTo = assignedTo;
                    if (!string.IsNullOrEmpty(priority))
                    {
                        if (t.Priority == priority)
                        {
                            priority = null;
                        }
                        else
                        {
                            t.Priority = priority;
                        }
                    }
                    var toName = UserDisplayInfo.GetUserInfo(assignedTo).DisplayName;
                    t.TicketEvents.AddActivityEvent(Context.SecurityProvider.CurrentUserId, activity, comment, priority, toName);
                };

            return await PerformActivity(ticketId, activityFn, activity);
        }

        private async Task<ActionResult> PerformActivity(int ticketId, Action<Ticket> activityFn, TicketActivity activity)
        {
            var ticket = await GetActivityTicket(ticketId, activity);
            if (ModelState.IsValid)
            {
                activityFn(ticket);

                var result = await Context.SaveChangesAsync(); //save changes catches lastupdatedby and date automatically
                if (result > 0)
                {
                    return new EmptyResult();
                }
            }
            ViewBag.CommentRequired = activity.IsCommentRequired();
            ViewBag.Activity = activity;
            return PartialView("_ActivityForm", ticket);
        }

        public async Task<Ticket> GetActivityTicket(int ticketId, TicketActivity activity)
        {
            var ticket = await Context.Tickets.FindAsync(ticketId);
            if (!Context.SecurityProvider.IsTicketActivityValid(ticket, activity))
            {
                ModelState.AddModelError("Auth", new ApplicationException("user is not authorized to perform this activity."));
            }

            return ticket;

        }


    }
}