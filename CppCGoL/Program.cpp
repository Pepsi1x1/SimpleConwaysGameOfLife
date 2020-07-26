#include "Program.h"
#include "GameOfLife.h"
#include "GameOfLifeSerializer.h"
#include "ConsoleInputHandler.h"
#include "IRenderer.h"
#include "ConsoleRenderer.h"

namespace SimpleGameOfLife
{

	const char* Program::TITLE_FORMAT = "Conway's Game of Life - Processed Generation %i - Rendering Generation %i";

	GameOfLife *Program::_gameOfLife;

	GameOfLifeSerializer *Program::_serializer = new GameOfLifeSerializer();

	ConsoleInputHandler *Program::_inputHandler = new ConsoleInputHandler();

	IRenderer *Program::_consoleRenderer = new ConsoleRenderer();

	EventHandler Program::SaveEventHandler;

	EventHandler Program::QuitEventHandler;

	EventHandler Program::NewBoardEventHandler;

	EventHandler Program::LoopEdgesEventHandler;

	EventHandler Program::RestartCurrentEventHandler;

	bool Program::Running = true;
	bool Program::_currentBoard[][];
	ConcurrentQueue<bool[][]> *Program::_renderQueue = new ConcurrentQueue<bool[][]>();
	int Program::_renderGeneration = 0;

	void Program::Main(char **args)
	{
		int maxHeight = Console::LargestWindowHeight;

		int maxWidth = Console::LargestWindowWidth;

		//GameOfLife = new GameOfLife(maxWidth / 2 - 1, maxHeight - 1);

		//GameOfLife = new GameOfLife(60, 20);

		bool seed[][] = _serializer->LoadSeed("GosperGliderGun.seed");

		_gameOfLife = new GameOfLife(seed);

		SaveEventHandler += OnSaveBoard;

		QuitEventHandler += OnQuit;

		NewBoardEventHandler += OnNewBoard;

		LoopEdgesEventHandler += OnLoopEdgesChange;

		RestartCurrentEventHandler += OnRestartCurrentBoard;

		_consoleRenderer->Initialise(_gameOfLife->getWidth(), _gameOfLife->getHeight());

		Thread *inputThread = new Thread(_inputHandler->ReadInput);

		inputThread->Start();

		Thread *gameThread = new Thread(Program::Process);

		gameThread->Start();

		Thread *renderThread = new Thread(Program::Render);

		renderThread->Start();

		renderThread->Join();

		delete renderThread;
		delete gameThread;
		delete inputThread;
	}

	void Program::OnRestartCurrentBoard()
	{
		_gameOfLife->ResetBoardToSeed();
		_renderGeneration = 0;
	}

	void Program::OnLoopEdgesChange()
	{
		_gameOfLife->setLoopEdges(!_gameOfLife->getLoopEdges());
	}

	void Program::Process()
	{
		while (Running)
		{
			if (_renderQueue->Count < 30)
			{
				_renderQueue->Enqueue(_gameOfLife->UpdateBoard());
			}
		}
	}

	void Program::Render()
	{
		_consoleRenderer->Render(_gameOfLife->getSeed());

		while (Running)
		{
			fprintf(stderr, TITLE_FORMAT, _gameOfLife->getGeneration(), _renderGeneration);

			bool currentRender[][];
			if (_renderQueue->TryDequeue(currentRender))
			{
				_consoleRenderer->Render(currentRender);
				_renderGeneration++;
			}

			delay(50);
		}
	}

	void Program::OnNewBoard()
	{
		_gameOfLife = new GameOfLife(_gameOfLife->getWidth(), _gameOfLife->getHeight());
		_renderGeneration = 0;
	}

	void Program::OnQuit()
	{
		Running = false;
	}

	void Program::OnSaveBoard()
	{
		_serializer->SaveSeed(_gameOfLife->getSeed());
	}
}
