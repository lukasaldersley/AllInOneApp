using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class FolderInspectionPage : Page
    {
        /*
            String Token=await SpeicherInterface.PickExternalStorageFolder();
            await SpeicherInterface.ROAMING_FOLDER.CopyFolderContentsTo(await SpeicherInterface.GetStorageFolderFromToken(Token));*/
        private Boolean MultiSelect = false;
        private StorageFolder SourceFolder;
        private StorageFile[] Files;
        private StorageFolder[] Folders;
        private AppBarButton[] FSave;
        private StackPanel[] EntryPanel;
        private TextBox[] NameBoxes;
        private AppBarButton[] OpenButton;
        private AppBarButton[] DeleteButton;
        private AppBarButton[] LaunchButton;
        private AppBarButton[] ExportButton;
        private Grid[] Grids;
        private CheckBox[] SelectItems;
        private Border[] FBorders;
        private Grid[] FGrids;
        private TextBox[] FTexts;
        private StackPanel[] FActionBar;
        private AppBarButton[] FNav;
        private AppBarButton[] FDelete;
        private AppBarButton[] FLaunch;
        private StackPanel[] FBase;
        private CheckBox[] FSelect;

        public FolderInspectionPage()
        {
            Debug.WriteLine("FIP: INIT");
            this.InitializeComponent();
            Debug.WriteLine("FIP: INIT DONE");
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            SourceFolder = e.Parameter as StorageFolder;
            FolderNameTextBox.Text = SourceFolder.Name;
            Files = await SourceFolder.GetStorageFileArray();
            Folders = await SourceFolder.GetStorageFolderArray();
            ReLoadPage();
            base.OnNavigatedTo(e);
        }

        private void ReLoadPage()
        {
            ContentPanel.Children.Clear();

            FBase = new StackPanel[Folders.Length];
            FSelect = new CheckBox[Folders.Length];
            FBorders = new Border[Folders.Length];
            FGrids = new Grid[Folders.Length];
            FTexts = new TextBox[Folders.Length];
            FActionBar = new StackPanel[Folders.Length];
            FNav = new AppBarButton[Folders.Length];
            FLaunch = new AppBarButton[Folders.Length];
            FDelete = new AppBarButton[Folders.Length];
            FSave = new AppBarButton[Folders.Length];

            for (int i = 0; i < Folders.Length; i++)
            {
                FBase[i] = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    //IsHitTestVisible = false
                };

                FSelect[i] = new CheckBox()
                {
                    Visibility = Visibility.Collapsed
                };

                FBorders[i] = new Border()
                {
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(15),
                    //IsHitTestVisible=false
                };

                FGrids[i] = new Grid()
                {
                    //IsHitTestVisible = false
                };
                FGrids[i].ColumnDefinitions.Add(new ColumnDefinition());
                FGrids[i].ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                FTexts[i] = new TextBox()
                {
                    Text = Folders[i].Name,
                    IsHitTestVisible = false,
                    IsReadOnly = true
                };
                FTexts[i].SetValue(Grid.ColumnProperty, 0);
                //FTexts[i].GotFocus += NavigateSubfolder;

                FActionBar[i] = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
                FActionBar[i].SetValue(Grid.ColumnProperty, 1);

                FNav[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Go)
                };
                FNav[i].Click += NavigateSubfolder;

                FLaunch[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Play)
                };
                FLaunch[i].Click += FolderLaunch_Click;

                FDelete[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Delete)
                };
                FDelete[i].Click += FolderDelete_Click;

                FSave[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Save)
                };
                FSave[i].Click += FolderSave_Click;

                FActionBar[i].Children.Add(FNav[i]);
                FActionBar[i].Children.Add(FLaunch[i]);
                FActionBar[i].Children.Add(FDelete[i]);
                FActionBar[i].Children.Add(FSave[i]);
                FGrids[i].Children.Add(FTexts[i]);
                FGrids[i].Children.Add(FActionBar[i]);
                FBorders[i].Child = FGrids[i];
                //FBase[i].Children.Add(FSelect[i]);
                //FBase[i].Children.Add(FBorders[i]);
                ContentPanel.Children.Add(FBorders[i]);
            }






            EntryPanel = new StackPanel[Files.Length];
            SelectItems = new CheckBox[Files.Length];
            NameBoxes = new TextBox[Files.Length];
            OpenButton = new AppBarButton[Files.Length];
            DeleteButton = new AppBarButton[Files.Length];
            LaunchButton = new AppBarButton[Files.Length];
            ExportButton = new AppBarButton[Files.Length];
            Grids = new Grid[Files.Length];
            for (int i = 0; i < Files.Length; i++)
            {
                Grids[i] = new Grid();
                Grids[i].ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                Grids[i].ColumnDefinitions.Add(new ColumnDefinition());
                Grids[i].ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                SelectItems[i] = new CheckBox()
                {
                    Visibility = Visibility.Collapsed,
                    Content = Files[i].Name
                };
                SelectItems[i].SetValue(Grid.RowProperty, 0);
                EntryPanel[i] = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
                EntryPanel[i].SetValue(Grid.ColumnProperty, 2);
                NameBoxes[i] = new TextBox()
                {
                    Text = Files[i].Name,
                    IsReadOnly = true,
                    IsHitTestVisible = false,
                    BorderThickness = new Thickness(0)
                };
                NameBoxes[i].SetValue(Grid.RowProperty, 1);
                OpenButton[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.OpenFile)
                };
                OpenButton[i].Click += OpenSingleFile;
                LaunchButton[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Play)
                };
                LaunchButton[i].Click += LaunchSingleFile;
                DeleteButton[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Delete)
                };
                DeleteButton[i].Click += DeleteSingleFile;
                ExportButton[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Save)
                };
                ExportButton[i].Click += ExportSingleFile;

                Grids[i].Children.Add(SelectItems[i]);
                Grids[i].Children.Add(NameBoxes[i]);
                Grids[i].Children.Add(EntryPanel[i]);
                EntryPanel[i].Children.Add(OpenButton[i]);
                EntryPanel[i].Children.Add(LaunchButton[i]);
                EntryPanel[i].Children.Add(DeleteButton[i]);
                EntryPanel[i].Children.Add(ExportButton[i]);
                ContentPanel.Children.Add(Grids[i]);
            }
        }

        private void NavigateSubfolder(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Folders.Length; i++)
            {
                if (sender == FNav[i])
                {
                    Frame.Navigate(typeof(FolderInspectionPage), Folders[i]);
                    return;
                }
            }
        }

        private async void FolderSave_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Folders.Length; i++)
            {
                if (sender == FSave[i])
                {
                    String Token = await StorageInterface.PickExternalStorageFolder();
                    await Folders[i].CopyFolderContentsTo(await (await StorageInterface.GetStorageFolderFromToken(Token)).CreateFolderAsync(Folders[i].Name, CreationCollisionOption.GenerateUniqueName));
                    //await Files[i].CopyAndReplaceAsync(await StorageInterface.GetStorageFileFromToken(Token));
                }
            }
        }

        private async void FolderLaunch_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Folders.Length; i++)
            {
                if (sender == FLaunch[i])
                {
                    Debug.WriteLine(await Launcher.LaunchFolderAsync(Folders[i]));
                }
            }
        }

        private async void FolderDelete_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Folders.Length; i++)
            {
                if (sender == FDelete[i])
                {
                    try
                    {
                        await Folders[i].DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        ex.PrintStackTrace();
                        await UserInteraction.ShowDialogAsync("ERROR", "Fehler beim Löschen des Ordners");
                    }
                    Folders = await SourceFolder.GetStorageFolderArray();
                    ReLoadPage();
                }
            }
            DeactivateMultiSelect();
        }

        private void OpenSingleFile(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Files.Length; i++)
            {
                if (sender == OpenButton[i])
                {
                    Frame.Navigate(typeof(TextEditorPage), Files[i]);
                }
            }
        }

        private async void ExportSingleFile(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Files.Length; i++)
            {
                if (sender == ExportButton[i])
                {
                    String Token = await StorageInterface.PickExternalStorageFile_NewFile(Files[i].DisplayName);
                    try
                    {
                        await Files[i].CopyAndReplaceAsync(await StorageInterface.GetStorageFileFromToken(Token));
                    }
                    catch (Exception ex)
                    {
                        ex.PrintStackTrace();
                    }
                }
            }
        }

        private async void LaunchSingleFile(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Files.Length; i++)
            {
                if (sender == LaunchButton[i])
                {
                    Debug.WriteLine(await Launcher.LaunchFileAsync(Files[i]));
                }
            }
        }

        private async void DeleteSingleFile(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Files.Length; i++)
            {
                if (sender == DeleteButton[i])
                {
                    try
                    {
                        await Files[i].DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        ex.PrintStackTrace();
                        await UserInteraction.ShowDialogAsync("ERROR", "Fehler beim Löschen des Ordners");
                    }
                    Files = await SourceFolder.GetStorageFileArray();
                    ReLoadPage();
                }
            }
            DeactivateMultiSelect();
        }

        private void MultiSelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (MultiSelect)
            {
                DeactivateMultiSelect();
            }
            else
            {
                ActivateMultiSelect();
            }
        }

        private void DeactivateMultiSelect()
        {
            for (int i = 0; i < Files.Length; i++)
            {
                SelectItems[i].Visibility = Visibility.Collapsed;
                NameBoxes[i].Visibility = Visibility.Visible;
            }
            for (int i = 0; i < Folders.Length; i++)
            {
                FSelect[i].Visibility = Visibility.Collapsed;
                //NameBoxes[i].Visibility = Visibility.Visible;
            }
            MultiSelect = false;
        }

        private void ActivateMultiSelect()
        {
            for (int i = 0; i < Files.Length; i++)
            {
                SelectItems[i].Visibility = Visibility.Visible;
                NameBoxes[i].Visibility = Visibility.Collapsed;
                MultiSelect = true;
            }
            for (int i = 0; i < Folders.Length; i++)
            {
                FSelect[i].Visibility = Visibility.Visible;
                //NameBoxes[i].Visibility = Visibility.Visible;
            }
            MultiSelect = true;
        }

        private async void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (MultiSelect)
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    if (SelectItems[i].IsChecked == true)
                    {
                        await Files[i].DeleteAsync();
                    }
                }
                for (int i = 0; i < Folders.Length; i++)
                {
                    if (FSelect[i].IsChecked == true)
                    {
                        await Folders[i].DeleteAsync();
                    }
                }
            }
            else
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    await Files[i].DeleteAsync();
                }
                for (int i = 0; i < Folders.Length; i++)
                {
                    await Folders[i].DeleteAsync();
                }
            }
            DeactivateMultiSelect();
            Files = await SourceFolder.GetStorageFileArray();
            ReLoadPage();
        }

        private async void ExportAllButton_Click(object sender, RoutedEventArgs e)
        {
            String Token = await StorageInterface.PickExternalStorageFolder();
            StorageFolder target = await StorageInterface.GetStorageFolderFromToken(Token);
            target = await target.CreateFolderAsync(SourceFolder.Name, CreationCollisionOption.OpenIfExists);
            if (MultiSelect)
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    if (SelectItems[i].IsChecked == true)
                    {
                        await Files[i].CopyAsync(target, Files[i].Name, NameCollisionOption.ReplaceExisting);
                    }
                }
                for (int i = 0; i < Folders.Length; i++)
                {
                    if (FSelect[i].IsChecked == true)
                    {
                        await Folders[i].CopyFolderContentsTo(await target.CreateFolderAsync(Folders[i].Name, CreationCollisionOption.GenerateUniqueName));
                    }
                }
            }
            else
            {
                await SourceFolder.CopyFolderContentsTo(target);
            }
            DeactivateMultiSelect();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            DeactivateMultiSelect();
            Files = await SourceFolder.GetStorageFileArray();
            Folders = await SourceFolder.GetStorageFolderArray();
            ReLoadPage();
        }

        private async void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            DeactivateMultiSelect();
            String[] Token = await StorageInterface.PickExternalStorageFiles_OpenFiles();
            for (int i = 0; i < Token.Length; i++)
            {
                StorageFile file = await StorageInterface.GetStorageFileFromToken(Token[i]);
                await file.CopyAsync(SourceFolder, file.Name, NameCollisionOption.ReplaceExisting);
            }
            Files = await SourceFolder.GetStorageFileArray();
            ReLoadPage();
        }

        private async void LaunchFolderButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderAsync(SourceFolder);
        }
    }
}