# 🛡️ Cybersecurity Awareness Bot

## 1. Project Description

The Cybersecurity Awareness Bot is a C# Windows Forms application designed to educate users about cybersecurity and online safety.

It provides cybersecurity tips and information about passwords, phishing, safe browsing, privacy, scams and malware. It also includes task management, reminders, an activity log and an interactive cybersecurity quiz.

## 2. Features

### Chatbot
- Recognises cybersecurity keywords and questions.
- Provides random cybersecurity tips.
- Detects emotions such as worried, curious and frustrated.
- Handles follow-up questions.
- Remembers the user's name and current topic.
- Plays a voice greeting when the application starts.

### Task Management
- Add, view, complete and delete tasks.
- Add task descriptions.
- Set reminders for future dates and times.
- Receive reminder notifications.

### Cybersecurity Quiz
- 10 multiple-choice questions.
- Four answer options per question.
- Feedback and explanations.
- Score calculation and replay option.

### Activity Log
- Records important application activities.
- Tracks task and reminder actions.
- Displays the latest 10 activities.
- Supports viewing up to 20 activities.

### Database
- MySQL integration.
- Stores tasks, reminders and activity records.
- Retrieves saved information.
- Maintains data after the application closes.

## 3. Classes Used

| Class | Purpose |
|---|---|
| Program | Starts the application. |
| Form1 | Manages the main GUI and application interactions. |
| Chat | Handles chatbot responses and NLP simulation. |
| Logo | Displays the application title and subtitle. |
| DatabaseManager | Manages MySQL operations. |
| QuizForm | Manages the cybersecurity quiz. |

## 4. Technologies Used

- C#
- Windows Forms
- .NET
- Visual Studio 2022
- MySQL Server
- MySQL Workbench
- MySql.Data (NuGet)
- Regular Expressions

## 5. Database Setup

**Database name:** `CybersecurityBotDB`

The application uses two tables:

- `Tasks` – stores tasks, descriptions, reminders and completion status.
- `ActivityLog` – stores recorded application activities and dates.

Run the following SQL commands in MySQL Workbench:

```sql
CREATE DATABASE IF NOT EXISTS CybersecurityBotDB;

USE CybersecurityBotDB;

CREATE TABLE IF NOT EXISTS Tasks
(
    TaskId INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    ReminderDate DATETIME NULL,
    IsCompleted BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ReminderNotified BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS ActivityLog
(
    ActivityId INT AUTO_INCREMENT PRIMARY KEY,
    ActivityDescription VARCHAR(255) NOT NULL,
    ActivityDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

## 6. Chatbot Commands

| Command | Purpose |
|---|---|
| `add task: Study programming` | Adds a task. |
| `show tasks` | Displays tasks. |
| `complete task: 1` | Completes a task. |
| `delete task: 1` | Deletes a task. |
| `add reminder: 1 \| 2026-11-15 14:30` | Sets a reminder. |
| `show activity` | Displays recent activities. |
| `show more activity` | Displays up to 20 activities. |
| `exit` / `quit` | Exits the chatbot. |

Reminder format: `yyyy-MM-dd HH:mm`

## 7. Project Files

- `Program.cs`
- `Form1.cs`
- `Chat.cs`
- `Logo.cs`
- `DatabaseManager.cs`
- `QuizForm.cs`
- `voice/CyberSecurityAwarenessBot.wav`
- `README.md`

## 8. How to Run

1. Install Visual Studio 2022 and the .NET Desktop Development workload.
2. Install and configure MySQL Server.
3. Create the database and tables using the SQL commands above.
4. Open `Cyber.sln` in Visual Studio.
5. Ensure the database connection is configured correctly.
6. Restore the required NuGet packages.
7. Build the solution.
8. Press **F5** to run the application.

**Note:** The application must remain open for reminder notifications to be checked and displayed.

## 9. Example Chatbot Questions

- What is cybersecurity?
- How can I protect my password?
- What is phishing?
- How can I browse safely?
- How can I protect my privacy?
- I'm worried about online scams.
- Give me another tip.
- Explain more.

## 10. Purpose of the Project

The project aims to promote cybersecurity awareness while demonstrating practical programming skills, including object-oriented programming, GUI development, database integration, CRUD operations, event handling, input validation and basic NLP simulation.

---

**Language:** C#  
**Application:** Windows Forms  
**Database:** MySQL  
**IDE:** Visual Studio 2022
