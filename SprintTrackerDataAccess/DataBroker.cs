using System;
using System.Collections.Generic;
using System.Text;
using db = System.Data;
using System.Data.Common;
using System.Configuration;


namespace SprintTrackerDataAccess
{
    public struct DbParameters
    {
        public int Timeout;
        public bool ReadOnly;
        public bool LocalOnly;
        public bool KeepConnectionOpen;
        public string ConnectionString;
    }


    public abstract class DataBroker : IDisposable    
    {
        protected DbParameters m_DbParameters;
        protected DbConnection m_DBConnection = null;
        protected DbTransaction m_Transaction = null;

        #region Protected constructor

        protected DataBroker() : this(new DbParameters()) {}

        protected DataBroker(DbParameters dbParameters)
        {
            try
            {
                if (dbParameters.ConnectionString != null && dbParameters.ConnectionString.Length > 0)
                {
                    ConnectionString = dbParameters.ConnectionString; // Use property to force username+password replacement
                }
                else
                {
                    string defaultConn = ConfigurationManager.AppSettings["ApplicationDb"].ToString();
                    m_DbParameters.ConnectionString = ConfigurationManager.ConnectionStrings[defaultConn].ToString();

                    if (!string.IsNullOrEmpty(m_DbParameters.ConnectionString))
                    {
                        ConnectionString = m_DbParameters.ConnectionString;
                    }
                    else
                    {
                        throw new Exception("Please provide connection string");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            if (dbParameters.Timeout > 0)
            {
                m_DbParameters.Timeout = dbParameters.Timeout;
            }
            else
            {
                m_DbParameters.Timeout = 300; // 5 minutes
            }
 
            if (dbParameters.ReadOnly)
            {
                m_DbParameters.ReadOnly = true;
            }
            else
            {
                m_DbParameters.ReadOnly = false;
            }

            if (dbParameters.LocalOnly)
            {
                m_DbParameters.LocalOnly = true;
            }
            else
            {
                m_DbParameters.LocalOnly = false;
            }
            if (dbParameters.KeepConnectionOpen)
            {
                m_DbParameters.KeepConnectionOpen = true;
            }
            else
            {
                m_DbParameters.KeepConnectionOpen = false;
            }
        }

        #endregion

        #region Implemented properties

        public DbParameters DataBaseConfigParams
        {
            get
            {
                return m_DbParameters;
            }
            set
            {
                m_DbParameters = value;
            }
        }

        public int Timeout
        {
            get { return m_DbParameters.Timeout; }
            set { m_DbParameters.Timeout = value; }
        }

        public bool ReadOnly
        {
            get { return m_DbParameters.ReadOnly; }
            set { m_DbParameters.ReadOnly = value; }
        }

        public bool KeepConnectionOpen
        {
            get { return m_DbParameters.KeepConnectionOpen; }
            set { m_DbParameters.KeepConnectionOpen = value; }
        }

        public bool LocalOnly
        {
            get { return m_DbParameters.LocalOnly; }
            set { m_DbParameters.LocalOnly = value; }
        }

        public string ConnectionString
        {
            get { return m_DbParameters.ConnectionString; }
            set {
                try
                {

                    m_DbParameters.ConnectionString = "Data Source = (local); Initial Catalog = ProgressTracker; User ID = UserName; Password=Password";

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                }
            }
        }

        public bool InTrans
        {
            get { return (m_Transaction != null); }
        }

        public string Database
        {
            get
            {
                try
                {
                    return m_DBConnection.Database;
                }
                catch
                {
                    return "";
                }
            }
        }

        public string Datasource
        {
            get
            {
                try
                {
                    return m_DBConnection.DataSource;
                }
                catch
                {
                    return "";
                }
            }
        }

        #endregion

        #region Public methods

        public void BeginTrans()
        {
            try
            {
                if (m_DBConnection == null)
                    this.zOpenConnection();

                m_Transaction = m_DBConnection.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CommitTrans()
        {
            try
            {
                if (m_Transaction != null)
                {
                    m_Transaction.Commit();
                    m_Transaction = null;
                    zCloseConnection(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RollbackTrans()
        {
            try
            {
                if (m_Transaction != null)
                {
                    m_Transaction.Rollback();
                    m_Transaction = null;
                    zCloseConnection(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CloseConnection()
        {
           // Trace.Enter();

            try
            {
                zCloseConnection(true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
               // Trace.Exit();
            }
        }
        #endregion

        #region Protected Methods

        protected abstract void zOpenConnection();

        protected void zCloseConnection(bool ForceClose)
        {
           // Trace.Enter();

            try
            {
                if (!ForceClose)
                {
                    // If the caller is not requesting a forced closure, then
                    // actual closure will depend on either the KeepConnectionOpen
                    // flag, or the transactional state. If either of these contravenes
                    // the casual closure request, then the closure request is ignored.
                    if (m_DbParameters.KeepConnectionOpen || m_Transaction != null)
                    {
                        return;
                    }
                }

                if (zDatabaseClosed() == false)
                {
                    m_DBConnection.Close();
                }

                if (m_DBConnection != null)
                {
                    m_DBConnection.Dispose();
                    m_DBConnection = null;
                }

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

        protected bool zDatabaseClosed()
        {
           // Trace.Enter();

            try
            {
                if (m_DBConnection == null)
                {
                    return true;
                }

                if (m_DBConnection.State != System.Data.ConnectionState.Open)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            finally
            {
               // Trace.Exit();
            }
        }

       
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_Transaction != null)
            {
                RollbackTrans();
                if (m_Transaction != null)
                {
                    m_Transaction.Dispose();
                    m_Transaction = null;
                }
            }
            zCloseConnection(true);
        }

        #endregion

    }
}
