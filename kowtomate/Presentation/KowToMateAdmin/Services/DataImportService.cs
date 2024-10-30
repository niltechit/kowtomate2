using ClosedXML.Excel;
using CutOutWiz.Data.DynamicReports;
using Radzen;
using static CutOutWiz.Core.Utilities.Enums;

namespace KowToMateAdmin.Services
{
    public class DataImportService : IDataImportService
    {
        public byte[] GenerateExcel(IEnumerable<IDictionary<string, object>> items, List<ReportTableColumn> columns, string sheetName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);

                // Get the property names for the header
                var headers = columns.Select(c => c.FieldName).ToArray();

                // Add table headers
                int i = 0;

                var haederBackgrondColor = XLColor.FromHtml("#AED6F1");

                foreach (var header in columns)
                {
                    header.TotalSum = 0;
                    worksheet.Cell(1, i+ 1).Value = header.DisplayName;

                    //Set font size
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;

                    //Set alignment
                    worksheet.Cell(1, i + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((TextAlign)header.TextAlign);

                    //Set width
                    if (!string.IsNullOrEmpty(header.Width))
                    {
                        worksheet.Column(i + 1).Width = ConvertPixelsToWidth(Convert.ToInt32(header.Width));
                    }
                    else
                    {
                        worksheet.Column(i + 1).Width = 20;
                    }

                    //Set Backgroud color
                    worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = haederBackgrondColor;

                    i++;
                }

                // Add table rows with data
                int rowIndex = 2;
                int columnIndex = 0;

                foreach (var item in items)
                {
                    columnIndex = 0;
                    foreach (var header in columns)
                    //for (int columnIndex = 0; columnIndex < headers.Length; columnIndex++)
                    {
                        //var columnName = headers[columnIndex];
                        if (item.ContainsKey(header.FieldName))
                        {
                            var value = item[header.FieldName];
                            string formattedValue;

                            if (header.FieldTypeEnum == TableFieldType.Integer)
                            {
                                formattedValue = value is decimal decimalValue ? decimalValue.ToString("N0") : value.ToString();

                                // Add the value to the total sum
                                if (decimal.TryParse(formattedValue, out decimal numericValue))
                                {
                                    header.TotalSum += numericValue;
                                }
                            }
                            else if (header.FieldTypeEnum == TableFieldType.Decimal)
                            {
                                formattedValue = value is decimal decimalValue ? decimalValue.ToString("N2") : value.ToString();

                                // Add the value to the total sum
                                if (decimal.TryParse(formattedValue, out decimal numericValue))
                                {
                                    header.TotalSum += numericValue;
                                }
                            }
                            else
                            {
                                formattedValue = value?.ToString();
                            }

                            worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                            //var value = item[header.FieldName]?.ToString();
                            //worksheet.Cell(rowIndex, columnIndex + 1).Value = value;
                        }

                        //Format
                        worksheet.Cell(rowIndex, columnIndex + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((TextAlign)header.TextAlign);

                        columnIndex++;
                    }

                    rowIndex++;
                }

                columnIndex = 0;

                // Add total cell for decimal columns
                foreach (var column in columns)
                {
                    if (column.FieldTypeEnum == TableFieldType.Integer)
                    {
                        worksheet.Cell(rowIndex, columnIndex + 1).Value = column.TotalSum.ToString("N0");
                    }
                    else if (column.FieldTypeEnum == TableFieldType.Decimal)
                    {
                        worksheet.Cell(rowIndex, columnIndex + 1).Value = column.TotalSum.ToString("N2");
                    }

                    worksheet.Cell(rowIndex, columnIndex + 1).Style.Font.Bold = true;
                    worksheet.Cell(rowIndex, columnIndex + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((TextAlign)column.TextAlign);
                    columnIndex++;
                }

                // Convert the workbook to a byte array
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        //public byte[] GenerateExcel(IEnumerable<IDictionary<string, object>> items, IDictionary<string, Type> columns, string sheetName)
        //{
        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add(sheetName);

        //        // Get the property names for the header
        //        var headers = columns.Keys.ToArray();

        //        // Add table headers
        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            worksheet.Cell(1, i + 1).Value = headers[i];
        //        }

        //        // Add table rows with data
        //        int rowIndex = 2;
        //        foreach (var item in items)
        //        {
        //            for (int columnIndex = 0; columnIndex < headers.Length; columnIndex++)
        //            {
        //                var columnName = headers[columnIndex];
        //                if (item.ContainsKey(columnName))
        //                {
        //                    var value = item[columnName]?.ToString();
        //                    worksheet.Cell(rowIndex, columnIndex + 1).Value = value;
        //                }
        //            }
        //            rowIndex++;
        //        }

        //        // Convert the workbook to a byte array
        //        using (var stream = new System.IO.MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            return stream.ToArray();
        //        }
        //    }
        //}

        public double ConvertPixelsToWidth(int pixels)
        {
            const int basePixels = 8; // Base pixel value used by ClosedXML.Excel
            const double baseWidth = 1.0; // Base width value used by ClosedXML.Excel

            return (pixels / basePixels) * baseWidth;
        }

        public XLAlignmentHorizontalValues GetAlignmentHorizontalValue(TextAlign textAlign)
        {
            if (textAlign == TextAlign.Left)
            {
                return XLAlignmentHorizontalValues.Left;
            }
            else if (textAlign == TextAlign.Right)
            {
                return XLAlignmentHorizontalValues.Right;
            }
            else if (textAlign == TextAlign.Center)
            {
                return XLAlignmentHorizontalValues.Center;
            }
            else if (textAlign == TextAlign.Justify)
            {
                return XLAlignmentHorizontalValues.Justify;
            }
            else
            {
                // Handle the case when the byte value doesn't correspond to a valid enum value
                // For example, you can return a default enum value or throw an exception
                return XLAlignmentHorizontalValues.Left; // Default value
            }
        }
    }
}
