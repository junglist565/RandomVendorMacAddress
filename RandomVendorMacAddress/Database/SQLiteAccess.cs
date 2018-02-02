using System;
using System.Data.SQLite;

public sealed class SQLiteAccess : IDisposable
{
    #region Global Vars
    private bool _connected = false;

    private String _connectionString = "";
    private String _dbFileName = "";

    private SQLiteConnection _conn = null;
    #endregion

    #region Properties
    public bool IsConnected
    {
        get { return _connected && _conn != null; }
    }

    public SQLiteConnection Connection
    {
        get { return _conn; }
    }
    #endregion

    #region Constructors
    public SQLiteAccess()
    {

    }

    public SQLiteAccess(String dbFileName)
    {
        _dbFileName = dbFileName;
        SetConnectionString();
    }
    #endregion

    public void OpenConnection()
    {
        if (_dbFileName == null || _dbFileName == String.Empty)
            throw new DatabaseAccessException("Invalid database filename");

        _conn = new SQLiteConnection();
        _conn.ConnectionString = _connectionString;
        _conn.Open();
        _connected = true;
    }

    public void CloseConnection()
    {
        if (_conn != null)
        {
            _conn.Close();
            _connected = false;
        }
    }

    private void SetConnectionString()
    {
        _connectionString = String.Format("Data Source={0};Version=3;New=False;Compress=True;", _dbFileName);
    }

    public void Dispose()
    {
        CloseConnection();
    }
}