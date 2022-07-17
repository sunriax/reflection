[![Version: 1.0 Release](https://img.shields.io/badge/Version-1.0%20Release-green.svg)](https://github.com/sunriax) [![NuGet](https://img.shields.io/nuget/dt/ragae.reflection.svg)](https://www.nuget.org/packages/ragae.reflection) [![Build Status](https://www.travis-ci.com/sunriax/reflection.svg?branch=main)](https://www.travis-ci.com/sunriax/reflection) [![codecov](https://codecov.io/gh/sunriax/reflection/branch/main/graph/badge.svg)](https://codecov.io/gh/sunriax/reflection) [![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

# ReflectionLib

## Description:

Loading DLL´s at runtime within a .net Framework/Core application. An example project (MakeReflection) can be found in the repository.

---

## Installation


To install ReflectionLib it is possible to download library [[zip](https://github.com/sunriax/reflection/releases/latest/download/Reflection.zip) | [gzip](https://github.com/sunriax/reflection/releases/latest/download/Reflection.tar.gz)] or install it via nuget.

```
PM> Install-Package RaGae.Reflection
```

After adding/installing the ReflectionLib in a project classes of same base or same type or different classes can be loaded dynamically at runtime.

---

## Structure

### **Initialize with config**

``` csharp
Reflection r = new Reflection("Path to config", "section number");
```

**`ReflectionLib.json`**

``` yaml
{
  "ReflectionConfig": [
    {
      "ReflectionPath": "Reflection",
      "FileSpecifier": "*ReflectorLib.dll",
      "Files": [
        "Reflection/ConcreteReflectorLib.dll"
      ]
    },
    {
      "ReflectionPath": "IReflection",
      "FileSpecifier": "*IReflectorLib.dll",
      "Files": [
        "IReflection/ConcreteIReflectorLib.dll"
      ]
    }
  ]
}
```

### **Initialize with directory**

``` csharp
Reflection r = new Reflection("Path to dll files", "Ending of filenames");
```

### **Initialize with filenames**

``` csharp
string[] files = { "file1.dll", "file2.dll" };

Reflection r = new Reflection(files);
```

---

## Parameter

### **ConfigPath**

Path to config file

``` csharp
Reflection r = new Reflection("ReflectionLib.json", "...");
```

### **Section**

Select section in **?.json** file

``` csharp
Reflection r = new Reflection("...", 0);
```

**`ReflectionLib.Files.json`**

``` yaml
{
  "ReflectionConfig": [
    {
      "Files": [
        "Reflection/ConcreteReflectorLib.dll"
      ]
    },
    {
      "Files": [
        "IReflection/ConcreteIReflectorLib.dll"
      ]
    }
  ]
}
```

**`ReflectionLib.Path.json`**

``` yaml
{
  "ReflectionConfig": [
    {
      "ReflectionPath": "Reflection",
      "FileSpecifier": "*ReflectorLib.dll"
    },
    {
      "ReflectionPath": "IReflection",
      "FileSpecifier": "*IReflectorLib.dll"
    }
  ]
}
```

### **LibraryPath**

Path to DLL files.

``` csharp
Reflection r = new Reflection(@"Reflection", "...");
```

### **FileSpecifier**

Description of file ending.

``` csharp
Reflection r = new Reflection("...", "*ReflectorLib.dll");
```

### **LibraryFile**

Include path for predefined libraries

``` csharp
string[] files = { "file1.dll", "file2.dll" };

Reflection r = new Reflection(files);
```

---

## Load instance by property

### **With abstract class**

``` csharp
static void Main(string[] args)
{
  Reflection r = new Reflection(@"Reflection", "*ReflectorLib.dll");

  // Use standard constructor of class
  AbstractReflector a1 = r.GetInstanceByProperty<AbstractReflector>(nameof(a1.Key), "Lib1");

  // Inject data into constructor of class
  AbstractReflector a2 = r.GetInstanceByProperty<AbstractReflector>(nameof(a2.Key), "Lib1", new object[] { "Injected constructor parameter" });

  // Call functions
  a1.?
  a2.?
}
```

### **With interface**

``` csharp
static void Main(string[] args)
{
  Reflection r = new Reflection(@"IReflection", "*IReflectorLib.dll");

  // Use standard constructor of class
  IReflector a1 = r.GetInstanceByProperty<IReflector>(nameof(a1.Key), "Lib1");

  // Inject data into constructor of class
  IReflector a2 = r.GetInstanceByProperty<IReflector>(nameof(a2.Key), "Lib1", new object[] { "Injected constructor parameter" });

  // Call functions
  a1.?
  a2.?
}
```

---

## Build your own class

1. Create a new VisualStudio .NET Standard class library (**??ReflectorLib**)
1. If more than one class of same functionallity will be used, create an abstract class or an interface.

### **With an abstract class**

#### **`AbstractReflector.cs`**
``` csharp
public abstract class AbstractReflector
{
    public abstract string Key { get; }
    public abstract string Message();
}
```

#### **`ConcreteReflector.cs`**
``` csharp
public class ConcreteReflector : AbstractReflector
{
    private readonly string message;

    // If the empty constructor should not be visible,
    // it is possible to make it private
    //private ConcreteReflector()
    //{
    //    this.message = "Constructor without parameter";
    //}
    public ConcreteReflector()
    {
        this.message = "Constructor without parameter";
    }

    public ConcreteReflector(string message)
    {
        this.message = message;
    }

    private const string key = "Lib1";

    public override string Key { get => key; }

    public override string Message()
    {
        return message;
    }
}
```

### **With an interface**

#### **`IReflector.cs`**
``` csharp
public interface IReflector
{
    string Key { get; }
    string Message();
}
```

#### **`ConcreteReflector.cs`**
``` csharp
public class ConcreteIReflector : IReflector
{
    private readonly string message;

    // If the empty constructor should not be visible,
    // it is possible to make it private
    //private ConcreteIReflector()
    //{
    //    this.message = "Constructor without parameter";
    //}
    public ConcreteIReflector()
    {
        this.message = "Constructor without parameter";
    }

    public ConcreteIReflector(string message)
    {
        this.message = message;
    }

    private const string key = "Lib1";

    public string Key { get => key; }

    public string Message()
    {
        return this.message;
    }
}
```

---

## Exception handling

All libraries in the RaGae.* namespace are using the [RaGae.Exception](https://github.com/sunriax/exception) model. The model implements an error code and message.

``` csharp
static void Main(string[] args)
{
  
  try
  {
    Reflection r5 = new Reflection(@"Reflection", "*ReflectorLib.dll");
    // ...
  }
  catch (ReflectionException ex)
  {
      Console.WriteLine(ex.ErrorCode);
      Console.WriteLine(ex.Message);
      Console.WriteLine(ex.ErrorMessage());
  }
  catch (Exception ex)
  {
      Console.WriteLine(ex.Message);
  }
}
```

---

R. GÄCHTER