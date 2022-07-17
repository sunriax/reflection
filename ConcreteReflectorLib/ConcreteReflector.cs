using AbstractReflectorLib;
using System;

namespace ConcreteReflectorLib
{
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
}
