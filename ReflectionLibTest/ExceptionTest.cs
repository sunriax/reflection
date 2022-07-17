using RaGae.ReflectionLib;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ReflectorLibTest
{
    public class ExceptionTest
    {
        private const string testArgument = "argument";
        private const string testParameter = "parameter";

        public static IEnumerable<object[]> GetExceptionType()
        {
            yield return new object[] {
                ErrorCode.OK,
                null,
                "TILT: Should not be reached!"
            };

            yield return new object[] {
                ErrorCode.DIRECTORY_NOT_FOUND,
                testArgument,
                $"Directory <{testArgument}> not found!"
            };

            yield return new object[] {
                ErrorCode.ASSEMBLIES_NOT_FOUND,
                testArgument,
                $"Assemblyfile <{testArgument}> not found!"
            };

            yield return new object[] {
                ErrorCode.MISSING_FILES,
                testArgument,
                $"Directory <{testArgument}> contains no assemblies!"
            };

            yield return new object[] {
                ErrorCode.EMPTY_LIST,
                null,
                $"Assemblyfile list is NULL or EMPTY"
            };

            yield return new object[] {
                ErrorCode.INVALID_PROPERTY,
                testArgument,
                $"PropertyName <{testArgument}> is null!"
            };

            yield return new object[] {
                ErrorCode.INVALID_INSTANCE,
                testArgument,
                $"PropertyName <{testArgument}> not found!"
            };

            yield return new object[] {
                ErrorCode.INSTANCE_ERROR,
                null,
                $"Instance with arguments not found!"
            };

            yield return new object[] {
                ErrorCode.EMPTY_CONFIG,
                testArgument,
                $"Config <{testArgument}> seems to be empty!"
            };

            yield return new object[] {
                ErrorCode.MISSING_CONFIG,
                testArgument,
                $"Config <{testArgument}> file not found!"
            };

            yield return new object[] {
                ErrorCode.TEST,
                null,
                string.Empty
            };
        }

        [Theory]
        [MemberData(nameof(GetExceptionType))]
        public void CreateExceptionWithErrorCodes_Passing(ErrorCode code, string argument, string message)
        {
            ReflectionException ex = new ReflectionException(code, argument);

            Assert.Equal(code, ex.ErrorCode);

            if (argument == null)
                Assert.Equal("Exception of type 'RaGae.ReflectionLib.ReflectionException' was thrown.", ex.Message);
            else
                Assert.Equal(argument, ex.Message);


            Assert.Equal(message, ex.ErrorMessage());
        }
    }
}
