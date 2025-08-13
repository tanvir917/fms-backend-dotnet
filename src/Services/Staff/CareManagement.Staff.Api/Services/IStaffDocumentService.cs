using CareManagement.Staff.Api.DTOs;

namespace CareManagement.Staff.Api.Services;

public interface IStaffDocumentService
{
    Task<IEnumerable<StaffDocumentDto>> GetDocumentsByStaffIdAsync(int staffId);
    Task<StaffDocumentDto?> GetDocumentByIdAsync(int id);
    Task<StaffDocumentDto> CreateDocumentAsync(StaffDocumentUploadDto uploadDto, string? createdBy = null);
    Task<StaffDocumentDto?> UpdateDocumentAsync(int id, UpdateStaffDocumentDto updateDto, string? updatedBy = null);
    Task<bool> DeleteDocumentAsync(int id);
    Task<(byte[] Content, string ContentType, string FileName)?> DownloadDocumentAsync(int id);
}
