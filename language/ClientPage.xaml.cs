using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private int limit = 10;
        private int offset = 0;
        private ObservableCollection<Client> MainGridSourse = new ObservableCollection<Client>();

        private List<Gender> allGenders = ProgramEntities1.GetContext().Gender.ToList();

        private List<Client> LimitDataGrid()
        {
            using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-3K8HP99;Initial Catalog=Program;Integrated Security=True"))
            {
                var clients = new List<Client>();
                connection.Open();

                using(SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = $"select * from Client order by ID offset {offset} rows fetch next {limit} rows only;";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clients.Add(new Client
                            {
                                ID = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Patronymic = reader.GetString(3),
                                Bithday = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4).Date,
                                RegistrationDate = reader.GetDateTime(5).Date,
                                Email = reader.GetString(6),
                                Phone = reader.GetString(7),
                                GenderCode = reader.GetInt32(8),
                                PhotoPath = reader.GetString(9)
                            });
                        }
                    }
                }

                offset += limit;

                return clients;
            }
        }
        public ClientPage()
        {
            InitializeComponent();
            MainDataGrid.ItemsSource = ProgramEntities1.GetContext().Client.ToList();
            allGenders.Insert(2, new Gender
            {
                Name = "Все типы"
            });

            FiltrComboBox.ItemsSource = allGenders;
            FiltrComboBox.DisplayMemberPath = "Name";
            FiltrComboBox.SelectedValuePath = "Code";
            FiltrComboBox.SelectedIndex = 2;
        }

        private void UpdateClients()
        {
            List<Client> clients = new List<Client>();
            foreach(var client in MainGridSourse)
                clients.Add(client);
            //Фильтрация
            if (FiltrComboBox.SelectedIndex < 2)
            {
                string selectedGender = Convert.ToString(FiltrComboBox.SelectedIndex);
                clients = clients.Where(x => x.GenderCode.ToString() == selectedGender).ToList();
            }
            //Сортировка
            switch (SortComboBox.SelectedIndex)
            {
                case 1:
                    {
                        if (UpRadioButton.IsChecked.Value)
                            clients = clients.OrderBy(x => x.LastName).ToList();
                        if (DownRadioButton.IsChecked.Value)
                            clients = clients.OrderByDescending(x => x.LastName).ToList();
                        break;
                    }
                case 2:
                    {
                        if (UpRadioButton.IsChecked.Value)
                            clients = clients.OrderBy(x => x.RegistrationDate).ToList();
                        if (DownRadioButton.IsChecked.Value)
                            clients = clients.OrderByDescending(x => x.RegistrationDate).ToList();
                        break;
                    }
            }
            //Поиск
            switch (SearchComboBox.SelectedIndex)
            {
                case 0:
                    {
                        clients = clients.Where(x => x.LastName.ToLower().StartsWith(SearchTextBox.Text.ToLower())).ToList();
                        break;
                    }
                case 1:
                    {
                        clients = clients.Where(x => x.FirstName.ToLower().StartsWith(SearchTextBox.Text.ToLower())).ToList();
                        break;
                    }
                case 2:
                    {
                        clients = clients.Where(x => x.Patronymic.ToLower().StartsWith(SearchTextBox.Text.ToLower())).ToList();
                        break;
                    }
                case 3:
                    {
                        clients = clients.Where(x => x.Email.ToLower().StartsWith(SearchTextBox.Text.ToLower())).ToList();
                        break;
                    }
                case 4:
                    {
                        clients = clients.Where(x => x.Phone.ToLower().StartsWith(SearchTextBox.Text.ToLower())).ToList();
                        break;
                    }
            }

            MainDataGrid.ItemsSource = clients.ToList();
            //Странный вывод
            if (clients.Count == 0) 
            {
                MessageBox.Show("По вашему запросу ничего не найдено");
                SearchTextBox.Text = "";
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPage(null));
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
           if (MessageBox.Show("Вы действительно хотите удалить данные?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    ProgramEntities1.GetContext().Client.Remove(MainDataGrid.SelectedItem as Client);
                    ProgramEntities1.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены");
                    MainDataGrid.ItemsSource = ProgramEntities1.GetContext().Client.ToList();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPage(MainDataGrid.SelectedItem as Client));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var valueClients = LimitDataGrid();
            foreach (var client in valueClients)
            {
                Gender clientGender = allGenders.FirstOrDefault(x => x.ID == client.GenderCode);

                client.Gender = clientGender;

                MainGridSourse.Add(client);
            }

            ClientCount.Text = $"Всего записей: {MainGridSourse.Count()}";

            MainDataGrid.ItemsSource = ProgramEntities1.GetContext().Client.ToList();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FiltrComboBox != null)
                UpdateClients();
        }

        private void UpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (FiltrComboBox != null)
                UpdateClients();
        }

        private void DownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (FiltrComboBox != null)
                UpdateClients();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void FiltrComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void Client_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientCard(MainDataGrid.SelectedItem as Client));
        }

        private void BtnPagination(object sender, RoutedEventArgs e)
        {
            var valueClients = LimitDataGrid();
            foreach (var client in valueClients)
            {
                Gender clientGender = allGenders.FirstOrDefault(x => x.ID == client.GenderCode);

                client.Gender = clientGender;

                MainGridSourse.Add(client);
            }

            ClientCount.Text = $"Всего записей: {MainGridSourse.Count()}";

            MainDataGrid.ItemsSource = MainGridSourse;
        }
    }
}
