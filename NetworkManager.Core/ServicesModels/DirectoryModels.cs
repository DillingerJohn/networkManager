using System;
using System.IO;
using System.Collections.Generic;

namespace NetworkManager.Core.Models
{
    public class Drive
    {
        public bool iScurrent { get; set; }
        public PathClass dName { get; set; }
        public virtual DriveType dType { get; set; }
        public string dVolumeLabel { get; set; }
        public string dFormat { get; set; }
        public long dAvailableFreeSpace { get; set; }
        public long dTotalFreeSpace { get; set; }
        public double dTotalFreeSpaceGB { get { return dTotalSizeGB - dTotalFreeSpace / 1073741824; } }
        public long dTotalSize { get; set; }
        public double dTotalSizeGB { get { return dTotalSize / 1073741824; } }
    }
    public class DirectoryClass
    {
        public List<Drive> Drives { get; set; }
        public PathClass path { get; set; }
        public List<SubDirectoryClass> Folders { get; set; }
        public List<FileClass> Files { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public double Size { get; set; }
        public CountCategory category { get; set; }
        private PathClass previusPath { get; set; }
        public virtual void GetCurrentDrive()
        {
            foreach (var dr in this.Drives)
            {
                if (this.path.rootPath.Contains(dr.dName.rootPath))
                { dr.iScurrent = true; }
                else { dr.iScurrent = false; }
            }
            
        }
    }
    public class SubDirectoryClass: DirectoryClass
    {

    }
    public class FileClass
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public double Size { get; set; }
    }
    public class PathClass
    {
        public string path { get; set; }
        public string parrentPath { get; set; }
        public string rootPath { get; set; }
    }
    public class CountCategory
    {
        public int totalSmallItems { get; set; }
        public int totalMediumItems { get; set; }
        public int totalBigItems { get; set; }
    }
}
