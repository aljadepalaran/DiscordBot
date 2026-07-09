# Understanding Lambdas, Delegates, and Configuration Methods in C#

## Overview

In C#, a lambda expression is a way of writing an anonymous function (a function without a name).

A common pattern in modern .NET applications is for a method to:

1. Create an object internally
2. Pass that object to a lambda function
3. Allow the lambda to configure or modify the object

This pattern is commonly used in configuration APIs, such as:

```csharp
builder.Services.AddDiscordGateway(options =>
{
    options.Token = "my-token";
});
```

The lambda is not creating the `options` object. The method creates the object and gives it to the lambda.

---

# Basic Example

Let's start with a simple class:

```csharp
class Person
{
    public string Name { get; set; }
}
```

A method can accept a function that knows how to configure a `Person`.

```csharp
static void ConfigurePerson(Action<Person> configure)
{
    Person person = new Person();

    configure(person);

    Console.WriteLine(person.Name);
}
```

---

## What is happening?

The method receives:

```csharp
Action<Person> configure
```

`Action<T>` means:

> "A function that accepts a T object and returns nothing."

In this case:

```csharp
Action<Person>
```

means:

> "A function that accepts a Person object."

---

When the method runs:

```csharp
Person person = new Person();
```

a new object is created:

```
Person object
--------------
Name = null
```

Then this line runs:

```csharp
configure(person);
```

The newly created `Person` object is passed into the function.

---

# Using the Method

Now we can call it:

```csharp
ConfigurePerson(person =>
{
    person.Name = "Alice";
});
```

The lambda:

```csharp
person =>
{
    person.Name = "Alice";
}
```

is a function.

The compiler understands that `person` must be a `Person` because the method expects:

```csharp
Action<Person>
```

It is equivalent to writing:

```csharp
ConfigurePerson((Person person) =>
{
    person.Name = "Alice";
});
```

The type does not need to be written because the compiler can infer it.

---

# The Parameter Name Does Not Matter

The name of the lambda parameter is completely up to you.

All of these are identical:

```csharp
ConfigurePerson(person =>
{
    person.Name = "Alice";
});
```

```csharp
ConfigurePerson(p =>
{
    p.Name = "Alice";
});
```

```csharp
ConfigurePerson(x =>
{
    x.Name = "Alice";
});
```

```csharp
ConfigurePerson(something =>
{
    something.Name = "Alice";
});
```

The compiler only cares about the type:

```
Action<Person>
       |
       |
       v
(Person anyNameYouWant) => { }
```

The name is only for readability.

---

# What the Lambda Is Not Doing

This code:

```csharp
ConfigurePerson(person =>
{
    person.Name = "Alice";
});
```

does **not** mean:

> "Create a Person object called person."

It means:

> "When you give me a Person object, I will modify it."

The method is responsible for creating the object:

```csharp
Person person = new Person();
```

The lambda is responsible for configuring it:

```csharp
person.Name = "Alice";
```

---

# Equivalent Code Without a Lambda

The lambda:

```csharp
ConfigurePerson(person =>
{
    person.Name = "Alice";
});
```

could also be written as a normal method:

```csharp
static void SetPersonName(Person person)
{
    person.Name = "Alice";
}
```

Then passed to the method:

```csharp
ConfigurePerson(SetPersonName);
```

A lambda is simply a shorter way of writing that function.

---

# Why Use This Pattern?

Imagine a library needs to create a complicated object.

Instead of forcing users to provide many parameters:

```csharp
CreateConnection(
    server,
    username,
    password,
    timeout,
    retryCount,
    encryption
);
```

the library can do:

```csharp
CreateConnection(options =>
{
    options.Server = "localhost";
    options.Timeout = 30;
    options.EnableEncryption = true;
});
```

The library controls object creation.

The user controls configuration.

---

# Connection to Discord Bot Configuration

The Discord example:

```csharp
builder.Services.AddDiscordGateway(options =>
{
    options.Token = "my-token";
});
```

likely works something like this internally:

```csharp
public void AddDiscordGateway(
    Action<DiscordGatewayOptions> configure)
{
    DiscordGatewayOptions options = new DiscordGatewayOptions();

    configure(options);

    StartGateway(options);
}
```

The flow is:

```
AddDiscordGateway()
        |
        |
        v
Create DiscordGatewayOptions object

        |
        |
        v
Pass object into lambda

        |
        |
        v
Lambda sets Token

        |
        |
        v
Use configured object
```

---

# Key Concepts Summary

| Concept          | Meaning                                     |
| ---------------- | ------------------------------------------- |
| Lambda           | Anonymous function                          |
| Action<T>        | Function that accepts T and returns nothing |
| Func<T>          | Function that returns a value               |
| `options => { }` | A lambda expression                         |
| Parameter name   | Can be anything                             |
| Parameter type   | Inferred from the delegate type             |
| Object creation  | Usually done by the receiving method        |
| Lambda purpose   | Configure, transform, or execute logic      |

---

# Simple Rule to Remember

When you see:

```csharp
SomeMethod(x =>
{
    x.SomeProperty = value;
});
```

ask:

1. What type does `SomeMethod` expect?
2. Is it an `Action<T>` or another delegate?
3. What object is being passed into the lambda?

The answer to those questions tells you what `x` actually is.
