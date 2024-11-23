using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Atlas_Ownership_Changer
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private string selectedPath = @"";
        public Form1()
        {
            InitializeComponent();
            pictureBox1.MouseDown += PictureBox_MouseDown;
            pictureBox1.MouseMove += PictureBox_MouseMove;
            pictureBox1.MouseUp += PictureBox_MouseUp;
            label1.MouseDown += PictureBox_MouseDown;
            label1.MouseMove += PictureBox_MouseMove;
            label1.MouseUp += PictureBox_MouseUp;
            textBox1.TextChanged += TextBox_TextChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Environment.MachineName != "ATLASPC")
            {
                MessageBox.Show("Üzgünüm, Bilgisayarýnýz Atlas Workspace Alan Adýna Dahil Deðil!\nDahil Olmak Ýçin Custom Windows'umuzu Yükleyebilirsiniz.", "Atlas Workspace");
                Environment.Exit(0);
            }
            else
            {
                label1.Text = "Atlas Sahiplik Deðiþtirici";
                button4.Text = "Gözat";
                textBox1.PlaceholderText = "Klasör Yolu";
                button3.Text = "Bütün Sistemin Sahipliðini Aktar (C Diski)";
                button2.Text = "Sahipliði Aktar (" + Environment.MachineName + "/" + Environment.UserName + ")";
            }
        }
        private void RunCommand(string fileName, string arguments)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                Verb = "runas"
            };
            using (Process process = Process.Start(processInfo))
            {
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                Form outputLog = new Form();
                Label log = new Label();
                log.Text = output;
                outputLog.Controls.Add(log);
                outputLog.Show();

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception("PowerShell komut çalýþtýrma hatasý: " + error);
                }
            }
        }
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }
        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private void TextBox_TextChanged(object? sender, EventArgs e)
        {
            selectedPath = textBox1.Text;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("cmd.exe", $"/c takeown /F \"{selectedPath}\" /R /D Y");
                Process.Start("cmd.exe", $"/c icacls \"{selectedPath}\" /grant {Environment.UserName}:(F) /T");
                MessageBox.Show("Klasör sahipliði alýndý, kullanýcýya tam yetki verildi ve kullanýcý yönetici grubuna eklendi.", "Atlas Workspace");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluþtu: " + ex.Message);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string programfiles = @"C:\Program Files";
            string program32 = @"C:\Program Files (x86)";
            string windows = @"C:\Windows";
            string perflogs = @"C:\PerfLogs";
            string users = @"C:\Users";
            try
            {
                //Program Files
                Process.Start("cmd.exe", $"/c takeown /F \"{programfiles}\" /R /D Y");
                Process.Start("cmd.exe", $"/c icacls \"{programfiles}\" /grant {Environment.UserName}:(F) /T");

                //Program Files X86
                Process.Start("cmd.exe", $"/c takeown /F \"{program32}\" /R /D Y");
                Process.Start("cmd.exe", $"/c icacls \"{program32}\" /grant {Environment.UserName}:(F) /T");

                //Windows
                Process.Start("cmd.exe", $"/c takeown /F \"{windows}\" /R /D Y");
                Process.Start("cmd.exe", $"/c icacls \"{windows}\" /grant {Environment.UserName}:(F) /T");

                //Performance Logs
                Process.Start("cmd.exe", $"/c takeown /F \"{perflogs}\" /R /D Y");
                Process.Start("cmd.exe", $"/c icacls \"{perflogs}\" /grant {Environment.UserName}:(F) /T");

                //Users
                Process.Start("cmd.exe", $"/c takeown /F \"{users}\" /R /D Y");
                Process.Start("cmd.exe", $"/c icacls \"{users}\" /grant {Environment.UserName}:(F) /T");

                MessageBox.Show("Artýk C Diskindeki Bütün Dosyalarýn ve Klasörlerin Sahibisiniz!", "Atlas Workspace");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluþtu: " + ex.Message);
            }
        }
    }
}
