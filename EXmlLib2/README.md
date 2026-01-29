# EXmlLib2

**EXmlLib2** is a flexible, extensible .NET library for XML serialization and deserialization built on top of `System.Xml.Linq` (`XElement`, `XDocument`). It provides a fluent, type-safe API and supports customization through pluggable serializers and deserializers.

---

## Features

- **Fluent Configuration**: Build serialization/deserialization pipelines with a clean, chainable API.
- **Extensible Architecture**: Register custom serializers and deserializers for any type.
- **Built-in Support** for:
  - Primitive types (int, double, bool, char, etc.)
  - Strings, enums, and nullable types
  - Common types (`DateTime`, etc.)
  - Collections (arrays, `List<T>`)
  - Custom objects with property-based mapping
- **Type Mapping**: Control XML element names for polymorphic collections (e.g., `<int>`, `<double>`).
- **Attribute and Element Serialization**: Choose whether to serialize as XML attributes or elements.
- **Custom Property Handling**: Ignore, rename, or provide custom serialization logic per property.
- **Factory-based Instance Creation**: Control how objects are instantiated during deserialization.

---

## Installation

Clone the repository and reference the `EXmlLib2` project in your solution, or build and distribute it as a NuGet package (if applicable).

```bash
git clone https://github.com/Engin1980/eSystem.NET
```

---

## Quick Start

### 1. Serialize a Simple Type

```csharp
using EXmlLib2;
using System.Xml.Linq;

string source = "Hello, World!";

EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
XElement root = new XElement("Root");
exml.Serialize(source, root);

Console.WriteLine(root); // <Root>Hello, World!</Root>
```

### 2. Deserialize a Simple Type

```csharp
string xml = "<Root>Hello, World!</Root>";
XElement element = XElement.Parse(xml);

EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
string result = exml.Deserialize<string>(element);

Console.WriteLine(result); // Hello, World!
```

### 3. Serialize and Deserialize Collections

```csharp
using EXmlLib2.Implementations.EnumerableSerialization;

List<int> source = new() { 1, 2, 3 };
XElement element = new XElement("Root");

// Serialize
EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();
exml.ElementSerializers.AddFirst(new ListAndArraySerializer());
exml.Serialize(source, element);

// Deserialize
EXml exmlDes = EXml.Create().WithPrimitiveTypesAndStringSerialization();
exmlDes.ElementDeserializers.AddFirst(new ListAndArrayDeserializer());
List<int> result = exmlDes.Deserialize<List<int>>(element);

// result: [1, 2, 3]
```

### 4. Serialize Custom Objects

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

Person person = new() { Name = "Alice", Age = 30 };
XElement root = new XElement("Root");

EXml exml = EXml.Create()
    .WithPrimitiveTypesAndStringSerialization()
    .WithObjectSerialization();

exml.Serialize(person, root);

Console.WriteLine(root);
// <Root>
//   <Name>Alice</Name>
//   <Age>30</Age>
// </Root>
```

---

## Core Concepts

### EXml Builder

The `EXml` class uses a fluent API to configure serializers and deserializers:

```csharp
EXml exml = EXml.Create()
    .WithPrimitiveTypesAndStringSerialization()
    .WithCommonTypesSerialization()
    .WithObjectSerialization();
```

**Convenience method:**

```csharp
// Equivalent to the above
EXml exml = EXml.CreateWithDefaultSerialization();
```

### Serializers and Deserializers

- **Element Serializers/Deserializers**: Serialize/deserialize data to/from XML elements.
- **Attribute Serializers/Deserializers**: Serialize/deserialize data to/from XML attributes.

You can register custom implementations or use built-in ones.

**Access registries:**

```csharp
exml.ElementSerializers.AddFirst(myCustomSerializer);
exml.ElementDeserializers.AddLast(myCustomDeserializer);
exml.AttributeSerializers.AddFirst(myAttributeSerializer);
exml.AttributeDeserializers.AddLast(myAttributeDeserializer);
```

### Type Mapping for Polymorphic Collections

When serializing heterogeneous collections (e.g., `List<object>`), you can specify element names per type:

```csharp
List<object> source = new() { 1, 2.5, 3 };
XElement element = new XElement("Root");

EXml exml = EXml.Create()
    .WithPrimitiveTypesAndStringSerialization()
    .WithObjectSerialization();

exml.ElementSerializers.AddFirst(
    new ListAndArraySerializer()
        .WithTypeMappingOptions(opt => opt
            .With<int>("int")
            .With<double>("double"))
);

