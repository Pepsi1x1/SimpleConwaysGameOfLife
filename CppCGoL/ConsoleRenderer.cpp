#include "ConsoleRenderer.h"

namespace SimpleGameOfLife
{

	const char* ConsoleRenderer::EMPTY_BLOCK_CHAR = "  ";

	const char* ConsoleRenderer::FULL_BLOCK_CHAR = "\u2588\u2588";

	void ConsoleRenderer::Initialise(int boardWidth, int boardHeight)
	{
		Console::BackgroundColor = ConsoleColor::Gray;
		Console::Clear();

		Console::CursorVisible = false;

		// Each cell is two characters wide and append an extra row on the bottom
		// just to prevent scrolling messing with the render of the board.
		int width = std::max(boardWidth, 8) * 2 + 1;
		int height = std::max(boardHeight, 8) + 1;

		Console::SetWindowSize(width, height);
		Console::SetBufferSize(width, height);

		Console::BackgroundColor = DEAD_COLOUR;
		Console::ForegroundColor = LIVE_COLOUR;
	}

	void ConsoleRenderer::Render(bool board[][])
	{
		int width = board.size();
		int height = (board.size() == 0 ? 0 : board[0].size());

		size_t buidler_len = width * height + 1; /* + 1 for terminating NULL */
    	char *builder = (char*) malloc(buidler_len);

		Console::SetCursorPosition(0, 0);
		//Console.ForegroundColor = (ConsoleColor)(Generation % 14);

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				char* c = board[x][y] ? FULL_BLOCK_CHAR : EMPTY_BLOCK_CHAR;

				strcat(builder, c);
			}

			strcat(builder, '\n');
		}

		fprintf( stdout, builder );
	}
}
