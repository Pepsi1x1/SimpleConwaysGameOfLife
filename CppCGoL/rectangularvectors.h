#pragma once

#include <stdbool.h>

class RectangularVectors
{
public:
    static bool[][] RectangularBoolVector(int size1, int size2)
    {
        bool newVector[size1][size2]
        for (int i = 0; i < size1; i++)
        {
            for (int j = 0; j < size2; ++j)
            {
                newVector[i][j] = false;
            }
        }

        return newVector;
    }
};
