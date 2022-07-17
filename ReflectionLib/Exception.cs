using System;
using System.Collections.Generic;
using System.Text;
using RaGae.ExceptionLib;

namespace RaGae.ReflectionLib
{
    public enum ErrorCode
    {
        OK,
        DIRECTORY_NOT_FOUND,
        ASSEMBLIES_NOT_FOUND,
        MISSING_FILES,
        MISSING_CONFIG,
        EMPTY_LIST,
        EMPTY_CONFIG,
        INVALID_PROPERTY,
        INVALID_INSTANCE,
        INSTANCE_ERROR,
        TEST
    }

    public class ReflectionException : BaseException<ErrorCode>
    {
        public ReflectionException(ErrorCode errorCode) : base(errorCode) { }
        public ReflectionException(ErrorCode errorCode, string errorMessage) : base(errorCode, errorMessage) { }

        public override string ErrorMessage()
        {
            switch (ErrorCode)
            {
                case ErrorCode.OK:
                    return "TILT: Should not be reached!";
                case ErrorCode.DIRECTORY_NOT_FOUND:
                    return $"Directory <{base.Message}> not found!";
                case ErrorCode.ASSEMBLIES_NOT_FOUND:
                    return $"Assemblyfile <{base.Message}> not found!";
                case ErrorCode.MISSING_FILES:
                    return $"Directory <{base.Message}> contains no assemblies!";
                case ErrorCode.EMPTY_LIST:
                    return $"Assemblyfile list is NULL or EMPTY";
                case ErrorCode.INVALID_PROPERTY:
                    return $"PropertyName <{base.Message}> is null!";
                case ErrorCode.INVALID_INSTANCE:
                    return $"PropertyName <{base.Message}> not found!";
                case ErrorCode.INSTANCE_ERROR:
                    return $"Instance with arguments not found!";
                case ErrorCode.EMPTY_CONFIG:
                    return $"Config <{base.Message}> seems to be empty!";
                case ErrorCode.MISSING_CONFIG:
                    return $"Config <{base.Message}> file not found!";
                default:
                    return string.Empty;
            }
        }
    }
}
