using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JobPortal.Domain.Models;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace JobPortal.Infrastructure.Data;

public class ApplicationUser : IdentityUser<Guid> { }

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobSeekerProfile> JobSeekerProfiles => Set<JobSeekerProfile>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<EmployerProfile> EmployerProfiles => Set<EmployerProfile>();
    public DbSet<Category> Categories { get; set; }
    public DbSet<UserLocation> Locations { get; set; }

    public DbSet<FormTypeCategory> FormTypeCategory => Set<FormTypeCategory>();
    public DbSet<FormSection> FormSections => Set<FormSection>();
    public DbSet<FormQuestion> FormQuestions => Set<FormQuestion>();
    public DbSet<FormOption> FormOptions => Set<FormOption>();
    public DbSet<FormAnswer> FormAnswers => Set<FormAnswer>();

    public DbSet<QuestionRating> QuestionRatings { get; set; }

    public DbSet<ProfileRating> ProfileRatings { get; set; }

    public DbSet<State> States { get; set; }
    public DbSet<City> Cities { get; set; }

    public DbSet<SkillCategory> SkillCategories { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;

    // Join tables (explicit many-to-many)
    public DbSet<JobSkill> JobSkills { get; set; } = null!;
    public DbSet<JobSeekerSkill> JobSeekerSkills { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Job>(e =>
        {
            e.HasKey(j => j.Id);
            e.Property(j => j.Title).HasMaxLength(200).IsRequired();
            e.HasIndex(j => j.Slug).IsUnique();
            e.Property(j => j.Description).HasColumnType("nvarchar(max)");
        });

        builder.Entity<JobSeekerProfile>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.UserId).IsUnique();
        });

        builder.Entity<JobApplication>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.Job).WithMany().HasForeignKey(a => a.JobId);
            e.HasOne(a => a.JobSeekerProfile).WithMany().HasForeignKey(a => a.JobSeekerProfileId);
        });

        builder.Entity<EmployerProfile>(e => { e.HasKey(x => x.Id); e.HasIndex(x => x.UserId).IsUnique(); });

        builder.Entity<FormSection>(e =>
        {
            e.HasKey(e => e.Id);
            e.HasMany(s => s.Questions).WithOne(q => q.Section).HasForeignKey(q => q.SectionId).OnDelete(DeleteBehavior.Cascade);
        });

       
            

        builder.Entity<FormQuestion>()
            .HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FormQuestion>()
            .HasMany(q => q.QuestionRatings)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FormAnswer>()
            .HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<QuestionRating>()
           .HasOne(qr => qr.Question)
           .WithMany(q => q.QuestionRatings)
           .HasForeignKey(qr => qr.QuestionId)
           .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<State>()
             .HasMany(s => s.Cities)
             .WithOne(c => c.State)
             .HasForeignKey(c => c.StateId)
             .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<JobSeekerProfile>()
            .HasOne(p => p.CurrentState)
            .WithMany()
            .HasForeignKey(p => p.CurrentStateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobSeekerProfile>()
            .HasOne(p => p.CurrentCity)
            .WithMany()
            .HasForeignKey(p => p.CurrentCityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobSeekerProfile>()
            .HasOne(p => p.DesiredState)
            .WithMany()
            .HasForeignKey(p => p.DesiredStateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobSeekerProfile>()
            .HasOne(p => p.DesiredCity)
            .WithMany()
            .HasForeignKey(p => p.DesiredCityId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- Skills & categories ----------
        builder.Entity<SkillCategory>()
            .HasMany(sc => sc.Skills)
            .WithOne(s => s.SkillCategory)
            .HasForeignKey(s => s.SkillCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // optional: unique index on skill name per category (or global)
        builder.Entity<Skill>()
            .HasIndex(s => new { s.Name, s.SkillCategoryId })
            .IsUnique();

        // ---------- JobSkill (Job <-> Skill) ----------
        builder.Entity<JobSkill>()
            .HasKey(js => new { js.JobId, js.SkillId });

        builder.Entity<JobSkill>()
            .HasOne(js => js.Job)
            .WithMany(j => j.JobSkills)
            .HasForeignKey(js => js.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<JobSkill>()
            .HasOne(js => js.Skill)
            .WithMany(s => s.JobSkills)
            .HasForeignKey(js => js.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- JobSeekerSkill (Profile <-> Skill) ----------
        builder.Entity<JobSeekerSkill>()
            .HasKey(js => new { js.JobSeekerProfileId, js.SkillId });

        builder.Entity<JobSeekerSkill>()
            .HasOne(js => js.JobSeekerProfile)
            .WithMany(p => p.JobSeekerSkills)
            .HasForeignKey(js => js.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<JobSeekerSkill>()
            .HasOne(js => js.Skill)
            .WithMany(s => s.JobSeekerSkills)
            .HasForeignKey(js => js.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- Job (State/City) relationships example (if present) ----------
        builder.Entity<Job>()
            .HasOne(j => j.State)
            .WithMany()
            .HasForeignKey(j => j.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Job>()
            .HasOne(j => j.City)
            .WithMany()
            .HasForeignKey(j => j.CityId)
            .OnDelete(DeleteBehavior.Restrict);

    } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
    }
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // 👇 Use your SQL Server connection string here
        optionsBuilder.UseSqlServer(
            "Data Source=plesk8700.is.cc;Initial Catalog=spavitec_greenjobs;User Id=ucards; Password=0mdFx251#;TrustServerCertificate=True;Encrypt=true;");

        return new AppDbContext(optionsBuilder.Options);
    }
}