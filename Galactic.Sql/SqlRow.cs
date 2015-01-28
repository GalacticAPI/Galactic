using System;
using System.Collections.Generic;

namespace Galactic.Sql
{
    /// <summary>
    /// SqlRow is a data class that contains a row from a SQL Database table.
    /// It is used in conjunction with StoredProcedure to return results from a procedure.
    /// </summary>
    public class SqlRow
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // An array of strings containing the name of the field at the 
        private readonly List<string> fieldOrder = new List<string>();

        // A dictionary of field values accessible by name.
        private readonly Dictionary<string, Object> fields = new Dictionary<string, object>();

        // ---------- PROPERTIES ----------

        /// <summary>
        /// Get or set the row's column values via their name.
        /// </summary>
        /// <param name="fieldName">The name of the field value to get or set.</param>
        /// <returns>The value of the field.</returns>
        public Object this[string fieldName]
        {
            get
            {
                if (!string.IsNullOrEmpty(fieldName))
                {
                    return fields[fieldName];
                }
                return null;
            }
            set
            {
                if (!string.IsNullOrEmpty(fieldName))
                {
                    if (fields.ContainsKey(fieldName))
                    {
                        fields[fieldName] = value;
                    }
                    else
                    {
                        Add(fieldOrder.Count + 1, fieldName, value);
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the row's column values via the column's index.
        /// </summary>
        /// <param name="columnIndex">The index value of the column</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The column index number specified was outside the range contained by this row.</exception>
        public Object this[int columnIndex]
        {
            get
            {
                if (columnIndex >= 0 && columnIndex < fieldOrder.Count)
                {
                    return fields[fieldOrder[columnIndex]];
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (columnIndex >= 0 && columnIndex < fieldOrder.Count)
                {
                    fields[fieldOrder[columnIndex]] = value;
                }
                throw new IndexOutOfRangeException();
            }
        }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SqlRow()
        {
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Adds a new field to the row.
        /// </summary>
        /// <param name="order">The order of the field.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <returns>True if the field was added, false otherwise.</returns>
        public bool Add(int order, string name, Object value)
        {
            // Check that the order is valid and a name was supplied.
            if (order >= 0 && !string.IsNullOrEmpty(name))
            {
                // The order is valid and a name was supplied.

                // Check whether the field already exists in the order list.
                if (fieldOrder.Count > order)
                {
                    // The field exists, update it.
                    fieldOrder[order] = name;
                }
                else
                {
                    // The field does not exist. Add it.
                    fieldOrder.Add(name);
                }
                fields.Add(name, value);
                return true;
            }
            else
            {
                // The order supplied was invalid or the name was not supplied.
                return false;
            }
        }

        /// <summary>
        /// Get the value of a field in this row by name.
        /// </summary>
        /// <param name="fieldName">The name of the field to get the value of.</param>
        /// <returns>The value of the field.</returns>
        public Object GetValue(string fieldName)
        {
            return this[fieldName];
        }

        /// <summary>
        /// Get the value of a field in this row by column index number.
        /// </summary>
        /// <param name="columnIndex">The column index number of the field to get the value of.</param>
        /// <returns>The value of the field.</returns>
        public Object GetValue(int columnIndex)
        {
            return this[columnIndex];
        }

        /// <summary>
        /// Sets the value of a field in this row by name.
        /// </summary>
        /// <param name="fieldName">The name of the field to set the value of.</param>
        /// <param name="value">The value to set the field to.</param>
        public void SetValue(string fieldName, Object value)
        {
            this[fieldName] = value;
        }

        /// <summary>
        /// Sets the value of a field in this row by column index number.
        /// </summary>
        /// <param name="columnIndex">The column index number of the field to set the value of.</param>
        /// <param name="value">The value to set the field to.</param>
        public void SetValue(int columnIndex, Object value)
        {
            this[columnIndex] = value;
        }
    }
}
