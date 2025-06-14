using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Microsoft.Win32;
using IWshRuntimeLibrary;
using System.Dynamic;
using Microsoft.Deployment.Compression.Cab;

namespace ForgeAir.InstallationWizard
{
    public partial class Form1 : Form
    {
        public string installationPath = @"C:\\Program Files\\ForgeAir";
        public int dbport;
        public string dbpass;
        public Form1()
        {
            installerWorker = new BackgroundWorker();
            installerWorker.WorkerReportsProgress = true;
            installerWorker.WorkerSupportsCancellation = true;
            installerWorker.DoWork += Worker_DoWork;
            installerWorker.ProgressChanged += Worker_ProgressChanged;
            installerWorker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (installerWorker.IsBusy)
                return;

            try
            {
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "ForgeAir_Setup"));
                progressBar1.Value = 5;
                textBox1.AppendText("\nDownloading MariaDB...\n");
                await downloadMariaDB();

                progressBar1.Value = 25;
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("\nInstalling MariaDB...\n");
                await installMariaDB();

                progressBar1.Value = 50;
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("\nInstalling ForgeAir...\n");
                await extractProgram();

                progressBar1.Value = 75;
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("\nFinishing-up...\n");
                await deployUninstaller();

                CreateShortcutDesktop();
                CreateShortcutStartMenu();

                progressBar1.Value = 95;
                await makeConfigFile();

                progressBar1.Value = 100;
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText("\nDone!");

                MessageBox.Show("Installation Complete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during installation:\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private async void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Installation Completed", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Run everything synchronously via Task.Run and Wait()
// Wait synchronously inside DoWork
        }


        private async Task extractProgram()
        {
            Directory.CreateDirectory(installationPath);
            await Task.Run(async () =>
            {
                System.IO.File.WriteAllBytes(Path.Combine(Path.GetTempPath(), "ForgeAir_Setup") + "\\package.cab", Properties.Resources.package);
            });
            string gameCabFile = Path.Combine(Path.GetTempPath(), "ForgeAir_Setup", "package.cab");
            CabInfo gameCab = new CabInfo(gameCabFile);
            gameCab.Unpack(installationPath);
        }

