using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace CTool.CDB
{
    public interface ICDB
    {
        OleDbDataReader ExecuteReader_Oledb(string strSql);
        OleDbTransaction GetTransaction_Oledb();
        SqlDataReader ExecuteReader_Sql(string strSql);
        SqlTransaction GetTransaction_Sql();

        DataTable GetDataTable(string strSql);
        DataSet GetDataSet(string strSql);
        object GetScalar(string strSql);
        bool Open();
        void Close();
        int ExecuteNonQuery(string strSql);
        int ExecuteNonQuery(string strSql, string[] pars, object[] vals);
        bool IsExistTable(string tableName);
    }
}
