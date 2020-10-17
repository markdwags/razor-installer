#region license

// Razor Installer
// Copyright (C) 2020 Razor Development Community on GitHub <https://github.com/markdwags/razor-installer>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Octokit;
using RazorInstaller.Helpers;
using RazorInstaller.Models;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RazorInstaller.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand InstallUpdateCommand { get; set; }
        public RelayCommand BrowseInstallPathCommand { get; set; }
        public RelayCommand BrowseUOPathCommand { get; set; }
        public RelayCommand WebsiteCommand { get; set; }

        private readonly IDialogCoordinator _dialogCoordinator;

        public string CUOVersion { get; set; }
        public string RazorVersion { get; set; }
        public bool UseRazorPreview { get; set; }
        public bool UseCUOPreview { get; set; }

        private string _installPath;
        public string InstallPath
        {
            get { return _installPath; }
            private set
            {
                _installPath = value;
                RaisePropertyChanged();
            }
        }

        private string _uoDataPath;
        public string UODataPath
        {
            get { return _uoDataPath; }
            private set
            {
                _uoDataPath = value;
                RaisePropertyChanged();
            }
        }

        private double _currentProgress;
        public double CurrentProgress
        {
            get { return _currentProgress; }
            private set
            {
                _currentProgress = value;
                RaisePropertyChanged();
            }
        }

        private string _serverHost;
        public string ServerHost
        {
            get { return _serverHost; }
            set
            {
                _serverHost = value;
                RaisePropertyChanged();
            }
        }

        private string _serverPort;
        public string ServerPort
        {
            get { return _serverPort; }
            set
            {
                _serverPort = value;
                RaisePropertyChanged();
            }
        }

        private bool _isButtonEnabled;
        public bool IsButtonEnabled
        {
            get { return _isButtonEnabled; }
            private set
            {
                _isButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        private bool _installCUO;
        public bool InstallCUO
        {
            get { return _installCUO; }
            private set
            {
                _installCUO = value;
                RaisePropertyChanged();
            }
        }

        private string _clientVersion;
        public string ClientVersion
        {
            get { return _clientVersion; }
            set
            {
                _clientVersion = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDialogCoordinator dialogCoordinator)
        {

            Release razor = GitHubHelper.GetLatestRazorVersion(false);
            Release cuo = GitHubHelper.GetLatestClassicUOVersion(false);

            RazorVersion = $"Razor {razor.TagName} ({razor.PublishedAt:g})";
            CUOVersion = $"ClassicUO {cuo.TagName} ({cuo.PublishedAt:g})";

            InstallUpdateCommand = new RelayCommand(DoConfirmInstallUpdate);
            BrowseInstallPathCommand = new RelayCommand(DoSetInstallPath);
            BrowseUOPathCommand = new RelayCommand(DoSetUODataPath);
            WebsiteCommand = new RelayCommand(DoWebsite);

            _dialogCoordinator = dialogCoordinator;

            IsButtonEnabled = true;
            InstallCUO = true;

            InstallPath = ConfigurationManager.AppSettings["InstallPath"];
            UODataPath = ConfigurationManager.AppSettings["UODataPath"];
            ServerHost = ConfigurationManager.AppSettings["ServerHost"];
            ServerPort = ConfigurationManager.AppSettings["ServerPort"];
            ClientVersion = ConfigurationManager.AppSettings["ClientVersion"];
        }

        private void DoSetInstallPath()
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog
            {
                Title = "Select install folder:",
                IsFolderPicker = true,
                InitialDirectory = InstallPath,

                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = InstallPath,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                InstallPath = dlg.FileName;
            }
        }

        private void DoSetUODataPath()
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog
            {
                Title = "Select UO folder:",
                IsFolderPicker = true,
                InitialDirectory = UODataPath,

                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = UODataPath,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                UODataPath = dlg.FileName;
            }
        }

        private void DoWebsite()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "http://www.uor-razor.com"
                };

                Process.Start(startInfo);
            }
            catch
            {
                //ignore
            }
        }

        public void DoConfirmInstallUpdate()
        {
            if (string.IsNullOrEmpty(InstallPath) || string.IsNullOrEmpty(UODataPath) || string.IsNullOrEmpty(ServerHost) || string.IsNullOrEmpty(ServerPort) || string.IsNullOrEmpty(ClientVersion))
            {
                MessageBox.Show("You must define an install path, UO data folder, client version, server host and/or port.", "Missing Fields", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ShowInstallDialogCommand.CanExecute(null))
                ShowInstallDialogCommand.Execute(null);
        }

        private ICommand showInstallDialogCommand { get; set; }

        public ICommand ShowInstallDialogCommand
        {
            get
            {
                return showInstallDialogCommand ?? (showInstallDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => PerformDialogCoordinatorAction(ShowInstallMessage((string)x), (string)x == "DISPATCHER_THREAD")
                });
            }
        }

        private Action ShowInstallMessage(string startingThread)
        {
            return () =>
            {
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    ColorScheme = MetroDialogColorScheme.Theme,
                    DefaultButtonFocus = MessageDialogResult.Affirmative
                };

                string title = InstallCUO ? "Install Razor and ClassicUO" : "Install Razor";
                string msg = InstallCUO ? "Would you like to download and install ClassicUO and Razor?" : "Would you like to download and install Razor?";

                MessageDialogResult result = _dialogCoordinator.ShowMessageAsync(this, title, msg,
                    MessageDialogStyle.AffirmativeAndNegative, mySettings).Result;

                if (result != MessageDialogResult.Negative)
                {
                    DoInstallUpdate();
                }
            };
        }

        private ICommand showLaunchDialogCommand { get; set; }

        public ICommand ShowLaunchDialogCommand
        {
            get
            {
                return showLaunchDialogCommand ?? (showLaunchDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => PerformDialogCoordinatorAction(ShowLaunchMessage((string)x), (string)x == "DISPATCHER_THREAD")
                });
            }
        }

        private Action ShowLaunchMessage(string startingThread)
        {
            return () =>
            {
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    ColorScheme = MetroDialogColorScheme.Theme,
                    DefaultButtonFocus = MessageDialogResult.Affirmative
                };

                string title = InstallCUO ? "Launch Razor and ClassicUO" : "Launch Razor";
                string msg = InstallCUO ? "Would you like to launch Razor and ClassicUO?" : "Would you like to download and install Razor?";

                MessageDialogResult result = _dialogCoordinator.ShowMessageAsync(this, title, msg,
                    MessageDialogStyle.AffirmativeAndNegative, mySettings).Result;

                if (result != MessageDialogResult.Negative)
                {
                    Process process = new Process
                    {
                        StartInfo =
                        {
                            FileName = InstallCUO ? Path.Combine(InstallPath, "ClassicUO.exe") : Path.Combine(InstallPath, "Razor.exe"),
                            WorkingDirectory = InstallPath
                        }
                    };

                    process.Start();
                }
            };
        }

        private static void PerformDialogCoordinatorAction(Action action, bool runInMainThread)
        {
            if (!runInMainThread)
            {
                Task.Factory.StartNew(action);
            }
            else
            {
                action();
            }
        }

        private void DoInstallUpdate()
        {
            BackgroundWorker _installUpdateBackgroundWorker = new BackgroundWorker();

            _installUpdateBackgroundWorker.DoWork += InstallUpdateBackgroundWorkerDoWork;
            _installUpdateBackgroundWorker.RunWorkerCompleted += InstallUpdateBackgroundWorkerCompleted;
            _installUpdateBackgroundWorker.RunWorkerAsync();
        }

        private void InstallUpdateBackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                IsButtonEnabled = false;

                if (string.IsNullOrEmpty(UODataPath) || !InstallerHelper.CheckUoPath(UODataPath))
                {
                    MessageBox.Show($"'{UODataPath}' doesn't appear to be a valid UO installation path.");
                    return;
                }

                InstallerHelper.ValidatePath(InstallPath);

                ReleaseAsset razor = GitHubHelper.GetLatestRazorVersion(UseRazorPreview).Assets[0];
                ReleaseAsset cuo = GitHubHelper.GetLatestClassicUOVersion(UseCUOPreview).Assets[0];

                DownloadFile(razor.BrowserDownloadUrl, Path.Combine(InstallPath, razor.Name));
                DownloadFile(cuo.BrowserDownloadUrl, Path.Combine(InstallPath, cuo.Name));

                ZipArchive razorZip = ZipFile.OpenRead(Path.Combine(InstallPath, razor.Name));
                foreach (ZipArchiveEntry entry in razorZip.Entries)
                {
                    string full = Path.Combine(InstallPath, "Razor", entry.FullName);
                    string dirName = Path.GetDirectoryName(full);

                    if (!Directory.Exists(dirName))
                        Directory.CreateDirectory(dirName);

                    if (!string.IsNullOrEmpty(Path.GetFileName(full)))
                        entry.ExtractToFile(full, true);
                }

                if (InstallCUO)
                {
                    ZipArchive cuoZip = ZipFile.OpenRead(Path.Combine(InstallPath, cuo.Name));
                    foreach (ZipArchiveEntry entry in cuoZip.Entries)
                    {
                        string full = Path.Combine(InstallPath, entry.FullName);
                        string dirName = Path.GetDirectoryName(full);

                        if (!Directory.Exists(dirName))
                            Directory.CreateDirectory(dirName);

                        if (!string.IsNullOrEmpty(Path.GetFileName(full)))
                            entry.ExtractToFile(full, true);
                    }

                    ClassicUoModel settings = new ClassicUoModel
                    {
                        UltimaOnlineDirectory = UODataPath,
                        IP = ServerHost,
                        Port = Convert.ToInt32(ServerPort),
                        ClientVersion = ClientVersion,
                        Plugins = new[] { Path.Combine(InstallPath, "Razor", "Razor.exe") }
                    };

                    File.WriteAllText(Path.Combine(InstallPath, "settings.json"), JsonConvert.SerializeObject(settings));
                }

                InstallerHelper.UpdateSetting("InstallPath", InstallPath);
                InstallerHelper.UpdateSetting("UODataPath", UODataPath);
                InstallerHelper.UpdateSetting("ServerHost", ServerHost);
                InstallerHelper.UpdateSetting("ServerPort", ServerPort);
                InstallerHelper.UpdateSetting("ClientVersion", ClientVersion);

                if (ShowLaunchDialogCommand.CanExecute(null))
                    ShowLaunchDialogCommand.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error. {ex.Message}");
            }
        }

        private void InstallUpdateBackgroundWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsButtonEnabled = true;
            CurrentProgress = 0;
        }

        public void DownloadFile(string url, string destination)
        {

            Uri uri = new Uri(url);

            using (var wc = new WebClient())
            {
                wc.DownloadProgressChanged += HandleDownloadProgress;
                wc.DownloadFileCompleted += HandleDownloadComplete;

                var syncObject = new object();
                lock (syncObject)
                {
                    wc.DownloadFileAsync(uri, destination, syncObject);
                    Monitor.Wait(syncObject);
                }
            }
        }

        public void HandleDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            lock (e.UserState)
            {
                CurrentProgress = 0;
                Monitor.Pulse(e.UserState);
            }
        }

        public void HandleDownloadProgress(object sender, DownloadProgressChangedEventArgs args)
        {
            CurrentProgress = args.ProgressPercentage;
        }
    }
}