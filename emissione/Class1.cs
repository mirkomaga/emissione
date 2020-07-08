using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.IO;
using Autodesk.AutoCAD.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Autodesk.AutoCAD.Geometry;
using System.Windows.Forms;
using WinForms = System.Windows.Forms;
using Autodesk.AutoCAD.Interop;

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

                    var layerName = "text";
                    this.CercaLunghezzaBarra(nuovoDocumento, layerName);

                    //this.CloseDocuments(nuovoDocumento.Name);
                    break;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Directory non trovata!");
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

        private void CercaLunghezzaBarra(Document nomeFile, string layerName)
        {
            var dbTmp = nomeFile.Database;

            using (var tr = dbTmp.TransactionManager.StartOpenCloseTransaction())
            {
                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                foreach (ObjectId btrId in blockTable)
                {
                    var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                    var textClass = RXObject.GetClass(typeof(DBText));
                    if (btr.IsLayout)
                    {
                        foreach (ObjectId id in btr)
                        {
                            System.Diagnostics.Debug.WriteLine(System.StringComparison.CurrentCultureIgnoreCase);
                            if (id.ObjectClass == textClass)
                            {
                                var text = (DBText)tr.GetObject(id, OpenMode.ForRead);
                                System.Diagnostics.Debug.WriteLine(text);
                                //if (text.Layer.Equals(layerName, System.StringComparison.CurrentCultureIgnoreCase))
                                //{
                                //yield return id;
                                //}
                            }
                        }
                    }
                }
            }
        }

        [CommandMethod("ListaBlocchi")]
        public void ListarBloques()
        {
            Database acCurDb;
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            acCurDb = doc.Database;
            Editor ed = doc.Editor;
            dynamic bt = db.BlockTableId;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                ObjectId blkRecId = ObjectId.Null;

                foreach (var singolo in acBlkTbl)
                {
                    System.Diagnostics.Debug.WriteLine(singolo);
                }

                //if (acBlkTbl.Has("RamProject_39305"))
                //{
                //    System.Diagnostics.Debug.WriteLine("si!");
                //}
            }

            //var textEnts = from btrs in (IEnumerable<dynamic>)bt

            //               from ent in (IEnumerable<dynamic>)btrs

            //               where

            //               ((ent.IsKindOf(typeof(DBText)) &&

            //                 (ent.TextString.Contains(str))) ||

            //               (ent.IsKindOf(typeof(MText)) &&

            //                 (ent.Contents.Contains(str))))

            //               select ent;

            //using (Transaction tr = db.TransactionManager.StartTransaction())
            //{
            //    var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

            //    var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);

            //    foreach (ObjectId id in modelSpace)
            //    {
            //        if (id.ObjectClass.DxfName == "INSERT")
            //        {
            //            var blockReference = (BlockReference)tr.GetObject(id, OpenMode.ForRead);

            //            BlockTableRecord acBlkTblRec = tr.GetObject(id, OpenMode.ForRead) as BlockTableRecord;

            //            ed.WriteMessage("\n" + blockReference.Name);
            //            System.Diagnostics.Debug.WriteLine(acBlkTblRec);

            //        }
            //    }

            //    tr.Commit();
            //}
        }
    }
}
