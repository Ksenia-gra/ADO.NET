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
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using System.IO;

namespace БД_Лаб1
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    
    public partial class MainPage : UserControl
    {
        string sql = "Select * from students;";
        string connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                @"\\Mac\Home\Desktop\3 курс\Лабораторные работы\БД\БД_Лаб1\БД_Лаб1\bin\Debug\Колледж(для студентов).mdb;Persist Security Info=False";
        public MainPage()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataBase(sql);
        }

        private void makeSelectButton_Click(object sender, RoutedEventArgs e)
        {
            string makeSelect = $"Select Familia, №gr, Budget from students where familia = @familia;";
            string lastName = Excercise1.Text;
            OleDbParameter param = new OleDbParameter("@familia", lastName);
            DataBase(makeSelect, param);
        }

        private void ex2Button_Click(object sender, RoutedEventArgs e)
        {
            string countAge = $"Select Imya, Familia,format(date()-datarogd,'yy') as age from students where pol='М';";
            DataBase(countAge);
        }

        private void ex3Button_Click(object sender, RoutedEventArgs e)
        {
            string makeSelectByGroup = $"Select Familia & ' ' & left(imya, 1) & '. ' & left(otchestvo, 1) & '.' " +
                $"as initials from students where №gr = @№gr; ";
            string groupNumber = Excercise3.Text;
            OleDbParameter param = new OleDbParameter("@№gr", groupNumber);
            DataBase(makeSelectByGroup, param);

        }

        private void ex4Button_Click(object sender, RoutedEventArgs e)
        {
            int yearOfBorn = int.Parse(Excercise4.Text);
            string makeSelect = $"Select Familia,№gr from students where year(datarogd)=@yearOfBorn ;";
            OleDbParameter param = new OleDbParameter("@yearOfBorn", yearOfBorn);
            DataBase(makeSelect, param);

        }

        private void ex5Button_Click(object sender, RoutedEventArgs e)
        {
            string makeSelect = $"Select familia, №gr from students where gorod is not null;";
            DataBase(makeSelect);
        }

       

        private void findStudentButton_Click(object sender, RoutedEventArgs e)
        {
            string choosenDate = ChooseDate.Text;
            string select = $"Select * from students where datarogd=@choosenDate;";
            OleDbParameter param = new OleDbParameter("@choosenDate", choosenDate);
            DataBase(select, param);
        }

        private void findStudentEx8_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ChooseDateRange2.Text))
            {
                MessageBox.Show("Ошибка выбора диапазона", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DateTime.Parse(ChooseDateRange1.Text) > DateTime.Parse(ChooseDateRange2.Text))
            {
                MessageBox.Show("Ошибка выбора диапазона", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DateTime startRange = DateTime.Parse(ChooseDateRange1.Text);
            DateTime endRange = DateTime.Parse(ChooseDateRange2.Text);
            string sql = $"Select * from students where datarogd between @startRange and @endRange";
            OleDbParameter param1 = new OleDbParameter("@startRange", startRange);
            OleDbParameter param2 = new OleDbParameter("@endRange", endRange);
            try
            {
                DataTable table = new DataTable();

                OleDbConnection connection = new OleDbConnection(connectionstring);
                connection.Open();
                OleDbCommand oleDbCommand = new OleDbCommand(sql, connection);

                oleDbCommand.Parameters.Add(param1);
                oleDbCommand.Parameters.Add(param2);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleDbCommand);
                dataAdapter.Fill(table);
                data.ItemsSource = table.DefaultView;
                connection.Close();
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ошибка доступа", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DataBase(string sql, OleDbParameter param)
        {
            try
            {
                DataTable table = new DataTable();

                OleDbConnection connection = new OleDbConnection(connectionstring);
                connection.Open();
                OleDbCommand oleDbCommand = new OleDbCommand(sql, connection);

                oleDbCommand.Parameters.Add(param);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleDbCommand);
                dataAdapter.Fill(table);
                data.ItemsSource = table.DefaultView;
                connection.Close();
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ошибка доступа", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        public void DataBase(string sql)
        {
            try
            {
                DataTable table = new DataTable();

                OleDbConnection connection = new OleDbConnection(connectionstring);
                connection.Open();
                OleDbCommand oleDbCommand = new OleDbCommand(sql, connection);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleDbCommand);
                dataAdapter.Fill(table);
                data.ItemsSource = table.DefaultView;
                connection.Close();
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ошибка доступа", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void SaveToFile()
        {

            if (!File.Exists(Environment.CurrentDirectory + "protocol.txt"))
            {
                File.Create(Environment.CurrentDirectory + "protocol.txt").Close();


                StreamWriter sw = new StreamWriter(File.Open(Environment.CurrentDirectory + "protocol.txt", FileMode.Append));
                sw.WriteLine($"Пользователь:{MainWindow.ActiveUser.Login} администратор: {MainWindow.ActiveUser.Admin} " +
                     $"Время входа:{MainWindow.EnterTime} Время выхода:{MainWindow.ExitTime}"
                     );
                sw.Close();
            }
            else
            {
                StreamWriter sw = new StreamWriter(File.Open(Environment.CurrentDirectory + "protocol.txt", FileMode.Append));
                sw.WriteLine($"Пользователь:{MainWindow.ActiveUser.Login} администратор: {MainWindow.ActiveUser.Admin} " +
                    $"Время входа:{MainWindow.EnterTime} Время выхода:{MainWindow.ExitTime}"
                    );
                sw.Close();

            }

        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = (DialogResult)MessageBox.Show("Уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (dialogResult == DialogResult.Yes)
            {
                MainWindow.ExitTime = DateTime.Now;
                SaveToFile();
                MainWindow.Instance.myUC.Content = new SignInControl();
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }


    }

}
