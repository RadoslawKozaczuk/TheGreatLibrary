#include <string>
#include <vector>
#include <iostream>
#include "Solid.cpp"

using namespace std;

int main()
{
	Solid::OpenClosePrinciple ocp{};
	Solid::LiskovsSubstitutionPrinciple liskov{};
	Solid::InterfaceSegregationPrinciple isp{};
	Solid::DependencyInversionPrinciple dip{};

	//ocp.open_closed_principle_demo();
	//liskov.liskovs_substitution_principle_demo();
	//isp.interface_segregation_principle_demo();
	dip.dependency_inversion_principle_demo();

	cout << "Program has ended. Press any button to close." << endl;
	getchar();
}