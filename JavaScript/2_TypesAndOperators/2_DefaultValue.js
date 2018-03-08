function greet(name) {
	name = name || "DefaultName"; // trick to set a default value
	// the way '||' operator works is that it returns the first value starting from the left that coerces to true
	// so undefined || "abc" returns "abc"
	console.log("Hello " + name);
}

greet("Tony");
greet();

// in case we had more than one script imported what happens is quite literally merging them all into one big file
// by stacking the code on the top of another

// lets say we have a variable libraryName and we don't want to touch it if other library already did
// we can do such thing in our .js file

var check = window.libraryName || "lib";
if(check === "lib")
{
	// library was found
}