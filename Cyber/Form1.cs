
using Recording;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Cyber
{
    public partial class Form1 : Form
    {
        private Chat chat;
        private DatabaseManager database;

        private RichTextBox chatArea;
        private TextBox messageBox;
        private Button sendButton;

        // Automatic reminder timer
        private System.Windows.Forms.Timer reminderTimer;
        private bool reminderErrorDisplayed = false;

        private Color backgroundColor = Color.FromArgb(22, 24, 42);
        private Color purpleColor = Color.FromArgb(125, 80, 255);
        private Color cyanColor = Color.FromArgb(50, 220, 230);

        public Form1()
        {
            InitializeComponent();

            string userName = "User";
            chat = new Chat(userName);
            database = new DatabaseManager();

            CreateWindow();
            ShowWelcomeMessage(userName);
            PlayVoiceGreeting();

            TestDatabaseConnection();

            // Start automatic reminder checking
            StartReminderTimer();
        }

        private void TestDatabaseConnection()
        {
            string result = database.TestConnection();
            AddMessage("DATABASE", result, cyanColor);
        }

        // Start the reminder timer
        private void StartReminderTimer()
        {
            reminderTimer = new System.Windows.Forms.Timer();

            reminderTimer.Interval = 30000;

            reminderTimer.Tick += ReminderTimer_Tick;

            reminderTimer.Start();

            FormClosed += Form1_FormClosed;
        }

        // Check for reminders when the timer ticks
        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
            string reminders = database.GetDueReminders();

            if (string.IsNullOrWhiteSpace(reminders))
            {
                return;
            }

            if (reminders.StartsWith(
                "Error checking reminders:",
                StringComparison.OrdinalIgnoreCase))
            {
                if (!reminderErrorDisplayed)
                {
                    AddMessage("DATABASE", reminders, cyanColor);
                    reminderErrorDisplayed = true;
                }

                return;
            }

            AddMessage("REMINDER", reminders, cyanColor);

            MessageBox.Show(
                this,
                reminders,
                "Cybersecurity Bot - Task Reminder",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // Stop timer when the application closes
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (reminderTimer != null)
            {
                reminderTimer.Stop();
                reminderTimer.Dispose();
            }
        }

        private void CreateWindow()
        {
            Text = "Cybersecurity Awareness Bot";
            Size = new Size(800, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = backgroundColor;
            ForeColor = Color.White;
            MinimumSize = new Size(600, 500);

            // Header
            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 95;
            header.BackColor = purpleColor;
            Controls.Add(header);

            Logo logo = new Logo();

            Label title = new Label();
            title.Text = logo.GetTitle();
            title.Font = new Font("Segoe UI", 17, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Location = new Point(20, 15);
            title.AutoSize = true;
            header.Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = logo.GetSubtitle();
            subtitle.Font = new Font("Segoe UI", 10);
            subtitle.ForeColor = Color.White;
            subtitle.Location = new Point(23, 55);
            subtitle.AutoSize = true;
            header.Controls.Add(subtitle);

            // Cybersecurity Quiz button
            Button quizButton = new Button();
            quizButton.Text = "Take Quiz";
            quizButton.Size = new Size(130, 40);
            quizButton.Location = new Point(640, 27);
            quizButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            quizButton.BackColor = cyanColor;
            quizButton.ForeColor = backgroundColor;
            quizButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            quizButton.FlatStyle = FlatStyle.Flat;
            quizButton.FlatAppearance.BorderSize = 0;

            quizButton.Click += (sender, e) =>
            {
                database.LogActivity("Opened the cybersecurity quiz.");

                using (QuizForm quiz = new QuizForm())
                {
                    quiz.ShowDialog(this);
                }
            };

            header.Controls.Add(quizButton);

            // Chat display
            chatArea = new RichTextBox();
            chatArea.Dock = DockStyle.Fill;
            chatArea.BackColor = Color.FromArgb(31, 34, 55);
            chatArea.ForeColor = Color.White;
            chatArea.Font = new Font("Segoe UI", 11);
            chatArea.ReadOnly = true;
            chatArea.BorderStyle = BorderStyle.None;
            chatArea.DetectUrls = true;
            chatArea.HideSelection = false;
            chatArea.Padding = new Padding(10);
            Controls.Add(chatArea);

            // Bottom input area
            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 75;
            bottomPanel.BackColor = backgroundColor;
            Controls.Add(bottomPanel);

            messageBox = new TextBox();
            messageBox.Location = new Point(15, 18);
            messageBox.Width = 570;
            messageBox.Anchor = AnchorStyles.Left |
                                AnchorStyles.Right |
                                AnchorStyles.Top;
            messageBox.Font = new Font("Segoe UI", 11);
            messageBox.BackColor = Color.White;
            messageBox.ForeColor = Color.Black;
            bottomPanel.Controls.Add(messageBox);

            sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Location = new Point(600, 14);
            sendButton.Size = new Size(150, 38);
            sendButton.Anchor = AnchorStyles.Right |
                                AnchorStyles.Top;
            sendButton.BackColor = cyanColor;
            sendButton.ForeColor = Color.FromArgb(22, 24, 42);
            sendButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            sendButton.FlatStyle = FlatStyle.Flat;
            sendButton.FlatAppearance.BorderSize = 0;
            bottomPanel.Controls.Add(sendButton);

            sendButton.Click += SendButton_Click;
            messageBox.KeyDown += MessageBox_KeyDown;

            AcceptButton = sendButton;
            chatArea.BringToFront();
        }

        private void ShowWelcomeMessage(string userName)
        {
            AddMessage(
                "BOT",
                "Welcome, " + userName +
                "! Ask me about passwords, phishing, safe browsing, privacy, scams or malware.",
                purpleColor);

            AddMessage(
                "BOT",
                "You can also say 'another tip' or 'explain more'. Type 'exit' to close the bot.",
                cyanColor);

            AddMessage(
                "BOT",
                "Type 'show activity' to view your recent activities.",
                cyanColor);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string message = messageBox.Text.Trim();

            if (message == "")
            {
                AddMessage("BOT", "Please type a message first.", cyanColor);
                return;
            }

            AddMessage("YOU", message, cyanColor);
            messageBox.Clear();

            // Exit command
            if (message.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                message.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                AddMessage("BOT", "Goodbye! Remember to browse safely.", purpleColor);
                sendButton.Enabled = false;
                messageBox.Enabled = false;
                return;
            }

            // Show latest 10 activities
            if (message.Equals("show activity",
                StringComparison.OrdinalIgnoreCase))
            {
                string result = database.GetActivityLogs(10);

                AddMessage("ACTIVITY LOG", result, cyanColor);

                return;
            }

            // Show latest 20 activities
            if (message.Equals("show more activity",
                StringComparison.OrdinalIgnoreCase))
            {
                string result = database.GetActivityLogs(20);

                AddMessage("ACTIVITY LOG", result, cyanColor);

                return;
            }

            // Add task command
            if (message.StartsWith("add task:", StringComparison.OrdinalIgnoreCase))
            {
                string taskDetails = message.Substring(9).Trim();

                string[] parts = taskDetails.Split('|');

                string title = parts[0].Trim();

                string description = "";

                if (parts.Length > 1)
                {
                    description = parts[1].Trim();
                }

                if (title == "")
                {
                    AddMessage("BOT", "Please enter a task title.", cyanColor);
                    return;
                }

                string result = database.AddTask(title, description);

                AddMessage("BOT", result, cyanColor);

                return;
            }

            // Show saved tasks command
            if (message.Equals("show tasks", StringComparison.OrdinalIgnoreCase))
            {
                string result = database.GetTasks();

                AddMessage("BOT", result, cyanColor);

                return;
            }

            // Complete task command
            if (message.StartsWith("complete task:", StringComparison.OrdinalIgnoreCase))
            {
                string taskIdText = message.Substring(14).Trim();

                int taskId;

                if (!int.TryParse(taskIdText, out taskId) || taskId <= 0)
                {
                    AddMessage("BOT", "Please enter a valid task ID. Example: complete task: 1", cyanColor);
                    return;
                }

                string result = database.CompleteTask(taskId);

                AddMessage("BOT", result, cyanColor);

                return;
            }

            // Delete task command
            if (message.StartsWith("delete task:", StringComparison.OrdinalIgnoreCase))
            {
                string taskIdText = message.Substring(12).Trim();

                int taskId;

                if (!int.TryParse(taskIdText, out taskId) || taskId <= 0)
                {
                    AddMessage("BOT", "Please enter a valid task ID. Example: delete task: 1", cyanColor);
                    return;
                }

                string result = database.DeleteTask(taskId);

                AddMessage("BOT", result, cyanColor);

                return;
            }

            // Add reminder command
            if (message.StartsWith("add reminder:", StringComparison.OrdinalIgnoreCase))
            {
                string reminderDetails = message.Substring(13).Trim();

                string[] parts = reminderDetails.Split('|');

                if (parts.Length < 2)
                {
                    AddMessage("BOT",
                        "Please use this format: add reminder: TaskID | yyyy-MM-dd HH:mm",
                        cyanColor);
                    return;
                }

                int taskId;
                DateTime reminderDate;

                if (!int.TryParse(parts[0].Trim(), out taskId) || taskId <= 0)
                {
                    AddMessage("BOT", "Please enter a valid task ID.", cyanColor);
                    return;
                }

                if (!DateTime.TryParseExact(
                    parts[1].Trim(),
                    "yyyy-MM-dd HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out reminderDate))
                {
                    AddMessage("BOT",
                        "Invalid date or time. Use yyyy-MM-dd HH:mm.",
                        cyanColor);
                    return;
                }

                if (reminderDate <= DateTime.Now)
                {
                    AddMessage("BOT", "Please enter a future date and time.", cyanColor);
                    return;
                }

                string result = database.AddReminder(taskId, reminderDate);

                AddMessage("BOT", result, cyanColor);

                return;
            }

            // Normal chatbot response
            string response = chat.GetResponse(message);
            AddMessage("BOT", response, purpleColor);
        }

        private void AddMessage(string sender, string message, Color colour)
        {
            chatArea.SelectionStart = chatArea.TextLength;
            chatArea.SelectionLength = 0;

            chatArea.SelectionColor = colour;
            chatArea.SelectionFont = new Font("Segoe UI", 11, FontStyle.Bold);
            chatArea.AppendText(sender + ": ");

            chatArea.SelectionColor = Color.White;
            chatArea.SelectionFont = new Font("Segoe UI", 11);
            chatArea.AppendText(message + Environment.NewLine + Environment.NewLine);

            chatArea.SelectionStart = chatArea.TextLength;
            chatArea.ScrollToCaret();
        }

        private void PlayVoiceGreeting()
        {
            string voicePath = Path.Combine(
                AppContext.BaseDirectory,
                "voice",
                "CyberSecurityAwarenessBot.wav");

            if (File.Exists(voicePath))
            {
                try
                {
                    SoundPlayer player = new SoundPlayer(voicePath);
                    player.Play();
                }
                catch (Exception)
                {
                    AddMessage("BOT", "The voice greeting could not be played.", cyanColor);
                }
            }
            else
            {
                AddMessage("BOT", "Voice greeting file not found. You can still use the chatbot.", cyanColor);
            }
        }

        // Preserved designer event methods
        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
