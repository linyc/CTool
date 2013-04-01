using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace CTool.CDB
{
    public class COleDb : ICDB
    {
        // Fields
        public static COleDb Cdb;
        private OleDbCommand cmd;
        private OleDbConnection conn;

        // Methods
        public COleDb(string conString)
        {
            this.conn = new OleDbConnection(conString);
            this.cmd = new OleDbCommand();
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

        public int ExecuteNonQuery(string strSql, Dictionary<string, object> dicParams)
        {
            if (Open())
            {
                cmd.Parameters.Clear();
                cmd.CommandText = strSql;

                foreach (var item in dicParams)
                {
                    cmd.Parameters.Add(item.Key, item.Value);
                }
                return cmd.ExecuteNonQuery();
            }
            return 0;
        }

        public OleDbDataReader ExecuteReader_Oledb(string strSql)
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
                OleDbDataAdapter dtaFill = new OleDbDataAdapter(strSql, conn);
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
                OleDbDataAdapter dtaFill = new OleDbDataAdapter(strSql, conn);
                dtaFill.Fill(ds);
                dtaFill.Dispose();
                Close();
                return ds;
            }
            return null;
        }

        public OleDbTransaction GetTransaction_Oledb()
        {
            if ((this.conn != null) && this.Open())
            {
                OleDbTransaction t = conn.BeginTransaction();
                cmd.Transaction = t;
                return t;
            }
            return null;
        }

        public bool IsExistTable(string tableName)
        {
            string strSql = "select * from dbo.sysobjects where id = object_id(N'[dbo].[" + tableName + "]') and \r\nOBJECTPROPERTY(id, N'IsUserTable') = 1";
            return (GetScalar(strSql) != null);
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

        #region ICDB ≥…‘±


        SqlDataReader ICDB.ExecuteReader_Sql(string strSql)
        {
            throw new NotImplementedException();
        }

        SqlTransaction ICDB.GetTransaction_Sql()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}