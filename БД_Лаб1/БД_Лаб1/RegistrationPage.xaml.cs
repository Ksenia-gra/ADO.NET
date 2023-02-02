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
using System.Windows.Markup;

namespace БД_Лаб1
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : UserControl
    {

        public IEnumerable<DataRow> GetDataRows(string sql)
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

        private User AddNewUserInDataBase(string sql)
        {
         
            
            string connectionstring =
                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                @"\\Mac\Home\Desktop\3 курс\Лабораторные работы\БД\БД_Лаб1\БД_Лаб1\bin\Debug\Колледж(для студентов).mdb;Persist Security Info=False";

            string login = loginText.Text;
            string password = passwordText.Password;
            OleDbParameter param1 = new OleDbParameter("@login", login);
            OleDbParameter param2 = new OleDbParameter("@password", password);
            try
            {
                DataTable table = new DataTable();

                OleDbConnection connection = new OleDbConnection(connectionstring);
                connection.Open();
                OleDbCommand oleDbCommand = new OleDbCommand(sql, connection);
                OleDbCommand dbCommand = connection.CreateCommand();
                
                dbCommand.CommandText = "insert into Пользователи (Логин,Пароль) values (@login,@password);";
                dbCommand.Parameters.Add(param1);
                dbCommand.Parameters.Add(param2);
                dbCommand.ExecuteNonQuery();
                
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleDbCommand);
                dataAdapter.Fill(table);
                
                connection.Close();
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ошибка доступа", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return new User(login, password);
        }
        private bool CheckUser(IEnumerable<User> users)
        {
            //проверяет наличие пользователя в таблице(по введенным данным)
            if (users.Any(u =>u.Login == loginText.Text))
            {

                return false;
            }
            else
            {
                
                return true;
                
            }

            
        }
        private void SetActiveUser(User user)
        {
            //задает активного пользователя
            MainWindow.ActiveUser = user;
            
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
        public RegistrationPage()
        {
            InitializeComponent();
            
        }


        private void authorization_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.myUC.Content = new SignInControl();
        }

        private void signUp_Click(object sender, RoutedEventArgs e)
        {
            string sql = "Select * from Пользователи ;";
            IEnumerable<User> users = GetUsers(GetDataRows(sql));
            if (CheckUser(users))
            {
                if(passwordText.Password.Length<6)
                {
                    MessageBox.Show("Слишком короткий пароль", "Ошибка регистрации",
                   MessageBoxButton.OK, MessageBoxImage.Error);
                    passwordText.Password = null;
                    return;
                }
                else
                {
                    
                    User u = AddNewUserInDataBase(sql);
                    SetActiveUser(u);
                    MainWindow.EnterTime = DateTime.Now;
                    MainWindow.Instance.myUC.Content = new MainPage();
                }
                
            }
                
            else
                MessageBox.Show("Пользователь с таким именем уже существует\n" +
                    "придумайте другое", "Ошибка регистрации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
