using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StampSystem.Models;
using StampSystem.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StampSystem.Areas.Identity.Pages
{
    [Authorize(Roles = CD.Role_HR)]  // ???? ?? ?? ?????? ???? ?????? ????? ??? ?? ??? ?????????
    public class HRReviewModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HRReviewModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IList<ApplicationUser> PendingUsers { get; set; }

        // ??? ????? ??????? ??? ?????????? ????? ?? ???? "Pending"
        public async Task OnGetAsync()
        {
            PendingUsers = await _userManager.Users
                .Where(u => u.Status == "Pending")
                .ToListAsync();
        }

        // ???????? ??? ?????
        public async Task<IActionResult> OnPostApproveAsync(string userId, string position, string unit)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Status = "Approved";
                user.Unit = unit;  // ????? ??????
                await _userManager.UpdateAsync(user);

                TempData["Message"] = $"User {user.FullName} has been approved!";
            }

            return RedirectToPage();
        }

        // ??? ?????
        public async Task<IActionResult> OnPostRejectAsync(string userId, string reason)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Status = "Rejected";
                await _userManager.UpdateAsync(user);

                TempData["Message"] = $"User {user.FullName} has been rejected. Reason: {reason}";
            }

            return RedirectToPage();
        }
    }
}
