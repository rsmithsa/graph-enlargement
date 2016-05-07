graph-enlargement
=================

Introduction
------------
This repository contains various C# implementations for enlarging graphs to ensure that all nodes are contained in
cycles. These implementations form part of my BSc. (Hons) research project. Currently the repository consists of
re-implementations of existing algorithms which are to form the comparison baseline for new algorithms developed as part
of the project.

Algorithms
----------
* Sanders:

    I. Sanders. Cooperating to buy shoes: An application of picking cycles in directed graphs.
In Proceedings of the South African Institute of Computer Scientists and Information Technologists (Theme: "A Connected
Society"), pages 8-16, East London, 2013.
    
    * First Pass
    
        Removal of isolated nodes, nodes with only in-edges and nodes with only out-edges.
        
    * Second Pass
    
        Removal of "bridge" nodes.
        
* Van der Linde:

    J. van der Linde, I. Sanders. Enlarging Directed Graphs To Ensure All Nodes Are Contained In Cycles.
In Proceedings of the South African Institute of Computer Scientists and Information Technologists, 2015.
    
    * Cost-optimised
        
        Improved version of Sanders' First Pass which minimizes the number of additional nodes added to the graph, followed
    by Sanders' Second Pass to remove "bridge" nodes.
    
    * Subgraph
        
        Creation of a subgraph consisting of non-optimal nodes before applying the Cost-optimised algorithm to the subgraph
    only.
    
    * Permutation-matrix
        
        Transformation of the graph adjacency matrix to a permutation matrix to perform enlargement.