#pragma once

#include <vector>

namespace SimpleGameOfLife
{
	class IRenderer
	{
	public:
		virtual void Initialise(int boardWidth, int boardHeight) = 0;
		virtual void Render(bool board[][]) = 0;
	};
}
