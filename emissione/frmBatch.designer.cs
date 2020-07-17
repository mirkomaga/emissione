namespace EdgeCode
{
    partial class frmBatch
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBatch));
            this.dlgFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.imgTree = new System.Windows.Forms.ImageList(this.components);
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dlgOpenList = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveList = new System.Windows.Forms.SaveFileDialog();
            this.imgStatus = new System.Windows.Forms.ImageList(this.components);
            this.dlgOpenExcel = new System.Windows.Forms.OpenFileDialog();
            this.btnAvvia = new System.Windows.Forms.Button();
            this.lstFiles = new System.Windows.Forms.ListView();
            this.col1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fldCount = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.statusBot = new System.Windows.Forms.StatusStrip();
            this.lgnd1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lgnd2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnClearAll = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.cmdLoadList = new System.Windows.Forms.ToolStripButton();
            this.cmdSaveList = new System.Windows.Forms.ToolStripButton();
            this.lgnd3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsPd = new System.Windows.Forms.ToolStripProgressBar();
            this.nameFile = new System.Windows.Forms.ToolStripStatusLabel();
            this.counter = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.statusBot.SuspendLayout();
            this.SuspendLayout();
            // 
            // dlgFolder
            // 
            this.dlgFolder.Description = "Seleziona cartella da aggiungene in elenco";
            this.dlgFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.dlgFolder.ShowNewFolderButton = false;
            // 
            // imgTree
            // 
            this.imgTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgTree.ImageStream")));
            this.imgTree.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(233)))), ((int)(((byte)(237)))));
            this.imgTree.Images.SetKeyName(0, "green_check");
            this.imgTree.Images.SetKeyName(1, "black_cross");
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // dlgOpen
            // 
            this.dlgOpen.Filter = "AutoCAD Drawing files|*.ipt|Tutti i file|*.*";
            this.dlgOpen.FilterIndex = 0;
            this.dlgOpen.Multiselect = true;
            this.dlgOpen.Title = "Seleziona blocchi da aggiornare";
            // 
            // dlgOpenList
            // 
            this.dlgOpenList.Filter = "Execution list files (*.lst)|*.lst|Tutti i file (*.*)|*.*";
            this.dlgOpenList.FilterIndex = 0;
            this.dlgOpenList.Multiselect = true;
            this.dlgOpenList.Title = "Seleziona elenco di esecuzione precedente";
            // 
            // dlgSaveList
            // 
            this.dlgSaveList.DefaultExt = "lst";
            this.dlgSaveList.Filter = "Execution list files (*.lst)|*.lst|Tutti i file (*.*)|*.*";
            this.dlgSaveList.FilterIndex = 0;
            // 
            // imgStatus
            // 
            this.imgStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgStatus.ImageStream")));
            this.imgStatus.TransparentColor = System.Drawing.Color.Transparent;
            this.imgStatus.Images.SetKeyName(0, "available");
            this.imgStatus.Images.SetKeyName(1, "working");
            this.imgStatus.Images.SetKeyName(2, "pending");
            this.imgStatus.Images.SetKeyName(3, "processing");
            this.imgStatus.Images.SetKeyName(4, "ok");
            this.imgStatus.Images.SetKeyName(5, "saved");
            this.imgStatus.Images.SetKeyName(6, "nok");
            this.imgStatus.Images.SetKeyName(7, "canceled");
            this.imgStatus.Images.SetKeyName(8, "aborted");
            // 
            // dlgOpenExcel
            // 
            this.dlgOpenExcel.Filter = "Excel workbook (*.xlsx)|*.xlsx|Excel workbook (*.xls)|*.xls|Tutti i file (*.*)|*." +
    "*";
            this.dlgOpenExcel.FilterIndex = 0;
            this.dlgOpenExcel.Multiselect = true;
            this.dlgOpenExcel.Title = "Seleziona elenco attributi da applicare";
            // 
            // btnAvvia
            // 
            this.btnAvvia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAvvia.Location = new System.Drawing.Point(692, 428);
            this.btnAvvia.Name = "btnAvvia";
            this.btnAvvia.Size = new System.Drawing.Size(109, 32);
            this.btnAvvia.TabIndex = 13;
            this.btnAvvia.Text = "Avvia";
            this.btnAvvia.UseVisualStyleBackColor = true;
            this.btnAvvia.Click += new System.EventHandler(this.btnAvvia_Click_1);
            // 
            // lstFiles
            // 
            this.lstFiles.AllowDrop = true;
            this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col1,
            this.col2,
            this.col3});
            this.lstFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstFiles.FullRowSelect = true;
            this.lstFiles.GridLines = true;
            this.lstFiles.HideSelection = false;
            this.lstFiles.Location = new System.Drawing.Point(0, 25);
            this.lstFiles.Margin = new System.Windows.Forms.Padding(4);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(812, 396);
            this.lstFiles.SmallImageList = this.imageList1;
            this.lstFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstFiles.TabIndex = 12;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            this.lstFiles.SelectedIndexChanged += new System.EventHandler(this.lstFiles_SelectedIndexChanged);
            this.lstFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstFiles_DragDrop);
            this.lstFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstFiles_DragEnter);
            // 
            // col1
            // 
            this.col1.Text = "Nome";
            this.col1.Width = 75;
            // 
            // col2
            // 
            this.col2.Text = "Percorso";
            this.col2.Width = 207;
            // 
            // col3
            // 
            this.col3.Text = "Stato";
            this.col3.Width = 69;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnDelete,
            this.toolStripSeparator1,
            this.btnClearAll,
            this.btnRedo,
            this.fldCount,
            this.toolStripSeparator2,
            this.cmdLoadList,
            this.cmdSaveList,
            this.toolStripSeparator3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(812, 25);
            this.toolStrip1.TabIndex = 11;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // fldCount
            // 
            this.fldCount.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.fldCount.BackColor = System.Drawing.SystemColors.Control;
            this.fldCount.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fldCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.fldCount.Name = "fldCount";
            this.fldCount.ReadOnly = true;
            this.fldCount.Size = new System.Drawing.Size(125, 25);
            this.fldCount.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // statusBot
            // 
            this.statusBot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lgnd1,
            this.lgnd2,
            this.lgnd3,
            this.nameFile,
            this.counter,
            this.tsPd});
            this.statusBot.Location = new System.Drawing.Point(0, 472);
            this.statusBot.Name = "statusBot";
            this.statusBot.Size = new System.Drawing.Size(812, 22);
            this.statusBot.TabIndex = 21;
            this.statusBot.Text = "statusStrip1";
            // 
            // lgnd1
            // 
            this.lgnd1.AccessibleName = "lgnd1";
            this.lgnd1.Image = global::emissione.Properties.Resources.exclamation;
            this.lgnd1.Name = "lgnd1";
            this.lgnd1.Size = new System.Drawing.Size(115, 17);
            this.lgnd1.Text = "L barra mancante";
            this.lgnd1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // lgnd2
            // 
            this.lgnd2.AccessibleName = "lgnd2";
            this.lgnd2.Image = global::emissione.Properties.Resources.bullet_error;
            this.lgnd2.Name = "lgnd2";
            this.lgnd2.Size = new System.Drawing.Size(118, 17);
            this.lgnd2.Text = "L taglio mancante";
            this.lgnd2.Click += new System.EventHandler(this.toolStripStatusLabel2_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(23, 22);
            this.btnAdd.Text = "toolStripButton1";
            this.btnAdd.ToolTipText = "Aggiungi file o cartelle";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click_1);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 22);
            this.btnDelete.Text = "toolStripButton1";
            this.btnDelete.ToolTipText = "Rimuovi elementi selezionati";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClearAll.Image = ((System.Drawing.Image)(resources.GetObject("btnClearAll.Image")));
            this.btnClearAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(23, 22);
            this.btnClearAll.Text = "toolStripButton1";
            this.btnClearAll.ToolTipText = "Azzera elenco file";
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRedo.Image = ((System.Drawing.Image)(resources.GetObject("btnRedo.Image")));
            this.btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(23, 22);
            this.btnRedo.Text = "toolStripButton1";
            this.btnRedo.ToolTipText = "Azzera Stato";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // cmdLoadList
            // 
            this.cmdLoadList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdLoadList.Image = ((System.Drawing.Image)(resources.GetObject("cmdLoadList.Image")));
            this.cmdLoadList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdLoadList.Name = "cmdLoadList";
            this.cmdLoadList.Size = new System.Drawing.Size(23, 22);
            this.cmdLoadList.Text = "Carica lista";
            this.cmdLoadList.ToolTipText = "Carica elenco esecuzione precedente";
            this.cmdLoadList.Click += new System.EventHandler(this.cmdLoadList_Click);
            // 
            // cmdSaveList
            // 
            this.cmdSaveList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdSaveList.Image = ((System.Drawing.Image)(resources.GetObject("cmdSaveList.Image")));
            this.cmdSaveList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdSaveList.Name = "cmdSaveList";
            this.cmdSaveList.Size = new System.Drawing.Size(23, 22);
            this.cmdSaveList.Text = "Save list";
            this.cmdSaveList.ToolTipText = "Salva lista con nome";
            this.cmdSaveList.Click += new System.EventHandler(this.cmdSaveList_Click);
            // 
            // lgnd3
            // 
            this.lgnd3.AccessibleName = "lgnd3";
            this.lgnd3.Image = global::emissione.Properties.Resources.information;
            this.lgnd3.Name = "lgnd3";
            this.lgnd3.Size = new System.Drawing.Size(97, 17);
            this.lgnd3.Text = "Info mancanti";
            // 
            // tsPd
            // 
            this.tsPd.AutoSize = false;
            this.tsPd.Name = "tsPd";
            this.tsPd.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tsPd.Size = new System.Drawing.Size(250, 16);
            this.tsPd.Click += new System.EventHandler(this.toolStripProgressBar1_Click);
            // 
            // nameFile
            // 
            this.nameFile.AccessibleName = "NomeFIle";
            this.nameFile.Name = "nameFile";
            this.nameFile.Size = new System.Drawing.Size(80, 17);
            this.nameFile.Text = "nomefile.dwg";
            this.nameFile.Click += new System.EventHandler(this.toolStripStatusLabel4_Click);
            // 
            // counter
            // 
            this.counter.Name = "counter";
            this.counter.Size = new System.Drawing.Size(24, 17);
            this.counter.Text = "0/0";
            this.counter.Click += new System.EventHandler(this.toolStripStatusLabel1_Click_1);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "exclamation.png");
            this.imageList1.Images.SetKeyName(1, "bullet_error.png");
            this.imageList1.Images.SetKeyName(2, "information.png");
            // 
            // frmBatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(812, 494);
            this.Controls.Add(this.statusBot);
            this.Controls.Add(this.btnAvvia);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.toolStrip1);
            this.Name = "frmBatch";
            this.Text = "frmBatch";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusBot.ResumeLayout(false);
            this.statusBot.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog dlgFolder;
        private System.Windows.Forms.ImageList imgTree;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.OpenFileDialog dlgOpenList;
        private System.Windows.Forms.SaveFileDialog dlgSaveList;
        private System.Windows.Forms.ImageList imgStatus;
        private System.Windows.Forms.OpenFileDialog dlgOpenExcel;
        private System.Windows.Forms.Button btnAvvia;
        private System.Windows.Forms.ListView lstFiles;
        private System.Windows.Forms.ColumnHeader col1;
        private System.Windows.Forms.ColumnHeader col2;
        private System.Windows.Forms.ColumnHeader col3;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnClearAll;
        private System.Windows.Forms.ToolStripButton btnRedo;
        private System.Windows.Forms.ToolStripTextBox fldCount;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton cmdLoadList;
        private System.Windows.Forms.ToolStripButton cmdSaveList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripProgressBar tsPd;
        public System.Windows.Forms.ToolStripStatusLabel nameFile;
        public System.Windows.Forms.ToolStripStatusLabel counter;
        public System.Windows.Forms.StatusStrip statusBot;
        public System.Windows.Forms.ToolStripStatusLabel lgnd1;
        public System.Windows.Forms.ToolStripStatusLabel lgnd2;
        public System.Windows.Forms.ToolStripStatusLabel lgnd3;
        private System.Windows.Forms.ImageList imageList1;
    }
}