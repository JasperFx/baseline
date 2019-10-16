using System;

namespace BaselineTypeDiscovery
{
    [Flags]
    public enum TypeClassification : short
    {
        /// <summary>
        /// All types
        /// </summary>
        All = 0,
        
        /// <summary>
        /// Open generic types
        /// </summary>
        Open = 1,
        
        /// <summary>
        /// "Closed" or non-generic types
        /// </summary>
        Closed = 2,
        
        /// <summary>
        /// Only Interface types
        /// </summary>
        Interfaces = 4,
        
        /// <summary>
        /// Abstract classes
        /// </summary>
        Abstracts = 8,
        
        /// <summary>
        /// Concrete classes
        /// </summary>
        Concretes = 16
    }
}