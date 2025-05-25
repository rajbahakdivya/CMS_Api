using AutoMapper;
using CMS_Api.Models;
using CMS_Api.DTOs;
using System.Globalization;

namespace CMS_Api.AutoMapperProfiles
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            // Map Document entity to DocumentDto
            CreateMap<Document, DocumentDto>()
                // Format CreatedAt to yyyy-MM-dd string for UploadDate
                .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)))
                // Default UploadedBy to "System" if null
                .ForMember(dest => dest.UploadedBy, opt => opt.MapFrom(src => src.UploadedBy ?? "System"))
                // Ignore DownloadUrl here, set it manually in controller because it depends on URL helper
                .ForMember(dest => dest.DownloadUrl, opt => opt.Ignore());

            // Map DocumentUploadDto to Document entity (used for POST)
            CreateMap<DocumentUploadDto, Document>()
                // Map FileType to uppercase automatically (optional)
                .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType.ToUpper()));
        }
    }
}
