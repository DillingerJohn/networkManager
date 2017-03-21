using System;
using System.IO;
using System.Collections.Generic;

namespace NetworkManager.Dto.Dtos
{
    public class DriveDto : BaseDto
    {
        public DriveDto()
        { }

        public string dName { get; set; }
        public DriveType dType { get; set; }
        public string dVolumeLabel { get; set; }
        public string dFormat { get; set; }
        public long dAvailableFreeSpace { get; set; }
        public long dTotalFreeSpace { get; set; }
        public long dTotalSize { get; set; }
        public string dRootDirectory { get; set; }

        public string dDirectories { get; set; }
        public string driveFiles { get; set; }
    }
    public class DirectoryDto: BaseDto
    {
        public List<FileDto> dFiles { get; set; }
    }
    public class FileDto: BaseDto
    {
        public string fName { get; set; }
        public string fSize { get; set; }
        public string fDateCreation { get; set; }
    }
}
