using AbstractReflectorLib;
using IReflectorLib;
using RaGae.ReflectionLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestReflectorLib;
using Xunit;
using Xunit.Sdk;

namespace ReflectorLibTest
{
    public enum ReflectionConstructor
    {
        WithConfig,
        WithPath,
        WithFiles
    }

    public class ReflectionConstructorData
    {
        public IEnumerable<string> Config { get; set; }
        public int Section { get; set; }
        public string Path { get; set; }
        public string Specifier { get; set; }
        public IEnumerable<string> Files { get; set; }
        public Type Type { get; set; }
        public IEnumerable<object> Parameters { get; set; }
    }

    public class ReflectorTest
    {
        private static string[] config = { "ReflectionLib.json", "ReflectionLib.Path.json", "ReflectionLib.Files.json" };
        private static string emptyConfig = "ReflectionLib.Empty.json";
        private static string testConfig = "ReflectionLib.Test.json";

        private static string testReflector = @"TestReflection";
        private static string reflector = @"Reflection";
        private static string ireflector = @"IReflection";

        private static string fileSpecifier = "*ReflectorLib.dll";

        public static IEnumerable<object[]> GetConstructorTypes()
        {
            yield return new object[] { ReflectionConstructor.WithConfig };
            yield return new object[] { ReflectionConstructor.WithPath };
            yield return new object[] { ReflectionConstructor.WithFiles };
        }

        private Reflection CreateConstructor_Passing(ReflectionConstructor type, ReflectionConstructorData data)
        {
            Reflection r = null;

            switch (type)
            {
                case ReflectionConstructor.WithConfig:
                    foreach (string config in data.Config)
                        r = new Reflection(config, data.Section);
                    break;
                case ReflectionConstructor.WithPath:
                    r = new Reflection(data.Path, data.Specifier);
                    break;
                case ReflectionConstructor.WithFiles:
                    r = new Reflection(data.Files);
                    break;
                default:
                    throw new XunitException("TILT: should not be reached!");
            }
            return r;
        }

        public IEnumerable<ReflectionConstructorData> GetDifferentReflectors()
        {
            yield return new ReflectionConstructorData()
            {
                Config = config,
                Section = 0,
                Path = reflector,
                Specifier = fileSpecifier,
                Files = Directory.GetFiles(reflector),
                Type = typeof(AbstractReflector),
                Parameters = new List<string>() { null }
            };

            yield return new ReflectionConstructorData()
            {
                Config = config,
                Section = 0,
                Path = reflector,
                Specifier = fileSpecifier,
                Files = Directory.GetFiles(reflector),
                Type = typeof(AbstractReflector),
                Parameters = new List<string>() { "Injected constructor parameter" }
            };

            yield return new ReflectionConstructorData()
            {
                Config = config,
                Section = 1,
                Path = ireflector,
                Specifier = fileSpecifier,
                Files = Directory.GetFiles(ireflector),
                Type = typeof(IReflector),
                Parameters = new List<string>() { null }
            };

            yield return new ReflectionConstructorData()
            {
                Config = config,
                Section = 1,
                Path = ireflector,
                Specifier = fileSpecifier,
                Files = Directory.GetFiles(ireflector),
                Type = typeof(IReflector),
                Parameters = new List<string>() { "Injected constructor parameter" }
            };
        }

        [Theory]
        [MemberData(nameof(GetConstructorTypes))]
        public void CreateReferenceWithDirectoryAndSpecifier_Passing(ReflectionConstructor type)
        {
            foreach (ReflectionConstructorData data in GetDifferentReflectors())
            {
                Reflection r = CreateConstructor_Passing(type, data);
                Assert.NotNull(r);
            }
        }

        public static IEnumerable<object[]> GetWrongConfigPath()
        {
            yield return new object[] { null, null};
            yield return new object[] { "", 0 };
            yield return new object[] { "   ", 0 };
            yield return new object[] { "WrongReflectorLib.json", 0 };
        }

