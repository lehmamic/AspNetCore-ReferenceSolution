using System.Threading.Tasks;
using Demo.Backend.SwisscomOpenApis.Dtos;
using Refit;

namespace Demo.Backend.SwisscomOpenApis
{
    public interface ISwisscomOpenDataApi
    {
        [Get("/api/records/1.0/search/?dataset=locations-of-lte-cell-towers")]
        public Task<SwisscomOpenApiResponseDto> GetLteCellTowers([Query] string q, [Query] int? rows, [Query] int? start, [Query] string facet);
    }
}