using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using InspirationLabProjectStanSeyit.Models;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;
using MySql.Data.MySqlClient;

namespace InspirationLabProjectStanSeyit
{
    public partial class StudyGroups : Window
    {
        public int? SelectedGroupId = null;
        private bool IsCurrentUserOwner = false;
        private List<User> CurrentGroupMembers = new List<User>();

        public StudyGroups()
        {
            InitializeComponent();
            LoadAllGroups();
            GroupDetailsPanel.Visibility = Visibility.Collapsed;
            // Optional: Load your images here if you want
            // Image1.Source = new BitmapImage(new Uri("path_to_your_image1.png"));
            // Image2.Source = new BitmapImage(new Uri("path_to_your_image2.png"));
            // Image3.Source = new BitmapImage(new Uri("path_to_your_image3.png"));
        }

        private void LoadAllGroups()
        {
            int userId = Session.CurrentUserId;
            var groups = Data.GetAllGroupsForUser(userId);
            AllGroupsListBox.ItemsSource = groups;
        }

        private void AllGroupsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllGroupsListBox.SelectedItem is StudyGroup group)
            {
                SelectedGroupId = group.Id;
                GroupDetailsPanel.Visibility = Visibility.Visible;
                GroupNameText.Text = group.Name;
                GroupDescriptionText.Text = group.Description;
                LoadMembers();
                LoadChatMessages();
                LoadFiles();
                LoadMaterials();
            }
            else
            {
                SelectedGroupId = null;
                GroupDetailsPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadMembers()
        {
            if (SelectedGroupId == null) return;
            var memberIds = Data.GetMembersOfGroup(SelectedGroupId.Value);
            var allUsers = Data.GetAllUsers();
            var group = AllGroupsListBox.SelectedItem as StudyGroup;
            int ownerId = group != null ? group.CreatedById : -1;
            IsCurrentUserOwner = ownerId == Session.CurrentUserId;
            CurrentGroupMembers = allUsers.Where(u => memberIds.Contains(u.Id)).ToList();
            var memberViewModels = CurrentGroupMembers.Select(u => new MemberViewModel
            {
                UserId = u.Id,
                Username = u.Username,
                ShowRemove = IsCurrentUserOwner && u.Id != ownerId
            }).ToList();
            MembersListBox.ItemsSource = memberViewModels;
        }

        private void RemoveMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsCurrentUserOwner || SelectedGroupId == null) return;
            if (sender is Button btn && btn.Tag is int userId)
            {
                if (MessageBox.Show($"Remove {CurrentGroupMembers.FirstOrDefault(u => u.Id == userId)?.Username}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Data.RemoveMemberFromGroup(SelectedGroupId.Value, userId, Session.CurrentUserId);
                    LoadMembers();
                }
            }
        }

        private void LoadChatMessages()
        {
            if (SelectedGroupId == null) return;
            var messages = Data.GetGroupMessages(SelectedGroupId.Value)
                .Select(m => new { Username = m.Username, Message = m.Message, SentAt = m.SentAt.ToString("g") })
                .ToList();
            ChatMessagesListBox.ItemsSource = messages;
        }

        private void LoadFiles()
        {
            if (SelectedGroupId == null) return;
            var files = Data.GetGroupFiles(SelectedGroupId.Value)
                .Select(f => new { FileName = f.FileName, FilePath = f.FilePath, Username = f.Username, UploadedAt = f.UploadedAt.ToString("g") })
                .ToList();
            FilesListBox.ItemsSource = files;
        }

        private void LoadMaterials()
        {
            if (SelectedGroupId == null) return;
            using (var conn = new MySqlConnection(Data.GetConnectionString()))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT Id, Title FROM studymaterials WHERE StudyGroupId = @groupId", conn);
                cmd.Parameters.AddWithValue("@groupId", SelectedGroupId.Value);
                var materials = new List<MaterialViewModel>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        materials.Add(new MaterialViewModel
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title")
                        });
                    }
                }
                MaterialsListBox.ItemsSource = materials;
            }
        }

        private void UploadMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGroupId == null)
            {
                MessageBox.Show("Please select a group first.");
                return;
            }
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;
                string title = Path.GetFileName(filePath);
                Data.UploadStudyMaterial(title, filePath, Session.CurrentUserId, SelectedGroupId.Value);
                MessageBox.Show("Material uploaded!");
                LoadMaterials();
            }
        }

        private void DownloadMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int materialId)
            {
                var (fileName, fileBytes) = Data.GetStudyMaterialFile(materialId);
                if (fileBytes != null)
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), fileName);
                    File.WriteAllBytes(tempPath, fileBytes);
                    Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("File not found in database.");
                }
            }
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            // Logic for previous images
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            // Logic for next images
        }


        private void GoBackToFeatures_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CreateGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var createWindow = new GroupCreateWindow();
            createWindow.ShowDialog();
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGroupId == null)
            {
                MessageBox.Show("Please select a group first.");
                return;
            }
            if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
            {
                MessageBox.Show("Please enter a message.");
                return;
            }
            Data.SendGroupMessage(SelectedGroupId.Value, Session.CurrentUserId, MessageTextBox.Text);
            MessageTextBox.Clear();
            LoadChatMessages();
        }

        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedGroupId == null)
            {
                MessageBox.Show("Please select a group first.");
                return;
            }
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string fileName = Path.GetFileName(dialog.FileName);
                string destDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GroupFiles", SelectedGroupId.Value.ToString());
                Directory.CreateDirectory(destDir);
                string destPath = Path.Combine(destDir, fileName);
                File.Copy(dialog.FileName, destPath, true);
                Data.UploadGroupFile(SelectedGroupId.Value, Session.CurrentUserId, fileName, destPath);
                LoadFiles();
            }
        }

        private void DownloadFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string filePath && File.Exists(filePath))
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("File not found.");
            }
        }

        private void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsCurrentUserOwner || SelectedGroupId == null)
            {
                MessageBox.Show("Only the group owner can add members.");
                return;
            }
            string username = AddMemberTextBox.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username.");
                return;
            }
            int userId = Data.GetUserIdByUsername(username);
            if (userId > 0)
            {
                bool success = Data.AddMemberToGroup(SelectedGroupId.Value, userId);
                if (success)
                {
                    MessageBox.Show("Member added!");
                    AddMemberTextBox.Clear();
                    LoadMembers();
                }
                else
                {
                    MessageBox.Show("User is already a member or does not exist.");
                }
            }
            else
            {
                MessageBox.Show("User not found.");
            }
        }

        private class MemberViewModel
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public bool ShowRemove { get; set; }
        }

        private class MaterialViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }
    }
}