using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Unity.Transforms;

[ConverterVersion("joe", 2)]
class HexMeshAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Mesh Mesh = null;
    public Color Color = Color.blue;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Assets in subscenes can either be created during conversion and embedded in the scene
        var material = new Material(Shader.Find("Standard"));
        material.color = Color;
        // ... Or be an asset that is being referenced.
        dstManager.AddComponentData(entity, new HexMeshRenderer
        {
            Mesh = Mesh,
            Material = material, 
        });
    }
}



public class HexMeshRenderer : IComponentData {
    public Mesh     Mesh;
    public Material Material;
}

[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
class HexMeshRendererSystem : ComponentSystem
{
    override protected void OnUpdate()
    {
        Entities.ForEach((HexMeshRenderer renderer, ref LocalToWorld localToWorld) =>
        {
            Graphics.DrawMesh(renderer.Mesh, localToWorld.Value, renderer.Material, 0);
        });
    }
}