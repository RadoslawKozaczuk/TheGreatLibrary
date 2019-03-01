/* Advantages
	- performance, C++ programs are typically faster or a least as fast than their C# counterparts
	- low-level hardware access (for example inline assembly)
	- C++ is compiled directly to native code which cannot be decompiled. This may be important in case of intelectual property protection.
	- C++ is mature language
*/

/* C++ Disadvantages
	- requires manual memory managment. This can be a source of bugs and requires more work, but gives you deterministic destruction.
	- compiled programs have no metadata, so it is difficuly to figure out hteir structure without additional information.
		Third-partyy libraries typically shipped as source code.
	- C++ has a preprocessor which is a potential source of bugs and ambiguities.
	- less readable error messages
	- compilation is slower than in C# or Java. This can be mitigated by using cluster compilation for larger programs.
	- lots of inheritated baggage from C due to backward compatibility.
*/

/* Compilers
	- Visual Studio uses Microsoft Visual C++ Compiler. Windows only.
	- Intel C++ Compiler. Exist on all operating systems.
	- GCC is a free compiler.
*/

/*
	Build system builds your program out of the source code files you provide. A local build system runs directly on your machiune.
	Visual Studio uses MSBuild build system.
*/

// a .cpp file with all the files it includes is called a translation unit and is the basic unit of compilation in C++
// a program is made from one or more .cpp files.
// Project can have a precompiled header - a header file containing #includes of commonly used libraries. The header can then be reused (even in other projects).
//	It is a performance optimization for compile times. Its   only the .cpp files are built.

// compilation in C++ is a proccess of tuirning a compilation unit into an object file (.obj)

// linking process in C++ is a process of merging all object files into  the final binary files
// we can also link to an external lbrary
// static library meaning that the entire library gets inclkuded wholsale in your binary
// dtnamic lbrary - files exists as a separate file

// in Visual Studio when we click on the Build button all  the things happens at once
// first preprocessor then compilation then liking
// in Visual Studio we cannot isolate substeps of the process

//#include <string>
#include <vector>
#include <iostream>
#include <array>
#include <functional>
using namespace std;

// preprocessor
#define PI 3.14

// macros
#define MUL(a,b) a*b
#define MAD(a,b,c) MUL(a,b) + c

#define IV(name) int var_ ## name = 0;

