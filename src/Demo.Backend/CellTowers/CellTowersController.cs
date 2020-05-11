using System;
using System.Threading.Tasks;
using AutoMapper;
using Demo.Backend.CellTowers.Dtos;
using Demo.Backend.SwisscomOpenApis;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Backend.CellTowers
{
    [ApiController]
    [Route("api/v1/swisscom-open-data")]
    public class CellTowersController : ControllerBase
    {
        private readonly ISwisscomOpenDataApi _swisscomOpenDataApi;
        private readonly IMapper _mapper;

        public CellTowersController(ISwisscomOpenDataApi swisscomOpenDataApi, IMapper mapper)
        {
            _swisscomOpenDataApi = swisscomOpenDataApi ?? throw new ArgumentNullException(nameof(swisscomOpenDataApi));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("lte-cell-towers")]
        public async Task<ActionResult<SwisscomOpenApiResponseDto>> GetLteCellTowers()
        {
            SwisscomOpenApis.Dtos.SwisscomOpenApiResponseDto result = await _swisscomOpenDataApi.GetLteCellTowers(string.Empty, 10, 0, "powercode");
            return Ok(_mapper.Map<SwisscomOpenApiResponseDto>(result));
        }
    }
}