        [Theory]
        [MemberData(nameof(GetWrongConfigPath))]
        public void CreateReferenceWithWrongConfigPath_Failing(string config, int section)
        {
            Reflection r;
            ReflectionException ex = Assert.Throws<ReflectionException>(() => r = new Reflection(config, section));

            Assert.Equal(ErrorCode.MISSING_CONFIG, ex.ErrorCode);
            Assert.Equal($"{config}:{section}", ex.Message);
            Assert.Equal($"Config <{config}:{section}> file not found!", ex.ErrorMessage());
        }

        [Fact]
        public void CreateReferenceWithEmptyConfig_Failing()
        {
            Reflection r;
            ReflectionException ex = Assert.Throws<ReflectionException>(() => r = new Reflection(emptyConfig, 0));

            Assert.Equal(ErrorCode.EMPTY_CONFIG, ex.ErrorCode);
            Assert.Equal($"{emptyConfig}:0", ex.Message);
            Assert.Equal($"Config <{emptyConfig}:0> seems to be empty!", ex.ErrorMessage());
        }

        public static IEnumerable<object[]> GetWrongDirectoryOrSpecifier()
        {
            yield return new object[] { null };
            yield return new object[] { "" };
            yield return new object[] { "   " };
            yield return new object[] { "test" };
            yield return new object[] { "*Test.dll" };
        }

        [Theory]
        [MemberData(nameof(GetWrongDirectoryOrSpecifier))]
        public void CreateReferenceWithWrongDirectoryAndSpecifier_Failing(string directory)
        {
            Reflection r;
            ReflectionException ex = Assert.Throws<ReflectionException>(() => r = new Reflection(directory, fileSpecifier));

            Assert.Equal(ErrorCode.DIRECTORY_NOT_FOUND, ex.ErrorCode);

            if (directory == null)
            {
                Assert.Equal("Exception of type 'RaGae.ReflectionLib.ReflectionException' was thrown.", ex.Message);
                Assert.Equal($"Directory <{ex.Message}> not found!", ex.ErrorMessage());
            }
            else
            {
                Assert.Equal(directory, ex.Message);
            }
            Assert.Equal($"Directory <{ex.Message}> not found!", ex.ErrorMessage());
        }

        [Theory]
        [MemberData(nameof(GetWrongDirectoryOrSpecifier))]
        public void CreateReferenceWithDirectoryAndWrongSpecifier_Failing(string specifier)
        {
            Reflection r;
            ReflectionException ex = Assert.Throws<ReflectionException>(() => r = new Reflection(reflector, specifier));

            Assert.Equal(ErrorCode.MISSING_FILES, ex.ErrorCode);
            Assert.Equal($"{reflector}/{specifier}", ex.Message);
            Assert.Equal($"Directory <{reflector}/{specifier}> contains no assemblies!", ex.ErrorMessage());
        }

        public static IEnumerable<object[]> GetWrongFilePath()
        {
            yield return new object[] { null };
            yield return new object[] { new List<string>() { "" } };
            yield return new object[] { new List<string>() { "   " } };
            yield return new object[] { new List<string>() { "Test.dll" } };
        }

        [Theory]
        [MemberData(nameof(GetWrongFilePath))]
        public void CreateReferenceWithWrongFiles_Failing(List<string> file)
        {
            Reflection r;
            ReflectionException ex = Assert.Throws<ReflectionException>(() => r = new Reflection(file));

            if (file == null)
            {
                Assert.Equal(ErrorCode.EMPTY_LIST, ex.ErrorCode);
                Assert.Equal("Exception of type 'RaGae.ReflectionLib.ReflectionException' was thrown.", ex.Message);
                Assert.Equal($"Assemblyfile list is NULL or EMPTY", ex.ErrorMessage());
            }
            else
            {
                Assert.Equal(ErrorCode.ASSEMBLIES_NOT_FOUND, ex.ErrorCode);
                Assert.Equal(file.ElementAt(0), ex.Message);
                Assert.Equal($"Assemblyfile <{file.ElementAt(0)}> not found!", ex.ErrorMessage());
            }
        }

