#include "GameOfLife.h"
#include "Program.h"

namespace SimpleGameOfLife
{

	int GameOfLife::getWidth() const
	{
		return Width;
	}

	void GameOfLife::setWidth(int value)
	{
		Width = value;
	}

	int GameOfLife::getHeight() const
	{
		return Height;
	}

	void GameOfLife::setHeight(int value)
	{
		Height = value;
	}

	bool[][] GameOfLife::getSeed() const
	{
		return Seed;
	}

	void GameOfLife::setSeed(const bool value[][])
	{
		Seed = value;
	}

	bool GameOfLife::getLoopEdges() const
	{
		return LoopEdges;
	}

	void GameOfLife::setLoopEdges(bool value)
	{
		LoopEdges = value;
	}

	int GameOfLife::getGeneration() const
	{
		return Generation;
	}

	void GameOfLife::setGeneration(int value)
	{
		Generation = value;
	}

	bool[][] GameOfLife::InitialiseRandomBoard()
	{
		Random *random = new Random();

		bool board[][] = RectangularVectors::RectangularBoolVector(this->getWidth(), this->getHeight());
		for (int y = 0; y < this->getHeight(); y++)
		{
			for (int x = 0; x < this->getWidth(); x++)
			{
				board[x][y] = random->Next(2) == 0;
			}
		}

		delete random;
		return board;
	}

	GameOfLife::GameOfLife()
	{
		this->_board = this->InitialiseRandomBoard();
	}

	GameOfLife::GameOfLife(int width, int height)
	{
		this->setWidth(width);

		this->setHeight(height);

		this->_board = this->InitialiseRandomBoard();

		this->setSeed(this->_board);

		this->setGeneration(0);
	}

	GameOfLife::GameOfLife(bool seed[][])
	{
		this->setSeed(seed);

		this->_board = seed;

		this->setWidth(seed.size());

		this->setHeight((seed.size() == 0 ? 0 : seed[0].size()));
	}

	bool[][] GameOfLife::UpdateBoard()
	{
		bool newBoard[][] = RectangularVectors::RectangularBoolVector(this->getWidth(), this->getHeight());

		for (int y = 0; y < this->getHeight(); y++)
		{
			for (int x = 0; x < this->getWidth(); x++)
			{
				int liveNeighbourCount = this->CountLiveNeighboursBail(x, y, 4);

				bool currentCellWasAlive = this->_board[x][y];

				bool liveCellRemainsAlive = currentCellWasAlive && this->MayContinueToLiveCondition(liveNeighbourCount);

				bool deadCellNowLives = !currentCellWasAlive && this->WillBeBornCondition(liveNeighbourCount);

				newBoard[x][y] = liveCellRemainsAlive || deadCellNowLives;
			}
		}

		this->_board = newBoard;

		this->setGeneration(this->getGeneration() + 1);

		return newBoard;
	}

	bool GameOfLife::MayContinueToLiveCondition(int liveNeighbourCount)
	{
		return liveNeighbourCount == 2 || liveNeighbourCount == 3;
	}

	bool GameOfLife::WillBeBornCondition(int liveNeighbourCount)
	{
		int condition = 3;
	#if defined(MUTATE)
		if (this->getGeneration() % 1000 == 0)
		{
			auto randNext = mutationRandom->Next(0, 10);
			if (randNext == 4)
			{
				condition = 2;
			}

		}
	#endif
		return liveNeighbourCount == condition;
	}

	int GameOfLife::CountLiveNeighboursBail(int x, int y, int bailThreshold)
	{
		// The number of live neighbours.
		// Subtract 1 if (x,y) is alive since we would count it as a neighbour.
		int value = this->_board[x][y] ? -1 : 0;

		// This nested loop enumerates the 9 cells in the given cells neighbourhood.
		for (int j = -1; j <= 1; j++)
		{
			// If loopEdges is set to false and y+j is off the board, continue.
			if (!this->getLoopEdges() && y + j < 0 || y + j >= this->getHeight())
			{
				continue;
			}

			// Loop around the edges if y+j is off the board.
			int k = (y + j + this->getHeight()) % this->getHeight();

			for (int i = -1; i <= 1; i++)
			{
				// If loopEdges is set to false and x+i is off the board, continue.
				if (!this->getLoopEdges() && x + i < 0 || x + i >= this->getWidth())
				{
					continue;
				}

				// Loop around the edges if x+i is off the board.
				int h = (x + i + this->getWidth()) % this->getWidth();

				// Count the neighbour cell at (h,k) if it is alive.
				value += this->_board[h][k] ? 1 : 0;

				if (value == bailThreshold)
				{
					return value;
				}
			}
		}


		return value;
	}

	int GameOfLife::CountLiveNeighbours(int x, int y)
	{
		// The number of live neighbours.
		int value = 0;

		// This nested loop enumerates the 9 cells in the given cells neighbourhood.
		for (int j = -1; j <= 1; j++)
		{
			// If loopEdges is set to false and y+j is off the board, continue.
			if (!this->getLoopEdges() && y + j < 0 || y + j >= this->getHeight())
			{
				continue;
			}

			// Loop around the edges if y+j is off the board.
			int k = (y + j + this->getHeight()) % this->getHeight();

			for (int i = -1; i <= 1; i++)
			{
				// If loopEdges is set to false and x+i is off the board, continue.
				if (!this->getLoopEdges() && x + i < 0 || x + i >= this->getWidth())
				{
					continue;
				}

				// Loop around the edges if x+i is off the board.
				int h = (x + i + this->getWidth()) % this->getWidth();

				// Count the neighbour cell at (h,k) if it is alive.
				value += this->_board[h][k] ? 1 : 0;
			}
		}

		// Subtract 1 if (x,y) is alive since we counted it as a neighbour.
		return value - (this->_board[x][y] ? 1 : 0);
	}

	void GameOfLife::ResetBoardToSeed()
	{
		this->_board = this->getSeed();
		this->setGeneration(0);
	}
}
