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
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.FileNewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileRefreshMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.EditUndoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditRedoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditCutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditPasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditSelectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewStatusBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsPlayAudioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsPlayVideoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsShowNewsletterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsEditAudioFileTagsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsOptionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsCascadeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsTileSideBySideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsTileStackedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsCloseAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsArrangeIconsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpContentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpIndexMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpSearchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpAboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.MenuStrip.SuspendLayout();
            this.ToolStrip.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.FileMenu,
            this.EditMenu,
            this.ViewMenu,
            this.ToolsMenu,
            this.WindowsMenu,
            this.HelpMenu});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.MdiWindowListItem = this.WindowsMenu;
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.MenuStrip.Size = new System.Drawing.Size(843, 24);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.Text = "MenuStrip";
            this.MenuStrip.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.MenuStrip_ItemAdded);
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.FileNewMenuItem,
            this.FileOpenMenuItem,
            this.FileRefreshMenuItem,
            this.FileExitMenuItem});
            this.FileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(37, 20);
            this.FileMenu.Text = "&File";
            // 
            // FileNewMenuItem
            // 
            this.FileNewMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.FileNewMenuItem.Name = "FileNewMenuItem";
            this.FileNewMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.FileNewMenuItem.Size = new System.Drawing.Size(207, 22);
            this.FileNewMenuItem.Text = "&New Table Editor";
            this.FileNewMenuItem.Click += new System.EventHandler(this.FileNewMenuItem_Click);
            // 
            // FileOpenMenuItem
            // 
            this.FileOpenMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.FileOpenMenuItem.Name = "FileOpenMenuItem";
            this.FileOpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.FileOpenMenuItem.Size = new System.Drawing.Size(207, 22);
            this.FileOpenMenuItem.Text = "&Open Table";
            this.FileOpenMenuItem.Click += new System.EventHandler(this.FileOpenMenuItem_Click);
            // 
            // FileRefreshMenuItem
            // 
            this.FileRefreshMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.FileRefreshMenuItem.Name = "FileRefreshMenuItem";
            this.FileRefreshMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.FileRefreshMenuItem.Size = new System.Drawing.Size(207, 22);
            this.FileRefreshMenuItem.Text = "&Refresh Table";
            this.FileRefreshMenuItem.Click += new System.EventHandler(this.FileRefreshMenuItem_Click);
            // 
            // FileExitMenuItem
            // 
            this.FileExitMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.FileExitMenuItem.Name = "FileExitMenuItem";
            this.FileExitMenuItem.Size = new System.Drawing.Size(207, 22);
            this.FileExitMenuItem.Text = "E&xit";
            this.FileExitMenuItem.Click += new System.EventHandler(this.FileExitMenuItem_Click);
            // 
            // EditMenu
            // 
            this.EditMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.EditUndoMenuItem,
            this.EditRedoMenuItem,
            this.EditCutMenuItem,
            this.EditCopyMenuItem,
            this.EditPasteMenuItem,
            this.EditSelectAllMenuItem});
            this.EditMenu.Name = "EditMenu";
            this.EditMenu.Size = new System.Drawing.Size(39, 20);
            this.EditMenu.Text = "&Edit";
            // 
            // EditUndoMenuItem
            // 
            this.EditUndoMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditUndoMenuItem.Name = "EditUndoMenuItem";
            this.EditUndoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.EditUndoMenuItem.Size = new System.Drawing.Size(164, 22);
            this.EditUndoMenuItem.Text = "&Undo";
            // 
            // EditRedoMenuItem
            // 
            this.EditRedoMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditRedoMenuItem.Name = "EditRedoMenuItem";
            this.EditRedoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.EditRedoMenuItem.Size = new System.Drawing.Size(164, 22);
            this.EditRedoMenuItem.Text = "&Redo";
            // 
            // EditCutMenuItem
            // 
            this.EditCutMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditCutMenuItem.Name = "EditCutMenuItem";
            this.EditCutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.EditCutMenuItem.Size = new System.Drawing.Size(164, 22);
            this.EditCutMenuItem.Text = "Cu&t";
            this.EditCutMenuItem.Click += new System.EventHandler(this.EditCutMenuItem_Click);
            // 
            // EditCopyMenuItem
            // 
            this.EditCopyMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditCopyMenuItem.Name = "EditCopyMenuItem";
            this.EditCopyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.EditCopyMenuItem.Size = new System.Drawing.Size(164, 22);
            this.EditCopyMenuItem.Text = "&Copy";
            this.EditCopyMenuItem.Click += new System.EventHandler(this.EditCopyMenuItem_Click);
            // 
            // EditPasteMenuItem
            // 
            this.EditPasteMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditPasteMenuItem.Name = "EditPasteMenuItem";
            this.EditPasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.EditPasteMenuItem.Size = new System.Drawing.Size(164, 22);
            this.EditPasteMenuItem.Text = "&Paste";
            this.EditPasteMenuItem.Click += new System.EventHandler(this.EditPasteMenuItem_Click);
            // 
            // EditSelectAllMenuItem
            // 
            this.EditSelectAllMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditSelectAllMenuItem.Name = "EditSelectAllMenuItem";
            this.EditSelectAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.EditSelectAllMenuItem.Size = new System.Drawing.Size(164, 22);
            this.EditSelectAllMenuItem.Text = "Select &All";
            // 
            // ViewMenu
            // 
            this.ViewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.ViewToolBarMenuItem,
            this.ViewStatusBarMenuItem});
            this.ViewMenu.Name = "ViewMenu";
            this.ViewMenu.Size = new System.Drawing.Size(44, 20);
            this.ViewMenu.Text = "&View";
            // 
            // ViewToolBarMenuItem
            // 
            this.ViewToolBarMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ViewToolBarMenuItem.Checked = true;
            this.ViewToolBarMenuItem.CheckOnClick = true;
            this.ViewToolBarMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewToolBarMenuItem.Name = "ViewToolBarMenuItem";
            this.ViewToolBarMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ViewToolBarMenuItem.Text = "&Toolbar";
            this.ViewToolBarMenuItem.Click += new System.EventHandler(this.ViewToolBarMenuItem_Click);
            // 
            // ViewStatusBarMenuItem
            // 
            this.ViewStatusBarMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ViewStatusBarMenuItem.Checked = true;
            this.ViewStatusBarMenuItem.CheckOnClick = true;
            this.ViewStatusBarMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewStatusBarMenuItem.Name = "ViewStatusBarMenuItem";
            this.ViewStatusBarMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ViewStatusBarMenuItem.Text = "&Status Bar";
            this.ViewStatusBarMenuItem.Click += new System.EventHandler(this.ViewStatusBarMenuItem_Click);
            // 
            // ToolsMenu
            // 
            this.ToolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.ToolsPlayAudioMenuItem,
            this.ToolsPlayVideoMenuItem,
            this.ToolsShowNewsletterMenuItem,
            this.ToolsEditAudioFileTagsMenuItem,
            this.ToolsOptionsMenuItem});
            this.ToolsMenu.Name = "ToolsMenu";
            this.ToolsMenu.Size = new System.Drawing.Size(48, 20);
            this.ToolsMenu.Text = "&Tools";
            // 
            // ToolsPlayAudioMenuItem
            // 
            this.ToolsPlayAudioMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ToolsPlayAudioMenuItem.Name = "ToolsPlayAudioMenuItem";
            this.ToolsPlayAudioMenuItem.Size = new System.Drawing.Size(187, 22);
            this.ToolsPlayAudioMenuItem.Text = "Play &Audio";
            this.ToolsPlayAudioMenuItem.Click += new System.EventHandler(this.ToolsPlayAudioMenuItem_Click);
            // 
            // ToolsPlayVideoMenuItem
            // 
            this.ToolsPlayVideoMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ToolsPlayVideoMenuItem.Name = "ToolsPlayVideoMenuItem";
            this.ToolsPlayVideoMenuItem.Size = new System.Drawing.Size(187, 22);
            this.ToolsPlayVideoMenuItem.Text = "Play &Video";
            this.ToolsPlayVideoMenuItem.Click += new System.EventHandler(this.ToolsPlayVideoMenuItem_Click);
            // 
            // ToolsShowNewsletterMenuItem
            // 
            this.ToolsShowNewsletterMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ToolsShowNewsletterMenuItem.Name = "ToolsShowNewsletterMenuItem";
            this.ToolsShowNewsletterMenuItem.Size = new System.Drawing.Size(187, 22);
            this.ToolsShowNewsletterMenuItem.Text = "Show &Newsletter";
            this.ToolsShowNewsletterMenuItem.Click += new System.EventHandler(this.ToolsShowNewsletterMenuItem_Click);
            // 
            // ToolsEditAudioFileTagsMenuItem
            // 
            this.ToolsEditAudioFileTagsMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ToolsEditAudioFileTagsMenuItem.Name = "ToolsEditAudioFileTagsMenuItem";
            this.ToolsEditAudioFileTagsMenuItem.Size = new System.Drawing.Size(187, 22);
            this.ToolsEditAudioFileTagsMenuItem.Text = "Edit Audio File &Tags...";
            this.ToolsEditAudioFileTagsMenuItem.Click += new System.EventHandler(this.ToolsEditAudioFileTagsMenuItem_Click);
            // 
            // ToolsOptionsMenuItem
            // 
            this.ToolsOptionsMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ToolsOptionsMenuItem.Name = "ToolsOptionsMenuItem";
            this.ToolsOptionsMenuItem.Size = new System.Drawing.Size(187, 22);
            this.ToolsOptionsMenuItem.Text = "&Options";
            // 
            // WindowsMenu
            // 
            this.WindowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.WindowsCascadeMenuItem,
            this.WindowsTileSideBySideMenuItem,
            this.WindowsTileStackedMenuItem,
            this.WindowsCloseAllMenuItem,
            this.WindowsArrangeIconsMenuItem});
            this.WindowsMenu.Name = "WindowsMenu";
            this.WindowsMenu.Size = new System.Drawing.Size(68, 20);
            this.WindowsMenu.Text = "&Windows";
            // 
            // WindowsCascadeMenuItem
            // 
            this.WindowsCascadeMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsCascadeMenuItem.Name = "WindowsCascadeMenuItem";
            this.WindowsCascadeMenuItem.Size = new System.Drawing.Size(159, 22);
            this.WindowsCascadeMenuItem.Text = "&Cascade";
            this.WindowsCascadeMenuItem.Click += new System.EventHandler(this.WindowsCascadeMenuItem_Click);
            // 
            // WindowsTileSideBySideMenuItem
            // 
            this.WindowsTileSideBySideMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsTileSideBySideMenuItem.Name = "WindowsTileSideBySideMenuItem";
            this.WindowsTileSideBySideMenuItem.Size = new System.Drawing.Size(159, 22);
            this.WindowsTileSideBySideMenuItem.Text = "Tile &Side By Side";
            this.WindowsTileSideBySideMenuItem.Click += new System.EventHandler(this.WindowsTileSideBySideMenuItem_Click);
            // 
            // WindowsTileStackedMenuItem
            // 
            this.WindowsTileStackedMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsTileStackedMenuItem.Name = "WindowsTileStackedMenuItem";
            this.WindowsTileStackedMenuItem.Size = new System.Drawing.Size(159, 22);
            this.WindowsTileStackedMenuItem.Text = "Tile S&tacked";
            this.WindowsTileStackedMenuItem.Click += new System.EventHandler(this.WindowsTileStackedMenuItem_Click);
            // 
            // WindowsCloseAllMenuItem
            // 
            this.WindowsCloseAllMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsCloseAllMenuItem.Name = "WindowsCloseAllMenuItem";
            this.WindowsCloseAllMenuItem.Size = new System.Drawing.Size(159, 22);
            this.WindowsCloseAllMenuItem.Text = "C&lose All";
            this.WindowsCloseAllMenuItem.Click += new System.EventHandler(this.WindowsCloseAllMenuItem_Click);
            // 
            // WindowsArrangeIconsMenuItem
            // 
            this.WindowsArrangeIconsMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsArrangeIconsMenuItem.Name = "WindowsArrangeIconsMenuItem";
            this.WindowsArrangeIconsMenuItem.Size = new System.Drawing.Size(159, 22);
            this.WindowsArrangeIconsMenuItem.Text = "&Arrange Icons";
            this.WindowsArrangeIconsMenuItem.Click += new System.EventHandler(this.WindowsArrangeIconsMenuItem_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.HelpContentsMenuItem,
            this.HelpIndexMenuItem,
            this.HelpSearchMenuItem,
            this.HelpAboutMenuItem});
            this.HelpMenu.Name = "HelpMenu";
            this.HelpMenu.Size = new System.Drawing.Size(44, 20);
            this.HelpMenu.Text = "&Help";
            // 
            // HelpContentsMenuItem
            // 
            this.HelpContentsMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.HelpContentsMenuItem.Name = "HelpContentsMenuItem";
            this.HelpContentsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.HelpContentsMenuItem.Size = new System.Drawing.Size(168, 22);
            this.HelpContentsMenuItem.Text = "&Contents";
            // 
            // HelpIndexMenuItem
            // 
            this.HelpIndexMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.HelpIndexMenuItem.Name = "HelpIndexMenuItem";
            this.HelpIndexMenuItem.Size = new System.Drawing.Size(168, 22);
            this.HelpIndexMenuItem.Text = "&Index";
            // 
            // HelpSearchMenuItem
            // 
            this.HelpSearchMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.HelpSearchMenuItem.Name = "HelpSearchMenuItem";
            this.HelpSearchMenuItem.Size = new System.Drawing.Size(168, 22);
            this.HelpSearchMenuItem.Text = "&Search";
            // 
            // HelpAboutMenuItem
            // 
            this.HelpAboutMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.HelpAboutMenuItem.Name = "HelpAboutMenuItem";
            this.HelpAboutMenuItem.Size = new System.Drawing.Size(168, 22);
            this.HelpAboutMenuItem.Text = "&About...";
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
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 558);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.MenuStrip);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.MenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainView";
            this.Text = "Sound Explorers Audio Archive";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainView_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.MainView_VisibleChanged);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStrip ToolStrip;
        public System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem FileNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileRefreshMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditMenu;
        private System.Windows.Forms.ToolStripMenuItem EditUndoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditRedoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditCutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditCopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditPasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditSelectAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewMenu;
        private System.Windows.Forms.ToolStripMenuItem ViewToolBarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewStatusBarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenu;
        private System.Windows.Forms.ToolStripMenuItem ToolsPlayAudioMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsPlayVideoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsShowNewsletterMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsEditAudioFileTagsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsOptionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsMenu;
        private System.Windows.Forms.ToolStripMenuItem WindowsCascadeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsTileSideBySideMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsTileStackedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsCloseAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsArrangeIconsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpMenu;
        private System.Windows.Forms.ToolStripMenuItem HelpContentsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpIndexMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpSearchMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpAboutMenuItem;
        private System.Windows.Forms.ToolStripButton NewToolStripButton;
        private System.Windows.Forms.ToolStripButton OpenToolStripButton;
        private System.Windows.Forms.ToolStripButton HelpToolStripButton;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.ToolStripButton RefreshToolStripButton;
        private System.Windows.Forms.ToolStripButton CopyToolStripButton;
        private System.Windows.Forms.ToolStripButton CutToolStripButton;
        private System.Windows.Forms.ToolStripButton PasteToolStripButton;
        private System.Windows.Forms.ToolStripButton PlayAudioToolStripButton;
        private System.Windows.Forms.ToolStripButton PlayVideoToolStripButton;
        private System.Windows.Forms.ToolStripButton ShowNewsletterToolStripButton;
        public System.Windows.Forms.ToolStripStatusLabel StatusLabel;
    }
}
