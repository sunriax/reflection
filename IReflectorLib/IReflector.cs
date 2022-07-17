using System;

namespace IReflectorLib
{
    public interface IReflector
    {
        string Key { get; }
        string Message();
    }
}
