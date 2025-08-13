using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CareManagement.Client.Api.Data;
using CareManagement.Client.Api.DTOs;
using CareManagement.Client.Api.Models;
using CareManagement.Client.Api.Services.Interfaces;
using CareManagement.Shared.Messaging;
using CareManagement.Shared.Events;

namespace CareManagement.Client.Api.Services.Implementations;

public class ClientDocumentService : IClientDocumentService
{
    private readonly ClientDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ClientDocumentService> _logger;

    public ClientDocumentService(
        ClientDbContext context,
        IMapper mapper,
        IWebHostEnvironment environment,
        IConfiguration configuration,
        IMessageBus messageBus,
        ILogger<ClientDocumentService> logger)
    {
        _context = context;
        _mapper = mapper;
        _environment = environment;
        _configuration = configuration;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<IEnumerable<ClientDocumentDto>> GetDocumentsByClientIdAsync(int clientId)
    {
        var documents = await _context.ClientDocuments
            .Include(d => d.Client)
            .Where(d => d.ClientId == clientId)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ClientDocumentDto>>(documents);
    }

    public async Task<ClientDocumentDto?> GetDocumentByIdAsync(int id)
    {
        var document = await _context.ClientDocuments
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id == id);

        return document == null ? null : _mapper.Map<ClientDocumentDto>(document);
    }

    public async Task<ClientDocumentDto> CreateDocumentAsync(DocumentUploadDto uploadDto, string? createdBy = null)
    {
        // Validate file
        if (uploadDto.File == null || uploadDto.File.Length == 0)
        {
            throw new ArgumentException("File is required");
        }

        // Create document record
        var document = new ClientDocument
        {
            ClientId = uploadDto.ClientId,
            Title = uploadDto.Title,
            Description = uploadDto.Description,
            DocumentType = uploadDto.DocumentType,
            FileName = uploadDto.File.FileName,
            ContentType = uploadDto.File.ContentType,
            FileSize = FormatFileSize(uploadDto.File.Length),
            UploadDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        // Save file
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads", "documents");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{uploadDto.File.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await uploadDto.File.CopyToAsync(stream);
        }

        document.FilePath = filePath;

        _context.ClientDocuments.Add(document);
        await _context.SaveChangesAsync();

        // Reload with client data
        await _context.Entry(document)
            .Reference(d => d.Client)
            .LoadAsync();

        // Publish document uploaded event
        try
        {
            var documentUploadedEvent = new ClientDocumentUploadedEvent
            {
                DocumentId = document.Id,
                ClientId = document.ClientId,
                DocumentType = document.DocumentType.ToString(),
                FileName = document.FileName,
                FileSize = long.Parse(document.FileSize),
                UploadedAt = document.UploadDate,
                UploadedBy = createdBy
            };

            await _messageBus.PublishAsync("client.document.uploaded", documentUploadedEvent);
            _logger.LogInformation("Published ClientDocumentUploadedEvent for document {DocumentId}", document.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish ClientDocumentUploadedEvent for document {DocumentId}", document.Id);
            // Don't throw - the document was successfully uploaded, messaging is secondary
        }

        return _mapper.Map<ClientDocumentDto>(document);
    }

    public async Task<ClientDocumentDto?> UpdateDocumentAsync(int id, UpdateClientDocumentDto updateDto, string? updatedBy = null)
    {
        var document = await _context.ClientDocuments
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
        {
            return null;
        }

        _mapper.Map(updateDto, document);
        document.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();

        return _mapper.Map<ClientDocumentDto>(document);
    }

    public async Task<bool> DeleteDocumentAsync(int id)
    {
        var document = await _context.ClientDocuments.FindAsync(id);
        if (document == null)
        {
            return false;
        }

        // Delete file from disk
        if (File.Exists(document.FilePath))
        {
            File.Delete(document.FilePath);
        }

        _context.ClientDocuments.Remove(document);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(byte[] Content, string ContentType, string FileName)?> DownloadDocumentAsync(int id)
    {
        var document = await _context.ClientDocuments.FindAsync(id);
        if (document == null || !File.Exists(document.FilePath))
        {
            return null;
        }

        var content = await File.ReadAllBytesAsync(document.FilePath);
        return (content, document.ContentType ?? "application/octet-stream", document.FileName);
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
