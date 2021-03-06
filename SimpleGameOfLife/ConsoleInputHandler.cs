﻿using System;

namespace SimpleGameOfLife.Console
{
	public class ConsoleInputHandler
	{
		public void ReadInput()
		{
			for (;;)
			{
				ConsoleKey key = ConsoleKey.NoName;
				while (!System.Console.KeyAvailable || (key = System.Console.ReadKey(true).Key) != ConsoleKey.Escape)
				{
					switch (key)
					{
						case ConsoleKey.S:
						{
							Program.SaveEventHandler.Invoke(null, null);
						}
							break;
						case ConsoleKey.Q:
						{
							Program.QuitEventHandler.Invoke(null, null);
						}
							break;
						case ConsoleKey.N:
						{
							Program.NewBoardEventHandler.Invoke(null, null);
						}
							break;
						case ConsoleKey.L:
						{
							Program.LoopEdgesEventHandler.Invoke(null, null);
						}
							break;
						case ConsoleKey.R:
						{
							Program.RestartCurrentEventHandler.Invoke(null, null);
						}
							break;
						default:
						{
						}
							break;
					}

					key = ConsoleKey.NoName;
				}
			}
		}
	}
}