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
            this.EditCutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditPasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditSelectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditDeleteSelectedRowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsOptionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsCascadeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsTileSideBySideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsTileStackedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsCloseAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsArrangeIconsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpAboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.NewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.OpenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.RefreshToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.CopyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.CutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.PasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.MenuStrip.SuspendLayout();
            this.ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.FileMenu, this.EditMenu, this.ViewMenu, this.ToolsMenu, this.WindowsMenu, this.HelpMenu});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.MdiWindowListItem = this.WindowsMenu;
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.MenuStrip.Size = new System.Drawing.Size(843, 28);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.Text = "MenuStrip";
            this.MenuStrip.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.MenuStrip_ItemAdded);
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.FileNewMenuItem, this.FileOpenMenuItem, this.FileRefreshMenuItem, this.FileExitMenuItem});
            this.FileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(44, 24);
            this.FileMenu.Text = "&File";
            // 
            // FileNewMenuItem
            // 
            this.FileNewMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.FileNewMenuItem.Name = "FileNewMenuItem";
            this.FileNewMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.FileNewMenuItem.Size = new System.Drawing.Size(244, 24);
            this.FileNewMenuItem.Text = "&New Table Editor";
            this.FileNewMenuItem.Click += new System.EventHandler(this.FileNewMenuItem_Click);
            // 
            // FileOpenMenuItem
            // 
            this.FileOpenMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.FileOpenMenuItem.Name = "FileOpenMenuItem";
            this.FileOpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.FileOpenMenuItem.Size = new System.Drawing.Size(244, 24);
            this.FileOpenMenuItem.Text = "&Open Table";
            this.FileOpenMenuItem.Click += new System.EventHandler(this.FileOpenMenuItem_Click);
            // 
            // FileRefreshMenuItem
            // 
            this.FileRefreshMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.FileRefreshMenuItem.Name = "FileRefreshMenuItem";
            this.FileRefreshMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.FileRefreshMenuItem.Size = new System.Drawing.Size(244, 24);
            this.FileRefreshMenuItem.Text = "&Refresh Table";
            this.FileRefreshMenuItem.Click += new System.EventHandler(this.FileRefreshMenuItem_Click);
            // 
            // FileExitMenuItem
            // 
            this.FileExitMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.FileExitMenuItem.Name = "FileExitMenuItem";
            this.FileExitMenuItem.Size = new System.Drawing.Size(244, 24);
            this.FileExitMenuItem.Text = "E&xit";
            this.FileExitMenuItem.Click += new System.EventHandler(this.FileExitMenuItem_Click);
            // 
            // EditMenu
            // 
            this.EditMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.EditCutMenuItem, this.EditCopyMenuItem, this.EditPasteMenuItem, this.EditSelectAllMenuItem, this.EditDeleteSelectedRowsMenuItem});
            this.EditMenu.Name = "EditMenu";
            this.EditMenu.Size = new System.Drawing.Size(47, 24);
            this.EditMenu.Text = "&Edit";
            // 
            // EditCutMenuItem
            // 
            this.EditCutMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditCutMenuItem.Name = "EditCutMenuItem";
            this.EditCutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.EditCutMenuItem.Size = new System.Drawing.Size(297, 24);
            this.EditCutMenuItem.Text = "Cu&t";
            this.EditCutMenuItem.Click += new System.EventHandler(this.EditCutMenuItem_Click);
            // 
            // EditCopyMenuItem
            // 
            this.EditCopyMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditCopyMenuItem.Name = "EditCopyMenuItem";
            this.EditCopyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.EditCopyMenuItem.Size = new System.Drawing.Size(297, 24);
            this.EditCopyMenuItem.Text = "&Copy";
            this.EditCopyMenuItem.Click += new System.EventHandler(this.EditCopyMenuItem_Click);
            // 
            // EditPasteMenuItem
            // 
            this.EditPasteMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditPasteMenuItem.Name = "EditPasteMenuItem";
            this.EditPasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.EditPasteMenuItem.Size = new System.Drawing.Size(297, 24);
            this.EditPasteMenuItem.Text = "&Paste";
            this.EditPasteMenuItem.Click += new System.EventHandler(this.EditPasteMenuItem_Click);
            // 
            // EditSelectAllMenuItem
            // 
            this.EditSelectAllMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.EditSelectAllMenuItem.Name = "EditSelectAllMenuItem";
            this.EditSelectAllMenuItem.ShortcutKeyDisplayString = "Ctrl+A, F2";
            this.EditSelectAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.EditSelectAllMenuItem.Size = new System.Drawing.Size(297, 24);
            this.EditSelectAllMenuItem.Text = "Select &All";
            this.EditSelectAllMenuItem.Click += new System.EventHandler(this.EditSelectAllMenuItem_Click);
            // 
            // EditDeleteSelectedRowsMenuItem
            // 
            this.EditDeleteSelectedRowsMenuItem.Name = "EditDeleteSelectedRowsMenuItem";
            this.EditDeleteSelectedRowsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.EditDeleteSelectedRowsMenuItem.Size = new System.Drawing.Size(297, 24);
            this.EditDeleteSelectedRowsMenuItem.Text = "&Delete Selected Row(s)";
            this.EditDeleteSelectedRowsMenuItem.Click += new System.EventHandler(this.EditDeleteSelectedRowsMenuItem_Click);
            // 
            // ViewMenu
            // 
            this.ViewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.ViewToolBarMenuItem});
            this.ViewMenu.Name = "ViewMenu";
            this.ViewMenu.Size = new System.Drawing.Size(53, 24);
            this.ViewMenu.Text = "&View";
            // 
            // ViewToolBarMenuItem
            // 
            this.ViewToolBarMenuItem.Checked = true;
            this.ViewToolBarMenuItem.CheckOnClick = true;
            this.ViewToolBarMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewToolBarMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ViewToolBarMenuItem.Name = "ViewToolBarMenuItem";
            this.ViewToolBarMenuItem.Size = new System.Drawing.Size(129, 24);
            this.ViewToolBarMenuItem.Text = "&Toolbar";
            this.ViewToolBarMenuItem.Click += new System.EventHandler(this.ViewToolBarMenuItem_Click);
            // 
            // ToolsMenu
            // 
            this.ToolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.ToolsOptionsMenuItem});
            this.ToolsMenu.Name = "ToolsMenu";
            this.ToolsMenu.Size = new System.Drawing.Size(56, 24);
            this.ToolsMenu.Text = "&Tools";
            // 
            // ToolsOptionsMenuItem
            // 
            this.ToolsOptionsMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ToolsOptionsMenuItem.Name = "ToolsOptionsMenuItem";
            this.ToolsOptionsMenuItem.Size = new System.Drawing.Size(130, 24);
            this.ToolsOptionsMenuItem.Text = "&Options";
            // 
            // WindowsMenu
            // 
            this.WindowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.WindowsCascadeMenuItem, this.WindowsTileSideBySideMenuItem, this.WindowsTileStackedMenuItem, this.WindowsCloseAllMenuItem, this.WindowsArrangeIconsMenuItem});
            this.WindowsMenu.Name = "WindowsMenu";
            this.WindowsMenu.Size = new System.Drawing.Size(82, 24);
            this.WindowsMenu.Text = "&Windows";
            // 
            // WindowsCascadeMenuItem
            // 
            this.WindowsCascadeMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsCascadeMenuItem.Name = "WindowsCascadeMenuItem";
            this.WindowsCascadeMenuItem.Size = new System.Drawing.Size(188, 24);
            this.WindowsCascadeMenuItem.Text = "&Cascade";
            this.WindowsCascadeMenuItem.Click += new System.EventHandler(this.WindowsCascadeMenuItem_Click);
            // 
            // WindowsTileSideBySideMenuItem
            // 
            this.WindowsTileSideBySideMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsTileSideBySideMenuItem.Name = "WindowsTileSideBySideMenuItem";
            this.WindowsTileSideBySideMenuItem.Size = new System.Drawing.Size(188, 24);
            this.WindowsTileSideBySideMenuItem.Text = "Tile &Side By Side";
            this.WindowsTileSideBySideMenuItem.Click += new System.EventHandler(this.WindowsTileSideBySideMenuItem_Click);
            // 
            // WindowsTileStackedMenuItem
            // 
            this.WindowsTileStackedMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsTileStackedMenuItem.Name = "WindowsTileStackedMenuItem";
            this.WindowsTileStackedMenuItem.Size = new System.Drawing.Size(188, 24);
            this.WindowsTileStackedMenuItem.Text = "Tile S&tacked";
            this.WindowsTileStackedMenuItem.Click += new System.EventHandler(this.WindowsTileStackedMenuItem_Click);
            // 
            // WindowsCloseAllMenuItem
            // 
            this.WindowsCloseAllMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsCloseAllMenuItem.Name = "WindowsCloseAllMenuItem";
            this.WindowsCloseAllMenuItem.Size = new System.Drawing.Size(188, 24);
            this.WindowsCloseAllMenuItem.Text = "C&lose All";
            this.WindowsCloseAllMenuItem.Click += new System.EventHandler(this.WindowsCloseAllMenuItem_Click);
            // 
            // WindowsArrangeIconsMenuItem
            // 
            this.WindowsArrangeIconsMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.WindowsArrangeIconsMenuItem.Name = "WindowsArrangeIconsMenuItem";
            this.WindowsArrangeIconsMenuItem.Size = new System.Drawing.Size(188, 24);
            this.WindowsArrangeIconsMenuItem.Text = "&Arrange Icons";
            this.WindowsArrangeIconsMenuItem.Click += new System.EventHandler(this.WindowsArrangeIconsMenuItem_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.HelpAboutMenuItem});
            this.HelpMenu.Name = "HelpMenu";
            this.HelpMenu.Size = new System.Drawing.Size(53, 24);
            this.HelpMenu.Text = "&Help";
            // 
            // HelpAboutMenuItem
            // 
            this.HelpAboutMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.HelpAboutMenuItem.Name = "HelpAboutMenuItem";
            this.HelpAboutMenuItem.Size = new System.Drawing.Size(128, 24);
            this.HelpAboutMenuItem.Text = "&About...";
            this.HelpAboutMenuItem.Click += new System.EventHandler(this.HelpAboutMenuItem_Click);
            // 
            // ToolStrip
            // 
            this.ToolStrip.AutoSize = false;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.NewToolStripButton, this.OpenToolStripButton, this.RefreshToolStripButton, this.CopyToolStripButton, this.CutToolStripButton, this.PasteToolStripButton});
            this.ToolStrip.Location = new System.Drawing.Point(0, 28);
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
            this.NewToolStripButton.Text = "New Table Editor (Ctrl+N)";
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
            this.OpenToolStripButton.Text = "Open Table (Ctrl+O)";
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
            this.RefreshToolStripButton.Text = "Refresh Table (F5)";
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
            this.CopyToolStripButton.Text = "Copy (Ctrl+C)";
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
            this.CutToolStripButton.Text = "Cut (Ctrl+X)";
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
            this.PasteToolStripButton.Text = "Paste (Ctrl+V)";
            this.PasteToolStripButton.Click += new System.EventHandler(this.EditPasteMenuItem_Click);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 558);
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
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ToolStripMenuItem EditDeleteSelectedRowsMenuItem;

        #endregion


        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem FileNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileRefreshMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditMenu;
        private System.Windows.Forms.ToolStripMenuItem EditCutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditCopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditPasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditSelectAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewMenu;
        private System.Windows.Forms.ToolStripMenuItem ViewToolBarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenu;
        private System.Windows.Forms.ToolStripMenuItem ToolsOptionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsMenu;
        private System.Windows.Forms.ToolStripMenuItem WindowsCascadeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsTileSideBySideMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsTileStackedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsCloseAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsArrangeIconsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpMenu;
        private System.Windows.Forms.ToolStripMenuItem HelpAboutMenuItem;
        private System.Windows.Forms.ToolStripButton NewToolStripButton;
        private System.Windows.Forms.ToolStripButton OpenToolStripButton;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.ToolStripButton RefreshToolStripButton;
        private System.Windows.Forms.ToolStripButton CopyToolStripButton;
        private System.Windows.Forms.ToolStripButton CutToolStripButton;
        private System.Windows.Forms.ToolStripButton PasteToolStripButton;
    }
}
