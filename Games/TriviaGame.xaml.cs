using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InspirationLabProjectStanSeyit.Games
{

    public partial class TriviaGame : Window
    {
        private List<TriviaQuestion> questions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private int lives = 3;
        private int timeLeft = 60;
        private Random random = new Random();
        private DispatcherTimer timer;

        public TriviaGame()
        {
            InitializeComponent();
            InitializeQuestions();
            InitializeTimer();
            LoadQuestion();
        }

        private void InitializeQuestions()
        {
            questions = new List<TriviaQuestion>
            {
                new TriviaQuestion
                {
                    Question = "What is the capital of France?",
                    Answers = new List<string> { "London", "Berlin", "Paris", "Madrid" },
                    CorrectAnswerIndex = 2
                },
                new TriviaQuestion
                {
                    Question = "Which planet is known as the Red Planet?",
                    Answers = new List<string> { "Venus", "Mars", "Jupiter", "Saturn" },
                    CorrectAnswerIndex = 1
                },
                new TriviaQuestion
                {
                    Question = "What is the largest mammal in the world?",
                    Answers = new List<string> { "African Elephant", "Blue Whale", "Giraffe", "Hippopotamus" },
                    CorrectAnswerIndex = 1
                },
                new TriviaQuestion
                {
                    Question = "Who painted the Mona Lisa?",
                    Answers = new List<string> { "Vincent van Gogh", "Pablo Picasso", "Leonardo da Vinci", "Michelangelo" },
                    CorrectAnswerIndex = 2
                },
                new TriviaQuestion
                {
                    Question = "What is the chemical symbol for gold?",
                    Answers = new List<string> { "Ag", "Fe", "Au", "Cu" },
                    CorrectAnswerIndex = 2
                },
                new TriviaQuestion
                {
                    Question = "Which country is known as the Land of the Rising Sun?",
                    Answers = new List<string> { "China", "Japan", "Thailand", "South Korea" },
                    CorrectAnswerIndex = 1
                },
                new TriviaQuestion
                {
                    Question = "What is the hardest natural substance on Earth?",
                    Answers = new List<string> { "Gold", "Iron", "Diamond", "Silver" },
                    CorrectAnswerIndex = 2
                },
                new TriviaQuestion
                {
                    Question = "Who wrote 'Romeo and Juliet'?",
                    Answers = new List<string> { "William Shakespeare", "Charles Dickens", "Jane Austen", "Mark Twain" },
                    CorrectAnswerIndex = 0
                },
                new TriviaQuestion
                {
                    Question = "What is the largest planet in our solar system?",
                    Answers = new List<string> { "Earth", "Jupiter", "Saturn", "Mars" },
                    CorrectAnswerIndex = 1
                },
                new TriviaQuestion
                {
                    Question = "Which element has the chemical symbol 'O'?",
                    Answers = new List<string> { "Gold", "Oxygen", "Osmium", "Oxide" },
                    CorrectAnswerIndex = 1
                },
                new TriviaQuestion
                {
                    Question = "What is the main language spoken in Brazil?",
                    Answers = new List<string> { "Spanish", "Portuguese", "French", "English" },
                    CorrectAnswerIndex = 1
                },
                new TriviaQuestion
                {
                    Question = "How many continents are there on Earth?",
                    Answers = new List<string> { "5", "6", "7", "8" },
                    CorrectAnswerIndex = 2
                },
                new TriviaQuestion
                {
                    Question = "Who discovered penicillin?",
                    Answers = new List<string> { "Marie Curie", "Alexander Fleming", "Isaac Newton", "Albert Einstein" },
                    CorrectAnswerIndex = 1
                },
                new TriviaQuestion
                {
                    Question = "What is the smallest prime number?",
                    Answers = new List<string> { "0", "1", "2", "3" },
                    CorrectAnswerIndex = 2
                },
                new TriviaQuestion
                {
                    Question = "Which ocean is the largest?",
                    Answers = new List<string> { "Atlantic", "Indian", "Pacific", "Arctic" },
                    CorrectAnswerIndex = 2
                }
            };
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

        private void LoadQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                MessageBox.Show($"Game Over! Your final score is: {score}", "Game Over");
                EndGame();
                return;
            }
            if (lives <= 0)
            {
                MessageBox.Show($"No lives left! Your final score is: {score}", "Game Over");
                EndGame();
                return;
            }
            LivesText.Text = $"Lives: {lives}";
            TimerText.Text = $"Time: {timeLeft}";

            var question = questions[currentQuestionIndex];
            QuestionText.Text = question.Question;

            // Shuffle answers
            var shuffledAnswers = new List<string>(question.Answers);
            for (int i = shuffledAnswers.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                string temp = shuffledAnswers[i];
                shuffledAnswers[i] = shuffledAnswers[j];
                shuffledAnswers[j] = temp;
            }

            Answer1.Content = shuffledAnswers[0];
            Answer2.Content = shuffledAnswers[1];
            Answer3.Content = shuffledAnswers[2];
            Answer4.Content = shuffledAnswers[3];

            ScoreText.Text = $"Score: {score}";
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var selectedAnswer = button.Content.ToString();
            var correctAnswer = questions[currentQuestionIndex].Answers[questions[currentQuestionIndex].CorrectAnswerIndex];

            if (selectedAnswer == correctAnswer)
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

            currentQuestionIndex++;
            LoadQuestion();
        }

        private class TriviaQuestion
        {
            public string Question { get; set; }
            public List<string> Answers { get; set; }
            public int CorrectAnswerIndex { get; set; }
        }
        private void EndGame()
        {
            if (timer != null)
                timer.Stop();
            Data.SaveGameScore(Session.CurrentUserId, "Trivia", score, DateTime.Now);
            // Show game over UI, etc...
        }
    }
} 