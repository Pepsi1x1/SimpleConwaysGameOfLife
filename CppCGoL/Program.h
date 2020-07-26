#pragma once
#include <stdbool.h>

//Forward class declarations:
namespace SimpleGameOfLife { class GameOfLife; }
namespace SimpleGameOfLife { class GameOfLifeSerializer; }
namespace SimpleGameOfLife { class ConsoleInputHandler; }
namespace SimpleGameOfLife { class IRenderer; }

namespace SimpleGameOfLife
{
	class Program
	{
	public:
		static EventHandler SaveEventHandler;

		static EventHandler QuitEventHandler;

		static EventHandler NewBoardEventHandler;

		static EventHandler LoopEdgesEventHandler;

		static EventHandler RestartCurrentEventHandler;

		static bool Running;

	private:
		static const char* TITLE_FORMAT;

		static GameOfLife *_gameOfLife;

		static GameOfLifeSerializer *_serializer;

		static ConsoleInputHandler *_inputHandler;

		static IRenderer *_consoleRenderer;

		static bool _currentBoard[][];

		static ConcurrentQueue<bool[][]> *_renderQueue;

		static int _renderGeneration;

	public:
		static void Main(char **args);

		static void Process();

		static void Render();

	private:
		static void OnRestartCurrentBoard();

		static void OnLoopEdgesChange();		

		static void OnNewBoard();

		static void OnQuit();
		
		static void OnSaveBoard();
	};
}
