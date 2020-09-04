using AngleSharp.Common;
using Gui2.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Gui2.Helpers.ConfigHelper;
using static Gui2.Classes.client;
using static Gui2.Helpers.UiHelper;
using static Gui2.Helpers.ClientHelper;
using YoutubeExplode;
using System.Collections.Generic;
using Gui2.Helpers;
using System.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading;
using System.Net;
using System.Runtime.Remoting.Messaging;

namespace Gui2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigGrid.Visibility = Visibility.Hidden;
            PathsGrid.Visibility = Visibility.Hidden;
            LocalFilesSetup();
            new client(CurrentList).Start();
        }









        //GUI
        ActiveTopWindow topWindow = ActiveTopWindow.MainWindow;
        public void ShowConfigWindow()
        {
            switch (topWindow)
            {
                case ActiveTopWindow.MainWindow:
                    ConfigGrid.Visibility = Visibility.Visible;
                    PathsGrid.Visibility = Visibility.Hidden;
                    topWindow = ActiveTopWindow.ConfigWindow;
                    _isMoving = false;
                    UpdateList();
                    break;
            }
        }
        public void ShowPathsWindow()
        {
            switch (topWindow)
            {
                case ActiveTopWindow.MainWindow:
                    PathsGrid.Visibility = Visibility.Visible;
                    ConfigGrid.Visibility = Visibility.Hidden;
                    topWindow = ActiveTopWindow.PathWindow;
                    _isMoving = false;
                    UpdateList();
                    break;
            }
        }

        public void HideConfigWindow()
        {
            switch (topWindow)
            {
                case ActiveTopWindow.ConfigWindow:
                    ConfigGrid.Visibility = Visibility.Hidden;
                    _isMoving = false;
                    topWindow = ActiveTopWindow.MainWindow;
                    break;
            }
        }
        public void HidePathsWindow()
        {
            switch (topWindow)
            {
                case ActiveTopWindow.PathWindow:
                    PathsGrid.Visibility = Visibility.Hidden;
                    _isMoving = false;
                    topWindow = ActiveTopWindow.MainWindow;
                    break;
            }
        }


        public void UpdateList()
        {
            try
            {
                var cfg = new ConfigSys(LocalData);
                var save = cfg.Read();

                if (save == new SaveInfo())
                    return;

                ViewIds.Items.Clear();

                if (save == null)
                    return;

                foreach (Streamer streamer in save.Streamers)
                {
                    ViewIds.Items.Add($"{streamer.Website}");
                }
            } catch(Exception) { MessageBox.Show($"Error occurred did you enter Interval in minutes?"); }
        }


        ///CONFIG TAB <------

        private void CloseConfig_GotFocus(object sender, RoutedEventArgs e)
        {
            HideConfigWindow();
        }

        private void Close_Path_GotFocus(object sender, RoutedEventArgs e)
        {
            HidePathsWindow();
        }


        private void AddId_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ChannelId.Text.Contains("youtube.com") && !ChannelId.Text.Contains("youtube.com/channel/"))
                {
                    MessageBox.Show("Wrong format for Youtube!. To add a Youtube Channel, Follow this format https://www.youtube.com/channel/UCxboW7x0jZqFdvMdCFKTMsQ, you can get this Url by searching the Channel, and copying the channel url");
                    return;
                }

                if (ChannelId.Text == "Link to Channel")
                {
                    MessageBox.Show("Empty Link to Channel Box");
                    return;
                }

                var cfg = new ConfigSys(LocalData);
                StreamerPlatform platform;
                Enum.TryParse<StreamerPlatform>(PlatformComboBox.Text, out platform);

                if (cfg.AddStreamer(new Streamer(ChannelId.Text, int.Parse(IntervalMinutes.Text), platform))) //I know i can use Enum Tryparse
                    MessageBox.Show($"Added Channel: {ChannelId.Text}");

                UpdateList();
            } catch(Exception) { MessageBox.Show($"Error occurred have you entered a proper Interval?"); }

        }
        private void RemoveId_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ChannelId.Text == "Channel Id")
                {
                    MessageBox.Show("Empty Channel Id Box");
                    return;
                }
                var cfg = new ConfigSys(LocalData);
                cfg.RemoveChannelId(ChannelId.Text);
                MessageBox.Show($"Removed Channel Id: {ChannelId.Text}");
                UpdateList();
            }
            catch (Exception x) { MessageBox.Show($"Exception occurred: {x.Message}"); }

        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            var cfg = new ConfigSys(LocalData);
            cfg.Reset();
            MessageBox.Show("Config was Reset");
            UpdateList();
        }

        private void ViewIds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ViewIds == null)
                return;

            if (ViewIds.SelectedItem == null)
                return;
            ChannelId.Text = ViewIds.SelectedItem.ToString();
        }


        private bool _isMoving;
        private Point? _buttonPosition;
        private double deltaX;
        private double deltaY;
        private TranslateTransform _currentTT;

        private void ConfigGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (topWindow != ActiveTopWindow.ConfigWindow)
                return;

            if (_buttonPosition == null)
                _buttonPosition = ConfigGrid.TransformToAncestor(WindowMain).Transform(new Point(0, 0));
            var mousePosition = Mouse.GetPosition(WindowMain);
            deltaX = mousePosition.X - _buttonPosition.Value.X;
            deltaY = mousePosition.Y - _buttonPosition.Value.Y;
            _isMoving = true;
        }

        private void ConfigGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMoving) return;

            var mousePoint = Mouse.GetPosition(WindowMain);

            var offsetX = (_currentTT == null ? _buttonPosition.Value.X : _buttonPosition.Value.X - _currentTT.X) + deltaX - mousePoint.X;
            var offsetY = (_currentTT == null ? _buttonPosition.Value.Y : _buttonPosition.Value.Y - _currentTT.Y) + deltaY - mousePoint.Y;

            this.ConfigGrid.RenderTransform = new TranslateTransform(-offsetX, -offsetY);
        }

        private void ConfigGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _currentTT = ConfigGrid.RenderTransform as TranslateTransform;
            _isMoving = false;
        }

        private void More_Info_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Link to streamers home page where the stream is displayed so the Streamlink library could download it. If you want to add a Youtube Channel Format is 'https://www.youtube.com/channel/UCxboW7x0jZqFdvMdCFKTMsQ', For Dlive Format is 'https://live.prd.dlive.tv/hls/live/streamername.m3u8', if that dosent work Search how to get hls stream url from Dlive, Should work normaly with twitch.");
        }

        private void PlatformComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlatformComboBox.Text != string.Empty)
                return;

            StreamerPlatform platform;
            Enum.TryParse<StreamerPlatform>(PlatformComboBox.Text, out platform);
            switch(platform)
            {
                case StreamerPlatform.YouTube:
                    ChannelId.Text = "Youtube Channel Unique Id";
                    break;
                case StreamerPlatform.Twitch:
                    ChannelId.Text = "Twitch Channel Page";
                    break;
                case StreamerPlatform.DLive:
                    ChannelId.Text = "Unique Id Complicated";
                    break;
                case StreamerPlatform.Other:
                    ChannelId.Text = "Hls Stream Url";
                    break;
            }
        }

        private void ShowPaths_Click(object sender, RoutedEventArgs e)
        {
            switch (topWindow)
            {
                case ActiveTopWindow.PathWindow:
                    HidePathsWindow();
                    break;
                case ActiveTopWindow.MainWindow:
                    ShowPathsWindow();
                    break;
                case ActiveTopWindow.ConfigWindow:
                    SystemSounds.Exclamation.Play();
                    break;
            }
        }
        private void Configbtn_Click(object sender, RoutedEventArgs e)
        {
            switch (topWindow)
            {
                case ActiveTopWindow.ConfigWindow:
                    HideConfigWindow();
                    break;
                case ActiveTopWindow.MainWindow:
                    ShowConfigWindow();
                    break;
                case ActiveTopWindow.PathWindow:
                    SystemSounds.Exclamation.Play();
                    break;
            }
        }

        private async void AuthenticateApi_Click(object sender, RoutedEventArgs e)
        {
            var cfg = new ConfigSys(LocalData);
            try
            {
                var obj = new Upload(ClientSecretsFolder + cfg.Read().settings.Clientsecrets, "user");
                await obj.Init();
                MessageBox.Show("Authenticated!");
            }
            catch (Exception x) { MessageBox.Show($"Failled to authenticate: {x.Message}"); }

        }
        //PATHS GRID <-----
        private void SavePath_Click(object sender, RoutedEventArgs e)
        {
            var cfg = new ConfigSys(LocalData);
            SaveInfo CurrentSave = cfg.Read();
            CurrentSave.settings.StreamsFolder = StreamSaveFolderTextBox.Text;
            CurrentSave.settings.Clientsecrets = ClientSecretsTextBox.Text;
            CurrentSave.settings.Uploading = (bool)UploadingCheckBox.IsChecked;
            if (cfg.Set(CurrentSave))
                MessageBox.Show("Successfully updated Configuration.");

        }

        private void StreamSaveFolderTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            dialog.ShowDialog();
            try
            { StreamSaveFolderTextBox.Text = dialog.FileName + "\\"; }
            catch (Exception) { }
        }

        private void PathsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPaths();
        }

        public void RefreshPaths()
        {
            var cfg = new ConfigSys(LocalData);
            var CurrentSave = cfg.Read();
            StreamSaveFolderTextBox.Text = CurrentSave.settings.StreamsFolder;
            ClientSecretsTextBox.Text = CurrentSave.settings.Clientsecrets;
            UploadingCheckBox.IsChecked = CurrentSave.settings.Uploading;
        }
        private void ResetPaths_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want Reset the configuration?", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;

            var cfg = new ConfigSys(LocalData);
            SaveInfo save = cfg.Read();
            save.settings.StreamsFolder = "Local Folder";
            save.settings.Clientsecrets = "client_secrets.json";
            save.settings.Uploading = false;
            if (cfg.Set(save))
            {
                MessageBox.Show("Configuration was Successfully Reset.");
                RefreshPaths();
            }
        }

        private void UploadingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ClientSecretsTextBox.IsEnabled = true;
        }

        private void UploadingCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ClientSecretsTextBox.IsEnabled = false;
        }
    }
}
