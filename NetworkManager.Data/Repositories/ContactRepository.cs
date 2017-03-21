using NetworkManager.Core.Data.Repositories;
using NetworkManager.Core.Entities;

namespace NetworkManager.Data.Repositories
{
    public class ContactRepository : BaseRepository<Contact>, IContactRepository
    {
        public ContactRepository(IDbContext context) : base (context)
        {
        }
    }
}
