using AutoMapper;
using CareManagement.Client.Api.Models;
using CareManagement.Client.Api.DTOs;

namespace CareManagement.Client.Api.Mapping;

public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        // Client mappings
        CreateMap<Models.Client, ClientDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                DateTime.Today.Year - src.DateOfBirth.Year -
                (DateTime.Today.DayOfYear < src.DateOfBirth.DayOfYear ? 1 : 0)));

        CreateMap<CreateClientDto, Models.Client>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CarePlans, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore())
            .ForMember(dest => dest.ClientNotes, opt => opt.Ignore());

        CreateMap<UpdateClientDto, Models.Client>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // CarePlan mappings
        CreateMap<CarePlan, CarePlanDto>();
        CreateMap<CreateCarePlanDto, CarePlan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore());

        CreateMap<UpdateCarePlanDto, CarePlan>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ClientDocument mappings
        CreateMap<ClientDocument, ClientDocumentDto>();
        CreateMap<CreateClientDocumentDto, ClientDocument>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FilePath, opt => opt.Ignore())
            .ForMember(dest => dest.FileName, opt => opt.Ignore())
            .ForMember(dest => dest.FileSize, opt => opt.Ignore())
            .ForMember(dest => dest.ContentType, opt => opt.Ignore())
            .ForMember(dest => dest.UploadDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore());

        CreateMap<UpdateClientDocumentDto, ClientDocument>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ClientNote mappings
        CreateMap<ClientNote, ClientNoteDto>();
        CreateMap<CreateClientNoteDto, ClientNote>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.NoteDate, opt => opt.MapFrom(src => src.NoteDate ?? DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore());

        CreateMap<UpdateClientNoteDto, ClientNote>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