exml.Serialize(source, element);

// Result:
// <Root>
//   <int>1</int>
//   <double>2.5</double>
//   <int>3</int>
// </Root>
```

---

## Advanced Usage

### Custom Property Serialization

Use `ObjectSerializer` and `ObjectDeserializer` to customize per-property behavior:

```csharp
using EXmlLib2.Implementations.TypeSerialization.PropertyBased;

EXml exml = EXml.Create().WithPrimitiveTypesAndStringSerialization();

var serializer = new ObjectSerializer()
    .WithAcceptedType<Person>()
    .WithTypeOptions<Person>(opts => opts
        .WithIgnoredProperty(p => p.Age)
    );

exml.ElementSerializers.AddFirst(serializer);
```

### Custom Instance Factory

Control how objects are instantiated during deserialization:

```csharp
var deserializer = new ObjectDeserializer()
    .WithAcceptedType<Person>()
    .WithTypeOptions<Person>(opts => opts
        .WithInstanceFactory(dict => new Person
        {
            Name = dict.Get<string>("Name"),
            Age = dict.Get<int>("Age")
        })
    );

exml.ElementDeserializers.AddFirst(deserializer);
```

### Controlling XML Output (Attributes vs Elements)

You can specify whether types should be serialized as attributes or elements:

```csharp
EXml exml = EXml.Create()
    .WithPrimitiveTypesAndStringSerialization(EXml.XmlSupport.Elements) // Only elements
    .WithCommonTypesSerialization(EXml.XmlSupport.Attributes);          // Only attributes
```

Available options:
- `XmlSupport.Attributes` — serialize to XML attributes
- `XmlSupport.Elements` — serialize to XML elements
- `XmlSupport.AttributesAndElements` (default) — register both

---

## Configuration Options

### Null String Representation

Customize how `null` values are represented in XML:

```csharp
EXml exml = EXml.Create();
exml.DefaultNullString = "NULL"; // default is "(# null #)"
```

### Custom Culture Info

Control number and date formatting:

```csharp
exml.DefaultCultureInfo = CultureInfo.GetCultureInfo("cs-CZ");
```

### Custom Boolean String Values

```csharp
exml.DefaultTrueString = "yes";
exml.DefaultFalseString = "no";
```

---

## Architecture

### Key Interfaces

- **`IElementSerializer`** / **`IElementDeserializer`**: Handle element-based serialization.
- **`IAttributeSerializer`** / **`IAttributeDeserializer`**: Handle attribute-based serialization.
- **`IXmlContext`**: Provides access to registered serializers/deserializers and configuration (null strings, culture info, etc.).

### Registry System

Serializers and deserializers are registered in a `SerializerDeserializerRegistry<T>` and selected dynamically based on the type being serialized/deserialized. The registry is a chain-of-responsibility pattern where the first matching serializer/deserializer is used.

**Order matters:**
- `AddFirst()` — insert at the beginning (higher priority)
- `AddLast()` — append at the end (lower priority)

---

## Testing

The `EXmlLib2Test` project contains comprehensive unit tests demonstrating:
- Primitive type serialization
- Nullable types
- Collections (arrays, lists)
- Custom objects
- Type mapping for polymorphic collections

Run tests using:

```bash
dotnet test
```

---

## Project Structure

```
EXmlLib2/
??? Abstractions/            # Core interfaces and abstract base classes
??? Implementations/
?   ??? BasicSerialization/  # Built-in serializers for primitives, strings, etc.
?   ??? EnumerableSerialization/  # Array and List serializers/deserializers
?   ??? TypeSerialization/   # Object (property-based) serializers
??? Types/                   # Internal types and helpers
??? EXml.cs                  # Main entry point and fluent API
```

---

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests for bug fixes, features, or documentation improvements.

### Development Guidelines

- Target .NET 8
- Follow existing code style (C# 12.0)
- Add unit tests for new features
- Update this README when adding major features

---

## License

This project is part of the **eSystem.NET** framework. See the repository root for license information.

---

## Contact

For questions or feedback, please open an issue on [GitHub](https://github.com/Engin1980/eSystem.NET).

---

## Acknowledgments

Part of the **eSystem.NET** framework by Engin1980.

---

## See Also

- **EXmlLib** — Previous version of the XML serialization library (also in this repository)
- [System.Xml.Linq Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq)

---

## Version History

- **Current** — Property-based object serialization, fluent API, type mapping for collections
- **Legacy (EXmlLib)** — Original implementation with factory-based deserialization
