using NetworkManager.Core.Data;
using NetworkManager.Core.Entities;
using NetworkManager.Core.Services;

namespace NetworkManager.Services.Services
{
    public class ContactService : BaseService<Contact>, IContactService
    {
        public ContactService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
