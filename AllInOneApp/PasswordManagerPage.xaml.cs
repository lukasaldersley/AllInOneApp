using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.Security.Cryptography.Core;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.System;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class PasswordManagerPage : Page
    {
        private CryptographicKey Key;
        public static readonly String INTRASPLIT = "$$##§§##$$";
        public static readonly String INTERSPLIT = "##!!||!!##";

        Border newBorder;
        Grid newGrid;
        ColumnDefinition newLeft;
        ColumnDefinition newRight;
        StackPanel newLeftPanel;
        TextBox newOverview;
        TextBox newUsername;
        TextBox newPassword;
        StackPanel newRightPanel;
        AppBarButton newDelete;
        AppBarButton newEditOk;

        Border[] border;
        Grid[] grid;
        ColumnDefinition[] left;
        ColumnDefinition[] right;
        StackPanel[] leftPanel;
        TextBox[] overview;
        TextBox[] username;
        TextBox[] password;
        StackPanel[] rightPanel;
        AppBarButton[] delete;
        AppBarButton[] editOk;
        Boolean[] isEditing;

        int index = 0;

        public PasswordManagerPage()
        {
            this.InitializeComponent();
            Setup();
        }

        private async void Setup()
        {
            Key = null;
            if (await CryptoInterface.IsInitial())
            {
                Debug.WriteLine("INITIAL STATE DETECTED");
                MODE_SETUP.Visibility = Visibility.Visible;
                MODE_LOGIN.Visibility = MODE_REGULAR.Visibility = MODE_REGULAR_BAR.Visibility = Visibility.Collapsed;
            }
            else
            {
                Debug.WriteLine("NOT INITIAL STATE DETECTED");
                MODE_LOGIN.Visibility = Visibility.Visible;
                MODE_SETUP.Visibility = MODE_REGULAR.Visibility = MODE_REGULAR_BAR.Visibility = Visibility.Collapsed;
            }
        }

        private void SetupPasswordBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            SetupOutput.Text = (SetupAPasswordBox.Password.Equals("") || SetupBPasswordBox.Password.Equals("")) ? "ALLE FELDER AUSFÜLLEN" : (SetupAPasswordBox.Password.Equals(SetupBPasswordBox.Password) ? "" : "PASSWORDS DON'T MATCH");
        }

        private async void InitialSetupButton_Click(object sender, RoutedEventArgs e)
        {
            String PWD = SetupAPasswordBox.Password;
            Debug.WriteLine(PWD);
            Key = CryptoInterface.GestAesKey(PWD);
            IBuffer X = CryptoInterface.CreateTestfile();
            await StorageInterface.WriteBufferToRoamingFolder("PWM/A", X);
            await StorageInterface.WriteBufferToRoamingFolder("PWM/B", CryptoInterface.EncryptAes(Key, X));
            await CryptoInterface.InitialSetup(Key);
            await CryptoInterface.StorePasswords("Dieser Passwortmanager" + INTRASPLIT + "Kein Benutzername" + INTRASPLIT + PWD + INTERSPLIT, Key);
            Debug.WriteLine("HAVE DONE SETUP WITH PASSWORD");
            MODE_REGULAR.Visibility = MODE_REGULAR_BAR.Visibility = Visibility.Visible;
            MODE_LOGIN.Visibility = MODE_SETUP.Visibility = Visibility.Collapsed;
            await Refresh();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await Logon();
        }

        private async Task Logon() {
            Key = CryptoInterface.GestAesKey(LoginPasswordBox.Password);
            LoginPasswordBox.Password = "";
            IBuffer A = await StorageInterface.ReadBufferFromRoamingFolder("PWM/A");
            IBuffer B = await StorageInterface.ReadBufferFromRoamingFolder("PWM/B");
            IBuffer EA = CryptoInterface.EncryptAes(Key, A);
            if (!CryptographicBuffer.Compare(B, EA))//Falsches Passwort, da die testfiles nicht zampassen
            {
                Debug.WriteLine("FALSCHES PASSWORT");
                await UserInteraction.ShowDialogAsync("INFORMAtiON", "Falsches Passwort!");
                return;
            }
            Debug.WriteLine("HAVE DONE SETUP WITH PASSWORD");
            MODE_REGULAR.Visibility = MODE_REGULAR_BAR.Visibility = Visibility.Visible;
            MODE_LOGIN.Visibility = MODE_SETUP.Visibility = Visibility.Collapsed;
            await Refresh();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Setup();
        }

        private async void Test_Click(object sender, RoutedEventArgs e)
        {
            String Passwords = await CryptoInterface.GetPasswords(Key);
            Debug.WriteLine("<passwords>");
            Debug.WriteLine(Passwords);
            Debug.WriteLine("</passwords>");
            Passwords += "LINE1" + INTRASPLIT + "LINE2" + INTRASPLIT + "LINE3" + INTERSPLIT;
            Debug.WriteLine("ALTERED PASSWORDS");
            Debug.WriteLine("<passwords>");
            Debug.WriteLine(Passwords);
            Debug.WriteLine("</passwords>");
            await CryptoInterface.StorePasswords(Passwords, Key);
            Debug.WriteLine("WROTE NEW PASSWORDS");
            Passwords = "";
            Debug.WriteLine("<clearedPasswords>");
            Debug.WriteLine(Passwords);
            Debug.WriteLine("</clearedPasswords>");
            Passwords = await CryptoInterface.GetPasswords(Key);
            Debug.WriteLine("READ NEW PASSWORDS");
            Debug.WriteLine("<passwords>");
            Debug.WriteLine(Passwords);
            Debug.WriteLine("</passwords>");
        }

        private async Task Refresh()
        {
            Stack.Children.Clear();
            Init();

            String cumulatedData = await CryptoInterface.GetPasswords(Key);
            String[] entries = cumulatedData.Split(INTERSPLIT);

            index = entries.Length;

            border = new Border[index];
            grid = new Grid[index];
            left = new ColumnDefinition[index];
            right = new ColumnDefinition[index];
            leftPanel = new StackPanel[index];
            overview = new TextBox[index];
            username = new TextBox[index];
            password = new TextBox[index];
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
                if (!(entries[i].Substring(INTRASPLIT.Length + entries[i].IndexOf(INTRASPLIT)).Contains(INTRASPLIT)))
                {
                    entries[i] += INTRASPLIT + " ";
                }
                String[] X = entries[i].Split(INTRASPLIT);



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

                username[i] = new TextBox()
                {
                    IsHitTestVisible = false,
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap,
                    Text = X[1]
                };

                password[i] = new TextBox()
                {
                    IsHitTestVisible = false,
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap,
                    Text = X[2]
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
                leftPanel[i].Children.Add(username[i]);
                leftPanel[i].Children.Add(password[i]);
                leftPanel[i].SetValue(Grid.ColumnProperty, 0);

                grid[i].Children.Add(rightPanel[i]);

                rightPanel[i].Children.Add(delete[i]);
                rightPanel[i].Children.Add(editOk[i]);
                rightPanel[i].SetValue(Grid.ColumnProperty, 1);

                delete[i].Click += Delete_Click;

                editOk[i].Click += EditOk_Click;
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

            newUsername = new TextBox()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = "",
                AcceptsReturn = true
                //BorderThickness=new Thickness(0)
            };

            newPassword = new TextBox()
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
            newLeftPanel.Children.Add(newUsername);
            newLeftPanel.Children.Add(newPassword);
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
            newUsername.Text = "";
            newPassword.Text = "";
            newBorder.Visibility = Visibility.Collapsed;
        }

        private async void StoreNewEntry(object sender, RoutedEventArgs e)
        {
            newEditOk.Click -= StoreNewEntry;
            newDelete.Click -= CancelNewEntry;
            String cumulatedData = await CryptoInterface.GetPasswords(Key);
            if (newUsername.Text.Equals(""))
            {
                newUsername.Text = "Kein Benutzername";
            }
            if (newPassword.Text.Equals(""))
            {
                newPassword.Text = "Kein Passwort";
            }
            if (newOverview.Text.Equals(""))
            {
                newOverview.Text = "Kein Name";
            }
            cumulatedData = newOverview.Text + INTRASPLIT + newUsername.Text + INTRASPLIT + newPassword.Text + INTERSPLIT + cumulatedData;
            await CryptoInterface.StorePasswords(cumulatedData, Key);
            await Refresh();
        }

        private async void EditOk_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < index; i++)
            {
                if (sender.Equals(editOk[i]))
                {
                    overview[i].IsReadOnly = isEditing[i];
                    username[i].IsReadOnly = isEditing[i];
                    password[i].IsReadOnly = isEditing[i];
                    isEditing[i] = !isEditing[i];//Ja, das ist bewusst in der mitte, da ISR activelow und ISHT activehigh ist
                    overview[i].IsHitTestVisible = isEditing[i];
                    username[i].IsHitTestVisible = isEditing[i];
                    password[i].IsHitTestVisible = isEditing[i];
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
                    username[i].Text = "";
                    password[i].Text = "";
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
                    if (overview[i].Text.Equals("") && username[i].Text.Equals("") && password[i].Text.Equals(""))
                    {
                        continue;//alle felder leer => wurde gelöscht
                    }
                    if (overview[i].Text.Equals(""))
                    {
                        overview[i].Text = "Kein Name";
                    }
                    if (username[i].Text.Equals(""))
                    {
                        username[i].Text = "Kein Benutzername";
                    }
                    if (password[i].Text.Equals(""))
                    {
                        password[i].Text = "Kein Passwort";
                    }
                    contentsToSTore += overview[i].Text + INTRASPLIT + username[i].Text + INTRASPLIT + password[i].Text + INTERSPLIT;
                }
                catch
                {
                    //DO NOTHING THE EXCEPTION PROBABLY HAS BEEN CAUSED BECAUSE THERE WAS NO DATA SO THE REFRESH METHOD JUST SKIPPED THIS DATASET
                }
            }
            if (!contentsToSTore.Equals(""))
            {
                await CryptoInterface.StorePasswords(contentsToSTore, Key);
            }
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

        private async void LoginPasswordBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                await Logon();
            }
        }
    }
}
