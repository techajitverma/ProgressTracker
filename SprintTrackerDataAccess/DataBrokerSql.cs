using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;

namespace SprintTrackerDataAccess
{
    public class DataBrokerSql : DataBroker
    {
        private enum DatabrokerCommandType
        {
            Select,
            Delete,
            Update,
            Insert,
            Other
        }

        #region Constructors

        public DataBrokerSql() : this(new DbParameters()) { }

        public DataBrokerSql(DbParameters dbParams) : base(dbParams)
        {
        }

        #endregion

        #region Pulic Methods

        #region "DataTable Operations"

        /// <summary>
        /// Retrieves a table - thru running SQL Query
        /// </summary>
        /// <param name="SQL"> The sql SELECT statement to execute</param>
        public DataTable GetDataTable(string SQL)
        {
            ////Trace.Enter();

            try
            {
                zOpenConnection();
                DataTable resultTable = new DataTable();
                GetDataTable(resultTable, SQL);
                return resultTable;
            }
            catch (Exception ex)
            {
               // //SRSException SRSEx = new SRSException("Failed to Get DataTable.", ex);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
               // //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves a table - thru running SQL Query
        /// </summary>
        /// <param name="SQL"> The sql SELECT statement to execute</param>
        /// <param name="resultTable"></param>
        private void GetDataTable(DataTable resultTable, string SQL)
        {
            //Trace.Enter();

            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(resultTable);
                    }
                    cmd.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
                ////SRSException SRSEx = new SRSException("GetDataTable(string SQL) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd); 
                ////SRSEx.Add("SQL", SQL);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(resultTable);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves a table - thru running the SQL Query
        /// </summary>
        /// <param name="SQL"> The sql SELECT statement to execute</param>
        /// <param name="Params"> The array of dbparameters that the query uses</param>
        public DataTable GetDataTable(string SQL, DbParameter[] Params)
        {

            //Trace.Enter();

            try
            {
                zOpenConnection();
                DataTable resultTable = new DataTable();
                GetDataTable(resultTable, SQL, Params);
                return resultTable;
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("Failed to Get Data Table.", ex);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves a table - thru running the SQL Query
        /// </summary>
        /// <param name="resultTable"></param>
        /// <param name="SQL"> The sql SELECT statement to execute</param>
        /// <param name="Params"> The array of dbparameters that the query uses</param>
        private void GetDataTable(DataTable resultTable, string SQL, DbParameter[] Params)
        {
            //Trace.Enter();

            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL, Params))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(resultTable);
                    }
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetDataTable(string SQL, DbParameter[] Params) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd); 
                ////SRSEx.Add("SQL", SQL);
                ////SRSEx.Add("Params", Params);
                throw ex;
            }
            finally
            {
                // TODO: 5/12/2011 (RN & VK) - need to review side effects of reusing the DbParameters across multiple sql commands
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                }
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        public DataTable GetDataTable(string SQL, System.Collections.ArrayList Params)
        {

            //Trace.Enter();

            try
            {
                zOpenConnection();
                DataTable resultTable = new DataTable();
                GetDataTable(resultTable, SQL, Params);
                return resultTable;
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("Failed to Get Data Table.", ex);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        private void GetDataTable(DataTable resultTable, string SQL, System.Collections.ArrayList Params)
        {
            SqlCommand cmd = null;

            //Trace.Enter();

            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL, Params))
                {
                    //if (Params != null)
                    //{
                    //    for (int idx = 0; idx < Params.Count; idx++)
                    //    {
                    //        cmd.Parameters.Add((System.Data.SqlClient.SqlParameter)Params[idx]);
                    //    }
                    //}

                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(resultTable);
                        cmd.Parameters.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetDataTable(string SQL, ArrayList Params) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd); 
                ////SRSEx.Add("SQL", SQL);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(resultTable);
                ////SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        private int PostDataTable(DataTable DT, string SQL)
        {
            //Trace.Enter();

            int result = 0;
            SqlCommand cmd = null;
            SqlCommand cmdI = null;
            SqlCommand cmdU = null;
            SqlCommand cmdD = null;
            SqlDataAdapter adapter;

            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL))
                {
                    using (adapter = new SqlDataAdapter())
                    {
                        adapter.UpdateBatchSize = 0;
                        adapter.SelectCommand = cmd;
                        using (SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(adapter))
                        {
                            using (cmdI = cmdBuilder.GetInsertCommand())
                            {
                                using (cmdU = cmdBuilder.GetUpdateCommand())
                                {
                                    using (cmdD = cmdBuilder.GetDeleteCommand())
                                    {

                                        using (adapter = new SqlDataAdapter())
                                        {
                                            adapter.UpdateBatchSize = 0;

                                            adapter.InsertCommand = cmdI;
                                            adapter.UpdateCommand = cmdU;
                                            adapter.DeleteCommand = cmdD;
                                            DataTable Changes = DT.GetChanges();
                                            if (Changes != null)
                                            {
                                                result = adapter.Update(Changes);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("PostDataTable(DataTable DT, string SQL) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd); 
                ////SRSEx.Add("SQL", SQL);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(result);
                ////SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }

            return result;
        }

        /// <summary>
        /// GetsDataTable running data reader and packs it into string
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        private string GetDataTableAsString(string dbName, string SQL)
        {
            //Trace.Enter();
            #region Getting Data and MetaData
            StringBuilder sbData = new StringBuilder();
            SqlDataReader reader = null;
            SqlCommand cmd = null;
            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL))
                {

                    using (reader = cmd.ExecuteReader())
                    {
                        m_DbParameters.KeepConnectionOpen = true;

                        // Call Read before accessing data.
                        int fieldCounter = 0;

                        sbData.Append("DataBegins:");
                        int rowCounter = 0;
                        string currentValue = string.Empty;
                        while (reader.Read())
                        {
                            fieldCounter = reader.VisibleFieldCount;

                            for (int i = 0; i < fieldCounter; i++)
                            {
                                currentValue = reader[i].ToString();
                                currentValue = (currentValue.Length == 0) ? DelimeterConstants.BLANK_DATA : currentValue;
                                if (i == fieldCounter - 1)
                                {
                                    sbData.Append(currentValue);
                                }
                                else
                                {
                                    sbData.Append(currentValue + DelimeterConstants.TABLE_COLUMN_DATA_DELIMETER);
                                }
                            }
                            rowCounter++;
                            sbData.Append(DelimeterConstants.TABLE_ROW_DATA_DELIMETER);
                        }
                        string rowData = sbData.ToString();

                        rowData = rowData.Substring(0, rowData.LastIndexOf(DelimeterConstants.TABLE_ROW_DATA_DELIMETER));
                        rowData += "RowNumber=" + rowCounter.ToString() + "DataEnds";

                        // Call Close when done reading.
                        reader.Close();
                        return rowData;
                    }
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetDataTableAsString() failed. Message=" + ex.Message);
               // //zParamsToException(ref SRSEx, cmd);
                throw ex;
            }
            finally
            {
                reader.Close();
                zCloseConnection(false);
                //Trace.Exit();
            }
            #endregion
        }

        /// <summary>
        /// Retrieves a table - thru running Store Procedure 
        /// </summary>
        /// <param name="StoreProcedureName"> The Store Procedure Name to execute</param>
        public DataTable GetDataTableBySp(string StoreProcedureName)
        {

            //Trace.Enter();

            try
            {
                zOpenConnection();
                DataTable resultTable = new DataTable();
                GetDataTableBySp(resultTable, StoreProcedureName);
                return resultTable;
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("Failed.", ex);
               // //SRSEx.Add("Store Procedure Name", StoreProcedureName);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves a table - thru running Store Procedure 
        /// </summary>
        /// <param name="resultTable"></param>
        /// <param name="StoreProcedureName">The Store Procedure Name to execute</param>
        private void GetDataTableBySp(DataTable resultTable, string StoreProcedureName)
        {
            //Trace.Enter();

            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSpCommand(StoreProcedureName))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(resultTable);
                    }
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetDataTableBySp(string StoreProcedureName) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd); 
                ////SRSEx.Add("Store Procedure Name", StoreProcedureName);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(resultTable);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves a table - thru running the  Store Procedure 
        /// </summary>
        /// <param name="StoreProcedureName"> The sql SELECT statement to execute</param>
        /// <param name="In"> The array of dbparameters that the query uses</param>
        public DataTable GetDataTableBySp(string StoreProcedureName, System.Collections.ArrayList In)
        {

            //Trace.Enter();

            try
            {
                zOpenConnection();
                DataTable resultTable = new DataTable();
                GetDataTableBySp(resultTable, StoreProcedureName, In);
                return resultTable;

            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("Failed to fetch Data Table via Stored Procedure.", ex);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves a table - thru running the  Store Procedure 
        /// </summary>
        /// <param name="resultTable"></param>
        /// <param name="StoreProcedureName"></param>
        /// <param name="In"></param>
        private void GetDataTableBySp(DataTable resultTable, string StoreProcedureName, System.Collections.ArrayList In)
        {
            //Trace.Enter();

            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSpCommand(StoreProcedureName, In))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(resultTable);
                    }
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetDataTableBySp(string StoreProcedureName, DbParameter[] Params) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd); 
                ////SRSEx.Add("Store Procedure Name", StoreProcedureName);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(resultTable);
                ////SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves a table - thru running the  Store Procedure 
        /// </summary>
        /// <param name="StoreProcedureName"> The stored procedure to execute</param>
        /// <param name="Params"> The array of dbparameters that the query uses</param>
        public DataTable GetDataTableBySp(string StoreProcedureName, DbParameter[] Params)
        {
            //Trace.Enter();

            SqlCommand cmd = null;
            DataTable resultTable = new DataTable();
            try
            {
                zOpenConnection();

                using (cmd = zToSpCommand(StoreProcedureName, Params))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(resultTable);
                    }
                }
                return resultTable; 
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetDataTableBySp(string StoreProcedureName, DbParameter[] Params) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd);
                ////SRSEx.Add("Store Procedure Name", StoreProcedureName);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(resultTable);
                ////SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        #endregion

        #region DataSet Operations

        /// <summary>
        /// Retrieves a DataSet - thru running the  Store Procedure 
        /// </summary>
        public DataSet GetDataSetBySp(string StoreProcedureName, System.Collections.ArrayList In)
        {
            //Trace.Enter();

            try
            {
                zOpenConnection();
                DataSet resultDataSet = new DataSet();
                GetDataSetBySp(resultDataSet, StoreProcedureName, In);
                return resultDataSet;

            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("Failed to fetch DataSet via Stored Procedure.", ex);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves a DataSet - thru running the  Store Procedure 
        /// </summary>
        private void GetDataSetBySp(DataSet resultDataSet, string StoreProcedureName, System.Collections.ArrayList In)
        {
            //Trace.Enter();

            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSpCommand(StoreProcedureName, In))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(resultDataSet);
                    }
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetDataSetBySp(DataSet resultDataSet, string StoreProcedureName, System.Collections.ArrayList In) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd);
                ////SRSEx.Add("Store Procedure Name", StoreProcedureName);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(resultDataSet);
                ////SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        #endregion

        #region "Exec SPROC"

        /// <summary>
        /// Execute a store procedure and return object = Return.Value
        /// </summary>
        /// <param name="StoreProcedureName"></param>
        /// <param name="In"></param>
        /// <param name="Return"></param>
        /// <returns></returns>
        public object ExecStoreProcedure(string StoreProcedureName,
                                          ArrayList In,
                                          SqlParameter Return)
        {
            
            //Trace.Enter();
            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = new SqlCommand(StoreProcedureName))
                {
                    cmd.Connection = (SqlConnection)m_DBConnection;
                    cmd.Transaction = (SqlTransaction)m_Transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Timeout;

                    if (In != null)
                    {
                        for (int i = 0; i < In.Count; i++)
                        {
                            cmd.Parameters.Add((System.Data.SqlClient.SqlParameter)In[i]);
                        }
                    }
                    if (Return != null)
                    {
                        cmd.Parameters.Add(Return);
                    }
                    cmd.ExecuteNonQuery();
                    if (Return != null)
                    {
                        return Return.Value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("ExecSQL(string SQL) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd); 
                ////SRSEx.Add("StoreProcedureName", StoreProcedureName);
                ////SRSEx.Add(In);
                ////SRSEx.Add(Return);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
            
        }

        /// <summary>
        /// Executes a stored procedure
        /// </summary>
        /// <param name="StoredProcedureName"> The name of the Stored Procedure to execute</param>
        /// <param name="Params"> The array of dbparameters that the query uses</param>
        public void ExecStoreProcedure(string StoredProcedureName, DbParameter[] Params = null)
        {
            //Trace.Enter();

            SqlDataReader result = null;
            SqlCommand cmd = null;
            try
            {
                zOpenConnection();
                using (cmd = new SqlCommand(StoredProcedureName))
                {
                    cmd.Connection = (SqlConnection)m_DBConnection;
                    cmd.Transaction = (SqlTransaction)m_Transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Timeout;
                    if (Params != null)
                    {
                        cmd.Parameters.AddRange(Params);
                    }
                    cmd.ExecuteNonQuery();
                    
                }
            }
            catch (Exception ex)
            {
                try { zCloseConnection(false); }
                catch { }
                //SRSException SRSEx = new SRSException("GetDataReaderBySp(string StoredProcedureName, DbParameter[] Params = null) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd);
                ////SRSEx.Add("StoredProcedureName", StoredProcedureName);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(result);
                ////SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                //Trace.Exit();
            }
        }


        static public void AddParameter(System.Collections.ArrayList input, ParameterDirection direction, string parameterName, SqlDbType sqlDbType, int size, Object value)
        {
            SqlParameter parm = null;

            try
            {
                parm = new SqlParameter();
                parm.ParameterName = parameterName;
                parm.SqlDbType = sqlDbType;
                parm.Value = value;
                parm.Size = size;
                parm.Direction = direction;
                input.Add(parm);
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("Failed to add parameter", ex);
                ////SRSEx.Add("Parm", parm);
                ////SRSEx.Add("direction", direction.ToString());
                ////SRSEx.Add("parameterName", parameterName);
                ////SRSEx.Add("sqlDbType", sqlDbType.ToString());
                ////SRSEx.Add("size", size.ToString());
                ////SRSEx.Add("value", value);
                throw ex;
            }
        }
        
        static public void AddParameter(System.Collections.ArrayList input, ParameterDirection direction, string parameterName, SqlDbType sqlDbType, Object value)
        {
            try
            {
                AddParameter(input, direction, parameterName, sqlDbType, 0, value);
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("Failed to add parameter [Default size of 0]", ex);
                throw ex;
            }
        }

        static public void RemoveParameter(System.Collections.ArrayList input, ParameterDirection direction, string parameterName, SqlDbType sqlDbType, Object value)
        {
            SqlParameter param = null;

            try
            {
                // NOTE: THIS DOES NOT WORK AND NEEDS TO BE REFACTORED!
                // What this code is currently doing is:
                // 1. Creating a reference to a new object
                // 2. Attempting to remove that reference from the input list
                // That reference will NEVER exist in the input list.
                // We should instead modify the parameter list to either:
                // a. Accept the ArrayList and a reference to the original SqlParameter that we want to remove
                // or
                // b. Accept the ArrayList and the parameterName to remove, then iterate through the ArrayList to find and remove
                // the parameter with that parameterName.
                // - RR
                param = new SqlParameter();
                param.ParameterName = parameterName;
                param.SqlDbType = sqlDbType;
                param.Value = value;
                param.Direction = direction;
                input.Remove(param);
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("Failed to remove parameter", ex);
                ////SRSEx.Add("Param", param);
                ////SRSEx.Add("direction", direction.ToString());
                ////SRSEx.Add("parameterName", parameterName);
                ////SRSEx.Add("sqlDbType", sqlDbType.ToString());
                ////SRSEx.Add("value", value);
                throw ex;
            }
        }

        /// <summary>
        /// Executes stored procedure and returns object = Return.Value
        /// </summary>
        /// <param name="StoreProcedureName"></param>
        /// <param name="In"></param>
        /// <param name="Out"></param>
        /// <param name="Return"></param>
        /// <returns>Object</returns>
        public object ExecStoreProcedure(string StoreProcedureName,
                                          ArrayList In,
                                          ArrayList Out,
                                          SqlParameter Return)
        {
            //Trace.Enter();

            SqlCommand cmd = null; 

            try
            {
                zOpenConnection();
                using (cmd = new SqlCommand(StoreProcedureName))
                {
                    cmd.Connection = (SqlConnection)m_DBConnection;
                    cmd.Transaction = (SqlTransaction)m_Transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Timeout;
                    if (In != null)
                    {
                        for (int i = 0; i < In.Count; i++)
                        {
                            cmd.Parameters.Add((System.Data.SqlClient.SqlParameter)In[i]);
                        }
                    }
                    if (Out != null)
                    {
                        for (int i = 0; i < Out.Count; i++)
                        {
                            cmd.Parameters.Add((System.Data.SqlClient.SqlParameter)Out[i]);
                        }
                    }
                    if (Return != null)
                    {
                        cmd.Parameters.Add(Return);
                    }

                    cmd.ExecuteNonQuery();

                    if (Return != null)
                    {
                        return Return.Value;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("ExecSQL(string SQL) Method failed within DataBroker class.", ex);
                ////zParamsToException(ref SRSEx, cmd); 
                ////SRSEx.Add("StoreProcedureName", StoreProcedureName);
                ////SRSEx.Add(In);
                ////SRSEx.Add(Out);
                ////SRSEx.Add(cmd);
                ////SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
            
        }

        #endregion

        #region "Exex Sql"

        /// <summary>
        /// Execute a non query statement
        /// </summary>
        /// <param name="SQL"> The non query query state to execute</param>
        public int ExecSQL(string SQL)
        {
            //Trace.Enter();

            int result = 0;
            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using(cmd = zToSqlCommand(SQL))
                {
                    cmd.CommandType = CommandType.Text;
                    result = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("ExecSQL(string SQL) Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd); 
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }

            return result;
        }

        /// <summary>
        /// Execute a non query statement
        /// </summary>
        /// <param name="SQL"> The non query query state to execute</param>
        /// <param name="Params"> The array of dbparameters that the query uses</param>
        public int ExecSQL(string SQL, DbParameter[] Params)
        {
            //Trace.Enter();

            int result = 0;
            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL, Params))
                {
                    result = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("ExecSQL(string SQL, DbParameter[] Params) Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd); 
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }

            return result;
        }

        #endregion

        #region "Exec Scalar"

        /// <summary>
        /// Retrieves the first row resulting from executing the query statement
        /// </summary>
        /// <param name="StoredProcedureName"> The query query to execute</param>
        /// <param name="In"></param>
        /// <returns></returns>
        public object GetScalarBySp(string StoredProcedureName, 
                                ArrayList In)
        {
            //Trace.Enter();

            object result = null;
            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(StoredProcedureName))
                {
                    if (In != null)
                    {
                        for (int i = 0; i < In.Count; i++)
                        {
                            cmd.Parameters.Add((System.Data.SqlClient.SqlParameter)In[i]);
                        }
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    result = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return result;
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetScalar Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd);
                //SRSEx.Add("SQL", StoredProcedureName);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves the first row resulting from executing the query statement
        /// </summary>
        /// <param name="StoredProcedureName"> The query query to execute</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object GetScalarBySp(string StoredProcedureName, DbParameter[] parameters)
        {
            //Trace.Enter();

            object result = null;
            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(StoredProcedureName))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    result = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return result;
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetScalar Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd);
                //SRSEx.Add("SQL", StoredProcedureName);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }


        /// <summary>
        /// Retrieves the first row resulting from executing the query statement
        /// </summary>
        /// <param name="SQL"> The query query to execute</param>
        public object GetScalar(string SQL)
        {
            //Trace.Enter();

            object result = null;
            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL))
                {
                    result = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return result;
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetScalar Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd); 
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }
        }

        /// <summary>
        ///     Retrieves single value resulting from executing the query statement
        ///     and converts object to type T.
        /// </summary>
        /// <param name="sql">The query to execute</param>
        public T GetScalar<T>(string sql)
        {
            //Trace.Enter();
            try
            {
                return (T) GetScalar(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //Trace.Exit();
            }
        }

        /// <summary>
        ///     Retrieves single value resulting from executing the query statement
        ///     and converts object to type T.
        /// </summary>
        /// <param name="sql">The query to execute</param>
        /// <param name="outval">The returned value converted to T</param>
        public bool TryGetScalar<T>(string sql, out T outval)
        {
            //Trace.Enter();
            outval = default(T);
            try
            {
                object result = GetScalar(sql);
                if (result == null)
                {
                    return false;
                }
                outval = (T)result;
                return true;
            }
            catch (Exception ex)
            {
                //Trace.WriteLine("Failed in GetScalar<T>. " + ex.Message);
                return false;
            }
            finally
            {
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves the first row resulting from executing the query statement
        /// </summary>
        /// <param name="SQL"> The query query to execute</param>
        /// <param name="Params"> The array of dbparameters that the query uses</param>
        public object GetScalar(string SQL, DbParameter[] Params)
        {
            //Trace.Enter();

            object result = null;
            SqlCommand cmd = null;

            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL, Params))
                {
                    result = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetScalar Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd); 
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                zCloseConnection(false);
                //Trace.Exit();
            }

            return result;
        }

        #endregion

        #region "Get DataReader"

        /// <summary>
        /// Retrieves an sqlDataReader from executing the query statement
        /// </summary>
        /// <param name="SQL"> The query query to execute</param>
        public DbDataReader GetDataReader(string SQL)
        {
            //Trace.Enter();

            SqlDataReader result = null;
            SqlCommand cmd = null;
            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL))
                {
                    result = cmd.ExecuteReader();
                    m_DbParameters.KeepConnectionOpen = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                try { zCloseConnection(false); }
                catch { }
                //SRSException SRSEx = new SRSException("GetDataReader(string SQL) Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd); 
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves an sqlDataReader from executing the query statement
        /// </summary>
        /// <param name="SQL"> The query query to execute</param>
        /// <param name="Params"> The array of dbparameters that the query uses</param>
        public DbDataReader GetDataReader(string SQL, DbParameter[] Params)
        {
            //Trace.Enter();

            SqlDataReader result = null;
            SqlCommand cmd = null;
            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL, Params))
                {
                    result = cmd.ExecuteReader();
                    m_DbParameters.KeepConnectionOpen = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                try { zCloseConnection(false); }
                catch { }
                //SRSException SRSEx = new SRSException("GetDataReader(string SQL, DbParameter[] Params) Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd); 
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Retrieves an sqlDataReader from executing the Stored Procedure
        /// </summary>
        /// <param name="StoredProcedureName"> The name of the Stored Procedure to execute</param>
        /// <param name="Params"> The array of dbparameters that the query uses</param>
        public DbDataReader GetDataReaderBySp(string StoredProcedureName, DbParameter[] Params = null)
        {
            //Trace.Enter();

            SqlDataReader result = null;
            SqlCommand cmd = null;
            try
            {
                zOpenConnection();
                using (cmd = new SqlCommand(StoredProcedureName))
                {
                    cmd.Connection = (SqlConnection)m_DBConnection;
                    cmd.Transaction = (SqlTransaction)m_Transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = Timeout;
                    if (Params != null)
                    {
                        cmd.Parameters.AddRange(Params);
                    }
                    result = cmd.ExecuteReader();
                    m_DbParameters.KeepConnectionOpen = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                try { zCloseConnection(false); }
                catch { }
                //SRSException SRSEx = new SRSException("GetDataReaderBySp(string StoredProcedureName, DbParameter[] Params = null) Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd);
                //SRSEx.Add("StoredProcedureName", StoredProcedureName);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                //Trace.Exit();
            }
        }

        /// <summary>
        /// Uses SQLDataReader to take data and converts the data into string for output.
        /// On ClientSide this string can be converted into two-dimensional array [columns -query params, rows]
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public string GetDataReaderResultAsString(string SQL)
        {
            //Trace.Enter();
            string result = string.Empty;
            SqlDataReader reader = null;
            SqlCommand cmd = null;
            StringBuilder sbData = new StringBuilder();
            int rowCounter = 0;
            int fieldCounter = 0;
            try
            {
                zOpenConnection();
                using (cmd = zToSqlCommand(SQL))
                {
                    using (reader = cmd.ExecuteReader())
                    {
                        m_DbParameters.KeepConnectionOpen = true;
                        while (reader.Read())
                        {
                            fieldCounter = reader.VisibleFieldCount;
                            for (int i = 0; i < fieldCounter; i++)
                            {
                                if (i == fieldCounter - 1)
                                {
                                    sbData.Append(reader[i].ToString());
                                }
                                else
                                {
                                    sbData.Append(reader[i].ToString() + DelimeterConstants.TABLE_COLUMN_DATA_DELIMETER);
                                }
                            }
                            rowCounter++;
                            sbData.Append(DelimeterConstants.TABLE_ROW_DATA_DELIMETER);
                        }
                        string res = sbData.ToString();
                        result = res.Substring(0, res.LastIndexOf(DelimeterConstants.TABLE_ROW_DATA_DELIMETER));
                    }
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("GetDataReaderAsString(string SQL, DbParameter[] Params) Method failed within DataBroker class.", ex);
                //zParamsToException(ref SRSEx, cmd); 
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                //SRSEx.Add(result);
                //SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                reader.Close();
                zCloseConnection(false);
                //Trace.Exit();
            }

            return result;
        }

        #endregion

        #endregion

        #region Private Methods

        #region Errors Support for Parameters


        //Used to format Parameters and pass them into Exceptions for testing
       

        #endregion

        /// <summary>
        /// Create Connection object and do Open action on Connection object.
        /// </summary>
        protected override void zOpenConnection()
        {
            //Trace.Enter();

            try
            {
                if (zDatabaseClosed() == true)
                {
                    if (m_DBConnection == null)
                    {
                        m_DBConnection = new SqlConnection(m_DbParameters.ConnectionString);
                    }
                    m_DBConnection.Open();
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("Failed to Open Database Connection.", ex);
                ////SRSEx.Add("ConnectionString", m_DbParameters.ConnectionString);
                ////SRSEx.Add(m_DBConnection);
                ////SRSEx.Add(this);
                throw ex;
            }
            finally
            {
                //Trace.Exit();
            }
        }

        private SqlCommand zToSqlCommand(String SQL)
        {
            return zInitSqlCommand(SQL);
        }

        /// <summary>
        /// Packing parameters into SqlCommand
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        private SqlCommand zToSqlCommand(String SQL, DbParameter[] Params)
        {
            SqlCommand cmd = null;

            try
            {
                using (cmd = zInitSqlCommand(SQL))
                {
                    if (Params != null)
                    {
                        cmd.Parameters.AddRange(Params);
                    }

                    return cmd;
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("zToSqlCommand failed", ex);
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                //SRSEx.Add(this);
                //SRSEx.Add(Params);
                throw ex;
            }
        }

        private SqlCommand zToSqlCommand(string SQL, System.Collections.ArrayList In)
        {
            SqlCommand cmd = null;
            try
            {
                using (cmd = zInitSqlCommand(SQL))
                {
                    if (In != null)
                    {
                        for (int i = 0; i < In.Count; i++)
                        {
                            cmd.Parameters.Add((System.Data.SqlClient.SqlParameter)In[i]);
                        }
                    }

                    return cmd;
                }
            }

            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("zToSqlCommand failed", ex);
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                //SRSEx.Add(this);
                //SRSEx.Add(In);
                throw ex;
            }
        }

        private SqlCommand zInitSqlCommand(string SQL)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SQL;
                cmd.Connection = (SqlConnection)m_DBConnection;
                cmd.Transaction = (SqlTransaction)m_Transaction;
                cmd.CommandTimeout = Timeout;
                return cmd;
            }

            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("zInitSqlCommand failed", ex);
                //SRSEx.Add("SQL", SQL);
                //SRSEx.Add(cmd);
                throw ex;
            }
        }

        /// <summary>
        /// Method for run Stored procedure 
        /// </summary>
        /// <param name="StoreProcedureName">Store Procedure Name string value</param>
        /// <returns>return SqlCommand</returns>
        private SqlCommand zToSpCommand(String StoreProcedureName)
        {
            return zToSpCommand(StoreProcedureName, (DbParameter[])null);//Cast to prevent ambiguity
        }

        /// <summary>
        /// Packing parameters into SqlCommand for stored procedure 
        /// </summary>
        /// <param name="StoreProcedureName">StoreProcedureName string value</param>
        /// <param name="In">Params</param>
        /// <returns>return SqlCommand type</returns>
        private SqlCommand zToSpCommand(String StoreProcedureName, System.Collections.ArrayList In)
        {
            SqlCommand cmd = null;

            try
            {
                using (cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = StoreProcedureName;
                    cmd.Connection = (SqlConnection)m_DBConnection;
                    cmd.Transaction = (SqlTransaction)m_Transaction;
                    cmd.CommandTimeout = Timeout;
                    if (In != null)
                    {
                        for (int i = 0; i < In.Count; i++)
                        {
                            cmd.Parameters.Add((System.Data.SqlClient.SqlParameter)In[i]);
                        }
                    }

                    return cmd;
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("zToSpCommand failed", ex);
                //zParamsToException(ref SRSEx, cmd); 
                //SRSEx.Add("Store Procedure Name", StoreProcedureName);
                //SRSEx.Add(cmd);
                //SRSEx.Add(this);
                //SRSEx.Add(In);
                throw ex;
            }
        }

        /// <summary>
        /// Packing parameters into SqlCommand for stored procedure 
        /// </summary>
        /// <param name="StoreProcedureName">StoreProcedureName string value</param>
        /// <param name="Params">Params DbParameter[] type</param>
        /// <returns>return SqlCommand type</returns>
        private SqlCommand zToSpCommand(String StoreProcedureName, DbParameter[] Params)
        {
            SqlCommand cmd = null;

            try
            {
                using (cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = StoreProcedureName;
                    cmd.Connection = (SqlConnection)m_DBConnection;
                    cmd.Transaction = (SqlTransaction)m_Transaction;
                    cmd.CommandTimeout = Timeout;
                    if (Params != null)
                    {
                        cmd.Parameters.AddRange(Params);
                    }

                    return cmd;
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("zToSpCommand failed", ex);
                //zParamsToException(ref SRSEx, cmd);
                //SRSEx.Add("Store Procedure Name", StoreProcedureName);
                //SRSEx.Add(cmd);
                //SRSEx.Add(this);
                //SRSEx.Add(Params);
                throw ex;
            }
        }


        /// <summary>
        /// Returns command type
        /// </summary>
        /// <param name="sbSQL"></param>
        /// <returns></returns>
        private DatabrokerCommandType zGetDatabrokerCommandType(ref StringBuilder sbSQL)
        {
            const string SELECT = "SELECT ";
            const string DELETE = "DELETE ";
            const string UPDATE = "UPDATE ";
            const string INSERT = "INSERT ";

            try
            {
                zTrim(sbSQL, 0);
                if (System.Char.IsLetterOrDigit(sbSQL[6]) || sbSQL[6] == '_')
                {
                    // Select etc are 6 characters. this must be a stored procedure or other
                    return DatabrokerCommandType.Other;
                }

                switch (sbSQL.ToString(0, 7).ToUpper())
                {
                    case SELECT:   // Select statement
                        return DatabrokerCommandType.Select;
                    case DELETE:   // Delete 
                        return DatabrokerCommandType.Delete;
                    case UPDATE:   // Update
                        return DatabrokerCommandType.Update;
                    case INSERT:   // Insert
                        return DatabrokerCommandType.Insert;
                    default:
                        return DatabrokerCommandType.Other;
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("zGetDatabrokerCommandType failed", ex);
                //SRSEx.Add("sbSQL", sbSQL);
                throw ex;
            }
        }

        /// <summary>
        /// Helper method
        /// </summary>
        /// <param name="sB"></param>
        /// <param name="Start"></param>
        private void zTrim(StringBuilder sB, int Start)
        {
            int Index = 0;

            try
            {
                //Find first non-whitespace
                for (Index = Start; Index < sB.Length && System.Char.IsWhiteSpace(sB[Index]); Index++)
                {
                }
                if (Index > Start)
                {
                    // zTrim white space
                    sB.Remove(Start, Index - Start);
                }
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("zTrim failed", ex);
                //SRSEx.Add("sB", sB);
                //SRSEx.Add("Index", Index);
                throw ex;
            }
        }
        #endregion

        public void ExecSQL(StringBuilder sb, DbParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        
    }

    #region Extension Methods

    public static class DataBrokerExtensions
    {
        /// <summary>
        /// Convenience method to infer the SqlDbType. 
        /// </summary>
        /// <param name="inList"></param>
        /// <param name="sName"></param>
        /// <param name="oValue"></param>
        /// <param name="direction"></param>
        public static void AddParameter(this ArrayList inList, string sName, object oValue, ParameterDirection direction = ParameterDirection.Input)
        {
            try
            {
                //Trace.Enter();
                SqlDbType st = SqlDbType.VarChar;
                if (oValue is string)
                {
                    st = SqlDbType.VarChar;
                }
                else if (oValue is int)
                {
                    st = SqlDbType.Int;
                }
                else if (oValue is bool)
                {
                    st = SqlDbType.Bit;
                }
                else if (oValue is float || oValue is double)
                {
                    st = SqlDbType.Float;
                }
                else if (oValue is DateTime)
                {
                    st = SqlDbType.DateTime;
                    //Smallest Date supported by SQL
                    if (oValue != null && ((DateTime)oValue) < new DateTime(1753, 1, 1))
                    {
                        //Don't swallow exceptions
                        throw new Exception("SQL Server does not support dates before January 1, 1753");
                    }
                }
                else if (oValue is DateTime?)
                {
                    st = SqlDbType.DateTime;
                    //Smallest Date supported by SQL
                    if (oValue != null && ((DateTime?)oValue).Value < new DateTime(1753, 1, 1))
                    {
                        //Don't swallow exceptions
                        throw new Exception("SQL Server does not support dates before January 1, 1753");
                    }
                }
                else if (oValue is Guid)
                {
                    st = SqlDbType.UniqueIdentifier;
                }
                else if (oValue is Guid?)
                {
                    st = SqlDbType.UniqueIdentifier;
                }

                DataBrokerSql.AddParameter(inList, direction, sName, st, oValue);
                
            }
            catch (Exception ex)
            {
                //SRSException SRSEx = new SRSException("AddParam Failed.", ex);
                //SRSEx.Add("sName", sName);
                //SRSEx.Add("oValue", oValue);
                throw ex;
            }
            finally
            {
                //Trace.Exit();
            }
        }

    }

    #endregion

    public class DelimeterConstants
    {
        public const string TABLE_ROW_DATA_DELIMETER = "^^^^";
        public const string TABLE_COLUMN_DATA_DELIMETER = "^^^";
        public const string TABLE_DELIMETER = "^^^^^";
        public const string METADATA_DELIMETER = "^^";
        public const string BLANK_DATA = "BLANK";

    }
}
