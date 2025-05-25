using AutoMapper;
using CMS_Api.Models;
using CMS_Api.Dtos;

namespace CMS_Api.AutoMapperConfig
{
    public class CaseProfile : Profile
    {
        public CaseProfile()
        {
            CreateMap<Case, CaseReadDto>();
            CreateMap<CaseCreateDto, Case>();
        }
    }
}
