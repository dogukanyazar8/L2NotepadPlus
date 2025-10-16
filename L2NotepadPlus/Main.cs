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
        private string currentFilePath = string.Empty;
        private const string appTitle = "L2Notepad Plus";

        private bool isDirty = false;        // Kullanıcı değişiklik yaptı mı
        private bool isL2ConfigFile = false; // Açılan dosya L2Config mi
        private bool isLoading = false;      // Dosya yükleniyor mu
        private bool isSaving = false;       // Dosya kaydediliyor mu

        // --- Kurucu Metot ---
        public MainForm()
        {
            InitializeComponent();
            InitializeEditor();

            // Form olayları
            this.FormClosing += Main_FormClosing;
            this.mainNotepad.TextChanged += mainNotepad_TextChanged;
        }

        // --- Başlangıç durumunu sıfırlar ---
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

        // --- Pencere başlığını günceller ---
        private void UpdateTitle()
        {
            string fileName = string.IsNullOrEmpty(currentFilePath)
                ? "Untitled"
                : Path.GetFileName(currentFilePath);

            string dirtyMark = isDirty ? "*" : "";
            this.Text = $"{fileName}{dirtyMark} - {appTitle}";
        }

        // --- RichTextBox içeriği değiştiğinde çalışır ---
        private void mainNotepad_TextChanged(object? sender, EventArgs e)
        {
            // Dosya yüklenirken veya kaydedilirken değişiklik olarak sayma
            if (isLoading || isSaving)
                return;

            isDirty = true;
            UpdateTitle();
        }

        // --- Form kapatılırken değişiklik varsa uyar ---
        private void Main_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!PromptSaveIfNeeded())
                e.Cancel = true; // Kullanıcı iptal etti
        }

        // --- Kullanıcıdan kaydetme onayı ister ---
        private bool PromptSaveIfNeeded()
        {
            // Eğer içerik boşsa veya değişiklik yapılmadıysa sormaya gerek yok
            if (!isDirty || mainNotepad.TextLength == 0)
                return true;

            var result = MessageBox.Show(
                "Değişiklikler kaydedilsin mi?",
                "Kaydet",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
                return false;

            if (result == DialogResult.Yes)
                return SaveCurrentDocument();

            return true; // "Hayır" seçildi
        }

        // --- Dosyayı kaydet (Save As gerekiyorsa diyalog açar) ---
        private bool SaveCurrentDocument()
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

            // Kaydetme süreci başlıyor
            isSaving = true;
            bool success = L2ConfigWriter.WriteFile(currentFilePath, content, saveAsL2Config, Encoding.UTF8);
            isSaving = false;

            // Başarılıysa durumu sıfırla
            if (success)
            {
                isDirty = false;
                string fileType = saveAsL2Config ? "L2Config dosyası" : "Metin dosyası";
                MessageBox.Show($"{fileType} başarıyla kaydedildi.", "Kaydetme Başarılı",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateTitle();
            }

            return success;
        }

        // --- Kullanıcı "File → Open" seçerse dosya aç ---
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

                // Dosya yüklenmeye başlıyor
                isLoading = true;
                string content = L2ConfigReader.Read(filePath, string.Empty, out detectedL2);
                mainNotepad.Text = content;
                isLoading = false;

                // Dosya bilgilerini güncelle
                currentFilePath = filePath;
                isDirty = false;
                isL2ConfigFile = detectedL2;

                // Radiobuttonları tespit edilen dosya türüne göre işaretle
                radioL2Config.Checked = detectedL2;
                radioTextFile.Checked = !detectedL2;

                UpdateTitle();
            }
        }

        // --- Basit kaydet (Save) ---
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

            // Kaydetme süreci başlıyor
            isSaving = true;
            bool success = L2ConfigWriter.WriteFile(currentFilePath, content, saveAsL2Config, Encoding.UTF8);
            isSaving = false;

            if (success)
            {
                isDirty = false;
                string fileType = saveAsL2Config ? "L2Config dosyası" : "Metin dosyası";
                MessageBox.Show($"{fileType} başarıyla kaydedildi.", "Kaydetme Başarılı",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateTitle();
            }

            return success;
        }

        // --- Menü: File → Save ---
        private void menuFileSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        // --- Menü: File → New ---
        private void menuFileNew_Click(object sender, EventArgs e)
        {
            // Eğer kullanıcı değişiklik yaptıysa uyar
            if (isDirty)
            {
                DialogResult result = MessageBox.Show(
                    "Değişiklikleri kaydetmeden yeni belge açmak istiyor musunuz?",
                    "Yeni Belge",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            InitializeEditor();
        }

        private void menuL2ConfigAnalyze_Click(object sender, EventArgs e)
        {
            // Eğer notepad boşsa kullanıcıya uyarı göster
            if (string.IsNullOrWhiteSpace(mainNotepad.Text))
            {
                MessageBox.Show("Analiz edilecek bir içerik bulunamadı.", "Uyarı",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Yeni analiz formunu aç
            FormAnalyze analyzeForm = new FormAnalyze();

            // İstersen notepad içeriğini bu forma aktarabilirsin
            analyzeForm.OriginalContent = mainNotepad.Text;
            analyzeForm.FileName = Path.GetFileName(currentFilePath);

            // Analiz penceresini modal olarak göster
            analyzeForm.ShowDialog(this);
        }
    }
}
