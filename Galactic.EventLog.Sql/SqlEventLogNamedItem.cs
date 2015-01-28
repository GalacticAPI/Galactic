namespace Galactic.EventLog.Sql
{
    /// <summary>
    /// SqlEventLogNamedItem is a lightweight data class containing properties that describe items that contain an Id and Name in an SqlEventLog database.
    /// </summary>
    public class SqlEventLogNamedItem
    {
        /// <summary>
        /// The ID of the item in the database.
        /// </summary>
        public int Id;

        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name;
    }
}
