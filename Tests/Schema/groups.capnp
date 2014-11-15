@0xa93fc509624c7207;
struct Person {
  # ...
  dummy0 @0 :AnyPointer;
  dummy1 @1 :Void;
  dummy2 @2 :Void;
  dummy3 @3 :Void;
  dummy4 @4 :Void;
  dummy5 @5 :Void;
  dummy6 @6 :Void;
  dummy7 @7 :Void;

  # Note:  This is a terrible way to use groups, and meant
  #   only to demonstrate the syntax.
  address :group {
    houseNumber @8 :UInt32;
    street @9 :Text;
    city @10 :Text;
    country @11 :Text;
  }
}

struct Shape {
  area @0 :Float64;

  union {
    circle :group {
      radius @1 :Float64;
    }
    rectangle :group {
      width @2 :Float64;
      height @3 :Float64;
    }
  }
}