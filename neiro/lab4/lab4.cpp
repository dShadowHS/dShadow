#include "stdio.h"
#include "conio.h"
#include "math.h"
#include "iostream"
#include "fstream"
#include <ctime>
#include <cstdlib>
#include <random>

using std::time;
using std::rand;
using std::default_random_engine;
using std::uniform_real_distribution;
using namespace std;

int const N = 10;	// max число столбцов исходной матрицы
int const M = 250;	// max число строк

double  Z[M][N];	// Исходная матрица
double  mc[N];		// массив средних по столбцам
double  dc[N];		// массив дисперсий по столбцам
double  X[M][N];	// стандартизованная матрица
double  S[N][N];	// ковариационная матрица
double  A[N][N];	// корреляционная матрица

int    n;
int    m;			// заданное число столбцов
int    i, j;

double Ip;
double TrA;
int p;
int sortInd[N];	    //массив индексов собственных значений
int sortPoints[N - 1];
int sortParts[N];
double stickParts[N];
double brakePoints[N - 1];
double eigenVals[N];		    // массив собственных значений (вектор)
double eigenVects[N][N];		// массив собственных векторов (матрица)
double MN[N][N];	            // матрица нагрузок
double PC[N][N];	            // матрица главных компонент

                                //    Входные данные:
                                //        n - размерность матрицы
                                //        a - исходная матрица. В процессе работы наддиагональные элементы
                                //            будут изменены, но их легко восстановить по поддиагональным
                                //    Выходные данные:
                                //        d - массив собственных значений
                                //        v - массив собственных векторов

void jacobi(const int n, double a[][N], double d[N], double v[][N])
{
    if (n == 0) return;
    double * b = new double[n + n];
    double * z = b + n;
    unsigned int i, k;
    for (i = 0; i < n; ++i)
    {
        z[i] = 0.;
        b[i] = d[i] = a[i][i];
        for (k = 0; k < n; ++k) v[i][k] = i == k ? 1. : 0.;
    }
    for (i = 0; i < 50; ++i)
    {
        double sm = 0.;
        unsigned int p, q;
        for (p = 0; p < n - 1; ++p)
        {
            for (q = p + 1; q < n; ++q) sm += fabs(a[p][q]);
        }
        if (sm == 0) break;
        const double alfa = i < 3 ? 0.2 * sm / (n*n) : 0.;
        for (p = 0; p < n - 1; ++p)
        {
            for (q = p + 1; q < n; ++q)
            {
                const double g = 1e12 * fabs(a[p][q]);
                if (i >= 3 && fabs(d[p]) > g && fabs(d[q]) > g) a[p][q] = 0.;
                else
                    if (fabs(a[p][q]) > alfa)
                    {
                        const double theta = 0.5 * (d[q] - d[p]) / a[p][q];
                        double t = 1. / (fabs(theta) + sqrt(1. + theta*theta));
                        if (theta < 0) t = -t;
                        const double c = 1. / sqrt(1. + t*t);
                        const double s = t * c;
                        const double tau = s / (1. + c);
                        const double h = t * a[p][q];
                        z[p] -= h;
                        z[q] += h;
                        d[p] -= h;
                        d[q] += h;
                        a[p][q] = 0.;
                        for (k = 0; k < p; ++k)
                        {
                            const double g = a[k][p];
                            const double h = a[k][q];
                            a[k][p] = g - s * (h + g * tau);
                            a[k][q] = h + s * (g - h * tau);
                        }
                        for (k = p + 1; k < q; ++k)
                        {
                            const double g = a[p][k];
                            const double h = a[k][q];
                            a[p][k] = g - s * (h + g * tau);
                            a[k][q] = h + s * (g - h * tau);
                        }
                        for (k = q + 1; k < n; ++k)
                        {
                            const double g = a[p][k];
                            const double h = a[q][k];
                            a[p][k] = g - s * (h + g * tau);
                            a[q][k] = h + s * (g - h * tau);
                        }
                        for (k = 0; k < n; ++k)
                        {
                            const double g = v[k][p];
                            const double h = v[k][q];
                            v[k][p] = g - s * (h + g * tau);
                            v[k][q] = h + s * (g - h * tau);
                        }
                    }
            }
        }
        for (p = 0; p < n; ++p)
        {
            d[p] = (b[p] += z[p]);
            z[p] = 0.;
        }
    }
}

