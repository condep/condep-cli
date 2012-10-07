﻿using System;
using System.Runtime.Serialization;

namespace ConDep.Console
{
    public class ConDepAssemblyNotFoundException : Exception
    {
        public ConDepAssemblyNotFoundException() {}

        public ConDepAssemblyNotFoundException(string message) : base(message) {}

        public ConDepAssemblyNotFoundException(string message, Exception innerException) : base(message, innerException) {}

        public ConDepAssemblyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}