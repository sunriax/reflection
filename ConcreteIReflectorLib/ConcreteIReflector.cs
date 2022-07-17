using IReflectorLib;
using System;

namespace ConcreteIReflectorLib
{
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
}
