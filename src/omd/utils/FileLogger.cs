/*
 * 
 */
using System;

namespace OMD.omd.utils
{
	/// <summary>
	/// Description of FileLogger.
	/// </summary>
	public class FileLogger
	{
		private const string FILE_EXT = ".log";
	    private readonly string datetimeFormat;
	    private readonly string logFilename;
	
	    /// <summary>
	    /// Initiate an instance of SimpleLogger class constructor.
	    /// If log file does not exist, it will be created automatically.
	    /// </summary>
	    public FileLogger(string loggerName, LogLevel level)
	    {
	        datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
	        logFilename = loggerName + FILE_EXT;
	
	        // Log file header line
	        string logHeader = logFilename + " is created.";
	        if (!System.IO.File.Exists(logFilename))
	        {
	            WriteLine(System.DateTime.Now.ToString(datetimeFormat) + " " + logHeader, false);
	        }
	    }
	
	    /// <summary>
	    /// Log a DEBUG message
	    /// </summary>
	    /// <param name="text">Message</param>
	    public void Debug(string text)
	    {
	        WriteFormattedLog(LogLevel.DEBUG, text);
	    }
	
	    /// <summary>
	    /// Log an ERROR message
	    /// </summary>
	    /// <param name="text">Message</param>
	    public void Error(string text)
	    {
	        WriteFormattedLog(LogLevel.ERROR, text);
	    }
	    
	    /// <summary>
	    /// Log an ERROR message
	    /// </summary>
	    /// <param name="text">Message</param>
	    /// <param name="exception">Exception</param>
	    public void Error(string text, Exception exception)
	    {
	    	string textWithException = text + exceptionToString(exception);
	        WriteFormattedLog(LogLevel.ERROR, textWithException);
	    }
	
	    /// <summary>
	    /// Log a FATAL ERROR message
	    /// </summary>
	    /// <param name="text">Message</param>
	    public void Fatal(string text)
	    {
	        WriteFormattedLog(LogLevel.FATAL, text);
	    }
	
	    /// <summary>
	    /// Log an INFO message
	    /// </summary>
	    /// <param name="text">Message</param>
	    public void Info(string text)
	    {
	        WriteFormattedLog(LogLevel.INFO, text);
	    }
	
	    /// <summary>
	    /// Log a TRACE message
	    /// </summary>
	    /// <param name="text">Message</param>
	    public void Trace(string text)
	    {
	        WriteFormattedLog(LogLevel.TRACE, text);
	    }
	
	    /// <summary>
	    /// Log a WARNING message
	    /// </summary>
	    /// <param name="text">Message</param>
	    public void Warning(string text)
	    {
	        WriteFormattedLog(LogLevel.WARNING, text);
	    }
	
	    private void WriteLine(string text, bool append = true)
	    {
	        try
	        {
	            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilename, append, System.Text.Encoding.UTF8))
	            {
	                if (!string.IsNullOrEmpty(text))
	                {
	                    writer.WriteLine(text);
	                }
	            }
	        }
	        catch
	        {
	            throw;
	        }
	    }
	
	    private void WriteFormattedLog(LogLevel level, string text)
	    {
	        string pretext;
	        switch (level)
	        {
	            case LogLevel.TRACE:
	                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [TRACE]   ";
	                break;
	            case LogLevel.INFO:
	                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [INFO]    ";
	                break;
	            case LogLevel.DEBUG:
	                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [DEBUG]   ";
	                break;
	            case LogLevel.WARNING:
	                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [WARNING] ";
	                break;
	            case LogLevel.ERROR:
	                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [ERROR]   ";
	                break;
	            case LogLevel.FATAL:
	                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [FATAL]   ";
	                break;
	            default:
	                pretext = "";
	                break;
	        }
	
	        WriteLine(pretext + text);
	    }
	    
	    private int GetLevelInt(LogLevel level)
	    {
	        switch (level)
	        {
	            case LogLevel.TRACE:
	                return 0;
	            case LogLevel.DEBUG:
	                return 1;
	            case LogLevel.INFO:
	                return 2;
	            case LogLevel.WARNING:
	                return 2;
	            case LogLevel.ERROR:
	                return 2;
	            case LogLevel.FATAL:
	                return 2;
	            default:
	                return 1;
	        }
	    }
	    
	    private string exceptionToString(Exception e)
	    {
	    	string exception = "";
	    	exception += Environment.NewLine + "Message ---" + Environment.NewLine + e.Message;
            exception += Environment.NewLine + "Source ---" + Environment.NewLine + e.Source;
            exception += Environment.NewLine + "StackTrace ---" + Environment.NewLine + e.StackTrace;
            
            return exception;
	    }
	
	    [System.Flags]
	    public enum LogLevel
	    {
	        TRACE,
	        INFO,
	        DEBUG,
	        WARNING,
	        ERROR,
	        FATAL
	    }
	}
}
