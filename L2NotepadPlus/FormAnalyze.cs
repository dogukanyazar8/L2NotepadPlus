using L2NotepadPlus.Analyze;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static L2NotepadPlus.Analyze.ConfigTypeDetector;

namespace L2NotepadPlus
{
    public partial class FormAnalyze : Form
    {
        public string OriginalContent { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public ConfigTypeDetector.ConfigType DetectedConfigType { get; set; } = ConfigTypeDetector.ConfigType.Unknown;

        public FormAnalyze()
        {
            InitializeComponent();
        }
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
                        PopulateListView(apps);
                        break;

                    case ConfigType.KernelConfig:
                        var tlvList = KernelConfigParser.Parse(OriginalContent);
                        PopulateListView_TLV(tlvList);
                        break;

                    default:
                        MessageBox.Show($"{DetectedConfigType} türü henüz desteklenmiyor.",
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

        private void ListViewApps_KeyDown(object? sender, KeyEventArgs e)
        {
            // Ctrl + C tuş kombinasyonunu yakala
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (listViewApps.FocusedItem != null)
                {
                    int subItemIndex = GetFocusedSubItemIndex();
                    if (subItemIndex >= 0)
                    {
                        string textToCopy = listViewApps.FocusedItem.SubItems[subItemIndex].Text;
                        Clipboard.SetText(textToCopy);
                        e.Handled = true;
                    }
                }
            }
        }
        private int GetFocusedSubItemIndex()
        {
            Point mousePos = listViewApps.PointToClient(Cursor.Position);
            ListViewHitTestInfo hit = listViewApps.HitTest(mousePos);
            return hit.Item != null && hit.SubItem != null
                ? hit.Item.SubItems.IndexOf(hit.SubItem)
                : -1;
        }
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
        private void PopulateListView(List<ApplicationEntry> apps)
        {
            listViewApps.BeginUpdate();
            listViewApps.Items.Clear();

            var aidMap = AidMappingLoader.Load();

            foreach (var app in apps)
            {
                aidMap.TryGetValue(app.AID, out string? appName);

                ListViewItem item = new(app.Index.ToString());
                item.SubItems.Add(app.AID);
                item.SubItems.Add(app.AIDLength.ToString());
                item.SubItems.Add(app.MultipleOccurrenceFlag.ToString());
                item.SubItems.Add(app.KernelIDCount.ToString());
                item.SubItems.Add(string.Join(", ", app.KernelIDList));
                item.SubItems.Add(appName ?? "");
                listViewApps.Items.Add(item);
            }

            listViewApps.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewApps.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listViewApps.EndUpdate();
        }

        private void PopulateListView_TLV(List<TlvEntry> list)
        {
            listViewApps.Clear(); // tüm kolon ve satırları temizle

            // Yeni başlıklar ekle
            listViewApps.Columns.Add("Tag", 120);
            listViewApps.Columns.Add("Length", 80);
            listViewApps.Columns.Add("Value", 300);
            listViewApps.Columns.Add("Tag Name", 250);

            // TLV verilerini listeye ekle
            foreach (var tlv in list)
            {
                var item = new ListViewItem(tlv.Tag);
                item.SubItems.Add(tlv.Length);
                item.SubItems.Add(tlv.Value);
                item.SubItems.Add(tlv.Definition);
                listViewApps.Items.Add(item);
            }

            // Kolon genişliklerini içeriğe göre ayarla
            foreach (ColumnHeader col in listViewApps.Columns)
                col.Width = -2;

            listViewApps.View = View.Details;
            listViewApps.FullRowSelect = true;
        }
    }
}