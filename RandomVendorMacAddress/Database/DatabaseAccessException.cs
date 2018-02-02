using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[Serializable]
public class DatabaseAccessException : Exception
{
    public DatabaseAccessException() : base()
    {

    }

    public DatabaseAccessException(string message) : base(message)
    {

    } 

    public DatabaseAccessException(string message, Exception inner) : base(message, inner)
    {

    }
}