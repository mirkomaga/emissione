using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Diagnostics;
using Autodesk.AutoCAD.Interop.Common;

namespace emissione
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PaletteMethod : Attribute { }
    public class AnalizzaDwg
    {
        string[] fileArray = null;

        private static PaletteSet _ps = null;

        private static DocumentCollection docCollect = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
        private static Document doc = docCollect.MdiActiveDocument;
        private static Editor ed = doc.Editor;
        private static Database db = doc.Database;

        [PaletteMethod]
        [CommandMethod("emissione")]
        public void init()
        {

            string path = this.ChooseDirectory();

            try
            {
                this.fileArray = Directory.GetFiles(@path, "*.dwg", SearchOption.AllDirectories);

                foreach (string nomeFile in this.fileArray)
                {
                    var nuovoDocumento = this.LockDoc(nomeFile);

                    var listaBlocchi = this.ListaBlocchi();

                    var testoToFind = "prova";
                    //this.findTestoFromTag(listaBlocchi, testoToFind);

                    break;
                }
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Erroraccio: " + e);
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

        private Document LockDoc(string nomeFile)
        {
            // Create a new drawing

            DocumentCollection acDocMgr = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

            Document acNewDoc = acDocMgr.Add(nomeFile);

            Database acDbNewDoc = acNewDoc.Database;

            // Lock the new document

            using (DocumentLock acLckDoc = acNewDoc.LockDocument())

            {
                // Start a transaction in the new database

                using (Transaction acTrans = acDbNewDoc.TransactionManager.StartTransaction())

                {

                    // Open the Block table for read

                    BlockTable acBlkTbl;

                    acBlkTbl = acTrans.GetObject(acDbNewDoc.BlockTableId, OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write

                    BlockTableRecord acBlkTblRec;

                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                }

            }

            acDocMgr.MdiActiveDocument = acNewDoc;

            return acNewDoc;
        }

        private void CloseDocuments(string nomeFile)
        {
            foreach (Document docTmp in Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager)
            {
                //System.Diagnostics.Debug.WriteLine(docTmp.Name);
                //System.Diagnostics.Debug.WriteLine(docTmp.IsActive);

                if (!docTmp.IsActive && docTmp.Name != nomeFile)
                {
                    docTmp.CloseAndDiscard();
                }
            }
        }
        private List<BlockReference> ListaBlocchi()
        {
            List<BlockReference> result = new List<BlockReference>();

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;

                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

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

        [CommandMethod("teseeeer")]
        public void test()
        {
            var listaBlocchi = this.ListaBlocchi();

            string testoToFind = "PROGRESSIVO";
            var lunghezzaBarraMax = this.findlunghezzaBarraMax(listaBlocchi, testoToFind);

            string testo = "RICAVARE DA BARRA";
            object totaleBarra = this.findLunghezzaBarra(listaBlocchi, testo);

            List<string> valori = new List<string>() { "COMMESSA", "ARTICOLO" };

            IDictionary titolo = this.findTitolo(listaBlocchi, valori);

            List<Table> tabella = this.findTabella();

            foreach (Table tbl in tabella)
            {
                IDictionary intestatura = this.getIntestaturaTabella(tbl);

                var toFind = "POS";
                if (this.findInTable(intestatura, toFind))
                {
                    int posizione = (int) intestatura[toFind];
                    List<string> pos = this.GetTableIndice(tbl, posizione);
                    
                    foreach(string p in pos)
                    {
                        NamePadre namePadre = new NamePadre ((string) titolo["COMMESSA"], (string) titolo["ARTICOLO"], p );

                        double quantita = this.calcoloQuantita(totaleBarra, lunghezzaBarraMax);
                    }
                }
            }

            //System.Diagnostics.Debug.WriteLine(lunghezzaBarraMax);
            //System.Diagnostics.Debug.WriteLine(totaleBarra);
            //System.Diagnostics.Debug.WriteLine(titolo);
        }

        private double calcoloQuantita(object lunghezza, double lunghezzaBarraMax)
        {
            System.Diagnostics.Debug.WriteLine(lunghezza.lung);
            return 0.0;
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
        private Boolean findInTable(IDictionary intestatura, string toFind)
        {
            var keys = intestatura.Contains(toFind);

            if (keys)
            {
                return true;
            }

            return false;
        }
        private object findLunghezzaBarra(List<BlockReference> listaBlocchi, string testo)
        {
            List<double> result = new List<double>();

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
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
                                try
                                {
                                    string toAnalize = mtext.Text;

                                    if (toAnalize.Contains(testo))
                                    {
                                        String[] split = toAnalize.Split("/".ToCharArray());

                                        var res = new { lunghezza = Convert.ToDouble(split[2]), testo = mtext.Text };

                                        return res;
                                    }
                                }
                                catch (System.Exception e)
                                {
                                    System.Diagnostics.Debug.WriteLine("Erroraccio: " + e);
                                }
                            }
                            else if (ent.GetType() == typeof(DBText))
                            {
                                DBText dbtext = (DBText)acTrans.GetObject(obj, OpenMode.ForRead);
                                ed.WriteMessage("Text-Type: " + ent.GetType().ToString() + "   |   Text-String: " + dbtext.TextString + "\n");
                            }
                        }
                    }
                }
            }
            return new { };
        }
        private double findlunghezzaBarraMax(List<BlockReference> listaBlocchi, string testoToFind)
        {
            List<double> result = new List<double>();

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                foreach (BlockReference br in listaBlocchi)
                {
                    foreach (ObjectId id in br.AttributeCollection)
                    {
                        AttributeReference attRef = (AttributeReference)acTrans.GetObject(id, OpenMode.ForRead);
                        try
                        {
                            if (attRef.Tag == testoToFind)
                            {
                                result.Add(Convert.ToDouble(attRef.TextString));
                            }
                        }
                        catch (System.Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Error: " + e);
                        }
                    }
                }
            }

            try
            {
                double max = result.Max();
                return max;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e);
            }
            return 0.0;
        }
        private IDictionary findTitolo(List<BlockReference> listaBlocchi, List<string> testoToFind)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
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

        private List<Table> findTabella()
        {
            List<Table> result = new List<Table>();

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;

                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

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

            return (IDictionary) result;
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

        public override string ToString() => commessa+ articolo + "-"+pos;
    }
}