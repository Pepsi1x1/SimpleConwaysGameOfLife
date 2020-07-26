#pragma once

#include <stdbool.h>
#include "rectangularvectors.h"

namespace SimpleGameOfLife
{
	class GameOfLife
	{
	private:
		int Width = 60;
		int Height = 20;
		bool Seed[][];
		bool LoopEdges = false;
		int Generation = 0;

		bool _board[][];

	public:
		virtual ~GameOfLife()
		{
			delete mutationRandom;
		}

		int getWidth() const;
		void setWidth(int value);

		int getHeight() const;
		void setHeight(int value);

		bool[][] getSeed() const;
		void setSeed(const bool value[][]);

		bool getLoopEdges() const;
		void setLoopEdges(bool value);

		int getGeneration() const;
		void setGeneration(int value);

		/// <summary>
		/// Sets up a board with a random initial seed
		/// </summary>
		/// <returns></returns>
	private:
		bool[][] InitialiseRandomBoard();

	public:
		GameOfLife();

		GameOfLife(int width, int height);

		GameOfLife(bool seed[][]);

		/// <summary>
		/// Runs the rule set and generates the next universe
		/// </summary>
		/// <returns></returns>
		bool[][] UpdateBoard();

		/// <summary>
		/// A live cell dies unless it has exactly 2 or 3 live neighbours.
		/// </summary>
		/// <param name="liveNeighbourCount"></param>
		/// <returns></returns>
	private:
		bool MayContinueToLiveCondition(int liveNeighbourCount);

#if defined(MUTATE)
		Random *mutationRandom = new Random();
#endif

		/// <summary>
		/// A dead cell remains dead unless it has exactly 3 live neighbours.
		/// </summary>
		/// <param name="liveNeighbourCount"></param>
		/// <returns></returns>
		bool WillBeBornCondition(int liveNeighbourCount);

		/// <summary>
		/// Slightly less brute
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="bailThreshold"></param>
		/// <returns>Number of live neighbours around the cell at position (x,y).</returns>
		int CountLiveNeighboursBail(int x, int y, int bailThreshold);

		/// <summary>
		/// Brute
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>Number of live neighbours around the cell at position (x,y).</returns>
		int CountLiveNeighbours(int x, int y);

	public:
		void ResetBoardToSeed();
	};
}
