using System.Collections.Generic;
using System.Threading.Tasks;
using NetworkManager.Core.Models;
using System.IO;


namespace NetworkManager.Core.Services
{
    public interface IDriveService
    {
        Task <List<Drive>> GetAllDrives();
        Task<Drive> GetDrive(DriveInfo driveInfo);
    }

}
