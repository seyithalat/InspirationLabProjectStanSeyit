using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace InspirationLabProjectStanSeyit.Games
{
    public partial class TriviaGame : Window
    {
        private List<TriviaQuestion> questions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private Random random = new Random();

        public TriviaGame()
        {
            InitializeComponent();
            InitializeQuestions();
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
                }
            };
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                MessageBox.Show($"Game Over! Your final score is: {score}", "Game Over");
                this.Close();
                return;
            }

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
                MessageBox.Show($"Wrong! The correct answer was: {correctAnswer}", "Answer");
            }

            currentQuestionIndex++;
            LoadQuestion();
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex++;
            LoadQuestion();
        }
    }

    public class TriviaQuestion
    {
        public string Question { get; set; }
        public List<string> Answers { get; set; }
        public int CorrectAnswerIndex { get; set; }
    }
} 