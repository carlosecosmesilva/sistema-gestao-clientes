using AutoMapper;
using SistemaGestao.Application.DTOs;
using SistemaGestao.Domain.Entities;

namespace SistemaGestao.Application.Mappings
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            // Entity -> DTO
            CreateMap<Cliente, ClienteResumoDto>();
            CreateMap<Cliente, ClienteDetalheDto>();
            CreateMap<Logradouro, LogradouroDto>().ReverseMap();

            // DTO -> Entity (Create/Update)
            CreateMap<ClienteCreateDto, Cliente>();
            CreateMap<ClienteDetalheDto, Cliente>();
        }
    }
}