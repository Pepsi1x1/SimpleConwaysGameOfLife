using System;
using System.IO;

namespace SimpleGameOfLife
{
	public class GameOfLifeSerializer
	{
		public bool[,] LoadSeed(string path)
		{
			string json = File.ReadAllText(path);

			bool[,] seed = Newtonsoft.Json.JsonConvert.DeserializeObject<bool[,]>(json);

			return RotateLeft(seed);
		}

		public void SaveSeed(bool[,] seed)
		{
			bool[,] seedToSave = RotateRight(seed);

			string json = Newtonsoft.Json.JsonConvert.SerializeObject(seedToSave);

			File.WriteAllText($"{DateTime.Now.ToFileTime()}.seed", json);
		}

		public bool[,] RotateRight(bool[,] matrix)
		{
			int lengthY = matrix.GetLength(0);

			int lengthX = matrix.GetLength(1);

			bool[,] result = new bool[lengthX, lengthY];

			for (int y = 0; y < lengthY; y++)
			{
				for (int x = 0; x < lengthX; x++)
				{
					result[x, y] = matrix[lengthY - 1 - y, x];
				}
			}

			return result;
		}

		public bool[,] RotateLeft(bool[,] matrix)
		{
			int lengthY = matrix.GetLength(0);

			int lengthX = matrix.GetLength(1);

			bool[,] result = new bool[lengthX, lengthY];

			for (int y = 0; y < lengthY; y++)
			{
				for (int x = 0; x < lengthX; x++)
				{
					result[x, y] = matrix[y, lengthX - 1 - x];
				}
			}

			return result;
		}
	}
}