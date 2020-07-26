namespace GameOfLife.Core
{
	public abstract class BaseRenderer : IRenderer
	{
		protected int BoardWidth = 0;

		protected int BoardHeight = 0;
		public virtual void Initialise(int boardWidth, int boardHeight)
		{
			this.BoardWidth = boardWidth;

			this.BoardHeight = boardHeight;
		}

		public virtual void Render(bool[,] board)
		{
		}
	}
}