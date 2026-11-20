using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ContractManager.Data {
  public class DataAccess {
    private readonly string connectionString;

    public DataAccess() {
      string server = "(localdb)\\Servercito";

      string database = "ContractManager";

      this.connectionString = $"Data Source={server};Initial Catalog={database};Integrated Security=True;Trusted_Connection=True";
    }

    public (int _result, string _msg) ExecuteScalar(string spName, SqlParameter[] sqlParams) {
      int result = 0;
      string msg = "";

      using (SqlConnection conn = new SqlConnection(this.connectionString)) {
        using (SqlCommand cmd = new SqlCommand(spName, conn)) {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddRange(sqlParams);

          try {
            conn.Open();

            result = (int)cmd.ExecuteScalar();
          } catch (Exception ex) {
            msg = ex.Message;
          }
        }
      }

      return (result, msg);
    }

    public DataTable GetProfile(string spName, SqlParameter[] sqlParams)  {
      DataTable response = new DataTable();
      string msg = "";

      using (SqlConnection conn = new SqlConnection(this.connectionString)) {
        using (SqlCommand cmd = new SqlCommand(spName, conn)) {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddRange(sqlParams);

          try {
            conn.OpenAsync();
            cmd.ExecuteScalar();

            var adatper = new SqlDataAdapter(cmd);

            adatper.Fill(response);

            return response;

          } catch (Exception ex) {
            msg = ex.Message;
          }
        }
      }

      return response;
    }
  }
}