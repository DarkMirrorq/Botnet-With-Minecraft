using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Plugin_Derleyici
{
    public partial class Form1 : Form
    {
        private string selectedOutputPath = "";
        private StringBuilder logBuilder = new StringBuilder(); // Maven loglarını burada tut

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e) { }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void textBox2_TextChanged(object sender, EventArgs e) { }

        private void label5_Click(object sender, EventArgs e) { }

        private async void button1_Click(object sender, EventArgs e)
        {
            string sunucuUrl = textBox1.Text;
            string sourceCodePath = Path.Combine(Application.StartupPath, "source_code");
            string envFilePath = Path.Combine(sourceCodePath, "src\\main\\resources\\.env");
            string targetPath = Path.Combine(sourceCodePath, "target");

            // Çıktı klasörü kontrolü
            if (string.IsNullOrWhiteSpace(selectedOutputPath) || !Directory.Exists(selectedOutputPath))
            {
                MessageBox.Show("Lütfen bir çıktı klasörü seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string outputDirectory = selectedOutputPath;
            logBuilder.Clear(); // Önceki logu temizle

            // .env dosyasını güncelle
            if (File.Exists(envFilePath))
            {
                File.WriteAllText(envFilePath, "API_URL=" + sunucuUrl);
            }
            else
            {
                MessageBox.Show(".env dosyası bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // MAVEN işlemi
            string mavenPath = Path.Combine(Application.StartupPath, "maven\\bin\\mvn");
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c \"{mavenPath}\" clean package && exit";
            process.StartInfo.WorkingDirectory = sourceCodePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            label5.Text = "İşlem başladı...";

            process.OutputDataReceived += (s, args) =>
            {
                if (args.Data != null)
                {
                    logBuilder.AppendLine(args.Data); // Log’a ekle
                }
            };
            process.ErrorDataReceived += (s, args) =>
            {
                if (args.Data != null)
                {
                    logBuilder.AppendLine("[ERR] " + args.Data); // Hatalı satırları da ekle
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Delay(15000); // 15 saniye bekle
            process.Kill();

            // Log dosyasını yaz
            string logFilePath = Path.Combine(outputDirectory, "log.txt");
            File.WriteAllText(logFilePath, logBuilder.ToString());

            // JAR dosyasını kopyala
            if (Directory.Exists(targetPath) && Directory.GetFiles(targetPath, "*.jar").Any())
            {
                string jarFilePath = Directory.GetFiles(targetPath, "*.jar").First();
                string jarFileName = Path.GetFileName(jarFilePath);
                string destinationPath = Path.Combine(outputDirectory, jarFileName);

                try
                {
                    File.Copy(jarFilePath, destinationPath, true);
                    label5.Text = "✅ Tamamlandı.";
                }
                catch
                {
                    label5.Text = "⚠ Kopyalama hatası.";
                }
            }
            else
            {
                label5.Text = "❌ JAR bulunamadı.";
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e) { }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Plugin çıktısının kaydedileceği klasörü seçin";
            folderDialog.ShowNewFolderButton = true;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                selectedOutputPath = folderDialog.SelectedPath;
                label6.Text = selectedOutputPath;
            }
        }
    }
}
