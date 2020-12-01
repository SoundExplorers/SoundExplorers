using System.Windows.Forms;

namespace SoundExplorers.View {
    partial class MainView {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            this.MainMenu = new System.Windows.Forms.MainMenu();
            this.FileMenu = new System.Windows.Forms.MenuItem();
            this.FileNewMenuItem = new System.Windows.Forms.MenuItem();
            this.FileOpenMenuItem = new System.Windows.Forms.MenuItem();
            this.FileRefreshMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.FileExitMenuItem = new System.Windows.Forms.MenuItem();
            this.EditMenu = new System.Windows.Forms.MenuItem();
            this.EditUndoMenuItem = new System.Windows.Forms.MenuItem();
            this.EditRedoMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.EditCutMenuItem = new System.Windows.Forms.MenuItem();
            this.EditCopyMenuItem = new System.Windows.Forms.MenuItem();
            this.EditPasteMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.EditSelectAllMenuItem = new System.Windows.Forms.MenuItem();
            this.ViewMenu = new System.Windows.Forms.MenuItem();
            this.ViewToolBarMenuItem = new System.Windows.Forms.MenuItem();
            this.ViewStatusBarMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolsMenu = new System.Windows.Forms.MenuItem();
            this.ToolsPlayAudioMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolsPlayVideoMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolsShowNewsletterMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolsEditAudioFileTagsMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolsOptionsMenuItem = new System.Windows.Forms.MenuItem();
            this.WindowsMenu = new System.Windows.Forms.MenuItem();
            this.WindowsCascadeMenuItem = new System.Windows.Forms.MenuItem();
            this.WindowsTileSideBySideMenuItem = new System.Windows.Forms.MenuItem();
            this.WindowsTileStackedMenuItem = new System.Windows.Forms.MenuItem();
            this.WindowsCloseAllMenuItem = new System.Windows.Forms.MenuItem();
            this.WindowsArrangeIconsMenuItem = new System.Windows.Forms.MenuItem();
            this.HelpMenu = new System.Windows.Forms.MenuItem();
            this.HelpContentsMenuItem = new System.Windows.Forms.MenuItem();
            this.HelpIndexMenuItem = new System.Windows.Forms.MenuItem();
            this.HelpSearchMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.HelpAboutMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.NewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.OpenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.RefreshToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.CopyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.CutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.PasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.PlayAudioToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.PlayVideoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ShowNewsletterToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.HelpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStrip.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.FileMenu,
            this.EditMenu,
            this.ViewMenu,
            this.ToolsMenu,
            this.WindowsMenu,
            this.HelpMenu});
            this.MainMenu.Name = "MainMenu";
            // 
            // FileMenu
            // 
            this.FileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.FileNewMenuItem,
            this.FileOpenMenuItem,
            this.FileRefreshMenuItem,
            //this.ToolStripSeparator3,
            this.FileExitMenuItem});
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Text = "&File";
            // 
            // FileNewMenuItem
            // 
            this.FileNewMenuItem.Name = "FileNewMenuItem";
            this.FileNewMenuItem.Shortcut = Shortcut.CtrlN;
            this.FileNewMenuItem.Text = "&New Table Editor";
            this.FileNewMenuItem.Click += new System.EventHandler(this.FileNewMenuItem_Click);
            // 
            // FileOpenMenuItem
            // 
            this.FileOpenMenuItem.Name = "FileOpenMenuItem";
            this.FileOpenMenuItem.Shortcut = Shortcut.CtrlO;
            this.FileOpenMenuItem.Text = "&Open Table";
            this.FileOpenMenuItem.Click += new System.EventHandler(this.FileOpenMenuItem_Click);
            // 
            // FileRefreshMenuItem
            // 
            this.FileRefreshMenuItem.Name = "FileRefreshMenuItem";
            this.FileRefreshMenuItem.Shortcut = Shortcut.F5;
            this.FileRefreshMenuItem.Text = "&Refresh Table";
            this.FileRefreshMenuItem.Click += new System.EventHandler(this.FileRefreshMenuItem_Click);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(204, 6);
            // 
            // FileExitMenuItem
            // 
            this.FileExitMenuItem.Name = "FileExitMenuItem";
            this.FileExitMenuItem.Text = "E&xit";
            this.FileExitMenuItem.Click += new System.EventHandler(this.FileExitMenuItem_Click);
            // 
            // EditMenu
            // 
            this.EditMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.EditUndoMenuItem,
            this.EditRedoMenuItem,
            //this.ToolStripSeparator6,
            this.EditCutMenuItem,
            this.EditCopyMenuItem,
            this.EditPasteMenuItem,
            //this.ToolStripSeparator7,
            this.EditSelectAllMenuItem});
            this.EditMenu.Name = "EditMenu";
            this.EditMenu.Text = "&Edit";
            // 
            // EditUndoMenuItem
            // 
            this.EditUndoMenuItem.Name = "EditUndoMenuItem";
            this.EditUndoMenuItem.Shortcut = Shortcut.CtrlZ;
            this.EditUndoMenuItem.Text = "&Undo";
            // 
            // EditRedoMenuItem
            // 
            this.EditRedoMenuItem.Name = "EditRedoMenuItem";
            this.EditRedoMenuItem.Shortcut = Shortcut.CtrlY;
            this.EditRedoMenuItem.Text = "&Redo";
            // 
            // ToolStripSeparator6
            // 
            this.ToolStripSeparator6.Name = "ToolStripSeparator6";
            this.ToolStripSeparator6.Size = new System.Drawing.Size(161, 6);
            // 
            // EditCutMenuItem
            // 
            this.EditCutMenuItem.Name = "EditCutMenuItem";
            this.EditCutMenuItem.Shortcut = Shortcut.CtrlX;
            this.EditCutMenuItem.Text = "Cu&t";
            this.EditCutMenuItem.Click += new System.EventHandler(this.EditCutMenuItem_Click);
            // 
            // EditCopyMenuItem
            // 
            this.EditCopyMenuItem.Name = "EditCopyMenuItem";
            this.EditCopyMenuItem.Shortcut = Shortcut.CtrlC;
            this.EditCopyMenuItem.Text = "&Copy";
            this.EditCopyMenuItem.Click += new System.EventHandler(this.EditCopyMenuItem_Click);
            // 
            // EditPasteMenuItem
            // 
            this.EditPasteMenuItem.Name = "EditPasteMenuItem";
            this.EditPasteMenuItem.Shortcut = Shortcut.CtrlV;
            this.EditPasteMenuItem.Text = "&Paste";
            this.EditPasteMenuItem.Click += new System.EventHandler(this.EditPasteMenuItem_Click);
            // 
            // ToolStripSeparator7
            // 
            this.ToolStripSeparator7.Name = "ToolStripSeparator7";
            this.ToolStripSeparator7.Size = new System.Drawing.Size(161, 6);
            // 
            // EditSelectAllMenuItem
            // 
            this.EditSelectAllMenuItem.Name = "EditSelectAllMenuItem";
            this.EditSelectAllMenuItem.Shortcut = Shortcut.CtrlA;
            this.EditSelectAllMenuItem.Text = "Select &All";
            // 
            // ViewMenu
            // 
            this.ViewMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ViewToolBarMenuItem,
            this.ViewStatusBarMenuItem});
            this.ViewMenu.Name = "ViewMenu";
            this.ViewMenu.Text = "&View";
            // 
            // ViewToolBarMenuItem
            // 
            this.ViewToolBarMenuItem.Checked = true;
            this.ViewToolBarMenuItem.Name = "ViewToolBarMenuItem";
            this.ViewToolBarMenuItem.Text = "&Toolbar";
            this.ViewToolBarMenuItem.Click += new System.EventHandler(this.ViewToolBarMenuItem_Click);
            // 
            // ViewStatusBarMenuItem
            // 
            this.ViewStatusBarMenuItem.Checked = true;
            this.ViewStatusBarMenuItem.Name = "ViewStatusBarMenuItem";
            this.ViewStatusBarMenuItem.Text = "&Status Bar";
            this.ViewStatusBarMenuItem.Click += new System.EventHandler(this.ViewStatusBarMenuItem_Click);
            // 
            // ToolsMenu
            // 
            this.ToolsMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ToolsPlayAudioMenuItem,
            this.ToolsPlayVideoMenuItem,
            this.ToolsShowNewsletterMenuItem,
            //this.ToolStripSeparator2,
            this.ToolsEditAudioFileTagsMenuItem,
            this.ToolsOptionsMenuItem});
            this.ToolsMenu.Name = "ToolsMenu";
            this.ToolsMenu.Text = "&Tools";
            // 
            // ToolsPlayAudioMenuItem
            // 
            this.ToolsPlayAudioMenuItem.Name = "ToolsPlayAudioMenuItem";
            this.ToolsPlayAudioMenuItem.Text = "Play &Audio";
            this.ToolsPlayAudioMenuItem.Click += new System.EventHandler(this.ToolsPlayAudioMenuItem_Click);
            // 
            // ToolsPlayVideoMenuItem
            // 
            this.ToolsPlayVideoMenuItem.Name = "ToolsPlayVideoMenuItem";
            this.ToolsPlayVideoMenuItem.Text = "Play &Video";
            this.ToolsPlayVideoMenuItem.Click += new System.EventHandler(this.ToolsPlayVideoMenuItem_Click);
            // 
            // ToolsShowNewsletterMenuItem
            // 
            this.ToolsShowNewsletterMenuItem.Name = "ToolsShowNewsletterMenuItem";
            this.ToolsShowNewsletterMenuItem.Text = "Show &Newsletter";
            this.ToolsShowNewsletterMenuItem.Click += new System.EventHandler(this.ToolsShowNewsletterMenuItem_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(184, 6);
            // 
            // ToolsEditAudioFileTagsMenuItem
            // 
            this.ToolsEditAudioFileTagsMenuItem.Name = "ToolsEditAudioFileTagsMenuItem";
            this.ToolsEditAudioFileTagsMenuItem.Text = "Edit Audio File &Tags...";
            this.ToolsEditAudioFileTagsMenuItem.Click += new System.EventHandler(this.ToolsEditAudioFileTagsMenuItem_Click);
            // 
            // ToolsOptionsMenuItem
            // 
            this.ToolsOptionsMenuItem.Name = "ToolsOptionsMenuItem";
            this.ToolsOptionsMenuItem.Text = "&Options";
            // 
            // WindowsMenu
            // 
            this.WindowsMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.WindowsCascadeMenuItem,
            this.WindowsTileSideBySideMenuItem,
            this.WindowsTileStackedMenuItem,
            this.WindowsCloseAllMenuItem,
            this.WindowsArrangeIconsMenuItem});
            this.WindowsMenu.Name = "WindowsMenu";
            this.WindowsMenu.Text = "&Windows";
            // 
            // WindowsCascadeMenuItem
            // 
            this.WindowsCascadeMenuItem.Name = "WindowsCascadeMenuItem";
            this.WindowsCascadeMenuItem.Text = "&Cascade";
            this.WindowsCascadeMenuItem.Click += new System.EventHandler(this.WindowsCascadeMenuItem_Click);
            // 
            // WindowsTileSideBySideMenuItem
            // 
            this.WindowsTileSideBySideMenuItem.Name = "WindowsTileSideBySideMenuItem";
            this.WindowsTileSideBySideMenuItem.Text = "Tile &Side By Side";
            this.WindowsTileSideBySideMenuItem.Click += new System.EventHandler(this.WindowsTileSideBySideMenuItem_Click);
            // 
            // WindowsTileStackedMenuItem
            // 
            this.WindowsTileStackedMenuItem.Name = "WindowsTileStackedMenuItem";
            this.WindowsTileStackedMenuItem.Text = "Tile S&tacked";
            this.WindowsTileStackedMenuItem.Click += new System.EventHandler(this.WindowsTileStackedMenuItem_Click);
            // 
            // WindowsCloseAllMenuItem
            // 
            this.WindowsCloseAllMenuItem.Name = "WindowsCloseAllMenuItem";
            this.WindowsCloseAllMenuItem.Text = "C&lose All";
            this.WindowsCloseAllMenuItem.Click += new System.EventHandler(this.WindowsCloseAllMenuItem_Click);
            // 
            // WindowsArrangeIconsMenuItem
            // 
            this.WindowsArrangeIconsMenuItem.Name = "WindowsArrangeIconsMenuItem";
            this.WindowsArrangeIconsMenuItem.Text = "&Arrange Icons";
            this.WindowsArrangeIconsMenuItem.Click += new System.EventHandler(this.WindowsArrangeIconsMenuItem_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.HelpContentsMenuItem,
            this.HelpIndexMenuItem,
            this.HelpSearchMenuItem,
            //this.ToolStripSeparator8,
            this.HelpAboutMenuItem});
            this.HelpMenu.Name = "HelpMenu";
            this.HelpMenu.Text = "&Help";
            // 
            // HelpContentsMenuItem
            // 
            this.HelpContentsMenuItem.Name = "HelpContentsMenuItem";
            this.HelpContentsMenuItem.Shortcut = Shortcut.F1;
            this.HelpContentsMenuItem.Text = "&Contents";
            // 
            // HelpIndexMenuItem
            // 
            this.HelpIndexMenuItem.Name = "HelpIndexMenuItem";
            this.HelpIndexMenuItem.Text = "&Index";
            // 
            // HelpSearchMenuItem
            // 
            this.HelpSearchMenuItem.Name = "HelpSearchMenuItem";
            this.HelpSearchMenuItem.Text = "&Search";
            // 
            // ToolStripSeparator8
            // 
            this.ToolStripSeparator8.Name = "ToolStripSeparator8";
            this.ToolStripSeparator8.Size = new System.Drawing.Size(165, 6);
            // 
            // HelpAboutMenuItem
            // 
            this.HelpAboutMenuItem.Name = "HelpAboutMenuItem";
            this.HelpAboutMenuItem.Text = "&About ... ...";
            this.HelpAboutMenuItem.Click += new System.EventHandler(this.HelpAboutMenuItem_Click);
            // 
            // ToolStrip
            // 
            this.ToolStrip.AutoSize = false;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewToolStripButton,
            this.OpenToolStripButton,
            this.RefreshToolStripButton,
            this.CopyToolStripButton,
            this.CutToolStripButton,
            this.PasteToolStripButton,
            this.PlayAudioToolStripButton,
            this.PlayVideoToolStripButton,
            this.ShowNewsletterToolStripButton,
            this.HelpToolStripButton});
            this.ToolStrip.Location = new System.Drawing.Point(0, 24);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(843, 41);
            this.ToolStrip.TabIndex = 1;
            this.ToolStrip.Text = "ToolStrip";
            // 
            // NewToolStripButton
            // 
            this.NewToolStripButton.AutoSize = false;
            this.NewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NewToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.ActionsDocumentNew32x32;
            this.NewToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.NewToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.NewToolStripButton.Name = "NewToolStripButton";
            this.NewToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.NewToolStripButton.Text = "New Table Editor";
            this.NewToolStripButton.Click += new System.EventHandler(this.FileNewMenuItem_Click);
            // 
            // OpenToolStripButton
            // 
            this.OpenToolStripButton.AutoSize = false;
            this.OpenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.ActionsDocumentOpen32x32;
            this.OpenToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.OpenToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.OpenToolStripButton.Name = "OpenToolStripButton";
            this.OpenToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.OpenToolStripButton.Text = "Open Table";
            this.OpenToolStripButton.Click += new System.EventHandler(this.FileOpenMenuItem_Click);
            // 
            // RefreshToolStripButton
            // 
            this.RefreshToolStripButton.AutoSize = false;
            this.RefreshToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RefreshToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.ActionsViewRefresh32x32;
            this.RefreshToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.RefreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RefreshToolStripButton.Name = "RefreshToolStripButton";
            this.RefreshToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.RefreshToolStripButton.Text = "Refresh Table";
            this.RefreshToolStripButton.Click += new System.EventHandler(this.FileRefreshMenuItem_Click);
            // 
            // CopyToolStripButton
            // 
            this.CopyToolStripButton.AutoSize = false;
            this.CopyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CopyToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.ActionsEditCopy32x32;
            this.CopyToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.CopyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CopyToolStripButton.Name = "CopyToolStripButton";
            this.CopyToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.CopyToolStripButton.Text = "Copy";
            this.CopyToolStripButton.Click += new System.EventHandler(this.EditCopyMenuItem_Click);
            // 
            // CutToolStripButton
            // 
            this.CutToolStripButton.AutoSize = false;
            this.CutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CutToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.ActionsEditCut32x32;
            this.CutToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.CutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CutToolStripButton.Name = "CutToolStripButton";
            this.CutToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.CutToolStripButton.Text = "Cut";
            this.CutToolStripButton.Click += new System.EventHandler(this.EditCutMenuItem_Click);
            // 
            // PasteToolStripButton
            // 
            this.PasteToolStripButton.AutoSize = false;
            this.PasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PasteToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.ActionsEditPaste32x32;
            this.PasteToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.PasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PasteToolStripButton.Name = "PasteToolStripButton";
            this.PasteToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.PasteToolStripButton.Text = "Paste";
            this.PasteToolStripButton.Click += new System.EventHandler(this.EditPasteMenuItem_Click);
            // 
            // PlayAudioToolStripButton
            // 
            this.PlayAudioToolStripButton.AutoSize = false;
            this.PlayAudioToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PlayAudioToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.MusicLibrary32x32;
            this.PlayAudioToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.PlayAudioToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PlayAudioToolStripButton.Name = "PlayAudioToolStripButton";
            this.PlayAudioToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.PlayAudioToolStripButton.Text = "Play Audio";
            this.PlayAudioToolStripButton.Click += new System.EventHandler(this.ToolsPlayAudioMenuItem_Click);
            // 
            // PlayVideoToolStripButton
            // 
            this.PlayVideoToolStripButton.AutoSize = false;
            this.PlayVideoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PlayVideoToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.DevicesVideoDisplay32x32;
            this.PlayVideoToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.PlayVideoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PlayVideoToolStripButton.Name = "PlayVideoToolStripButton";
            this.PlayVideoToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.PlayVideoToolStripButton.Text = "Play Video";
            this.PlayVideoToolStripButton.Click += new System.EventHandler(this.ToolsPlayVideoMenuItem_Click);
            // 
            // ShowNewsletterToolStripButton
            // 
            this.ShowNewsletterToolStripButton.AutoSize = false;
            this.ShowNewsletterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowNewsletterToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.News32x32;
            this.ShowNewsletterToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ShowNewsletterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowNewsletterToolStripButton.Name = "ShowNewsletterToolStripButton";
            this.ShowNewsletterToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.ShowNewsletterToolStripButton.Text = "Show Newsletter";
            this.ShowNewsletterToolStripButton.Click += new System.EventHandler(this.ToolsShowNewsletterMenuItem_Click);
            // 
            // HelpToolStripButton
            // 
            this.HelpToolStripButton.AutoSize = false;
            this.HelpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.HelpToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.HelpAndSupport32x32;
            this.HelpToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.HelpToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.HelpToolStripButton.Name = "HelpToolStripButton";
            this.HelpToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.HelpToolStripButton.Text = "Help";
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 536);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.StatusStrip.Size = new System.Drawing.Size(843, 22);
            this.StatusStrip.TabIndex = 2;
            this.StatusStrip.Text = "StatusStrip";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(792, 17);
            this.StatusLabel.Spring = true;
            this.StatusLabel.Text = "StatusLabel";
            // 
            // MdiParentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 558);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.ToolStrip);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Menu = this.MainMenu;
            this.Name = "MainView";
            this.Text = "Sound Explorers Audio Archive";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainView_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.MainView_VisibleChanged);
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.MainMenu MainMenu;
        private System.Windows.Forms.ToolStrip ToolStrip;
        public System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator8;
        private System.Windows.Forms.MenuItem FileMenu;
        private System.Windows.Forms.MenuItem FileNewMenuItem;
        private System.Windows.Forms.MenuItem FileOpenMenuItem;
        private System.Windows.Forms.MenuItem FileRefreshMenuItem;
        private System.Windows.Forms.MenuItem FileExitMenuItem;
        private System.Windows.Forms.MenuItem EditMenu;
        private System.Windows.Forms.MenuItem EditUndoMenuItem;
        private System.Windows.Forms.MenuItem EditRedoMenuItem;
        private System.Windows.Forms.MenuItem EditCutMenuItem;
        private System.Windows.Forms.MenuItem EditCopyMenuItem;
        private System.Windows.Forms.MenuItem EditPasteMenuItem;
        private System.Windows.Forms.MenuItem EditSelectAllMenuItem;
        private System.Windows.Forms.MenuItem ViewMenu;
        private System.Windows.Forms.MenuItem ViewToolBarMenuItem;
        private System.Windows.Forms.MenuItem ViewStatusBarMenuItem;
        private System.Windows.Forms.MenuItem ToolsMenu;
        private System.Windows.Forms.MenuItem ToolsPlayAudioMenuItem;
        private System.Windows.Forms.MenuItem ToolsPlayVideoMenuItem;
        private System.Windows.Forms.MenuItem ToolsShowNewsletterMenuItem;
        private System.Windows.Forms.MenuItem ToolsEditAudioFileTagsMenuItem;
        private System.Windows.Forms.MenuItem ToolsOptionsMenuItem;
        private System.Windows.Forms.MenuItem WindowsMenu;
        private System.Windows.Forms.MenuItem WindowsCascadeMenuItem;
        private System.Windows.Forms.MenuItem WindowsTileSideBySideMenuItem;
        private System.Windows.Forms.MenuItem WindowsTileStackedMenuItem;
        private System.Windows.Forms.MenuItem WindowsCloseAllMenuItem;
        private System.Windows.Forms.MenuItem WindowsArrangeIconsMenuItem;
        private System.Windows.Forms.MenuItem HelpMenu;
        private System.Windows.Forms.MenuItem HelpContentsMenuItem;
        private System.Windows.Forms.MenuItem HelpIndexMenuItem;
        private System.Windows.Forms.MenuItem HelpSearchMenuItem;
        private System.Windows.Forms.MenuItem HelpAboutMenuItem;
        private System.Windows.Forms.ToolStripButton NewToolStripButton;
        private System.Windows.Forms.ToolStripButton OpenToolStripButton;
        private System.Windows.Forms.ToolStripButton HelpToolStripButton;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.ToolStripButton RefreshToolStripButton;
        private System.Windows.Forms.ToolStripButton CopyToolStripButton;
        private System.Windows.Forms.ToolStripButton CutToolStripButton;
        private System.Windows.Forms.ToolStripButton PasteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
        private System.Windows.Forms.ToolStripButton PlayAudioToolStripButton;
        private System.Windows.Forms.ToolStripButton PlayVideoToolStripButton;
        private System.Windows.Forms.ToolStripButton ShowNewsletterToolStripButton;
        public System.Windows.Forms.ToolStripStatusLabel StatusLabel;
    }
}
