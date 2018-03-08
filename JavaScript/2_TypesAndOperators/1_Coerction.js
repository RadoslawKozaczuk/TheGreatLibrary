/* Dynamic Typing 
	- you don't tell the engine what type of data a variable holds, it figures it out while your code is running.
	- variables can hold  different types of values because it's all figured out during execution.
*/

/* there is 6 primitive types in JS
	- undefined // variable was never set in code
	- null // represents lack of value
	- boolean
	- number // always floating point number
	- string
	- NaN // not a number, when coercion to a number failed for example Number("abc")
	- symbol // only in ES6 - ???
*/

// Coercion - converting a value from one type to another. This happens quite a bit it JS.

var a = 1 + "2";
var b = "1" + 2;

console.log("a: type: " + typeof a + " value: " + a);
console.log("b: type: " + typeof b + " value: " + b);

// result is string in both cases meaing string is being proritetized


console.log(1 < 2 < 3); // true
console.log(3 < 2 < 1); // also true!

/* there is assiosiativity and '<' operator goes from left to right so:
	3 < 2 < 1
	false < 1
	Number(false) < 1 // coerction we convert boolean value to number, Number(true) would be 1
	0 < 1
	true
*/

console.log("Number(true) = " + Number(true));
console.log("Number(undefined) = " + Number(undefined));
console.log("Number(null) = " + Number(null)); // this is weird one - it is good to remember

console.log(false == 0); // true
console.log(null == 0); // false, this does not coerce to zero when in comparision

console.log("" == 0); // true
console.log("" == false); // false

// so save our butt we use triple equals
console.log("3" === 3); // false 
// it basicaly says false whenever types are different
// in general 99% of times use triple equals to avoid unpredictible errors