@0xa93fc509624c720E;
struct Person {
  # ...

  name @0 :Text;
  email @1 :Text;
  id @2 : Int32;
  ref @3 : Text;
  employment :union {
    unemployed @4 :Void;
    employer @5 :Company;
    school @6 :School;
    selfEmployed @7 :Void;
    # We assume that a person is only one of these.
  }
}

struct Company {
  id @0 : Int32;
}
struct School {
  id @0 : Int32;
}

struct Shape {
  area @0 :Float64;

  union {
    circle @1 :Float64;      # radius
    square @2 :Float64;      # width
  }
}