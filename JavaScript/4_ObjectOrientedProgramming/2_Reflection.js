/* === Reflection ===
	An object can look at itself, listening and changing its properties and methods.
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

// don't do this EVER! for demo purposes only!!!
john.__proto__ = person;

// reflection
// this will loop over each property in the object
for(var prop in john) {
	// prop will store the name so we use that name to grab the value
	console.log(prop + ": " + john[prop]);
	
	// this loop reached up even for the prototype's objects
}

for(var prop in john) {
	// only what john contains
	if(john.hasOwnProperty(prop)) {
		console.log("Pure John: " + prop + ": " + john[prop]);	
	}
}