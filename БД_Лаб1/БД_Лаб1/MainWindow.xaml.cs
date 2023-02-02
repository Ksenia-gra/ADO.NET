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
using System.Data.OleDb;
using System.Data;
using System.Windows.Markup;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using MessageBox = System.Windows.MessageBox;

namespace БД_Лаб1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public static User ActiveUser { get; set; }
        public static DateTime EnterTime { get; set; }
        public static DateTime ExitTime { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            myUC.Content = new SignInControl();

        }
    }
        
}

