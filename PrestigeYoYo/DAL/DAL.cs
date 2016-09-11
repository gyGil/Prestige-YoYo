/// \file DAL
///
/// \mainpage Advanced SQL (PROG3070) Milestone 01 - Data Access Layer
///
/// \section intro Program Introduction
/// - This is the data access layer which is used for handling the Prestige YOYO
///   manufacturing data within the queue backload and the SQL database. 
///   
/// \reference
/// - Advanced SQL Modules and Demos
///   Norbert Mika: Demo code
///
/// Major <b>DAL.cs</b>
/// \section version Current version of this Program
/// <ul>
/// <li>\authors  
///	<li>          Marcus Rankin (3379187) & Geun Young Gil (6944920)</li>	
/// <li>\version  1.00.00</li>
/// <li>\date     2016.03.17</li>
/// <li>\pre      Nothing
/// <li>\warning  Nothing
/// <li>\copyright    Geun Young Gil & Marcus Rankin
/// <ul>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using System.Data;
using System.Text.RegularExpressions;

namespace DALlib
{
    public class DAL
    {
        private static SqlConnection conn;      ///< SQL connection
        private static List<Part> queueList;    ///< Data backup queue
        private static Part yoyo;               ///< Yoyo part data
        public int transactSize;                ///< Dynamic transaction size
        private Thread transactThread;          ///< Transaction Thread

        private string connectionString;        ///< Connection string
        private string computerName;            ///< Users computer name
        private string userName;                ///< Username (will be dynamic when user interface is complete)
        private string password;                ///< Password (will be dynamic when user interface is complete)
        public static string sqlException;      ///< For returning SQL exception messages to the Reader screen

        private Boolean connStatus;             ///< For monitoring current database conneciton status

        public DAL()
        {
            computerName = Environment.MachineName; ///< Get users computer name
            queueList = new List<Part>();                                       ///< Create backup data list made up of Yoyo part data
            sqlException = "";                                                  ///< Initialize exception string
            connStatus = false;                                                 ///< Initialize conneciton status
            transactSize = 10;                                                  ///< Set initial transaction size to 10
        }

        /// \brief  OpenConnection
        ///
        /// \details <b>Details</b>
        /// - Opens database connection
        ///
        /// \param userName - <b>string</b> - username
        /// \param password - <b>string</b> - password
        ///
        /// \return <b>Boolean</b> - connection status
        public Boolean OpenConnection(string userName, string password)
        {
            connectionString = "Data Source=" + computerName + "\\SQLEXPRESS;Initial Catalog=yoyo;User id=" + userName + ";Password=" + password + ";";

            conn = new SqlConnection(connectionString);
            try
            {               
                conn.Open();
                connStatus = true;
            } 
            catch (SqlException ex)
            {
                connStatus = false;
                sqlException = ex.ToString();
            }

            return connStatus;
        }

        /// \brief  CloseConnection
        ///
        /// \details <b>Details</b>
        /// - Closes database connection and pushes out remaing queue data into
        ///   the database before closing as to not lose any data.
        ///
        /// \param N/A - <b>N/A</b> - N/A
        ///
        /// \return <b>Boolean</b> - connection status
        public Boolean CloseConnection()
        {
            // Check if there is still data in the queue beforing closing
            if (queueList.Count > 0)
            {
                // Set sqlexception string to closed to notify database thread to clean up rest of queue before closing
                sqlException = "Connection Closed";

                // Execute database transaction thread
                InsertTransactionThread transact = new InsertTransactionThread();
                transactThread = new Thread(() => transact.SqlTransaction(queueList, conn, sqlException, transactSize));
                transactThread.Start();

                transactThread.Join();
            }

            // Check if all data was pushed from the queue and into the database
            if (queueList.Count == 0)
            {
                try
                {
                    conn.Close();
                    connStatus = false;
                    
                }
                catch (SqlException ex)
                {
                    connStatus = true;
                    sqlException = ex.ToString();
                }
                catch (Exception e)
                {
                    connStatus = true;
                    sqlException = e.ToString();
                }
            }

            return connStatus;
        }

