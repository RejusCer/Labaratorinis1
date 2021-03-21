using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Labaratorinis1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\PC\OneDrive\Documents\Mokiniai.mdf;Integrated Security=True;Connect Timeout=30");
        public class Mokinys
        {
            public string mokinioVardas { get; set; }

            public int balas { get; set; }
        }

        public class Paskaita
        {
            public Mokinys[] moki;

            public Paskaita()
            {
                moki = new Mokinys[7];

                for (int i = 0; i < moki.Length; i++)
                {
                    moki[i] = new Mokinys();
                }
            }
        }

        public class Modulis
        {
            public string pavadinimas;
            public Paskaita[] pask;
            
            public Modulis()
            {
                pask = new Paskaita[5];

                for (int i = 0; i < pask.Length; i++)
                {
                    pask[i] = new Paskaita(); 
                }
            }

        }

        Modulis[] mod = new Modulis[3];

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < mod.Length; i++)
            {
                mod[i] = new Modulis(); // sukonstruoja objekta
            }


            string[] Moduliai = { "Matematika", "Python", "Objektinis" };
            for (int i = 0; i < Moduliai.Length; i++)
            {
                mod[i].pavadinimas = Moduliai[i]; // Priskiria modulio pavadinimus
            }

            for (int i = 0; i < Moduliai.Length; i++)
            {
                comboBox1.Items.Add(mod[i].pavadinimas);
            }



            int n = 213; // ID skaitliukas
            SqlDataReader da;

            con.Open();
            for (int i = 0; i < mod.Length; i++) // Priskiria vardus ir balus is DB
            {
                for (int j = 0; j < mod[i].pask.Length; j++)
                {
                    for (int k = 0; k < mod[i].pask[j].moki.Length; k++)
                    {
                        SqlCommand cmd = new SqlCommand("SELECT vardas, balas FROM [Table] WHERE Id = @id", con);
                        cmd.Parameters.AddWithValue("@id", n);
                        da = cmd.ExecuteReader();
                        while (da.Read())
                        {
                            mod[i].pask[j].moki[k].mokinioVardas = da.GetValue(0).ToString();
                            mod[i].pask[j].moki[k].balas = (int)da.GetValue(1);
                        }
                        da.Close();
                        n++;
                    }
                }
            }
            con.Close();

            ApskaiciuotiGalutiniBala();

            AtnaujintiLentele(); // atnaujina RichTextBox
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "";

            for (int i = 0; i < mod[comboBox1.SelectedIndex].pask.Length; i++)
            {
                comboBox2.Items.Add(mod[comboBox1.SelectedIndex].pavadinimas + " Paskaita " + (i+1));
            }
            AtnaujintiLentele(comboBox1.SelectedIndex);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox3.Text = "";

            for (int i = 0; i < mod[comboBox1.SelectedIndex].pask[comboBox2.SelectedIndex].moki.Length; i++)
            {
                comboBox3.Items.Add(mod[comboBox1.SelectedIndex].pask[comboBox2.SelectedIndex].moki[i].mokinioVardas);
            }
            AtnaujintiLentele(comboBox1.SelectedIndex, comboBox2.SelectedIndex);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Visible = true;
            try
            {
                label1.Text = mod[comboBox1.SelectedIndex].pask[comboBox2.SelectedIndex].moki[comboBox3.SelectedIndex].mokinioVardas;
                label1.Text += " " + mod[comboBox1.SelectedIndex].pask[comboBox2.SelectedIndex].moki[comboBox3.SelectedIndex].balas;
            }
            catch
            {
                MessageBox.Show("Klaida! Pasirinkite paskaitą.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Int32.Parse(textBox1.Text) >= 0 && Int32.Parse(textBox1.Text) <= 10)
                {
                    mod[comboBox1.SelectedIndex].pask[comboBox2.SelectedIndex].moki[comboBox3.SelectedIndex].balas = Int32.Parse(textBox1.Text);
                    label1.Text = mod[comboBox1.SelectedIndex].pask[comboBox2.SelectedIndex].moki[comboBox3.SelectedIndex].mokinioVardas;
                    label1.Text += " " + textBox1.Text;
                    textBox1.Text = "";
                    MessageBox.Show("Sekmingai pakeistas/įrašytas pažymys");
                }
                else
                {
                    MessageBox.Show("Klaida! (0-10)");
                }
            }
            catch
            {
                MessageBox.Show("Įveskite pažymį");
            }
            AtnaujintiLentele(comboBox1.SelectedIndex, comboBox2.SelectedIndex);
            ApskaiciuotiGalutiniBala();
        }

        private void button2_Click(object sender, EventArgs e) // Issaugo duomenys
        {
            try
            {
                con.Open();
                for (int i = 0; i < mod.Length; i++)
                {
                    for (int j = 0; j < mod[i].pask.Length; j++)
                    {
                        for (int k = 0; k < mod[i].pask[j].moki.Length; k++)
                        {
                            //cmd = new SqlCommand("INSERT INTO [Table] values('" + mod[i].pavadinimas + "', '" + j + "', '" + mod[i].pask[j].moki[k].mokinioVardas + "', '" + mod[i].pask[j].moki[k].balas + "')", con);
                            //cmd.ExecuteNonQuery();


                            var sql = "UPDATE [Table] SET balas = @balas WHERE modulis = @modulis AND paskaita = @paskaita AND vardas = @vardas";
                            try
                            {
                                using (var command = new SqlCommand(sql, con))
                                {
                                    command.Parameters.Add("@modulis", SqlDbType.NVarChar).Value = mod[i].pavadinimas;
                                    command.Parameters.Add("@paskaita", SqlDbType.Int).Value = j;
                                    command.Parameters.Add("@vardas", SqlDbType.NVarChar).Value = mod[i].pask[j].moki[k].mokinioVardas;
                                    command.Parameters.Add("@balas", SqlDbType.Int).Value = mod[i].pask[j].moki[k].balas;

                                    command.ExecuteNonQuery();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Nepavyko išsaugoti. Error: {ex.Message}");
                            }

                        }
                    }
                }
                MessageBox.Show("Išsaugota");
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nepavyko išsaugoti. Error: {ex.Message}");
            }
        }

        public void AtnaujintiLentele()
        {
            richTextBox1.Text = "";

            for (int i = 0; i < mod.Length; i++)
            {
                for (int j = 0; j < mod[i].pask.Length; j++)
                {
                    for (int k = 0; k < mod[i].pask[j].moki.Length; k++)
                    {
                        richTextBox1.Text += mod[i].pavadinimas + " " + (j+1) + ";   ";
                        richTextBox1.Text += mod[i].pask[j].moki[k].mokinioVardas + " ";
                        richTextBox1.Text += mod[i].pask[j].moki[k].balas;
                        richTextBox1.Text += "\n";
                    }
                }
            }
        }
        public void AtnaujintiLentele(int modSk)
        {
            richTextBox1.Text = "";

            for (int j = 0; j < mod[modSk].pask.Length; j++)
            {
                for (int k = 0; k < mod[modSk].pask[j].moki.Length; k++)
                {
                    richTextBox1.Text += mod[modSk].pavadinimas + " " + (j+1) + ";   ";
                    richTextBox1.Text += mod[modSk].pask[j].moki[k].mokinioVardas + " ";
                    richTextBox1.Text += mod[modSk].pask[j].moki[k].balas;
                    richTextBox1.Text += "\n";
                }
            }
        }
        public void AtnaujintiLentele(int modSk, int pasSk)
        {
            richTextBox1.Text = "";
            
            for (int k = 0; k < mod[modSk].pask[pasSk].moki.Length; k++)
            {
                richTextBox1.Text += mod[modSk].pask[pasSk].moki[k].mokinioVardas + " ";
                richTextBox1.Text += mod[modSk].pask[pasSk].moki[k].balas;
                richTextBox1.Text += "\n";
            }
        }

        public void ApskaiciuotiGalutiniBala()
        {
            richTextBox2.Text = "";
            double sum;
            double galutinisBalas;
            for (int i = 0; i < mod.Length; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    galutinisBalas = 0;
                    sum = 0;
                    richTextBox2.Text += mod[i].pavadinimas + " ";
                    richTextBox2.Text += mod[i].pask[0].moki[j].mokinioVardas + " ";
                    for (int k = 0; k < mod[i].pask.Length; k++)
                    {
                        sum += mod[i].pask[k].moki[j].balas;
                    }
                    galutinisBalas = sum / mod[i].pask.Length;
                    richTextBox2.Text += galutinisBalas + "\n";
                }
                richTextBox2.Text += "\n";
            }
        }
    }
}
