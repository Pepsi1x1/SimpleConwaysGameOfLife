#pragma once

#include "IRenderer.h"
#include <stdio.h>

#include <iostream>

namespace SimpleGameOfLife
{
	class ConsoleRenderer : public IRenderer
	{
	private:
		static constexpr ConsoleColor DEAD_COLOUR = ConsoleColor::White;

		static constexpr ConsoleColor LIVE_COLOUR = ConsoleColor::Black;

	public:
		static const char* EMPTY_BLOCK_CHAR;

		static const char* FULL_BLOCK_CHAR;

		void Initialise(int boardWidth, int boardHeight) override;

		/// <summary>
		/// Renders the board to the console
		/// </summary>
		/// <param name="board"></param>
		void Render(bool board[][]) override;
	};
}
