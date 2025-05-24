using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace InspirationLabProjectStanSeyit.Games
{
    public partial class WordScrambleGame : Window
    {
        private List<WordPuzzle> puzzles;
        private int currentPuzzleIndex = 0;
        private int score = 0;
        private int timeLeft = 60;
        private DispatcherTimer timer;
        private Random random = new Random();

        public WordScrambleGame()
        {
            InitializeComponent();
            InitializePuzzles();
            InitializeTimer();
            LoadPuzzle();
        }

        private void InitializePuzzles()
        {
            puzzles = new List<WordPuzzle>
            {
                new WordPuzzle { Word = "COMPUTER", Hint = "A device that processes information" },
                new WordPuzzle { Word = "PROGRAMMING", Hint = "The process of creating computer software" },
                new WordPuzzle { Word = "ALGORITHM", Hint = "A step-by-step procedure for solving a problem" },
                new WordPuzzle { Word = "DATABASE", Hint = "A structured collection of data" },
                new WordPuzzle { Word = "NETWORK", Hint = "A system of connected computers" },
                new WordPuzzle { Word = "INTERNET", Hint = "A global network of computers" },
                new WordPuzzle { Word = "SOFTWARE", Hint = "Programs and operating information used by a computer" },
                new WordPuzzle { Word = "HARDWARE", Hint = "The physical components of a computer" }
            };
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

        private void LoadPuzzle()
        {
            if (currentPuzzleIndex >= puzzles.Count)
            {
                timer.Stop();
                MessageBox.Show($"Congratulations! You've completed all puzzles! Final score: {score}", "Game Over");
                this.Close();
                return;
            }

            var puzzle = puzzles[currentPuzzleIndex];
            ScrambledWordText.Text = ScrambleWord(puzzle.Word);
            HintText.Text = puzzle.Hint;
            AnswerInput.Text = "";
            AnswerInput.Focus();
        }

        private string ScrambleWord(string word)
        {
            var chars = word.ToCharArray();
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                char temp = chars[i];
                chars[i] = chars[j];
                chars[j] = temp;
            }
            return new string(chars);
        }

        private void SubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            var userAnswer = AnswerInput.Text.ToUpper();
            var correctAnswer = puzzles[currentPuzzleIndex].Word;

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
            currentPuzzleIndex++;
            LoadPuzzle();
        }

        private class WordPuzzle
        {
            public string Word { get; set; }
            public string Hint { get; set; }
        }
    }
} 