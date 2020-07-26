using System;
using GameOfLife.Core;

namespace SimpleGameOfLife.Console
{
	public sealed class ConsoleRenderer : BaseRenderer
	{
		private const ConsoleColor DEAD_COLOUR = ConsoleColor.White;

		private const ConsoleColor LIVE_COLOUR = ConsoleColor.Black;

		public const string EMPTY_BLOCK_CHAR = "  ";

		public const string FULL_BLOCK_CHAR = "\u2588\u2588";

		

		public override void Initialise(int boardWidth, int boardHeight)
		{
			base.Initialise(boardWidth, boardHeight);

			System.Console.BackgroundColor = ConsoleColor.Gray;
			System.Console.Clear();

			System.Console.CursorVisible = false;

			// Each cell is two characters wide and append an extra row on the bottom
			// just to prevent scrolling messing with the render of the board.
			int width = Math.Max(boardWidth, 8) * 2 + 1;
			int height = Math.Max(boardHeight, 8) + 1;

			System.Console.SetWindowSize(width, height);
			System.Console.SetBufferSize(width, height);

			System.Console.BackgroundColor = DEAD_COLOUR;
			System.Console.ForegroundColor = LIVE_COLOUR;
		}

		/// <summary>
		/// Renders the board to the console
		/// </summary>
		/// <param name="board"></param>
		public override void Render(bool[,] board)
		{
			string builder = string.Empty;

			System.Console.SetCursorPosition(0, 0);
			//Console.ForegroundColor = (ConsoleColor)(Generation % 14);

			for (int y = 0; y < this.BoardHeight; y++)
			{
				for (int x = 0; x < this.BoardWidth; x++)
				{
					string c = board[x, y] ? FULL_BLOCK_CHAR : EMPTY_BLOCK_CHAR;

					builder += c;
				}

				builder += "\n";
			}

			System.Console.Write(builder);
		}
	}
}