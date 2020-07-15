using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace emissione
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PaletteMethod : Attribute { }
    public class AnalizzaDwg
    {
        string[] fileArray = null;

        private static PaletteSet _ps = null;

        private static DocumentCollection docCollect = null;
        private List<string> pos;
        private IDictionary titolo;
        private IDictionary totaleBarra;
        private IDictionary<int, List<string>> dataExcelD;
        private double lunghezzaBarraMax;
        public string path;
        Form1 frm = new Form1();

        [PaletteMethod]
        [CommandMethod("emissione")]
        public void init()
        {
            docCollect = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

            dataExcelD = new Dictionary<int, List<string>>();

            this.path = this.ChooseDirectory();

            try
            {
                this.fileArray = Directory.GetFiles(this.path, "*.dwg", SearchOption.AllDirectories);

                int prog = 0;

                frm.pb.Maximum = this.fileArray.Length;
                frm.Show();
                frm.lblFile.Update();
                frm.Update();


                foreach (string nomeFile in this.fileArray)
                {
                    prog += 1;

                    Database db = new Database(false, true);

                    System.Diagnostics.Debug.WriteLine(prog+"/"+this.fileArray.Length);

                    //frm.counter(prog, this.fileArray.Length);
                    frm.pb.Value = prog;
                    frm.lblFile.Text = "Nome file: " + System.IO.Path.GetFileName(nomeFile);
                    frm.lblAnalisi.Text = "Analisi file "+prog+" di "+ this.fileArray.Length;
                    frm.lblFile.Update();
                    frm.Update();

                    using (db)
                    {
                        try
                        {
                            db.ReadDwgFile(nomeFile, System.IO.FileShare.Read, false, "");
                            var listaBlocchi = this.ListaBlocchi(db);

                            string testoToFind = "PROGRESSIVO";
                            lunghezzaBarraMax = this.findlunghezzaBarraMax(db, listaBlocchi, testoToFind);

                            if (lunghezzaBarraMax != 0)
                            {
                                string testo = "RICAVARE DA BARRA";
                                totaleBarra = this.findLunghezzaBarra(db, listaBlocchi, testo);
                            }


                            List<string> valori = new List<string>() { "COMMESSA", "ARTICOLO" };
                            titolo = this.findTitolo(db, listaBlocchi, valori);

                            List<Table> tabella = this.findTabella(db);

                            foreach (Table tbl in tabella)
                            {
                                IDictionary intestatura = this.getIntestaturaTabella(tbl);

                                var toFind = "POS";
                                if (this.findInTable(intestatura, toFind))
                                {
                                    int posizione = (int)intestatura[toFind];
                                    pos = this.GetTableIndice(tbl, posizione);
                                }
                            }
                        }
                        catch(System.Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Errore nella lettura file: " + e);
                        }
                    }

                    if (pos.Count > 0 && !string.IsNullOrEmpty(lunghezzaBarraMax.ToString()) && lunghezzaBarraMax != 0 && totaleBarra.Contains("lunghezza"))
                    {
                        string p = pos[0];

                        var lunghezzaTot = Convert.ToDouble(totaleBarra["lunghezza"]);

                        var quantita = new LunghezzaPercentuale((double)lunghezzaTot, (double)lunghezzaBarraMax);

                        this.createKeyIfExistDataExcel(new List<int>() { 1, 2, 3, 5 });

                        dataExcelD[1].Add("1");
                        dataExcelD[2].Add((new NamePadre(commessaP: (string)titolo["COMMESSA"], articoloP: (string)titolo["ARTICOLO"], posP: p)).ToString());
                        dataExcelD[3].Add((string)totaleBarra["testo"]);
                        dataExcelD[5].Add(quantita.ToString());

                    }
                    else
                    {
                        string p = pos[0];

                        this.createKeyIfExistDataExcel(new List<int>() { 1, 2, 3, 5 });

                        dataExcelD[1].Add("1");
                        dataExcelD[2].Add((new NamePadre(commessaP: (string)titolo["COMMESSA"], articoloP: (string)titolo["ARTICOLO"], posP: p)).ToString());
                        dataExcelD[3].Add("");
                        dataExcelD[5].Add("");
                    }
                }

                frm.lblFile.Text = "Compilazione excel in corso.";
                
                frm.pb.Visible = false;
                frm.lblAnalisi.Visible = false;

                this.gestiscoExcel(dataExcelD);

                frm.Close();
                frm.Dispose();

                System.Windows.Forms.MessageBox.Show("Estrazione lunghezza barre", "Operazione conclusa.", MessageBoxButtons.OK);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Erroraccio: " + e);
            }


        }
        private void createKeyIfExistDataExcel(List<int> colonna) 
        {
            foreach (int col in colonna)
            {
                if (!dataExcelD.ContainsKey(col))
                {
                    dataExcelD.Add(col, new List<string>());
                }
            }
        }
        private string ChooseDirectory()
        {
            string path;
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                path = dialog.SelectedPath;
            }
            return path;
        }
        private List<Table> findTabella(Database db)
        {
            List<Table> result = new List<Table>();

            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;

                acBlkTbl = acTrans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                foreach (ObjectId asObjId in acBlkTblRec)
                {
                    Entity elem = (Entity)acTrans.GetObject(asObjId, OpenMode.ForRead);

                    if (elem.GetType() == typeof(Table))
                    {
                        Table tbl = (Table)acTrans.GetObject(asObjId, OpenMode.ForRead);
                        result.Add(tbl);
                    }
                }
            }

            return result;
        }
        private List<BlockReference> ListaBlocchi(Database db)
        {
            List<BlockReference> result = new List<BlockReference>();
 
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;

                acBlkTbl = acTrans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                foreach (ObjectId asObjId in acBlkTblRec)
                {
                    Entity elem = (Entity)acTrans.GetObject(asObjId, OpenMode.ForRead);

                    if (elem.GetType() == typeof(BlockReference))
                    {
                        BlockReference br = (BlockReference)acTrans.GetObject(asObjId, OpenMode.ForRead);
                        result.Add(br);
                    }
                }
            }

            return result;
        }
        private List<string> GetTableIndice(Table tabella, int indice)
        {
            List<string> result = new List<string>();

            for (int r = 0; r < tabella.Rows.Count; r++)
            {
                if (r != 0)
                {
                    var cella = tabella.Cells[r, indice];
                    result.Add(cella.GetTextString(FormatOption.ForEditing));
                }
            }

            return result;
        }
        private double findlunghezzaBarraMax(Database db, List<BlockReference> listaBlocchi, string testoToFind)
        {
            List<double> result = new List<double>();

            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                foreach (BlockReference br in listaBlocchi)
                {
                    foreach (ObjectId id in br.AttributeCollection)
                    {
                        AttributeReference attRef = (AttributeReference)acTrans.GetObject(id, OpenMode.ForRead);
                        if (attRef.Tag == testoToFind)
                        {
                            result.Add(Convert.ToDouble(attRef.TextString));
                        }
                    }
                }
            }

            double max = 0;
            if (result.Count >= 1)
            {
                max = result.Max();
            }

            return max;
        }
        private Boolean findInTable(IDictionary intestatura, string toFind)
        {
            var keys = intestatura.Contains(toFind);

            if (keys)
            {
                return true;
            }

            return false;
        }
        private IDictionary findLunghezzaBarra(Database db, List<BlockReference> listaBlocchi, string testo)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                foreach (BlockReference br in listaBlocchi)
                {
                    BlockTableRecord acBlkTblRec = (BlockTableRecord)acTrans.GetObject(br.BlockTableRecord, OpenMode.ForRead);

                    foreach (ObjectId obj in acBlkTblRec)
                    {
                        Entity ent = (Entity)obj.GetObject(OpenMode.ForRead, false, true);
                        if (ent != null)
                        {
                            //ed.WriteMessage("Type: " + ent.GetType().ToString() + "\n");

                            if (ent.GetType() == typeof(MText))
                            {
                                MText mtext = (MText)acTrans.GetObject(obj, OpenMode.ForRead);
                                string toAnalize = mtext.Text;

                                if (toAnalize.Contains(testo))
                                {
                                    String[] split = toAnalize.Split("/".ToCharArray());


                                    result.Add("lunghezza", (Convert.ToDouble(split[2]).ToString()));

                                    String[] testoCompleto = toAnalize.Split(" ".ToCharArray());

                                    result.Add("testo", (testoCompleto[testoCompleto.Length - 1]).ToString());

                                    return (IDictionary)result;
                                }
                            }
                            else if (ent.GetType() == typeof(DBText))
                            {
                                DBText dbtext = (DBText)acTrans.GetObject(obj, OpenMode.ForRead);
                                ///*ed*/.WriteMessage("Text-Type: " + ent.GetType().ToString() + "   |   Text-String: " + dbtext.TextString + "\n");
                            }
                        }
                    }
                }
            }
            return (IDictionary)result;
        }
        private IDictionary findTitolo(Database db, List<BlockReference> listaBlocchi, List<string> testoToFind)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                foreach (BlockReference br in listaBlocchi)
                {
                    foreach (ObjectId id in br.AttributeCollection)
                    {
                        AttributeReference attRef = (AttributeReference)acTrans.GetObject(id, OpenMode.ForRead);
                        try
                        {
                            if ((testoToFind).Contains(attRef.Tag))
                            {
                                result.Add(attRef.Tag, attRef.TextString);
                            }
                        }
                        catch (System.Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Error: " + e);
                        }
                    }
                }
            }

            return (IDictionary)result;
        }
        public IDictionary getIntestaturaTabella(Table tabella)
        {
            IDictionary<string, int> result = new Dictionary<string, int>();

            for (int r = 0; r < tabella.Rows.Count; r++)
            {
                var cella = tabella.Cells[r, 0];
                try
                {
                    result.Add(cella.GetTextString(FormatOption.ForEditing), r);
                }
                catch
                {

                }
            }

            return (IDictionary)result;
        }
        public void gestiscoExcel(IDictionary<int, List<string>> data)
        {
            IDictionary<string, dynamic> dataExcel = new Dictionary<string, dynamic>();

            dataExcel.Add("percorso", this.path+"/");
            dataExcel.Add("nomeFile", "commessa");

            IDictionary<int, string> intestaturaToExc = new Dictionary<int, string>();
            intestaturaToExc[1] = "Posizione";
            intestaturaToExc[2] = "Padre";
            intestaturaToExc[3] = "Figlio";
            intestaturaToExc[4] = "Descrizione";
            intestaturaToExc[5] = "Quantità";
            intestaturaToExc[6] = "Destinazione";


            dataExcel.Add("intestatura", intestaturaToExc);
            
            dataExcel.Add("data", data);

            var a = new Excel();
            a.WriteSample(dataExcel);
        }
    }
    public class NamePadre
    {
        private string commessaP;
        private string articoloP;
        private string posP;
        public NamePadre(string commessaP, string articoloP, string posP)
        {
            commessa = commessaP;
            articolo = articoloP;
            pos = posP;
        }
        public string commessa { get; set; }
        public string articolo { get; set; }
        public string pos { get; set; }
        public override string ToString() => commessa + articolo + "-" + pos;
    }
    public class LunghezzaPercentuale
    {
        private double lunghezza;
        private double lunghezzaBarraMax;
        //private double result = 0;
        private IDictionary<double, double> tmpMisure;
        private Boolean maggiore = false;
        private Boolean minore = false;
        public LunghezzaPercentuale(double lunghezzaP, double lunghezzaBarraMaxP)
        {
            this.lunghezza = lunghezzaP;
            this.lunghezzaBarraMax = lunghezzaBarraMaxP;

            if (lunghezzaP != 0 && lunghezzaBarraMaxP !=0) 
            { 
                this.tmpMisure = (IDictionary<double, double>) this.misureBlindate();
                this.result = this.findResult();
            }
            else
            {
                this.result = 0;
            }

        }
        public double findResult()
        {
            double res = 0.5;
            for (int i = this.tmpMisure.Count-1; i >= 0; i--)
            {
                try
                {
                    double toCheck = this.tmpMisure.ElementAt(i).Key;
                    if(this.lunghezzaBarraMax <= toCheck)
                        {
                        return (double) this.tmpMisure.ElementAt(i).Value;
                    }
                        
                }
                catch(System.Collections.Generic.KeyNotFoundException e)
                {
                    System.Diagnostics.Debug.WriteLine("Chiave non trovata");
                }
            }

            return (double) res;
        }
        private IDictionary misureBlindate()
        {
            IDictionary<double, double> risultato = new Dictionary<double, double>();

            List<double> valori = new List<double>() {0.5,
                0.333,
                0.25,
                0.2,
                0.167,
                0.143,
                0.125,
                0.111,
                0.1,
                0.091,
                0.083,
                0.077,
                0.071,
                0.067,
                0.063,
                0.059,
                0.056,
                0.053,
                0.05,
                0.048,
                0.045,
                0.043,
                0.042,
                0.04,
                0.38,
                0.037,
                0.036,
                0.034,
                0.033,
                0.032,
                0.031,
                0.03,
                0.029,
                0.028,
                0.027,
                0.026,
                0.025 };

            foreach (double tmp in valori) 
            {
                risultato[Math.Round(tmp * this.lunghezza, 0)] = tmp;
            }

            return (IDictionary) risultato;
        }
        public double result { get; set; }
        public override string ToString() => this.result.ToString();
    }
}