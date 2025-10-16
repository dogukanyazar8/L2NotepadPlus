using L2NotepadPlus.Analyze;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static L2NotepadPlus.Analyze.ConfigTypeDetector;

namespace L2NotepadPlus
{
    public partial class FormAnalyze : Form
    {
        // --- Özellikler ---
        public string OriginalContent { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public ConfigType DetectedConfigType { get; set; } = ConfigType.Unknown;
        public string? UpdatedHexData { get; private set; } = null;

        // --- Durum değişkenleri ---
        private TextBox? editBox;
        private bool dataModified = false; // Hücre düzenleme oldu mu

        // --- Kurucu metot ---
        public FormAnalyze()
        {
            InitializeComponent();
        }

        // --- Form yüklendiğinde çağrılır ---
        private void FormAnalyze_Load(object sender, EventArgs e)
        {
            this.Text = $"Analyze - {FileName} ({DetectedConfigType})";
            SetupListView();
            listViewApps.KeyDown += ListViewApps_KeyDown;

            if (string.IsNullOrEmpty(OriginalContent))
                return;

            try
            {
                switch (DetectedConfigType)
                {
                    case ConfigType.Applist:
                        var apps = ApplistParser.Parse(OriginalContent);
                        PopulateListView_Applist(apps);
                        break;

                    case ConfigType.KernelConfig:
                        var tlvList = KernelConfigParser.Parse(OriginalContent);
                        PopulateListView_TLV(tlvList);
                        break;

                    default:
                        MessageBox.Show($"{DetectedConfigType} türü desteklenmiyor.",
                                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Analiz sırasında hata oluştu:\n{ex.Message}",
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Form kapatılırken değişiklik varsa hex'i yeniden oluştur ---
        private void FormAnalyze_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!dataModified)
                return;

            StringBuilder newHex = new();

            foreach (ListViewItem item in listViewApps.Items)
            {
                string tag = item.SubItems[0].Text;
                int len = item.SubItems[2].Text.Length / 2;
                string length = len.ToString("X2");
                string value = item.SubItems[2].Text;

                newHex.Append(tag);
                newHex.Append(length);
                newHex.Append(value);
            }

            UpdatedHexData = newHex.ToString();
            this.DialogResult = DialogResult.OK; // Ana forma değişiklik sinyali
        }

        // --- Ctrl + C ile hücre kopyalama desteği ---
        private void ListViewApps_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C && listViewApps.FocusedItem != null)
            {
                int subItemIndex = GetFocusedSubItemIndex();
                if (subItemIndex >= 0)
                {
                    Clipboard.SetText(listViewApps.FocusedItem.SubItems[subItemIndex].Text);
                    e.Handled = true;
                }
            }
        }

        // --- Hangi alt hücrenin odakta olduğunu bulur ---
        private int GetFocusedSubItemIndex()
        {
            Point mousePos = listViewApps.PointToClient(Cursor.Position);
            ListViewHitTestInfo hit = listViewApps.HitTest(mousePos);
            return hit.Item != null && hit.SubItem != null
                ? hit.Item.SubItems.IndexOf(hit.SubItem)
                : -1;
        }

        // --- ListView temel görünüm ayarları ---
        private void SetupListView()
        {
            listViewApps.View = View.Details;
            listViewApps.FullRowSelect = true;
            listViewApps.GridLines = true;

            listViewApps.Columns.Clear();
            listViewApps.Columns.Add("No", 50);
            listViewApps.Columns.Add("AID", 200);
            listViewApps.Columns.Add("AID Length", 100);
            listViewApps.Columns.Add("MO Flag", 80);
            listViewApps.Columns.Add("Kernel Count", 100);
            listViewApps.Columns.Add("Kernel IDs", 300);
            listViewApps.Columns.Add("Application", 250);
        }

        // --- Applist içeriğini ListView'e yükler ---
        private void PopulateListView_Applist(List<ApplicationEntry> apps)
        {
            listViewApps.Clear();
            listViewApps.Columns.Add("AID Length", 90);
            listViewApps.Columns.Add("AID", 180);
            listViewApps.Columns.Add("Kernel Count", 100);
            listViewApps.Columns.Add("Application Name", 250);

            var aidMap = MappingLoader.Load("applist");

            foreach (var app in apps)
            {
                string appName = aidMap.TryGetValue(app.AID, out string? mappedName)
                    ? mappedName
                    : "(Unknown Application)";

                ListViewItem item = new ListViewItem(app.AIDLength.ToString());
                item.SubItems.Add(app.AID);
                item.SubItems.Add(app.KernelIDList.Count.ToString());
                item.SubItems.Add(appName);

                listViewApps.Items.Add(item);
            }

            foreach (ColumnHeader col in listViewApps.Columns)
                col.Width = -2;
        }

        // --- TLV verilerini ListView'e yükler ---
        private void PopulateListView_TLV(List<TlvEntry> list)
        {
            listViewApps.Clear();
            listViewApps.Columns.Add("Tag", 120);
            listViewApps.Columns.Add("Length", 80);
            listViewApps.Columns.Add("Value", 300);
            listViewApps.Columns.Add("Tag Name", 250);

            var tagMap = MappingLoader.Load("kernelconfig");

            foreach (var tlv in list)
            {
                string tagName = tagMap.TryGetValue(tlv.Tag, out string? mappedName)
                    ? mappedName
                    : "UNDEFINED TAG";

                var item = new ListViewItem(tlv.Tag);
                item.SubItems.Add(tlv.Length);
                item.SubItems.Add(tlv.Value);
                item.SubItems.Add(tagName);

                listViewApps.Items.Add(item);
            }

            foreach (ColumnHeader col in listViewApps.Columns)
                col.Width = -2;
        }

        // --- Hücre düzenleme (çift tıklama) ---
        private void listViewApps_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var hit = listViewApps.HitTest(e.Location);
            if (hit.Item == null || hit.SubItem == null)
                return;

            int colIndex = hit.Item.SubItems.IndexOf(hit.SubItem);
            if (colIndex < 0) return;

            Rectangle bounds = hit.SubItem.Bounds;

            // Tag sütunu için genişliği sınırlı tut
            if (colIndex == 0)
                bounds.Width = listViewApps.Columns[colIndex].Width;

            editBox = new TextBox
            {
                Bounds = bounds,
                Text = hit.SubItem.Text,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Odak kaybedildiğinde değeri uygula
            editBox.Leave += (s, ev) =>
            {
                hit.SubItem.Text = editBox.Text;
                dataModified = true;
                listViewApps.Controls.Remove(editBox);
                editBox.Dispose();
            };

            // Enter / Escape davranışları
            editBox.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter || ev.KeyCode == Keys.Escape)
                {
                    if (ev.KeyCode == Keys.Enter)
                    {
                        hit.SubItem.Text = editBox.Text;
                        dataModified = true;
                    }
                    listViewApps.Controls.Remove(editBox);
                    editBox.Dispose();
                }
            };

            listViewApps.Controls.Add(editBox);
            editBox.BringToFront();
            editBox.Focus();
        }
    }
}