void Sortdata(int n, double v[N], int Id[N]) {  // сортировка массива v оптимизированным методом пузырька
    double tmp;		// вспомогательная переменная для обмена 2-х эл-тов массива
    bool swap;		// I оптимизация: флаг-признак: 1 = обмен при текущем проходе был, 0 = обмена не было
    int tmpi;
    for (int i = n - 1; i >= 1; --i) {
        swap = false;
        for (int j = 0; j < i; ++j) {  // II оптимизация: исключаем отсортированный "хвост" после i-го эл-та
            if (v[j] < v[j + 1]) {
                tmp = v[j + 1];
                v[j + 1] = v[j];
                v[j] = tmp;
                tmpi = Id[j + 1];
                Id[j + 1] = Id[j];
                Id[j] = tmpi;
                swap = true;
            }
        }
        if (!swap) break;
    }
}

// Среднее по столбцам
void EvalMC(double pZ[][N], int rows, int cols, double pmc[N])
{
    double sum;
    for (int j = 0; j < cols; j++)
    {
        sum = 0.0;
        for (int i = 0; i < rows; i++)
        {
            sum += pZ[i][j];
        }
        pmc[j] = sum / rows;
    }
}

void EvalDC1(double pZ[][N], int rows, int cols, double pmc[N], double pdc[N])//дисперсия по столбцу
{
    double sum;
    for (int j = 0; j < cols; j++)
    {
        sum = 0.0;
        for (int i = 0; i < rows; i++)
        {
            sum += ((pZ[i][j] - pmc[j])*(pZ[i][j] - pmc[j]));
        }
        pdc[j] = sum / rows;
    }
}

// СКО 
void EvalDC2(double pZ[][N], int rows, int cols, double pmc[N], double pdc[N])
{
    double sum;
    for (int j = 0; j < cols; j++)
    {
        sum = 0.0;
        for (int i = 0; i < rows; i++)
        {
            sum += (pZ[i][j] * pZ[i][j]);
        }
        pdc[j] = sqrt(sum);
    }
}

// Стандартизованная матрица
void EvalX(double pZ[][N], int rows, int cols, double pmc[N], double pdc[N], double pX[][N])
{
    for (int j = 0; j < cols; j++)
    {
        for (int i = 0; i < rows; i++)
        {
            pX[i][j] = (pZ[i][j] - pmc[j]) / sqrt(pdc[j]); //(элемент-среднее)/корень дисперсии
        }
    }
}

// Ковариционная матрица
void EvalS(double pZ[][N], int rows, int cols, double pmc[N], double pS[][N])
{
    double sum;
    for (int j = 0; j < cols; j++)
    {
        for (int i = 0; i < cols; i++)
        {
            sum = 0.0;
            for (int k = 0; k < rows; k++)
            {
                sum += ((pZ[k][i] - pmc[i])*(pZ[k][j] - pmc[j]));
            }
            pS[i][j] = sum / rows;
        }
    }
}

// Корреляционная матрица
void EvalR(double pX[][N], int rows, int cols, double pA[][N])
{
    double sum;
    for (int j = 0; j < cols; j++)
    {
        for (int i = 0; i < cols; i++)
        {
            sum = 0.0;
            for (int k = 0; k < rows; k++)
            {
                sum += pX[k][i] * pX[k][j];
            }
            pA[i][j] = sum / rows;
        }
    }
}

//нормировка
void Norm(double pT[][N], int rows, int cols, double pdc[N], double pMN[][N]) 
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            pMN[i][j] = (pT[i][j] / pdc[j]);
        }
    }
}

int EvalIp(double pL[N], int cols)
{
    double sumi;
    double sumj;

    sumj = 0.0;
    for (int j = 0; j < cols; j++)
    {
        sumj += pL[j];
    }

    sumi = 0.0;
    for (int i = 0; i < cols; i++)
    {
        sumi += pL[i] / sumj;
        if (sumi > 0.95) return i;
    }
    return 0;
}