int main()
{
	string A{ 'l', 'e', 'm', 'o', 'n' };
	string B{ 'h', 'a', 'r', 'd', 'e', 'r' };

	// write your code in C++14 (g++ 6.2.0)
	int size_of_a = A.size();
	int size_of_b = B.size();

	std::cout << size_of_a << endl;
	std::cout << size_of_b << endl;

	int how_many_chars_a[26];
	int how_many_chars_b[26];

	std::cout << A[0] << endl;
	std::cout << A[1] << endl;
	std::cout << A[2] << endl;

	std::cout << (int)A[0] << endl;
	std::cout << (int)A[1] << endl;
	std::cout << (int)A[2] << endl;

	std::cout << (int)'a' << endl;
	std::cout << (int)'b' << endl;
	std::cout << (int)'z' << endl;

	int letter = 0;
	for (int i = 0; i < size_of_a; i++)
	{
		// a - 97
		// z - 122
		letter = A[i] - 97;
		how_many_chars_a[letter]++;

		letter = B[i] - 97;
		how_many_chars_b[letter]++;
	}

	int totalResult = 0;
	for (int i = 0; i < 26; i++)
	{
		// first we have
		totalResult += abs(how_many_chars_a[i] - how_many_chars_b[i]);
	}

	return totalResult;




	// ion C++ esist something called unified initialization
	int foo{ 42 };
	char boo{ 'b' };

	// another way si the classic way
	int a = 2;

	// the result of an assignment is the value assigned


	// C++ standard only specify the minimum number of bits the particular integral can take
	// so for example int an be 32 bit on what platform and 16 on another
	unsigned int n = 42u; // int is at least 16 bits (but can be 32 or even 64 bits)
	// long is at least 32 bites

	// variables are signed by default

	// C++ does not have a byte type. We can resuse an unsigned char or uint8_t from <cstdint>
	unsigned char byteEquivalent;
	uint8_t anotherByteEquivalent;

	// conversion
	// a large type converts into a smaller type but extra information is lost
	// in C++ conversions does not requires any special operators or function calls - this is dangerous

	// long double is typically 80 bits but some compilators equate it with double (64 bits). It is not recomended to use it.

	// 'auto' keyword is an equivalent of 'var' in C#
	auto c{ 7ul }; // this would typically infer to int but we want unsigned long so postfix is necessary

	// bool to int conversion is implicit
	int i = 1; // if(i) -> true
	i = true; // i = 1;


	// Scope
	// in C++ it is possible to declare an identically named variable in an inner scope
	int n = 0;
	{
		int n = 2;
		n++; // inner n is 3, outer is still 0
	}

	// C++ alows for using global variables


	// arrays can be allocated dynamically
	float* data = new float[123];
	delete[] data;
	// in C++ stack allocated arrays must have a constant size
	//void foo(int count) { int values[count]; } // illegal

	// === pointers ===
	// pointers can be made by using the new operator or by taking an address of another variable
	int32_t x = 0;
	int32_t* y = &x; // taking address of an variable

	// to get the data we have to dereference the pointer
	cout << "x=" << *y << endl;

	// a pointer that does not point anywhere should have a value of nullptr
	y = nullptr; // dereferenciing a nullptr can result in a run time error // I am not sure if IDE protects from it

	// unfortunatelly some operators happens before the pointers
	int a1 = 42;
	int* b1 = &a;
	//*b1++; // this will first increment the pointer and then dereference it
	*(b1++); // this is the correct approach
	int& c1 = (*b1);
	c1++;

	// === references ===
	// references are like pointers in that they refer to memory locations but syntax is different.
	int32_t x2 = 0;
	int32_t& x2reference = x;
	x2reference = 123;
	cout << "x2=" << x2reference << endl; // we don't need to dereference

	// there are also rvalue references - more advaned


	// standard C++ arrays are so-called 'c-styled arrays'
	float cStyleArray[12]; // there is a huge disadvantage it does not have any length property or method in built

	// what if we don't know the size until the runtime
	int runTimeSize = 123;
	float* pointerToTheFirstElement = new float[runTimeSize]; // heap allocation
	delete[] pointerToTheFirstElement; // so has to be cleaned

	// range-based loop
	// std::vector is a dynamically resizable array. The vector manages its own memory internally.
	vector<int> vectors{ 0, 1, 2, 3 };
	for (int& v : vectors)
		cout << v << endl;

	// C++ super multidimension feature, previously we had to decleare an array of arrays
	float multiDim[2][2] = { {0, 1}, {2, 3} };
	delete[] multiDim; // convienient delete

	// c-style arrays are outdateted
	// construction uses aggregate initialization
	std::array<int, 3> a1{1, 2, 3}; // double-braces required in C++11 (not in C++14)
	std::array<int, 3> a2 = { 1, 2, 3 };
	std::array<std::string, 2> a3 = { std::string("a"), "b" };


	// Wide Strings
	//wchar_t* wideString = L"Hello"; // can be 2 bytes (sometimes 1)
	// TODO something doesnt work with the above


	// Function Pointers
	int(*f)(int) = &function_to_be_pointed_at;
	int x = f(5);

	// part of the #include <functional>
	// better way
	std::function<int(int)> f = function_to_be_pointed_at;

	// store a lambda
	std::function<int(int)> f_lambda = [](auto a) { return function_to_be_pointed_at(a); };
	int function_result = 5;
	function_result = f(function_result);
	function_result = f_lambda(function_result);

	// capture list - defines which vaiables ouside the lambda the lambda uses. It can capture by value or by reference.
	// In C# everything is just captured by ref or val depends on data type
	auto f_lambda_with_capture = [function_result](int x) { return function_result + x; };

	// possible capture values
	// [=] - capture everything by value
	// [&] - capture everything by reference
	// [x, &y] - capture x by value and y by reference
	// [&, z] - capture everything by reference and z by value
	// using a variable without specifing it in a capture list will result in an compilation error


	// The typedef keyword lets us define a new name for a particular type
	typedef unsigned char byte;
	byte myByte = 100;

	enum class Color { Red, Green, Blue = 3};
	//Color c = 123; // illegal;
	Color d = Color::Red;


	// A union is a data structure which lets you to treat several bytes of data in different ways.
	union FourBytes
	{
		uint32_t int_value;
		unsigned char bytes[4];
	};
	FourBytes fb;
	fb.int_value = 123;
	fb.bytes[2] = 4;


	// structs
	struct Point
	{
		float x, y;
	};
	Point p;
	p.x = 1.2f;

	// we atumatically get a copy constructor
	Point p2(p);
	Point p3{ p };
	// and copy assignment operator
	Point p4 = p;

	// a struct supports uniform initialization
	Point p5{ 10, 20 };

	Point* p_pointer = &p;
	float* xptr = &p.x;

	// we can define and initialize a struct in one line which is not super readable but ok
	struct Point2 { float x, y; } p6{ 2.f, 12 };

	// structs can have methods in C++
	// in C++ structs and classes are almost identical
	// the key difference is that struct members by default are public
	// while class members are by defult private
	struct PersonStruct
	{
		std::string name;

		void say_hello()
		{
			cout << "Hello" << endl;
		}
	};

	// class in opposition to structs can have constructors
	class PersonClass
	{
	public:
		PersonClass(); // constructor without a body
		~PersonClass();
		std::string name;
		int age;
	};

	class SuperHuman : public PersonClass
	{
	public:
		SuperHuman() { name = "super"; }
	};

	// encapsulation - a class can hide some of its members
	// polymorphism - you can treat a derived type as the base class

	// dynamic_cast operator
	auto regular_guy = PersonClass();
	auto super_human = SuperHuman();
	PersonClass* regular_guy_ptr = &regular_guy;
	SuperHuman* super_human_ptr = &super_human;

	// we have a pointer - that points at PersonClass object
	// but how we can tell if it is by chance a SuperHuman?
	//if (SuperHuman* sh = dynamic_cast<SuperHuman*>(regular_guy_ptr)) // something doesnt work
	//{
		// we got a valid pointer
	//}
	//else
	//{
		// if it not a superHuman pointer we will get a nullptr value
	//}
}

// C++ can infer the return type - I am not sure if it is a common practice tho
auto get_some_double() { return 0.5; }

int function_to_be_pointed_at(int x) { return x + 1; }

//



