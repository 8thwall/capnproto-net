using Bar = import "bar.capnp";

struct Foo {
  # Use type "Baz" defined in bar.capnp.
  baz @0 :Bar.Baz;
}