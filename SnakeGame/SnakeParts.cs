using System.Windows;

namespace SnakeGame
{
	class SnakeParts
	{
		public class SnakePart
		{
			public UIElement UIElement { get; set; }
			public Point Position { get; set; }
			public bool IsHead{ get; set; }
		}
	}
}
