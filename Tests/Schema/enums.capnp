enum Rfc3092Variable {
  foo @0;
  bar @1;
  baz @2;
  qux @3;
  # ...
}

struct hasEnum {
  val @0 :Rfc3092Variable;
}