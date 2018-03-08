console.log(this); // this will point to the global execution context

function a() {
	console.log(this);
	this.newVariable = "hello";
}

a(); // this will point to the global execution context

// after running the function I can access the variable
console.log("newVariable: " + newVariable);


var c = {
	name: "NameV1",
	log: function() {
		console.log("before update name: " + this.name);
		this.name = "NameV2";
		console.log("after update name: " + this.name); // this keyword points to the c object
		
		var setname = function(newname) {
			this.name = newname; // this keyword points at the global object
		}
		setname("NameV3");
		
		// so here we don't see the difference but the global object contain the name variable
		console.log("after setname call, name: " + this.name); 
	}
}

c.log();

// so to sum up:
// in global context this point at the global object.
// in any object's method this keyword point at the object.
// however any internal functions' this keywords will still point at the global object.
// But there is a pattern to avoid this problem we simply add a new variable and give it a meaningful name like self.

var c = {
	name: "NameV1",
	log: function() {
		var self = this;
		
		console.log("before update name: " + self.name);
		self.name = "NameV2";
		console.log("after update name: " + self.name); // this keyword points to the c object
		
		var setname = function(newname) {
			self.name = newname; // this keyword points at the global object
			// self is not declared in this function so engine will go one scope up
			// so the self will be taken from the outer context which is the one we want
		}
		setname("NameV3");
		
		// so here we don't see the difference but the global object contain the name variable
		console.log("after setname call, name: " + self.name); 
	}
}

c.log();