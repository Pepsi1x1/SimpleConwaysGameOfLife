using System;
using System.IO;
using System.Threading;

namespace SimpleGameOfLife
{
	class Program
	{
		private const ConsoleColor DEAD_COLOUR = ConsoleColor.White;

		private const ConsoleColor LIVE_COLOUR = ConsoleColor.Black;

		public const string EMPTY_BLOCK_CHAR = "  ";

		public const string FULL_BLOCK_CHAR = "\u2588\u2588";

		private const string TITLE_FORMAT = "Conway's Game of Life - Generation {0}";

		public static int Generation = 0;

		public static EventHandler SaveEventHandler;

		private static GameOfLife _gameOfLife;

		public static EventHandler QuitEventHandler;

		public static EventHandler NewBoardEventHandler;

		public static EventHandler LoopEdgesEventHandler;

		public static EventHandler RestartCurrentEventHandler;

		public static bool Running = true;

		static void Main(string[] args)
		{
			int maxHeight = Console.LargestWindowHeight;

			int maxWidth = Console.LargestWindowWidth;

			//GameOfLife = new GameOfLife(maxWidth / 2 - 1, maxHeight - 1);

			//GameOfLife = new GameOfLife(60, 20);

			bool[,] seed = LoadSeed("GosperGliderGun.seed");

			_gameOfLife = new GameOfLife(seed);

			SaveEventHandler += OnSaveBoard;

			QuitEventHandler += OnQuit;

			NewBoardEventHandler += OnNewBoard;

			LoopEdgesEventHandler += OnLoopEdgesChange;

			RestartCurrentEventHandler += OnRestartCurrentBoard;

			InitialiseConsole(_gameOfLife.Width, _gameOfLife.Height);

			Thread inputThread = new Thread(new ParameterizedThreadStart(o => Program.ReadInput()));

			inputThread.Start();

			Thread gameThread = new Thread(new ParameterizedThreadStart(o => Program.Process()));

			gameThread.Start();

			Thread renderThread = new Thread(new ParameterizedThreadStart(o => Program.Render()));

			renderThread.Start();

			renderThread.Join();
		}

		private static void OnRestartCurrentBoard(object sender, EventArgs e)
		{
			Generation = 0;
			_gameOfLife.ResetBoardToSeed();
		}

		private static void OnLoopEdgesChange(object sender, EventArgs e)
		{
			_gameOfLife.LoopEdges = !_gameOfLife.LoopEdges;
		}

		private static bool[,] currentBoard;

		public static void Process()
		{
			while (Running)
			{
				Console.Title = string.Format(TITLE_FORMAT, Generation++);

				currentBoard = _gameOfLife.UpdateBoard();
			}
		}

		public static void Render()
		{
			DrawBoard(_gameOfLife.Seed);

			while (Running)
			{
				DrawBoard(currentBoard);

				//Thread.Sleep(10);
			}
		}

		private static void OnNewBoard(object sender, EventArgs e)
		{
			Generation = 0;
			_gameOfLife = new GameOfLife(_gameOfLife.Width, _gameOfLife.Height);
		}

		private static void OnQuit(object sender, EventArgs e)
		{
			Running = false;
		}

		private static bool[,] LoadSeed(string path)
		{
			string json = File.ReadAllText(path);

			bool[,] seed = Newtonsoft.Json.JsonConvert.DeserializeObject<bool[,]>(json);

			return RotateLeft(seed);
		}

		private static void OnSaveBoard(object sender, EventArgs e)
		{
			bool[,] seedToSave = RotateRight(_gameOfLife.Seed);

			string json = Newtonsoft.Json.JsonConvert.SerializeObject(seedToSave);

			File.WriteAllText($"{DateTime.Now.ToFileTime()}.seed", json);
		}

		public static bool[,] RotateRight(bool[,] matrix)
		{
			int lengthY = matrix.GetLength(0);

			int lengthX = matrix.GetLength(1);

			bool[,] result = new bool[lengthX, lengthY];

			for (int y = 0; y < lengthY; y++)
			{
				for (int x = 0; x < lengthX; x++)
				{
					result[x, y] = matrix[lengthY - 1 - y, x];
				}
			}

			return result;
		}

		public static bool[,] RotateLeft(bool[,] matrix)
		{
			int lengthY = matrix.GetLength(0);

			int lengthX = matrix.GetLength(1);

			bool[,] result = new bool[lengthX, lengthY];

			for (int y = 0; y < lengthY; y++)
			{
				for (int x = 0; x < lengthX; x++)
				{
					result[x, y] = matrix[y, lengthX - 1 - x];
				}
			}

			return result;
		}

		public static void ReadInput()
		{
			for (;;)
			{
				ConsoleKey key = ConsoleKey.NoName;
				while (!Console.KeyAvailable || (key = Console.ReadKey(true).Key) != ConsoleKey.Escape)
				{
					switch (key)
					{
						case ConsoleKey.S:
						{
							Program.SaveEventHandler.Invoke(null, null);
						}
							break;
						case ConsoleKey.Q:
						{
							Program.QuitEventHandler.Invoke(null, null);
						}
							break;
						case ConsoleKey.N:
						{
							Program.NewBoardEventHandler.Invoke(null, null);
						}
							break;
						case ConsoleKey.L:
						{
							Program.LoopEdgesEventHandler.Invoke(null, null);
						}
							break;
						case ConsoleKey.R:
						{
							Program.RestartCurrentEventHandler.Invoke(null, null);
						}
							break;
						default:
						{
						}
							break;
					}

					key = ConsoleKey.NoName;
				}
			}
		}

		private static void InitialiseConsole(int boardWidth, int boardHeight)
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
		private static void DrawBoard(bool[,] board)
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