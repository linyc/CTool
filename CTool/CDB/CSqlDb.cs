using System;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;

namespace CTool.CDB
{
    class CSqlDb:ICDB
    {
        
        // Fields
        public static CSqlDb Cdb;
        private SqlCommand cmd;
        private SqlConnection conn;

        // Methods
        public CSqlDb(string conString)
        {
            this.conn = new SqlConnection(conString);
            this.cmd = new SqlCommand();
        }

        public void Close()
        {
            if ((this.conn != null) && (this.conn.State != ConnectionState.Closed))
            {
                this.conn.Close();
            }
        }

        public bool Open()
        {
            try
            {
                if ((this.conn != null) && (this.conn.State == ConnectionState.Closed))
                {
                    this.conn.Open();
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public int ExecuteNonQuery(string strSql)
        {
            if (Open())
            {
                try
                { 
                    cmd.CommandText = strSql;
                    return cmd.ExecuteNonQuery();
                }
                catch
                {
                    return -1;
                }
            }
            return 0;
        }

        public int ExecuteNonQuery(string strSql,string[] pars, object[] vals)
        {
            if (Open())
            {
                cmd.Parameters.Clear();
                cmd.CommandText = strSql;
                for (int i = 0; i < pars.Length; i++)
                {
                    cmd.Parameters.Add(new OleDbParameter(pars[i], vals[i]));
                }
                return cmd.ExecuteNonQuery();
            }
            return 0;
        }

        public SqlDataReader ExecuteReader_Sql(string strSql)
        {
            if (Open())
            {
                cmd.CommandText = strSql;
                return cmd.ExecuteReader();
            }
            return null;
        }

        public DataTable GetDataTable(string strSql)
        {
            if (this.Open())
            {
                DataTable dt = new DataTable();
                SqlDataAdapter dtaFill = new SqlDataAdapter(strSql, conn);
                dtaFill.Fill(dt);
                dtaFill.Dispose();
                Close();
                return dt;
            }
            return null;
        }

        public DataSet GetDataSet(string strSql)
        {
            if (this.Open())
            {
                DataSet ds = new DataSet();
                SqlDataAdapter dtaFill = new SqlDataAdapter(strSql, conn);
                dtaFill.Fill(ds);
                dtaFill.Dispose();
                Close();
                return ds;
            }
            return null;
        }

        public SqlTransaction GetTransaction_Sql()
        {
            if ((this.conn != null) && this.Open())
            {
                SqlTransaction t = conn.BeginTransaction();
                cmd.Transaction = t;
                return t;
            }
            return null;
        }

        public bool IsExistTable(string tableName)
        {
            string strSql = "select * from dbo.sysobjects where id = object_id(N'[dbo].[" + tableName + "]') and \r\nOBJECTPROPERTY(id, N'IsUserTable') = 1";
            return (this.GetScalar(strSql) != null);
        }

        public object GetScalar(string strSql)
        {
            if (this.Open())
            {
                cmd.CommandText = strSql;
                return cmd.ExecuteScalar();
            }
            return null;
        }

        #region ICDB 成员

        OleDbDataReader ICDB.ExecuteReader_Oledb(string strSql)
        {
            throw new NotImplementedException();
        }

        OleDbTransaction ICDB.GetTransaction_Oledb()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
