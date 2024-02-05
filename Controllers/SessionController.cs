using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmailAssist.Data;
using EmailAssistant.Models;
using GmailAPI;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EmailAssistant.Controllers
{
    public class SessionController : Controller
    {
        private readonly EmailAssistContext _context;

        public SessionController(EmailAssistContext context)
        {
            _context = context;
        }

        // GET: Session
        public async Task<IActionResult> Index()
        {
            ViewData["UserEmail"] = GetUserEmail();
            return View(await _context.Session.ToListAsync());
        }

        // GET: Session/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Session
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Session/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Session/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SessionNumber,SessionName,EmailAddress,StartDate,EndDate")] Session session)
        {
            if (ModelState.IsValid)
            {
                List<Sender> senders = new List<Sender>();
                List<Day> days = new List<Day>();
                List<Gmail> gmails = GmailMethods.RetrieveSession(session.StartDate, session.EndDate);
                if (gmails != null) {
                    foreach (Gmail gmail in gmails) {
                        Email email = new Email(gmail.Id, gmail.InternalDate, gmail.Date, gmail.Body,
                        gmail.From, gmail.Subject, session.SessionNumber, session.EmailAddress);

                        // check the senders list for an existing entry matching the current gmail's sender address
                        // if a match is found, increment numEmails for that entry by 1
                        // if no match is found, create a new entry with numEmails = 1
                        if (senders.Any(sender => sender.SenderAddress == gmail.From)) {
                            foreach (Sender sender2 in senders.Where(sender3 => sender3.SenderAddress == gmail.From)) {
                                sender2.NumEmails += 1;
                            }
                        } else {
                            senders.Add(new Sender(session.SessionNumber, session.EmailAddress, gmail.From, 1));
                        }
                        _context.Add(email);

                        // check the days list for an existing entry matching the current gmail's date
                        // if a match is found, increment numEmails for that entry by 1
                        // if no match is found, create a new entry with numEmails = 1
                        if (days.Any(day => day.Date.Date == gmail.Date.Date)) {
                            foreach (Day day2 in days.Where(day3 => day3.Date.Date == gmail.Date.Date)) {
                                day2.NumEmails += 1;
                            }
                        } else {
                            days.Add(new Day(gmail.Date.Date, 1));
                        }
                    }
                    foreach (Sender sender in senders) {
                        _context.Add(sender);
                    }
                    foreach (Day day in days) {
                        Day existingDay = await _context.Day.FirstOrDefaultAsync(entry => entry.Date == day.Date);
                        if (existingDay != null) {
                            existingDay.NumEmails = day.NumEmails;
                        } else {
                            _context.Add(day);
                        }
                    }
                    _context.Add(session);
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        // GET: Session/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Session.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }

        // POST: Session/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SessionNumber,SessionName,EmailAddress,StartDate,EndDate")] Session session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        // GET: Session/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Session
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Session/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Session.FindAsync(id);
            if (session != null)
            {
                IEnumerable<Email> sessionEmails = await _context.Email.Where(email =>
                email.SessionEmailAddress == session.EmailAddress
                && email.SessionNumber == session.SessionNumber).ToListAsync();
                foreach (Email email in sessionEmails) {
                    _context.Email.Remove(email);
                }
                _context.Session.Remove(session);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SessionOverview(int id) {
            return View(await _context.Session.FirstOrDefaultAsync(m => m.Id == id));
        }

        public async Task<IActionResult> SessionEmails(int sessionNumber, string sessionEmail) {
            return View(await _context.Email.Where(email =>
                email.SessionEmailAddress == sessionEmail
                && email.SessionNumber == sessionNumber).ToListAsync());
        }

        public async Task<IActionResult> SessionStatistics(int sessionNumber, string sessionEmail) {
            return View(await _context.Email.Where(email =>
                email.SessionEmailAddress == sessionEmail
                && email.SessionNumber == sessionNumber).ToListAsync());
        }

        public async Task<IActionResult> SessionSummary(int sessionNumber, string sessionEmail) {
            ViewData["test"] = new List<Gmail>();
            return View(await _context.Email.Where(email =>
                email.SessionEmailAddress == sessionEmail
                && email.SessionNumber == sessionNumber).ToListAsync());
        }

        public IActionResult EmailBody(string contents) {
            ViewData["Contents"] = contents;
            return View();
        }

        private bool SessionExists(int id)
        {
            return _context.Session.Any(e => e.Id == id);
        }

        private string GetUserEmail() {
            string userEmail = "NaN";
            var userIdentity = HttpContext.User.Identity as ClaimsIdentity;
            // Check if the user is authenticated
            if (userIdentity.IsAuthenticated)
            {
                // Retrieve the email claim
                var userEmailClaim = userIdentity.FindFirst(ClaimTypes.Email);
                // Check if the email claim exists
                if (userEmailClaim != null)
                {
                    // Access the email value
                    userEmail = userEmailClaim.Value;
                }

            }
            return userEmail;
        }
    }
}
