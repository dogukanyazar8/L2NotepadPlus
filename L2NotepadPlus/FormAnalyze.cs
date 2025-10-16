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

namespace L2NotepadPlus
{
    public partial class FormAnalyze : Form
    {
        public string OriginalContent { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public FormAnalyze()
        {
            InitializeComponent();
        }
        private void FormAnalyze_Load(object sender, EventArgs e)
        {
            this.Text = $"{FileName} - L2Notepad Plus Analyze";

            SetupListView();

            listViewApps.KeyDown += ListViewApps_KeyDown;

            if (!string.IsNullOrEmpty(OriginalContent))
            {
                try
                {
                    var apps = ParseApplist(OriginalContent);
                    PopulateListView(apps);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"APPLIST parse edilirken hata oluştu:\n{ex.Message}",
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private List<ApplicationEntry> ParseApplist(string hexData)
        {
            List<ApplicationEntry> result = new();
            int i = 0;
            int appIndex = 1;

            while (i < hexData.Length)
            {
                // Güvenlik: sınır kontrolü
                if (i + 2 > hexData.Length)
                    break;

                int aidLen = Convert.ToInt32(hexData.Substring(i, 2), 16);
                i += 2;

                if (i + aidLen * 2 > hexData.Length)
                    throw new Exception("AID uzunluğu geçersiz.");

                string aid = hexData.Substring(i, aidLen * 2);
                i += 32; // AID alanı 16 byte (32 hex karakter)
                if (i + 2 > hexData.Length)
                    throw new Exception("Eksik Multiple Occurrence Flag.");

                int moFlag = Convert.ToInt32(hexData.Substring(i, 2), 16);
                i += 2;

                if (i + 2 > hexData.Length)
                    throw new Exception("Eksik KernelIDCount alanı.");

                int kernelCount = Convert.ToInt32(hexData.Substring(i, 2), 16);
                i += 2;

                List<string> kernelIds = new();

                for (int k = 0; k < kernelCount; k++)
                {
                    if (i + 2 > hexData.Length)
                        throw new Exception("Eksik KernelID uzunluğu.");

                    int kidLen = Convert.ToInt32(hexData.Substring(i, 2), 16);
                    i += 2;

                    if (i + kidLen * 2 > hexData.Length)
                        throw new Exception("Eksik KernelID verisi.");

                    string kid = hexData.Substring(i, kidLen * 2);
                    i += kidLen * 2;
                    kernelIds.Add(kid);
                }

                result.Add(new ApplicationEntry
                {
                    Index = appIndex++,
                    AID = aid,
                    AIDLength = aidLen,
                    MultipleOccurrenceFlag = moFlag,
                    KernelIDCount = kernelCount,
                    KernelIDList = kernelIds
                });
            }

            return result;
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
    }

    public class ApplicationEntry
    {
        public int Index { get; set; }
        public string AID { get; set; } = "";
        public int AIDLength { get; set; }
        public int MultipleOccurrenceFlag { get; set; }
        public int KernelIDCount { get; set; }
        public List<string> KernelIDList { get; set; } = new();
    }
}