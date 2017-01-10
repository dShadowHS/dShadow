#include "lab3.h"

Quantity& Quantity::operator=(Quantity& lhs) //перегрузка оператора присваивания
{
    for (int i = 0; i < 5; i++)
    {
        pnt[i] = lhs.pnt[i];
    }
    class_ = lhs.class_;
    return *this;
}

Quantity& Quantity::operator+=(Quantity &rhs)
{
    for (int i = 0; i < 5; i++)
    {
        pnt[i] += rhs.pnt[i];
    }
    return *this;
}

Quantity& Quantity::operator/=(int k)
{
    for (int i = 0; i < 5; i++)
    {
        pnt[i] /= k;
    }
    return *this;
}

bool Quantity::operator==(const Quantity& lhs) const
{
    if ((pnt[0] == lhs.pnt[0]) && (pnt[1] == lhs.pnt[1]) && (pnt[2] == lhs.pnt[2]) && (pnt[3] == lhs.pnt[3]) && (pnt[4] == lhs.pnt[4]))
        return true;
    else
        return false;
}

double evk(Quantity& q, Quantity& c, int g)
{
    if (g == 3)
    {
        return sqrt(pow(c.rx() - q.rx(), 2) + pow(c.ry() - q.ry(), 2) + pow(c.rz() - q.rz(), 2) + pow(c.rw() - q.rw(), 2));
    }
    else
    {
        return sqrt(pow(c.rx() - q.rx(), 2) + pow(c.ry() - q.ry(), 2) + pow(c.rz() - q.rz(), 2) + pow(c.rw() - q.rw(), 2) + pow(c.rr() - q.rr(), 2));
    }
}

Quantity Quantity::givepnt(double p0, double p1, double p2, double p3, double p4, int cl)
{
    pnt[0] = p0;
    pnt[1] = p1;
    pnt[2] = p2;
    pnt[3] = p3;
    pnt[4] = p4;
    class_ = cl;
    return *this;
}

Quantity Quantity::givepnti(double p0, double p1, double p2, double p3, int cl)
{
    pnt[0] = p0;
    pnt[1] = p1;
    pnt[2] = p2;
    pnt[3] = p3;
    class_ = cl;
    return *this;
}

Quantity Quantity::ncl(int cl)
{
    class_ = cl;
    return *this;
}

ostream& operator<<(ostream& os, const Quantity& q)
{
    os << "(" << q.pnt[0] << " " << q.pnt[1] << " " << q.pnt[2] << " " << q.pnt[3] << " " << q.pnt[4] << ")";
    return os;
}

