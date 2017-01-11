//-----------------------------------------------------------------------
// <copyright file="IGraphEnlargementVertex.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a graph enlargement vertex.
    /// </summary>
    public interface IGraphEnlargementVertex
    {
        ///// <summary>
        ///// Determines whether this vertex is redundant.
        ///// </summary>
        ///// <returns><c>true</c> if this vertex is redundant; otherwise, <c>false</c>.</returns>
        //bool IsRedundant();

        object GetKey();

        object GetTarget();
    }
}
