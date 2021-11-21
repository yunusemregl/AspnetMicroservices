using Ordering.Application.Models;
using System.Threading.Tasks;

namespace Ordering.Application.Contract.Infrastructure
{
    public interface IEmailService
    {
        Task<bool> SendEmail(Email email);
    }
}
