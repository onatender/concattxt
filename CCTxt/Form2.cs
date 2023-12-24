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
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CCTxt
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        static List<Notepad> notepads = new List<Notepad>();


        static string content;
        private void Form2_Load(object sender, EventArgs e)
        {
            timer1.Start();
            Start();
        }

        void Start()
        {
            notepads.Clear(); content = string.Empty;
            content = File.ReadAllText(Form1.cctxtFile);
            for (int i = 0; i < content.Split('\n').Length; i++)
            {
                string line = content.Split('\n')[i];
                if (line.Contains("### CCTEXTBYKUSTAH *"))
                {
                    Notepad np = new Notepad(line.Split('*')[1], i);
                    notepads.Add(np);
                }
            }

            ShowNotepads();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        class Notepad
        {
            public int startLine { get; set; }
            public string fileName { get; set; }
            public bool dynamic { get; set; }

            public Notepad(string _fileName, int _startLine) { this.fileName= _fileName; this.startLine = _startLine; }
            public Notepad(string _fileName, int _startLine, bool _dynamic) { this.fileName= _fileName; this.startLine = _startLine; this.dynamic =  _dynamic; }

            public void Simulate()
            {
                File.WriteAllText(fileName, GetContent());
                Process t = new Process();
                t.StartInfo.FileName = fileName;
                t.Start();
                t.WaitForExit();
                File.Delete(fileName);
            }

            public string GetContent()
            {
                string npContent = string.Empty;
                for (int i = startLine+1; i < content.Split('\n').Length; i++)
                {
                    string line = content.Split('\n')[i];
                    if (line.Contains("### CCTEXTBYKUSTAH *")) break;
                    npContent += line+"\n";
                }
                return npContent.Trim();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Kayıt Klasörünü Seçin";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            if (DialogResult.OK != fbd.ShowDialog()) return;


            foreach (Notepad notepad in notepads)
            {
                string dosyaAdi = notepad.fileName.Split('.')[0];
                string main = notepad.fileName.Split('.')[0];
                int tryCount = 2;
                bool degistiMi = false;
                while (File.Exists(fbd.SelectedPath+$"\\{dosyaAdi}.txt"))
                {
                    dosyaAdi = main+tryCount.ToString();
                    degistiMi = true;
                    tryCount++;
                }
                if (degistiMi) MessageBox.Show(main+" dosyası bu konumda zaten olduğu için yeni adı "+dosyaAdi+" olarak değiştirilmiştir.", "CCText", MessageBoxButtons.OK);
                File.WriteAllText(fbd.SelectedPath+$"\\{dosyaAdi}.txt", notepad.GetContent());
            }
            File.Delete(Form1.cctxtFile);
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        void ShowNotepads()
        {
            listBox1.Items.Clear();
            foreach (Notepad notepad in notepads)
            {
                listBox1.Items.Add(notepad.fileName);
            }
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("Concat dosyasına yeni text dosyası eklemek istiyor musun?", "CCText", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string text = File.ReadAllText(Form1.cctxtFile);
                foreach (string file in files)
                {
                    if (!file.EndsWith(".txt")) continue;
                    string[] arr = file.Split('\\');
                    Start();
                    text += $"### CCTEXTBYKUSTAH *{arr[arr.Length-1]}* ###";
                    text += "\n"+File.ReadAllText(file)+"\n";
                    File.WriteAllText(Form1.cctxtFile, text);
                    File.Delete(file);
                }
            }
            Start();
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0) { return; }
            if (DialogResult.Yes != MessageBox.Show("Dosyayı çalıştırmak istiyor musun?\nDosya üzerinde yaptığın değişiklikler kaydedilmeyecek.", "CCText", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
            notepads[(listBox1.SelectedIndex)].Simulate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0 || listBox1.Items[0] == idleMessage) return;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Kayıt Klasörünü Seçin";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            if (DialogResult.OK != fbd.ShowDialog()) return;


            string dosyaAdi = notepads[(listBox1.SelectedIndex)].fileName.Split('.')[0];
            string main = notepads[(listBox1.SelectedIndex)].fileName.Split('.')[0];
            int tryCount = 2;
            bool degistiMi = false;
            while (File.Exists(fbd.SelectedPath+$"\\{dosyaAdi}.txt"))
            {
                dosyaAdi = main+tryCount.ToString();
                degistiMi = true;
                tryCount++;
            }
            if (degistiMi) MessageBox.Show(main+" dosyası bu konumda zaten olduğu için yeni adı "+dosyaAdi+" olarak değiştirilmiştir.", "CCText", MessageBoxButtons.OK);
            File.WriteAllText(fbd.SelectedPath+$"\\{dosyaAdi}.txt", notepads[(listBox1.SelectedIndex)].GetContent());


            string total = string.Empty;

            notepads.Remove(notepads[listBox1.SelectedIndex]);

            foreach (Notepad notepad in notepads)
            {
                total += $"### CCTEXTBYKUSTAH *{notepad.fileName}* ###";
                total += "\n"+notepad.GetContent()+"\n";
            }
            File.WriteAllText(Form1.cctxtFile, total);
            Start();
        }

        string idleMessage = "No notepads in this cctxt file...";

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) { listBox1.Items.Add(idleMessage); }
        }
    }
}
