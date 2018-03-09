/* === Inheritance ===
	One object gets access to the properties and methods of another object.
*/

/* === Prototypal Inheritance ===
	Simplified version of the regular inheritance used for example in C#.
	All objects in JS (so including functions) have a property made by engine named '__proto__'.
	If the engine does not find the property we are looking for in the object itself it goes to what __proto__ points at.
	What __proto__ points at may have its own prototype.
	Such chain of prototypes is called the 'Prototype Chain'.
	Many object can reference the same object through theirs proto variable.
*/

var person = {
	firstname: "Default",
	lastname: "Default",
	getFullName: function() {
		return this.firstname + " " + this.lastname;
	}
}

var john = {
	firstname: "John",
	lastname: "Doe"
}

// we should never access the __proto__ variable unless we really know what we are doing.
john.__proto__ = person;

// now john inherits from person
console.log(john.getFullName());

// engine first look in john object
// so john's firstname hides person's firstname thats why we don't get 'Default'
console.log(john.firstname);

var jane = {
	firstname: "Jane"
}

jane.__proto__ = person;

// we get 'Jane Default' because there is no lastname variable in jane object so it look for it in the prototype
console.log(jane.getFullName());


// at the bottom of every prototype chain is the base object (for objects) or the base function object (for functions)
// and these prototypes provide all the stuff like toString(), bind() etc.
// arrays have their own prototype with methods like push reduce and so on

// in case of an array it looks like this
var arr = [];
console.log(arr.__proto__) // base array object
console.log(arr.__proto__.__proto__) // base object