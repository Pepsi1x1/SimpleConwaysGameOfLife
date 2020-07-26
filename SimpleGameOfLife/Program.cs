using System;
using System.Collections.Concurrent;
using System.Threading;
using GameOfLife.Core;

namespace SimpleGameOfLife.Console
{
	class Program
	{
		private const string TITLE_FORMAT = "Conway's Game of Life - Processed Generation {0} - Rendering Generation {1}";

		private static GameOfLife.Core.GameOfLife _gameOfLife;

		private static readonly GameOfLifeSerializer Serializer = new GameOfLifeSerializer();

		private static readonly ConsoleInputHandler InputHandler = new ConsoleInputHandler();

		private static readonly IRenderer ConsoleRenderer = new ConsoleRenderer();

		public static EventHandler SaveEventHandler;
		
		public static EventHandler QuitEventHandler;

		public static EventHandler NewBoardEventHandler;

		public static EventHandler LoopEdgesEventHandler;

		public static EventHandler RestartCurrentEventHandler;

		public static bool Running = true;
		
		private static readonly ConcurrentQueue<bool[,]> RenderQueue = new ConcurrentQueue<bool[,]>();

		static void Main(string[] args)
		{
			int maxHeight = System.Console.LargestWindowHeight;

			int maxWidth = System.Console.LargestWindowWidth;

			//GameOfLife = new GameOfLife(maxWidth / 2 - 1, maxHeight - 1);

			//GameOfLife = new GameOfLife(60, 20);

			bool[,] seed = Serializer.LoadSeed("GosperGliderGun.seed");

			_gameOfLife = new GameOfLife.Core.GameOfLife(seed);

			SaveEventHandler += OnSaveBoard;

			QuitEventHandler += OnQuit;

			NewBoardEventHandler += OnNewBoard;

			LoopEdgesEventHandler += OnLoopEdgesChange;

			RestartCurrentEventHandler += OnRestartCurrentBoard;

			ConsoleRenderer.Initialise(_gameOfLife.Width, _gameOfLife.Height);

			Thread inputThread = new Thread((InputHandler.ReadInput));

			inputThread.Start();

			Thread gameThread = new Thread((Program.Process));

			gameThread.Start();

			Thread renderThread = new Thread((Program.Render));

			renderThread.Start();

			renderThread.Join();
		}

		private static void OnRestartCurrentBoard(object sender, EventArgs e)
		{
			_gameOfLife.ResetBoardToSeedState();

			_renderGeneration = 0;
		}

		private static void OnLoopEdgesChange(object sender, EventArgs e)
		{
			_gameOfLife.LoopEdges = !_gameOfLife.LoopEdges;
		}

		public static void Process()
		{
			while (Running)
			{
				if (RenderQueue.Count < 30)
				{
					RenderQueue.Enqueue(_gameOfLife.UpdateBoard());
				}
			}
		}

		private static int _renderGeneration = 0;

		public static void Render()
		{
			ConsoleRenderer.Render(_gameOfLife.Seed);

			while (Running)
			{
				System.Console.Title = string.Format(TITLE_FORMAT, _gameOfLife.Generation, _renderGeneration);

				if (RenderQueue.TryDequeue(out bool[,] currentRender))
				{
					ConsoleRenderer.Render(currentRender);
					_renderGeneration++;
				}

				Thread.Sleep(50);
			}
		}

		private static void OnNewBoard(object sender, EventArgs e)
		{
			_gameOfLife = new GameOfLife.Core.GameOfLife(_gameOfLife.Width, _gameOfLife.Height);
			_renderGeneration = 0;
		}

		private static void OnQuit(object sender, EventArgs e)
		{
			Running = false;
		}
		private static void OnSaveBoard(object sender, EventArgs e)
		{
			Serializer.SaveSeed(_gameOfLife.Seed);
		}
}
}