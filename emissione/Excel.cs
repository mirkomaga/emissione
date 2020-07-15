using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;

namespace emissione
{
    class Excel
    {
        public void WriteSample(IDictionary<string, dynamic> data)
        {
            //Excel.Application excelApp = new Excel.Application();

            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

            if (excelApp != null)
            {
                Microsoft.Office.Interop.Excel.Workbook excelWorkbook = excelApp.Workbooks.Add();
                Microsoft.Office.Interop.Excel.Worksheet excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelWorkbook.Sheets.Add();

                this.WriteIntestatura(excelWorksheet, data["intestatura"]);
                this.WriteData(excelWorksheet, data["data"]);

                try
                {
                    excelApp.ActiveWorkbook.SaveAs(data["percorso"] + data["nomeFile"], Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);

                    excelWorkbook.Close();
                    excelApp.Quit();

                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelWorksheet);
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelWorkbook);
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    System.Diagnostics.Debug.WriteLine("Impossibile scrivere il file");
                }
            }
        }

        public void WriteIntestatura(Worksheet excelWorksheet, IDictionary<int, string> data)
        {
            foreach (var tmp in data)
            {
                excelWorksheet.Cells[1, tmp.Key] = tmp.Value;
            }
        }

        public void WriteData(Worksheet excelWorksheet, IDictionary<int, List<string>> data)
        {
            try
            {
                foreach (KeyValuePair<int, List<string>> kv in data)
                {
                    int colonna = kv.Key;
                    List<String> value = kv.Value;
                    var listEnumerator = value.GetEnumerator();

                    for (int i = 2; listEnumerator.MoveNext() == true; i++)
                    {
                        var valore = listEnumerator.Current;

                        System.Diagnostics.Debug.WriteLine(valore);
                        System.Diagnostics.Debug.WriteLine(i);
                        System.Diagnostics.Debug.WriteLine(colonna);

                        excelWorksheet.Cells[i, colonna] = valore;
                    }
                }
            }
            catch (System.InvalidCastException e)
            {
                System.Diagnostics.Debug.WriteLine("Problemi cast");
            }
        }
    }
}
