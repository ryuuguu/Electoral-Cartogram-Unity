using System;
using Unity.Entities;
using Unity.Mathematics;

// ReSharper disable once InconsistentNaming
[GenerateAuthoringComponent]
///
///  action component
///    change the state to this
/// 
public struct Highlight_c : IComponentData
{
    public bool Value;
}

