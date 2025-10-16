using L2NotepadPlus.Analyze;
using L2NotepadPlus.Core;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace L2NotepadPlus
{
    public partial class MainForm : Form
    {
        // --- Durum değişkenleri ---
        private string currentFilePath = string.Empty;  // Açık dosyanın tam yolu
        private const string appTitle = "L2Notepad Plus";

        private bool isDirty = false;        // Kullanıcı değişiklik yaptı mı
        private bool isL2ConfigFile = false; // Dosya türü L2Config mi
        private bool isLoading = false;      // Dosya yükleme süreci aktif mi
        private bool isSaving = false;       // Dosya kaydetme süreci aktif mi

        // --- Kurucu metot ---
        public MainForm()
        {
            InitializeComponent();
            InitializeEditor();

            // Temel olay bağlantıları
            this.FormClosing += Main_FormClosing;
            this.mainNotepad.TextChanged += mainNotepad_TextChanged;
        }

        // --- Editör başlangıç durumunu sıfırlar ---
        private void InitializeEditor()
        {
            mainNotepad.Clear();
            currentFilePath = string.Empty;
            isDirty = false;
            isL2ConfigFile = false;
            radioTextFile.Checked = true;
            radioL2Config.Checked = false;
            UpdateTitle();
        }

        // --- Başlık çubuğunu günceller ---
        private void UpdateTitle()
        {
            string fileName = string.IsNullOrEmpty(currentFilePath)
                ? "Untitled"
                : Path.GetFileName(currentFilePath);

            string dirtyMark = isDirty ? "*" : "";
            this.Text = $"{fileName}{dirtyMark} - {appTitle}";
        }

        // --- RichTextBox içeriği değiştiğinde çağrılır ---
        private void mainNotepad_TextChanged(object? sender, EventArgs e)
        {
            // Dosya yükleme/kaydetme esnasında değişiklik sayılmaz
            if (isLoading || isSaving)
                return;

            isDirty = true;
            UpdateTitle();
        }

        // --- Form kapatılmadan önce olası değişiklikleri kontrol eder ---
        private void Main_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!PromptSaveIfNeeded())
                e.Cancel = true; // Kullanıcı iptal etti
        }

        // --- Kullanıcıdan kaydetme onayı ister ---
        private bool PromptSaveIfNeeded()
        {
            if (!isDirty || mainNotepad.TextLength == 0)
                return true; // Değişiklik yoksa devam et

            var result = MessageBox.Show(
                "Değişiklikler kaydedilsin mi?",
                "Kaydet",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
                return false;

            if (result == DialogResult.Yes)
                return SaveCurrentDocument();

            return true; // "Hayır" seçildiyse devam et
        }

        // --- Dosyayı kaydeder; gerekirse Save As diyalog açar ---
        private bool SaveCurrentDocument()
        {
            bool saveAsL2Config = radioL2Config.Checked;

            // Eğer daha önce dosya yolu belirlenmemişse kullanıcıdan alınır
            if (string.IsNullOrEmpty(currentFilePath))
            {
                using SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Dosyayı Kaydet",
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                    FileName = "Untitled.txt"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return false;

                currentFilePath = saveDialog.FileName;
            }

            string content = mainNotepad.Text;
            isSaving = true;
            bool success = L2ConfigWriter.WriteFile(currentFilePath, content, saveAsL2Config, Encoding.UTF8);
            isSaving = false;

            if (success)
            {
                isDirty = false;
                string fileType = saveAsL2Config ? "L2Config dosyası" : "Metin dosyası";
                MessageBox.Show($"{fileType} başarıyla kaydedildi.",
                                "Kaydetme Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateTitle();
            }

            return success;
        }

        // --- Menü: File → Open ---
        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            if (!PromptSaveIfNeeded())
                return;

            using OpenFileDialog openDialog = new OpenFileDialog
            {
                Title = "Config Dosyası Seçin",
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openDialog.FileName;
                bool detectedL2;

                isLoading = true;
                string content = L2ConfigReader.Read(filePath, string.Empty, out detectedL2);
                isLoading = false;

                mainNotepad.Text = content;
                currentFilePath = filePath;
                isDirty = false;
                isL2ConfigFile = detectedL2;

                radioL2Config.Checked = detectedL2;
                radioTextFile.Checked = !detectedL2;

                UpdateTitle();
            }
        }

        // --- Menü: File → Save ---
        private void menuFileSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        // --- Menü: File → New ---
        private void menuFileNew_Click(object sender, EventArgs e)
        {
            if (isDirty)
            {
                DialogResult result = MessageBox.Show(
                    "Değişiklikleri kaydetmeden yeni belge açmak istiyor musunuz?",
                    "Yeni Belge", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            InitializeEditor();
        }

        // --- Dosyayı analiz formunda açar ---
        private void menuL2ConfigAnalyze_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("Lütfen önce bir dosya açın.", "Uyarı",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var type = ConfigTypeDetector.DetectFromFileName(currentFilePath);
            if (type == ConfigTypeDetector.ConfigType.Unknown)
            {
                MessageBox.Show("Dosya tipi tanınamadı.",
                                "Bilinmeyen Dosya", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var analyzeForm = new FormAnalyze
            {
                OriginalContent = mainNotepad.Text,
                FileName = Path.GetFileName(currentFilePath),
                DetectedConfigType = type
            };

            var result = analyzeForm.ShowDialog(this);

            // Analyze formda değişiklik yapıldıysa içeriği güncelle
            if (result == DialogResult.OK && !string.IsNullOrEmpty(analyzeForm.UpdatedHexData))
                mainNotepad.Text = analyzeForm.UpdatedHexData;
        }

        // --- Basit kaydet işlemi (menüden veya kısayoldan) ---
        private bool Save()
        {
            bool saveAsL2Config = radioL2Config.Checked;

            if (string.IsNullOrEmpty(currentFilePath))
            {
                using SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Dosyayı Kaydet",
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                    FileName = "Untitled.txt"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return false;

                currentFilePath = saveDialog.FileName;
            }

            string content = mainNotepad.Text;
            isSaving = true;
            bool success = L2ConfigWriter.WriteFile(currentFilePath, content, saveAsL2Config, Encoding.UTF8);
            isSaving = false;

            if (success)
            {
                isDirty = false;
                string fileType = saveAsL2Config ? "L2Config dosyası" : "Metin dosyası";
                MessageBox.Show($"{fileType} başarıyla kaydedildi.",
                                "Kaydetme Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateTitle();
            }

            return success;
        }
    }
}
