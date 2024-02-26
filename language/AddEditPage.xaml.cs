using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace language
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Client _currentClient = new Client();
        private string _selectedImagePath;
        public AddEditPage(Client selectedClient)
        {
            InitializeComponent();
            if (selectedClient != null)
            {
                AddEditTextBlock.Text = "Редактирование клиента";
                IDTextBox.IsEnabled = false;
            }
            else
            {
                AddEditTextBlock.Text = "Добавление клиента";
                IDTextBox.Visibility = Visibility.Hidden;
                IDTextBlock.Visibility = Visibility.Hidden;    
            }
            DataContext = _currentClient;
            GenderComboBox.ItemsSource = ProgramEntities1.GetContext().Gender.ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentClient.LastName))
                errors.AppendLine("He задана фамилия клиента");
            if (string.IsNullOrWhiteSpace(_currentClient.FirstName))
                errors.AppendLine("He задано имя клиента");
            if (_currentClient.Gender == null)
                errors.AppendLine("He Bыбран пол");
            if (_currentClient.Phone == null)
                errors.AppendLine("Не указан контактный телефон");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if(_currentClient.ID == 0)
            {
                ProgramEntities1.GetContext().Client.Add(_currentClient);
            }
            try
            {
                _currentClient.PhotoPath = _selectedImagePath;
                ProgramEntities1.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ChossePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберете фото";
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                ProfilImage.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }
    }
}
