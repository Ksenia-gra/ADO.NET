using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace БД_Лаб1
{
    public class User
    {
        private string id;
        private string login;
        private string password;
        private bool admin;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Login
        {
            get { return login; }
            set { login = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
       
        public bool Admin
        {
            get { return admin; }
            set { admin = value; }
        }
        public User ()
        {
            Login = null;
            Password = null;
            Admin = false;
        }
        public User (string login,string password)
        {
            Login = login;
            Password = password; 
        }
        public User(DataRow parametrs)
        {

            Login = parametrs[1].ToString();
            Password = parametrs[2].ToString();
            Admin = parametrs[3].ToString().ToLower()=="true";
        }

    }
}
