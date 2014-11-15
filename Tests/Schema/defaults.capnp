@0xa93fc509624c7205;
struct Person {
  name @0 :Text;
  email @1 :Text;
}
struct HazDefaults {
	foo @0 :Int32 = 123;
	bar @1 :Text = "blah";
	baz @2 :List(Bool) = [ true, false, false, true ];
	qux @3 :Person = (name = "Bob", email = "bob@example.com");
	corge @4 :Void = void;
}