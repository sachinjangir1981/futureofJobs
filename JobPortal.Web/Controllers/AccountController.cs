using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using JobPortal.Infrastructure.Data;
using JobPortal.Web.ViewModels;

namespace JobPortal.Web.Controllers;
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _um;
    private readonly SignInManager<ApplicationUser> _sm;
    private readonly AppDbContext _db;

    public AccountController(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm, AppDbContext db)
    {
        _um = um; _sm = sm; _db = db;
    }

    public IActionResult Register(int? id)
    {
        ViewBag.RoleId = id.HasValue ? id.Value :0;
        RegisterModel register = new RegisterModel();
        return View(register);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel register)
    {
       
        string role2 = register.Role,  name= register.Name,  phone= register.Phone,  email= register.Email,  password = register.Password;

        string role = "Student/JobSeeker";

        if (role2 == "1")
        {
            role = "Student/JobSeeker";
        }
        else if (role2 == "2")
        {
            role = "Employer";
        }
        else if (role2 == "3")
        {
            role = "Academics";
        }
        else if (role2 == "4")
        {
            role = "Intern";
        }
        else if (role2 == "5")
        {
            role = "Mid-Career";
        }
        else if (role2 == "6")
        {
            role = "Silver-Talent";
        }
        else
            role = "Student/JobSeeker";

        var user = new ApplicationUser { UserName = email, Email = email };
        var r = await _um.CreateAsync(user, password);
        string error = "";
        if (!r.Succeeded)
        {
            
            foreach (var e in r.Errors)
                ModelState.AddModelError("", e.Description);

            return View(register);
        }
         
         await _um.AddClaimAsync(user, new Claim("AccountType", role));
        if ((await _um.GetRolesAsync(user)).Count > 0)
        {
            await _um.RemoveFromRoleAsync(user, role);
        }
        await _um.AddToRoleAsync(user, role);


       await _sm.SignInAsync(user, false);

       
        int profileId = 0;
        Guid userId = Guid.Parse(user.Id.ToString());
        if (role == "Employer")
        {
            if (!_db.EmployerProfiles.Any(p => p.UserId == userId))
            {
               var emp = _db.EmployerProfiles.Add(new Domain.Models.EmployerProfile { UserId = userId, Email=register.Email, FullName = register.Name, Phone = register.Phone, RecType=1 });
                await _db.SaveChangesAsync();
                profileId = emp.Entity.Id;
            }
            return RedirectToAction("MyAccount", "Home");
            //  return RedirectToAction("Edit", "Profile", new { area = "Employer", id = profileId });
        }
        else if (role == "Student/JobSeeker" || role== "Mid-Career" || role== "Intern" || role == "Silver-Talent")
        {
            
            if (!_db.JobSeekerProfiles.Any(p => p.UserId == userId))
            {
               var stu =  _db.JobSeekerProfiles.Add(new Domain.Models.JobSeekerProfile { UserId = userId,  FullName = register.Name, Phone=register.Phone, ProfileName ="My Profile 1", RcdInsTs= DateTime.UtcNow, RcdUpdtTs=DateTime.UtcNow });
                await _db.SaveChangesAsync();
                profileId = stu.Entity.Id;
            }
            return RedirectToAction("MyAccount", "Home");
            //  return RedirectToAction("Edit", "Profile", new { area = "JobSeeker", id=profileId });
        }
        else if (role == "Academics")
        {

            if (!_db.EmployerProfiles.Any(p => p.UserId == userId))
            {
                var emp = _db.EmployerProfiles.Add(new Domain.Models.EmployerProfile { UserId = userId, Email = register.Email, FullName = register.Name, Phone = register.Phone, RecType = 2 });
                await _db.SaveChangesAsync();
                profileId = emp.Entity.Id;
            }
            return RedirectToAction("MyAccount", "Home");
            // return RedirectToAction("Edit", "Profile", new { area = "Academics", id = profileId });
        }


        return View();
    }

    public IActionResult Login(string returnUrl = null) {
        return View();
        //return RedirectToAction("Index", "Home", new { returnUrl =returnUrl});
    } 
     

    [HttpGet]
    public IActionResult ChooseRole() => View();

    [HttpPost]
    public async Task<IActionResult> ChooseRole(string role)
    {
        var user = await _um.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");
        var existing = (await _um.GetClaimsAsync(user)).FirstOrDefault(c => c.Type == "AccountType");
        if (existing != null) await _um.RemoveClaimAsync(user, existing);
        await _um.AddClaimAsync(user, new Claim("AccountType", role));
        if ((await _um.GetRolesAsync(user)).Count > 0)
        {
            await _um.RemoveFromRoleAsync(user, role);
        }
        await _um.AddToRoleAsync(user, role);
        // Refresh the sign-in cookie
        await _sm.RefreshSignInAsync(user);

        Guid userId = Guid.Parse(user.Id.ToString());
        if (role == "Employer")
        {
            int profileId = 0;
            if (!_db.EmployerProfiles.Any(p => p.UserId == userId))
            {
               var emp =  _db.EmployerProfiles.Add(new JobPortal.Domain.Models.EmployerProfile { UserId = userId });
               
                await _db.SaveChangesAsync();
                profileId = emp.Entity.Id;
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Employer"});
        }
        else if (role == "Student/JobSeeker")
        {
            var profileId = 0;
            if (!_db.JobSeekerProfiles.Any(p => p.UserId == userId))
            {
               
                var student  = _db.JobSeekerProfiles.Add(new JobPortal.Domain.Models.JobSeekerProfile { UserId = userId });
                await _db.SaveChangesAsync();
                profileId = student.Entity.Id;
            }
            return RedirectToAction("Edit", "Profile", new { area = "JobSeeker", id = profileId });
        }

        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> Logout()
    {
        await _sm.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Register",model);
        }

        var result = await _sm.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            var user = await _um.FindByEmailAsync(model.Email);
            var claims = await _um.GetClaimsAsync(user);
            var accountType = claims.FirstOrDefault(c => c.Type == "AccountType")?.Value;
            var lst = await _um.GetRolesAsync(user);


            if (accountType == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            else  
                return RedirectToAction("MyAccount", "Home");

            //if (accountType == "Employer")
            //    return RedirectToAction("Dashboard", "Employer");
            //else if (accountType == "JobSeeker")
            //    return RedirectToAction("Dashboard", "JobSeeker");
            //else if (accountType == "Admin")
            //    return RedirectToAction("Dashboard", "Admin");
            //else if (accountType == "Academics")
            //    return RedirectToAction("Dashboard", "Academics");
            //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            //    return Redirect(returnUrl);

            //return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid login attempt.");
        return RedirectToAction("Register", model); // View(model); // For popup we’ll inject error
    }
}
