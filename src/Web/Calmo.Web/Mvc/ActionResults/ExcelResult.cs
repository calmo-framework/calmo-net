using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ClosedXML.Excel;
using Calmo.Web.Properties;

namespace System.Web.Mvc
{
    public class ExcelResult : ActionResult
    {
        private readonly IEnumerable<object> _values;
        private string _fileName;
        private string _contentType;
        private readonly object _columnsDefinition;
        private readonly Action<IXLWorksheet> _onBeforeSave;

        public ExcelResult(IEnumerable<object> values)
            : this(values, null, null)
        {
        }

        public ExcelResult(IEnumerable<object> values, string fileName)
            : this(values, null, fileName)
        {
        }

        public ExcelResult(IEnumerable<object> values, object columnsDefinition)
            : this(values, columnsDefinition, null)
        {
        }

        public ExcelResult(IEnumerable<object> values, object columnsDefinition, string fileName)
            : this(values, columnsDefinition, fileName, null)
        {
        }

        public ExcelResult(IEnumerable<object> values, object columnsDefinition, string fileName, Action<IXLWorksheet> onBeforeSave)
        {
            _values = values;
            _columnsDefinition = columnsDefinition;
            _onBeforeSave = onBeforeSave;

            if (!ValidateExtension(fileName))
                throw new FormatException(Messages.InvalidExcelFile);

            _fileName = fileName;
        }

        private bool ValidateExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            if (String.IsNullOrWhiteSpace(extension))
                return false;

            extension = extension.ToLower();
            return extension == ".xls" || extension == ".xlsx";
        }

        public override void ExecuteResult(ControllerContext context)
        {
            GenerateSheet(context);
        }

        private void GenerateSheet(ControllerContext context)
        {
            HandleFileNameAndContentType(context);

            var workBook = new XLWorkbook();
            var sheet = workBook.Worksheets.Add(Path.GetFileNameWithoutExtension(_fileName));

            if (_values != null)
            {
                var columns = GetColumns();
                var colIndex = 1;
                var rowIndex = 1;

                foreach (var column in columns)
                {
                    sheet.Cell(rowIndex, colIndex).SetValue(column.Title);
                    colIndex++;
                }

                rowIndex++;
                foreach (var value in _values)
                {
                    colIndex = 1;
                    foreach (var column in columns)
                    {
                        sheet.Cell(rowIndex, colIndex).Value = column.PropertyInfo != null 
                                                                ? column.PropertyInfo.GetValue(value)
                                                                : ((Dictionary<string, object>)value)[column.Property];
                        colIndex++;
                    }
                    rowIndex++;
                }

                if (this._onBeforeSave != null)
                    this._onBeforeSave(sheet);
            }

            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = _contentType;
            context.HttpContext.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}", _fileName));

            using (var stream = new MemoryStream())
            {
                workBook.SaveAs(stream);
                stream.WriteTo(context.HttpContext.Response.OutputStream);
                stream.Close();
            }
        }

        private void HandleFileNameAndContentType(ControllerContext context)
        {
            if (!String.IsNullOrWhiteSpace(_fileName))
            {
                _fileName = _fileName.Trim();
                return;
            }

            _fileName = String.Format("{0}{1}.xlsx", context.RouteData.Values["Controller"], context.RouteData.Values["Action"]);

            var extension = Path.GetExtension(_fileName);

            _contentType = extension == ".xlsx" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/vnd.ms-excel";
        }

        private IEnumerable<ExcelColumnDefinition> GetColumns()
        {
            if (!this._values.HasItems() && this._columnsDefinition == null)
                return null;

            List<ExcelColumnDefinition> columns = null;
            if (this._columnsDefinition != null)
            {
                var columnsDefinitionType = _columnsDefinition.GetType();
                if (columnsDefinitionType.IsAnonymous())
                {
                    columns = columnsDefinitionType.GetProperties().Select(p => new ExcelColumnDefinition
                    {
                        Property = p.Name,
                        Title = p.GetValue(_columnsDefinition) as string
                    }).ToList();
                }
                else
                {
                    if (columnsDefinitionType != typeof(Dictionary<string, string>))
                        throw new InvalidCastException(Messages.InvalidColumnDefinition);

                    var dictionaryColumns = (Dictionary<string, string>)_columnsDefinition;
                    columns = dictionaryColumns.Keys.Select(key => new ExcelColumnDefinition
                    {
                        Property = key, 
                        Title = dictionaryColumns[key]
                    }).ToList();
                }
            }

            if (!this._values.HasItems())
                return columns;

            List<ExcelColumnDefinition> valuesColumns;
            var valuesType = _values.First().GetType();

            var isAnonymousValues = valuesType.IsAnonymous();
            if (isAnonymousValues)
            {
                valuesColumns = valuesType.GetProperties().Select(p => new ExcelColumnDefinition
                {
                    Title = p.Name,
                    Property = p.Name, 
                    PropertyInfo = p
                }).ToList();
            }
            else
            {
                if (valuesType != typeof(Dictionary<string, object>))
                    throw new InvalidCastException(Messages.InvalidValues);

                var valuesDictionaryColumns = (Dictionary<string, object>)_values.First();
                valuesColumns = valuesDictionaryColumns.Keys.Select(key => new ExcelColumnDefinition { Property = key, Title = key}).ToList();
            }

            if (valuesColumns.HasItems() && columns.HasItems())
            {
                for (var i = 0; i < columns.Count(); i++)
                {
                    var columnFoundInValues = valuesColumns.FirstOrDefault(c => c.Property == columns[i].Property);
                    if (columnFoundInValues != null)
                    {
                        if (isAnonymousValues)
                            columns[i].PropertyInfo = columnFoundInValues.PropertyInfo;
                    }
                    else
                    {
                        columns.Remove(columns[i]);
                        i--;
                    }
                }
            }

            if (valuesColumns.HasItems() && !columns.HasItems())
                columns = valuesColumns;

            return columns;
        }

        private class ExcelColumnDefinition
        {
            public string Property { get; set; }
            public string Title { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }
    }
}