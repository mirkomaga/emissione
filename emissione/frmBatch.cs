using System;
using System.Collections.Generic;

using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using emissione.Properties;
using System.Drawing;
using Microsoft.Office.Interop.Excel;

namespace EdgeCode
{

    public partial class frmBatch : System.Windows.Forms.Form
    {
        bool m_IsBusy = false;
        bool m_Cancel = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandData">the external command data</param>
        public frmBatch()
        { 
            InitializeComponent();
            this.tsPd.Width = 30;
        }

        //private void frmBatch_Load_1(object sender, EventArgs e)
        //{
        //    //lstMessage.Items.Clear();
        //}

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            if (m_IsBusy) return;

            // aggiungi cartella e suo contenuto all'elenco
            if (dlgFolder.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo d = new DirectoryInfo(dlgFolder.SelectedPath);

                // carica cartella in modo ricorsivo
                RecurseFolder(d);
                UpdateCount();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Rimuovi elementi selezionati da elenco file
            foreach (ListViewItem itm in lstFiles.SelectedItems)
            {
                itm.Remove();
            }
            UpdateCount();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (m_IsBusy) return;

            // azzera elenco file
            lstFiles.Items.Clear();
            UpdateCount();
        }

        private void UpdateCount()
        {
            int n = lstFiles.Items.Count;

            if (n == 0) fldCount.Text = "";
            else if (n == 1) fldCount.Text = "1 file";
            else fldCount.Text = n.ToString() + " files";
        }

        private void cmdLoadList_Click(object sender, EventArgs e)
        {
            dlgOpenList.CheckFileExists = true;
            dlgOpenList.Multiselect = false;

            if (dlgOpenList.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader srList = new StreamReader(dlgOpenList.FileName);
                String strBuff = "";
                while ((strBuff = srList.ReadLine()) != null)
                {
                    strBuff = strBuff.Trim();
                    if (strBuff.Length == 0) continue;

                    FileInfo fi = new FileInfo(strBuff);
                    if (fi.Exists) this.AddFileToList(lstFiles, fi);
                }

                srList.Close();
                UpdateCount();
            }
        }

        private void SaveFileList(String szListFile)
        {
            StreamWriter swList = new StreamWriter(szListFile);    // apri e sovrascrivi

            foreach (ListViewItem itm in lstFiles.Items)
            {
                swList.WriteLine(itm.SubItems[1].Text + "\\" + itm.SubItems[0].Text);
            }

            swList.Close();  // chiudi
            swList = null;
        }

        private void SaveSettings()
        {
            // salva impostazioni correnti
            //if (cmbPlotters.SelectedIndex > -1) csSettings.plt_Plotter = cmbPlotters.Text;
            //if (cmbFormati.SelectedIndex > -1) csSettings.plt_Formato = cmbFormati.Text;
            //if (cmbStili.SelectedIndex > -1) csSettings.plt_Stile = cmbStili.Text;
            //if (cmbScala.SelectedIndex > -1) csSettings.plt_Scala = cmbScala.Text;
            //csSettings.plt_bCentraPagina = true;
            //csSettings.Save();
        }

        /// <summary>
        /// Aggiungi messaggio a ListBox di LOG con indicazione oraria
        /// </summary>
        /// <param name="szMessage"></param>
        private void AddMessage(String szMessage)
        {
            // aggiungi messaggio con indicazione temporale se il messaggio è valido
            // altrimenti aggiungi una riga vuota, probabilmente utilizzata come separazione
            //if (szMessage.Length > 0)
            //    lstMessage.Items.Add("[" + DateTime.Now.ToShortTimeString() + "] " + szMessage);
            //else
            //    lstMessage.Items.Add("");

            //// scrolla sino al fondo
            //int nr = lstMessage.ClientSize.Height / lstMessage.ItemHeight;
            //if (nr < lstMessage.Items.Count)
            //{
            //    lstMessage.TopIndex = lstMessage.Items.Count - nr;
            //    lstMessage.Refresh();
            //}
        }

