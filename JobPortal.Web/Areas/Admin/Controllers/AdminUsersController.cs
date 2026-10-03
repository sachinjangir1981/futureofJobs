using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using JobPortal.Web.AdminAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    // Access to this controller is Super Admin only (see AdminSections.ForController).
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private const string AdminRole = "Admin";
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _um;
        private readonly RoleManager<IdentityRole<Guid>> _roles;

        public AdminUsersController(AppDbContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole<Guid>> roles)
        {
            _db = db;
            _um = um;
            _roles = roles;
        }

        public IActionResult Index() => RedirectToAction(nameof(SubRoles));

        public async Task<IActionResult> SubRoles()
        {
            var roles = await _db.AdminSubRoles
                .Include(r => r.Permissions)
                .Include(r => r.Forms)
                .OrderBy(r => r.Name)
                .ToListAsync();

            ViewBag.UserCounts = await _db.AdminUserSubRoles
                .GroupBy(u => u.SubRoleId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            return View(roles);
        }

        public async Task<IActionResult> EditSubRole(int? id)
        {
            var vm = new SubRoleEditVm();
            if (id.HasValue)
            {
                var role = await _db.AdminSubRoles
                    .Include(r => r.Permissions)
                    .Include(r => r.Forms)
                    .FirstOrDefaultAsync(r => r.Id == id.Value);
                if (role == null) return NotFound();

                vm = new SubRoleEditVm
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    AllFormsAccess = role.AllFormsAccess,
                    Levels = role.Permissions.ToDictionary(p => p.Section, p => p.AccessLevel),
                    FormIds = role.Forms.Select(f => f.FormTypeCategoryId).ToList()
                };
            }

            await LoadFormsAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSubRole(SubRoleEditVm vm)
        {
            vm.Name = (vm.Name ?? "").Trim();
            if (vm.Name.Length == 0)
                ModelState.AddModelError(nameof(vm.Name), "Name is required.");
            else if (await _db.AdminSubRoles.AnyAsync(r => r.Name == vm.Name && r.Id != vm.Id))
                ModelState.AddModelError(nameof(vm.Name), "A sub-role with this name already exists.");

            if (!vm.AllFormsAccess && vm.FormIds.Count == 0)
                ModelState.AddModelError(nameof(vm.FormIds), "Pick at least one form, or choose All forms.");

            if (!ModelState.IsValid)
            {
                await LoadFormsAsync();
                return View(vm);
            }

            AdminSubRole role;
            if (vm.Id > 0)
            {
                role = await _db.AdminSubRoles
                    .Include(r => r.Permissions)
                    .Include(r => r.Forms)
                    .FirstOrDefaultAsync(r => r.Id == vm.Id) ?? throw new InvalidOperationException("Sub-role not found.");
                _db.AdminSubRolePermissions.RemoveRange(role.Permissions);
                _db.AdminSubRoleForms.RemoveRange(role.Forms);
            }
            else
            {
                role = new AdminSubRole();
                _db.AdminSubRoles.Add(role);
            }

            role.Name = vm.Name;
            role.Description = string.IsNullOrWhiteSpace(vm.Description) ? null : vm.Description.Trim();
            role.IsActive = vm.IsActive;
            role.AllFormsAccess = vm.AllFormsAccess;

            role.Permissions = AdminSections.Grantable
                .Where(s => vm.Levels.TryGetValue(s.Key, out var l) && (l == AdminSections.View || l == AdminSections.Full))
                .Select(s => new AdminSubRolePermission { Section = s.Key, AccessLevel = vm.Levels[s.Key] })
                .ToList();

            role.Forms = vm.AllFormsAccess
                ? new List<AdminSubRoleForm>()
                : vm.FormIds.Distinct().Select(id => new AdminSubRoleForm { FormTypeCategoryId = id }).ToList();

            await _db.SaveChangesAsync();
            TempData["Message"] = $"Sub-role \"{role.Name}\" saved.";
            return RedirectToAction(nameof(SubRoles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubRole(int id)
        {
            var role = await _db.AdminSubRoles.FindAsync(id);
            if (role == null) return RedirectToAction(nameof(SubRoles));

            if (await _db.AdminUserSubRoles.AnyAsync(u => u.SubRoleId == id))
            {
                TempData["Error"] = "This sub-role still has admins assigned. Reassign them first.";
                return RedirectToAction(nameof(SubRoles));
            }

            _db.AdminSubRoles.Remove(role);
            await _db.SaveChangesAsync();
            TempData["Message"] = $"Sub-role \"{role.Name}\" deleted.";
            return RedirectToAction(nameof(SubRoles));
        }

        public async Task<IActionResult> Users()
        {
            var admins = (await _um.GetUsersInRoleAsync(AdminRole)).OrderBy(u => u.Email ?? u.UserName).ToList();
            ViewBag.Assignments = await _db.AdminUserSubRoles.Include(a => a.Users).ToDictionaryAsync(a => a.UserId);
            ViewBag.SubRoles = await _db.AdminSubRoles.OrderBy(r => r.Name).ToListAsync();
            ViewBag.CurrentUserId = _um.GetUserId(User);
            return View(admins);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignSubRole(string userId, int subRoleId, string? validUntil)
        {
            if (userId == _um.GetUserId(User))
            {
                TempData["Error"] = "You can't change your own access.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _um.FindByIdAsync(userId);
            if (user == null || !await _um.IsInRoleAsync(user, AdminRole))
            {
                TempData["Error"] = "That user isn't an admin.";
                return RedirectToAction(nameof(Users));
            }

            if (subRoleId != 0 && !await _db.AdminSubRoles.AnyAsync(r => r.Id == subRoleId))
            {
                TempData["Error"] = "Unknown sub-role.";
                return RedirectToAction(nameof(Users));
            }

            if (subRoleId != 0 && await IsOnlySuperAdminAsync(userId))
            {
                TempData["Error"] = "At least one Super Admin must remain.";
                return RedirectToAction(nameof(Users));
            }

            if (!TryGetValidUntil(subRoleId, validUntil, out var until, out var untilError))
            {
                TempData["Error"] = untilError;
                return RedirectToAction(nameof(Users));
            }

            await SetAssignmentAsync(userId, subRoleId, until);
            TempData["Message"] = $"Updated access for {user.Email ?? user.UserName}.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmin(string email, int subRoleId, string? validUntil)
        {
            email = (email ?? "").Trim();
            var user = await _um.FindByEmailAsync(email) ?? await _um.FindByNameAsync(email);
            if (user == null)
            {
                TempData["Error"] = "No registered user with that email. They need to sign up first.";
                return RedirectToAction(nameof(Users));
            }

            if (subRoleId != 0 && !await _db.AdminSubRoles.AnyAsync(r => r.Id == subRoleId))
            {
                TempData["Error"] = "Unknown sub-role.";
                return RedirectToAction(nameof(Users));
            }

            if (!TryGetValidUntil(subRoleId, validUntil, out var until, out var untilError))
            {
                TempData["Error"] = untilError;
                return RedirectToAction(nameof(Users));
            }

            // Admin needs both the Identity role and the AccountType claim, which login uses to send admins to the admin area.
            if (!await _roles.RoleExistsAsync(AdminRole))
                await _roles.CreateAsync(new IdentityRole<Guid>(AdminRole));

            if (!await _um.IsInRoleAsync(user, AdminRole))
            {
                var result = await _um.AddToRoleAsync(user, AdminRole);
                if (!result.Succeeded)
                {
                    TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
                    return RedirectToAction(nameof(Users));
                }
            }

            var accountType = (await _um.GetClaimsAsync(user)).FirstOrDefault(c => c.Type == "AccountType");
            if (accountType == null)
                await _um.AddClaimAsync(user, new System.Security.Claims.Claim("AccountType", AdminRole));
            else if (accountType.Value != AdminRole)
                await _um.ReplaceClaimAsync(user, accountType, new System.Security.Claims.Claim("AccountType", AdminRole));

            await SetAssignmentAsync(user.Id.ToString(), subRoleId, until);
            TempData["Message"] = $"{user.Email ?? user.UserName} is now an admin.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            if (userId == _um.GetUserId(User))
            {
                TempData["Error"] = "You can't remove your own admin access.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _um.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction(nameof(Users));

            if (await IsOnlySuperAdminAsync(userId))
            {
                TempData["Error"] = "At least one Super Admin must remain.";
                return RedirectToAction(nameof(Users));
            }

            await _um.RemoveFromRoleAsync(user, AdminRole);
            var adminClaim = (await _um.GetClaimsAsync(user)).FirstOrDefault(c => c.Type == "AccountType" && c.Value == AdminRole);
            if (adminClaim != null)
                await _um.RemoveClaimAsync(user, adminClaim);
            await SetAssignmentAsync(userId, 0, null);
            TempData["Message"] = $"Removed admin access for {user.Email ?? user.UserName}.";
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> VisibleUsers(string userId)
        {
            var admin = await _um.FindByIdAsync(userId);
            var assignment = await _db.AdminUserSubRoles.Include(a => a.Users).FirstOrDefaultAsync(a => a.UserId == userId);
            if (admin == null || assignment == null)
            {
                TempData["Error"] = "Assign a sub-role first. Super Admins always see every user.";
                return RedirectToAction(nameof(Users));
            }

            var vm = new VisibleUsersVm
            {
                AdminUserId = userId,
                AdminEmail = admin.Email ?? admin.UserName ?? "",
                AllUsers = assignment.AllUsersAccess,
                SelectedUserIds = assignment.Users.Select(u => u.TargetUserId).ToList(),
                Candidates = await LoadCandidateUsersAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VisibleUsers(string userId, bool allUsers, List<string>? selected)
        {
            var assignment = await _db.AdminUserSubRoles.Include(a => a.Users).FirstOrDefaultAsync(a => a.UserId == userId);
            if (assignment == null)
                return RedirectToAction(nameof(Users));

            selected = (selected ?? new List<string>()).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (!allUsers && selected.Count == 0)
            {
                TempData["Error"] = "Pick at least one user, or choose All users.";
                return RedirectToAction(nameof(VisibleUsers), new { userId });
            }

            _db.AdminUserSubRoleUsers.RemoveRange(assignment.Users);
            assignment.AllUsersAccess = allUsers;
            assignment.Users = allUsers
                ? new List<AdminUserSubRoleUser>()
                : selected.Select(id => new AdminUserSubRoleUser { TargetUserId = id.ToLowerInvariant() }).ToList();

            await _db.SaveChangesAsync();
            TempData["Message"] = "Visible users updated.";
            return RedirectToAction(nameof(Users));
        }

        // Users who have submitted (or started) at least one form, since those are the ones with data to show.
        private async Task<List<VisibleUserOption>> LoadCandidateUsersAsync()
        {
            var ids = await _db.FormAnswers.Select(a => a.UserId).Distinct().ToListAsync();
            return (await _db.Users.Where(u => ids.Contains(u.Id.ToString())).ToListAsync())
                .Select(u => new VisibleUserOption { Id = u.Id.ToString(), Label = u.Email ?? u.UserName ?? u.Id.ToString() })
                .OrderBy(u => u.Label)
                .ToList();
        }

        // A sub-role assignment must end at a chosen future date and time; Super Admin (0) has no end.
        private static bool TryGetValidUntil(int subRoleId, string? input, out DateTime? utc, out string? error)
        {
            utc = null;
            error = null;
            if (subRoleId == 0)
                return true;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Choose an end date and time for the sub-role.";
                return false;
            }
            if (!AdminTime.TryParseInput(input, out utc) || utc == null)
            {
                error = "That end date and time isn't valid.";
                return false;
            }
            if (utc <= DateTime.UtcNow)
            {
                error = "The end date and time must be in the future.";
                return false;
            }
            return true;
        }

        // True when this user is a Super Admin and no other admin is.
        private async Task<bool> IsOnlySuperAdminAsync(string userId)
        {
            var assigned = (await _db.AdminUserSubRoles.Select(a => a.UserId).ToListAsync()).ToHashSet();
            var supers = (await _um.GetUsersInRoleAsync(AdminRole))
                .Select(u => u.Id.ToString())
                .Where(id => !assigned.Contains(id))
                .ToList();
            return supers.Count == 1 && supers[0] == userId;
        }

        // subRoleId 0 means Super Admin, which is stored as no assignment.
        private async Task SetAssignmentAsync(string userId, int subRoleId, DateTime? validUntil)
        {
            var existing = await _db.AdminUserSubRoles.FirstOrDefaultAsync(a => a.UserId == userId);
            if (subRoleId == 0)
            {
                if (existing != null) _db.AdminUserSubRoles.Remove(existing);
            }
            else if (existing != null)
            {
                existing.SubRoleId = subRoleId;
                existing.ValidUntil = validUntil!.Value;
            }
            else
            {
                _db.AdminUserSubRoles.Add(new AdminUserSubRole { UserId = userId, SubRoleId = subRoleId, ValidUntil = validUntil!.Value });
            }

            await _db.SaveChangesAsync();
        }

        private async Task LoadFormsAsync()
        {
            ViewBag.Forms = await _db.FormTypeCategory
                .Where(f => f.Id > 5)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }
    }

    public class VisibleUserOption
    {
        public string Id { get; set; } = "";
        public string Label { get; set; } = "";
    }

    public class VisibleUsersVm
    {
        public string AdminUserId { get; set; } = "";
        public string AdminEmail { get; set; } = "";
        public bool AllUsers { get; set; } = true;
        public List<string> SelectedUserIds { get; set; } = new();
        public List<VisibleUserOption> Candidates { get; set; } = new();
    }

    public class SubRoleEditVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool AllFormsAccess { get; set; } = true;
        public Dictionary<string, int> Levels { get; set; } = new();
        public List<int> FormIds { get; set; } = new();
    }
}
