using System;

namespace GameOfLife.Core
{
	public class GameOfLife
	{
		private bool[,] _board;

		public int Width { get; set; } = 60;

		public int Height { get; set; } = 20;

		public bool[,] Seed { get; set; }

		public bool LoopEdges { get; set; }

		public int Generation { get; private set; }

		/// <summary>
		/// Sets up a board with a random initial seed
		/// </summary>
		/// <returns></returns>
		private bool[,] InitialiseRandomBoard()
		{
			Random random = new Random();

			bool[,] board = new bool[this.Width, this.Height];
			for (int y = 0; y < this.Height; y++)
			{
				for (int x = 0; x < this.Width; x++)
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

			this.Generation = 0;
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
			bool[,] newBoard = new bool[this.Width, this.Height];

			for (int y = 0; y < this.Height; y++)
			{
				for (int x = 0; x < this.Width; x++)
				{
					int liveNeighbourCount = this.CountLiveNeighboursBail(x, y, 4);

					bool currentCellWasAlive = this._board[x, y];

					bool liveCellRemainsAlive = currentCellWasAlive && this.MayContinueToLiveCondition(liveNeighbourCount);

					bool deadCellNowLives = !currentCellWasAlive && this.WillBeBornCondition(liveNeighbourCount);

					newBoard[x, y] = liveCellRemainsAlive || deadCellNowLives;
				}
			}

			this._board = newBoard;

			this.Generation++;

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
			int value = this._board[x, y] ? -1 : 0;

			// This nested loop enumerates the 9 cells in the given cells neighbourhood.
			for (int j = -1; j <= 1; j++)
			{
				// If loopEdges is set to false and y+j is off the board, continue.
				if (!this.LoopEdges && y + j < 0 || y + j >= this.Height)
				{
					continue;
				}

				// Loop around the edges if y+j is off the board.
				int k = (y + j + this.Height) % this.Height;

				for (int i = -1; i <= 1; i++)
				{
					// If loopEdges is set to false and x+i is off the board, continue.
					if (!this.LoopEdges && x + i < 0 || x + i >= this.Width)
					{
						continue;
					}

					// Loop around the edges if x+i is off the board.
					int h = (x + i + this.Width) % this.Width;

					// Count the neighbour cell at (h,k) if it is alive.
					value += this._board[h, k] ? 1 : 0;

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
				if (!this.LoopEdges && y + j < 0 || y + j >= this.Height)
				{
					continue;
				}

				// Loop around the edges if y+j is off the board.
				int k = (y + j + this.Height) % this.Height;

				for (int i = -1; i <= 1; i++)
				{
					// If loopEdges is set to false and x+i is off the board, continue.
					if (!this.LoopEdges && x + i < 0 || x + i >= this.Width)
					{
						continue;
					}

					// Loop around the edges if x+i is off the board.
					int h = (x + i + this.Width) % this.Width;

					// Count the neighbour cell at (h,k) if it is alive.
					value += this._board[h, k] ? 1 : 0;
				}
			}

			// Subtract 1 if (x,y) is alive since we counted it as a neighbour.
			return value - (this._board[x, y] ? 1 : 0);
		}

		public void ResetBoardToSeedState()
		{
			this._board = this.Seed;
			this.Generation = 0;
		}
	}
}