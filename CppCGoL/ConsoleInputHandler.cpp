#include "ConsoleInputHandler.h"
#include "Program.h"

namespace SimpleGameOfLife
{

	void ConsoleInputHandler::ReadInput()
	{
		for (;;)
		{
			ConsoleKey key = ConsoleKey::NoName;
			while (!Console::KeyAvailable || (key = Console::ReadKey(true).Key) != ConsoleKey::Escape)
			{
				switch (key)
				{
					case ConsoleKey::S:
					{
						Program::SaveEventHandler->Invoke();
					}
						break;
					case ConsoleKey::Q:
					{
						Program::QuitEventHandler->Invoke();
					}
						break;
					case ConsoleKey::N:
					{
						Program::NewBoardEventHandler->Invoke();
					}
						break;
					case ConsoleKey::L:
					{
						Program::LoopEdgesEventHandler->Invoke();
					}
						break;
					case ConsoleKey::R:
					{
						Program::RestartCurrentEventHandler->Invoke();
					}
						break;
					default:
					{
					}
						break;
				}

				key = ConsoleKey::NoName;
			}
		}
	}
}
