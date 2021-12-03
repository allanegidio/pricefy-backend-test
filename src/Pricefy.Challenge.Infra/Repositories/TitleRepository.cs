using System.Collections.Generic;
using System.Data;
using Pricefy.Challenge.Domain.Entities;
using Pricefy.Challenge.Domain.Repositories;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Pricefy.Challenge.Infra.Repositories
{
    public class TitleRepository : ITitleRepository
    {
        string connection = @"Server=localhost; Database=Import; User ID=sa; Password=MeuSQLEXPRESS123456789;";
        SqlConnection _con;
        SqlBulkCopy _bulk;

        public TitleRepository()
        {
            _con = new SqlConnection(connection);
            _bulk = new SqlBulkCopy(_con);
        }

        public void BulkInsert(List<Title> titles)
        {
            var tbl = new DataTable();
            tbl.Columns.Add(new DataColumn("Id", typeof(string)));
            tbl.Columns.Add(new DataColumn("Type", typeof(string)));
            tbl.Columns.Add(new DataColumn("PrimaryTitle", typeof(string)));
            tbl.Columns.Add(new DataColumn("OriginalTitle", typeof(string)));
            tbl.Columns.Add(new DataColumn("IsAdult", typeof(bool)));
            tbl.Columns.Add(new DataColumn("StartYear", typeof(string)));
            tbl.Columns.Add(new DataColumn("EndYear", typeof(string)));
            tbl.Columns.Add(new DataColumn("RuntimeMinutes", typeof(string)));
            tbl.Columns.Add(new DataColumn("Genres", typeof(string)));

            foreach (var title in titles)
            {
                var dr = tbl.NewRow();
                dr["Id"] = title.Id;
                dr["Type"] = title.Type;
                dr["PrimaryTitle"] = title.PrimaryTitle;
                dr["OriginalTitle"] = title.OriginalTitle;
                dr["IsAdult"] = title.IsAdult;
                dr["StartYear"] = title.StartYear;
                dr["EndYear"] = title.EndYear;
                dr["RuntimeMinutes"] = title.RuntimeMinutes;
                dr["Genres"] = title.Genres;

                tbl.Rows.Add(dr);
            }

            _bulk.DestinationTableName = "Title";

            _bulk.ColumnMappings.Add("Id", "Id");
            _bulk.ColumnMappings.Add("Type", "Type");
            _bulk.ColumnMappings.Add("PrimaryTitle", "PrimaryTitle");
            _bulk.ColumnMappings.Add("OriginalTitle", "OriginalTitle");
            _bulk.ColumnMappings.Add("IsAdult", "IsAdult");
            _bulk.ColumnMappings.Add("StartYear", "StartYear");
            _bulk.ColumnMappings.Add("EndYear", "EndYear");
            _bulk.ColumnMappings.Add("RuntimeMinutes", "RuntimeMinutes");
            _bulk.ColumnMappings.Add("Genres", "Genres");

            _con.Open();

            _bulk.WriteToServer(tbl);

            _con.Close();
        }

        public async Task<bool> IsImported(string fileName)
        {
            _con.Open();

            var cmd = new SqlCommand("SELECT FileName FROM File WHERE FileName = @fileName", _con);

            cmd.Parameters.AddWithValue("@fileName", fileName);

            using var reader = await cmd.ExecuteReaderAsync();

            return reader.HasRows ? true
                                : false;
        }
    }
}