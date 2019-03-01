#include <string>
#include <vector>
#include <iostream>
using namespace std;

namespace Solid
{
	// === open closed principle and a specification pattern ===
	// open for extension, closed for modification
	class OpenClosePrinciple
	{
		enum class Color { red, green, blue };
		enum class Size { small, medium, large };

		struct Product
		{
			string name;
			Color color;
			Size size;
		};

		// this filter violates open-closes principle because every time new filtering method is added we have to modify this class
		struct ProductFilter
		{
			typedef vector<Product*> Items;

			Items by_color(Items items, const Color color)
			{
				Items result;
				for (auto& i : items)
					if (i->color == color)
						result.push_back(i);
				return result;
			}

			Items by_size(Items items, const Size size)
			{
				Items result;
				for (auto& i : items)
					if (i->size == size)
						result.push_back(i);
				return result;
			}

			Items by_size_and_color(Items items, const Size size, const Color color)
			{
				Items result;
				for (auto& i : items)
					if (i->size == size && i->color == color)
						result.push_back(i);
				return result;
			}
		};

		template <typename T> struct AndSpecification;

		template <typename T> struct Specification
		{
			virtual ~Specification() = default;
			virtual bool is_satisfied(T* item) const = 0;

			// it breakes OCP a bit as we have to extend the Specification class afterwards
			template <typename T> AndSpecification<T> operator&& (const Specification<T>& second)
			{
				return { *this, second };
			}
		};

		template <typename T> struct Filter
		{
			virtual vector<T*> filter(vector<T*> items, Specification<T>& spec) = 0;
		};

		struct BetterFilter : Filter<Product>
		{
			vector<Product*> filter(vector<Product*> items, Specification<Product> &spec) override
			{
				vector<Product*> result;
				for (auto& p : items)
					if (spec.is_satisfied(p))
						result.push_back(p);
				return result;
			}
		};

		struct ColorSpecification : Specification<Product>
		{
			Color color;

			ColorSpecification(Color color) : color(color) {}

			bool is_satisfied(Product *item) const override
			{
				return item->color == color;
			}
		};

		struct SizeSpecification : Specification<Product>
		{
			Size size;

			explicit SizeSpecification(const Size size)
				: size{ size }
			{
			}

			bool is_satisfied(Product* item) const override
			{
				return item->size == size;
			}
		};

		template <typename T> struct AndSpecification : Specification<T>
		{
			const Specification<T>& first;
			const Specification<T>& second;

			AndSpecification(const Specification<T>& first, const Specification<T>& second)
				: first(first), second(second) {}

			bool is_satisfied(T *item) const override
			{
				return first.is_satisfied(item) && second.is_satisfied(item);
			}
		};

	public:
		void open_closed_principle_demo()
		{
			Product apple{ "Apple", Color::green, Size::small };
			Product tree{ "Tree", Color::green, Size::large };
			Product house{ "House", Color::blue, Size::large };

			const vector<Product*> all{ &apple, &tree, &house };

			BetterFilter bf;
			ColorSpecification green(Color::green);
			auto green_things = bf.filter(all, green);
			for (auto& x : green_things)
				cout << x->name << " is green\n";

			SizeSpecification large(Size::large);
			AndSpecification<Product> green_and_large(green, large);

			//auto big_green_things = bf.filter(all, green_and_large);

			// use the operator instead (same for || etc.)
			auto spec = green && large;
			for (auto& x : bf.filter(all, spec))
				cout << x->name << " is green and large\n";

			// warning: the following will compile but will NOT work
			//auto spec2 = SizeSpecification{Size::large}
			//	&& ColorSpecification{Color::blue};
		}
	};

	class LiskovsSubstitutionPrinciple
	{
		class Rectangle
		{
		protected:
			int width, height;
		public:
			Rectangle(const int width, const int height)
				: width{ width }, height{ height } { }

			int get_width() const { return width; }
			virtual void set_width(const int width) { this->width = width; }
			int get_height() const { return height; }
			virtual void set_height(const int height) { this->height = height; }

			int area() const { return width * height; }
		};

		class Square : public Rectangle
		{
		public:
			Square(int size) : Rectangle(size, size) {}

			// this broken the Liskov's Substitution Princpile
			void set_width(const int width) override {
				this->width = height = width; // square has broken the setter mechanics
			}
			void set_height(const int height) override {
				this->height = width = height;
			}
		};

