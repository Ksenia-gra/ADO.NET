using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
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
using System.ComponentModel;
using System.Windows.Markup;

namespace БД_Лаб1
{
    /// <summary>
    /// Логика взаимодействия для SignInControl.xaml
    /// </summary>
    /// 
    public partial class SignInControl : UserControl
    {

        private IEnumerable<DataRow> GetDataRows(string sql)
        {
            // получает таблицу данных из бд
            List<DataRow> dataRows = new List<DataRow>();
            string connectionstring =
                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                @"\\Mac\Home\Desktop\3 курс\Лабораторные работы\БД\БД_Лаб1\БД_Лаб1\bin\Debug\Колледж(для студентов).mdb;Persist Security Info=False";
            
            try
            {
                DataTable table = new DataTable();
                OleDbConnection connection = new OleDbConnection(connectionstring);
                connection.Open();
                OleDbCommand oleDbCommand = new OleDbCommand(sql, connection);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleDbCommand);
                
                dataAdapter.Fill(table);
                dataRows = table.Select().ToList<DataRow>();
                connection.Close();

            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ошибка доступа", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return dataRows;
        }
        private List<User> GetUsers(IEnumerable<DataRow> rows)
        {
            //создает список пользователей из полученной таблицы
            List<User> users = new List<User>();
            foreach (DataRow row in rows)
            {
                users.Add(new User(row));
                
            }
           
            return users;
        }
        private bool CheckUser(IEnumerable<User> users)
        {
            //проверяет наличие пользователя в таблице(по введенным данным)
            if (users.Any(u => u.Password == passwordText.Password && u.Login == loginText.Text))
            {
                
                return true;
            }
            else return false;
        }
        private void SetActiveUser(IEnumerable<User> users)
        {
            //задает активного пользователя
            foreach(User user in users)
            {
                if(user.Password==passwordText.Password && user.Login==loginText.Text)
                {
                    MainWindow.ActiveUser = user;
                    break;
                }
            }
        }
        public SignInControl()
        {
            InitializeComponent();
            
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {

            string sql = "Select * from Пользователи ;";
          
            IEnumerable<User> users= GetUsers(GetDataRows(sql));
            if (CheckUser(users))
            {
                SetActiveUser(users);
                MainWindow.EnterTime = DateTime.Now;
                if (MainWindow.ActiveUser.Admin)
                    MainWindow.Instance.myUC.Content = new AdminPage();
                else
                    MainWindow.Instance.myUC.Content = new MainPage();
            }
            else
            {
                MessageBox.Show("Пользователь не найден,проверьте введенные данные", "Ошибка входа",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                passwordText.Password = null;
            }
                
           
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.myUC.Content = new RegistrationPage();
        }

        private void loginText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "0123456789QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm.@_".IndexOf(e.Text) < 0;
        }
    }
}
