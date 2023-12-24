using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CCTxt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public static string cctxtFile;
        public void Seperate(string path)
        {
            cctxtFile = path;
            Form2 form = new Form2();
            form.ShowDialog();
        }

        public static string cctx = string.Empty;

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (listBox1.Items[0] == "Drag notepads here...") listBox1.Items.Clear();
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (file.EndsWith(".cctxt"))
                    {
                        listBox1.Items.Clear();
                        e.Effect = DragDropEffects.None;
                        cctx = file;
                        return;
                    }

                    if (!file.EndsWith(".txt")) continue;
                    listBox1.Items.Add(file);
                }
            }
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0 || listBox1.Items[0] == "Drag notepads here...")
            {
                MessageBox.Show("En az bir not defteri import etmeniz gereklidir.", "CCText");
                return;
            }
            if (textBox1.ForeColor == Color.Gray)
            {
                MessageBox.Show("Lütfen istediğiniz concat dosya adını giriniz.");
                return;
            }

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Kayıt Klasörünü Seçin";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            if (DialogResult.OK != fbd.ShowDialog()) return;


            string finalName = textBox1.Text.Replace(".cctxt", string.Empty).Trim().Replace(" ", string.Empty);
            if (File.Exists(fbd.SelectedPath+$"\\{finalName}.cctxt"))
            {
                MessageBox.Show(finalName+".cctxt dosyası bu konumda zaten mevcut!");
                return;
            }


            string total = string.Empty;

            foreach (string file in listBox1.Items)
            {
                string[] arr = file.Split('\\');
                total += $"### CCTEXTBYKUSTAH *{arr[arr.Length-1]}* ###";
                total += "\n"+File.ReadAllText(file)+"\n";
                File.Delete(file);
            }



            File.WriteAllText(fbd.SelectedPath+$"\\{finalName}.cctxt", total);
            listBox1.Text = string.Empty;
            listBox1.Items.Clear();
            textBox1.Text = string.Empty;
            textBox1_Leave(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.ForeColor == Color.Gray)
            {
                textBox1.Text = string.Empty;
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = "Concat File Name";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) listBox1.Items.Add("Drag notepads here...");
            if (cctx == string.Empty) return;
            timer1.Stop();
            Seperate(cctx);
            timer1.Start();
            cctx = string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0 || listBox1.Items[0] == "Drag notepads here..." || listBox1.SelectedIndex == -1)
            {
                return;
            }

            listBox1.Items.Remove(listBox1.Items[listBox1.SelectedIndex]);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
