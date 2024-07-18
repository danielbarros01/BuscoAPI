using BuscoAPI.Entities.GeoApi;
using BuscoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BuscoAPI.Controllers.GeoApi
{
    [ApiController]
    [Route("[controller]")]
    public class GeoController : ControllerBase
    {
        private readonly SNDGService _sndgService;

        public GeoController(SNDGService sndgService)
        {
            _sndgService = sndgService;
        }

        [HttpGet("provincias")]
        public async Task<ActionResult<List<Localidad>>> GetProvincias()
        {
            var data = await _sndgService.GetProvinces();
            return data.Provincias;
        }
        
        [HttpGet("departamentos/{provincia}")]
        public async Task<ActionResult<List<Localidad>>> GetDepartamentos(String provincia)
        {
            var data = await _sndgService.GetDepartments(provincia);
            return data.Departamentos;
        }
        
        [HttpGet("ciudades/{provincia}/{departamento}")]
        public async Task<ActionResult<List<Localidad>>> GetCiudades(String provincia, String departamento)
        {
            var data = await _sndgService.GetCiudades(provincia,departamento);
            return data.localidades_censales;
        }
    }
}
