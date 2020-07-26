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

			InitializeConsole(_gameOfLife.Width, _gameOfLife.Height);

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

		private static void InitializeConsole(int boardWidth, int boardHeight)
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

	public class GameOfLife
	{
		private bool[,] _board;

		public int Width { get; set; } = 60;

		public int Height { get; set; } = 20;

		public bool[,] Seed { get; set; }

		public bool LoopEdges { get; set; }

		/// <summary>
		/// Sets up a board with a random initial seed
		/// </summary>
		/// <returns></returns>
		private bool[,] InitialiseRandomBoard()
		{
			Random random = new Random();

			bool[,] board = new bool[Width, Height];
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					board[x, y] = random.Next(2) == 0;
				}
			}

			return board;
		}

		public GameOfLife()
		{
			this._board = this.InitialiseRandomBoard();
		}

		public GameOfLife(int width, int height)
		{
			this.Width = width;

			this.Height = height;

			this._board = this.InitialiseRandomBoard();

			this.Seed = this._board;
		}

		public GameOfLife(bool[,] seed)
		{
			this.Seed = seed;

			this._board = seed;

			this.Width = seed.GetLength(0);

			this.Height = seed.GetLength(1);
		}

		/// <summary>
		/// Runs the rule set and generates the next universe
		/// </summary>
		/// <returns></returns>
		public bool[,] UpdateBoard()
		{
			bool[,] newBoard = new bool[Width, Height];

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					int liveNeighbourCount = this.CountLiveNeighboursBail(x, y, 4);

					bool currentCellWasAlive = _board[x, y];

					bool liveCellRemainsAlive = currentCellWasAlive && this.MayContinueToLiveCondition(liveNeighbourCount);

					bool deadCellNowLives = !currentCellWasAlive && this.WillBeBornCondition(liveNeighbourCount);

					newBoard[x, y] = liveCellRemainsAlive || deadCellNowLives;
				}
			}

			_board = newBoard;

			return newBoard;
		}

		/// <summary>
		/// A live cell dies unless it has exactly 2 or 3 live neighbours.
		/// </summary>
		/// <param name="liveNeighbourCount"></param>
		/// <returns></returns>
		private bool MayContinueToLiveCondition(int liveNeighbourCount)
		{
			return liveNeighbourCount == 2 || liveNeighbourCount == 3;
		}

#if MUTATE
        Random mutationRandom = new Random();
#endif

		/// <summary>
		/// A dead cell remains dead unless it has exactly 3 live neighbours.
		/// </summary>
		/// <param name="liveNeighbourCount"></param>
		/// <returns></returns>
		private bool WillBeBornCondition(int liveNeighbourCount)
		{
			int condition = 3;
#if MUTATE
            if (Program.Generation % 1000 == 0)
            {
	            var randNext = mutationRandom.Next(0, 10);
	            if (randNext == 4)
	            {
		            condition = 2;
	            }

            }
#endif
			return liveNeighbourCount == condition;
		}

		/// <summary>
		/// Slightly less brute
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="bailThreshold"></param>
		/// <returns>Number of live neighbours around the cell at position (x,y).</returns>
		private int CountLiveNeighboursBail(int x, int y, int bailThreshold)
		{
			// The number of live neighbours.
			// Subtract 1 if (x,y) is alive since we would count it as a neighbour.
			int value = _board[x, y] ? -1 : 0;

			// This nested loop enumerates the 9 cells in the given cells neighbourhood.
			for (int j = -1; j <= 1; j++)
			{
				// If loopEdges is set to false and y+j is off the board, continue.
				if (!LoopEdges && y + j < 0 || y + j >= Height)
				{
					continue;
				}

				// Loop around the edges if y+j is off the board.
				int k = (y + j + Height) % Height;

				for (int i = -1; i <= 1; i++)
				{
					// If loopEdges is set to false and x+i is off the board, continue.
					if (!LoopEdges && x + i < 0 || x + i >= Width)
					{
						continue;
					}

					// Loop around the edges if x+i is off the board.
					int h = (x + i + Width) % Width;

					// Count the neighbour cell at (h,k) if it is alive.
					value += _board[h, k] ? 1 : 0;

					if (value == bailThreshold)
						return value;
				}
			}


			return value;
		}

		/// <summary>
		/// Brute
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>Number of live neighbours around the cell at position (x,y).</returns>
		private int CountLiveNeighbours(int x, int y)
		{
			// The number of live neighbours.
			int value = 0;

			// This nested loop enumerates the 9 cells in the given cells neighbourhood.
			for (int j = -1; j <= 1; j++)
			{
				// If loopEdges is set to false and y+j is off the board, continue.
				if (!LoopEdges && y + j < 0 || y + j >= Height)
				{
					continue;
				}

				// Loop around the edges if y+j is off the board.
				int k = (y + j + Height) % Height;

				for (int i = -1; i <= 1; i++)
				{
					// If loopEdges is set to false and x+i is off the board, continue.
					if (!LoopEdges && x + i < 0 || x + i >= Width)
					{
						continue;
					}

					// Loop around the edges if x+i is off the board.
					int h = (x + i + Width) % Width;

					// Count the neighbour cell at (h,k) if it is alive.
					value += _board[h, k] ? 1 : 0;
				}
			}

			// Subtract 1 if (x,y) is alive since we counted it as a neighbour.
			return value - (_board[x, y] ? 1 : 0);
		}

		internal void ResetBoardToSeed()
		{
			this._board = this.Seed;
		}
	}
}