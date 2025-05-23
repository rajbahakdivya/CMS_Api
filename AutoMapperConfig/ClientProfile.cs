using AutoMapper;
using CMS_Api.DTOs;
using CMS_Api.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CMS_Api.AutoMapperConfig
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientReadDto>();
            CreateMap<ClientCreateDto, Client>();
        }
    }
}
