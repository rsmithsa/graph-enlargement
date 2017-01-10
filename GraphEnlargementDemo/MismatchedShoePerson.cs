//-----------------------------------------------------------------------
// <copyright file="MismatchedShoePerson.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargementDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A person with differing left and right shoe sizes.
    /// </summary>
    internal class MismatchedShoePerson
    {
        /// <summary>
        /// Gets or sets the name of the person.
        /// </summary>
        /// <value>The name of the person.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the size of the left shoe.
        /// </summary>
        /// <value>The size of the left shoe.</value>
        public int LeftSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the right shoe.
        /// </summary>
        /// <value>The size of the right shoe.</value>
        public int RightSize { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{this.Name} [{this.LeftSize}, {this.RightSize}]";
        }
    }
}