        [Theory]
        [MemberData(nameof(GetConstructorTypes))]
        public void CreateReferenceAndGetInstanceByProperty_Passing(ReflectionConstructor type)
        {
            foreach (ReflectionConstructorData data in GetDifferentReflectors())
            {
                Reflection r = CreateConstructor_Passing(type, data);
                Assert.NotNull(r);

                dynamic a;

                if (data.Type == typeof(AbstractReflector))
                {
                    a = r.GetInstanceByProperty<AbstractReflector>(nameof(a.Key), "Lib1", data.Parameters.ToArray<object>());
                }
                else if (data.Type == typeof(IReflector))
                {
                    a = r.GetInstanceByProperty<IReflector>(nameof(a.Key), "Lib1", data.Parameters.ToArray<object>());
                }
                else
                    throw new XunitException("TILT: should not be reached!");

                Assert.Equal("Lib1", a.Key);

                if (data.Parameters == null)
                    Assert.Equal("Constructor without parameter", a.Message());
                else
                    Assert.Equal(data.Parameters.ElementAt(0), a.Message());
            }
        }

        [Theory]
        [MemberData(nameof(GetConstructorTypes))]
        public void CreateReferenceAndGetInstanceByPropertyWithNullPropertyName_Failing(ReflectionConstructor type)
        {
            foreach (ReflectionConstructorData data in GetDifferentReflectors())
            {
                Reflection r = CreateConstructor_Passing(type, data);
                Assert.NotNull(r);

                ReflectionException ex;

                if (data.Type == typeof(AbstractReflector))
                {
                    AbstractReflector a;

                    ex = Assert.Throws<ReflectionException>(() => a = r.GetInstanceByProperty<AbstractReflector>(null, "Lib1", data.Parameters.ToArray<object>()));

                }
                else if (data.Type == typeof(IReflector))
                {
                    IReflector a;

                    ex = Assert.Throws<ReflectionException>(() => a = r.GetInstanceByProperty<IReflector>(null, "Lib1", data.Parameters.ToArray<object>()));
                }
                else
                    throw new XunitException("TILT: should not be reached!");

                Assert.Equal(ErrorCode.INVALID_PROPERTY, ex.ErrorCode);
                Assert.Equal("Exception of type 'RaGae.ReflectionLib.ReflectionException' was thrown.", ex.Message);
                Assert.Equal($"PropertyName <{ex.Message}> is null!", ex.ErrorMessage());
            }
        }

        [Theory]
        [MemberData(nameof(GetConstructorTypes))]
        public void CreateReferenceAndGetInstanceByPropertyWithNullPropertyValue_Failing(ReflectionConstructor type)
        {
            foreach (ReflectionConstructorData data in GetDifferentReflectors())
            {
                Reflection r = CreateConstructor_Passing(type, data);
                Assert.NotNull(r);

                ReflectionException ex;
                string propertyName;

                if (data.Type == typeof(AbstractReflector))
                {
                    AbstractReflector a;
                    propertyName = nameof(a.Key);

                    ex = Assert.Throws<ReflectionException>(() => a = r.GetInstanceByProperty<AbstractReflector>(nameof(a.Key), null, data.Parameters.ToArray<object>()));

                }
                else if (data.Type == typeof(IReflector))
                {
                    IReflector a;
                    propertyName = nameof(a.Key);

                    ex = Assert.Throws<ReflectionException>(() => a = r.GetInstanceByProperty<IReflector>(nameof(a.Key), null, data.Parameters.ToArray<object>()));
                }
                else
                    throw new XunitException("TILT: should not be reached!");

                Assert.Equal(ErrorCode.INVALID_INSTANCE, ex.ErrorCode);
                Assert.Equal($"{propertyName}:", ex.Message);
                Assert.Equal($"PropertyName <{propertyName}:> not found!", ex.ErrorMessage());
            }
        }

