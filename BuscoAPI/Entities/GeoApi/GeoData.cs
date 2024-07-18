namespace BuscoAPI.Entities.GeoApi
{
    public class GeoData
    {
        public List<Localidad> Provincias { get; set; }
        public List<Localidad> Departamentos { get; set; }
        public List<Localidad> localidades_censales { get; set; }
    }
}
