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
using System.Data.Common;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace БД_Лаб1
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : UserControl
    {
   
        private static System.Data.DataTable Table { get; set; }
        private static OleDbDataAdapter dataAdapter { get; set; }
        public AdminPage()
        {
            InitializeComponent();
            
            OleDbConnection c=DataBase(GetSQLQuery("Students"));
            List < string > tables= GetTablesFromDB(c);
            if(tables.Contains("Пользователи"))
                tables.Remove("Пользователи");
            tablesBox.ItemsSource= tables;
            tablesBox.SelectedIndex = 0;

            
        }
        private string GetSQLQuery(string tableName)
        {
            
            return string.Format("Select * from [{0}];", tableName);

        }
        public List<string> GetTablesFromDB(OleDbConnection connection)
        {
            connection.Open();
            System.Data.DataTable a = connection.GetSchema("Tables", new string[4] { null, null, null, "TABLE" });
            connection.Close();
            List<string> res = new List<string>();
            for (int i = 0; i < a.Rows.Count; i++)
            {
                res.Add(a.DefaultView[i][2] as string);
            }
            return res;
        }
        public OleDbConnection DataBase(string sql)
        {
            string connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                @"\\Mac\Home\Desktop\3 курс\Лабораторные работы\БД\БД_Лаб1\БД_Лаб1\bin\Debug\Колледж(для студентов).mdb;Persist Security Info=False";
            OleDbConnection connection=new OleDbConnection(connectionstring);
            try
            {
                Table = new System.Data.DataTable();

              
                connection.Open();
                OleDbCommand oleDbCommand = new OleDbCommand(sql, connection);
                
                dataAdapter = new OleDbDataAdapter(oleDbCommand);
                dataAdapter.Fill(Table);
                data.ItemsSource = Table.DefaultView;
                connection.Close();
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("Ошибка доступа", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return connection;
        }
        private void UpdateBD()
        {
            
            OleDbCommandBuilder oleDbCommandBuilder = new OleDbCommandBuilder(dataAdapter);//создает команду sql (добавление,удаление,изменение)
                                                                                           //для выполнения запроса
            
            dataAdapter.Update(Table);//обновляет таблицу в БД соответсвенно изменениям в DataGrid
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

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Table.Rows.Add();
            UpdateBD();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //по кнопке вызывает метод обновления дб
            try
            {
                UpdateBD();
            }
            catch
            {
                MessageBox.Show("Попытка добавления несуществующей группы", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tablesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OleDbConnection c = DataBase(GetSQLQuery(tablesBox.SelectedItem.ToString()));
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

        

        private void ImportToExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();//создаешь экземпляр приложения эксель
            
            Excel.Workbook wb = excel.Workbooks.Add();//добавляет в экземпляр новую книгу
            Excel.Worksheet worksheet = wb.Worksheets.Add();//добавляет в книгу новый лист
            worksheet.Name = tablesBox.SelectedItem.ToString();//добавляет имя листа(у меня это выбранный элемент комбобокс)

            worksheet.Columns.AutoFit();//размер колонок по контенту,и можно не добавлять

            for (int Idx = 0; Idx < Table.Columns.Count; Idx++)
            {
                //цикл для заголовка, можно не добавлять, но тогда следующий цикл будет с другими значениями
                worksheet.Range["A1"].Offset[0, Idx].Value = Table.Columns[Idx].ColumnName;
            }
            for (int Idx = 0; Idx < Table.Rows.Count; Idx++)
            {
                //цикл для добавления остальных данных в таблицу(не заголовок)
                //если не делаешь заголовок , то вместо А2 -А1, вместо цифры 1-0
                worksheet.Range["A2"].Offset[Idx].Resize[1, Table.Columns.Count].Value = Table.Rows[Idx].ItemArray;
            }
            //делает приложение эксель видимым
            excel.Visible = true;
            //не знаю зачем, загугли
            wb.Activate();

        }
    }
}
