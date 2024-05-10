
namespace BuscoAPI.Services
{
    public class LocalFileStore : IFileStore
    {
        private readonly IWebHostEnvironment env; //Obtener ruta donde esta wwwroot
        private readonly IHttpContextAccessor httpContextAccessor; //determinar el dominio donde esta webapi

        public LocalFileStore(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string route, string container)
        {
            if(route != null)
            {
                var nameFile = Path.GetFileName(route);
                string directoryFile = Path.Combine(env.WebRootPath, container, nameFile);

                if(File.Exists(directoryFile))
                {
                    File.Delete(directoryFile);
                }
            }

            return Task.CompletedTask;
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string route, string contentType)
        {
            await DeleteFile(route, container);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            var nameFile = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, container);

            if(!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nameFile);
            await File.WriteAllBytesAsync(ruta, content); //write in hard disk

            //scheme = http or https
            var currentUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var urlToBd = Path.Combine(currentUrl, container, nameFile).Replace("\\", "/");

            return urlToBd;
        }
    }
}