        private async Task deployUninstaller()
        {
            using (RegistryKey parent64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                if (parent64 == null)
                {
                    Console.WriteLine("[ERROR] Uninstall registry key not found.");
                    MessageBox.Show("Uninstall registry key not found. There might be a corruption to your system's registry or you might be attempting to install ForgeAir in a 32-bit system ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    using (RegistryKey key = parent64.OpenSubKey("ForgeAir", true) ?? parent64.CreateSubKey("ForgeAir"))
                    {
                        if (key == null)
                        {
                            Console.WriteLine("[{0}] - UninstallInfo couldn't be deployed to registry. Passing...", DateTime.Now);
                        }
                        Console.WriteLine("[{0}] - Attempting to write UninstallInfo information", DateTime.Now);

                        Assembly asm = Assembly.LoadFrom(Path.Combine(installationPath, "ForgeAir.Playout.dll"));
                        FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(asm.Location);
                        key.SetValue("ApplicationVersion", fileVersion.ProductVersion.ToString());
                        key.SetValue("DisplayName", "ForgeAir");
                        key.SetValue("Publisher", "Karapatakis Aggelos");
                        key.SetValue("DisplayIcon", Path.Combine(installationPath, "ForgeAir.Playout.exe"));
                        key.SetValue("DisplayVersion", key.GetValue("ApplicationVersion"));
                        key.SetValue("URLInfoAbout", "https://chocolateadventurouz.rf.gd");
                        key.SetValue("Contact", "aggeloskarapatakis@outlook.com.gr");
                        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
                        key.SetValue("UninstallString", Path.Combine(installationPath, "uninstall.exe"));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[{0}] - Couldn't write uninstall information. Passing...", DateTime.Now);
                    MessageBox.Show($"An error occurred while writing the uninstall information. ForgeAir is fully installed but it is recommended to re-run the installation wizard again. \nIf you intend to contact support about this error, provide this log below: \n{ex.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private async Task makeConfigFile()
        {
            System.IO.File.WriteAllText(Path.Combine(installationPath, "configuration.ini"), $"""
                [General]
                AutoStart=1
                PlayAutoAtStart=1

                [Database]
                Server=localhost
                Port={dbport}
                Password={dbpass}
                DatabaseName=forgeair

                [Audio]
                MainOutDevice=-1
                MainOutDeviceMethod=WASAPI
                MainOutSampleRate=44100
                MainOutBitDepth=16
                MainOutChannels=2
                MainOutBuffer=333
                MainOutUseDSound=0
                FixClickingWorkAround=1

                [Built-In DSP]
                Enabled=1
                AMStereo=0
                AM=0
                FM=0

                [Butt]
                Path=
                Pre-Text=Now Playing: 
                Post-Text=
                NoTrack-Text=---
                Default-Text=No Playback

                [VST]
                Enabled=0
                EffectPath=

                [Video]
                Enabled=0
                UseOverlay=0
                StretchToWidescreen=1
                ResolutionW=1280
                ResolutionH=720
                ShowClock=0
                UseLogo=0
                OverlayPath=
                LogoPath=
                
                """);
            return;
        }
        private async Task downloadMariaDB()
        {
            await Task.Run(() => Utils.DownloadFileTaskAsync(new HttpClient(), new Uri("https://archive.mariadb.org///mariadb-11.4.4/winx64-packages/mariadb-11.4.4-winx64.msi"), Path.Combine(Path.GetTempPath(), "mariadb_setup.msi")));
            return;
        }
        private void CreateShortcutDesktop()
        {
            string link = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                + Path.DirectorySeparatorChar + "ForgeAir" + ".lnk";
            var shell = new WshShell();
            var shortcut = shell.CreateShortcut(link) as IWshShortcut;
            shortcut.TargetPath = installationPath + "\\ForgeAir.Playout.exe";
            shortcut.IconLocation = installationPath + "\\ForgeAir.Playout.exe";
            shortcut.WorkingDirectory = installationPath;
            Thread.Sleep(333);
            shortcut.Save();
        }
        private void CreateShortcutStartMenu()
        {
            string link = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)
                + Path.DirectorySeparatorChar + "ForgeAir" + ".lnk";
            var shell = new WshShell();
            var shortcut = shell.CreateShortcut(link) as IWshShortcut;
            shortcut.TargetPath = installationPath + "\\ForgeAir.Playout.exe";
            shortcut.IconLocation = installationPath + "\\ForgeAir.Playout.exe";
            shortcut.WorkingDirectory = installationPath;
            Thread.Sleep(333);
            shortcut.Save();
        }

        private async Task installMariaDB()
        {
            string msiPath = Path.Combine(Path.GetTempPath(), "mariadb_setup.msi");
            if (!System.IO.File.Exists(msiPath))
                throw new FileNotFoundException("MariaDB installer not found.");

            DatabaseSetup databaseSetup = new DatabaseSetup();
            if (databaseSetup.ShowDialog() != DialogResult.OK)
                throw new OperationCanceledException("Database setup canceled by user.");

            if (!int.TryParse(databaseSetup.dbPortBox.Text, out dbport) || string.IsNullOrWhiteSpace(databaseSetup.dbPasswordBox.Text))
                throw new ArgumentException("Invalid database port or password.");

            dbpass = databaseSetup.dbPasswordBox.Text;

            Process mariadbSetup = new Process();
            mariadbSetup.StartInfo.FileName = "msiexec";
            mariadbSetup.StartInfo.Arguments = $"/i \"{msiPath}\" PORT={dbport} PASSWORD={dbpass} ADDLOCAL=ALL REMOVE=HeidiSQL /qn /l*v mariadb_install.log";
            mariadbSetup.Start();
            await mariadbSetup.WaitForExitAsync();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}


