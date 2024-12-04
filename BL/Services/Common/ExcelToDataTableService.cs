using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class ExcelToDataTableService
    {
        public static DataTable GetDataTableFromExcel_data(IFormFile file)
        {
            // Temporary file path to save the uploaded file.
            using (var stream = new MemoryStream())
            {
                // Copy the uploaded file to the memory stream.
                file.CopyTo(stream);

                // Create a new ClosedXML workbook from the stream.
                using (var workBook = new XLWorkbook(stream))
                {
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    DataTable dt = new DataTable();
                    int RowCount = 0;

                    // Iterate through the rows of the worksheet.
                    foreach (IXLRow row in workSheet.RowsUsed())
                    {
                        if (RowCount == 0)
                        {
                            // Create columns for the DataTable.
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            RowCount = 1;
                        }
                        else
                        {
                            // Add rows to the DataTable.
                            dt.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }
                    }
                    return dt;
                }
            }
        }
    }
}
