using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CareManagement.Staff.Api.Data;
using CareManagement.Staff.Api.DTOs;

namespace CareManagement.Staff.Api.Services;

public class StaffDocumentService : IStaffDocumentService
{
    private readonly StaffDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<StaffDocumentService> _logger;

    public StaffDocumentService(
        StaffDbContext context,
        IMapper mapper,
        IWebHostEnvironment environment,
        ILogger<StaffDocumentService> logger)
    {
        _context = context;
        _mapper = mapper;
        _environment = environment;
        _logger = logger;
    }

    public async Task<IEnumerable<StaffDocumentDto>> GetDocumentsByStaffIdAsync(int staffId)
    {
        var documents = await _context.StaffDocuments
            .Include(d => d.Staff)
            .Where(d => d.StaffId == staffId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

        return documents.Select(d => new StaffDocumentDto
        {
            Id = d.Id,
            StaffId = d.StaffId,
            Title = d.Title,
            Description = d.Description,
            DocumentType = d.DocumentType,
            FileName = d.FileName,
            MimeType = d.MimeType,
            FileSize = d.FileSize,
            FormattedFileSize = FormatFileSize(d.FileSize),
            UploadedAt = d.UploadedAt,
            UploadedBy = d.UploadedBy,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        });
    }

    public async Task<StaffDocumentDto?> GetDocumentByIdAsync(int id)
    {
        var document = await _context.StaffDocuments
            .Include(d => d.Staff)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
            return null;

        return new StaffDocumentDto
        {
            Id = document.Id,
            StaffId = document.StaffId,
            Title = document.Title,
            Description = document.Description,
            DocumentType = document.DocumentType,
            FileName = document.FileName,
            MimeType = document.MimeType,
            FileSize = document.FileSize,
            FormattedFileSize = FormatFileSize(document.FileSize),
            UploadedAt = document.UploadedAt,
            UploadedBy = document.UploadedBy,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    public async Task<StaffDocumentDto> CreateDocumentAsync(StaffDocumentUploadDto uploadDto, string? createdBy = null)
    {
        // Validate file
        if (uploadDto.File == null || uploadDto.File.Length == 0)
        {
            throw new ArgumentException("File is required");
        }

        // Validate staff exists
        var staffExists = await _context.StaffMembers.AnyAsync(s => s.Id == uploadDto.StaffId);
        if (!staffExists)
        {
            throw new ArgumentException("Staff member not found");
        }

        // Create document record
        var document = new StaffDocument
        {
            StaffId = uploadDto.StaffId,
            Title = uploadDto.Title,
            Description = uploadDto.Description,
            DocumentType = uploadDto.DocumentType,
            FileName = uploadDto.File.FileName,
            MimeType = uploadDto.File.ContentType ?? "application/octet-stream",
            FileSize = uploadDto.File.Length,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = int.TryParse(createdBy, out var userId) ? userId : 0
        };

        // Save file
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads", "staff-documents");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{uploadDto.File.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await uploadDto.File.CopyToAsync(stream);
        }

        document.FilePath = filePath;

        _context.StaffDocuments.Add(document);
        await _context.SaveChangesAsync();

        // Reload with staff data
        await _context.Entry(document)
            .Reference(d => d.Staff)
            .LoadAsync();

        _logger.LogInformation("Document uploaded for staff {StaffId}: {DocumentId}", uploadDto.StaffId, document.Id);

        return new StaffDocumentDto
        {
            Id = document.Id,
            StaffId = document.StaffId,
            Title = document.Title,
            Description = document.Description,
            DocumentType = document.DocumentType,
            FileName = document.FileName,
            MimeType = document.MimeType,
            FileSize = document.FileSize,
            FormattedFileSize = FormatFileSize(document.FileSize),
            UploadedAt = document.UploadedAt,
            UploadedBy = document.UploadedBy,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    public async Task<StaffDocumentDto?> UpdateDocumentAsync(int id, UpdateStaffDocumentDto updateDto, string? updatedBy = null)
    {
        var document = await _context.StaffDocuments.FindAsync(id);
        if (document == null)
            return null;

        // Update properties if provided
        if (!string.IsNullOrWhiteSpace(updateDto.Title))
            document.Title = updateDto.Title;

        if (updateDto.Description != null)
            document.Description = updateDto.Description;

        if (updateDto.DocumentType.HasValue)
            document.DocumentType = updateDto.DocumentType.Value;

        document.UpdatedAt = DateTime.UtcNow;
        if (int.TryParse(updatedBy, out var userId))
        {
            document.UploadedBy = userId; // Using UploadedBy as UpdatedBy since we don't have a separate field
        }

        await _context.SaveChangesAsync();

        return new StaffDocumentDto
        {
            Id = document.Id,
            StaffId = document.StaffId,
            Title = document.Title,
            Description = document.Description,
            DocumentType = document.DocumentType,
            FileName = document.FileName,
            MimeType = document.MimeType,
            FileSize = document.FileSize,
            FormattedFileSize = FormatFileSize(document.FileSize),
            UploadedAt = document.UploadedAt,
            UploadedBy = document.UploadedBy,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    public async Task<bool> DeleteDocumentAsync(int id)
    {
        var document = await _context.StaffDocuments.FindAsync(id);
        if (document == null)
            return false;

        // Delete physical file
        try
        {
            if (File.Exists(document.FilePath))
            {
                File.Delete(document.FilePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete physical file: {FilePath}", document.FilePath);
        }

        _context.StaffDocuments.Remove(document);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Document deleted: {DocumentId}", id);
        return true;
    }

    public async Task<(byte[] Content, string ContentType, string FileName)?> DownloadDocumentAsync(int id)
    {
        var document = await _context.StaffDocuments.FindAsync(id);
        if (document == null || !File.Exists(document.FilePath))
        {
            return null;
        }

        var content = await File.ReadAllBytesAsync(document.FilePath);
        return (content, document.MimeType, document.FileName);
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
