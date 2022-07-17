using System;

namespace TestReflectorLib
{
    public class TestReflector
    {
        // If the empty constructor should not be visible,
        // it is possible to make it private
        //private TestReflector()
        //{
        //    // It is not allowed to throw an exception in the parameterless constructor
        //    throw new NotImplementedException();
        //}

        public TestReflector()
        {
            // It is not allowed to throw an exception in the parameterless constructor
            throw new NotImplementedException();
        }
    }
}
