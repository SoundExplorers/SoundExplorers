﻿using System.Windows.Forms;

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
      this.FileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.FileExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.EditCutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditCopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditPasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditDeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.EditSelectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditSelectRowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditDeleteSelectedRowsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ViewMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.ViewToolBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ViewStatusBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ToolsMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.ToolsLinkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ToolsOptionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsCascadeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsTileSideBySideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsTileStackedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsArrangeIconsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.WindowsNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsPreviousMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.WindowsCloseCurrentTableEditorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsCloseAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.WindowsSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.HelpAboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
      this.ToolStrip = new System.Windows.Forms.ToolStrip();
      this.NewToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.OpenToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.RefreshToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.CutToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.CopyToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.PasteToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.LinkToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.StatusLabel = new System.Windows.Forms.Label();
      this.MenuStrip.SuspendLayout();
      this.ToolStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // MenuStrip
      // 
      this.MenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.EditMenu,
            this.ViewMenu,
            this.ToolsMenu,
            this.WindowsMenu,
            this.HelpMenu});
      this.MenuStrip.Location = new System.Drawing.Point(0, 0);
      this.MenuStrip.MdiWindowListItem = this.WindowsMenu;
      this.MenuStrip.Name = "MenuStrip";
      this.MenuStrip.Size = new System.Drawing.Size(843, 24);
      this.MenuStrip.TabIndex = 0;
      this.MenuStrip.Text = "MenuStrip";
      this.MenuStrip.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.MenuStrip_ItemAdded);
      // 
      // FileMenu
      // 
      this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileNewMenuItem,
            this.FileOpenMenuItem,
            this.FileRefreshMenuItem,
            this.FileSeparator1,
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
      this.FileNewMenuItem.Size = new System.Drawing.Size(205, 22);
      this.FileNewMenuItem.Text = "&New Table Editor";
      this.FileNewMenuItem.Click += new System.EventHandler(this.FileNewMenuItem_Click);
      // 
      // FileOpenMenuItem
      // 
      this.FileOpenMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.FileOpenMenuItem.Name = "FileOpenMenuItem";
      this.FileOpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
      this.FileOpenMenuItem.Size = new System.Drawing.Size(205, 22);
      this.FileOpenMenuItem.Text = "&Open Table";
      this.FileOpenMenuItem.Click += new System.EventHandler(this.FileOpenMenuItem_Click);
      // 
      // FileRefreshMenuItem
      // 
      this.FileRefreshMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.FileRefreshMenuItem.Name = "FileRefreshMenuItem";
      this.FileRefreshMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
      this.FileRefreshMenuItem.Size = new System.Drawing.Size(205, 22);
      this.FileRefreshMenuItem.Text = "&Refresh Current Table";
      this.FileRefreshMenuItem.Click += new System.EventHandler(this.FileRefreshMenuItem_Click);
      // 
      // FileSeparator1
      // 
      this.FileSeparator1.Name = "FileSeparator1";
      this.FileSeparator1.Size = new System.Drawing.Size(202, 6);
      // 
      // FileExitMenuItem
      // 
      this.FileExitMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.FileExitMenuItem.Name = "FileExitMenuItem";
      this.FileExitMenuItem.ShortcutKeyDisplayString = "Alt+F4, Ctrl+Q";
      this.FileExitMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
      this.FileExitMenuItem.Size = new System.Drawing.Size(205, 22);
      this.FileExitMenuItem.Text = "E&xit";
      this.FileExitMenuItem.Click += new System.EventHandler(this.FileExitMenuItem_Click);
      // 
      // EditMenu
      // 
      this.EditMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditCutMenuItem,
            this.EditCopyMenuItem,
            this.EditPasteMenuItem,
            this.EditDeleteMenuItem,
            this.EditSeparator1,
            this.EditSelectAllMenuItem,
            this.EditSelectRowMenuItem,
            this.EditDeleteSelectedRowsMenuItem});
      this.EditMenu.Name = "EditMenu";
      this.EditMenu.Size = new System.Drawing.Size(39, 20);
      this.EditMenu.Text = "&Edit";
      // 
      // EditCutMenuItem
      // 
      this.EditCutMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.EditCutMenuItem.Name = "EditCutMenuItem";
      this.EditCutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
      this.EditCutMenuItem.Size = new System.Drawing.Size(244, 22);
      this.EditCutMenuItem.Text = "Cu&t";
      this.EditCutMenuItem.Click += new System.EventHandler(this.EditCutMenuItem_Click);
      // 
      // EditCopyMenuItem
      // 
      this.EditCopyMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.EditCopyMenuItem.Name = "EditCopyMenuItem";
      this.EditCopyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
      this.EditCopyMenuItem.Size = new System.Drawing.Size(244, 22);
      this.EditCopyMenuItem.Text = "&Copy";
      this.EditCopyMenuItem.Click += new System.EventHandler(this.EditCopyMenuItem_Click);
      // 
      // EditPasteMenuItem
      // 
      this.EditPasteMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.EditPasteMenuItem.Name = "EditPasteMenuItem";
      this.EditPasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
      this.EditPasteMenuItem.Size = new System.Drawing.Size(244, 22);
      this.EditPasteMenuItem.Text = "&Paste";
      this.EditPasteMenuItem.Click += new System.EventHandler(this.EditPasteMenuItem_Click);
      // 
      // EditDeleteMenuItem
      // 
      this.EditDeleteMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.EditDeleteMenuItem.Name = "EditDeleteMenuItem";
      this.EditDeleteMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
      this.EditDeleteMenuItem.Size = new System.Drawing.Size(244, 22);
      this.EditDeleteMenuItem.Text = "&Delete";
      this.EditDeleteMenuItem.Click += new System.EventHandler(this.EditDeleteMenuItem_Click);
      // 
      // EditSeparator1
      // 
      this.EditSeparator1.Name = "EditSeparator1";
      this.EditSeparator1.Size = new System.Drawing.Size(241, 6);
      // 
      // EditSelectAllMenuItem
      // 
      this.EditSelectAllMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.EditSelectAllMenuItem.Name = "EditSelectAllMenuItem";
      this.EditSelectAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
      this.EditSelectAllMenuItem.Size = new System.Drawing.Size(244, 22);
      this.EditSelectAllMenuItem.Text = "Select &All";
      this.EditSelectAllMenuItem.Click += new System.EventHandler(this.EditSelectAllMenuItem_Click);
      // 
      // EditSelectRowMenuItem
      // 
      this.EditSelectRowMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.EditSelectRowMenuItem.Name = "EditSelectRowMenuItem";
      this.EditSelectRowMenuItem.ShortcutKeyDisplayString = "Shift+Space";
      this.EditSelectRowMenuItem.Size = new System.Drawing.Size(244, 22);
      this.EditSelectRowMenuItem.Text = "&Select Row";
      this.EditSelectRowMenuItem.Click += new System.EventHandler(this.EditSelectRowMenuItem_Click);
      // 
      // EditDeleteSelectedRowsMenuItem
      // 
      this.EditDeleteSelectedRowsMenuItem.Name = "EditDeleteSelectedRowsMenuItem";
      this.EditDeleteSelectedRowsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
      this.EditDeleteSelectedRowsMenuItem.Size = new System.Drawing.Size(244, 22);
      this.EditDeleteSelectedRowsMenuItem.Text = "&Delete Selected Row(s)";
      this.EditDeleteSelectedRowsMenuItem.Click += new System.EventHandler(this.EditDeleteSelectedRowsMenuItem_Click);
      // 
      // ViewMenu
      // 
      this.ViewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewToolBarMenuItem,
            this.ViewStatusBarMenuItem});
      this.ViewMenu.Name = "ViewMenu";
      this.ViewMenu.Size = new System.Drawing.Size(44, 20);
      this.ViewMenu.Text = "&View";
      // 
      // ViewToolBarMenuItem
      // 
      this.ViewToolBarMenuItem.Checked = true;
      this.ViewToolBarMenuItem.CheckOnClick = true;
      this.ViewToolBarMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ViewToolBarMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.ViewToolBarMenuItem.Name = "ViewToolBarMenuItem";
      this.ViewToolBarMenuItem.Size = new System.Drawing.Size(123, 22);
      this.ViewToolBarMenuItem.Text = "&Toolbar";
      this.ViewToolBarMenuItem.Click += new System.EventHandler(this.ViewToolBarMenuItem_Click);
      // 
      // ViewStatusBarMenuItem
      // 
      this.ViewStatusBarMenuItem.Checked = true;
      this.ViewStatusBarMenuItem.CheckOnClick = true;
      this.ViewStatusBarMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ViewStatusBarMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.ViewStatusBarMenuItem.Name = "ViewStatusBarMenuItem";
      this.ViewStatusBarMenuItem.Size = new System.Drawing.Size(123, 22);
      this.ViewStatusBarMenuItem.Text = "&Statusbar";
      this.ViewStatusBarMenuItem.Click += new System.EventHandler(this.ViewStatusBarMenuItem_Click);
      // 
      // ToolsMenu
      // 
      this.ToolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolsLinkMenuItem,
            this.ToolsOptionsMenuItem});
      this.ToolsMenu.Name = "ToolsMenu";
      this.ToolsMenu.Size = new System.Drawing.Size(46, 20);
      this.ToolsMenu.Text = "&Tools";
      // 
      // ToolsLinkMenuItem
      // 
      this.ToolsLinkMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.ToolsLinkMenuItem.Name = "ToolsLinkMenuItem";
      this.ToolsLinkMenuItem.ShortcutKeyDisplayString = "Space, double-click";
      this.ToolsLinkMenuItem.Size = new System.Drawing.Size(244, 22);
      this.ToolsLinkMenuItem.Text = "Follow &Link";
      this.ToolsLinkMenuItem.Click += new System.EventHandler(this.ToolsLinkMenuItem_Click);
      // 
      // ToolsOptionsMenuItem
      // 
      this.ToolsOptionsMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.ToolsOptionsMenuItem.Name = "ToolsOptionsMenuItem";
      this.ToolsOptionsMenuItem.Size = new System.Drawing.Size(244, 22);
      this.ToolsOptionsMenuItem.Text = "&Options";
      this.ToolsOptionsMenuItem.Click += new System.EventHandler(this.ToolsOptionsMenuItem_Click);
      // 
      // WindowsMenu
      // 
      this.WindowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.WindowsCascadeMenuItem,
            this.WindowsTileSideBySideMenuItem,
            this.WindowsTileStackedMenuItem,
            this.WindowsArrangeIconsMenuItem,
            this.WindowsSeparator1,
            this.WindowsNextMenuItem,
            this.WindowsPreviousMenuItem,
            this.WindowsSeparator2,
            this.WindowsCloseCurrentTableEditorMenuItem,
            this.WindowsCloseAllMenuItem,
            this.WindowsSeparator3});
      this.WindowsMenu.Name = "WindowsMenu";
      this.WindowsMenu.Size = new System.Drawing.Size(68, 20);
      this.WindowsMenu.Text = "&Windows";
      // 
      // WindowsCascadeMenuItem
      // 
      this.WindowsCascadeMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.WindowsCascadeMenuItem.Name = "WindowsCascadeMenuItem";
      this.WindowsCascadeMenuItem.Size = new System.Drawing.Size(300, 22);
      this.WindowsCascadeMenuItem.Text = "&Cascade";
      this.WindowsCascadeMenuItem.Click += new System.EventHandler(this.WindowsCascadeMenuItem_Click);
      // 
      // WindowsTileSideBySideMenuItem
      // 
      this.WindowsTileSideBySideMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.WindowsTileSideBySideMenuItem.Name = "WindowsTileSideBySideMenuItem";
      this.WindowsTileSideBySideMenuItem.Size = new System.Drawing.Size(300, 22);
      this.WindowsTileSideBySideMenuItem.Text = "Tile &Side By Side";
      this.WindowsTileSideBySideMenuItem.Click += new System.EventHandler(this.WindowsTileSideBySideMenuItem_Click);
      // 
      // WindowsTileStackedMenuItem
      // 
      this.WindowsTileStackedMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.WindowsTileStackedMenuItem.Name = "WindowsTileStackedMenuItem";
      this.WindowsTileStackedMenuItem.Size = new System.Drawing.Size(300, 22);
      this.WindowsTileStackedMenuItem.Text = "Tile S&tacked";
      this.WindowsTileStackedMenuItem.Click += new System.EventHandler(this.WindowsTileStackedMenuItem_Click);
      // 
      // WindowsArrangeIconsMenuItem
      // 
      this.WindowsArrangeIconsMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.WindowsArrangeIconsMenuItem.Name = "WindowsArrangeIconsMenuItem";
      this.WindowsArrangeIconsMenuItem.Size = new System.Drawing.Size(300, 22);
      this.WindowsArrangeIconsMenuItem.Text = "&Arrange Icons";
      this.WindowsArrangeIconsMenuItem.Click += new System.EventHandler(this.WindowsArrangeIconsMenuItem_Click);
      // 
      // WindowsSeparator1
      // 
      this.WindowsSeparator1.Name = "WindowsSeparator1";
      this.WindowsSeparator1.Size = new System.Drawing.Size(297, 6);
      // 
      // WindowsNextMenuItem
      // 
      this.WindowsNextMenuItem.Name = "WindowsNextMenuItem";
      this.WindowsNextMenuItem.ShortcutKeyDisplayString = "Ctrl+F6";
      this.WindowsNextMenuItem.Size = new System.Drawing.Size(300, 22);
      this.WindowsNextMenuItem.Text = "&Next";
      this.WindowsNextMenuItem.Click += new System.EventHandler(this.WindowsNextMenuItem_Click);
      // 
      // WindowsPreviousMenuItem
      // 
      this.WindowsPreviousMenuItem.Name = "WindowsPreviousMenuItem";
      this.WindowsPreviousMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+F6";
      this.WindowsPreviousMenuItem.Size = new System.Drawing.Size(300, 22);
      this.WindowsPreviousMenuItem.Text = "&Previous";
      this.WindowsPreviousMenuItem.Click += new System.EventHandler(this.WindowsPreviousMenuItem_Click);
      // 
      // WindowsSeparator2
      // 
      this.WindowsSeparator2.Name = "WindowsSeparator2";
      this.WindowsSeparator2.Size = new System.Drawing.Size(297, 6);
      // 
      // WindowsCloseCurrentTableEditorMenuItem
      // 
      this.WindowsCloseCurrentTableEditorMenuItem.Name = "WindowsCloseCurrentTableEditorMenuItem";
      this.WindowsCloseCurrentTableEditorMenuItem.ShortcutKeyDisplayString = "Ctrl+F4, Ctrl+W";
      this.WindowsCloseCurrentTableEditorMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
      this.WindowsCloseCurrentTableEditorMenuItem.Size = new System.Drawing.Size(300, 22);
      this.WindowsCloseCurrentTableEditorMenuItem.Text = "C&lose Current Table Editor";
      this.WindowsCloseCurrentTableEditorMenuItem.Click += new System.EventHandler(this.WindowsCloseCurrentTableEditorMenuItem_Click);
      // 
      // WindowsCloseAllMenuItem
      // 
      this.WindowsCloseAllMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.WindowsCloseAllMenuItem.Name = "WindowsCloseAllMenuItem";
      this.WindowsCloseAllMenuItem.Size = new System.Drawing.Size(300, 22);
      this.WindowsCloseAllMenuItem.Text = "Cl&ose All";
      this.WindowsCloseAllMenuItem.Click += new System.EventHandler(this.WindowsCloseAllMenuItem_Click);
      // 
      // WindowsSeparator3
      // 
      this.WindowsSeparator3.Name = "WindowsSeparator3";
      this.WindowsSeparator3.Size = new System.Drawing.Size(297, 6);
      this.WindowsSeparator3.Visible = false;
      // 
      // HelpMenu
      // 
      this.HelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HelpAboutMenuItem});
      this.HelpMenu.Name = "HelpMenu";
      this.HelpMenu.Size = new System.Drawing.Size(44, 20);
      this.HelpMenu.Text = "&Help";
      // 
      // HelpAboutMenuItem
      // 
      this.HelpAboutMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
      this.HelpAboutMenuItem.Name = "HelpAboutMenuItem";
      this.HelpAboutMenuItem.Size = new System.Drawing.Size(116, 22);
      this.HelpAboutMenuItem.Text = "&About...";
      this.HelpAboutMenuItem.Click += new System.EventHandler(this.HelpAboutMenuItem_Click);
      // 
      // ToolStrip
      // 
      this.ToolStrip.AutoSize = false;
      this.ToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewToolStripButton,
            this.OpenToolStripButton,
            this.RefreshToolStripButton,
            this.CutToolStripButton,
            this.CopyToolStripButton,
            this.PasteToolStripButton,
            this.LinkToolStripButton});
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
      this.NewToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("NewToolStripButton.Image")));
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
      this.OpenToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenToolStripButton.Image")));
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
      this.RefreshToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("RefreshToolStripButton.Image")));
      this.RefreshToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.RefreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.RefreshToolStripButton.Name = "RefreshToolStripButton";
      this.RefreshToolStripButton.Size = new System.Drawing.Size(38, 38);
      this.RefreshToolStripButton.Text = "Refresh Current Table (F5)";
      this.RefreshToolStripButton.Click += new System.EventHandler(this.FileRefreshMenuItem_Click);
      // 
      // CutToolStripButton
      // 
      this.CutToolStripButton.AutoSize = false;
      this.CutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.CutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CutToolStripButton.Image")));
      this.CutToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.CutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.CutToolStripButton.Name = "CutToolStripButton";
      this.CutToolStripButton.Size = new System.Drawing.Size(38, 38);
      this.CutToolStripButton.Text = "Cut (Ctrl+X)";
      this.CutToolStripButton.Click += new System.EventHandler(this.EditCutMenuItem_Click);
      // 
      // CopyToolStripButton
      // 
      this.CopyToolStripButton.AutoSize = false;
      this.CopyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.CopyToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CopyToolStripButton.Image")));
      this.CopyToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.CopyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.CopyToolStripButton.Name = "CopyToolStripButton";
      this.CopyToolStripButton.Size = new System.Drawing.Size(38, 38);
      this.CopyToolStripButton.Text = "Copy (Ctrl+C)";
      this.CopyToolStripButton.Click += new System.EventHandler(this.EditCopyMenuItem_Click);
      // 
      // PasteToolStripButton
      // 
      this.PasteToolStripButton.AutoSize = false;
      this.PasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.PasteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("PasteToolStripButton.Image")));
      this.PasteToolStripButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.PasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.PasteToolStripButton.Name = "PasteToolStripButton";
      this.PasteToolStripButton.Size = new System.Drawing.Size(38, 38);
      this.PasteToolStripButton.Text = "Paste (Ctrl+V)";
      this.PasteToolStripButton.Click += new System.EventHandler(this.EditPasteMenuItem_Click);
      // 
      // LinkToolStripButton
      // 
      this.LinkToolStripButton.AutoSize = false;
      this.LinkToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.LinkToolStripButton.Image = global::SoundExplorers.View.Properties.Resources.Link_Noto_Emoji_32x32;
      this.LinkToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.LinkToolStripButton.Name = "LinkToolStripButton";
      this.LinkToolStripButton.Size = new System.Drawing.Size(24, 38);
      this.LinkToolStripButton.Text = "Follow Link (Space or double-click)";
      this.LinkToolStripButton.Click += new System.EventHandler(this.ToolsLinkMenuItem_Click);
      // 
      // StatusLabel
      // 
      this.StatusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.StatusLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.StatusLabel.Location = new System.Drawing.Point(0, 524);
      this.StatusLabel.Name = "StatusLabel";
      this.StatusLabel.Size = new System.Drawing.Size(843, 34);
      this.StatusLabel.TabIndex = 2;
      this.StatusLabel.Text = "StatusLabel";
      this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // MainView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(843, 558);
      this.Controls.Add(this.ToolStrip);
      this.Controls.Add(this.MenuStrip);
      this.Controls.Add(this.StatusLabel);
      this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.IsMdiContainer = true;
      this.KeyPreview = true;
      this.MainMenuStrip = this.MenuStrip;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "MainView";
      this.Text = "Sound Explorers Audio Archive";
      this.MenuStrip.ResumeLayout(false);
      this.MenuStrip.PerformLayout();
      this.ToolStrip.ResumeLayout(false);
      this.ToolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }
        #endregion
        
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem FileNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileRefreshMenuItem;
        private System.Windows.Forms.ToolStripSeparator FileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem FileExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditMenu;
        private System.Windows.Forms.ToolStripMenuItem EditCutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditCopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditPasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditDeleteMenuItem;
        private System.Windows.Forms.ToolStripSeparator EditSeparator1;
        private System.Windows.Forms.ToolStripMenuItem EditSelectAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditSelectRowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditDeleteSelectedRowsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewMenu;
        private System.Windows.Forms.ToolStripMenuItem ViewToolBarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewStatusBarMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenu;
        private System.Windows.Forms.ToolStripMenuItem ToolsLinkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsOptionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsMenu;
        private System.Windows.Forms.ToolStripMenuItem WindowsCascadeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsTileSideBySideMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsTileStackedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsArrangeIconsMenuItem;
        private System.Windows.Forms.ToolStripSeparator WindowsSeparator1;
        private System.Windows.Forms.ToolStripMenuItem WindowsNextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsPreviousMenuItem;
        private System.Windows.Forms.ToolStripSeparator WindowsSeparator2;
        private System.Windows.Forms.ToolStripMenuItem WindowsCloseCurrentTableEditorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem WindowsCloseAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator WindowsSeparator3;
        private System.Windows.Forms.ToolStripMenuItem HelpMenu;
        private System.Windows.Forms.ToolStripMenuItem HelpAboutMenuItem;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.ToolStripButton NewToolStripButton;
        private System.Windows.Forms.ToolStripButton OpenToolStripButton;
        private System.Windows.Forms.ToolStripButton RefreshToolStripButton;
        public System.Windows.Forms.ToolStripButton CutToolStripButton;
        public System.Windows.Forms.ToolStripButton CopyToolStripButton;
        public System.Windows.Forms.ToolStripButton PasteToolStripButton;
        public System.Windows.Forms.ToolStripButton LinkToolStripButton;
        public System.Windows.Forms.Label StatusLabel;
  }
}
