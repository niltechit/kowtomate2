using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.DynamicReports;
using ClosedXML.Excel;
using System.Text;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.ImportExport
{
    public class DataImportExportService : IDataImportExportService
    {
        #region Export from list
        /// <summary>
        /// Export from a list of object with ecceptcolumn
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetName"></param>
        /// <param name="items"></param>
        /// <param name="exceptColumns"></param>
        /// <returns></returns>
        public byte[] GenerateExcel<T>(string sheetName, List<T> items, List<string> exceptColumns)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);

                //// Add company information and logo
                //worksheet.Cell("A1").Value = companyName;
                //worksheet.Cell("A2").Value = "Phone: " + phone;
                //worksheet.Cell("A3").Value = "Address: " + address;

                //var logo = worksheet.AddPicture(logoPath)
                //                    .MoveTo(worksheet.Cell("C1"))
                //                    .WithSize(150, 50);

                // Get the property names for the header
                var properties = typeof(T).GetProperties();

                properties = properties.ToList().Where(f => !exceptColumns.Any(o => o == f.Name)).ToArray();

                var headers = properties.Select(p => p.Name).ToArray();

                // Add table headers
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1,i+1).Value = headers[i];
                }

                // Add table rows with data
                for (int rowIndex = 0; rowIndex < items.Count; rowIndex++)
                {
                    var item = items[rowIndex];
                    for (int columnIndex = 0; columnIndex < properties.Length; columnIndex++)
                    {
                        var value = properties[columnIndex].GetValue(item)?.ToString();
                        worksheet.Cell(rowIndex + 2, columnIndex + 1).Value = value;
                    }
                }

                // Convert the workbook to a byte array
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        #endregion

        #region Export Excel from Dictionary
        public async Task<byte[]> GenerateExcel(IEnumerable<IDictionary<string, object>> items, List<ReportTableColumnModel> columns, string sheetName, Action<int> progressCallback)
        {

            await Task.Yield();

            if (items == null || !items.Any())
            {
                return null;
            }

            int processedCount = 0;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);

                // Add table headers
                int i = 0;

                var haederBackgrondColor = XLColor.FromHtml("#C0C0C0");   //default = #AED6F1

                foreach (var header in columns)
                {
                    header.TotalSum = 0;
                    worksheet.Cell(1, i + 1).Value = header.DisplayName;

                    //Set font size
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;

                    //Set alignment
                    worksheet.Cell(1, i + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((CustomTextAlign)header.TextAlign);

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
                string formattedValue;
                foreach (var item in items)
                {
                    columnIndex = 0;
                    foreach (var header in columns)
                    {
                        if (item.ContainsKey(header.FieldName))
                        {
                            var value = item[header.FieldName];

                            formattedValue = "";

                            if (value == null)
                            {
                                formattedValue = value?.ToString();

                                worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                            }
                            else if (header.FieldTypeEnum == TableFieldTypeSm.Integer)
                            {
                                formattedValue = value is decimal decimalValue ? decimalValue.ToString(header.DispalyFormat) : value.ToString();

                                // Add the value to the total sum
                                //if (decimal.TryParse(formattedValue, out decimal numericValue))
                                //{
                                //    header.TotalSum += numericValue;
                                //}

                                worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                            }
                            else if (header.FieldTypeEnum == TableFieldTypeSm.Decimal)
                            {
                                formattedValue = value is decimal decimalValue ? decimalValue.ToString(header.DispalyFormat) : value.ToString();

                                //// Add the value to the total sum
                                //if (decimal.TryParse(formattedValue, out decimal numericValue))
                                //{
                                //    header.TotalSum += numericValue;
                                //}

                                worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                            }
                            else if (header.FieldTypeEnum == TableFieldTypeSm.Date)
                            {
                                formattedValue = value is DateTime decimalValue ? decimalValue.ToString(header.DispalyFormat) : value.ToString();
                                worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                            }
                            else if (header.FieldTypeEnum == TableFieldTypeSm.Boolean)
                            {
                                formattedValue = value is bool decimalValue && decimalValue == true ? "YES" : "NO";
                                worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                            }
                            else
                            {
                                formattedValue = $"{value}"; 
                                worksheet.Cell(rowIndex, columnIndex + 1).SetValue(formattedValue);
                            }


                            //Set Font Color
                            worksheet.Cell(rowIndex, columnIndex + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((CustomTextAlign)header.TextAlign);

                            if (!string.IsNullOrEmpty(header.TextColor))
                            {
                                worksheet.Cell(rowIndex, columnIndex + 1).Style.Font.FontColor = XLColor.FromHtml(header.TextColor);
                            }

                            //Set Background color
                            if (!string.IsNullOrEmpty(header.BackgroundColorRules))
                            {
                                if (header.BackgroundColorRules.Contains("="))
                                {
                                    //YES=#158419,NO=#F30F
                                    var bgColor = StringHelper.GetColorValueUsingKeyFormString(header.BackgroundColorRules, formattedValue);
                                    
                                    if (!string.IsNullOrEmpty(bgColor))
                                    {
                                        worksheet.Cell(rowIndex, columnIndex + 1).Style.Fill.PatternType = XLFillPatternValues.Solid;
                                        worksheet.Cell(rowIndex, columnIndex + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(bgColor);
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(header.BackgroundColor))
                            {
                                worksheet.Cell(rowIndex, columnIndex + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(header.BackgroundColor);
                            }

                        } //Data not found

                        columnIndex++;
                    }

                    rowIndex++;

                    
                    processedCount++;

                    if (processedCount % 500 == 0)
                    {
                        progressCallback?.Invoke(processedCount);
                    }
                }

                columnIndex = 0;

                //Add total cell for decimal columns
                if (columns.Any(f => f.ShowFooterTotal))
                {
                    foreach (var column in columns)
                    {
                        if (column.ShowFooterTotal)
                        {
                            if (column.FieldTypeEnum == TableFieldTypeSm.Integer)
                            {
                                long sum = 0;

                                foreach (var item in items)
                                {
                                    sum += Convert.ToInt64(item[column.FieldName]);
                                }
                                
                                worksheet.Cell(rowIndex, columnIndex + 1).Value = sum.ToString(column.DispalyFormat);
                            }

                            else if (column.FieldTypeEnum == TableFieldTypeSm.Decimal)
                            {
                                decimal sum = 0;

                                foreach (var item in items)
                                {
                                    sum += Convert.ToDecimal(item[column.FieldName]);
                                }

                                worksheet.Cell(rowIndex, columnIndex + 1).Value = sum.ToString(column.DispalyFormat);
                            }

                            worksheet.Cell(rowIndex, columnIndex + 1).Style.Font.Bold = true;
                            worksheet.Cell(rowIndex, columnIndex + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((CustomTextAlign)column.TextAlign);
                        }

                        columnIndex++;
                    }
                }

                progressCallback?.Invoke(processedCount - 3);

                // Set the font and border for all cells
                var allCells = worksheet.CellsUsed();
                var allCellStyle = allCells.Style;
                var allFont = allCellStyle.Font;
                allFont.FontName = "Calibri";
                allFont.FontSize = 10;
                //allCellStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
                //allCellStyle.Border.InsideBorder = XLBorderStyleValues.Thin;

                progressCallback?.Invoke(processedCount - 1);

                // Convert the workbook to a byte array
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> GenerateExcel(IEnumerable<IDictionary<string, object>> items, List<ReportTableColumnModel> columns, 
            string sheetName, Action<int> progressCallback, string groupingColumn)
        {
            await Task.Yield();
            int processedCount = 0;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);

                // Add table headers
                int i = 0;
                int columnIndex = 0;
                var haederBackgrondColor = XLColor.FromHtml("#C0C0C0");

                foreach (var header in columns)
                {
                    header.TotalSum = 0;
                    worksheet.Cell(1, i + 1).Value = header.DisplayName;

                    //Set font size
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;

                    //Set alignment
                    worksheet.Cell(1, i + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((CustomTextAlign)header.TextAlign);

                    //Set width
                    if (!string.IsNullOrEmpty(header.Width))
                    {
                        worksheet.Column(i + 1).Width = ConvertPixelsToWidth(Convert.ToInt32(header.Width));
                    }
                    else
                    {
                        worksheet.Column(i + 1).Width = 100;
                    }

                    //Set Backgroud color
                    worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = haederBackgrondColor;

                    i++;
                }

                // Grouped data dictionary to store items for each group
                var groupedData = new Dictionary<object, List<IDictionary<string, object>>>();

                // Group data based on the provided grouping column
                foreach (var item in items)
                {
                    var groupKey = item[groupingColumn];

                    if (groupKey == null || groupKey?.ToString() == "")
                    {
                        groupKey = "-";
                    }

                    if (!groupedData.ContainsKey(groupKey))
                        groupedData[groupKey] = new List<IDictionary<string, object>>();

                    groupedData[groupKey].Add(item);
                }

                // Add table rows with grouped data and totals
                int rowIndex = 2;
                int headerRow = 0;

                var groupColumnName = columns.FirstOrDefault(f => f.FieldName == groupingColumn)?.DisplayName;

                foreach (var group in groupedData)
                {
                    // Add group header row
                    headerRow = rowIndex;
                    worksheet.Range(rowIndex, 1, rowIndex, columns.Count).Merge();

                    var groupHeaderKey = group.Key;
                    //Get Name by Id
                    if (groupingColumn.ToLower().EndsWith("id"))
                    {
                        var nameGroupField = groupingColumn.Substring(0, groupingColumn.Length - 2) + "Name";

                        var firstGroupItem = group.Value.FirstOrDefault();

                        if (firstGroupItem != null)
                        {
                            if (firstGroupItem.ContainsKey(nameGroupField))
                            {
                                var groupKeyName = firstGroupItem[nameGroupField];

                                if (groupKeyName != null)
                                {
                                    groupHeaderKey = $"{groupHeaderKey} - {groupKeyName}";
                                }
                            }
                        }
                    }

                    worksheet.Cell(rowIndex, 1).Value = $"{groupColumnName}: {groupHeaderKey}";
                    worksheet.Cell(rowIndex, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Cell(rowIndex, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FCD5B4");
                    worksheet.Cell(rowIndex, 1).Style.Font.Bold = true;
                    rowIndex++;

                    // Add table rows with data for the current group
                    foreach (var groupItem in group.Value)
                    {
                        columnIndex = 0;
                        string formattedValue;

                        foreach (var header in columns)
                        {
                            formattedValue = "";
                            // (data formatting code remains the same)
                            if (groupItem.ContainsKey(header.FieldName))
                            {
                                var value = groupItem[header.FieldName];

                                if (value == null)
                                {
                                    formattedValue = value?.ToString();
                                    worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                                }
                                else if (header.FieldTypeEnum == TableFieldTypeSm.Integer)
                                {
                                    formattedValue = value is decimal decimalValue ? decimalValue.ToString(header.DispalyFormat) : value.ToString();
                                    if (decimal.TryParse(formattedValue, out decimal numericValue))
                                    {
                                        header.TotalSum += numericValue;
                                    }
                                    worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                                }
                                else if (header.FieldTypeEnum == TableFieldTypeSm.Decimal)
                                {
                                    formattedValue = value is decimal decimalValue ? decimalValue.ToString(header.DispalyFormat) : value.ToString();
                                    if (decimal.TryParse(formattedValue, out decimal numericValue))
                                    {
                                        header.TotalSum += numericValue;
                                    }
                                    worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                                }
                                else if (header.FieldTypeEnum == TableFieldTypeSm.Date)
                                {
                                    formattedValue = value is DateTime dateTimeValue ? dateTimeValue.ToString(header.DispalyFormat) : value.ToString();
                                    worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                                }
                                else if (header.FieldTypeEnum == TableFieldTypeSm.Boolean)
                                {
                                    formattedValue = value is bool boolValue && boolValue == true ? "YES" : "NO";
                                    worksheet.Cell(rowIndex, columnIndex + 1).Value = formattedValue;
                                }
                                else
                                {
                                    formattedValue = $"{value}";
                                    worksheet.Cell(rowIndex, columnIndex + 1).SetValue(formattedValue);
                                }
                            }

                            //Add format
                            worksheet.Cell(rowIndex, columnIndex + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((CustomTextAlign)header.TextAlign);

                            if (!string.IsNullOrEmpty(header.TextColor))
                            {
                                worksheet.Cell(rowIndex, columnIndex + 1).Style.Font.FontColor = XLColor.FromHtml(header.TextColor);
                            }

                            if (!string.IsNullOrEmpty(header.BackgroundColorRules))
                            {
                                if (header.BackgroundColorRules.Contains("="))
                                {
                                    worksheet.Cell(rowIndex, columnIndex + 1).Style.Fill.PatternType = XLFillPatternValues.Solid;
                                    worksheet.Cell(rowIndex, columnIndex + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(StringHelper.GetColorValueUsingKeyFormString(header.BackgroundColorRules, formattedValue));
                                }
                            }
                            else if (!string.IsNullOrEmpty(header.BackgroundColor))
                            {
                                worksheet.Cell(rowIndex, columnIndex + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(header.BackgroundColor);
                            }
                            //End of add format
                            columnIndex++;
                        }

                        rowIndex++;

                        processedCount++;

                        if (processedCount % 500 == 0)
                        {
                            progressCallback?.Invoke(processedCount);
                        }
                    }

                    // Add total row for the group
                    columnIndex = 0;

                    var noOfTotalColumns = columns.Count(f => f.ShowGroupTotal);

                    if (noOfTotalColumns > 0)
                    {                      
                        foreach (var column in columns)
                        {
                            

                            if (column.ShowGroupTotal)
                            {                                          
                                if (column.FieldTypeEnum == TableFieldTypeSm.Integer)
                                {
                                    var displayGroupValue = group.Value.Sum(item => Convert.ToInt32(item[column.FieldName])).ToString(column.DispalyFormat);
                                    worksheet.Cell(rowIndex, columnIndex + 1).Value = displayGroupValue;

                                    if (noOfTotalColumns == 1)
                                    {
                                        worksheet.Cell(headerRow, 1).Value = worksheet.Cell(headerRow, 1).Value + "  -  " + displayGroupValue;
                                    }
                                    else
                                    {
                                        worksheet.Cell(headerRow, 1).Value = worksheet.Cell(headerRow, 1).Value + "   (" + column.DisplayName + " : " + displayGroupValue + ")";
                                    }
                                }
                                else if (column.FieldTypeEnum == TableFieldTypeSm.Decimal)
                                {
                                    var displayGroupValue = group.Value.Sum(item => Convert.ToDecimal(item[column.FieldName])).ToString(column.DispalyFormat);
                                    worksheet.Cell(rowIndex, columnIndex + 1).Value = displayGroupValue;

                                    if (noOfTotalColumns == 1)
                                    {
                                        worksheet.Cell(headerRow, 1).Value = worksheet.Cell(headerRow, 1).Value + "  -  " + displayGroupValue;
                                    }
                                    else
                                    {
                                        worksheet.Cell(headerRow, 1).Value = worksheet.Cell(headerRow, 1).Value + "   (" + column.DisplayName + " : " + displayGroupValue + ")";
                                    }
                                }

                                worksheet.Cell(rowIndex, columnIndex + 1).Style.Font.Bold = true;
                                worksheet.Cell(rowIndex, columnIndex + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((CustomTextAlign)column.TextAlign);                                
                            }
                            else if (column.FieldName == groupingColumn)
                            {
                                worksheet.Cell(rowIndex, columnIndex + 1).Style.Font.Bold = true;
                                worksheet.Cell(rowIndex, columnIndex + 1).Value = "Totals :";
                                worksheet.Cell(rowIndex, columnIndex + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue(CustomTextAlign.Right);
                            }

                            worksheet.Cell(rowIndex, columnIndex + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#DAEEF5");

                            columnIndex++;
                        }

                        rowIndex++;

                    }
                }


                columnIndex = 0;

                //Add total cell for decimal columns
                if (columns.Any(f => f.ShowFooterTotal))
                {
                    foreach (var column in columns)
                    {
                        if (column.ShowFooterTotal)
                        {
                            if (column.FieldTypeEnum == TableFieldTypeSm.Integer)
                            {
                                var displayGroupValue = items.Sum(item => Convert.ToInt32(item[column.FieldName])).ToString(column.DispalyFormat);
                                worksheet.Cell(rowIndex, columnIndex + 1).Value = displayGroupValue;
                            }
                            else if (column.FieldTypeEnum == TableFieldTypeSm.Decimal)
                            {
                                var displayGroupValue = items.Sum(item => Convert.ToDecimal(item[column.FieldName])).ToString(column.DispalyFormat);
                                worksheet.Cell(rowIndex, columnIndex + 1).Value = displayGroupValue;
                            }

                            worksheet.Cell(rowIndex, columnIndex + 1).Style.Font.Bold = true;
                            worksheet.Cell(rowIndex, columnIndex + 1).Style.Alignment.Horizontal = GetAlignmentHorizontalValue((CustomTextAlign)column.TextAlign);
                        }

                        columnIndex++;
                    }
                }

                // Set the font and border for all cells
                var allCells = worksheet.CellsUsed();
                var allCellStyle = allCells.Style;
                var allFont = allCellStyle.Font;
                allFont.FontName = "Calibri";
                allFont.FontSize = 10;

                //allCellStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
                //allCellStyle.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Convert the workbook to a byte array
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        #endregion end of Export Excel from Dictionary

        #region Import from sheet
        //https://www.youtube.com/watch?v=TaaFbuqaeBU&t=204s
        public Response<List<T>> ImportExcel<T>(string path, string sheetName, out List<string> excelColumns)
        {
            var response = new Response<List<T>>();
            Type typeOfObject = typeof(T);
            var list = new List<T>();
            excelColumns = new List<string>();

            try
            {
                using (IXLWorkbook workbook = new XLWorkbook(path))
                {
                    var workSheet = workbook.Worksheets.First();

                    var properties = typeOfObject.GetProperties();

                    //Header column texts 
                    var columns = workSheet.FirstRow().Cells().Select((v, i) => new { Value = GetNameWithoutSpaceAndSpecialChars(v.Value?.ToString()), Index = i + 1 });

                    excelColumns = columns.Select(f => f.Value.ToString()).ToList();

                    foreach (IXLRow row in workSheet.RowsUsed().Skip(1))
                    {
                        T obj = (T)Activator.CreateInstance(typeOfObject);

                        foreach (var prop in properties)
                        {
                            try
                            {
                                var colIndexObj = columns.FirstOrDefault(c => c.Value.ToString() == prop.Name.ToLower().ToString());

                                if (colIndexObj != null)
                                {
                                    Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                    int colIndex = colIndexObj.Index;
                                    var val = row.Cell(colIndex).Value;

                                    if (t == typeof(string))
                                    {
                                        object safeValue = (val == null) ? null : Convert.ChangeType(val, t);
                                        prop.SetValue(obj, safeValue, null);
                                    }
                                    else
                                    {
                                        object safeValue = (val == null || val.ToString() == "") ? null : Convert.ChangeType(val, t);
                                        prop.SetValue(obj, safeValue, null);
                                    }

                                    //int colIndex = colIndexObj.Index;
                                    //var val = row.Cell(colIndex).Value;
                                    //var type = prop.PropertyType;

                                    //try
                                    //{
                                    //    if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                                    //    {
                                    //        if (type == typeof(bool) || type == typeof(Nullable<bool>))
                                    //        {
                                    //            if (val.ToString().ToLower() == "false")
                                    //            {
                                    //                prop.SetValue(obj, false);
                                    //            }
                                    //            else if (val.ToString().ToLower() == "true")
                                    //            {
                                    //                prop.SetValue(obj, true);
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            prop.SetValue(obj, Convert.ChangeType(val, type));
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        if (type == typeof(string))
                                    //        {
                                    //            prop.SetValue(obj, Convert.ChangeType(val, type));
                                    //        }
                                    //    }
                                    //}
                                    //catch(Exception ex)
                                    //{ 
                                    //    string message = ex.Message;
                                    //}
                                }
                            }
                            catch (Exception ex)
                            {
                                response.Message = ex.Message;
                                return response;
                            }
                        }

                        list.Add(obj);
                    }


                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return response;
            }

            response.IsSuccess = true;
            response.Result = list;
            return response;

        }
        #endregion

        #region Private Methods
        //

        private string GetNameWithoutSpaceAndSpecialChars(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            StringBuilder sb = new StringBuilder();

            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    sb.Append(char.ToLower(c));
                }
            }

            return sb.ToString();
        }

        public double ConvertPixelsToWidth(int pixels)
        {
            const double basePixels = 0.1357; // Base pixel value used by ClosedXML.Excel
            //const double baseWidth = 1.0; // Base width value used by ClosedXML.Excel

            return pixels * basePixels;
        }

        public XLAlignmentHorizontalValues GetAlignmentHorizontalValue(CustomTextAlign textAlign)
        {
            if (textAlign == CustomTextAlign.Left)
            {
                return XLAlignmentHorizontalValues.Left;
            }
            else if (textAlign == CustomTextAlign.Right)
            {
                return XLAlignmentHorizontalValues.Right;
            }
            else if (textAlign == CustomTextAlign.Center)
            {
                return XLAlignmentHorizontalValues.Center;
            }
            else if (textAlign == CustomTextAlign.Justify)
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
        #endregion

    }
}
