using System;

namespace AbstractReflectorLib
{
    public abstract class AbstractReflector
    {
        public abstract string Key { get; }
        public abstract string Message();
    }
}
