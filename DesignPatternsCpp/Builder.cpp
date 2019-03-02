#include <iostream>
#include <string>
#include <vector>
#include <sstream>
#include <memory>
#include <tuple>
#include <fstream>
using namespace std;

// some objects are complicated and required a lot of work to be created
// having an object with a constructor with 10 arguments is not a good idea
// instead, opt for piecewise construction
// builder provides an API for constructing an object step-by-step

// domain specific language approach
struct Tag
{
	string name, text;
	vector<Tag> children;
	vector<pair<string, string>> attributes;

	// print all the tags and childrean
	friend std::ostream& operator<<(std::ostream& os, const Tag& tag)
	{
		os << "<" << tag.name;

		for (const auto& att : tag.attributes)
			os << " " << att.first << "=\"" << att.second << "\"";

		if (tag.children.size() == 0 && tag.text.length() == 0)
		{
			os << "/>" << std::endl;
		}
		else
		{
			os << ">" << std::endl;
			if (tag.text.length())
				os << tag.text << std::endl;
			for (const auto& child : tag.children)
				os << child;
			os << "</" << tag.name << ">" << std::endl;
		}

		return os;
	}

protected:
	Tag(const string &name, const string &text) : name(name), text(text) {}
	Tag(const string &name, const vector<Tag> &childrean)
		: name(name), children(children) { }

};

struct P : Tag
{
	explicit P(const string &text) : Tag("p", text) { }
	P(std::initializer_list<Tag> children)
		: Tag("p", children) { }
};

struct IMG : Tag
{
	explicit IMG(const string& url)
		: Tag{ "img", "" }
	{
		attributes.emplace_back(make_pair("src", url));
	}
};


struct HtmlBuilder;

struct HtmlElement
{
	string name;
	string text;
	vector<HtmlElement> elements;
	const size_t indent_size = 2;

	HtmlElement() {}
	HtmlElement(const string& name, const string& text)
		: name(name), text(text) { }

	string str(int indent = 0) const
	{
		ostringstream oss;
		string i(indent_size*indent, ' ');
		oss << i << "<" << name << ">" << endl;
		if (text.size() > 0)
			oss << string(indent_size*(indent + 1), ' ') << text << endl;

		for (const auto& e : elements)
			oss << e.str(indent + 1);

		oss << i << "</" << name << ">" << endl;
		return oss.str();
	}

	// referece based API
	static HtmlBuilder build_ref(string root_name)
	{
		return HtmlBuilder(root_name);
	}

	// pointer based API
	static unique_ptr<HtmlBuilder> build_ptr(string root_name)
	{
		return make_unique<HtmlBuilder>(root_name);
	}
};

class HtmlBuilder
{
	HtmlElement root;

public:
	HtmlBuilder(string root_name)
	{
		root.name = root_name;
	}

	// overriding the operator gives us the ability to return HtmlElement instead of a Builder
	// purerly for convience sake
	operator HtmlElement() const { return root; }

	// more primitive not fluent interface
	void add_child(string child_name, string child_text)
	{
		HtmlElement e{ child_name, child_text };
		root.elements.emplace_back(e);
	}

	// fluent reference based - provides method chaining ability
	HtmlBuilder& add_child_fluent_ref(string child_name, string child_text)
	{
		HtmlElement e{ child_name, child_text };
		root.elements.emplace_back(e);
		return *this;
	}

	// fluent pointer based - provides method chaining ability
	HtmlBuilder* add_child_fluent_ptr(string child_name, string child_text)
	{
		HtmlElement e{ child_name, child_text };
		root.elements.emplace_back(e);
		return this;
	}

	string str() { return root.str(); }
};

int demo()
{
	// <p>hello</p>
	auto text = "hello";
	string output;
	output += "<p>";
	output += text;
	output += "</p>";
	printf("<p>%s</p>", text);

	// <ul><li>hello</li><li>world</li></ul>
	string words[] = { "hello", "world" };
	ostringstream oss;
	oss << "<ul>";
	for (auto w : words)
		oss << "  <li>" << w << "</li>";
	oss << "</ul>";
	printf(oss.str().c_str());

	// builder approach
	HtmlBuilder builder1{ "ul" };
	builder1.add_child("li", "hello");
	builder1.add_child("li", "world");
	cout << builder1.str() << endl;

	// we have a dedicated component to create things we need
	HtmlBuilder builder2{ "ul" };

	// we add elements piece by piece
	// here only two but could be millions :)
	builder2.add_child_fluent_ref("li", "hello")
		.add_child_fluent_ref("li", "world");

	// and finally we retrieve whatever we constructed and work with that
	cout << builder2.str() << endl;

	// same as above but different internal implementation
	auto builder3 = HtmlElement::build_ptr("ul")
		->add_child_fluent_ptr("li", "hello")
		->add_child_fluent_ptr("li", "world");
	cout << builder3 << endl;


	// domain specific language approach
	// the idea is to use object initializers to create a syntax
	// that resambles the html syntax
	std::cout <<
		P {
			IMG {"http://pokemon.com/pikachu.png"}
		}
	<< std::endl;

	getchar();
	return 0;
}