        private void AddMessage(String szFirstLine, String szLog, String szLastLine)
        {
            // aggiungi messaggio con indicazione temporale se il messaggio è valido
            // altrimenti aggiungi una riga vuota, probabilmente utilizzata come separazione
            //String szTime = DateTime.Now.ToShortTimeString();

            //lstMessage.Items.Add("[" + szTime + "] " + szFirstLine);

            //// stampa log riga per riga
            //String[] v = szLog.Split('\n');

            //foreach (String l in v)
            //    lstMessage.Items.Add(">> " );

            //// scrivi riga finale
            //lstMessage.Items.Add("[" + szTime + "] " + szLastLine);

            //// scrolla sino al fondo
            //int nr = lstMessage.ClientSize.Height / lstMessage.ItemHeight;
            //if (nr < lstMessage.Items.Count)
            //{
            //    lstMessage.TopIndex = lstMessage.Items.Count - nr;
            //    lstMessage.Refresh();
            //}
        }

        // analizza cartell ed aggiungi tutti i file .dwg in elenco file da elaborare
        private void RecurseFolder(DirectoryInfo dParent)
        {
            // carica file dwg presenti
            FileInfo[] v = dParent.GetFiles("*.dwg");

            foreach (FileInfo fi in v)
            {
                AddFileToList(lstFiles, fi);
            }

            // analizza subfolders
            DirectoryInfo[] d = dParent.GetDirectories();

            foreach (DirectoryInfo dSub in d)
            {
                RecurseFolder(dSub);
            }
        }

        private void AddFileToList(ListView lstFiles, FileInfo fi)
        {
            // riduci a percorso locale, se possibile
            String szFolder = (fi.DirectoryName);

            emissione.Main.path = szFolder;

            // controlla che l'elemento non esista già
            foreach (ListViewItem o in lstFiles.Items)
            {
                if ((String.Compare(o.Text, fi.Name, true) == 0) &&
                   (String.Compare(o.SubItems[1].Text, szFolder, true) == 0))
                    return;
            }

            // se non trovato, aggiungi a lista
            ListViewItem itm = lstFiles.Items.Add(fi.Name);
            itm.SubItems.Add(szFolder);
            itm.SubItems.Add("");
        }

        private void lstFiles_DragEnter(object sender, DragEventArgs e)
        {
            // filtra drag a soli file .dwg
            e.Effect = DragDropEffects.None;
            if (m_IsBusy) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else if (e.Data.GetDataPresent("System.String[]"))
            {
                // elenco file provenienti da altre applicazioni interne
                e.Effect = DragDropEffects.Link;
            }
        }

        private void lstFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (m_IsBusy) return;

            String[] vFiles;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                vFiles = (String[])e.Data.GetData(DataFormats.FileDrop);
            }
            else if (e.Data.GetDataPresent("System.String[]"))
            {
                // elenco file provenienti da altre applicazioni interne
                vFiles = (String[])e.Data.GetData("System.String[]");
            }
            else return;