		// one of the possible solutions
		struct RectangleFactory
		{
			static Rectangle create_rectangle(int w, int h);
			static Rectangle create_square(int size);
		};

		void process(Rectangle& r)
		{
			int w = r.get_width();
			r.set_height(10);

			std::cout << "expected area = " << (w * 10)
				<< ", got " << r.area() << std::endl;
		}

	public:
		void liskovs_substitution_principle_demo()
		{
			Rectangle r{ 5,5 };
			process(r);

			Square s{ 5 };
			process(s);

			getchar();
		}
	};

	// break up the interface into smaller interfaces so all the method are always needed
	// in need we can always combine smaller interfaces into a bigger one
	class InterfaceSegregationPrinciple
	{
		struct Document;

		// this interface is just too big
		struct IMachine
		{
		  virtual void print(Document& doc) = 0;
		  virtual void fax(Document& doc) = 0;
		  virtual void scan(Document& doc) = 0;
		};

		// in case when everything is needed
		struct MFP : IMachine
		{
		  void print(Document& doc) override;
		  void fax(Document& doc) override;
		  void scan(Document& doc) override;
		};

		// Client does not need this.
		// Forcing implementors to implement too much
		// and then what? Do nothing, return 0, throw an exception?
		// It sends a wrong message as it suggest such method is needed
		// while it is not.

		// this is how it is suppose to be done
		struct IPrinter
		{
			virtual void print(Document& doc) = 0;
		};

		struct IScanner
		{
			virtual void scan(Document& doc) = 0;
		};

		struct Printer : IPrinter
		{
			void print(Document& doc) override;
		};

		struct Scanner : IScanner
		{
			void scan(Document& doc) override;
		};

		// in case we need something more complicated we can always combine interfaces
		struct IMachineV2 : IPrinter, IScanner
		{
		};

		struct Machine : IMachineV2
		{
			IPrinter& printer;
			IScanner& scanner;

			Machine(IPrinter& printer, IScanner& scanner)
				: printer{ printer },
				scanner{ scanner }
			{
			}

			void print(Document& doc) override {
				printer.print(doc);
			}
			void scan(Document& doc) override;
		};

	public:
		void interface_segregation_principle_demo()
		{
			// nothing to show check the code
			getchar();
		}
	};

	// its split into two ideas
	// A. High-level modules should not depend on low-level modules.
//    Both should depend on abstractions.
// B. Abstractions should not depend on details.
//    Details should depend on abstractions.

	// when we talk about abstractions we generally mean interfaces or base classes

	// it proects you from the changes in the details
	class DependencyInversionPrinciple
	{
		enum class Relationship
		{
			parent,
			child,
			sibling
		};

		struct Person
		{
			string name;
		};

		// let's introduce an abstraction
		struct RelationshipBrowser
		{
			virtual vector<Person> find_all_children_of(const string& name) = 0;
		};

		// data is low level
		struct Relationships : RelationshipBrowser // low-level
		{
			vector<tuple<Person, Relationship, Person>> relations;

			void add_parent_and_child(const Person& parent, const Person& child)
			{
				relations.push_back({ parent, Relationship::parent, child });
				relations.push_back({ child, Relationship::child, parent });
			}

			vector<Person> find_all_children_of(const string &name) override
			{
				vector<Person> result;

				for (auto&&[first, rel, second] : relations)
					if (first.name == name && rel == Relationship::parent)
						result.push_back(second);

				return result;
			}
		};

		// analizing data is high-level
		struct Research // high-level
		{
			// this is the violation of the dependency inversion principle
			// we depend on the details and in case the vector of tuples was replaced
			// our code will no longer work
			Research(const Relationships& relationships)
			{
				auto& relations = relationships.relations;
				for (auto&& [first, rel, second] : relations)
					if (first.name == "John" && rel == Relationship::parent)
						cout << "John has a child called " << second.name << endl;
			}

			// now we don't depend on the details but ont the browser
			Research(RelationshipBrowser& browser)
			{
				for (auto& child : browser.find_all_children_of("John"))
					cout << "John has a child called " << child.name << endl;
			}
		};

	public:
		void dependency_inversion_principle_demo()
		{
			Person parent{ "John" };
			Person child1{ "Chris" };
			Person child2{ "Mat" };

			Relationships relationships;
			relationships.add_parent_and_child(parent, child1);
			relationships.add_parent_and_child(parent, child2);

			Research research(relationships);

			getchar();
		}
	};
}