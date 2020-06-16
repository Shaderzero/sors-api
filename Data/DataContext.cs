using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using sors.Data.Entities;
using sors.Data.Entities.Incidents;
using sors.Data.Entities.Passports;
using sors.Data.Entities.References;

namespace sors.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DomainDepartment> DomainDepartments { get; set; }
        public DbSet<Draft> Drafts { get; set; }
        public DbSet<IncidentDraft> IncidentDrafts { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Measure> Measures { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Responsible> Responsibles { get; set; }
        public DbSet<ResponsibleAccount> ResponsibleAccounts { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<BusinessProcess> BusinessProcesses { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }
        public DbSet<RiskSignificance> RiskSignificances { get; set; }
        public DbSet<RiskArea> RiskAreas { get; set; }
        public DbSet<RiskDuration> RiskDurations { get; set; }
        public DbSet<RiskFactor> RiskFactors { get; set; }
        public DbSet<RiskManageability> RiskManageabilities { get; set; }
        public DbSet<RiskReaction> RiskReactions { get; set; }
        public DbSet<RiskStatus> RiskStatuses { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<TextData> TextDatas { get; set; }
        public DbSet<IncidentType> IncidentTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<TextData>()
                .HasIndex(i => new { i.Name, i.Param })
                .IsUnique();
            
            builder.Entity<AccountRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.AccountId, ur.RoleId });
                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.AccountRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
                userRole.HasOne(ur => ur.Account)
                    .WithMany(r => r.AccountRoles)
                    .HasForeignKey(ur => ur.AccountId)
                    .IsRequired();
            });

            builder.Entity<Draft>(d =>
            {
                d.HasOne(d => d.Department)
                    .WithMany(d => d.Drafts)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
                d.HasOne(d => d.IncidentType)
                    .WithMany(d => d.Drafts)
                    .HasForeignKey(d => d.IncidentTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Incident>(i =>
            {
                i.HasOne(d => d.IncidentType)
                    .WithMany(d => d.Incidents)
                    .HasForeignKey(d => d.IncidentTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<ResponsibleAccount>(e =>
            {
                e.HasKey(p => new { p.ResponsibleId, p.AccountId });
                e.HasOne(p => p.Account)
                    .WithMany(p => p.ResponsibleAccounts)
                    .HasForeignKey(p => p.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.Responsible)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(p => p.ResponsibleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<IncidentDraft>(e =>
            {
                e.HasKey(p => new { p.IncidentId, p.DraftId });
                e.HasOne(p => p.Draft)
                    .WithMany(p => p.IncidentDrafts)
                    .HasForeignKey(p => p.DraftId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(p => p.Incident)
                    .WithMany(p => p.Drafts)
                    .HasForeignKey(p => p.IncidentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<DraftProp>()
                .HasOne(u => u.Author)
                .WithMany(u => u.DraftProps)
                .HasForeignKey(u => u.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<IncidentProp>()
                .HasOne(u => u.Author)
                .WithMany(u => u.IncidentProps)
                .HasForeignKey(u => u.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ResponsibleProp>()
                .HasOne(u => u.Author)
                .WithMany(u => u.ResponsibleProps)
                .HasForeignKey(u => u.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MeasureProp>()
                .HasOne(u => u.Author)
                .WithMany(u => u.MeasureProps)
                .HasForeignKey(u => u.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReportProp>()
                .HasOne(u => u.Author)
                .WithMany(u => u.ReportProps)
                .HasForeignKey(u => u.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}