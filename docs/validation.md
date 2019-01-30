# What can this thing do? And how can I do it?

Validate (almost) everything you need.
and
Just add the Calmo.Core.Validator namespace whenever you need it.

# Features

The most basic example, validating if a string is null or empty and throwing an exception if the validation fails
```
var model = new Model { Name = "John" };
model.Validate()
  .Using()
  .Rule(p => p.Name, n => !String.IsNullOrEmpty(n))
  .ThrowOnFail();
```

Again the most basic example, but with a custom exception
```
var model = new Model { Name = "John" };
model.Validate()
  .Using()
  .Rule(p => p.Name, n => !String.IsNullOrEmpty(n))
  .ThrowOnFail<MyCustomException>();
```

Validating multiple properties
When asking for the error messages, you can also provide a separator (",", new lines, etc)
```
var model = new Model { Name = "John", Age = 15 };
model.Validate()
  .Using()
  .Rule(p => p.Name, n => !String.IsNullOrEmpty(n))
  .Rule(p => p.Age, a => a >= 21)
  .ThrowOnFail<MyCustomException>(",");
```

You can have all your errors as a string, no exceptions
```
var model = new Model { Name = "John", Age = 15 };
var validationSummary= model.Validate()
  .Using()
  .Rule(p => p.Name, n => !String.IsNullOrEmpty(n), "Name is empty")
  .Rule(p => p.Age, a => a >= 21, "Underage!")
  .GetSummary(",");
```

You can validate documents and string formats in general (using the framework rules or extending it and creating your own)

```
var validation = model.Validate()
                    .Using()
                    .Rule(p => p.CPF, FormatValidation.Brazil.CPF)
                    .Rule(p => p.CPF, DocumentValidation.Brazil.CPF)
                    .Rule(p => p.Child, child => child != null)
                    .BreakIfIsInvalid()
                    .Rule(p => p.Child, child => !string.IsNullOrEmpty(child.Name));
```
Note: You can create break points in the validation chain using the 'BreakIfIsInvalid' to stop the validation and return only the errors generated up to that point.
