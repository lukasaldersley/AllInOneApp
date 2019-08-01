using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace AllInOneApp
{
    class StorageInterface
    {
        private static StorageFolder BaseFolder;
        public static StorageFolder LOCAL_FOLDER = ApplicationData.Current.LocalFolder;
        public static StorageFolder ROAMING_FOLDER = ApplicationData.Current.RoamingFolder;
        public const int OTHER = -1;
        public const int LOCAL = 0;
        public const int ROAMING = 1;
        public const int BASE = 2;
        public const int OPEN = 0;
        public const int REPLACE = 1;
        public static String BaseFolderFutureAccessToken { get; set; } = null;




        //API CALLS----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        internal async static Task<String> PickExternalStorageFile_NewFile(String suggestedName = "LifeLog")
        {
            FileSavePicker fsPicker = new FileSavePicker()
            {
                SuggestedFileName = suggestedName,
                DefaultFileExtension = ".txt",
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            fsPicker.FileTypeChoices.Add("Textdatei", new List<string> { ".txt" });
            fsPicker.FileTypeChoices.Add("Datei", new List<string> { "." });
            StorageFile sf = await fsPicker.PickSaveFileAsync();
            String Token = null;
            if (sf != null)
            {
                Token = StorageApplicationPermissions.FutureAccessList.Add(sf);
            }
            else
            {
                return null;
            }
            return Token;
        }

        internal async static Task<String> PickExternalStorageFile_OpenFile()
        {
            FileOpenPicker foPicker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                ViewMode = PickerViewMode.Thumbnail
            };
            foPicker.FileTypeFilter.Add("*");
            foPicker.FileTypeFilter.Add(".txt");
            StorageFile sf = await foPicker.PickSingleFileAsync();
            String Token = null;
            if (sf != null)
            {
                Token = StorageApplicationPermissions.FutureAccessList.Add(sf);
            }
            else
            {
                return null;
            }
            return Token;
        }

        internal async static Task<String[]> PickExternalStorageFiles_OpenFiles()
        {
            FileOpenPicker foPicker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                ViewMode = PickerViewMode.Thumbnail
            };
            foPicker.FileTypeFilter.Add(".csv");
            foPicker.FileTypeFilter.Add(".txt");
            foPicker.FileTypeFilter.Add("*");
            IReadOnlyList<StorageFile> sf = await foPicker.PickMultipleFilesAsync();
            IEnumerator<StorageFile> sfe = sf.GetEnumerator();
            String[] Token = new String[sf.Count];
            for (int i = 0; i < sf.Count; i++)
            {
                sfe.MoveNext();
                StorageFile sfl = sfe.Current;
                if (sfl != null)
                {
                    Token[i] = StorageApplicationPermissions.FutureAccessList.Add(sfl);
                }
                else
                {
                    Token[i] = null;
                }
            }
            return Token;
        }

        internal async static Task<String> PickExternalStorageFolder()
        {
            FolderPicker fPicker = new FolderPicker()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                ViewMode = PickerViewMode.Thumbnail
            };
            fPicker.FileTypeFilter.Add("*");
            StorageFolder sf = await fPicker.PickSingleFolderAsync();
            String Token = null;
            if (sf != null)
            {
                Token = StorageApplicationPermissions.FutureAccessList.Add(sf);
            }
            else
            {
                return null;
            }
            return Token;
        }


        internal static async Task<StorageFile> GetStorageFileFromToken(String Token)
        {
            if (Token == null)
            {
                return null;
            }
            return await StorageApplicationPermissions.FutureAccessList.GetFileAsync(Token);
        }

        internal static async Task<StorageFolder> GetStorageFolderFromToken(String Token)
        {
            if (Token == null)
            {
                return null;
            }
            return await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(Token);
        }



        internal async static Task<String> ReadFromKnownStorageFile(String FileToken)
        {
            StorageFile sf = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FileToken);
            if (sf == null)
            {
                return null;
            }
            return await ReadFromStorageFile(sf);
        }

        internal async static Task<IBuffer> ReadBufferFromKnownStorageFile(String FileToken)
        {
            StorageFile sf = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FileToken);
            if (sf == null)
            {
                return null;
            }
            return await ReadBufferFromStorageFile(sf);
        }

        internal static async Task WriteToKnownStorageFile(String FileToken, String content)
        {
            StorageFile sf = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FileToken);
            if (sf == null)
            {
                return;
            }
            await WriteToStorageFile(sf, content);
        }

        internal static async Task WriteBufferToKnownStorageFile(String FileToken, IBuffer content)
        {
            StorageFile sf = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FileToken);
            if (sf == null)
            {
                return;
            }
            await WriteBufferToStorageFile(sf, content);
        }

        internal static async Task WriteBytesToKnownStorageFile(String FileToken, byte[] content)
        {
            StorageFile sf = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FileToken);
            if (sf == null)
            {
                return;
            }
            await WriteBytesToStorageFile(sf, content);
        }

        internal static async Task AppendToKnownStorageFile(String FileToken, String content)
        {
            StorageFile sf = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FileToken);
            if (sf == null)
            {
                return;
            }
            await AppendToStorageFile(sf, content);
        }



        internal static async Task<String> ReadFromStorageFile(StorageFile sf)
        {
            return await FileIO.ReadTextAsync(sf);
        }

        internal static async Task<IBuffer> ReadBufferFromStorageFile(StorageFile sf)
        {
            return await FileIO.ReadBufferAsync(sf);
        }

        internal async static Task WriteToStorageFile(StorageFile sf, String content)
        {
            await FileIO.WriteTextAsync(sf, content);
        }

        internal async static Task WriteBufferToStorageFile(StorageFile sf, IBuffer content)
        {
            await FileIO.WriteBufferAsync(sf, content);
        }

        internal async static Task WriteBytesToStorageFile(StorageFile sf, byte[] content)
        {
            await FileIO.WriteBytesAsync(sf, content);
        }

        internal async static Task AppendToStorageFile(StorageFile sf, String content)
        {
            await FileIO.AppendTextAsync(sf, content);
        }



        private static async Task<StorageFile> NavigateFileSystem(int folderType, String targetName, int collisionOption,StorageFolder specialFolder=null)
        {
            Debug.WriteLine("STARTING");
            StorageFolder storageFolder;
            if (folderType == OTHER)
            {
                if (specialFolder == null)
                {
                    storageFolder = KnownFolders.PicturesLibrary;
                    await UserInteraction.ShowDialogAsync("WARNING", "The file could not be saved in the desired Location.\r\nIt will be saved to your PicturesLibrary");
                }
                else
                {
                    storageFolder = specialFolder;
                    
                }
            }
            else if (folderType == BASE)
            {
                storageFolder = BaseFolder;
            }
            else if (folderType == ROAMING)
            {
                storageFolder = ApplicationData.Current.RoamingFolder;
            }
            else
            {
                storageFolder = ApplicationData.Current.LocalFolder;
            }
            targetName = targetName.Replace("\\", "/");
            String[] paths = targetName.Split('/');
            if (paths.Length > 1)
            {
                for (int i = 0; i < paths.Length - 1; i++)
                {
                    storageFolder = await storageFolder.CreateFolderAsync(paths[i], CreationCollisionOption.OpenIfExists);
                }
            }
            if (collisionOption == REPLACE)
            {
                return await storageFolder.CreateFileAsync(paths[paths.Length - 1], CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                return await storageFolder.CreateFileAsync(paths[paths.Length - 1], CreationCollisionOption.OpenIfExists);
            }
        }



        private static async Task LoadBaseFolderAsync()
        {
            String Token = await ReadFromLocalFolder("BaseFolderFutureAccessToken.token");
            if (Token == null || Token.Equals(""))
            {
                Token = await PickExternalStorageFolder();
                if (Token == null || Token.Equals(""))//Full-On FAIL => Just give up and exit, but notify the user
                {
                    await UserInteraction.ShowDialogAsync("ERROR","Unable to load required resources.\n App will be closed.");
                    ForceTerminateApp();
                }
            }
            BaseFolderFutureAccessToken = Token;
            await WriteToLocalFolder("BaseFolderFutureAccessToken.token", BaseFolderFutureAccessToken);
        }


        internal static async Task CheckBaseFolder()
        {
            if (BaseFolder == null)
            {
                if (BaseFolderFutureAccessToken == null || BaseFolderFutureAccessToken.Equals(""))
                {
                    await LoadBaseFolderAsync();
                }
                BaseFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(BaseFolderFutureAccessToken);
            }
        }

        internal static async Task<String> ReadFromBaseFolder(String fileName)
        {
            await CheckBaseFolder();
            StorageFile storageFile = await NavigateFileSystem(BASE, fileName, OPEN);
            return await FileIO.ReadTextAsync(storageFile);
        }

        internal static async Task<IBuffer> ReadBufferFromBaseFolder(String fileName)
        {
            await CheckBaseFolder();
            StorageFile storageFile = await NavigateFileSystem(BASE, fileName, OPEN);
            return await FileIO.ReadBufferAsync(storageFile);
        }

        internal async static Task WriteToBaseFolder(String fileName, String content)
        {
            await CheckBaseFolder();
            StorageFile storageFile = await NavigateFileSystem(BASE, fileName, REPLACE);
            await FileIO.WriteTextAsync(storageFile, content);
        }

        internal async static Task WriteBufferToBaseFolder(String fileName, IBuffer content)
        {
            await CheckBaseFolder();
            StorageFile storageFile = await NavigateFileSystem(BASE, fileName, REPLACE);
            await FileIO.WriteBufferAsync(storageFile, content);
        }

        internal async static Task WriteBytesToBaseFolder(String fileName, byte[] content)
        {
            await CheckBaseFolder();
            StorageFile storageFile = await NavigateFileSystem(BASE, fileName, REPLACE);
            await FileIO.WriteBytesAsync(storageFile, content);
        }

        internal async static Task AppendToBaseFolder(String fileName, String content)
        {
            await CheckBaseFolder();
            StorageFile storageFile = await NavigateFileSystem(BASE, fileName, OPEN);
            await FileIO.AppendTextAsync(storageFile, content);
        }

        internal async static Task DeleteFromBaseFolder(String fileName)
        {
            StorageFile storageFile = await NavigateFileSystem(BASE, fileName, OPEN);
            await storageFile.DeleteAsync();
        }



        internal static async Task<String> ReadFromLocalFolder(String fileName)
        {
            StorageFile storageFile = await NavigateFileSystem(LOCAL, fileName, OPEN);
            return await FileIO.ReadTextAsync(storageFile);
        }

        internal static async Task<IBuffer> ReadBufferFromLocalFolder(String fileName)
        {
            StorageFile storageFile = await NavigateFileSystem(LOCAL, fileName, OPEN);
            return await FileIO.ReadBufferAsync(storageFile);
        }

        internal static async Task WriteToLocalFolder(String fileName, String content)
        {
            StorageFile storageFile = await NavigateFileSystem(LOCAL, fileName, REPLACE);
            await FileIO.WriteTextAsync(storageFile, content);
        }

        internal static async Task WriteBufferToLocalFolder(String fileName, IBuffer content)
        {
            StorageFile storageFile = await NavigateFileSystem(LOCAL, fileName, REPLACE);
            await FileIO.WriteBufferAsync(storageFile, content);
        }

        internal static async Task WriteBytesToLocalFolder(String fileName, byte[] content)
        {
            StorageFile storageFile = await NavigateFileSystem(LOCAL, fileName, REPLACE);
            await FileIO.WriteBytesAsync(storageFile, content);
        }

        internal async static Task AppendToLocalFolder(String fileName, String content)
        {
            StorageFile storageFile = await NavigateFileSystem(LOCAL, fileName, OPEN);
            await FileIO.AppendTextAsync(storageFile, content);
        }

        internal async static Task DeleteFromLocalFolder(String fileName)
        {
            StorageFile storageFile = await NavigateFileSystem(LOCAL, fileName, OPEN);
            await storageFile.DeleteAsync();
        }



        internal static async Task<String> ReadFromSpecifiedFolder(String fileName, StorageFolder storageFolder)
        {
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, OPEN, storageFolder);
            return await FileIO.ReadTextAsync(storageFile);
        }

        internal static async Task<IBuffer> ReadBufferFromSpecifiedFolder(String fileName, StorageFolder storageFolder)
        {
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, OPEN, storageFolder);
            return await FileIO.ReadBufferAsync(storageFile);
        }

        internal static async Task WriteToSpecifiedFolder(String fileName, String content, StorageFolder storageFolder)
        {
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, REPLACE, storageFolder);
            await FileIO.WriteTextAsync(storageFile, content);
        }

        internal static async Task WriteBufferToSpecifiedFolder(String fileName, IBuffer content, StorageFolder storageFolder)
        {
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, REPLACE, storageFolder);
            await FileIO.WriteBufferAsync(storageFile, content);
        }

        internal static async Task WriteBytesToSpecifiedFolder(String fileName, byte[] content, StorageFolder storageFolder)
        {
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, REPLACE, storageFolder);
            await FileIO.WriteBytesAsync(storageFile, content);
        }

        internal async static Task AppendToSpecifiedFolder(String fileName, String content, StorageFolder storageFolder)
        {
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, OPEN, storageFolder);
            await FileIO.AppendTextAsync(storageFile, content);
        }

        internal async static Task DeleteFromSpecifiedFolder(String fileName, StorageFolder storageFolder)
        {
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, OPEN, storageFolder);
            await storageFile.DeleteAsync();
        }



        internal static async Task<String> ReadFromKnownFolder(String fileName, String FolderToken)
        {
            StorageFolder storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FolderToken);
            if (storageFolder == null)
            {
                return null;
            }
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, OPEN, storageFolder);
            return await FileIO.ReadTextAsync(storageFile);
        }

        internal static async Task<IBuffer> ReadBufferFromKnownFolder(String fileName, String FolderToken)
        {
            StorageFolder storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FolderToken);
            if (storageFolder == null)
            {
                return null;
            }
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, OPEN, storageFolder);
            return await FileIO.ReadBufferAsync(storageFile);
        }

        internal static async Task WriteToKnownFolder(String fileName, String content, String FolderToken)
        {
            StorageFolder storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FolderToken);
            if (storageFolder == null)
            {
                return;
            }
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, REPLACE, storageFolder);
            await FileIO.WriteTextAsync(storageFile, content);
        }

        internal static async Task WriteBufferToKnownFolder(String fileName, IBuffer content, String FolderToken)
        {
            StorageFolder storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FolderToken);
            if (storageFolder == null)
            {
                return;
            }
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, REPLACE, storageFolder);
            await FileIO.WriteBufferAsync(storageFile, content);
        }

        internal static async Task WriteBytesToKnownFolder(String fileName, byte[] content, String FolderToken)
        {
            StorageFolder storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FolderToken);
            if (storageFolder == null)
            {
                return;
            }
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, REPLACE, storageFolder);
            await FileIO.WriteBytesAsync(storageFile, content);
        }

        internal async static Task AppendToKnowndFolder(String fileName, String content, String FolderToken)
        {
            StorageFolder storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FolderToken);
            if (storageFolder == null)
            {
                return;
            }
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, OPEN, storageFolder);
            await FileIO.AppendTextAsync(storageFile, content);
        }

        internal async static Task DeleteFromKnownFolder(String fileName,String FolderToken)
        {
            StorageFolder storageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(FolderToken);
            if (storageFolder == null)
            {
                return;
            }
            StorageFile storageFile = await NavigateFileSystem(OTHER, fileName, OPEN, storageFolder);
            await storageFile.DeleteAsync();
        }



        internal async static Task<String> ReadFromRoamingFolder(String fileName)
        {
            StorageFile storageFile = await NavigateFileSystem(ROAMING, fileName, OPEN);
            return await FileIO.ReadTextAsync(storageFile);
        }

        internal async static Task<IBuffer> ReadBufferFromRoamingFolder(String fileName)
        {
            StorageFile storageFile = await NavigateFileSystem(ROAMING, fileName, OPEN);
            return await FileIO.ReadBufferAsync(storageFile);
        }

        internal async static Task WriteToRoamingFolder(String fileName, String content)
        {
            StorageFile storageFile = await NavigateFileSystem(ROAMING, fileName, REPLACE);
            await FileIO.WriteTextAsync(storageFile, content);
        }

        internal async static Task WriteBufferToRoamingFolder(String fileName, IBuffer content)
        {
            StorageFile storageFile = await NavigateFileSystem(ROAMING, fileName, REPLACE);
            await FileIO.WriteBufferAsync(storageFile, content);
        }

        internal async static Task WriteBytesToRoamingFolder(String fileName, byte[] content)
        {
            StorageFile storageFile = await NavigateFileSystem(ROAMING, fileName, REPLACE);
            await FileIO.WriteBytesAsync(storageFile, content);
        }

        internal async static Task AppendToRoamingFolder(String fileName, String content)
        {
            StorageFile storageFile = await NavigateFileSystem(ROAMING, fileName, OPEN);
            await FileIO.AppendTextAsync(storageFile, content);
        }

        internal async static Task DeleteFromRoamingFolder(String fileName)
        {
            StorageFile storageFile = await NavigateFileSystem(ROAMING, fileName, OPEN);
            await storageFile.DeleteAsync();
        }

        internal async static Task<ImageSource> GetImageSourceFromStorageFile(StorageFile sf)
        {
            using (var randomAccessStream = await sf.OpenAsync(FileAccessMode.Read))
            {
                var result = new BitmapImage();
                await result.SetSourceAsync(randomAccessStream);
                return result;
            }
        }





        internal static void ForceTerminateApp()
        {
            Application.Current.Exit();
        }
    }
}
