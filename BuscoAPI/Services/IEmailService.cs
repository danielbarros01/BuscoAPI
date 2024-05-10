using BuscoAPI.DTOS;

namespace BuscoAPI.Services
{
    public interface IEmailService
    {
        void SendEmail(MailRequest request);
    }
}
