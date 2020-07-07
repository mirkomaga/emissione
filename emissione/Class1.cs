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


namespace emissione
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PaletteMethod : Attribute { }
    public class AnalizzaDwg
    {
        string[] fileArray = null;

        private static PaletteSet _ps = null;

        private static Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        private static Editor ed = doc.Editor;
        private static Database db = doc.Database;


        [PaletteMethod]
        [CommandMethod("emissione")]
        public void Paletta()
        {

            string path = this.ChooseDirectory();

            try
            {
                this.fileArray = Directory.GetFiles(@path, "*.dwg", SearchOption.AllDirectories);

                foreach (string nomeFile in this.fileArray)
                {
                    System.Diagnostics.Debug.WriteLine(nomeFile);
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
    }
}
