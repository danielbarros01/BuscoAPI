using BuscoAPI.DTOS.Users;
using BuscoAPI.Entities;
using BuscoAPI.Services;
using System.ComponentModel;

namespace BuscoAPI.Helpers
{
    public class SaveImage
    {
        public async void SaveOrEditImage(UserImageDto user, User userDb, string container, IFileStore fileStore)
        {
            if (user.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await user.Image.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(user.Image.FileName);

                    userDb.Image = await fileStore.SaveFile(content, extension, container, user.Image.ContentType);
                }
            }
        }
    }
}
