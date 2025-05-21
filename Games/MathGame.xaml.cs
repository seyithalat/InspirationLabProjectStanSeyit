using System;
using System.Windows;
using System.Windows.Threading;

namespace InspirationLabProjectStanSeyit.Games
{
    public partial class MathGame : Window
    {
        private Random random = new Random();
        private int score = 0;
        private int timeLeft = 30;
        private DispatcherTimer timer;
        private int correctAnswer;

        public MathGame()
        {
            InitializeComponent();
            InitializeTimer();
            GenerateNewProblem();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            TimerText.Text = $"Time: {timeLeft}";

            if (timeLeft <= 0)
            {
                timer.Stop();
                MessageBox.Show($"Game Over! Your final score is: {score}", "Game Over");
                this.Close();
            }
        }

        private void GenerateNewProblem()
        {
            int num1 = random.Next(1, 20);
            int num2 = random.Next(1, 20);
            string[] operators = { "+", "-", "*" };
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
                    MessageBox.Show($"Wrong! The correct answer was: {correctAnswer}", "Answer");
                }

                ScoreText.Text = $"Score: {score}";
                GenerateNewProblem();
            }
            else
            {
                MessageBox.Show("Please enter a valid number!", "Invalid Input");
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            score = 0;
            timeLeft = 30;
            ScoreText.Text = $"Score: {score}";
            TimerText.Text = $"Time: {timeLeft}";
            timer.Start();
            GenerateNewProblem();
        }
    }
} 