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
    public partial class login : Form
    {
        string user_id;
        string user_name;
        string password_user;
        string aes_key;
        string user_salt;
        public login()
        {
            InitializeComponent();
        }
        static public string ComputeSha256Hash(string input, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        private void label1_Click(object sender, EventArgs e)
        {

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
        private void login_Load(object sender, EventArgs e)
        {

           
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int usersalt = 0;
            foreach (char c in textBox2.Text)
            {
                usersalt += Convert.ToInt32(c);
            }
           
            string connetionString = "server=localhost;database=systemphone1;uid=root;pwd=\"\";";
                {
                    using (MySqlConnection cn = new MySqlConnection(connetionString))
                    {
                        List<string> list = new List<string>();
                        String gg = ComputeSha256Hash(textBox1.Text,usersalt.ToString());
                         
                        try
                        {
                            string query = "SELECT * FROM user WHERE username='" + gg + "'";
                            cn.Open();
                            using (MySqlCommand cmd = new MySqlCommand(query, cn))
                            {
                                MySqlDataReader dataReader = cmd.ExecuteReader();
                                while (dataReader.Read())
                                {
                                    for (int i = 0; i < dataReader.FieldCount; i++)
                                    {
                                        list.Add(dataReader.GetString(i));
                                    }
                                }
                                dataReader.Close();
                            }
                            if (list.Count == 0)
                            {
                                MessageBox.Show("اطلاعات وارد شده صحیح نیستند");
                            }
                            else
                            {
                                user_id = list[0];
                                user_salt = list[2];
                                string hp = list[3];
                                if (hp == ComputeSha256Hash(textBox2.Text, user_salt) && list[1]==gg)
                                {
                                   
                                    user_name = textBox1.Text;
                                    password_user = textBox2.Text;

                                   string y1 = ComputeSha256Hash(textBox2.Text + textBox1.Text, user_salt);

                                    string x = textBox2.Text;
                                    int u = 0;
                                    for (int j = x.Length; j < 32; j++)
                                    {
                                        x +=y1[u];
                                    }
                                    aes_key= x;
                                    
                                    cn.Close();
                                    this.Hide();
                                    var form2 = new panel(user_id,user_name,password_user,user_salt,aes_key);
                                    form2.Closed += (s, args) => this.Close();
                                    form2.Show();
                                }
                                else
                                    MessageBox.Show("اطلاعات وارد شده صحیح نیستند");
                            }
                        }

                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Error in adding mysql row. Error: " + ex.Message);
                        }

                    }
                }
            textBox2.Text = "";
           
        }
        private void button2_Click(object sender, EventArgs e)
        {
          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            adduser f = new adduser();
            f.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
