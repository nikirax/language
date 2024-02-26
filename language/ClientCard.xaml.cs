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
    /// Логика взаимодействия для ClientCard.xaml
    /// </summary>
    public partial class ClientCard : Page
    {
        private Client _currentClient = new Client();
        public ClientCard(Client selectedClient)
        {
            InitializeComponent();
            _currentClient = selectedClient;
            DataContext = _currentClient;
            GenderComboBox.ItemsSource = ProgramEntities1.GetContext().Gender.ToList();
        }
    }
}
