using System;
using System.Data;
using System.Data.SQLite;

public sealed class QueryHelper
{
    private SQLiteConnection _conn = null;

    private struct QueryParam
    {
        public String _key;
        public object _value;

        public QueryParam(String aKey, object aValue)
        {
            _key = aKey;
            _value = aValue;
        }
    }

    public QueryHelper(SQLiteConnection conn)
    {
        if (conn == null)
            throw new DatabaseAccessException(
                @"Connection can't be 'null': pass a valid connection");

        _conn = conn;
    }

    public void ExecuteNonQuery(String querySql)
    {
        SQLiteCommand command = new SQLiteCommand(@querySql, _conn);
        command.ExecuteNonQuery();
    }

    public DataTable DoSimpleSelect(String querySql, String tableName)
    {
        SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        SQLiteCommand command = new SQLiteCommand(@querySql, _conn);
        command.CommandType = CommandType.Text;
        adapter.SelectCommand = command;

        DataSet dataSet = new DataSet();
        adapter.Fill(dataSet, tableName);

        return dataSet.Tables[tableName];
    }

    public DataTable DoSimpleSelectWithParams(String querySql, String tableName, String[] keys, params object[] values)
    {
        if (keys.Length != values.Length)
            throw new ArgumentException("Params \"keys\" and \"values\" must have the same length");

        SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        SQLiteCommand command = new SQLiteCommand(@querySql, _conn);

        for (int i = 0; i < keys.Length; i++)
        {
            if (values[i] is int)  // Valore di tipo Integer
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.Int32, int.MaxValue);
                param.Value = (int)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is float)  // Valore di tipo Float
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.Decimal, int.MaxValue);
                param.Value = (float)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is long)  // Valore di tipo Long
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.Int64, int.MaxValue);
                param.Value = (long)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is bool)  // Valore di tipo Boolean
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.Boolean, 255);
                param.Value = (bool)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is String)  // Valore di tipo String (o Varchar per MySQL)
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.String, int.MaxValue);
                param.Value = (String)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is DateTime)  // Valore di tipo DateTime
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.DateTime, int.MaxValue);
                param.Value = (DateTime)sqlParams._value;
                command.Parameters.Add(param);
            }
        }

        command.Prepare();
        adapter.SelectCommand = command;

        DataSet dataSet = new DataSet();
        adapter.Fill(dataSet, tableName);

        return dataSet.Tables[tableName];
    }

    public void InsertData(String querySql, String[] keys, params object[] values)
    {
        if (keys.Length != values.Length)
            throw new ArgumentException("Params \"keys\" and \"values\" must have the same length");

        SQLiteDataAdapter adapter = new SQLiteDataAdapter();
        SQLiteCommand command = new SQLiteCommand(@querySql, _conn);

        for (int i = 0; i < keys.Length; i++)
        {
            if (values[i] is int)  // Valore di tipo Integer
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.Int32, int.MaxValue);
                param.Value = (int)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is float)  // Valore di tipo Float
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.Decimal, int.MaxValue);
                param.Value = (float)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is long)  // Valore di tipo Long
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.Int64, int.MaxValue);
                param.Value = (long)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is bool)  // Valore di tipo Boolean
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.Boolean, 255);
                param.Value = (bool)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is String)  // Valore di tipo String (o Varchar per MySQL)
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.String, int.MaxValue);
                param.Value = (String)sqlParams._value;
                command.Parameters.Add(param);
            }
            else if (values[i] is DateTime)  // Valore di tipo Datetime
            {
                QueryParam sqlParams = new QueryParam(keys[i], values[i]);
                SQLiteParameter param = new SQLiteParameter(sqlParams._key, DbType.DateTime, int.MaxValue);
                param.Value = (DateTime)sqlParams._value;
                command.Parameters.Add(param);
            }
        }

        command.Prepare();
        command.ExecuteNonQuery();
    }
}