// Вывод матрицы на экран
void ShowMatrix(int rows, int cols, double matrix[][N], char name)
{
    switch (name)
    {
    case 'A': { cout << "Source Matrix:" << endl; break; }
    case 'T': { cout << "Eigen Vectors:" << endl; break; }
    case 'S': { cout << "Reorded Eigen Vectors:" << endl; break; }
    case 'N': { cout << "Norm Matrix:" << endl; break; }
    case 'P': { cout << "PCA Matrix:" << endl; break; }
    }
    for (i = 0; i < rows; i++) {
        for (j = 0; j < cols; j++) {
            cout.fill(' ');
            cout.width(9);
            cout.precision(4);
            cout << matrix[i][j];
        }
        cout << endl;
    }
    cout << endl;
}

void CloneMatrix(int rows, int cols, double matrix[][N], double clone[][N]) {
    for (i = 0; i < rows; i++) {
        for (j = 0; j < cols; j++) clone[i][j] = matrix[i][j];
    }
}

void ReoderCols(int rows, int cols, double matrix[][N], int Id[N])
{
    double tmp[N][N];

    CloneMatrix(rows, cols, matrix, tmp);
    for (j = 0; j < cols; j++) {
        if (Id[j] != j) {
            for (i = 0; i < rows; i++) {
                matrix[i][j] = tmp[i][Id[j]];
            }
        }
    }
}

double Trace(int rows, double matrix[][N]) //сумма элементов по главной диагонали 
{
    double sum = 0.0;
    for (i = 0; i < rows; i++)
        sum += matrix[i][i];

    return sum;
}

//определяем количество главных компонент
int Kaizer(double tr, double pL[N], int cols)
{
    int cnt = 0;
    for (j = 0; j < cols; j++)
        if (pL[j] > tr / cols) cnt++;
    return cnt;
}

int BrokenStick(double tr, int cols, double pL[N], int Id[N])
{
    default_random_engine e(time(0));
    uniform_real_distribution<double> u(0, 1);
    for (j = 0; j < cols - 1; j++)
    {
        brakePoints[j] = u(e);
        sortPoints[j] = j;
    }
    Sortdata(cols - 1, brakePoints, sortPoints);

    double restOfStick = 1.0;
    for (j = 0; j < cols - 1; j++)
    {
        stickParts[j] = restOfStick - brakePoints[j];
        restOfStick -= stickParts[j];
    }
    stickParts[cols - 1] = restOfStick;
    Sortdata(cols, stickParts, sortParts);

    int cnt = 0;
    for (j = 0; j < cols; j++)
    {
        double sum = 0.0;
        for (int k = 0; k < cols - j; k++)
        {
            sum += stickParts[k] / cols;
        }
        if (pL[j] / tr > sum) cnt++;
    }
    return cnt;
}

void MultMatrix(int rows, int colsC, double matrixA[][N], double matrixB[][N], double matrixC[][N])
{
    for (i = 0; i < rows; i++)
    {
        for (j = 0; j < colsC; j++)
        {
            matrixC[i][j] = 0.0;
            for (int k = 0; k < rows; k++)
            {
                matrixC[i][j] += matrixA[i][k] * matrixB[k][j];
            }
        }
    }
}

// Вывод вектора на экран
void ShowVector(int rows, double v[N], char name)
{
    switch (name)
    {
    case 'L': { cout << "Eigen Values:" << endl; break; }
    case 'S': { cout << "Reordered Eigen Values:" << endl; break; }
    case 'M': { cout << "Means Vector:" << endl; break; }
    case 'D': { cout << "Diver Vector:" << endl; break; }
    }

    for (i = 0; i < rows; i++) {
        cout.fill(' ');
        cout.width(9);
        cout.precision(4);
        cout << v[i];
    }
    cout << endl;
}

