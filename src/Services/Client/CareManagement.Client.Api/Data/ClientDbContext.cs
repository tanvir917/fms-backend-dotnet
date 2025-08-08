using Microsoft.EntityFrameworkCore;
using CareManagement.Client.Api.Models;

namespace CareManagement.Client.Api.Data;

public class ClientDbContext : DbContext
{
    public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Client> Clients { get; set; }
    public DbSet<CarePlan> CarePlans { get; set; }
    public DbSet<ClientDocument> ClientDocuments { get; set; }
    public DbSet<ClientNote> ClientNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Client entity
        modelBuilder.Entity<Models.Client>(entity =>
        {
            entity.ToTable("clients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.FirstName).HasColumnName("first_name").IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").IsRequired();
            entity.Property(e => e.MiddleName).HasColumnName("middle_name");
            entity.Property(e => e.PreferredName).HasColumnName("preferred_name");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.ZipCode).HasColumnName("zip_code");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.EmergencyContactName).HasColumnName("emergency_contact_name");
            entity.Property(e => e.EmergencyContactPhone).HasColumnName("emergency_contact_phone");
            entity.Property(e => e.EmergencyContactRelationship).HasColumnName("emergency_contact_relationship");
            entity.Property(e => e.MedicaidNumber).HasColumnName("medicaid_number");
            entity.Property(e => e.MedicareNumber).HasColumnName("medicare_number");
            entity.Property(e => e.SocialSecurityNumber).HasColumnName("social_security_number");
            entity.Property(e => e.CareLevel).HasColumnName("care_level").HasConversion<string>();
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
            entity.Property(e => e.AdmissionDate).HasColumnName("admission_date");
            entity.Property(e => e.DischargeDate).HasColumnName("discharge_date");
            entity.Property(e => e.MedicalConditions).HasColumnName("medical_conditions");
            entity.Property(e => e.Medications).HasColumnName("medications");
            entity.Property(e => e.Allergies).HasColumnName("allergies");
            entity.Property(e => e.SpecialInstructions).HasColumnName("special_instructions");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            // Indexes
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.SocialSecurityNumber).IsUnique();
            entity.HasIndex(e => new { e.FirstName, e.LastName });
        });

        // Configure CarePlan entity
        modelBuilder.Entity<CarePlan>(entity =>
        {
            entity.ToTable("care_plans");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Title).HasColumnName("title").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Goals).HasColumnName("goals");
            entity.Property(e => e.InterventionStrategies).HasColumnName("intervention_strategies");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.ReviewDate).HasColumnName("review_date");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            // Foreign key relationship
            entity.HasOne(e => e.Client)
                  .WithMany(c => c.CarePlans)
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ClientDocument entity
        modelBuilder.Entity<ClientDocument>(entity =>
        {
            entity.ToTable("client_documents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Title).HasColumnName("title").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DocumentType).HasColumnName("document_type").HasConversion<string>();
            entity.Property(e => e.FilePath).HasColumnName("file_path").IsRequired();
            entity.Property(e => e.FileName).HasColumnName("file_name").IsRequired();
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.ContentType).HasColumnName("content_type");
            entity.Property(e => e.UploadDate).HasColumnName("upload_date");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            // Foreign key relationship
            entity.HasOne(e => e.Client)
                  .WithMany(c => c.Documents)
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ClientNote entity
        modelBuilder.Entity<ClientNote>(entity =>
        {
            entity.ToTable("client_notes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Title).HasColumnName("title").IsRequired();
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.NoteType).HasColumnName("note_type").HasConversion<string>();
            entity.Property(e => e.NoteDate).HasColumnName("note_date");
            entity.Property(e => e.IsPrivate).HasColumnName("is_private");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            // Foreign key relationship
            entity.HasOne(e => e.Client)
                  .WithMany(c => c.ClientNotes)
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Models.Client || e.Entity is CarePlan ||
                       e.Entity is ClientDocument || e.Entity is ClientNote);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                if (entityEntry.Entity is Models.Client client)
                {
                    client.CreatedAt = DateTime.UtcNow;
                    client.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is CarePlan carePlan)
                {
                    carePlan.CreatedAt = DateTime.UtcNow;
                    carePlan.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is ClientDocument document)
                {
                    document.CreatedAt = DateTime.UtcNow;
                    document.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is ClientNote note)
                {
                    note.CreatedAt = DateTime.UtcNow;
                    note.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                if (entityEntry.Entity is Models.Client client)
                {
                    client.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is CarePlan carePlan)
                {
                    carePlan.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is ClientDocument document)
                {
                    document.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is ClientNote note)
                {
                    note.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Models.Client || e.Entity is CarePlan ||
                       e.Entity is ClientDocument || e.Entity is ClientNote);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                if (entityEntry.Entity is Models.Client client)
                {
                    client.CreatedAt = DateTime.UtcNow;
                    client.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is CarePlan carePlan)
                {
                    carePlan.CreatedAt = DateTime.UtcNow;
                    carePlan.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is ClientDocument document)
                {
                    document.CreatedAt = DateTime.UtcNow;
                    document.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is ClientNote note)
                {
                    note.CreatedAt = DateTime.UtcNow;
                    note.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                if (entityEntry.Entity is Models.Client client)
                {
                    client.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is CarePlan carePlan)
                {
                    carePlan.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is ClientDocument document)
                {
                    document.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is ClientNote note)
                {
                    note.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
