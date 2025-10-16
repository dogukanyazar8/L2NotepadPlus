namespace L2NotepadPlus
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainNotepad = new RichTextBox();
            mainMenuStrip = new MenuStrip();
            menuFile = new ToolStripMenuItem();
            menuFileNew = new ToolStripMenuItem();
            menuFileOpen = new ToolStripMenuItem();
            menuFileSave = new ToolStripMenuItem();
            menuEdit = new ToolStripMenuItem();
            menuFormat = new ToolStripMenuItem();
            menuView = new ToolStripMenuItem();
            menuL2Config = new ToolStripMenuItem();
            menuHelp = new ToolStripMenuItem();
            radioL2Config = new RadioButton();
            radioTextFile = new RadioButton();
            statusStrip1 = new StatusStrip();
            menuL2ConfigAnalyze = new ToolStripMenuItem();
            mainMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mainNotepad
            // 
            mainNotepad.Dock = DockStyle.Fill;
            mainNotepad.Location = new Point(0, 37);
            mainNotepad.Name = "mainNotepad";
            mainNotepad.Size = new Size(2310, 598);
            mainNotepad.TabIndex = 0;
            mainNotepad.Text = "";
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Font = new Font("Trebuchet MS", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            mainMenuStrip.ImageScalingSize = new Size(24, 24);
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { menuFile, menuEdit, menuFormat, menuView, menuL2Config, menuHelp });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(2310, 37);
            mainMenuStrip.TabIndex = 1;
            mainMenuStrip.Text = "menuStrip1";
            // 
            // menuFile
            // 
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuFileNew, menuFileOpen, menuFileSave });
            menuFile.Name = "menuFile";
            menuFile.Size = new Size(71, 33);
            menuFile.Text = "File";
            // 
            // menuFileNew
            // 
            menuFileNew.Name = "menuFileNew";
            menuFileNew.Size = new Size(270, 38);
            menuFileNew.Text = "New";
            menuFileNew.Click += menuFileNew_Click;
            // 
            // menuFileOpen
            // 
            menuFileOpen.Name = "menuFileOpen";
            menuFileOpen.Size = new Size(270, 38);
            menuFileOpen.Text = "Open";
            menuFileOpen.Click += menuFileOpen_Click;
            // 
            // menuFileSave
            // 
            menuFileSave.Name = "menuFileSave";
            menuFileSave.Size = new Size(270, 38);
            menuFileSave.Text = "Save";
            menuFileSave.Click += menuFileSave_Click;
            // 
            // menuEdit
            // 
            menuEdit.Name = "menuEdit";
            menuEdit.Size = new Size(74, 33);
            menuEdit.Text = "Edit";
            // 
            // menuFormat
            // 
            menuFormat.Name = "menuFormat";
            menuFormat.Size = new Size(110, 33);
            menuFormat.Text = "Format";
            // 
            // menuView
            // 
            menuView.Name = "menuView";
            menuView.Size = new Size(85, 33);
            menuView.Text = "View";
            // 
            // menuL2Config
            // 
            menuL2Config.DropDownItems.AddRange(new ToolStripItem[] { menuL2ConfigAnalyze });
            menuL2Config.Name = "menuL2Config";
            menuL2Config.Size = new Size(127, 33);
            menuL2Config.Text = "L2Config";
            // 
            // menuHelp
            // 
            menuHelp.Name = "menuHelp";
            menuHelp.Size = new Size(80, 33);
            menuHelp.Text = "Help";
            // 
            // radioL2Config
            // 
            radioL2Config.AutoSize = true;
            radioL2Config.BackColor = SystemColors.Window;
            radioL2Config.Font = new Font("Consolas", 14F, FontStyle.Bold);
            radioL2Config.Location = new Point(1942, 558);
            radioL2Config.Name = "radioL2Config";
            radioL2Config.Size = new Size(175, 37);
            radioL2Config.TabIndex = 3;
            radioL2Config.TabStop = true;
            radioL2Config.Text = "L2 Config";
            radioL2Config.UseVisualStyleBackColor = false;
            // 
            // radioTextFile
            // 
            radioTextFile.AutoSize = true;
            radioTextFile.BackColor = SystemColors.Window;
            radioTextFile.Font = new Font("Consolas", 14F, FontStyle.Bold);
            radioTextFile.Location = new Point(2123, 558);
            radioTextFile.Name = "radioTextFile";
            radioTextFile.Size = new Size(175, 37);
            radioTextFile.TabIndex = 4;
            radioTextFile.TabStop = true;
            radioTextFile.Text = "Text File";
            radioTextFile.UseVisualStyleBackColor = false;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(24, 24);
            statusStrip1.Location = new Point(0, 613);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RenderMode = ToolStripRenderMode.Professional;
            statusStrip1.Size = new Size(2310, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // menuL2ConfigAnalyze
            // 
            menuL2ConfigAnalyze.Name = "menuL2ConfigAnalyze";
            menuL2ConfigAnalyze.Size = new Size(270, 38);
            menuL2ConfigAnalyze.Text = "Analyze";
            menuL2ConfigAnalyze.Click += menuL2ConfigAnalyze_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(12F, 26F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2310, 635);
            Controls.Add(radioTextFile);
            Controls.Add(radioL2Config);
            Controls.Add(statusStrip1);
            Controls.Add(mainNotepad);
            Controls.Add(mainMenuStrip);
            Font = new Font("Consolas", 11F, FontStyle.Regular, GraphicsUnit.Point, 162);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = mainMenuStrip;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "L2Notepad Plus";
            mainMenuStrip.ResumeLayout(false);
            mainMenuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox mainNotepad;
        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuEdit;
        private ToolStripMenuItem menuFormat;
        private ToolStripMenuItem menuView;
        private ToolStripMenuItem menuL2Config;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem menuFileOpen;
        private ToolStripMenuItem menuFileSave;
        private ToolStripMenuItem menuFileNew;
        private RadioButton radioL2Config;
        private RadioButton radioTextFile;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem menuL2ConfigAnalyze;
    }
}
