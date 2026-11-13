
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Cyber
{
    public class QuizForm : Form
    {
        private class Question
        {
            public string Text;
            public string[] Options;
            public int CorrectAnswer;
            public string Explanation;

            public Question(string text, string[] options,
                int correctAnswer, string explanation)
            {
                Text = text;
                Options = options;
                CorrectAnswer = correctAnswer;
                Explanation = explanation;
            }
        }

        private List<Question> questions;
        private int currentQuestion = 0;
        private int score = 0;
        private bool answered = false;

        private Label progressLabel;
        private Label scoreLabel;
        private Label questionLabel;
        private Label feedbackLabel;
        private Button[] answerButtons;
        private Button nextButton;

        private Color backgroundColor = Color.FromArgb(22, 24, 42);
        private Color purpleColor = Color.FromArgb(125, 80, 255);
        private Color cyanColor = Color.FromArgb(50, 220, 230);

        public QuizForm()
        {
            CreateQuestions();
            CreateQuizWindow();
            DisplayQuestion();
        }

        private void CreateQuestions()
        {
            questions = new List<Question>
            {
                new Question(
                    "1. What makes a password strong?",
                    new string[] {
                        "Your first name",
                        "A long, unique password",
                        "Your birthday",
                        "The word password"
                    },
                    1,
                    "A long, unique password is harder to guess. Avoid reusing passwords."
                ),

                new Question(
                    "2. What should you do with a suspicious email link?",
                    new string[] {
                        "Click it immediately",
                        "Forward it to everyone",
                        "Verify the sender before clicking",
                        "Enter your password"
                    },
                    2,
                    "Always verify unexpected messages and links before interacting with them."
                ),

                new Question(
                    "3. What is phishing?",
                    new string[] {
                        "A type of computer",
                        "An attempt to steal information through deception",
                        "A software update",
                        "A secure website"
                    },
                    1,
                    "Phishing uses deceptive messages or websites to trick people into sharing information."
                ),

                new Question(
                    "4. What does multi-factor authentication do?",
                    new string[] {
                        "Adds another verification step",
                        "Deletes your account",
                        "Makes passwords public",
                        "Disables security"
                    },
                    0,
                    "Multi-factor authentication adds another layer of protection when signing in."
                ),

                new Question(
                    "5. What does HTTPS indicate?",
                    new string[] {
                        "The website is always trustworthy",
                        "The connection is encrypted",
                        "The website has no viruses",
                        "The website is free"
                    },
                    1,
                    "HTTPS encrypts information sent between your browser and the website, but does not guarantee the site is legitimate."
                ),

                new Question(
                    "6. What should you do with an unexpected attachment?",
                    new string[] {
                        "Open it immediately",
                        "Download it without checking",
                        "Verify the sender and attachment first",
                        "Share it online"
                    },
                    2,
                    "Unexpected attachments can contain malware. Verify them before opening."
                ),

                new Question(
                    "7. Why should you be careful about sharing personal information online?",
                    new string[] {
                        "It can be misused by others",
                        "It improves your password",
                        "It removes malware",
                        "It makes every website safe"
                    },
                    0,
                    "Personal information can be used for identity theft, scams and other harmful activities."
                ),

                new Question(
                    "8. What is malware?",
                    new string[] {
                        "A security password",
                        "A harmful type of software",
                        "A computer screen",
                        "A search engine"
                    },
                    1,
                    "Malware is software designed to harm devices, steal information or disrupt systems."
                ),

                new Question(
                    "9. Why are software updates important?",
                    new string[] {
                        "They remove all passwords",
                        "They make your information public",
                        "They can fix security weaknesses",
                        "They stop internet access"
                    },
                    2,
                    "Updates often fix security vulnerabilities and improve software protection."
                ),

                new Question(
                    "10. What should you do if you suspect your account has been compromised?",
                    new string[] {
                        "Ignore the problem",
                        "Share your password",
                        "Change your password through the official website and enable MFA",
                        "Delete all your emails"
                    },
                    2,
                    "Secure your account using its official website, change your password and enable multi-factor authentication."
                )
            };
        }

        private void CreateQuizWindow()
        {
            Text = "Cybersecurity Awareness Quiz";
            Size = new Size(780, 600);
            MinimumSize = new Size(780, 600);
            MaximumSize = new Size(780, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = backgroundColor;
            ForeColor = Color.White;

            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 85;
            header.BackColor = purpleColor;
            Controls.Add(header);

            Label title = new Label();
            title.Text = "CYBERSECURITY QUIZ";
            title.Font = new Font("Segoe UI", 19, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Location = new Point(25, 12);
            title.AutoSize = true;
            header.Controls.Add(title);

            Label subtitle = new Label();
            subtitle.Text = "Test your cybersecurity knowledge!";
            subtitle.Font = new Font("Segoe UI", 10);
            subtitle.ForeColor = Color.White;
            subtitle.Location = new Point(28, 50);
            subtitle.AutoSize = true;
            header.Controls.Add(subtitle);

            progressLabel = new Label();
            progressLabel.Location = new Point(30, 105);
            progressLabel.Size = new Size(300, 30);
            progressLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            Controls.Add(progressLabel);

            scoreLabel = new Label();
            scoreLabel.Location = new Point(550, 105);
            scoreLabel.Size = new Size(180, 30);
            scoreLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            scoreLabel.ForeColor = cyanColor;
            scoreLabel.TextAlign = ContentAlignment.MiddleRight;
            Controls.Add(scoreLabel);

            questionLabel = new Label();
            questionLabel.Location = new Point(30, 155);
            questionLabel.Size = new Size(700, 75);
            questionLabel.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            questionLabel.ForeColor = Color.White;
            Controls.Add(questionLabel);

            answerButtons = new Button[4];

            for (int i = 0; i < 4; i++)
            {
                int answerIndex = i;

                Button answerButton = new Button();
                answerButton.Size = new Size(340, 65);
                answerButton.BackColor = Color.FromArgb(45, 48, 72);
                answerButton.ForeColor = Color.White;
                answerButton.Font = new Font("Segoe UI", 10);
                answerButton.FlatStyle = FlatStyle.Flat;
                answerButton.FlatAppearance.BorderColor = purpleColor;
                answerButton.FlatAppearance.BorderSize = 1;

                answerButton.Click += (sender, e) =>
                    CheckAnswer(answerIndex);

                answerButtons[i] = answerButton;
                Controls.Add(answerButton);
            }

            answerButtons[0].Location = new Point(30, 255);
            answerButtons[1].Location = new Point(385, 255);
            answerButtons[2].Location = new Point(30, 335);
            answerButtons[3].Location = new Point(385, 335);

            feedbackLabel = new Label();
            feedbackLabel.Location = new Point(30, 420);
            feedbackLabel.Size = new Size(700, 65);
            feedbackLabel.Font = new Font("Segoe UI", 10);
            feedbackLabel.ForeColor = cyanColor;
            Controls.Add(feedbackLabel);

            nextButton = new Button();
            nextButton.Text = "Next Question";
            nextButton.Location = new Point(535, 500);
            nextButton.Size = new Size(195, 40);
            nextButton.BackColor = cyanColor;
            nextButton.ForeColor = backgroundColor;
            nextButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            nextButton.FlatStyle = FlatStyle.Flat;
            nextButton.FlatAppearance.BorderSize = 0;
            nextButton.Visible = false;
            nextButton.Click += NextButton_Click;
            Controls.Add(nextButton);
        }

        private void DisplayQuestion()
        {
            answered = false;

            Question question = questions[currentQuestion];

            progressLabel.Text =
                "Question " + (currentQuestion + 1) +
                " of " + questions.Count;

            scoreLabel.Text = "Score: " + score;

            questionLabel.Text = question.Text;

            feedbackLabel.Text = "";

            nextButton.Visible = false;

            for (int i = 0; i < 4; i++)
            {
                answerButtons[i].Text = question.Options[i];
                answerButtons[i].Enabled = true;
                answerButtons[i].BackColor =
                    Color.FromArgb(45, 48, 72);
                answerButtons[i].ForeColor = Color.White;
            }
        }

        private void CheckAnswer(int selectedAnswer)
        {
            if (answered)
            {
                return;
            }

            answered = true;

            Question question = questions[currentQuestion];

            if (selectedAnswer == question.CorrectAnswer)
            {
                score++;

                feedbackLabel.Text =
                    "Correct! " + question.Explanation;

                feedbackLabel.ForeColor = Color.LightGreen;
            }
            else
            {
                feedbackLabel.Text =
                    "Incorrect. " +
                    question.Explanation;

                feedbackLabel.ForeColor = Color.Salmon;
            }

            scoreLabel.Text = "Score: " + score;

            answerButtons[question.CorrectAnswer].BackColor =
                Color.DarkGreen;

            answerButtons[question.CorrectAnswer].ForeColor =
                Color.White;

            if (selectedAnswer != question.CorrectAnswer)
            {
                answerButtons[selectedAnswer].BackColor =
                    Color.DarkRed;
            }

            foreach (Button button in answerButtons)
            {
                button.Enabled = false;
            }

            nextButton.Visible = true;

            if (currentQuestion == questions.Count - 1)
            {
                nextButton.Text = "View Results";
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (currentQuestion < questions.Count - 1)
            {
                currentQuestion++;
                DisplayQuestion();
            }
            else
            {
                ShowResults();
            }
        }

        private void ShowResults()
        {
            foreach (Button button in answerButtons)
            {
                button.Visible = false;
            }

            questionLabel.Text = "Quiz Completed!";

            progressLabel.Text = "Final Results";

            scoreLabel.Text =
                "Score: " + score + " / " + questions.Count;

            if (score >= 8)
            {
                feedbackLabel.Text =
                    "Excellent work! You have a strong understanding of cybersecurity.";
            }
            else if (score >= 5)
            {
                feedbackLabel.Text =
                    "Good effort! Keep learning and practising safe online habits.";
            }
            else
            {
                feedbackLabel.Text =
                    "Keep practising! Review the cybersecurity tips and try again.";
            }

            nextButton.Text = "Play Again";
            nextButton.Click -= NextButton_Click;
            nextButton.Click += RestartQuiz;
        }

        private void RestartQuiz(object sender, EventArgs e)
        {
            currentQuestion = 0;
            score = 0;

            foreach (Button button in answerButtons)
            {
                button.Visible = true;
            }

            nextButton.Click -= RestartQuiz;
            nextButton.Click += NextButton_Click;
            nextButton.Text = "Next Question";

            DisplayQuestion();
        }
    }
}
