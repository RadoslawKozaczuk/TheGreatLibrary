function makeGreeting(language) {
	
	// this returns a function that has access to what the language variable was at the time of creation
	return function(firstname, lastname) {
		if(language === "en") {
			console.log("Hello " + firstname + " " + lastname);
		}
		
		if(language === "es") {
			console.log("Hola " + firstname + " " + lastname);
		}
	}
}

// greetEnglish is a function whose closure points to a language variable that store "en"
var greetEnglish = makeGreeting("en");

// this one on the other hand will find "es" in the scope chain
var greetSpanish = makeGreeting("es");

// now we have two spots in memory that hangs around a stores languages
greetEnglish("John", "Doe"); // JS knows that its outer environment points to the first language and forms a closure
greetSpanish("John", "Doe"); // same but different spot in memory


function sayHiLater() {
	var greeting = "Hi!";
	
	// this takes advantage of closures, greeting will be available
	setTimeout(function() {
		console.log(greeting);
	}, 3000);
}

sayHiLater();

/* === Callback Function ===
	A function you give to another function, to be run when the other function is finished.
	So the function you call, 'calls back' by calling the function you gave it when it finishes.
*/

// named callback for simplicity
function tellMeWhenDone(callback) {
	var a = 1000; // some work
	callback(); // the 'callback', it runs the function I gave it
}

tellMeWhenDone(function() {
	console.log("I am done!");
});