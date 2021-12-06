using System.Collections.Generic;
using System.Data;
using Pricefy.Challenge.Domain.Entities;
using Pricefy.Challenge.Domain.Repositories;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Pricefy.Challenge.Infra.Repositories
{
    public class TitleRepository : ITitleRepository
    {
        string connection = @"Server=localhost; Database=Import; User ID=sa; Password=MeuSQLEXPRESS123456789;";
        private SqlConnection _con;
        private SqlBulkCopy _bulk;
        private SqlTransaction _transaction;
        private readonly ILogger<TitleRepository> _logger;

        public TitleRepository(ILogger<TitleRepository> logger)
        {
            _con = new SqlConnection(connection);
            _logger = logger;
        }

        public async Task<bool> BulkInsert(string fileName, List<Title> titles)
        {
            try
            {
                var importedTitles = await GetAllImportedTitles();

                titles.RemoveAll(title => importedTitles.Any(imported => imported.Id == title.Id));

                if (titles.Count <= 0)
                    return false;

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

                _con.Open();

                _transaction = _con.BeginTransaction();

                _bulk = new SqlBulkCopy(_con, SqlBulkCopyOptions.Default, _transaction);

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

                await _bulk.WriteToServerAsync(tbl);

                using var cmd = new SqlCommand("INSERT INTO FileDetail (FileName, TotalRegistries, RegistriesImported, ImportDate) VALUES (@fileName, @totalRegistries, @registriesImported, @importDate)", _con, _transaction);

                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.Parameters.AddWithValue("@totalRegistries", titles.Count);
                cmd.Parameters.AddWithValue("@registriesImported", titles.Count);
                cmd.Parameters.AddWithValue("@importDate", DateTime.Now.ToString("yyyy-MM-dd"));

                await cmd.ExecuteNonQueryAsync();

                //await SaveFileDetail(fileName, titles.Count);

                await _transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                _logger.LogError($"An error occured when tried sql bulky insert titles. Message is: {ex.Message}");
                return false;
            }
            finally
            {
                _con.Close();
            }
        }

        private async Task<List<Title>> GetAllImportedTitles()
        {
            try
            {
                _con.Open();

                var listTitles = new List<Title>();

                var cmd = new SqlCommand("SELECT * FROM Title", _con);

                using var reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listTitles.Add(new Title
                        {
                            Id = (string)reader["Id"],
                            Type = (string)reader["Type"],
                            PrimaryTitle = (string)reader["PrimaryTitle"],
                            OriginalTitle = (string)reader["OriginalTitle"],
                            IsAdult = (bool)reader["IsAdult"],
                            StartYear = (string)reader["StartYear"],
                            EndYear = (string)reader["EndYear"],
                            RuntimeMinutes = (string)reader["RuntimeMinutes"],
                            Genres = (string)reader["Genres"],
                        });
                    }
                }

                return listTitles;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured when tried get all titles imported. Message is: {ex.Message}");
                return null;
            }
            finally
            {
                _con.Close();
            }
        }

        public async Task<List<FileDetail>> GetAllFilesImported()
        {
            try
            {
                _con.Open();

                var cmd = new SqlCommand("SELECT * FROM FileDetail", _con);

                var files = new List<FileDetail>();

                using var reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        files.Add(new FileDetail()
                        {
                            FileName = (string)reader["FileName"],
                            TotalRegistries = (long)reader["TotalRegistries"],
                            RegistriesImported = (long)reader["RegistriesImported"],
                            ImportDate = (DateTime)reader["ImportDate"]
                        });
                    }
                }

                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured when tried get all files imported. Message is: {ex.Message}");
                return null;
            }
            finally
            {
                _con.Close();
            }
        }

        public async Task<bool> IsImported(string fileName)
        {
            try
            {
                _con.Open();

                var cmd = new SqlCommand("SELECT FileName FROM FileDetail WHERE FileName = @fileName", _con);

                cmd.Parameters.AddWithValue("@fileName", fileName);

                using var reader = await cmd.ExecuteReaderAsync();

                var result = reader.HasRows ? true : false;

                _con.Close();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured when tried to check if file is already imported. Message is: {ex.Message}");
                return false;
            }
            finally
            {
                _con.Close();
            }
        }

        public async Task SaveFileDetail(string fileName, int count)
        {
            using var cmd = new SqlCommand("INSERT INTO FileDetail (FileName, TotalRegistries, RegistriesImported, ImportDate) VALUES (@fileName, @totalRegistries, @registriesImported, @importDate)", _con);

            cmd.Parameters.AddWithValue("@fileName", fileName);
            cmd.Parameters.AddWithValue("@totalRegistries", count);
            cmd.Parameters.AddWithValue("@registriesImported", count);
            cmd.Parameters.AddWithValue("@importDate", DateTime.Now);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}