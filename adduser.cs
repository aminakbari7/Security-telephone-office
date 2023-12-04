using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
namespace systemphone
{
    public partial class adduser : Form
    {
        MySqlConnection connection=new MySqlConnection();
        public adduser()
        {
            InitializeComponent();
        }

        void fakefiller()
        {
            Random rand = new Random();
            int num = rand.Next(1, 4);


            for (int i = 0; i < num; i++)
            {
                int fksalt = rand.Next();
                string fkname = addcap();
                string fknumber = addcap();
                string connetionString = null;
                MySqlConnection cnn;
                connetionString = "server=localhost;database=systemphone1;uid=root;pwd=\"\";";
                using (MySqlConnection cn = new MySqlConnection(connetionString))
                {

                    string query = "INSERT INTO user(username, password,saltp) VALUES (?username,?password,?saltp);";
                    cn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, cn))
                    {
                        Random rnd = new Random();
                        int salt = rnd.Next();
                        cmd.Parameters.Add("?username", MySqlDbType.VarString).Value = ComputeSha256Hash(fkname, 0);
                        cmd.Parameters.Add("?saltp", MySqlDbType.Int32).Value = salt;
                        cmd.Parameters.Add("?password", MySqlDbType.VarChar).Value = ComputeSha256Hash(fknumber, fksalt);
                        cmd.ExecuteNonQuery();
                        
                    }
                    cn.Close();
                }
                
            }

        }
        public string addcap()
        {
            Random rand = new Random();
            int num = rand.Next(6, 8);
            string captcha = "";
            int tol = 0;
            do
            {
                int chr = rand.Next(48, 123);
                if ((chr >= 48 && chr <= 57) || (chr >= 65 && chr <= 90) || (chr >= 97 && chr <= 122))
                {
                    captcha = captcha + (char)chr;
                    tol++;
                    if (tol == num)
                    {
                        break;
                    }
                }

            } while (true);
            return captcha.ToLower();
        }
        private void adduser_Load(object sender, EventArgs e)
        {
            
        }
        static public string ComputeSha256Hash(string input,int salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt.ToString());
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            fakefiller();
            string connetionString = null;
            MySqlConnection cnn;
            connetionString = "server=localhost;database=systemphone1;uid=root;pwd=\"\";";
            List<string> errors = new List<string>();
            if(textBox1.Text.Length<3)
            {
                errors.Add("نام کاربری باید طول بزرگتر از 4 داشته باشد");
            }
            if (textBox2.Text.Length <3)
            {
                errors.Add("پسورد باید طول بزرگتر از4  داشته باشد");
            }
            if (textBox3.Text.Length < 3|| textBox2.Text!=textBox3.Text)
            {
                errors.Add("پسورد را به درستی تکرار کنید");
            }
            string errorMessage = string.Join("\n", errors.ToArray());
            if (errorMessage.Length != 0)
                MessageBox.Show(errorMessage);
            if(errorMessage.Length==0)
            {
               
                using (MySqlConnection cn = new MySqlConnection(connetionString))
                {
                    try
                    {
                        int usersalt=0;
                        foreach (char c in textBox2.Text)
                        {
                            usersalt+=Convert.ToInt32(c);
                        }
                        string query = "INSERT INTO user(username, password,saltp) VALUES (?username,?password,?saltp);";
                        cn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, cn))
                        {
                            Random rnd = new Random();
                            int salt = rnd.Next();
                            cmd.Parameters.Add("?username", MySqlDbType.VarString).Value = ComputeSha256Hash(textBox1.Text,usersalt);
                            cmd.Parameters.Add("?saltp", MySqlDbType.Int32).Value = salt;
                            cmd.Parameters.Add("?password", MySqlDbType.VarChar).Value = ComputeSha256Hash(textBox2.Text,salt);
                            cmd.ExecuteNonQuery();
                           
                            MessageBox.Show("خوش آمدید ثبت نام با موفقیت انجام شد");
                            this.Close();
                            fakefiller();
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error in adding mysql row. Error: " + ex.Message);
                    }

                }
                
            }
            
          
            textBox2.Text = "";
            textBox3.Text = "";
         
        }
    }
}
