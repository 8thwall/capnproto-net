struct Foo {
  # Use type "Baz" defined in bar.capnp.
  baz @0 :import "bar.capnp".Baz;
}