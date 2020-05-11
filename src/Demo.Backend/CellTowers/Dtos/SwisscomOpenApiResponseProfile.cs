using AutoMapper;

namespace Demo.Backend.CellTowers.Dtos
{
    public class SwisscomOpenApiResponseProfile : Profile
    {
        public SwisscomOpenApiResponseProfile()
        {
            CreateMap<SwisscomOpenApis.Dtos.SwisscomOpenApiResponseDto, SwisscomOpenApiResponseDto>();
            CreateMap<SwisscomOpenApis.Dtos.RecordDto, RecordDto>();
            CreateMap<SwisscomOpenApis.Dtos.ParametersDto, ParametersDto>();
            CreateMap<SwisscomOpenApis.Dtos.GeoDto, GeoDto>();
            CreateMap<SwisscomOpenApis.Dtos.FieldsDto, FieldsDto>();
            CreateMap<SwisscomOpenApis.Dtos.FacetGroupDto, FacetGroupDto>();
            CreateMap<SwisscomOpenApis.Dtos.FacetDto, FacetDto>();
        }
    }
}