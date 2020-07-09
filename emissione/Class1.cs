using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
            catch(System.Exception e)
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

            var testoToFind = "PROGRESSIVO";
            var lunghezzaBarraMax = this.findlunghezzaBarraMax(listaBlocchi, testoToFind);

            var testo = "RICAVARE DA BARRA";
            var totaleBarra = this.findLunghezzaBarra(listaBlocchi, testo);

            System.Diagnostics.Debug.WriteLine(lunghezzaBarraMax);
            System.Diagnostics.Debug.WriteLine(totaleBarra);
        }

        private double findLunghezzaBarra(List<BlockReference> listaBlocchi, string testo)
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

                                        var tot = Convert.ToDouble(split[2]);

                                        return tot;
                                    }
                                }
                                catch(System.Exception e)
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
            return 0.0;
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
                            if (attRef.Tag == testoToFind) { 
                                result.Add(Convert.ToDouble(attRef.TextString));
                            }
                        }
                        catch(System.Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Error: " + e);
                        }
                    }
                }
            }

            double max = result.Max();

            return max;

        }
    }
}
