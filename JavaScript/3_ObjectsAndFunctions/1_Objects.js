var person = new Object(); // object is just a set of name-value pairs and value maybe another set of name-value pairs

person["firstName"] = "Tony"; // this line will create that place in the memory and assign a value to it
person["lastName"] = "Morena";

var firstNameProperty = "firstName";

console.log(person);
console.log(person[firstNameProperty]);
console.log(person.firstName);


person.address = new Object();
person.address.street = "123 Main St.";

console.log(person.address.street);
console.log(person["address"]["street"]);