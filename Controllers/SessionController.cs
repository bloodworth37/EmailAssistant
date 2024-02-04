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
                List<Gmail> gmails = GmailMethods.RetrieveSession(session.StartDate, session.EndDate);
                if (gmails != null) {
                    foreach (Gmail gmail in gmails) {
                        Email email = new Email();
                        email.EmailId = gmail.Id;
                        email.InternalDate = gmail.InternalDate;
                        email.Date = gmail.Date;
                        email.Body = gmail.Body;
                        email.From = gmail.From;
                        email.Subject = gmail.Subject;
                        email.SessionNumber = session.SessionNumber;
                        email.SessionEmailAddress = session.EmailAddress;
                        _context.Add(email);
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
