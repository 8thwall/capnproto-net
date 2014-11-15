@0xa93fc509624c720D;
struct Foo {
  struct Bar {
    #...
	dummy @0 :Void;
  }
  bar @0 :Bar;
}

struct Baz {
  bar @0 :Foo.Bar;
}
struct Qux {
  using Foo.Bar;
  bar @0 :Bar;
}

struct Corge {
  using T = Foo.Bar;
  bar @0 :T;
}