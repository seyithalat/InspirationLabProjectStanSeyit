using System;
using System.Windows;
using System.Windows.Threading;

namespace InspirationLabProjectStanSeyit.Games
{
    public partial class MathGame : Window
    {
        private Random random = new Random();
        private int score = 0;
        private int lives = 3;
        private int timeLeft = 60;
        private DispatcherTimer timer;
        private int correctAnswer;

        public MathGame()
        {
            InitializeComponent();
            InitializeTimer();
            LivesText.Text = $"Lives: {lives}";
            GenerateNewProblem();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            TimerText.Text = $"Time: {timeLeft}";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            TimerText.Text = $"Time: {timeLeft}";

            if (timeLeft <= 0)
            {
                timer.Stop();
                MessageBox.Show($"Time's up! Your final score is: {score}", "Game Over");
                EndGame();
            }
        }

        private void GenerateNewProblem()
        {
            int num1 = random.Next(1, 101);
            int num2 = random.Next(1, 101);
            string[] operators = { "+", "-", "*", "/" };
            string op = operators[random.Next(operators.Length)];

            switch (op)
            {
                case "+":
                    correctAnswer = num1 + num2;
                    break;
                case "-":
                    correctAnswer = num1 - num2;
                    break;
                case "*":
                    correctAnswer = num1 * num2;
                    break;
                case "/":
                    num2 = random.Next(1, 13);
                    correctAnswer = random.Next(1, 13);
                    num1 = correctAnswer * num2;
                    break;
            }

            ProblemText.Text = $"{num1} {op} {num2} = ?";
            AnswerInput.Text = "";
            AnswerInput.Focus();
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AnswerInput.Text, out int userAnswer))
            {
                if (userAnswer == correctAnswer)
                {
                    score += 10;
                    MessageBox.Show("Correct!", "Answer");
                }
                else
                {
                    lives--;
                    LivesText.Text = $"Lives: {lives}";
                    MessageBox.Show($"Wrong! The correct answer was: {correctAnswer}", "Answer");
                    if (lives <= 0)
                    {
                        MessageBox.Show($"No lives left! Your final score is: {score}", "Game Over");
                        EndGame();
                        return;
                    }
                }

                ScoreText.Text = $"Score: {score}";
                GenerateNewProblem();
            }
            else
            {
                MessageBox.Show("Please enter a valid number!", "Invalid Input");
            }
        }
        private void EndGame()
        {
            if (timer != null)
                timer.Stop();
            Data.SaveGameScore(Session.CurrentUserId, "Math", score, DateTime.Now);
            // Show game over UI, etc.
        }
    }
} 