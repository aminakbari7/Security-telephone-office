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
    public partial class edit : Form
    {
        int idp;
        string user_id;
        string user_name;
        string password_user;
        string aes_key;
        string user_salt;
        string name;
        string number;

        public edit(int idpp,string nameu,string numberu,string uid, string uname, string upass, string usal, string uaesk)
        {
            name= nameu;
            number=numberu;
            idp= idpp;
            user_id = uid;
            user_name = uname;
            password_user = upass;
            aes_key = uaesk;
            user_salt = usal;
            InitializeComponent();
        }
        static public string ComputeSha256Hash(string input, int salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt.ToString());
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);

        }
        private void edit_Load(object sender, EventArgs e)
        {
            textBox1.Text = name;
            textBox2.Text = number;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            MySqlConnection cnn;
            string connetionString = "server=localhost;database=systemphone1;uid=root;pwd=\"\";";
            Random rnd = new Random();
            int num = rnd.Next();
            string t1 = num.ToString();

            for (int i = t1.Length; i < aes_key.Length; i++)
            {
                t1 += aes_key[i];
            }
            string local_aes_key = t1;
            var encryptedString1 = aes.EncryptString(local_aes_key, textBox1.Text);
            var encryptedString2 = aes.EncryptString(local_aes_key, textBox2.Text);
            string thash = ComputeSha256Hash(user_name, num+ int.Parse(user_id));
            string query = "UPDATE phones SET userid=@userid, salt =@salt ,hn =@hn,namephone =@namephone,hp =@hp,phonenumber =@phonenumber, time=@time                 WHERE id ='"+idp+"'";
            using (MySqlConnection cn = new MySqlConnection(connetionString))
            {
                try
                {
                    cn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, cn))
                    {
                        cmd.Parameters.Add("?userid", MySqlDbType.VarString).Value = thash;
                        cmd.Parameters.Add("?salt", MySqlDbType.VarString).Value = num;
                        cmd.Parameters.Add("?hn", MySqlDbType.VarString).Value = ComputeSha256Hash(encryptedString1, num);
                        cmd.Parameters.Add("?namephone", MySqlDbType.VarString).Value = encryptedString1;

                        cmd.Parameters.Add("?hp", MySqlDbType.VarString).Value = ComputeSha256Hash(encryptedString2, num);
                        cmd.Parameters.Add("?phonenumber", MySqlDbType.VarString).Value = encryptedString2;
                        DateTime dateTimeVariable = DateTime.Now;
                        string date = dateTimeVariable.ToString("yyyy-MM-dd H:mm:ss");
                        cmd.Parameters.Add("?time", MySqlDbType.VarString).Value = aes.EncryptString(local_aes_key, date);
                        cmd.ExecuteNonQuery();
                    }
                   
                    MessageBox.Show("بروزرسانی با موفقیت  ثبت شد");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error in adding mysql row. Error: " + ex.Message);
                }
                this.Hide();
                var form2 = new panel(user_id, user_name, password_user, user_salt, aes_key);
               form2.Closed += (s, args) => this.Close();
               form2.Show();
            }
        }
    }
}
