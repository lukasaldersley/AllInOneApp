using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ToDoListPage : Page
    {
        public static readonly String INTRASPLIT = "§§##$$##§§";
        public static readonly String INTERSPLIT = "$$##§§##$$";

        Border newBorder;
        Grid newGrid;
        ColumnDefinition newLeft;
        ColumnDefinition newRight;
        StackPanel newLeftPanel;
        TextBox newOverview;
        TextBox newDetails;
        StackPanel newRightPanel;
        AppBarButton newDelete;
        AppBarButton newEditOk;

        Border[] border;
        Grid[] grid;
        ColumnDefinition[] left;
        ColumnDefinition[] right;
        StackPanel[] leftPanel;
        TextBox[] overview;
        TextBox[] details;
        StackPanel[] rightPanel;
        AppBarButton[] delete;
        AppBarButton[] editOk;
        Boolean[] isEditing;

        int index = 0;

        String cumulatedData = "";

        public ToDoListPage()
        {
            this.InitializeComponent();
            Refresh().GetAwaiter();
        }

        private async Task Upload()
        {
            await ServerInterface.UploadFile(await StorageInterface.GetStorageFileFromLocalStorage("ToDo.list"),"/home/aio/AIO/ToDo.list");
        }

        private async Task Download()
        {
            ServerInterface.ServerPacket sp = ServerInterface.ReadFile("/home/aio/AIO/ToDo.list");
            if (sp.Flags[1] == ServerInterface.ACK) {
                await StorageInterface.WriteBytesToLocalFolder("ToDo.list",sp.Message);
            }
        }

        private void Init()
        {
            newBorder = new Border()
            {
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(5),
                Margin = new Thickness(5)
            };

            newGrid = new Grid();

            newLeft = new ColumnDefinition();

            newRight = new ColumnDefinition()
            {
                Width = GridLength.Auto
            };

            newLeftPanel = new StackPanel();

            newOverview = new TextBox()
            {
                FontSize = 20,
                //BorderThickness = new Thickness(0),
                Text = ""
            };

            newDetails = new TextBox()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = "",
                AcceptsReturn = true
                //BorderThickness=new Thickness(0)
            };

            newRightPanel = new StackPanel();

            newDelete = new AppBarButton()
            {
                Icon = new SymbolIcon(Symbol.Cancel),
                Content = "Abbrechen"
            };

            newEditOk = new AppBarButton()
            {
                Icon = new SymbolIcon(Symbol.Save),
                Content = "Speichern"
            };

            Stack.Children.Add(newBorder);

            newBorder.Visibility = Visibility.Collapsed;
            newBorder.Child = newGrid;

            newGrid.ColumnDefinitions.Add(newLeft);
            newGrid.ColumnDefinitions.Add(newRight);

            newGrid.Children.Add(newLeftPanel);

            newLeftPanel.Children.Add(newOverview);
            newLeftPanel.Children.Add(newDetails);
            newLeftPanel.SetValue(Grid.ColumnProperty, 0);

            newGrid.Children.Add(newRightPanel);

            newRightPanel.Children.Add(newDelete);
            newRightPanel.Children.Add(newEditOk);
            newRightPanel.SetValue(Grid.ColumnProperty, 1);

            newEditOk.Click += StoreNewEntry;
            newDelete.Click += CancelNewEntry;
        }

        private void CancelNewEntry(object sender, RoutedEventArgs e)
        {
            newOverview.Text = "";
            newDetails.Text = "";
            newBorder.Visibility = Visibility.Collapsed;
        }

        private async void StoreNewEntry(object sender, RoutedEventArgs e)
        {
            newEditOk.Click -= StoreNewEntry;
            newDelete.Click -= CancelNewEntry;
            cumulatedData = await StorageInterface.ReadFromLocalFolder("ToDo.list");
            cumulatedData = newOverview.Text + INTRASPLIT + newDetails.Text + INTERSPLIT + cumulatedData;
            await StorageInterface.WriteToLocalFolder("ToDo.list", cumulatedData);
            await Upload();
            await Refresh();
        }

        private async Task Refresh()
        {
            Stack.Children.Clear();
            Init();
            await Download();
            cumulatedData = await StorageInterface.ReadFromLocalFolder("ToDo.list");
            if (cumulatedData == null)
            {
                await StorageInterface.WriteToLocalFolder("ToDo.list", "");
                await Upload();
            }
            String[] entries = cumulatedData.Split(INTERSPLIT);

            index = entries.Length;

            border = new Border[index];
            grid = new Grid[index];
            left = new ColumnDefinition[index];
            right = new ColumnDefinition[index];
            leftPanel = new StackPanel[index];
            overview = new TextBox[index];
            details = new TextBox[index];
            rightPanel = new StackPanel[index];
            delete = new AppBarButton[index];
            editOk = new AppBarButton[index];
            isEditing = new Boolean[index];

            for (int i = 0; i < index; i++)
            {
                if (entries[i].Equals(""))
                {
                    continue;
                }
                if (!entries[i].Contains(INTRASPLIT))
                {
                    entries[i] += INTRASPLIT + " ";
                }
                if (entries[i].EndsWith(INTRASPLIT))
                {
                    entries[i] += " ";
                }
                String[] X = entries[i].Split(INTRASPLIT);
                /*for(int j = 0; j < X.Length; j++)
                {
                    if (X[j].Equals(""))
                    {
                        X[j] = " ";
                    }
                }*/



                border[i] = new Border()
                {
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush(Colors.LightGray),
                    CornerRadius = new CornerRadius(15),
                    Padding = new Thickness(5),
                    Margin = new Thickness(5)
                };

                grid[i] = new Grid();

                left[i] = new ColumnDefinition();

                right[i] = new ColumnDefinition()
                {
                    Width = GridLength.Auto
                };

                leftPanel[i] = new StackPanel();

                overview[i] = new TextBox()
                {
                    IsHitTestVisible = false,
                    IsReadOnly = true,
                    FontSize = 20,
                    //BorderThickness = new Thickness(0),
                    Text = X[0]
                };

                details[i] = new TextBox()
                {
                    IsHitTestVisible = false,
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap,
                    Text = X[1]
                };

                rightPanel[i] = new StackPanel();

                delete[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Delete),
                    Content = "Erledigt"
                };

                editOk[i] = new AppBarButton()
                {
                    Icon = new SymbolIcon(Symbol.Edit),
                    Content = "Bearbeiten"
                };

                Stack.Children.Add(border[i]);

                border[i].Child = grid[i];

                grid[i].ColumnDefinitions.Add(left[i]);
                grid[i].ColumnDefinitions.Add(right[i]);

                grid[i].Children.Add(leftPanel[i]);

                leftPanel[i].Children.Add(overview[i]);
                leftPanel[i].Children.Add(details[i]);
                leftPanel[i].SetValue(Grid.ColumnProperty, 0);

                grid[i].Children.Add(rightPanel[i]);

                rightPanel[i].Children.Add(delete[i]);
                rightPanel[i].Children.Add(editOk[i]);
                rightPanel[i].SetValue(Grid.ColumnProperty, 1);

                delete[i].Click += Delete_Click;

                editOk[i].Click += EditOk_Click;
            }
        }

        private async void EditOk_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < index; i++)
            {
                if (sender.Equals(editOk[i]))
                {
                    overview[i].IsReadOnly = isEditing[i];
                    details[i].IsReadOnly = isEditing[i];
                    isEditing[i] = !isEditing[i];//Ja, das ist bewusst in der mitte, da ISR activelow und ISHT activehigh ist
                    overview[i].IsHitTestVisible = isEditing[i];
                    details[i].IsHitTestVisible = isEditing[i];
                    if (!isEditing[i])
                    {
                        await SaveCurrentContents();
                    }
                    else
                    {
                        editOk[i].Icon = new SymbolIcon(Symbol.Accept);
                    }
                    break;
                }
            }
            //Debug.WriteLine("NOT IMPLEMENTED YET");
            //await UserInteraction.ShowDialogAsync("ERROR", "THIS FEATURE IS NOT IMPLEMENTED IN THIS VERSION, PLEASE CHECK AGAIN AFTER THE NEXT UPDATE");
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < index; i++)
            {
                if (sender.Equals(delete[i]))
                {
                    overview[i].Text = "";
                    details[i].Text = "";
                }
            }
            await SaveCurrentContents();
            //Debug.WriteLine("NOT IMPLEMENTED YET");
            // await UserInteraction.ShowDialogAsync("ERROR", "THIS FEATURE IS NOT IMPLEMENTED IN THIS VERSION, PLEASE CHECK AGAIN AFTER THE NEXT UPDATE");
        }

        private async Task SaveCurrentContents()
        {
            String contentsToSTore = "";
            for (int i = 0; i < index; i++)
            {
                try
                {
                    if (overview[i].Text.Equals("") && details[i].Text.Equals(""))
                    {
                        continue;
                    }
                    contentsToSTore += overview[i].Text + INTRASPLIT + details[i].Text + INTERSPLIT;
                }
                catch
                {
                    //DO NOTHING THE EXCEPTION PROBABLY HAS BEEN CAUSED BECAUSE THERE WAS NO DATA SO THE REFRESH METHOD JUST SKIPPED THIS DATASET
                }
            }
            await StorageInterface.WriteToLocalFolder("ToDo.list", contentsToSTore);
            await Upload();
            await Refresh();
        }

        private void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            newBorder.Visibility = Visibility.Visible;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }
    }
}
