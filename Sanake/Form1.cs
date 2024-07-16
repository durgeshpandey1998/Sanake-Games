using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sanake
{
    public partial class Form1 : Form
    {
        private const int GridSize = 20;
        private const int GridWidth = 30;
        private const int GridHeight = 20;
        private const int StartLength = 3;

        private List<Point> snake;
        private Point fruit;
        private int score;
        private bool gameOver;
        private Direction direction;
        private Timer gameTimer;

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            this.KeyPreview = true;
            this.Focus(); 
            this.KeyDown += Form1_KeyDown; 
        }

        private void InitializeGame()
        {
            snake = new List<Point>();
            snake.Add(new Point(GridWidth / 2, GridHeight / 2)); 
            for (int i = 1; i < StartLength; i++)
            {
                snake.Add(new Point(snake[0].X - i, snake[0].Y)); // Initialize snake body
            }

            GenerateFruit();

            score = 0;
            gameOver = false;
            direction = Direction.Right;

            gameTimer = new Timer();
            gameTimer.Interval = 100; // Set game speed (higher value = slower)
            gameTimer.Tick += UpdateGame;
            gameTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics canvas = e.Graphics;

            if (!gameOver)
            {
                // Draw snake
                foreach (Point segment in snake)
                {
                    Brush snakeColor = Brushes.Green; // All segments are green
                    canvas.FillRectangle(snakeColor, segment.X * GridSize, segment.Y * GridSize, GridSize, GridSize);
                }

                // Draw fruit
                canvas.FillEllipse(Brushes.Red, fruit.X * GridSize, fruit.Y * GridSize, GridSize, GridSize);
            }
            else
            {
                string gameOverText = "Game Over\nYour score: " + score;
                Font gameOverFont = new Font("Arial", 20, FontStyle.Bold);
                SizeF textSize = canvas.MeasureString(gameOverText, gameOverFont);
                PointF textPosition = new PointF((ClientSize.Width - textSize.Width) / 2, (ClientSize.Height - textSize.Height) / 2);
                canvas.DrawString(gameOverText, gameOverFont, Brushes.Black, textPosition);
            }
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            if (!gameOver)
            {
                MoveSnake();
                CheckCollision();
                Invalidate(); // Redraw the form
            }
        }

        private void MoveSnake()
        {
            for (int i = snake.Count - 1; i > 0; i--)
            {
                snake[i] = snake[i - 1]; // Move body segment to the position of the segment ahead
            }

            // Move head based on direction
            switch (direction)
            {
                case Direction.Up:
                    snake[0] = new Point(snake[0].X, snake[0].Y - 1);
                    break;
                case Direction.Down:
                    snake[0] = new Point(snake[0].X, snake[0].Y + 1);
                    break;
                case Direction.Left:
                    snake[0] = new Point(snake[0].X - 1, snake[0].Y);
                    break;
                case Direction.Right:
                    snake[0] = new Point(snake[0].X + 1, snake[0].Y);
                    break;
            }
        }

        private void GenerateFruit()
        {
            Random random = new Random();
            fruit = new Point(random.Next(0, GridWidth), random.Next(0, GridHeight));
        }

        private void CheckCollision()
        {
            // Check collision with walls or itself
            if (snake[0].X < 0 || snake[0].X >= GridWidth || snake[0].Y < 0 || snake[0].Y >= GridHeight)
            {
                gameOver = true;
            }

            for (int i = 1; i < snake.Count; i++)
            {
                if (snake[0].Equals(snake[i]))
                {
                    gameOver = true;
                    break;
                }
            }

            // Check collision with fruit
            if (snake[0].Equals(fruit))
            {
                score++;
                snake.Add(snake[snake.Count - 1]); // Add new segment to the snake
                GenerateFruit();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle keyboard input for snake direction
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != Direction.Down)
                        direction = Direction.Up;
                    break;
                case Keys.Down:
                    if (direction != Direction.Up)
                        direction = Direction.Down;
                    break;
                case Keys.Left:
                    if (direction != Direction.Right)
                        direction = Direction.Left;
                    break;
                case Keys.Right:
                    if (direction != Direction.Left)
                        direction = Direction.Right;
                    break;
                case Keys.Space:
                    if (gameOver)
                    {
                        InitializeGame();
                    }
                    break;
            }
        }
    }
}
