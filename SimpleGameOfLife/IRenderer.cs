namespace SimpleGameOfLife
{
	public interface IRenderer
	{
		void Initialise(int boardWidth, int boardHeight);
		void Render(bool[,] board);
	}
}