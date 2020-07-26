using System;

namespace SimpleGameOfLife
{
	public class ConsoleRenderer : IRenderer
	{
		private const ConsoleColor DEAD_COLOUR = ConsoleColor.White;

		private const ConsoleColor LIVE_COLOUR = ConsoleColor.Black;

		public const string EMPTY_BLOCK_CHAR = "  ";

		public const string FULL_BLOCK_CHAR = "\u2588\u2588";

		public void Initialise(int boardWidth, int boardHeight)
		{
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.Clear();

			Console.CursorVisible = false;

			// Each cell is two characters wide and append an extra row on the bottom
			// just to prevent scrolling messing with the render of the board.
			int width = Math.Max(boardWidth, 8) * 2 + 1;
			int height = Math.Max(boardHeight, 8) + 1;

			Console.SetWindowSize(width, height);
			Console.SetBufferSize(width, height);

			Console.BackgroundColor = DEAD_COLOUR;
			Console.ForegroundColor = LIVE_COLOUR;
		}

		/// <summary>
		/// Renders the board to the console
		/// </summary>
		/// <param name="board"></param>
		public void Render(bool[,] board)
		{
			string builder = string.Empty;

			int width = board.GetLength(0);
			int height = board.GetLength(1);

			Console.SetCursorPosition(0, 0);
			//Console.ForegroundColor = (ConsoleColor)(Generation % 14);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					string c = board[x, y] ? FULL_BLOCK_CHAR : EMPTY_BLOCK_CHAR;

					builder += c;
				}

				builder += "\n";
			}

			Console.Write(builder);
		}
	}
}