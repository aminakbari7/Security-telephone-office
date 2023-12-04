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
    public partial class panel : Form
    {
        string user_id;
        string user_name;
        string password_user;
        string aes_key;
        string user_salt;
        public panel(string uid, string uname, string upass, string usal, string uaesk)
        {
            user_id = uid;
            user_name = uname;
            password_user = upass;
            aes_key = uaesk;
            user_salt = usal;


            InitializeComponent();
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

                    string query = "INSERT INTO phones(userid,salt,hn, namephone,hp,phonenumber,time) VALUES (?userid,?salt,?hn,?namephone,?hp,?phonenumber,?time);";
                    cn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, cn))
                    {
                        Random rnd = new Random();
                        string t1 = fksalt.ToString();

                        for (int j = t1.Length; j < aes_key.Length; j++)
                        {
                            t1 += aes_key[j];
                        }
                        string local_aes_key = t1;
                        var encryptedString1 = aes.EncryptString(local_aes_key, fkname);
                        var encryptedString2 = aes.EncryptString(local_aes_key, fknumber);
                        string thash = ComputeSha256Hash(user_id+fksalt, fksalt);
                        cmd.Parameters.Add("?userid", MySqlDbType.VarString).Value = thash;
                        cmd.Parameters.Add("?salt", MySqlDbType.VarString).Value = fksalt;

                        cmd.Parameters.Add("?hn", MySqlDbType.VarString).Value = ComputeSha256Hash(encryptedString1, 0);
                        cmd.Parameters.Add("?namephone", MySqlDbType.VarString).Value = encryptedString1;

                        cmd.Parameters.Add("?hp", MySqlDbType.VarString).Value = ComputeSha256Hash(encryptedString2, 0);
                        cmd.Parameters.Add("?phonenumber", MySqlDbType.VarString).Value = encryptedString2;
                        DateTime dateTimeVariable = DateTime.Now;
                        string date = dateTimeVariable.ToString("yyyy-MM-dd H:mm:ss");
                        cmd.Parameters.Add("?time", MySqlDbType.VarString).Value = aes.EncryptString(local_aes_key, date);
                        cmd.ExecuteNonQuery();
                    }

                }
            }

        }
        public void grid()
        {
            string zz= aes. EncryptString(aes_key, user_id);
            string connetionString = "server=localhost;database=systemphone1;uid=root;pwd=\"\";";
            string query = "SELECT * FROM phones";
            string query2 = "SELECT * FROM phones where id ='" +1+"'";
            using (MySqlConnection conn = new MySqlConnection(connetionString))
            {
                conn.Open();    
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0]; 
                }
                conn.Close();
                int s=dataGridView1.RowCount;

                string[] lid = new string[s];
                string[] lu = new string[s];
                string[] ls = new string[s];

                string[] lhn = new string[s];
                string[] ln = new string[s];


                string[] lhp = new string[s];
                string[] lp = new string[s];

                string[] lt = new string[s];
                for (int i= 0; i < s-1; i++)
                {
                    lid[i]=dataGridView1.Rows[i].Cells[0].Value.ToString();
                    lu[i] = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    ls[i] = dataGridView1.Rows[i].Cells[2].Value.ToString();

                    lhn[i] = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    ln[i] = dataGridView1.Rows[i].Cells[4].Value.ToString();

                    lhp[i] = dataGridView1.Rows[i].Cells[5].Value.ToString();
                    lp[i] = dataGridView1.Rows[i].Cells[6].Value.ToString();


                    lt[i] = dataGridView1.Rows[i].Cells[7].Value.ToString();
                }
                foreach (DataGridViewRow dataGridView1 in dataGridView1.Rows)
                {
                    dataGridView1.Cells[0].Value = 0;
                    for (int i = 1; i < 8; i++)
                    {
                        dataGridView1.Cells[i].Value ="";     
                    }
                }
                int j = 0;
                for (int i = 0; i < s - 1; i++)
                {
                    string temp = ComputeSha256Hash(user_name, int.Parse(ls[i])+ int.Parse(user_id));

                    if(lu[i] == temp && int.Parse(lid[i])!=0)
                    {
                        // string t1=ls[i];
                        string op =ls[i] ;
                        for (int u = op.Length; u < aes_key.Length; u++)
                        {
                           op+= aes_key[u];
                        }
                        
                        dataGridView1.Rows[j].Cells[0].Value = int.Parse(lid[i]);
                        dataGridView1.Rows[j].Cells[1].Value = user_id;
                        dataGridView1.Rows[j].Cells[2].Value = int.Parse(ls[i]);
        
                        if (lhn[i] == ComputeSha256Hash(ln[i], int.Parse(ls[i])))
                        {
                            //MessageBox.Show(ComputeSha256Hash(ln[j], 0));

                            dataGridView1.Rows[j].Cells[3].Value = lhn[j];
                            dataGridView1.Rows[j].Cells[4].Value = aes.DecryptString(op, ln[i]).ToString();
                        }
                        else
                        {
                            dataGridView1.Rows[j].Cells[3].Value = "دستکاری شده";
                            dataGridView1.Rows[j].Cells[4].Value = "دستکاری شده";
                        }
                        //

                        if (lhp[i] == ComputeSha256Hash(lp[i], int.Parse(ls[i])))
                        {
                            dataGridView1.Rows[j].Cells[5].Value = lhp[j];
                            dataGridView1.Rows[j].Cells[6].Value = aes.DecryptString(op, lp[i]).ToString(); 

                        }
                        else
                        {
                            dataGridView1.Rows[j].Cells[5].Value = "دستکاری شده";
                            dataGridView1.Rows[j].Cells[6].Value = "دستکاری شده";
                        }
                        dataGridView1.Rows[j].Cells[7].Value = aes.DecryptString(op, lt[i]).ToString();
                        j++;
                    } 
                }

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
               
            }
        }
        private void panel_Load(object sender, EventArgs e)
        {
            grid();
            dataGridView1.Columns[0].HeaderText = "شناسه";
            dataGridView1.Columns[1].HeaderText = " کاربر";
            dataGridView1.Columns[2].HeaderText = " نمک";
            dataGridView1.Columns[3].HeaderText = "هش اسم ";
            dataGridView1.Columns[4].HeaderText = "نام ";
            dataGridView1.Columns[5].HeaderText = "هش شماره ";
            dataGridView1.Columns[6].HeaderText = "شماره تلفن";
            dataGridView1.Columns[7].HeaderText = " تاریخ ثبت";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
       
        
        static public string ComputeSha256Hash(string input, int salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt.ToString());
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            fakefiller();
            MySqlConnection cnn;
            string  connetionString = "server=localhost;database=systemphone1;uid=root;pwd=\"\";";
            using (MySqlConnection cn = new MySqlConnection(connetionString))
            {
                try
                {
                    string query = "INSERT INTO phones(userid,salt,hn, namephone,hp,phonenumber,time) VALUES (?userid,?salt,?hn,?namephone,?hp,?phonenumber,?time);";
                    cn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, cn))
                    {
                        Random rnd = new Random();
                        int num = rnd.Next();
                        string t1=num.ToString();
                       
                        for(int i=t1.Length;i<aes_key.Length;i++)
                        {
                            t1+=aes_key[i];
                        }
                        string  local_aes_key = t1;
                       // MessageBox.Show(local_aes_key);
                        var encryptedString1 = aes.EncryptString(local_aes_key, textBox1.Text);
                        var encryptedString2 = aes.EncryptString(local_aes_key, textBox2.Text);
                        string thash = ComputeSha256Hash(user_name,num+int.Parse(user_id));
                        cmd.Parameters.Add("?userid", MySqlDbType.VarString).Value = thash;
                        cmd.Parameters.Add("?salt", MySqlDbType.VarString).Value = num;

                        cmd.Parameters.Add("?hn", MySqlDbType.VarString).Value = ComputeSha256Hash(encryptedString1,num);
                        cmd.Parameters.Add("?namephone", MySqlDbType.VarString).Value = encryptedString1;

                        cmd.Parameters.Add("?hp", MySqlDbType.VarString).Value = ComputeSha256Hash(encryptedString2, num);
                        cmd.Parameters.Add("?phonenumber", MySqlDbType.VarString).Value = encryptedString2;
                        DateTime dateTimeVariable = DateTime.Now;
                        string date = dateTimeVariable.ToString("yyyy-MM-dd H:mm:ss");
                        cmd.Parameters.Add("?time", MySqlDbType.VarString).Value = aes.EncryptString(local_aes_key, date);
                        cmd.ExecuteNonQuery();
                    }
                    fakefiller();
                    MessageBox.Show("شماره جدید ثبت شد");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error in adding mysql row. Error: " + ex.Message);
                }
                textBox1.Text = "";
                textBox2.Text = "";
                cn.Close();
                grid();
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {
            int ide = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            DialogResult result = MessageBox.Show("آیا مطمئن هستید؟", "system message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //
                string connetionString = null;
                string query = "DELETE FROM phones WHERE id=@id";
                connetionString = "server=localhost;database=systemphone1;uid=root;pwd=\"\";";
                using (MySqlConnection conn = new MySqlConnection(connetionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = dataGridView1.CurrentRow.Cells[0].Value;
                    cmd.ExecuteNonQuery();
                }
               
                MessageBox.Show("با موفقیت حذف شد", "system message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //
            }

                
                grid();
            }

        private void button4_Click(object sender, EventArgs e)
        {
          
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string connetionString = "server=localhost;database=systemphone1;uid=root;pwd=\"\";";
            using (MySqlConnection cn = new MySqlConnection(connetionString))
            {
                string query = "INSERT INTO history(userid,status) VALUES (?userid,?status);";
                cn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, cn))
                {
                    cmd.Parameters.Add("?userid", MySqlDbType.VarChar).Value = aes.EncryptString(aes_key, user_id);
                    string ll= "شما از نرم افزرا خارج شدید";
                    cmd.Parameters.Add("status", MySqlDbType.VarChar).Value = aes.EncryptString(aes_key, ll);
                    cmd.ExecuteNonQuery();
                }
                cn.Close();
                this.Close();
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {

           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int idp = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            string name = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            string number = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            this.Hide();
            edit h = new edit(idp,name,number, user_id, user_name, password_user, user_salt, aes_key);
            h.Closed += (s, args) => this.Close();
            h.Show();
        }
    }
    }

