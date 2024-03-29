﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using static SnakeGame.SnakeParts;

namespace SnakeGame
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		const int SnakeSquareSize = 20;
		const int SnakeStartLength = 3;
		const int SnakeStartSpeed = 400;
		const int SnakeSpeedTreshoald = 100;

		private SolidColorBrush snakeBodyBrush = Brushes.Green;
		private SolidColorBrush snakeHeadBrush = Brushes.YellowGreen;
		private List<SnakePart> snakeParts = new List<SnakePart>();

		public enum SnakeDirection { Left, Right, Up, Down};
		private SnakeDirection snakeDirection = SnakeDirection.Right;
		private int snakeLength;

		private DispatcherTimer gameTickTimer = new DispatcherTimer();

		private Random rnd = new Random();
		private UIElement snakeFood = null;
		private SolidColorBrush foodBrush = Brushes.Purple;

		private int currentScore = 0;

		public MainWindow()
		{
			InitializeComponent();
			gameTickTimer.Tick += GameTickTimer_Tick;
		}

		

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			StartNewGame();
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			SnakeDirection originalSnakeDirection = snakeDirection;
			switch (e.Key)
			{
				case Key.Up:
					if (snakeDirection != SnakeDirection.Down)
						snakeDirection = SnakeDirection.Up;
					break;
				case Key.Down:
					if (snakeDirection != SnakeDirection.Up)
						snakeDirection = SnakeDirection.Down;
					break;
				case Key.Left:
					if (snakeDirection != SnakeDirection.Right)
						snakeDirection = SnakeDirection.Left;
					break;
				case Key.Right:
					if (snakeDirection != SnakeDirection.Left)
						snakeDirection = SnakeDirection.Right;
					break;
				case Key.Space:
					StartNewGame();
					break;
			}

			if (snakeDirection != originalSnakeDirection)
				MoveSnake();
				
		}

		

		private void DrawSnake()
		{
			foreach (SnakePart snakePart in snakeParts)
			{
				if(snakePart.UIElement == null)
				{
					snakePart.UIElement = new Rectangle
					{
						Width = SnakeSquareSize,
						Height = SnakeSquareSize,
						Fill = (snakePart.IsHead ? snakeHeadBrush : snakeBodyBrush)
					};
					GameArea.Children.Add(snakePart.UIElement);
					Canvas.SetTop(snakePart.UIElement, snakePart.Position.Y);
					Canvas.SetLeft(snakePart.UIElement, snakePart.Position.X);
				}
			}
		}

		private void MoveSnake()
		{
			while(snakeParts.Count >= snakeLength)
			{
				GameArea.Children.Remove(snakeParts[0].UIElement);
				snakeParts.RemoveAt(0);
			}

			foreach(SnakePart snakePart in snakeParts)
			{
				(snakePart.UIElement as Rectangle).Fill = snakeBodyBrush;
				snakePart.IsHead = false;
			}

			SnakePart snakeHead = snakeParts[snakeParts.Count - 1];
			double nextX = snakeHead.Position.X;
			double nextY = snakeHead.Position.Y;

			switch (snakeDirection)
			{
				case SnakeDirection.Left:
					nextX -= SnakeSquareSize;
					break;
				case SnakeDirection.Right:
					nextX += SnakeSquareSize;
					break;
				case SnakeDirection.Up:
					nextY -= SnakeSquareSize;
					break;
				case SnakeDirection.Down:
					nextY += SnakeSquareSize;
					break;
			}

			snakeParts.Add(new SnakePart()
			{
				Position = new Point(nextX, nextY),
				IsHead = true
			});

			DrawSnake();

			DoCollisionCheck();
		}
		private void GameTickTimer_Tick(object sender, EventArgs e)
		{
			MoveSnake();
		}

		private void StartNewGame()
		{
			foreach(SnakePart snakeBodyPart in snakeParts)
			{
				if (snakeBodyPart.UIElement != null)
					GameArea.Children.Remove(snakeBodyPart.UIElement);
			}
			snakeParts.Clear();
			if (snakeFood != null)
				GameArea.Children.Remove(snakeFood);

			currentScore = 0;
			snakeLength = SnakeStartLength;
			snakeDirection = SnakeDirection.Right;
			snakeParts.Add(new SnakePart() { Position = new Point(SnakeSquareSize * 5, SnakeSquareSize * 5) });
			gameTickTimer.Interval = TimeSpan.FromMilliseconds(SnakeStartSpeed);

			DrawSnake();
			DrawSnakeFood();

			UpdateGameStatus();

			gameTickTimer.IsEnabled = true;
		}

		private Point GetNextFoodPosition()
		{
			int maxX = (int)(GameArea.ActualWidth / SnakeSquareSize);
			int maxY = (int)(GameArea.ActualHeight / SnakeSquareSize);
			int foodX = rnd.Next(0, maxX) * SnakeSquareSize;
			int foodY = rnd.Next(0, maxY) * SnakeSquareSize;

			foreach (SnakePart snakePart in snakeParts)
			{
				if ((snakePart.Position.X == foodX) && (snakePart.Position.Y == foodY))
					return GetNextFoodPosition();
			}

			return new Point(foodX, foodY);
		}

		private void DrawSnakeFood()
		{
			Point foodPosition = GetNextFoodPosition();

			snakeFood = new Ellipse()
			{
				Width = SnakeSquareSize,
				Height = SnakeSquareSize,
				Fill = foodBrush
			};

			GameArea.Children.Add(snakeFood);
			Canvas.SetTop(snakeFood, foodPosition.Y);
			Canvas.SetLeft(snakeFood, foodPosition.X);
		}

		private void DoCollisionCheck()
		{
			SnakePart snakeHead = snakeParts[snakeParts.Count - 1];

			if((snakeHead.Position.X == Canvas.GetLeft(snakeFood)) && (snakeHead.Position.Y == Canvas.GetTop(snakeFood)))
			{
				EatSnakeFood();
				return;
			}

			if((snakeHead.Position.Y < 0) || (snakeHead.Position.Y >= GameArea.ActualHeight) ||
				(snakeHead.Position.X < 0) || (snakeHead.Position.X >= GameArea.ActualWidth))
			{
				EndGame();
			}

			foreach(SnakePart snakeBodyPart in snakeParts.Take(snakeParts.Count - 1))
			{
				if((snakeHead.Position.X == snakeBodyPart.Position.X) && (snakeHead.Position.Y == snakeBodyPart.Position.Y))
				{
					EndGame();
				}
			}
		}

		private void EndGame()
		{
			gameTickTimer.IsEnabled = false;
			MessageBox.Show("You lose, to retry press SPACE", "Snake Game");
		}

		private void EatSnakeFood()
		{
			snakeLength++;
			currentScore++;

			int timerInterval = Math.Max(SnakeSpeedTreshoald, (int)gameTickTimer.Interval.TotalMilliseconds - (currentScore * 2));
			gameTickTimer.Interval = TimeSpan.FromMilliseconds(timerInterval);

			GameArea.Children.Remove(snakeFood);
			DrawSnakeFood();
			UpdateGameStatus();

			
			ImageBrush ib = new ImageBrush();
			SnakePics pics = new SnakePics();
			ib.ImageSource = pics.DiplayPic();
			GameArea.Background = ib;
		}

		private void UpdateGameStatus()
		{
			this.Title = "Score: " + currentScore + " - Speed: " + gameTickTimer.Interval.TotalMilliseconds;
		}
	}
}
