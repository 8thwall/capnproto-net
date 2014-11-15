@0xa93fc509624c7206;
enum Rfc3092Variable {
  foo @0;
  bar @1;
  baz @2;
  qux @3;
  # ...
}

struct HazEnum {
  val @0 :Rfc3092Variable;
}