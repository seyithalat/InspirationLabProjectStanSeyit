using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace InspirationLabProjectStanSeyit
{

    //Interaction logic for GroupCreateWindow.xaml

    public partial class GroupCreateWindow : Window
    {
        // Connection string for phpMyAdmin MySQL database (same as Data.cs)
        public GroupCreateWindow()
        {
            InitializeComponent();
        }

        private void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            string name = GroupNameBox.Text.Trim();
            string desc = GroupDescBox.Text.Trim();
            int ownerId = Session.CurrentUserId; // Or however you store the logged-in user

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a group name.");
                return;
            }

            int groupId = Data.CreateStudyGroup(name, desc, ownerId);
            if (groupId > 0)
            {
                MessageBox.Show("Group created!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to create group.");
            }
        }
    }
}