        /// \brief  AddManufacturingData
        ///
        /// \details <b>Details</b>
        /// - Add manufacturing data into the data queue (list)
        ///
        /// \param dataString - <b>string</b> - manufacturing data string
        ///
        /// \return queueList.Count <b>int</b> - return size of queue
        public int AddManufacturingData(string dataString)
        {
            // Separate data into separate elements for queue entry
            string[] dataElements = dataString.Split(',');

            // Split and fill data elements
            for (int i = 0; i < dataElements.Length; i++)
            {
                dataElements[i] = dataElements[i].Trim();
            }

            // intialize reject and rework to zero
            int reject = 0;
            int rework = 0; 

            // Check if yoyo part was rejected or reworked
            if (dataElements[4] != "") 
            {
                bool rejection = dataElements[3].Contains("SCRAP");
                bool reworked = dataElements[3].Contains("REWORK");
                if (rejection) { reject = 1; }
                else if (reworked) { rework = 1; }
            }

            /***************** ADD TO QUEUE LIST BEFORE INSERTING AND COMMITTING TRANSACTIONAL INSERT BATCHES *********************/
            try
            {
                // Add yoyo part to list
                yoyo = new Part(dataElements[0], dataElements[1], dataElements[2], dataElements[3], dataElements[4], dataElements[5], reject, rework);
                queueList.Add(yoyo);
            } 
            catch (Exception ex)
            {
                sqlException = ex.ToString();
            }
            
            return queueList.Count;
        }

        /// \brief  AddDataToDatabase
        ///
        /// \details <b>Details</b>
        /// - Insert transactional batch of data into database once transaction size is met.
        ///   Creates separate transaction thread to continue collecting data while database
        ///   interaction takes place.
        ///
        /// \param transactSize - <b>int</b> - transaction size
        ///
        /// \return <b>int</b> - transaction size
        public int AddDataToDatabase (int transactSize)
        {
            // Check if connection is still open
            if (connStatus == true)
            {
                InsertTransactionThread transact = new InsertTransactionThread();
                Thread transactThread = new Thread(() => transact.SqlTransaction(queueList, conn, sqlException, transactSize));
                transactThread.Start();
                transactThread.Join();
            }
            // If user has closed connection or shutdown program signal final data push to database before closing
            else if (connStatus == false)
            {
                sqlException = "Connection Closed";
                InsertTransactionThread transact = new InsertTransactionThread();
                Thread transactThread = new Thread(() => transact.SqlTransaction(queueList, conn, sqlException, transactSize));
                transactThread.Start();
                transactThread.Join();
            }

            return transactSize;
        }

        /// \brief  ManufacturingYields
        ///
        /// \details <b>Details</b>
        /// - For returning manufacturing database information. 
        ///   ***Need to implement when final user interface is built. (not current temporary UI) ****
        ///
        /// \param query data - <b>all query settings</b> - query settings set by user
        ///
        /// \return <b>void</b> - N/A
        public void ManufacturingYields(int type, string startDate, string stopDate, string product, string colour, string defect)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        //===========================================================[ FOR RRESTIGE YOYO CLASS ]===========================================================//
        /// \brief  AuthenticateUser
        ///
        /// \details <b>Details</b>
        /// - Authenticate user whether is user or admin.
        ///
        /// \param id - <b>string</b>
        /// \param password - <b>string</b>
        ///
        /// \return <b>int</b> - fail: -1 , user: 0, admin: 1
        public int AuthenticateUser(string id, string password, SqlConnection conn)
        {
            if (conn == null) return -2;

            int ret = -1;
            using(SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT isADMIN FROM users WHERE userID = @userID AND userPassword = @userPassword;";
                cmd.Parameters.AddWithValue("@userID", id);
                cmd.Parameters.AddWithValue("@userPassword", password);

                conn.Open();
                object o = cmd.ExecuteScalar();
                conn.Close();

                if (o != null)
                {
                    if (!int.TryParse(o.ToString(), out ret))   // if tryparse fail, out 0.
                        ret = -1;
                }
            }
            
            return ret;
        }

        public DataTable GetModelList(SqlConnection conn)
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("model");
            tbl.Columns.Add("model number");

            SqlDataReader sdr;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT sku FROM products";

            conn.Open();
            sdr = cmd.ExecuteReader();

            int i = 0;
            while (sdr.Read())
            {
                DataRow dr = tbl.NewRow();
                dr[0] = ++i;
                dr[1] = sdr[0];
                tbl.Rows.Add(dr);
            }

            sdr.Close();
            conn.Close();

            return tbl;
        }

        /// <summary>
        /// Total 1 to 3 : the total number of passing station 1 to 3
        /// Station 1 to 3: the defect/reject number of passing station 1 to 3
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public Dictionary<string, int> QueryFirstYieldTotal(SqlConnection conn)
        {
            if (conn == null) return null;
            Dictionary<string, int> dic = this.QueryFirstYield(conn);

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT COUNT(*) FROM manufacturingData WHERE station = @inspection";

                // Get the reject/rework numbers for each station and put into Dictionary 
                for (int i = 1; i <= 3; ++i)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@inspection", "INSPECTION_" + i);
   

                    conn.Open();
                    object o = cmd.ExecuteScalar();
                    conn.Close();

                    int ret = -1;
                    if (o != null)
                    {
                        if (!int.TryParse(o.ToString(), out ret))   // if tryparse fail, out 0.
                            ret = -1;
                    }

                    dic.Add("Total " + i.ToString(), ret);
                }