void fKmeans(int p, const int kcent)
{
    //Точки
    int qc;
    if (p == 3)
    {
        qc = 150;
    }
    else
    {
        qc = 250;
    }
    Quantity *Q = new Quantity[qc];
    Quantity *Q1 = new Quantity[kcent];
    ifstream chtf;
    int *array = new int[kcent];
    bool fl1 = true;
    if (p == 1)
    {
        chtf.open("C:\\C\\raz.txt");
    }
    else if (p == 2)
    {
        chtf.open("C:\\C\\neraz.txt");
    }
    else if (p == 3)
    {
        chtf.open("C:\\C\\iris.txt");
    }
    if (!chtf)
    {
        cout << "Can't open file.\n" << endl;
    }
    for (int i = 0; i < qc; i++)
    {
        double pnt[5];
        int cl;
        if (p == 3)
        {
            char buff[18];
            chtf.getline(buff, 18);
            sscanf_s(buff, "%lf|%lf|%lf|%lf|%i", &pnt[0], &pnt[1], &pnt[2], &pnt[3], &cl);
            Q[i].givepnti(pnt[0], pnt[1], pnt[2], pnt[3], cl);
        }
        else
        {
            char buff[42];
            chtf.getline(buff, 42);
            sscanf_s(buff, "%lf|%lf|%lf|%lf|%lf|%i", &pnt[0], &pnt[1], &pnt[2], &pnt[3], &pnt[4], &cl);
            Q[i].givepnt(pnt[0], pnt[1], pnt[2], pnt[3], pnt[4], cl);
        }
    }
    chtf.close();

    //Рандомизация центроидов
    int *massclass = new int[qc];
    int *massclassb = new int[qc];
    int lastm = 0;
    for (int i = 0; i < kcent; i++)
    {
        Quantity *QT = new Quantity[kcent];
        for (int j = 0; j < (qc / kcent); j++)
        {
            QT[i] += Q[j + i *(qc / kcent)];
            massclass[j + i *(qc / kcent)] = i;
            massclassb[j + i *(qc / kcent)] = i;
        }
        if (i == kcent - 1)
        {
            for (int j = (qc / kcent)*kcent; j < qc; j++)
            {
                QT[i] += Q[j];
                massclass[j] = i;
                massclassb[j] = i;
            }
            QT[i] /= ((qc / kcent) + (qc%kcent));
        }
        else
        {
            QT[i] /= (qc / kcent);
        }
        Q1[i] = QT[i];
        delete[] QT;
    }

    //Начало
    int *Qkol = new int[kcent];
    double *fevk = new double[kcent];
    double *ievk = new double[qc];
    int ch6, che, ch4 = 0;
    bool fl = true;
    while (fl)
    {
        ch6 = 0;
        che = 0;
        for (int i = 0; i < kcent; i++)
        {
            Qkol[i] = 1;
        }
        for (int i = 0; i < qc; i++)
        {
            for (int j = 0; j < kcent; j++)
            {
                fevk[j] = evk(Q[i], Q1[j], p);
            }
            int min = 0;
            double znach = fevk[0];
            for (int k = 0; k < kcent; k++)
            {
                if (fevk[k] < znach)
                {
                    znach = fevk[k];
                    min = k;
                }
            }
            massclass[i] = min;
        }
        for (int i = 0; i < kcent; i++)
        {
            Quantity *QT = new Quantity[kcent];
            for (int j = 0; j < qc; j++)
            {
                if (massclass[j] == i)
                {
                    QT[i] += Q[j];
                    Qkol[i]++;
                }
            }
            QT[i] /= Qkol[i];
            if (Q1[i] == QT[i])
            {
                che++;
            }
            Q1[i] = QT[i];
            delete[] QT;
        }
        for (int i = 0; i < qc; i++)
        {
            if (massclass[i] != Q[i].classs())
            {
                ch6++;
            }
        }
        ch4++;
        if (che == kcent)
        {
            fl = false;
        }
    }
    //Конец
    for (int j = 0; j < kcent; j++)
    {
        int ch5 = 0;
        cout << endl;
        cout << "Claster #" << (j + 1) << endl;
        cout << endl;
        for (int i = 0; i < qc; i++)
        {
            if (massclass[i] == j)
            {
                if (p == 3) cout << i << ": " << Q[i].rx() << " " << Q[i].ry() << " " << Q[i].rz() << " " << Q[i].rw() << endl;
                else cout << i << ": " << Q[i].rx() << " " << Q[i].ry() << " " << Q[i].rz() << " " << Q[i].rw() << " " << Q[i].rr() << endl;
                ch5++;
            }
        }
        cout << "Number of dots: " << ch5 << endl;
    }
    cout << endl << "Number of iterations: " << ch4 << endl;
    cout << "Error% - " << (((ch6 * 100) / qc) % 100) << "%" << endl;
}

void check(int p)
{
    int kcent;
    cout << "Enter number of clusters: ";
    cin >> kcent;
    fKmeans(p, kcent);
}

void main()
{
    srand(time(NULL));
    int p = 0;
    do
    {
        cout << "\n1. Separability quantity" << endl;
        cout << "2. Inseparable quantity" << endl;
        cout << "3. Irises" << endl;
        cout << "4. Exit" << endl;
        cin >> p;
        switch (p)
        {
            case 1: check(p); break;
            case 2: check(p); break;
            case 3: check(p); break;
            default: p = 4; cout << "Exiting..." << endl;
        }
    } while (p != 4);
}