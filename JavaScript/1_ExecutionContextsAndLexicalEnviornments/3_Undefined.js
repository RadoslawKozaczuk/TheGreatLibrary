// in JavaScript undefined is a special value
// and all variables are initially set to undefined

var a;
console.log(a);

if(a === undefined) {
	console.log('a is undefined!')
}
else {
	console.log('a is defined!')
}

// not to confuse with error 'a is not defined' which means variable 'a' was not present in the momory

// never set values to undefined it is a bad practice
a = undefined;
// because it makes debuging consfusing. Its better when only compilator is able to set this value 
// so it always mean the variable was not set at all