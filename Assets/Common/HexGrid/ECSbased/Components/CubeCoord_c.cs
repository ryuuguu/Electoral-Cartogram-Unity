using System;
using Unity.Entities;
using Unity.Mathematics;

// ReSharper disable once InconsistentNaming
[GenerateAuthoringComponent]
public struct CubeCoord_c : IComponentData
{
    public float3 Value;
}

