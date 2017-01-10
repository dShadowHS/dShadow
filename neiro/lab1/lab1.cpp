#include <iostream>
#include <fstream>
#include <stdlib.h>
#include <time.h>
#include <cmath> 


using namespace std;
double raz[250][6];
double neraz[250][6];
double one, two, three, four, five;
double checkone, checktwo, checkthree, checkfour;

void gen1()
{
    for (int i = 0; i < 250; i++)
    {
        if (i <= 49)
        {
            checkone = 0;
            checktwo = 0;
            checkthree = 0;
            checkfour = 0;
        }
        if ((i > 49) && (i <= 99))
        {
            checkone = 1;
            checktwo = 0;
            checkthree = 0;
            checkfour = 0;
        }
        if ((i > 99) && (i <= 149))
        {
            checkone = 0;
            checktwo = 1;
            checkthree = 0;
            checkfour = 0;
        }
        if ((i > 149) && (i <= 200))
        {
            checkone = 0;
            checktwo = 0;
            checkthree = 1;
            checkfour = 0;
        }
        if (i > 199)
        {
            checkone = 0;
            checktwo = 0;
            checkthree = 0;
            checkfour = 1;
        }
        do
        {
            one = (rand() % 2001)*0.1 - 100*checkone;
            two = (rand() % 2001)*0.1 - 100*checktwo;
            three = (rand() % 2001)*0.1 - 100*checkthree;
            four = (rand() % 2001)*0.1 - 100*checkfour;
            five = (rand() % 2001)*0.1 - 100;
        } while (sqrt(pow(one, 2) + pow(two, 2) + pow(three, 2) + pow(four, 2) + pow(five, 2))>=100);

        raz[i][1] = 201 * checkone + one;
        raz[i][2] = 201 * checktwo + two;
        raz[i][3] = 201 * checkthree + three;
        raz[i][4] = 201 * checkfour + four;
        raz[i][5] = five;
    }
}

void gen2()
{
    for (int i = 0; i < 250; i++)
    {
        if (i <= 49)
        {
            checkone = 0;
            checktwo = 0;
            checkthree = 0;
            checkfour = 0;
        }
        if ((i > 49) && (i <= 99))
        {
            checkone = 1;
            checktwo = 0;
            checkthree = 0;
            checkfour = 0;
        }
        if ((i > 99) && (i <= 149))
        {
            checkone = 0;
            checktwo = 1;
            checkthree = 0;
            checkfour = 0;
        }
        if ((i > 149) && (i <= 200))
        {
            checkone = 0;
            checktwo = 0;
            checkthree = 0.3;
            checkfour = 0;
        }
        if (i > 199)
        {
            checkone = 0;
            checktwo = 0;
            checkthree = 0;
            checkfour = 0.6;
        }
        do
        {
            one = (rand() % 2001)*0.1 - 100 * checkone;
            two = (rand() % 2001)*0.1 - 100 * checktwo;
            three = (rand() % 2001)*0.1 - 100 * checkthree;
            four = (rand() % 2001)*0.1 - 100 * checkfour;
            five = (rand() % 2001)*0.1 - 100;
        } while (sqrt(pow(one, 2) + pow(two, 2) + pow(three, 2) + pow(four, 2) + pow(five, 2))>=100);

        neraz[i][1] = 205 * checkone + one;
        neraz[i][2] = 205 * checktwo + two;
        neraz[i][3] = 205 * checkthree + three;
        neraz[i][4] = 205 * checkfour + four;
        neraz[i][5] = five;
    }
}

void main()
{
    srand((unsigned)time(NULL));
    gen1();
    gen2();

    ofstream f;
    f.open("C:\\C\\raz.txt");

    for (int i = 0; i < 250; i++)
    {
        f << raz[i][1];
        f << "|";
        f << raz[i][2];
        f << "|";
        f << raz[i][3];
        f << "|";
        f << raz[i][4];
        f << "|";
        f << raz[i][5];
        f << "|";
        f << (i / 50);
        f << '\n';
    }
    f.close();

    f.open("C:\\C\\neraz.txt");
    for (int i = 0; i < 250; i++)
    {
        f << neraz[i][1];
        f << "|";
        f << neraz[i][2];
        f << "|";
        f << neraz[i][3];
        f << "|";
        f << neraz[i][4];
        f << "|";
        f << neraz[i][5];
        f << "|";
        f << (i / 50);
        f << '\n';
    }
    f.close();
}