int main()
{
    for (j = 0; j < N; j++)		// инициализация переменных
    {
        mc[j] = 0;
    }
    fstream MtrxFile;
    int p = 0;
    cout << "\n1. Separability quantity" << endl;
    cout << "2. Inseparable quantity" << endl;
    cout << "3. Irises" << endl;
    cout << "4. Exit" << endl;
    cin >> p;

    switch (p)	
    {
    case 1: // размерность текущей матрицы (n*m)
        n = 250;
        m = 5;
        MtrxFile.open("C:\\C\\neiro\\labs\\lab4\\raz.txt", ios::in);
        break;
    case 2:
        n = 250;
        m = 5;
        MtrxFile.open("C:\\C\\neiro\\labs\\lab4\\neraz.txt", ios::in);
        break;
    case 3:
        n = 150;
        m = 4;
        MtrxFile.open("C:\\C\\neiro\\labs\\lab4\\iris.txt", ios::in);
        break;
    default: p = 4; cout << "Exiting..." << endl;
    }

    if (!MtrxFile)
    {
        cerr << "Error reading file !\n";
        exit(6);
    }

    // Чтение матрицы из файла
    for (i = 0; i < n; i++)
    {
        for (j = 0; j < m; j++)
            MtrxFile >> Z[i][j];
    }
    MtrxFile.close();

    cout << "MGK" << endl;
    cout << "   " << endl << endl;
    cout.setf(ios::fixed);

    // Вывод считанной матрицы на экран
    cout << "Original matrix:  " << endl;
    cout.setf(ios::fixed);

    for (i = 0; i < n; i++)
    {
        for (j = 0; j < m; j++)
        {
            cout.fill(' ');
            cout.width(7);
            cout.precision(2);
            cout << Z[i][j];
        }
        cout << endl;
    }
    cout << endl;

    MtrxFile.close();

    EvalMC(Z, n, m, mc); //среднее по столбцам
    cout << "Average by columns:  " << endl;

    for (j = 0; j < m; j++)
    {
        cout.fill(' ');
        cout.width(7);
        cout.precision(2);
        cout << mc[j];
    }
    cout << endl << endl;

    EvalDC1(Z, n, m, mc, dc); //дисперсия по столбцу
    cout << "Dispers by columns:  " << endl;

    for (j = 0; j < m; j++)
    {
        cout.fill(' ');
        cout.width(7);
        cout.precision(2);
        cout << dc[j];
    }
    cout << endl << endl;

    EvalX(Z, n, m, mc, dc, X); // Стандартизованная матрица
    EvalS(Z, n, m, mc, S); 

    EvalR(X, n, m, A);

    cout << "Correlation matrix:  " << endl;
    for (i = 0; i < m; i++)
    {
        for (j = 0; j < m; j++)
        {
            cout.fill(' ');
            cout.width(7);
            cout.precision(2);
            cout << A[i][j];
        }
        cout << endl;
    }
    cout << endl;

    jacobi(m, A, eigenVals, eigenVects);

    ShowVector(m, eigenVals, 'L'); //собственный вектор
    cout << endl;

    ShowMatrix(m, m, eigenVects, 'T'); //матрица собственных векторов
    cout << endl;

    for (i = 0; i < m; i++) sortInd[i] = i;
    Sortdata(m, eigenVals, sortInd); //сортируем собственные ветора по величине значений

    cout << "Sort order: "; //определяем порядок значений
    for (i = 0; i < m; i++)  cout << sortInd[i] << " ";
    cout << endl << endl;

    ShowVector(m, eigenVals, 'S');
    cout << endl;

    ReoderCols(m, m, eigenVects, sortInd); //меняем порядок столбцов матрицы в соответствии с предыдущей сортировкой
    ShowMatrix(m, m, eigenVects, 'S');

    EvalMC(eigenVects, m, m, mc);//среднеарифметическое для векторов

    EvalDC2(eigenVects, m, m, mc, dc);//среднеквадратичное отклонение для векторов

    Norm(eigenVects, m, m, dc, MN);

    TrA = Trace(m, A);

    p = Kaizer(TrA, eigenVals, m);
    cout << "Principal Components Analysis by Kaizer method = " << p << endl;
    cout << endl;

    p = BrokenStick(TrA, m, eigenVals, sortInd);
    cout << "Principal Components Analysis by BrokenStick method = " << p << endl;
    cout << endl;

    p = EvalIp(eigenVals, m); //количество главных компонент

    MultMatrix(m, p, A, MN, PC);
    ShowMatrix(m, p, PC, 'P');
    cout << endl;
    cin >> n;

    return 0;
}