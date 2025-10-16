namespace L2NotepadPlus
{
    partial class FormAnalyze
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listViewApps = new ListView();
            SuspendLayout();
            // 
            // listViewApps
            // 
            listViewApps.Font = new Font("Consolas", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            listViewApps.FullRowSelect = true;
            listViewApps.GridLines = true;
            listViewApps.Location = new Point(12, 12);
            listViewApps.Name = "listViewApps";
            listViewApps.Size = new Size(1663, 774);
            listViewApps.TabIndex = 0;
            listViewApps.UseCompatibleStateImageBehavior = false;
            listViewApps.View = View.Details;
            listViewApps.MouseDoubleClick += listViewApps_MouseDoubleClick;
            // 
            // FormAnalyze
            // 
            AutoScaleDimensions = new SizeF(12F, 26F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1687, 1338);
            Controls.Add(listViewApps);
            Font = new Font("Consolas", 11F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FormAnalyze";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "L2Notepad Plus - Analyzer";
            FormClosing += FormAnalyze_FormClosing;
            Load += FormAnalyze_Load;
            ResumeLayout(false);
        }

        #endregion

        private ListView listViewApps;
    }
}