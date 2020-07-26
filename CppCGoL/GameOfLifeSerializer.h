#pragma once

#include <stdbool.h>
#include "stringhelper.h"
#include "rectangularvectors.h"

namespace SimpleGameOfLife
{
	class GameOfLifeSerializer
	{
	public:
		bool[][] LoadSeed(const char* path);

		void SaveSeed(bool seed[][]);

		bool[][] RotateRight(bool matrix[][]);

		bool[][] RotateLeft(bool matrix[][]);
	};
}
