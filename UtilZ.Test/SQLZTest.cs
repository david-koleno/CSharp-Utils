using LocalDataGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UtilZ.Test
{
    public class SQLZTest
    {

        #region SQL Column Tests
        [Fact]
        public void TestSQLColumnOutputBasic ()
        {
            SQLColumn testColumn = new SQLColumn("testColumnName",System.Data.SqlDbType.Int);
            Assert.Equal("testColumnName Int NOT NULL",testColumn.GetCreateSqlColumnInfo());
        }

        [Fact]
        public void TestSQLColumnOutputVarCharMax ()
        {
            SQLColumn testColumn = new SQLColumn("testColumnName",System.Data.SqlDbType.VarChar);
            Assert.Equal("testColumnName VarChar(MAX) NOT NULL",testColumn.GetCreateSqlColumnInfo());
        }

        [Fact]
        public void TestSQLColumnOutputVarCharSetLength ()
        {
            SQLColumn testColumn = new SQLColumn("testColumnName",System.Data.SqlDbType.VarChar,255);
            Assert.Equal("testColumnName VarChar(255) NOT NULL",testColumn.GetCreateSqlColumnInfo());
        }

        [Fact]
        public void TestSQLColumnOutputDecimalDefault ()
        {
            SQLColumn testColumn = new SQLColumn("testColumnName",System.Data.SqlDbType.Decimal);
            Assert.Equal("testColumnName Decimal(18,2) NOT NULL",testColumn.GetCreateSqlColumnInfo());
        }

        [Fact]
        public void TestSQLColumnOutputDecimalSetPrecision ()
        {
            SQLColumn testColumn = new SQLColumn("testColumnName",System.Data.SqlDbType.Decimal,new Tuple<int,int>(4,2));
            Assert.Equal("testColumnName Decimal(4,2) NOT NULL",testColumn.GetCreateSqlColumnInfo());
        }

        [Fact]
        public void TestSQLColumnOutputNull ()
        {
            SQLColumn testColumn = new SQLColumn("testColumnName",System.Data.SqlDbType.Int,true);
            Assert.Equal("testColumnName Int NULL",testColumn.GetCreateSqlColumnInfo());
        }
        
        [Fact]
        public void TestSQLColumnOutputPrimaryKey ()
        {
            SQLColumn testColumn = new SQLColumn("testColumnName",System.Data.SqlDbType.Int,false,true);
            Assert.Equal("testColumnName Int NOT NULL PRIMARY KEY",testColumn.GetCreateSqlColumnInfo());
        }

        [Fact]
        public void TestSQLColumnOutputAutoIncrement ()
        {
            SQLColumn testColumn = new SQLColumn("testColumnName",System.Data.SqlDbType.Int,false,new Tuple<int,int>(1,1));
            Assert.Equal("testColumnName Int IDENTITY(1,1) NOT NULL",testColumn.GetCreateSqlColumnInfo());
        }

        [Fact]
        public void TestSQLColumnOutputAll ()
        {
            SQLColumn testColumn = new SQLColumn("testColumnName",System.Data.SqlDbType.Binary,255,false,true,new Tuple<int,int>(1,1));
            Assert.Equal("testColumnName Binary(255) IDENTITY(1,1) NOT NULL PRIMARY KEY",testColumn.GetCreateSqlColumnInfo());
        }
        #endregion

        #region SQL Create Statement Test
        [Fact]
        public void TestSQLCreateTableList ()
        {
            List<SQLColumn> columns = new List<SQLColumn>
            {
                new SQLColumn("ID",System.Data.SqlDbType.Int,false,true,new Tuple<int, int>(1,1)),
                new SQLColumn("date",System.Data.SqlDbType.Date,false),
                new SQLColumn("PayerCategory",System.Data.SqlDbType.VarChar,50,false),
                new SQLColumn("Amount",System.Data.SqlDbType.Decimal,new Tuple<int, int>(8,2))
            };
            Assert.Equal("CREATE TABLE TestTable (ID Int IDENTITY(1,1) NOT NULL PRIMARY KEY,date Date NOT NULL,PayerCategory VarChar(50) NOT NULL,Amount Decimal(8,2) NOT NULL);",SQLZ.GenCreateTable("TestTable",columns));
        }

        [Fact]
        public void TestSQLCreateTableParams ()
        {
            Assert.Equal("CREATE TABLE TestTable (ID Int IDENTITY(1,1) NOT NULL PRIMARY KEY,date Date NOT NULL,PayerCategory VarChar(50) NOT NULL,Amount Decimal(8,2) NOT NULL);",SQLZ.GenCreateTable("TestTable",new SQLColumn("ID",System.Data.SqlDbType.Int,false,true,new Tuple<int,int>(1,1)),
                new SQLColumn("date",System.Data.SqlDbType.Date,false),
                new SQLColumn("PayerCategory",System.Data.SqlDbType.VarChar,50,false),
                new SQLColumn("Amount",System.Data.SqlDbType.Decimal,new Tuple<int,int>(8,2))));
        }
        #endregion
        
    }

}
