#pragma once
#include <stdbool.h>
#include <ctype.h>

class StringHelper
{
public:
	static bool isEmptyOrWhiteSpace(const char *source)
	{
		size_t sourceLength = sizeof(source) / sizeof(*source);
		if (sourceLength == 0)
			return true;
		else
		{
			for (size_t index = 0; index < sourceLength; index++)
			{
				if (!isspace(source[index]))
					return false;
			}

			return true;
		}
	}
};
