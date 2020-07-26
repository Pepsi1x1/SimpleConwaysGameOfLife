#include <stdio.h>
#include <time.h>
#include "GameOfLifeSerializer.h"

namespace SimpleGameOfLife
{

	bool[][] GameOfLifeSerializer::LoadSeed(const std::wstring &path)
	{
		std::wstring json = File::ReadAllText(path);

		bool[][] seed = Newtonsoft::Json::JsonConvert::DeserializeObject<bool[][]>(json);

		return RotateLeft(seed);
	}

	void GameOfLifeSerializer::SaveSeed(bool[][] &seed)
	{
		bool[][] seedToSave = RotateRight(seed);

		std::wstring json = Newtonsoft::Json::JsonConvert::SerializeObject(seedToSave);

		char* filename = "%ld.seed";
		sprintf(filename, time(NULL));
		File::WriteAllText(filename, json);
	}

	bool[][] GameOfLifeSerializer::RotateRight(bool[][] &matrix)
	{
		int lengthY = matrix.size();

		int lengthX = (matrix.size() == 0 ? 0 : matrix[0].size());

		bool[][] result = RectangularVectors::RectangularBoolVector(lengthX, lengthY);

		for (int y = 0; y < lengthY; y++)
		{
			for (int x = 0; x < lengthX; x++)
			{
				result[x][y] = matrix[lengthY - 1 - y][x];
			}
		}

		return result;
	}

	bool[][] GameOfLifeSerializer::RotateLeft(bool[][] &matrix)
	{
		int lengthY = matrix.size();

		int lengthX = (matrix.size() == 0 ? 0 : matrix[0].size());

		bool[][] result = RectangularVectors::RectangularBoolVector(lengthX, lengthY);

		for (int y = 0; y < lengthY; y++)
		{
			for (int x = 0; x < lengthX; x++)
			{
				result[x][y] = matrix[y][lengthX - 1 - x];
			}
		}

		return result;
	}
}
