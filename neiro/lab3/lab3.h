#ifndef lab3_H
#define lab3_H

#include <array>
#include <algorithm>
#include <iostream>
#include <fstream>
#include <time.h>
#include <fstream>
#include <string>

using namespace std;

class Quantity
{
public:
    Quantity() {};
    ~Quantity() = default;
    static constexpr ptrdiff_t dim() { return dimCount_; }
    friend ostream& operator<<(ostream& os, const Quantity& Q);
    double x() const { return pnt[0]; }
    double y() const { return pnt[1]; }
    double z() const { return pnt[2]; }
    double w() const { return pnt[3]; }
    double r() const { return pnt[4]; }
    double& rx() { return pnt[0]; }
    double& ry() { return pnt[1]; }
    double& rz() { return pnt[2]; }
    double& rw() { return pnt[3]; }
    double& rr() { return pnt[4]; }
    int classs() { return class_; }
    Quantity givepnt(double p0, double p1, double p2, double p3, double p4, int cl);
    Quantity givepnti(double p0, double p1, double p2, double p3, int cl);
    Quantity ncl(int cl);
    Quantity& operator=(Quantity& lhs);
    Quantity& operator+=(Quantity& lhs);
    Quantity& operator/=(int k);
    bool operator==(const Quantity& lhs) const;
    double* begin() { return pnt; }
    double* end() { return pnt + dimCount_; }
    const double* cbegin() { return pnt; }
    const double* cend() { return pnt + dimCount_; }
private:
    static const ptrdiff_t dimCount_{ 5 };
    double pnt[dimCount_] = { 0 };
    int class_;
};

void fKmeansIr();
void SMenu();
void fGeneratorR();
void fGenerator();
void fKmeansI(int g, const int kcent);
void fKmeans();

#endif