            if (vFiles != null)
            {
                foreach (String s in vFiles)
                {
                    if (s.Length > 0)
                    {
                        FileInfo fi = new FileInfo(s);
                        if (fi.Exists)
                        {
                            if (fi.Extension.ToLower() == ".dwg")
                            {
                                AddFileToList(lstFiles, fi);
                            }
                        }
                        else
                        {
                            // forse è una cartella
                            DirectoryInfo d = new DirectoryInfo(s);
                            if (d.Exists)
                            {
                                // aggiungi file ricorsivamente
                                RecurseFolder(d);
                            }
                        }
                    }
                }
                UpdateCount();
            }
        }

        // riempi lista programmaticamente da elenco esterno
        public void FillList(List<String> vFiles)
        {
            if (vFiles != null)
            {
                // azzera elenco corrente
                lstFiles.Items.Clear();

                // cicla elenco e aggiungi file validi
                foreach (String s in vFiles)
                {
                    FileInfo fi = new FileInfo(s);
                    if (fi.Exists)
                    {
                        if (fi.Extension.ToLower() == ".dwg")
                        {
                            AddFileToList(lstFiles, fi);
                        }
                    }
                    else
                    {
                        // forse è una cartella
                        DirectoryInfo d = new DirectoryInfo(s);
                        if (d.Exists)
                        {
                            // aggiungi file ricorsivamente
                            RecurseFolder(d);
                        }
                    }
                }
                UpdateCount();
            }
        }

        private void lstFiles_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (lstFiles.TopItem != null))
            {
                if ((e.X < 3) || (e.Y < (lstFiles.TopItem.Position.Y + 3)) || (e.X > (lstFiles.ClientSize.Width - 3)) || (e.Y > (lstFiles.ClientSize.Height - 3)))
                {
                    // movimento del mouse vicino ai bordi con pulsante sinistro premuto
                    // avvia drag
                    if (lstFiles.SelectedItems.Count == 0) return;

                    // componi stringa con tutti i file selezionati
                    String[] szPayLoad = new String[lstFiles.SelectedItems.Count];

                    int i = 0;
                    foreach (ListViewItem itm in lstFiles.SelectedItems)
                    {
                        szPayLoad[i++] = itm.SubItems[1].Text + "\\" + itm.SubItems[0].Text; ;
                    }

                    lstFiles.DoDragDrop(szPayLoad, DragDropEffects.Link);
                }
            }
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            // azzera stato precedente per consentire riesecuzione comendo con la stessa selezione
            foreach (ListViewItem itm in lstFiles.Items)
            {
                itm.SubItems[2].Text = "";
                itm.StateImageIndex = -1;
            }
        }

        private void cmdSaveList_Click(object sender, EventArgs e)
        {
            if (dlgSaveList.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // salva elenco corrente con nome
                SaveFileList(dlgSaveList.FileName);
            }
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
        }

        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAvvia_Click_1(object sender, EventArgs e)
        {

            {
                // se l'elaborazione è già in corso, il pulsante funziona da Annulla
                if (m_IsBusy)
                {
                    AddMessage("    *** Ricevuta richiesta di annullamento. Attendere completamento file corrente. ***");
                    m_Cancel = true;

                    return;
                }

                // altrimenti siamo in condizioni normali. il pulsante funziona da Avvio
                errorProvider1.Clear();
                m_Cancel = false;

                // salva configurazione corrente
                SaveSettings();

                if (lstFiles.Items.Count == 0)
                {
                    AddMessage("*** Nessun file selezionato per l'elaborazione multipla ***");
                    return;
                }

                // avvia elaborazione
                //lstMessage.Items.Clear();
                AddMessage("Inizio elaborazione.");

                btnAvvia.Text = "Annulla";
                m_IsBusy = true;


                //pBar.Value = 0;
                //pBar.Visible = true;

                List<string> listaFile = new List<string>();

                int nFiles = 0;
                foreach (ListViewItem itm in lstFiles.Items)
                {

                    if (itm.SubItems[2].Text == "")
                    {
                        // aggiorna progress bar
                        //pBar.Value = nFiles * 100 / lstFiles.Items.Count;

                        // assicura che la riga sia visibile, scroll se necessario
                        lstFiles.EnsureVisible(itm.Index);

                        // processa messaggi in coda. L'elaborazione può essere lunga
                        //Application.DoEvents();
                        string filename = itm.SubItems[1].Text + "\\" + itm.SubItems[0].Text;

                        listaFile.Add(filename);

                        //csAdskToRFA.Esegui(m_commandData, IPTconverter.dwgtoADSK(filename));

                        // controlla se è stato premuto Annulla
                        if (m_Cancel)
                        {
                            AddMessage("");
                            AddMessage("*** Annullato dall'utente ***");
                            break;
                        }

                        // Elabora singolo file
                        AddMessage("Elaborazione " + itm.Text + " ...");

                        nFiles++;
                    }
                }

                emissione.Main.processo(listaFile);

                //pBar.Visible = false;
                btnAvvia.Text = "Avvia";
                m_IsBusy = false;

                AddMessage("Elaborazione terminata.");
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {

        }

        public void gestiscoErrori(IDictionary errore)
        {
            foreach(var file in lstFiles.Items)
            {
                string nomeFile = ((System.Windows.Forms.ListViewItem)file).Text;

                if (errore.Contains(nomeFile))
                {
                    IDictionary<string, bool> e = (IDictionary<string, bool>)errore[nomeFile];

                    int totale = 2;

                    int parziale = 0;

                    List<string> errName = new List<string>();

                    foreach (KeyValuePair<string, bool> es in e)
                    {
                        if (es.Value == false)
                        {
                            parziale += 1;

                            errName.Add(es.Key);
                        }
                    }

                    int errToShow = 2;

                    if (parziale == totale)
                    {
                        errToShow = 2;
                    }
                    else
                    {
                        foreach(string ne in errName)
                        {
                            if (ne == "tb")
                            {
                                errToShow = 0;
                            }

                            if (ne == "lb")
                            {
                                errToShow = 1;
                            }
                        }
                    }

                    ((System.Windows.Forms.ListViewItem)file).ImageIndex = errToShow;
                }
                else
                {
                    ((System.Windows.Forms.ListViewItem)file).Remove();
                }
            }
        }

        private void toolStripStatusLabel4_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click_1(object sender, EventArgs e)
        {

        }

    }
}
