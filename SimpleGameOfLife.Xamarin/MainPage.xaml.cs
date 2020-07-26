using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameOfLife.Core;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SimpleGameOfLife.XF
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		private readonly SkiaRenderer Renderer = new SkiaRenderer();

		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		public MainPage()
		{
			InitializeComponent();
		}

		public MainPageViewModel ViewModel
		{
			get
			{
				if (this.BindingContext is null)
				{
					this.BindingContext = new MainPageViewModel();
				}

				return (MainPageViewModel) this.BindingContext;
			}
		}

		private void SkiaView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			SKCanvas canvas = e.Surface.Canvas;

			// get the screen density for scaling
			float scale = (float)(e.Info.Width / this.skiaView.Width);
			
			this.ViewModel.RenderQueue.TryDequeue(out bool[,] board);

			if (board != null)
			{
				this.Renderer.Initialise(canvas, scale);

				this.Renderer.Render(board);
			}
		}


		private void SkiaView_OnSizeChanged(object sender, EventArgs e)
		{
			this._cancellationTokenSource?.Cancel();

			this._cancellationTokenSource = new CancellationTokenSource();
			
			this.ViewModel.CreateNewGame((int)this.skiaView.Width, (int)this.skiaView.Height, this._cancellationTokenSource.Token);

			this.Renderer.Initialise(this.ViewModel._gameOfLife.Width, this.ViewModel._gameOfLife.Height);

			Thread gameThread = new Thread(() => this.ViewModel.Process(this._cancellationTokenSource.Token));

			gameThread.Start();

			Thread renderThread = new Thread(() => this.Render(this._cancellationTokenSource.Token));

			renderThread.Start();

			//renderThread.Join();
		}

		private void Render(CancellationToken cancellationToken)
		{
			for (;;)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				Device.InvokeOnMainThreadAsync(this.skiaView.InvalidateSurface);
			}
		}
	}

	internal class SkiaRenderer : BaseRenderer
	{

		public SKCanvas Canvas { get; internal set; }

		private SKPaint Paint;

		public void Initialise(SKCanvas canvas, float scale)
		{
			this.Canvas = canvas;
			
			// handle the device screen density
			canvas.Scale(scale);

			canvas.Clear(SKColors.White);

			Paint = new SKPaint
			{
				Color = SKColors.Black,
				IsAntialias = false,
				Style = SKPaintStyle.Fill
			};

		}

		/// <summary>
		/// Renders the board to the console
		/// </summary>
		/// <param name="board"></param>
		public override void Render(bool[,] board)
		{
			var boardHeight = board.GetLength(0) - 1;
			var boardWidth = board.GetLength(1) - 1;
			for (int y = 0; y < boardWidth; y++)
			{
				for (int x = 0; x < boardHeight; x++)
				{
					if (board[x, y])
					{
						this.Canvas.DrawPoint(x, y, this.Paint);
					}
				}
			}
		}
	}

	public class MainPageViewModel
	{
		public readonly ConcurrentQueue<bool[,]> RenderQueue = new ConcurrentQueue<bool[,]>();

		public GameOfLife.Core.GameOfLife _gameOfLife { get; private set; }

		private readonly GameOfLife.Core.GameOfLifeSerializer _serializer = new GameOfLifeSerializer();

		public void CreateNewGame(int width, int height, CancellationToken cancellationToken)
		{
			//bool[,] seed = this._serializer.LoadSeed("GosperGliderGun.seed");

			//_gameOfLife = new GameOfLife.Core.GameOfLife(seed);

			this._gameOfLife = new GameOfLife.Core.GameOfLife(width, height);

			//Task.Run(() =>
			//{
			//	Process(cancellationToken);
			//}, cancellationToken);
		}

		internal void Process(CancellationToken cancellationToken)
		{
			for (; ; )
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				if (RenderQueue.Count < 30)
				{
					RenderQueue.Enqueue(_gameOfLife.UpdateBoard());
				}
			}
		}
	}
}
