using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Staff.Extensions
{
    public static class ExcelPackageExtension
    {
        public static DataTable ToDataTable(this IEnumerable<IDictionary<string, object>> list)
        {
            var dataTable = new DataTable();

            var enumerable = list?.ToList();
            if (enumerable != null && enumerable.Any())
            {
                var columnNames = enumerable.SelectMany(dict => dict.Keys).Distinct();
                dataTable.Columns.AddRange(columnNames.Select(col => new DataColumn(col)).ToArray());

                foreach (var item in enumerable)
                {
                    var row = dataTable.NewRow();
                    foreach (var key in item.Keys)
                    {
                        row[key] = item[key];
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }

    }
}
