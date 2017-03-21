using NetworkManager.Core.Models;
using NetworkManager.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkManager.Services.Services
{
    public class DirectoryService : IDirectoryService, IDriveService, IDisposable
    {
        private string _path;
        private DirectoryClass directory = new DirectoryClass();
        private DirectoryInfo directoryInfo;
        public static long Mb = 1000000;

        public DirectoryService()
        {
            //getting Logical Drives
            directory.Drives = _getDrives();
        }
        public async Task<DirectoryClass> _getNames(string path)
        {
            if (String.IsNullOrEmpty(path) || String.IsNullOrWhiteSpace(path))
            {
                return await _getNames();
            }
            return await _ProcessDirectoryNames(path);
        }
        public async Task<DirectoryClass> _getInfo(string path)
        {
            if (String.IsNullOrEmpty(path) || String.IsNullOrWhiteSpace(path))
            {
                return await _getInfo();
            }
            return await _ProcessDirectory(path);
        }
        public async Task<DirectoryClass> _ProcessDirectory(string path)
        {
            directoryInfo = await _getDirectory(path);

            directory.Name = directoryInfo.Name;
            directory.path = new PathClass { path = path, parrentPath = (directoryInfo.Parent == null) ? path : directoryInfo.Parent.FullName, rootPath = directoryInfo.Root.FullName };
            //getting directory.Directories
            directory.Folders = await GetSubDirectories(directoryInfo, SearchOption.TopDirectoryOnly);
            //directory.Files
            directory.Files = await GetDirFiles(directoryInfo, SearchOption.TopDirectoryOnly);

            directory.Size = Math.Round(directory.Folders.Sum(q => q.Size) + directory.Files.Sum(q => q.Size), 2);
            directory.Created = directoryInfo.CreationTime;
            directory.Modified = directoryInfo.LastWriteTime;
            directory.GetCurrentDrive();
            await SortItems();

            return directory;
        }
        public async Task<DirectoryClass> _ProcessDirectoryNames(string path)
        {
            directoryInfo = await _getDirectory(path);
            directory.Name = directoryInfo.Name;
            directory.path = new PathClass { path = path, parrentPath = (directoryInfo.Parent == null) ? path : directoryInfo.Parent.FullName, rootPath = directoryInfo.Root.FullName };
            
            //getting directory.Directories
            directory.Folders = await GetSubDirectoriesNames(directoryInfo, SearchOption.TopDirectoryOnly);
           // GetSubDirectories(directoryInfo, SearchOption.TopDirectoryOnly);
            //directory.Files
            directory.Files = await GetDirFiles(directoryInfo, SearchOption.TopDirectoryOnly);

            directory.Created = directoryInfo.CreationTime;
            directory.Modified = directoryInfo.LastWriteTime;
            directory.GetCurrentDrive();
            return directory;
        }
        public async Task<DirectoryInfo> _getDirectory(string path)
        {
            return await Task.Run(() => {
                var dir = new DirectoryInfo(path);
                if (dir.Exists)
                    return dir;
                return new DirectoryInfo(Directory.GetCurrentDirectory());
            }
            );
        }
        public static async Task<List<SubDirectoryClass>> GetSubDirectoriesNames(DirectoryInfo d, SearchOption searchOption)
        {
            return await Task.Run(() =>
            {
                return d.EnumerateDirectories("*", searchOption).Select(q => new SubDirectoryClass
                {
                    path = new PathClass { path = q.FullName },
                    Name = q.Name,
                    FullName = q.FullName,
                    Created = q.CreationTime,
                    Modified = q.LastWriteTime
                }).ToList();
            });
        }
        public static async Task<List<SubDirectoryClass>> GetSubDirectories(DirectoryInfo d, SearchOption searchOption)
        {
            return await Task.Run(async () =>
            {
                var subDirs = await GetSubDirectoriesNames(d, searchOption);

                foreach (var folder in subDirs)
                {
                    try {
                        //    var alldirFiles = await GetAllDirFiles(folder.FullName, SearchOption.AllDirectories);
                        var alldirFiles = await GetDirFilesSizes(new DirectoryInfo(folder.FullName));

                        folder.category = new CountCategory { };
                        folder.category.totalSmallItems = alldirFiles.Where(q => q < (Mb * 10)).Count();
                        folder.category.totalMediumItems = alldirFiles.Where(q => q >= (Mb * 10) && q <= (Mb * 50)).Count();
                        folder.category.totalBigItems = alldirFiles.Where(q => q > (Mb * 100)).Count();
                        folder.Size = ConvertBytesToMegabytes(alldirFiles.Sum());
                    }
                    catch(Exception ex){ }
                }
                return subDirs;
            });
        }

    
        public static async Task<List<FileClass>> GetDirFiles(DirectoryInfo d, SearchOption searchOption)
        {
            return await Task.Run(() =>
            {
                var files = new List<FileClass>();
                try
                {
                    foreach (var file in d.EnumerateFiles("*", searchOption))
                    {
                        try { files.Add(new FileClass { Size = ConvertBytesToMegabytes(file.Length), Name = file.Name }); }
                        catch { files.Add(new FileClass { Size = 0, Name = file.Name }); }
                    }
                }
                catch(Exception ex)
                {
                }
                return files;
            });
        }

        public static async Task<List<long>> GetDirFilesSizes(DirectoryInfo d)
        {
            List<long> Sizes = new List<long>();
            try
            {
                // Add subdirectory sizes.
                var Enumfiles = d.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                foreach (FileInfo file in Enumfiles)
                {
                    try
                    {
                        Sizes.Add(file.Length);
                    }
                    catch (Exception ex) { }
                }
            }
            catch(Exception ex) { }
            try
             {
                 // Add subdirectory sizes.
                 var Enumdirs = d.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                 foreach (DirectoryInfo dir in Enumdirs)
                 {
                     try
                     {
                        Sizes.AddRange(await GetDirFilesSizes(dir));
                     }
                    catch (Exception ex) { }
                }
             }
            catch (Exception ex) { }
            // Add file sizes.
            return Sizes;
        }
        public async Task SortItems()
        {
            await Task.Run(() => {
                directory.category = new CountCategory { };
                try { directory.category.totalSmallItems = directory.Files.Where(q => q.Size < 10).Count() + directory.Folders.Sum(q => q.category.totalSmallItems); } catch { }
                try { directory.category.totalMediumItems = directory.Files.Where(q => q.Size >= 10 && q.Size <= 50).Count() + directory.Folders.Sum(q => q.category.totalMediumItems); } catch (Exception ex) { }
                try { directory.category.totalBigItems = directory.Files.Where(q => q.Size > 100).Count() + directory.Folders.Sum(q => q.category.totalBigItems); } catch (Exception ex) { }
            });
        }


        public List<Drive> _getDrives()//getting Logical Drives
        {
            var pcDrives = Task.Run(async () => {
                return await GetAllDrives();
            });
            return pcDrives.Result;
        }
        public async Task<List<Drive>> GetAllDrives()
        {
            return await Task.Run(async () =>
            {
                var DrivesList = new List<Drive>();
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (var drive in allDrives)
                {
                    if (drive.IsReady)
                        try
                        {
                            Drive hardDisk = await GetDrive(drive);
                            DrivesList.Add(hardDisk);
                        }
                        catch (Exception ex) { }
                }
                return DrivesList;
            });
        }
        public async Task<Drive> GetDrive(DriveInfo driveInfo)
        {
            return await Task.Run(() => {
                var drive = new Drive();
                drive.dAvailableFreeSpace = driveInfo.AvailableFreeSpace;
                drive.dType = driveInfo.DriveType;
                drive.dFormat = driveInfo.DriveFormat;
                drive.dName = new PathClass { rootPath = driveInfo.Name };
                drive.dTotalFreeSpace = driveInfo.TotalFreeSpace;
                drive.dTotalSize = driveInfo.TotalSize;
                drive.dVolumeLabel = driveInfo.VolumeLabel;
                drive.iScurrent = false;
                return drive;
            });
        }

        public async Task<DirectoryClass> _getNames()
        {
            _path = Directory.GetCurrentDirectory();
            return await _ProcessDirectoryNames(_path);
        }
        public async Task<DirectoryClass> _getInfo()
        {
            _path = Directory.GetCurrentDirectory();
            return await _ProcessDirectory(_path);
        }
        static double ConvertBytesToMegabytes(long bytes)
        {
            long B = 0, KB = 1024, MB = KB * 1024, GB = MB * 1024, TB = GB * 1024;
            double size = 0;
            size = Math.Round((double)bytes / MB, 2);
            return size;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    directory = null;
                    directoryInfo = null;
                    _path = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DirectoryService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
