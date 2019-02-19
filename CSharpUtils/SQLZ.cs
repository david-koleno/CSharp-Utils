using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalDataGeneration
{
    #region SQLZ Class Library
    public static class SQLZ
    {
        #region SQLZ Constants
        public static int bulkInsertCount = 1000;

        public static List<SqlDbType> lengthTypes = new List<SqlDbType>(){ SqlDbType.VarChar,SqlDbType.VarBinary,SqlDbType.NVarChar,SqlDbType.NChar,SqlDbType.Char,SqlDbType.Binary};
        #endregion

        #region SQLZ Public Methods
        public static string GenCreateTable (string tableName, params SQLColumn[] columns)
        {
            string ColumnString = "";
            foreach(SQLColumn column in columns)
            {
                ColumnString += column.GetCreateSqlColumnInfo() + ",";
            }
            string CreateTableString = String.Format("CREATE TABLE {0} ({1});",tableName,ColumnString.TrimEnd(','));



            return CreateTableString;
        }
        public static string GenCreateTable (string tableName,List<SQLColumn> columns)
        {

            string ColumnString = "";
            foreach(SQLColumn column in columns)
            {
                ColumnString += column.GetCreateSqlColumnInfo() + ",";
            }
            string CreateTableString = String.Format("CREATE TABLE {0} ({1});",tableName,ColumnString.TrimEnd(','));

            

            return CreateTableString;
        }
        #endregion

        #region SQLZ Private Methods

        #endregion
    }
    #endregion

    #region SQL Column Object
    public class SQLColumn
    {
        #region SQL Column Instance Variables
        public string Name { get; set; }
        public SqlDbType Type { get; set; }
        public int Length { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Null { get; set; }
        public Tuple<int,int> AutoIncrement { get; set; }
        public Tuple<int,int> Precision { get; set; }
        #endregion

        #region SQL Column Initializers
        public SQLColumn (string name,SqlDbType type)
        {
            Name = name;
            Type = type;
            Length = int.MaxValue;
            PrimaryKey = false;
            Null = false;
            AutoIncrement = new Tuple<int,int>(0,0);
            Precision = new Tuple<int,int>(18,2);
        }
        public SQLColumn (string name,SqlDbType type,int length)
        {
            Name = name;
            Type = type;
            Length = length;
            PrimaryKey = false;
            Null = false;
            AutoIncrement = new Tuple<int,int>(0,0);
            Precision = new Tuple<int,int>(18,2);
        }
        public SQLColumn (string name,SqlDbType type,bool nullVal)
        {
            Name = name;
            Type = type;
            Length = int.MaxValue;
            PrimaryKey = false;
            Null = nullVal;
            AutoIncrement = new Tuple<int,int>(0,0);
            Precision = new Tuple<int,int>(18,2);
        }
        public SQLColumn (string name,SqlDbType type,int length,bool nullVal)
        {
            Name = name;
            Type = type;
            Length = length;
            PrimaryKey = false;
            Null = nullVal;
            AutoIncrement = new Tuple<int,int>(0,0);
            Precision = new Tuple<int,int>(18,2);
        }
        public SQLColumn (string name,SqlDbType type,bool nullVal,bool primary)
        {
            Name = name;
            Type = type;
            Length = int.MaxValue;
            PrimaryKey = primary;
            Null = nullVal;
            AutoIncrement = new Tuple<int,int>(0,0);
            Precision = new Tuple<int,int>(18,2);
        }
        public SQLColumn (string name,SqlDbType type,int length,bool nullVal,bool primary)
        {
            Name = name;
            Type = type;
            Length = length;
            PrimaryKey = primary;
            Null = nullVal;
            AutoIncrement = new Tuple<int,int>(0,0);
            Precision = new Tuple<int,int>(18,2);
        }
        public SQLColumn (string name,SqlDbType type,Tuple<int,int> precision)
        {
            Name = name;
            Type = type;
            Length = int.MaxValue;
            PrimaryKey = false;
            Null = false;
            AutoIncrement = new Tuple<int,int>(0,0);
            Precision = precision;
        }
        public SQLColumn (string name,SqlDbType type,int length,Tuple<int,int> autoInc)
        {
            Name = name;
            Type = type;
            Length = length;
            PrimaryKey = false;
            Null = false;
            AutoIncrement = autoInc;
            Precision = new Tuple<int,int>(18,2);
        }
        public SQLColumn (string name,SqlDbType type,bool nullVal,Tuple<int,int> autoInc)
        {
            Name = name;
            Type = type;
            Length = int.MaxValue;
            PrimaryKey = false;
            Null = false;
            AutoIncrement = autoInc;
            Precision = new Tuple<int,int>(18,2);
        }
        public SQLColumn (string name,SqlDbType type,int length,bool nullVal,Tuple<int,int> precision)
        {
            Name = name;
            Type = type;
            Length = length;
            PrimaryKey = false;
            Null = nullVal;
            AutoIncrement = new Tuple<int,int>(0,0);
            Precision = precision;

        }
        public SQLColumn (string name,SqlDbType type,bool nullVal,bool primary,Tuple<int,int> autoInc)
        {
            Name = name;
            Type = type;
            Length = int.MaxValue;
            PrimaryKey = primary;
            Null = nullVal;
            AutoIncrement = autoInc;
            Precision = new Tuple<int,int>(18,2);
        }
        public SQLColumn (string name,SqlDbType type,int length,bool nullVal,bool primary,Tuple<int,int> autoInc)
        {
            Name = name;
            Type = type;
            Length = length;
            PrimaryKey = primary;
            Null = nullVal;
            AutoIncrement = autoInc;
            Precision = new Tuple<int,int>(18,2);
        }
        #endregion

        #region SQL Column Public Methods
        public string GetCreateSqlColumnInfo ()
        {
            string genSQLColumn = Name + " ";
            if(SQLZ.lengthTypes.Contains(Type))
            {
                genSQLColumn += String.Format("{0}({1}) ",Type.ToString(),( Length > 8000 ? "MAX" : Length.ToString()));
            }else if(Type.Equals(SqlDbType.Decimal))
            {

                genSQLColumn += String.Format("{0}({1},{2}) ",Type.ToString(),Precision.Item1,Precision.Item2);
            }
            else
            {
                genSQLColumn += Type.ToString() + " ";
            }
            if(AutoIncrement.Item2 != 0)
            {
                genSQLColumn += String.Format("IDENTITY({0},{1}) ",AutoIncrement.Item1,AutoIncrement.Item2);
            }
            if(Null)
            {
                genSQLColumn += "NULL ";
            }
            else
            {
                genSQLColumn += "NOT NULL ";
            }
            if(PrimaryKey)
            {
                genSQLColumn += "PRIMARY KEY";
            }
            return genSQLColumn.TrimEnd(' ');
        }
        #endregion

        #region SQL Column Private Methods

        #endregion
    }
    #endregion
}
