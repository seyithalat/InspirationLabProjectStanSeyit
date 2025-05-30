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
        private int lives = 3;
        private int timeLeft = 60;
        private DispatcherTimer timer;
        private WordPuzzle currentPuzzle;
        private Random random = new Random();

        public WordScrambleGame()
        {
            InitializeComponent();
            InitializePuzzles();
            InitializeTimer();
            LivesText.Text = $"Lives: {lives}";
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
                new WordPuzzle { Word = "HARDWARE", Hint = "The physical components of a computer" },
                new WordPuzzle { Word = "DEVELOPMENT", Hint = "The process of creating something new" },
                new WordPuzzle { Word = "ENCAPSULATION", Hint = "OOP concept: wrapping data and code together" },
                new WordPuzzle { Word = "INHERITANCE", Hint = "OOP concept: deriving new classes from existing ones" },
                new WordPuzzle { Word = "POLYMORPHISM", Hint = "OOP concept: many forms" },
                new WordPuzzle { Word = "ABSTRACTION", Hint = "OOP concept: hiding complex reality" },
                new WordPuzzle { Word = "MULTITHREADING", Hint = "Running multiple threads simultaneously" },
                new WordPuzzle { Word = "SYNCHRONIZATION", Hint = "Making sure things happen in order" },
                new WordPuzzle { Word = "SERIALIZATION", Hint = "Turning objects into a format for storage or transmission" },
                new WordPuzzle { Word = "DESERIALIZATION", Hint = "Turning data back into objects" },
                new WordPuzzle { Word = "AUTHENTICATION", Hint = "Verifying identity" },
                new WordPuzzle { Word = "AUTHORIZATION", Hint = "Granting access rights" },
                new WordPuzzle { Word = "CONFIGURATION", Hint = "Settings for software or hardware" },
                new WordPuzzle { Word = "IMPLEMENTATION", Hint = "The act of putting a plan into effect" },
                new WordPuzzle { Word = "DOCUMENTATION", Hint = "Written details about a program" },
                new WordPuzzle { Word = "REFACTORING", Hint = "Improving code without changing its behavior" },
                new WordPuzzle { Word = "DEBUGGING", Hint = "Finding and fixing errors in code" },
                new WordPuzzle { Word = "OPTIMIZATION", Hint = "Making code run faster or use less resources" },
                new WordPuzzle { Word = "COMPATIBILITY", Hint = "Ability to work with other systems" },
                new WordPuzzle { Word = "MAINTAINABILITY", Hint = "How easy it is to keep code working" },
                new WordPuzzle { Word = "SCALABILITY", Hint = "Ability to handle growth" },
                new WordPuzzle { Word = "PERFORMANCE", Hint = "How well something works" },
                new WordPuzzle { Word = "ALGORITHMIC", Hint = "Relating to algorithms" },
                new WordPuzzle { Word = "FUNCTIONALITY", Hint = "What something can do" },
                new WordPuzzle { Word = "RESPONSIVENESS", Hint = "How quickly something reacts" },
                new WordPuzzle { Word = "SUSTAINABILITY", Hint = "Ability to be maintained over time" },
                new WordPuzzle { Word = "INTEGRATION", Hint = "Combining parts into a whole" },
                new WordPuzzle { Word = "ARCHITECTURE", Hint = "The structure of a system" },
                new WordPuzzle { Word = "DEPENDENCIES", Hint = "Other things your code relies on" },
                new WordPuzzle { Word = "REQUIREMENTS", Hint = "What needs to be done" },
                new WordPuzzle { Word = "SPECIFICATION", Hint = "Detailed description of requirements" },
                new WordPuzzle { Word = "CONCURRENCY", Hint = "Multiple things happening at once" },
                new WordPuzzle { Word = "ASYNCHRONOUS", Hint = "Not happening at the same time" },
                new WordPuzzle { Word = "SYNCHRONOUS", Hint = "Happening at the same time" },
                new WordPuzzle { Word = "DEPRECATION", Hint = "Marked for future removal" },
                new WordPuzzle { Word = "COMPILATION", Hint = "Turning code into executable form" },
                new WordPuzzle { Word = "INTERPRETER", Hint = "Executes code line by line" },
                new WordPuzzle { Word = "COMPILER", Hint = "Turns code into machine code" },
                new WordPuzzle { Word = "FRAMEWORK", Hint = "A platform for developing applications" },
                new WordPuzzle { Word = "LIBRARY", Hint = "Reusable code" },
                new WordPuzzle { Word = "ALPHABETIZATION", Hint = "Arranging in alphabetical order" }
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

        private void LoadPuzzle()
        {
            if (currentPuzzleIndex >= puzzles.Count)
            {
                timer.Stop();
                MessageBox.Show($"Congratulations! You've completed all puzzles! Final score: {score}", "Game Over");
                EndGame();
                return;
            }
            if (lives <= 0)
            {
                timer.Stop();
                MessageBox.Show($"No lives left! Your final score is: {score}", "Game Over");
                EndGame();
                return;
            }
            LivesText.Text = $"Lives: {lives}";
            TimerText.Text = $"Time: {timeLeft}";

            // Increase difficulty: use longer words as player progresses
            currentPuzzle = puzzles[currentPuzzleIndex];
            if (currentPuzzleIndex > 5 && currentPuzzle.Word.Length < 8)
            {
                // Replace with a harder word
                currentPuzzle = new WordPuzzle { Word = "DEVELOPMENT", Hint = "The process of creating something new" };
            }
            if (currentPuzzleIndex > 10 && currentPuzzle.Word.Length < 10)
            {
                currentPuzzle = new WordPuzzle { Word = "ENCAPSULATION", Hint = "OOP concept: wrapping data and code together" };
            }
            ScrambledWordText.Text = ScrambleWord(currentPuzzle.Word);
            HintText.Text = currentPuzzle.Hint;
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
            var correctAnswer = currentPuzzle.Word;

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
                // No lives left, end game
                {
                    timer.Stop();
                    MessageBox.Show($"No lives left! Your final score is: {score}", "Game Over");
                    EndGame();
                    return;
                }
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

        private void EndGame()
        {
            if (timer != null)
                timer.Stop();
            Data.SaveGameScore(Session.CurrentUserId, "WordScramble", score, DateTime.Now);
            // Show game over UI, etc...
        }
    }
}