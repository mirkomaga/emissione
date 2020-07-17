using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using EdgeCode;
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
        public static frmBatch frmMain = new frmBatch();

        [PaletteMethod]
        [CommandMethod("emissione")]
        public void init()
        {
            try
            {
                frmMain.Show();
            }
            catch
            {
                frmMain.Dispose();
                frmMain = new frmBatch();
                frmMain.Show();
            }
        }
    }

    public static class Main
    {
        private static string[] fileArray = null;
        private static PaletteSet _ps = null;
        private static DocumentCollection docCollect = null;
        private static List<string> pos;
        private static IDictionary titolo;
        public static IDictionary errore;
        private static IDictionary<int, List<string>> dataExcelD;
        private static IDictionary totaleBarra;
        private static double lunghezzaBarraMax;
        public static string path;
        private static Form1 frm = new Form1();

        private static bool tbStatus;
        private static bool lbStatus;

        public static void processo(List<string> listaFile)
        {

            docCollect = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

            dataExcelD = new Dictionary<int, List<string>>();
            errore = new Dictionary<string, IDictionary>();

            int prog = 0;

            frm.pb.Maximum = listaFile.Count;

            //try
            //{
            //    frm.Show();
            //}
            //catch
            //{
            //    System.Diagnostics.Debug.WriteLine("Form già aperto");
            //}

            //frm.lblFile.Update();
            //frm.Update();


            foreach (string nomeFile in listaFile)
            {
                prog += 1;

                tbStatus = false;
                lbStatus = false;

                Database db = new Database(false, true);

                var form = emissione.AnalizzaDwg.frmMain;

                form.tsPd.Maximum = (int) listaFile.Count;
                form.tsPd.Minimum = (int) 0;
                form.tsPd.Value = (int) prog;
                form.nameFile.Text = (string)System.IO.Path.GetFileName(nomeFile);
                form.counter.Text = prog + "/" + listaFile.Count;
                form.tsPd.Width = form.statusBot.Width - 30 - (form.lgnd1.Width + form.lgnd2.Width + form.lgnd3.Width + form.nameFile.Width + form.counter.Width);
                form.Refresh();

                using (db)
                {
                    db.ReadDwgFile(nomeFile, System.IO.FileShare.Read, false, "");
                    var listaBlocchi = ListaBlocchi(db);

                    string testoToFind = "PROGRESSIVO";
                    lunghezzaBarraMax = findlunghezzaBarraMax(db, listaBlocchi, testoToFind);

                    string testo = "RICAVARE DA BARRA";
                    totaleBarra = findLunghezzaBarra(db, listaBlocchi, testo);

                    if (lunghezzaBarraMax > 0)
                    {
                        lbStatus = true;
                    }

                    if (!String.IsNullOrEmpty((string)totaleBarra["testo"]) && !String.IsNullOrEmpty((string)totaleBarra["lunghezza"]))
                    {
                        tbStatus = true;
                    }

                    List<string> valori = new List<string>() { "COMMESSA", "ARTICOLO" };
                    titolo = findTitolo(db, listaBlocchi, valori);

                    List<Table> tabella = findTabella(db);

                    foreach (Table tbl in tabella)
                    {
                        IDictionary intestatura = getIntestaturaTabella(tbl);

                        var toFind = "POS";
                        if (findInTable(intestatura, toFind))
                        {
                            int posizione = (int)intestatura[toFind];
                            pos = GetTableIndice(tbl, posizione);
                        }
                    }
                }

                if (pos.Count > 0 && tbStatus && lbStatus)
                {
                    string p = pos[0];

                    var lunghezzaTot = Convert.ToDouble(totaleBarra["lunghezza"]);

                    var quantita = new LunghezzaPercentuale((double)lunghezzaTot, (double)lunghezzaBarraMax);

                    createKeyIfExistDataExcel(new List<int>() { 1, 2, 3, 5 });

                    dataExcelD[1].Add("1");
                    dataExcelD[2].Add((new NamePadre(commessaP: (string)titolo["COMMESSA"], articoloP: (string)titolo["ARTICOLO"], posP: p)).ToString());
                    dataExcelD[3].Add((string)totaleBarra["testo"]);
                    dataExcelD[5].Add(quantita.ToString());

                    dataExcelD[1].Add("2");
                    dataExcelD[2].Add((new NamePadre(commessaP: (string)titolo["COMMESSA"], articoloP: (string)titolo["ARTICOLO"], posP: p)).ToString());
                    dataExcelD[3].Add("");
                    dataExcelD[5].Add("");

                    dataExcelD[1].Add("");
                    dataExcelD[2].Add("");
                    dataExcelD[3].Add("");
                    dataExcelD[5].Add("");

                }
                else
                {
                    string p = pos[0];

                    IDictionary<string, bool> e = new Dictionary<string, bool>();

                    if (lbStatus == false)
                    {
                        e.Add("lb", false);
                    }

                    if (tbStatus == false)
                    {
                        e.Add("tb", false);
                    }

                    errore.Add(System.IO.Path.GetFileName(nomeFile), e);

                    createKeyIfExistDataExcel(new List<int>() { 1, 2, 3, 5 });

                    dataExcelD[1].Add("1");
                    dataExcelD[2].Add((new NamePadre(commessaP: (string)titolo["COMMESSA"], articoloP: (string)titolo["ARTICOLO"], posP: p)).ToString());
                    dataExcelD[3].Add("");
                    dataExcelD[5].Add("");

                    dataExcelD[1].Add("");
                    dataExcelD[2].Add("");
                    dataExcelD[3].Add("");
                    dataExcelD[5].Add("");
                }
            }

            //frm.lblFile.Text = "Compilazione excel in corso.";
                
            //frm.pb.Visible = false;
            //frm.lblAnalisi.Visible = false;

            gestiscoExcel(dataExcelD);

            emissione.AnalizzaDwg.frmMain.gestiscoErrori((IDictionary)errore);

            //frm.Close();
            //frm.Dispose();

            //System.Windows.Forms.MessageBox.Show("Estrazione lunghezza barre", "Operazione conclusa.", MessageBoxButtons.OK);
        }
        private static void createKeyIfExistDataExcel(List<int> colonna) 
        {
            foreach (int col in colonna)
            {
                if (!dataExcelD.ContainsKey(col))
                {
                    dataExcelD.Add(col, new List<string>());
                }
            }
        }
        public static void gestiscoExcel(IDictionary<int, List<string>> data)
        {
            IDictionary<string, dynamic> dataExcel = new Dictionary<string, dynamic>();

            dataExcel.Add("percorso", path + "/");
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
        private static List<Table> findTabella(Database db)
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
        private static List<BlockReference> ListaBlocchi(Database db)
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
        private static List<string> GetTableIndice(Table tabella, int indice)
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
        private static double findlunghezzaBarraMax(Database db, List<BlockReference> listaBlocchi, string testoToFind)
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
        private static Boolean findInTable(IDictionary intestatura, string toFind)
        {
            var keys = intestatura.Contains(toFind);

            if (keys)
            {
                return true;
            }

            return false;
        }
        private static IDictionary findLunghezzaBarra(Database db, List<BlockReference> listaBlocchi, string testo)
        {
            // Cerco in tutto il modello il testo, MText e Text

            IDictionary<string, string> result = new Dictionary<string, string>();

            result["lunghezza"] = "";
            result["testo"] = "";

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


                                    result["lunghezza"] = (Convert.ToDouble(split[2]).ToString());

                                    String[] testoCompleto = toAnalize.Split(" ".ToCharArray());

                                    result["testo"] = testoCompleto[testoCompleto.Length - 1].ToString();

                                    return (IDictionary) result;
                                }
                            }
                            else if (ent.GetType() == typeof(DBText))
                            {
                                DBText dbtext = (DBText)acTrans.GetObject(obj, OpenMode.ForRead);
                            }
                        }
                    }
                }
            }
            return (IDictionary)result;
        }
        private static IDictionary findTitolo(Database db, List<BlockReference> listaBlocchi, List<string> testoToFind)
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
        public static IDictionary getIntestaturaTabella(Table tabella)
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
            lunghezza = lunghezzaP;
            lunghezzaBarraMax = lunghezzaBarraMaxP;

            if (lunghezzaP != 0 && lunghezzaBarraMaxP !=0) 
            { 
                tmpMisure = (IDictionary<double, double>) misureBlindate();
                result = findResult();
            }
            else
            {
                result = 0;
            }

        }
        public double findResult()
        {
            double res = 1;
            for (int i = tmpMisure.Count-1; i >= 0; i--)
            {
                try
                {
                    double toCheck = tmpMisure.ElementAt(i).Key;
                    if(lunghezzaBarraMax <= toCheck)
                        {
                        return (double) tmpMisure.ElementAt(i).Value;
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

            List<double> valori = new List<double>() {
                0.5,
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
                0.038,
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
                0.025 
            };

            foreach (double tmp in valori) 
            {
                risultato[Math.Round(tmp * lunghezza, 0)] = tmp;
            }

            return (IDictionary) risultato;
        }
        public double result { get; set; }
        public override string ToString() => result.ToString();
    }
}