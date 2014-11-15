@0xa93fc509624c7204;
const pi :Float32 = 3.14159;
const bob :Person = (name = "Bob", email = "bob@example.com");

struct Person {
  name @0 :Text;
  email @1 :Text;
}

const foo :Int32 = 123;
const bar :Text = "Hello";
const baz :SomeStruct = (id = .foo, message = .bar);

struct SomeStruct {
  message @0 :Text;
  id @1 :Int32;
}