# Declare an annotation 'foo' which applies to struct and enum types.
annotation foo(struct, enum) :Text;

# Apply 'foo' to to MyType.
struct MyType $foo("bar") {
  dummy @0 :Void;
}

# 'baz' can annotate anything!
annotation baz(*) :Int32;

$baz(1);  # Annotate the file.

struct MyStruct $baz(2) {
  myField @0 :Text = "default" $baz(3);
  myUnion :union $baz(4) {
    dummy @0 :Void;
  }
}

enum MyEnum $baz(5) {
  myEnumerant @0 $baz(6);
}

interface MyInterface $baz(7) {
  myMethod @0 (myParam :Text $baz(9)) -> () $baz(8);
}

annotation myAnnotation(struct) :Int32 $baz(10);
const myConst :Int32 = 123 $baz(11);


annotation qux(struct, field) :Void;

struct MyStruct2 $qux {
  string @0 :Text $qux;
  number @1 :Int32 $qux;
}

annotation corge(file) :MyStruct2;

$corge(string = "hello", number = 123);

struct Grault {
  value @0 :Int32 = 123;
}

annotation grault(file) :Grault;

$grault();  # value defaults to 123
$grault(value = 456);