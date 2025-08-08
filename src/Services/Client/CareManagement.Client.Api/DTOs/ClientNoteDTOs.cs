using CareManagement.Client.Api.Models;

namespace CareManagement.Client.Api.DTOs;

public class ClientNoteDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NoteType NoteType { get; set; }
    public DateTime NoteDate { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public ClientDto? Client { get; set; }
}

public class CreateClientNoteDto
{
    public int ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NoteType NoteType { get; set; }
    public DateTime? NoteDate { get; set; }
    public bool IsPrivate { get; set; }
}

public class UpdateClientNoteDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public NoteType? NoteType { get; set; }
    public DateTime? NoteDate { get; set; }
    public bool? IsPrivate { get; set; }
}
