@0xa93fc509624c7209;
struct Foo {
  # Use type "Baz" defined in bar.capnp.
  baz @0 :import "bar.capnp".Baz;
}