                return dic;
            }
        }

        /// <summary>
        /// Total 1 to 3 : the total number of passing station 1 to 3
        /// Station 1 to 3: the reject number of passing station 1 to 3
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public Dictionary<string, int> QueryFinalYieldTotal(SqlConnection conn)
        {
            if (conn == null) return null;
            Dictionary<string, int> dic = this.QueryFinalYield(conn);

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT COUNT(*) FROM manufacturingData WHERE station = @inspection";

                // Get the reject/rework numbers for each station and put into Dictionary 
                for (int i = 1; i <= 3; ++i)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@inspection", "INSPECTION_" + i);


                    conn.Open();
                    object o = cmd.ExecuteScalar();
                    conn.Close();

                    int ret = -1;
                    if (o != null)
                    {
                        if (!int.TryParse(o.ToString(), out ret))   // if tryparse fail, out 0.
                            ret = -1;
                    }

                    dic.Add("Total " + i.ToString(), ret);
                }

                return dic;
            }
        }

        /// <summary>
        /// It query to DB to get rework/reject numbers on each station  and return on dictionary
        /// </summary>
        /// <param name="conn"></param>
        /// <returns>Dictionary (ex) Key: station_1, Value:1234</returns>
        public Dictionary<string, int> QueryFirstYield(SqlConnection conn)
        {
            if (conn == null) return null;
            Dictionary<string, int> dic = new Dictionary<string, int>();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT COUNT(*) FROM manufacturingData WHERE station = @rework OR station = @scrap";
              
                // Get the reject/rework numbers for each station and put into Dictionary 
                for (int i = 1; i <= 3; ++i )
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@rework", "INSPECTION_" + i + "_REWORK");
                    cmd.Parameters.AddWithValue("@scrap", "INSPECTION_" + i +"_SCRAP");

                    conn.Open();
                    object o = cmd.ExecuteScalar();
                    conn.Close();

                    int ret = -1;
                    if (o != null)
                    {
                        if (!int.TryParse(o.ToString(), out ret))   // if tryparse fail, out 0.
                            ret = -1;
                    }

                    dic.Add("Station " + i.ToString(), ret);
                }

                return dic;
            }
        }

        public Dictionary<string, int> QueryFinalYield(SqlConnection conn)
        {
            if (conn == null) return null;
            Dictionary<string, int> dic = new Dictionary<string, int>();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT COUNT(*) FROM manufacturingData WHERE station = @scrap";

                // Get the reject/rework numbers for each station and put into Dictionary 
                for (int i = 1; i <= 3; ++i)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@scrap", "INSPECTION_" + i + "_SCRAP");

                    conn.Open();
                    object o = cmd.ExecuteScalar();
                    conn.Close();

                    int ret = -1;
                    if (o != null)
                    {
                        if (!int.TryParse(o.ToString(), out ret))   // if tryparse fail, out 0.
                            ret = -1;
                    }

                    dic.Add("Station " + i.ToString(), ret);
                }

                return dic;
            }
        }

        public Dictionary<int, int> QueryDefectCategories(SqlConnection conn)
        {
            if (conn == null) return null;
            Dictionary<int, int> dic = new Dictionary<int, int>();
            string[] defectCate = {"INCONSISTENT_THICKNESS","PITTING","WARPING",
                                  "PRIMER_DEFECT","DRIP_MARK","FINAL_COAT_FLAW",
                                  "BROKEN_SHELL","BROKEN_AXLE","TANGLED_STRING" };
            const int NUM_DEFECT = 9;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT COUNT(*) FROM manufacturingData WHERE resultState = @defect";

                // Get numbers of defect categories
                for (int i = 0; i < NUM_DEFECT; ++i)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@defect", defectCate[i]);

                    conn.Open();
                    object o = cmd.ExecuteScalar();
                    conn.Close();

                    int ret = -1;
                    if (o != null)
                    {
                        if (!int.TryParse(o.ToString(), out ret))   // if tryparse fail, out 0.
                            ret = -1;
                    }

                    dic.Add(i, ret);
                }

                return dic;
            }
        }

        public Dictionary<int, string> AddSchedule(SqlConnection conn, string startDate, string endDate, string model)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();

            if (conn == null)
            {
                dic.Add(-1, "Connection is failed.");
                return dic;
            }

            if(!Regex.IsMatch(startDate,@"^1?[0-9]\/[1-3]?[0-9]\/\d\d\d\d\s\d\d:\d\d:\d\d\s(AM|PM)$") ||
                !Regex.IsMatch(endDate, @"^1?[0-9]\/[1-3]?[0-9]\/\d\d\d\d\s\d\d:\d\d:\d\d\s(AM|PM)$"))
            {
                dic.Add(-1, "Invalid date format.");
                return dic;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"INSERT INTO schedules (startDate,endDate,sku) 
	                                    VALUES (@startDate,@endDate,@model)";

                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", startDate);
                cmd.Parameters.AddWithValue("@model", model);

                

                try
                {
                    // Attempt to execute single INSERT query
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    dic.Add(1, "Success to input new schedule.");
                }
                catch (SqlException singleInsertException)
                {
                    // Save exception string and throw to outer loop exception
                    sqlException += singleInsertException.ToString();
                    dic.Add(-1, singleInsertException.ToString());
                    //throw (singleInsertException);
                }
            }

            return dic;
        }

        //===========================================================[ END FOR RRESTIGE YOYO CLASS ]===========================================================//
    }

     public class Part
    {
        public Part(string work, string sin, string line, string station, string state, string time, int reject, int rework)
        {
            workArea = work;
            sinNumber = sin;
            this.line = line;
            this.station = station;
            resultState = state;
            actionTimeStamp = time;
            this.reject = reject;
            this.rework = rework;
        }

        public string workArea { get; set; }
        public string sinNumber { get; set; }
        public string line { get; set; }
        public string station { get; set; }
        public string resultState { get; set; }
        public string actionTimeStamp { get; set; }
        public int reject { get; set; }
        public int rework { get; set; }
    }

    /// \brief  InsertTransactionThread
    ///
    /// \details <b>Details</b>
    /// - Database transaction thread
    public class InsertTransactionThread
    {
        /// \brief  SqlTransaction
        ///
        /// \details <b>Details</b>
        /// - Thread function for database transactions
        ///
        /// \param queueList - <b>List<Part></b> - data queue
        /// \param conn - <b>SqlConnection</b> - SQL connection
        /// \param sqlException - <b>string</b> - SQL exception string for messaging
        /// \param transactSize - <b>int</b> - transaction size
        ///
        /// \return <b>void</b> - N/A
        public void SqlTransaction(List<Part> queueList, SqlConnection conn, string sqlException, int transactSize)
        {
            SqlCommand cmd = new SqlCommand();
            string query;
            
            // Begin the transaction
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            
            SqlTransaction transaction = conn.BeginTransaction();
            
            // set connection and transaction
            cmd.Connection = conn;
            cmd.Transaction = transaction;

            // Build INSERT transaction 
            if (sqlException != "Connection Closed")
            {
                try
                {
                    // Do transaction size amount of inserts
                    for (int i = 0; i < transactSize; ++i)
                    {
                        query = "INSERT INTO manufacturingData (workArea, sinNumber, line, station, resultState, actionTimeStamp, reject, rework)" +
                                        "VALUES('" + queueList[i].workArea + "', '" + queueList[i].sinNumber + "', '" + queueList[i].line + "', '" +
                                        queueList[i].station + "', '" + queueList[i].resultState + "', '" + queueList[i].actionTimeStamp + "', " + 
                                        queueList[i].reject + ", " + queueList[i].rework + ");";

                        try
                        {
                            // Attempt to execute single INSERT query
                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException singleInsertException)
                        {
                            // Save exception string and throw to outer loop exception
                            sqlException += singleInsertException.ToString();

                            throw (singleInsertException);
                        }
                    }

                    // Commit transaction after INSERT loop completed
                    transaction.Commit();
                    queueList.RemoveRange(0, transactSize);
                }
                catch (SqlException completeInsertException)
                {
                    // Rollback transaction if exception thrown
                    transaction.Rollback();

                    sqlException += completeInsertException.ToString();
                }
            }
            else       // Only executed when application is closed and data remains in the queue list
            {
                try
                {
                    int dataLeft = queueList.Count;
                    // Do remaining INSERTS
                    for (int i = 0; i < queueList.Count; ++i)
                    {
                        query = "INSERT INTO manufacturingData (workArea, sinNumber, line, station, resultState, actionTimeStamp, reject, rework)" +
                                        "VALUES('" + queueList[i].workArea + "', '" + queueList[i].sinNumber + "', '" + queueList[i].line + "', '" +
                                        queueList[i].station + "', '" + queueList[i].resultState + "', '" + queueList[i].actionTimeStamp + "', " +
                                         queueList[i].reject + ", " + queueList[i].rework + ");";
                        try
                        {
                            // Attempt to execute single INSERT query
                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException singleInsertException)
                        {
                            // Save exception string and throw to outer loop exception
                            sqlException += singleInsertException.ToString();

                            throw (singleInsertException);
                        }
                    }

                    // Commit transaction after INSERT loop completed
                    transaction.Commit();
                    queueList.RemoveRange(0, dataLeft);
                }
                catch (SqlException completeInsertException)
                {
                    // Rollback transaction if exception thrown
                    transaction.Rollback();

                    sqlException += completeInsertException.ToString();
                }
            }

            // End the transaction
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}
