using System.Collections.Generic;
using System.Data;
using Pricefy.Challenge.Domain.Entities;
using Pricefy.Challenge.Domain.Repositories;
using System.Data.SqlClient;

namespace Pricefy.Challenge.Infra.Repositories
{
    public class TitleRepository : ITitleRepository
    {
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

            string connection = @"Server=localhost; Database=Import; User ID=sa; Password=MeuSQLEXPRESS123456789;";
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy bulk = new SqlBulkCopy(con);

            bulk.DestinationTableName = "Titles";

            bulk.ColumnMappings.Add("Id", "Id");
            bulk.ColumnMappings.Add("Type", "Type");
            bulk.ColumnMappings.Add("PrimaryTitle", "PrimaryTitle");
            bulk.ColumnMappings.Add("OriginalTitle", "OriginalTitle");
            bulk.ColumnMappings.Add("IsAdult", "IsAdult");
            bulk.ColumnMappings.Add("StartYear", "StartYear");
            bulk.ColumnMappings.Add("EndYear", "EndYear");
            bulk.ColumnMappings.Add("RuntimeMinutes", "RuntimeMinutes");
            bulk.ColumnMappings.Add("Genres", "Genres");

            con.Open();

            bulk.WriteToServer(tbl);

            con.Close();
        }
    }
}