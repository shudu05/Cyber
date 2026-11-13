
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Cyber
{
    public class DatabaseManager
    {
        private string connectionString =
            "Server=localhost;Port=3306;Database=CybersecurityBotDB;Uid=root;Pwd=Ntombifuthi@2005;";

        // Test database connection
        public string TestConnection()
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();
                    return "Database connected successfully!";
                }
            }
            catch (Exception ex)
            {
                return "Connection failed: " + ex.Message;
            }
        }

        // Save an activity using an existing connection
        private void SaveActivity(
            MySqlConnection connection, string description)
        {
            try
            {
                string query =
                    "INSERT INTO ActivityLog (ActivityDescription) " +
                    "VALUES (@description)";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@description", description);

                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                // Activity logging should not stop the main task operation.
            }
        }

        // Public method for logging activities from other classes
        public void LogActivity(string description)
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();
                    SaveActivity(connection, description);
                }
            }
            catch
            {
                // Prevent logging errors from interrupting the application.
            }
        }

        // Retrieve activity logs
        public string GetActivityLogs(int limit = 10)
        {
            try
            {
                if (limit <= 0)
                {
                    limit = 10;
                }

                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query =
                        "SELECT ActivityDescription, ActivityDate " +
                        "FROM ActivityLog " +
                        "ORDER BY ActivityId DESC LIMIT @limit";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@limit", limit);

                        using (MySqlDataReader reader =
                            command.ExecuteReader())
                        {
                            StringBuilder activities = new StringBuilder();

                            int count = 0;

                            while (reader.Read())
                            {
                                count++;

                                activities.AppendLine(
                                    count + ". " +
                                    reader["ActivityDescription"]);

                                DateTime activityDate =
                                    Convert.ToDateTime(reader["ActivityDate"]);

                                activities.AppendLine(
                                    "   Date: " +
                                    activityDate.ToString("dd/MM/yyyy HH:mm"));

                                activities.AppendLine();
                            }

                            if (count == 0)
                            {
                                return "No activities have been recorded yet.";
                            }

                            return activities.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error retrieving activity logs: " + ex.Message;
            }
        }

        // Add a new task
        public string AddTask(string title, string description)
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query =
                        "INSERT INTO Tasks (Title, Description) " +
                        "VALUES (@title, @description)";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@description", description);

                        command.ExecuteNonQuery();
                    }

                    SaveActivity(connection, "Added task: " + title);
                }

                return "Task added successfully!";
            }
            catch (Exception ex)
            {
                return "Error adding task: " + ex.Message;
            }
        }

        // Mark a task as completed
        public string CompleteTask(int taskId)
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query =
                        "UPDATE Tasks SET IsCompleted = TRUE WHERE TaskId = @taskId";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@taskId", taskId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            SaveActivity(
                                connection, "Completed task ID: " + taskId);

                            return "Task marked as completed!";
                        }
                        else
                        {
                            return "Task ID not found. Please check the task ID.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error completing task: " + ex.Message;
            }
        }

        // Delete a task
        public string DeleteTask(int taskId)
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query =
                        "DELETE FROM Tasks WHERE TaskId = @taskId";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@taskId", taskId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            SaveActivity(
                                connection, "Deleted task ID: " + taskId);

                            return "Task deleted successfully!";
                        }
                        else
                        {
                            return "Task ID not found. Please check the task ID.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error deleting task: " + ex.Message;
            }
        }

        // Display saved tasks, including reminders
        public string GetTasks()
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query =
                        "SELECT TaskId, Title, Description, ReminderDate, IsCompleted " +
                        "FROM Tasks";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            string tasks = "";

                            while (reader.Read())
                            {
                                tasks += "Task ID: " + reader["TaskId"] + "\n";
                                tasks += "Title: " + reader["Title"] + "\n";
                                tasks += "Description: " + reader["Description"] + "\n";

                                if (reader["ReminderDate"] != DBNull.Value)
                                {
                                    DateTime reminderDate =
                                        Convert.ToDateTime(reader["ReminderDate"]);

                                    tasks += "Reminder: " +
                                        reminderDate.ToString("dd/MM/yyyy HH:mm") + "\n";
                                }
                                else
                                {
                                    tasks += "Reminder: No reminder set\n";
                                }

                                bool completed =
                                    Convert.ToBoolean(reader["IsCompleted"]);

                                tasks += "Status: " +
                                    (completed ? "Completed" : "Pending") + "\n\n";
                            }

                            if (tasks == "")
                            {
                                return "You have no saved tasks yet.";
                            }

                            return tasks;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error retrieving tasks: " + ex.Message;
            }
        }

        // Add or update a reminder
        public string AddReminder(int taskId, DateTime reminderDate)
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query =
                        "UPDATE Tasks SET ReminderDate = @reminderDate, " +
                        "ReminderNotified = FALSE WHERE TaskId = @taskId";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@reminderDate", reminderDate);
                        command.Parameters.AddWithValue("@taskId", taskId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            SaveActivity(
                                connection,
                                "Set reminder for task ID: " + taskId);

                            return "Reminder saved successfully!";
                        }
                        else
                        {
                            return "Task ID not found. Please check the task ID.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error saving reminder: " + ex.Message;
            }
        }

        // Find reminders that are due
        public string GetDueReminders()
        {
            try
            {
                using (MySqlConnection connection =
                    new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query =
                        "SELECT TaskId, Title, Description " +
                        "FROM Tasks " +
                        "WHERE ReminderDate IS NOT NULL " +
                        "AND ReminderDate <= NOW() " +
                        "AND ReminderNotified = FALSE " +
                        "AND IsCompleted = FALSE";

                    List<int> taskIds = new List<int>();
                    string reminders = "";

                    using (MySqlCommand command =
                        new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int taskId =
                                    Convert.ToInt32(reader["TaskId"]);

                                taskIds.Add(taskId);

                                reminders += "Reminder: " +
                                    reader["Title"].ToString() + "\n";

                                reminders += "Description: " +
                                    reader["Description"].ToString() + "\n\n";
                            }
                        }
                    }

                    // Mark reminders as notified
                    foreach (int taskId in taskIds)
                    {
                        string updateQuery =
                            "UPDATE Tasks SET ReminderNotified = TRUE " +
                            "WHERE TaskId = @taskId";

                        using (MySqlCommand updateCommand =
                            new MySqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue(
                                "@taskId", taskId);

                            updateCommand.ExecuteNonQuery();
                        }

                        SaveActivity(
                            connection,
                            "Reminder notification delivered for task ID: " + taskId);
                    }

                    return reminders;
                }
            }
            catch (Exception ex)
            {
                return "Error checking reminders: " + ex.Message;
            }
        }
    }
}
