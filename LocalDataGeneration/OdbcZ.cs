using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocalDataGeneration
{
    #region ODBCZ Constants
    public static class OdbcZ
    {
        public static ODBCHandler defaultConnection;
    }
    #endregion


    public class ODBCHandler
    {
        #region ODBC Handler Instance variables
        public string DSN { get; set; }
        public string Driver { get; set; }
        public string Server { get; set; }
        public string Uid { get; set; }
        public string Pwd { get; set; }
        public string Database { get; set; }
        #endregion

        private OdbcConnection connection;

        public ODBCHandler (string driver,string server,string uid,string pwd,string database)
        {
            Driver = driver;
            Server = server;
            Uid = uid;
            Pwd = pwd;
            Database = database;
        }
        public ODBCHandler (IEnumerable<XElement> elements)
        {
            DSN = elements.ElementAt(0).Value.ToString();
            Driver = elements.ElementAt(1).Value.ToString();
            Server = elements.ElementAt(2).Value.ToString();
            Uid = elements.ElementAt(3).Value.ToString();
            Pwd = elements.ElementAt(4).Value.ToString();
            Database = elements.ElementAt(5).Value.ToString();
        }
        private string GenerateODBCConnectionString ()
        {
            string connectionString;
            if(DSN != "")
            {
                connectionString = "DSN=" + DSN + ";";
                if(Database != "")
                {
                    connectionString += "database=" + Database + ";";
                }
                
            }
            else if(Driver != "" & Server != "" & Uid != "" & Database != "")
            {
                connectionString = "driver=" + Driver + ";server=" + Server + ";uid=" + Uid + ";pwd=" + Pwd + ";database=" + Database + ";";
            }
            else
            {
                LoggerZ.FatalError("All Connection Data was not set!");
                return "";
            }
            return connectionString;

        }
        public void Connect ()
        {
            try
            {
                connection = new OdbcConnection(GenerateODBCConnectionString());
                connection.Open();
                LoggerZ.Log(String.Format("Connection opened: {0}",connection.ConnectionString),Level.Debug,LogTarget.Console,LogTarget.File);
            }
            catch(Exception e)
            {
                LoggerZ.Log(String.Format("could not open ODBC connection with paramenters:{0}\n{1}",GenerateODBCConnectionString(),e),Level.Fatal,LogTarget.Console,LogTarget.File);
                Console.ReadLine();
                Environment.Exit(160);
            }
        }
        public OdbcDataReader RunQuery (string command)
        {
            if(connection == null)
            {
                Connect();
            }

            OdbcCommand Query = new OdbcCommand(command,connection);

            OdbcDataReader reader = Query.ExecuteReader();
            return reader;
        }
        public void CreateTable (string tableName,List<SQLColumn> fields)
        {
            LoggerZ.Log(string.Format("Creating {0}",tableName),Level.Debug,LogTarget.Console,LogTarget.File);
            string createString = string.Format("IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{0}' AND xtype='U') CREATE TABLE {0} (",tableName);
            foreach(SQLColumn field in fields)
            {
                createString += string.Format("[{0}] varchar({1}) NULL,",field.Name,( field.Length > 8000 ? "MAX" : field.Length.ToString() ));
            }
            RunQuery(createString.TrimEnd(',') + ");");
            LoggerZ.Log(string.Format("Created {0}",tableName),Level.Debug,LogTarget.Console,LogTarget.File);
        }
        public void DropTable (string tableName)
        {
            RunQuery(string.Format("IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = '{0}' AND xtype = 'U') DROP TABLE {0}",tableName));
        }
        public void BulkInsert (string tableName,DataTable data)
        {
            string columns = "";
            foreach(DataColumn column in data.Columns)
            {
                columns += "[" + column.ColumnName + "],";
            }
            string insertString = string.Format("INSERT INTO {0}({1}) Values ",tableName,columns.TrimEnd(','));
            int rowCounter = 0;
            string rowString = "";
            foreach(DataRow row in data.Rows)
            {
                rowString += "(";
                foreach(object item in row.ItemArray)
                {
                    rowString += "'" + item.ToString().Trim(' ').Replace("'","''") + "',";
                }
                rowString = rowString.TrimEnd(',') + "),";
                rowCounter++;
                if(rowCounter % SQLZ.bulkInsertCount == 0)
                {
                    RunQuery(string.Format("{0}{1}",insertString,rowString.TrimEnd(',')));
                    rowString = "";
                    LoggerZ.Log(string.Format("Inserted {0} rows into {1}",rowCounter,tableName),Level.Debug,LogTarget.Console);
                }
            }
            if(rowCounter % SQLZ.bulkInsertCount != 0)
            {
                RunQuery(string.Format("{0}{1}",insertString,rowString.TrimEnd(',')));
                rowString = "";
                LoggerZ.Log(string.Format("Inserted {0} rows into {1}",rowCounter,tableName),Level.Debug,LogTarget.Console);
            }
            LoggerZ.Log(string.Format("{1} extracted, {0} rows inserted",rowCounter,tableName),Level.Debug,LogTarget.Console,LogTarget.File);
        }
        public void BulkInsert (string tableName,string valueString,DataTable data)
        {
            string columns = "";
            foreach(DataColumn column in data.Columns)
            {
                columns += "[" + column.ColumnName + "],";
            }
            string insertString = string.Format("INSERT INTO {0}({1}) Values ",tableName,columns.TrimEnd(','));
            RunQuery(string.Format("{0}{1};",insertString,valueString.Replace("\0","")));
        }
    }
}
