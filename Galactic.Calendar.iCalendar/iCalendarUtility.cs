using Galactic.Configuration;
using DDay.iCal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Galactic.Calendar.iCalendar
{
    /// <summary>
    /// Provides various utility methods for use with iCalendars.
    /// Uses the DDay.iCal library. http://www.ddaysoftware.com/Pages/Projects/DDay.iCal/
    /// </summary>
    public class iCalendarUtility : CalendarUtility
    {
        // ---------- CONSTANTS ----------

        // Priority thresholds for events.
        // Events with priorities less than or equal to these numbers fall under these levels.
        private const int PRIORITY_THRESHOLD_UNDEFINED = 0;
        private const int PRIORITY_THRESHOLD_HIGH = 4;
        private const int PRIORITY_THRESHOLD_MEDIUM = 6;
        private const int PRIORITY_THRESHOLD_LOW = 9;

        // ---------- VARIABLES ----------

        // A Dictionary that maps a calendar's name to its URI.
        private readonly Dictionary<string, Uri> calendars = new Dictionary<string, Uri>();

        // A Dictionary that maps a calendar's name to its credentials (if required).
        // Credentials: Key = username, Value = password
        private readonly Dictionary<string, KeyValuePair<string, string>> credentials = new Dictionary<string, KeyValuePair<string, string>>();

        // ---------- PROPERTIES ----------

        /// <summary>
        /// A list of the names of calendars available via the calendar provider.
        /// </summary>
        public override List<string> CalendarNames
        {
            get
            {
                return calendars.Keys.ToList();
            }
        }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Default constructor.
        /// </summary>
        public iCalendarUtility()
        {
        }

        /// <summary>
        /// Creates the utility class that allows for loading iCalendar files.
        /// </summary>
        /// <param name="configurationItemDirectoryPath">The physical path to the directory where configuration item files can be found.</param>
        /// <param name="configurationItemName">The name of the configuration item containing the iCalendar configuration.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if one of the parameters provided is empty, null, or whitespace.</exception>
        /// <exception cref="System.ArgumentException">Thrown if there was an error reading the configuration data.</exception>
        /// <remarks>Configuration data has the following format: Name|URI|username|password
        /// Username and password can be empty if they are not required.
        /// The configuration file should list one calendar per line.</remarks>
        public iCalendarUtility(string configurationItemDirectoryPath, string configurationItemName)
        {
            if (!string.IsNullOrWhiteSpace(configurationItemDirectoryPath) && !string.IsNullOrWhiteSpace(configurationItemName))
            {
                // Get the configuration item with the configuration data from a file.
                ConfigurationItem configItem = new ConfigurationItem(configurationItemDirectoryPath, configurationItemName, true);

                try
                {
                    // Get the configuration data from the configuration item.
                    StringReader reader = new StringReader(configItem.Value);

                    // NOTE: See remarks in method documentation above for configuration file format.

                    // Load all of the calendars specified in the configuration file.
                    string configLine = reader.ReadLine();
                    while (!string.IsNullOrWhiteSpace(configLine))
                    {
                        // Split the line into its entries.
                        string[] configLineEntries = configLine.Split('|');

                        // Get the calendar's name.
                        string name = configLineEntries[0];

                        // Get the calendar's URI.
                        Uri uri = new Uri(configLineEntries[1]);

                        // Get the username and password to use with the calendar.
                        string username = configLineEntries[2];
                        string password = configLineEntries[3];

                        // Add the calendar to the list of calendars.
                        calendars.Add(name, uri);

                        // If credentials were supplied, add them to the credentials dictionary.
                        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                        {
                            credentials.Add(name, new KeyValuePair<string, string>(username, password));
                        }

                        // Read the next configuration line.
                        configLine = reader.ReadLine();
                    }
                }
                catch
                {
                    throw new ArgumentException("Unable to read calendar configuration data from configuration item.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(configurationItemDirectoryPath))
                {
                    throw new ArgumentNullException("configurationItemDirectoryPath");
                }
                if (string.IsNullOrWhiteSpace(configurationItemName))
                {
                    throw new ArgumentNullException("configurationItemName");
                }
            }
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Gets a calendar from the provider.
        /// </summary>
        /// <param name="name">The name of the calendar to retrieve.</param>
        /// <param name="startDate">Events starting on or after this date will be included in the list returned.</param>
        /// <param name="endDate">Events starting on or before this date will be included in the list returned.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a name that is empty, null,
        /// or full of whitespace is provided.</exception>
        /// <returns>The calendar with the supplied name, or null if a calendar of that name does not exist
        /// or could not be retrieved.</returns>
        /// <remarks>This method only returns the first calendar in iCalendar files that contain multiple calendars.</remarks>
        override public Calendar GetCalendar(string name, DateTime startDate, DateTime endDate)
        {
            // Check that a name was provided.
            if (!string.IsNullOrWhiteSpace(name))
            {
                // A name was provided.
                // Check whether the name exists in the list of available calendars.
                if (calendars.ContainsKey(name))
                {
                    // The calendar is available.
                    // Get the calendar from the file.
                    IICalendarCollection calendarCollection = DDay.iCal.iCalendar.LoadFromUri(calendars[name]);
                    if (calendarCollection.Count > 0)
                    {
                        // The iCalendar file contains a calendar, get the first one.
                        IICalendar iCalCalendar = calendarCollection[0];

                        // Create the calendar object from the file.
                        Calendar calendar = new Calendar { Name = name, Description = "" };

                        // Get the events associated with the calendar.
                        foreach (IEvent iCalEvent in iCalCalendar.Events)
                        {
                            // Create an event for the calendar.
                            Event calendarEvent = new Event
                            {
                                // Populate the event with data from the iCalendar event.
                                Title = iCalEvent.Summary,
                                Description = iCalEvent.Description,
                                Location = iCalEvent.Location,
                                LocationUrl = null,
                                Canceled = iCalEvent.Status.HasFlag(EventStatus.Cancelled),
                                NoEndTime = false
                            };

                            // Set the event's priorty based upon the thresholds defined in the iCalendar standard.
                            if (iCalEvent.Priority == PRIORITY_THRESHOLD_UNDEFINED)
                            {
                                // A priority is not defined for this event.
                                // Default it to Medium.
                                calendarEvent.Priority = Event.PriorityLevels.Medium;
                            }
                            else if (iCalEvent.Priority <= PRIORITY_THRESHOLD_HIGH)
                            {
                                calendarEvent.Priority = Event.PriorityLevels.High;
                            }
                            else if (iCalEvent.Priority <= PRIORITY_THRESHOLD_MEDIUM)
                            {
                                calendarEvent.Priority = Event.PriorityLevels.Medium;
                            }
                            else
                            {
                                calendarEvent.Priority = Event.PriorityLevels.Low;
                            }

                            calendarEvent.StartDate = iCalEvent.Start.Value;
                            calendarEvent.EndDate = iCalEvent.End.Value;
                            calendarEvent.AllDayEvent = iCalEvent.IsAllDay;
                            if (iCalEvent.Contacts != null && iCalEvent.Contacts.Count > 0)
                            {
                                // Only grab the first contact. (Some other calendars don't support multiple contacts.)
                                calendarEvent.ContactName = iCalEvent.Contacts[0];
                            }
                            calendarEvent.ContactPhone = null;
                            calendarEvent.ContactEmail = null;
                            calendarEvent.OnMultipleCalendars = false;
                            if (iCalEvent.RecurrenceDates != null && iCalEvent.RecurrenceDates.Count > 0)
                            {
                                calendarEvent.Reoccurring = true;
                            }
                            else
                            {
                                calendarEvent.Reoccurring = false;
                            }
                            if (iCalEvent.LastModified != null)
                            {
                                calendarEvent.LastUpdated = iCalEvent.LastModified.Value;
                            }
                            calendarEvent.LastUpdatedBy = null;
                            if (iCalEvent.LastModified != null)
                            {
                                calendarEvent.DetailsLastUpdated = iCalEvent.LastModified.Value;
                            }
                            calendarEvent.DetailsLastUpdatedBy = null;

                            // Add the events to the list of of events in the calendar.
                            calendar.Events.Add(calendarEvent);
                        }

                        // Return the initialized calendar.
                        return calendar;
                    }
                    else
                    {
                        // The iCalendar file does not contain any calendars.
                        return null;
                    }
                }
                else
                {
                    // The calendar doesn't exist.
                    return null;
                }
            }
            else
            {
                // A name was not provided.
                throw new ArgumentNullException("name");
            }
        }

        /// <summary>
        /// Returns the text of the iCalendar feed associated with the calendar object supplied.
        /// </summary>
        /// <param name="calendar">The object to get the iCalendar feed text of.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a calendar is not provided.</exception>
        /// <returns>The text of the iCalendar feed associated with the calendar supplied.</returns>
        public string GetCalendarText(Calendar calendar)
        {
            // Check if a calendar was provided.
            if (calendar != null)
            {
                // A calendar was provided.

                // Create a serializer to save the iCalendar calendar feed.
                DDay.iCal.Serialization.iCalendar.iCalendarSerializer serializer = new DDay.iCal.Serialization.iCalendar.iCalendarSerializer();

                // Serialize the iCalendar calendar feed to a string.
                return serializer.SerializeToString(ToIICalendar(calendar));
            }
            else
            {
                // A calendar was not provided.
                throw new ArgumentNullException("calendar");
            }
        }

        /// <summary>
        /// Saves the supplied calendar to its file location specified in the configuration file.
        /// </summary>
        /// <param name="calendar">The calendar object to save.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a calendar is not provided.</exception>
        /// <returns>Always returns true. (The iCalendar serializer does not report whether it was successful.)</returns>
        override public bool SaveCalendar(Calendar calendar)
        {
            // Check if a calendar was provided.
            if (calendar != null)
            {
                // A calendar was provided.

                // Save the calendar to its location as specified in the configuration file.
                SaveCalendar(calendar, calendars[calendar.Name].AbsolutePath);

                // The iCalendar serializer does not report whether it was successful while serializing.
                // Always return true;
                return true;
            }
            else
            {
                // A calendar was not provided.
                throw new ArgumentNullException("calendar");
            }
        }

        /// <summary>
        /// Saves the supplied calendar object as an iCalendar file at the supplied path.
        /// </summary>
        /// <param name="calendar">The calendar object to save.</param>
        /// <param name="path">The path to save the iCalendar file at.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a calendar is not provided, or a path was not provided.</exception>
        /// <returns>Always returns true. (The iCalendar serializer does not report whether it was successful.)</returns>
        public void SaveCalendar(Calendar calendar, string path)
        {
            // Check if a calendar and path was provided.
            if (calendar != null && !string.IsNullOrWhiteSpace(path))
            {
                // A calendar and path was provided.

                // Create a serializer to save the iCalendar calendar file.
                DDay.iCal.Serialization.iCalendar.iCalendarSerializer serializer = new DDay.iCal.Serialization.iCalendar.iCalendarSerializer();

                // Serialize the iCalendar calendar file to the path supplied.
                serializer.Serialize(ToIICalendar(calendar), path);
            }
            else
            {
                // A calendar or path not provided.
                if (calendar == null)
                {
                    throw new ArgumentNullException("calendar");
                }
                else
                {
                    throw new ArgumentNullException("path");
                }
            }
        }

        /// <summary>
        /// Converts a Calendar to an IICalendar.
        /// </summary>
        /// <param name="calendar">The calendar object to convert.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a calendar is not provided.</exception>
        /// <returns>The converted IICalendar, or null if the calendar couldn't be converted.</returns>
        private static IICalendar ToIICalendar(Calendar calendar)
        {
            // Check if a calendar was provided.
            if (calendar != null)
            {
                // A calendar was provided.

                // Create the iCalendar calendar to populate with events.
                IICalendar iCalCalendar = new DDay.iCal.iCalendar();

                // Populate the calendar with events.
                foreach (Event calendarEvent in calendar.Events)
                {
                    DDay.iCal.Event iCalEvent = new DDay.iCal.Event
                    {
                        Summary = calendarEvent.Title,
                        Description = calendarEvent.Description,
                        Location = calendarEvent.Location
                    };
                    if (calendarEvent.Canceled.HasValue && calendarEvent.Canceled == true)
                    {
                        iCalEvent.Status = EventStatus.Cancelled;
                    }
                    // Set the priority to the lowest value in its defined range.
                    switch (calendarEvent.Priority)
                    {
                        case Event.PriorityLevels.High:
                            iCalEvent.Priority = PRIORITY_THRESHOLD_UNDEFINED + 1;
                            break;
                        case Event.PriorityLevels.Medium:
                            iCalEvent.Priority = PRIORITY_THRESHOLD_HIGH + 1;
                            break;
                        case Event.PriorityLevels.Low:
                            iCalEvent.Priority = PRIORITY_THRESHOLD_MEDIUM + 1;
                            break;
                    }
                    if (calendarEvent.StartDate.HasValue)
                    {
                        iCalEvent.Start = new iCalDateTime(calendarEvent.StartDate.Value);
                    }
                    if (calendarEvent.EndDate.HasValue)
                    {
                        iCalEvent.End = new iCalDateTime(calendarEvent.EndDate.Value);
                    }
                    if (calendarEvent.AllDayEvent.HasValue)
                    {
                        iCalEvent.IsAllDay = calendarEvent.AllDayEvent.Value;
                    }
                    else
                    {
                        iCalEvent.IsAllDay = false;
                    }
                    if (!string.IsNullOrWhiteSpace(calendarEvent.ContactName))
                    {
                        iCalEvent.Contacts = new List<string> { calendarEvent.ContactName };
                    }
                    if (calendarEvent.LastUpdated.HasValue)
                    {
                        iCalEvent.LastModified = new iCalDateTime(calendarEvent.LastUpdated.Value);
                    }

                    // Add the event to the iCalendar calendar.
                    iCalCalendar.Events.Add(iCalEvent);
                }

                // Return the populated iCalendar calendar.
                return iCalCalendar;
            }
            else
            {
                // A calendar was not provided.
                throw new ArgumentNullException("calendar");
            }
        }
    }
}
