using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Linq;

using System.Windows.Forms;
using Path = System.IO.Path;

namespace PSPXSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game activePSPSelection = null;
        Game activeEmuSelection = null;
        bool sendToPSP = true;
        bool haltSettingChange = false;

        public MainWindow()
        {
            InitializeComponent();

            Game.Load();

            Settings.Load();
                
            TextPSXDir.Text = Settings.Contains("PSXDIR") ? Settings.Get("PSXDIR").ToString() : "Not Set";
            TextPSPDir.Text = Settings.Contains("PSPDIR") ? Settings.Get("PSPDIR").ToString() : "Not Set";

            PopulateEmuCardList();
            PopulatePSPList();

            DetermineLatest();
        }


        public void DetermineLatest()
        {
            if(activeEmuSelection != null && activePSPSelection != null)
            {
                sendToPSP = activePSPSelection.MemCard.Modified >= activeEmuSelection.MemCard.Modified;
            }
            else
            {
                return;
            }

            TextEmu.Text = sendToPSP ? "Older" : "Latest";
            TextPSP.Text = sendToPSP ? "Latest" : "Older";

            TextEmu.Visibility = sendToPSP ? Visibility.Hidden : Visibility.Visible;
            TextPSP.Visibility = sendToPSP ? Visibility.Visible : Visibility.Hidden;

            TextEmu.IsReadOnly = true;
            TextPSP.IsReadOnly = true;


            ButtonRunFromPspToEmu.BorderThickness = sendToPSP ? new Thickness(10) : new Thickness(1);
            ButtonRunFromEmuToPsp.BorderThickness = sendToPSP ? new Thickness(1) : new Thickness(10);

            ButtonRunFromPspToEmu.Margin = !sendToPSP ? new Thickness(10) : new Thickness(1);
            ButtonRunFromEmuToPsp.Margin = !sendToPSP ? new Thickness(1) : new Thickness(10);
        }

        private void PopulatePSPList()
        {
            haltSettingChange = true;
            ListPSPCards.Items.Clear();
            if (Directory.Exists(Settings.Get("PSPDIR").ToString()))
            {
                List<Game> games = new List<Game>();

                foreach (string mem in Directory.GetDirectories(Settings.Get("PSPDIR").ToString()))
                {
                    string dir = Path.GetFileName(mem);

                    if(Game.GameIDs.ContainsKey(dir))
                    {
                        games.Add(new Game(Game.GameIDs[dir].Name, Game.GameIDs[dir].ID, mem));
                    }
                    else
                    {
                        games.Add(new Game(dir, dir, mem));
                    }
                }

                games.Sort();


                if (Settings.Contains("PSPCARD"))
                {
                    try
                    {
                        activePSPSelection = Newtonsoft.Json.JsonConvert.DeserializeObject<Game>(
                            Settings.Get("PSPCARD").ToString()
                            );

                        // refresh modified timestamp with latest file
                        FileInfo f = new FileInfo(activePSPSelection.MemCard.Path);
                        activePSPSelection.MemCard.Modified = f.LastWriteTime;
                    }
                    catch(Exception ex)
                    {

                    }
                }

                foreach(Game game in games)
                {
                    ListPSPCards.Items.Add(game);

                    if(activePSPSelection != null && game.ID == activePSPSelection.ID)
                    {
                        ListPSPCards.SelectedItem = ListPSPCards.Items[ListPSPCards.Items.Count - 1];
                        ListPSPCards.Focus();

                    }

                }
            }
            else
            {
                TextPSPDir.Foreground = Brushes.Red;
            }
            haltSettingChange = false;
        }
        private void PopulateEmuCardList()
        {
            haltSettingChange = true;
            ListEmuCards.Items.Clear();

            if (Directory.Exists(Settings.Get("PSXDIR").ToString()))
            {
                List<Game> games = new List<Game>();

                foreach (string file in Directory.GetFiles(Settings.Get("PSXDIR").ToString()))
                {
                    if (file.EndsWith("mcr"))
                    {
                        //ListEmuCards.Items.Add(Path.GetFileName(file));
                        games.Add(new Game(Path.GetFileName(file), "", file));



                    }
                }


                if (Settings.Contains("EMUCARD"))
                {
                    try
                    {
                        activeEmuSelection = Newtonsoft.Json.JsonConvert.DeserializeObject<Game>(
                            Settings.Get("EMUCARD").ToString()
                            );
                        
                        // refresh modified timestamp with latest file
                        FileInfo f = new FileInfo(activeEmuSelection.MemCard.Path);
                        activeEmuSelection.MemCard.Modified = f.LastWriteTime;
                    }
                    catch (Exception ex)
                    {

                    }
                }

                foreach (Game g in games)
                {
                    ListEmuCards.Items.Add(g);

                    if (activeEmuSelection != null && g.Name == activeEmuSelection.Name)
                    {
                        ListEmuCards.SelectedItem = ListEmuCards.Items[ListEmuCards.Items.Count - 1];
                        //ListEmuCards.Focus();
                    }
                }
            }
            else
            {
                TextPSXDir.Foreground = Brushes.Red;
            }
            haltSettingChange = false;
        }

        private void btnPSXDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if(string.Equals(Path.GetFileName(dialog.SelectedPath),"psx", StringComparison.OrdinalIgnoreCase))
                {
                    dialog.SelectedPath = Path.Combine(dialog.SelectedPath, "cards");
                }

                if (string.Equals(Path.GetFileName(dialog.SelectedPath), "epsxe", StringComparison.OrdinalIgnoreCase))
                {
                    dialog.SelectedPath = Path.Combine(dialog.SelectedPath, "memcards");
                }

                Settings.Set("PSXDIR", dialog.SelectedPath);
                TextPSXDir.Text = dialog.SelectedPath;
                TextPSXDir.Foreground = Brushes.Black;

                Settings.Save();
            
                PopulateEmuCardList();
            }

        }

        private void btnPSPDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if(!string.Equals(Path.GetFileName(dialog.SelectedPath), "SAVEDATA", StringComparison.OrdinalIgnoreCase))
                {
                    dialog.SelectedPath = Path.Combine(dialog.SelectedPath, "PSP\\SAVEDATA");
                }

                Settings.Set("PSPDIR", dialog.SelectedPath);
                TextPSPDir.Text = dialog.SelectedPath;
                TextPSPDir.Foreground = Brushes.Black;

                Settings.Save();

                PopulatePSPList();
            }
        }

        private void ListPSPCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (haltSettingChange) return;

            //string activeID = e.AddedItems[0];
            Settings.Set("PSPCARD", e.AddedItems[0] as Game);

            activePSPSelection = e.AddedItems[0] as Game;

            Settings.Save();
            DetermineLatest();
        }

        private void ListEmuCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (haltSettingChange) return;

            Settings.Set("EMUCARD", e.AddedItems[0] as Game);
            
            activeEmuSelection = e.AddedItems[0] as Game;

            Settings.Save();
            DetermineLatest();
        }

        private void ButtonRunFromEmuToPsp_Click(object sender, RoutedEventArgs e)
        {
            Log.Text = "";
            if (activePSPSelection == null)
            {
                Log.Text = "ABORT: No left-hand PSX emulation selection";
                return;
            }
            try
            {
                Backup(false);

                Log.Text += "\nConverting from MCR (PSX) to VMP (PSP)";

                Log.Text += $"\nmcr2vmp.exe {activeEmuSelection.MemCard.Path}";
                Converter.Run(activeEmuSelection.MemCard.Path);

                System.Threading.Thread.Sleep(2000);

                Log.Text += $"\nReplacing: {activePSPSelection.MemCard.Path}";
                File.Delete(activePSPSelection.MemCard.Path);
                File.Move(activeEmuSelection.MemCard.Path + ".VMP", activePSPSelection.MemCard.Path);
            }
            catch(Exception ex)
            {
                Log.Text += $"\nFailure: {ex.Message}";
            }

            PopulatePSPList();
            DetermineLatest();
        }

        private void ButtonRunFromPspToEmu_Click(object sender, RoutedEventArgs e)
        {
            Log.Text = "";
            
            if(activePSPSelection == null)
            {
                Log.Text = "ABORT: No right-hand PSP selection";
                return;
            }

            try
            {
                Backup(true);

                Log.Text += "\nConverting from VMP (PSP) to MCR (PSX)";

                Log.Text += $"\nmcr2vmp.exe {activePSPSelection.MemCard.Path}";
                Converter.Run(activePSPSelection.MemCard.Path);

                System.Threading.Thread.Sleep(2000);

                Log.Text += $"\nReplacing: {activeEmuSelection.MemCard.Path}";
                File.Delete(activeEmuSelection.MemCard.Path);
                File.Move(activePSPSelection.MemCard.Path + ".mcr", activeEmuSelection.MemCard.Path);

            }
            catch (Exception ex)
            { 
                Log.Text += $"\nFailure: {ex.Message}";
            }

            PopulateEmuCardList();
            DetermineLatest();
        }

        private void Backup(bool isPspSource)
        {
            if(activeEmuSelection == null || activePSPSelection == null)
            {
                return;
            }

            string backupDir = Directory.GetCurrentDirectory();
            backupDir = Path.Combine(backupDir, "Backup");

            FileInfo backupSource ;
            FileInfo backupTarget ;

            if(isPspSource)
            {
                backupSource = new FileInfo(activePSPSelection.MemCard.Path);
                backupTarget = new FileInfo(activeEmuSelection.MemCard.Path);
            }
            else
            {
                backupSource = new FileInfo(activeEmuSelection.MemCard.Path);
                backupTarget = new FileInfo(activePSPSelection.MemCard.Path);
            }

            DateTime now = DateTime.Now;

            //File.Copy(backupSourceName Path.Combine())

            FileInfo newBackupSource = new FileInfo(
            Path.Combine(backupDir, $"{Path.GetFileNameWithoutExtension(backupSource.Name)}_source_{now.ToString("yyyyMMddHHmmssfff")}{Path.GetExtension(backupSource.Name)}"));

            FileInfo newBackupTarget = new FileInfo(
            Path.Combine(backupDir, $"{Path.GetFileNameWithoutExtension(backupTarget.Name)}_target_{now.ToString("yyyyMMddHHmmssfff")}{Path.GetExtension(backupTarget.Name)}"));

            if(!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            File.Copy(backupSource.FullName, newBackupSource.FullName);
            File.Copy(backupTarget.FullName, newBackupTarget.FullName);

            Log.Text = "";

            Log.Text = $"Backup: {newBackupSource.FullName}";
            Log.Text += $"\nBackup: {newBackupTarget.FullName}";
        }
    }
}