        [Theory]
        [MemberData(nameof(GetConstructorTypes))]
        public void CreateReferenceAndGetInstanceByPropertyWithWrongPropertyValue_Failing(ReflectionConstructor type)
        {
            foreach (ReflectionConstructorData data in GetDifferentReflectors())
            {
                Reflection r = CreateConstructor_Passing(type, data);
                Assert.NotNull(r);

                ReflectionException ex;
                string propertyName;

                if (data.Type == typeof(AbstractReflector))
                {
                    AbstractReflector a;
                    propertyName = nameof(a.Key);

                    ex = Assert.Throws<ReflectionException>(() => a = r.GetInstanceByProperty<AbstractReflector>(nameof(a.Key), "NotFound", data.Parameters.ToArray<object>()));

                }
                else if (data.Type == typeof(IReflector))
                {
                    IReflector a;
                    propertyName = nameof(a.Key);

                    ex = Assert.Throws<ReflectionException>(() => a = r.GetInstanceByProperty<IReflector>(nameof(a.Key), "NotFound", data.Parameters.ToArray<object>()));
                }
                else
                    throw new XunitException("TILT: should not be reached!");

                Assert.Equal(ErrorCode.INVALID_INSTANCE, ex.ErrorCode);
                Assert.Equal($"{propertyName}:NotFound", ex.Message);
                Assert.Equal($"PropertyName <{propertyName}:NotFound> not found!", ex.ErrorMessage());
            }
        }

        [Theory]
        [MemberData(nameof(GetConstructorTypes))]
        public void CreateReferenceAndGetInstanceByPropertyWithWrongArguments_Failing(ReflectionConstructor type)
        {
            foreach (ReflectionConstructorData data in GetDifferentReflectors())
            {
                Reflection r = CreateConstructor_Passing(type, data);
                Assert.NotNull(r);

                ReflectionException ex;
                string propertyName;

                if (data.Type == typeof(AbstractReflector))
                {
                    AbstractReflector a;
                    propertyName = nameof(a.Key);

                    ex = Assert.Throws<ReflectionException>(() => a = r.GetInstanceByProperty<AbstractReflector>(nameof(a.Key), "Lib1", new object[] { "To", "many", "arguments" }));

                }
                else if (data.Type == typeof(IReflector))
                {
                    IReflector a;
                    propertyName = nameof(a.Key);

                    ex = Assert.Throws<ReflectionException>(() => a = r.GetInstanceByProperty<IReflector>(nameof(a.Key), "Lib1", new object[] { "To", "many", "arguments" }));
                }
                else
                    throw new XunitException("TILT: should not be reached!");

                Assert.Equal(ErrorCode.INVALID_INSTANCE, ex.ErrorCode);
                Assert.Equal($"{propertyName}:Lib1", ex.Message);
                Assert.Equal($"PropertyName <{propertyName}:Lib1> not found!", ex.ErrorMessage());
            }
        }

        [Theory]
        [MemberData(nameof(GetConstructorTypes))]
        public void CreateReferenceAndGetInstanceByPropertyWithConstructorException_Failing(ReflectionConstructor type)
        {
            ReflectionConstructorData data = new ReflectionConstructorData()
            {
                Config = new List<string>() { testConfig },
                Section = 0,
                Path = testReflector,
                Specifier = fileSpecifier,
                Files = Directory.GetFiles(testReflector),
                Type = null,
                Parameters = null
            };

            Reflection r = CreateConstructor_Passing(type, data);
            Assert.NotNull(r);

            TestReflector a;
            ReflectionException ex = Assert.Throws<ReflectionException>(() => a = r.GetInstanceByProperty<TestReflector>("None", "None"));

            Assert.Equal(ErrorCode.INSTANCE_ERROR, ex.ErrorCode);
            Assert.Equal($"Exception of type 'RaGae.ReflectionLib.ReflectionException' was thrown.", ex.Message);
            Assert.Equal($"Instance with arguments not found!", ex.ErrorMessage());
        }
    }
}
