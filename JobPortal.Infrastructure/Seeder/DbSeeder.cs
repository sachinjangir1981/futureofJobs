using Microsoft.AspNetCore.Identity;
using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;

namespace JobPortal.Infrastructure.Seeder;

public class DbSeeder
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _um;
    private readonly RoleManager<IdentityRole<Guid>> _rm;

    public DbSeeder(AppDbContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole<Guid>> rm)
    {
        _db = db;
        _um = um;
        _rm = rm;
    }

    public async Task SeedAsync()
    {
        await _db.Database.EnsureCreatedAsync();

        var roles = new[] { "Admin", "Employer", "JobSeeker" };
        foreach (var r in roles)
        {
            if (!await _rm.RoleExistsAsync(r))
                await _rm.CreateAsync(new IdentityRole<Guid>(r));
        }

        // admin
        var adminEmail = "admin@jobportal.local";
        if (await _um.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await _um.CreateAsync(admin, "Admin@1234");
            await _um.AddToRoleAsync(admin, "Admin");
        }

        // sample employers and seekers
        var empEmail = "acme.hr@jobportal.local";
        var emp = await _um.FindByEmailAsync(empEmail);
        if (emp == null)
        {
            emp = new ApplicationUser { UserName = empEmail, Email = empEmail, EmailConfirmed = true };
            await _um.CreateAsync(emp, "Employer@1234");
            await _um.AddToRoleAsync(emp, "Employer");
        }

        var seekerEmail = "alice@jobportal.local";
        var seeker = await _um.FindByEmailAsync(seekerEmail);
        if (seeker == null)
        {
            seeker = new ApplicationUser { UserName = seekerEmail, Email = seekerEmail, EmailConfirmed = true };
            await _um.CreateAsync(seeker, "Seeker@1234");
            await _um.AddToRoleAsync(seeker, "JobSeeker");
        }

        if (!_db.JobSeekerProfiles.Any())
        {
            _db.JobSeekerProfiles.Add(new JobSeekerProfile { UserId = seeker.Id, FullName = "Alice Johnson",   Experience = "3 years" });
            await _db.SaveChangesAsync();
        }

       

        if (!_db.JobApplications.Any())
        {
            var profile = _db.JobSeekerProfiles.First();
            var job = _db.Jobs.First();
            _db.JobApplications.Add(new JobApplication { JobId = job.Id, JobSeekerProfileId = profile.Id });
        }

        await _db.SaveChangesAsync();
    }
}
