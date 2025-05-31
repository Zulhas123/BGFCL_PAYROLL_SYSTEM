using DocumentFormat.OpenXml.Office2013.Excel;
using Entities;
using System.Data;
using static BgfclApp.Controllers.Report.SalaryReportController;

namespace BgfclApp.Service
{
    public static class DataTableHelper
    {
        //public static DataTable ToDataTable<T>(List<T> items)
        //{
        //    var dataTable = new DataTable(typeof(T).Name);
        //    var props = typeof(T).GetProperties();

        //    foreach (var prop in props)
        //    {
        //        dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //    }

        //    foreach (var item in items)
        //    {
        //        var values = new object[props.Length];
        //        for (int i = 0; i < props.Length; i++)
        //        {
        //            values[i] = props[i].GetValue(item, null);
        //        }

        //        dataTable.Rows.Add(values);
        //    }

        //    return dataTable;
        //}
        public class AttendancePivotRow
        {
            public string JobCode { get; set; }
            public string EmployeeName { get; set; }
            public string DesignationName { get; set; } // Optional
            public Dictionary<string, string> DayStatus { get; set; } = new();
        }

        public static DataTable ToDataTable(List<AttendancePivotRow> rows, List<DateTime> dates)
        {
            var table = new DataTable();

            table.Columns.Add("JobCode");
            table.Columns.Add("EmployeeName");

            foreach (var date in dates)
            {
                string colName = date.ToString("dd-MMM");
                table.Columns.Add(colName);
            }

            foreach (var row in rows ?? Enumerable.Empty<AttendancePivotRow>())
            {
                var dataRow = table.NewRow();

                dataRow["JobCode"] = row.JobCode ?? "";
                dataRow["EmployeeName"] = row.EmployeeName ?? "";

                foreach (var date in dates)
                {
                    string colName = date.ToString("dd-MMM");
                    string value = row.DayStatus?.TryGetValue(colName, out var val) == true ? val : "0";
                    dataRow[colName] = value;
                }

                table.Rows.Add(dataRow);
            }

            return table;
        }





        public class ReportCell
        {
            public int RowId { get; set; }
            public string ColumnName { get; set; }
            public string Value { get; set; }

            public static List<ReportCell> ConvertTableToCells(DataTable table)
            {
                List<ReportCell> cells = new List<ReportCell>();

                int rowId = 0;
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        cells.Add(new ReportCell
                        {
                            RowId = rowId,
                            ColumnName = col.ColumnName,
                            Value = row[col]?.ToString()
                        });
                    }
                    rowId++;
                }

                return cells;
            }
        }


    }

}
