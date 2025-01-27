using System;
using System.Data.SQLite;
using System.Data;
using Haden.Library;

namespace Haden.Runtime
{
    //public enum Actions { Apriori = "a", Aposteriori = "aposteriori" }
    /// <summary>
    /// The controller designed to be used with <see cref="RueTheWhirl"/>.
    /// </summary>
    public class Controller
    {
        static readonly string FilePath = Environment.CurrentDirectory;
        static readonly string Data = @"\data\hadenAutonomyData.db";
        /// <summary>
        /// The controller's session data.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public int Session { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        public Controller() 
        {
            Session = Library.Session.SessionID;
        }

        /// <summary>
        /// Reads the data from the offline source.
        /// </summary>
        public static DataTable ReadData()
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + FilePath + Data);
                SQLiteCommand cmd = new SQLiteCommand();
                DataTable table = new DataTable();
                cmd = conn.CreateCommand();
                cmd.CommandText = "Select LastTurn, TurnsMade from Turns";
                SQLiteDataAdapter adapt = new SQLiteDataAdapter(cmd);
                adapt.Fill(table);

                return table;
            }
            catch (Exception ex)
            {
                Logging.WriteLog("Error in recalling historical data: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Control);
                return null;
            }
        }
        /// <summary>
        /// Stores the data into an offline source.
        /// </summary>
        /// <param name="lastTurn">The last turn.</param>
        /// <param name="turnsMade">The turns made.</param>
        public void StoreData(string lastTurn, int turnsMade, int iteration)
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(@"Data Source=" + FilePath + Data);
                SQLiteCommand cmd = new SQLiteCommand();
                cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Turns (SessionID, LastTurn, TurnsMade, Iteration) VALUES ('" + Library.Session.SessionID + "', '" + TransformData(lastTurn) + "', '" + turnsMade + "', '" + iteration + "')";
                conn.Open();
                SQLiteTransaction trans = conn.BeginTransaction();
                cmd.ExecuteNonQuery();
                trans.Commit();
                conn.Close();
                trans.Dispose();
                cmd.Dispose();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Control);
            }
            finally
            {
                Logging.WriteLog("Data posited successfully.", Logging.LogType.Information);
            }
        }
        /// <summary>
        /// Transforms the input data.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Zero for left, one for everything else</returns>
        public int TransformData(string entry)
        {
            if (entry == "left")
                return 0;
            else 
                return 1;
        }
    }
}
