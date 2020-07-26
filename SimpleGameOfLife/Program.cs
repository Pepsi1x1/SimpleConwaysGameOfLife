using System;
using System.Threading;
using Newtonsoft.Json;

namespace SimpleGameOfLife
{
	class Program
	{
		private const string TITLE_FORMAT = "Conway's Game of Life - Generation {0}";

		private static int _generation = 0;

		private static GameOfLife _gameOfLife;

		private static GameOfLifeSerializer _serializer = new GameOfLifeSerializer();

		private static ConsoleInputHandler _inputHandler = new ConsoleInputHandler();

		private static IRenderer _consoleRenderer = new ConsoleRenderer();

		public static EventHandler SaveEventHandler;
		
		public static EventHandler QuitEventHandler;

		public static EventHandler NewBoardEventHandler;

		public static EventHandler LoopEdgesEventHandler;

		public static EventHandler RestartCurrentEventHandler;

		public static bool Running = true;

		private static bool[,] _currentBoard;

		static void Main(string[] args)
		{
			int maxHeight = Console.LargestWindowHeight;

			int maxWidth = Console.LargestWindowWidth;

			//GameOfLife = new GameOfLife(maxWidth / 2 - 1, maxHeight - 1);

			//GameOfLife = new GameOfLife(60, 20);

			bool[,] seed = _serializer.LoadSeed("GosperGliderGun.seed");

			_gameOfLife = new GameOfLife(seed);

			SaveEventHandler += OnSaveBoard;

			QuitEventHandler += OnQuit;

			NewBoardEventHandler += OnNewBoard;

			LoopEdgesEventHandler += OnLoopEdgesChange;

			RestartCurrentEventHandler += OnRestartCurrentBoard;

			_consoleRenderer.Initialise(_gameOfLife.Width, _gameOfLife.Height);

			Thread inputThread = new Thread((_inputHandler.ReadInput));

			inputThread.Start();

			Thread gameThread = new Thread((Program.Process));

			gameThread.Start();

			Thread renderThread = new Thread((Program.Render));

			renderThread.Start();

			renderThread.Join();
		}

		private static void OnRestartCurrentBoard(object sender, EventArgs e)
		{
			_gameOfLife.ResetBoardToSeed();
		}

		private static void OnLoopEdgesChange(object sender, EventArgs e)
		{
			_gameOfLife.LoopEdges = !_gameOfLife.LoopEdges;
		}

		public static void Process()
		{
			while (Running)
			{
				Console.Title = string.Format(TITLE_FORMAT, _gameOfLife.Generation);

				_currentBoard = _gameOfLife.UpdateBoard();
			}
		}

		public static void Render()
		{
			_consoleRenderer.Render(_gameOfLife.Seed);

			while (Running)
			{
				_consoleRenderer.Render(_currentBoard);

				Thread.Sleep(50);
			}
		}

		private static void OnNewBoard(object sender, EventArgs e)
		{
			_gameOfLife = new GameOfLife(_gameOfLife.Width, _gameOfLife.Height);
		}

		private static void OnQuit(object sender, EventArgs e)
		{
			Running = false;
		}
		private static void OnSaveBoard(object sender, EventArgs e)
		{
			_serializer.SaveSeed(_gameOfLife.Seed);
		}
}
}