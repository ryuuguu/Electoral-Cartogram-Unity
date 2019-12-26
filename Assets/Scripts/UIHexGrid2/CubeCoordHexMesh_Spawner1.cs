using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Com.Ryuuguu.HexGridCC;
using Unity.Rendering;

public class CubeCoordHexMesh_Spawner : MonoBehaviour {
    
    
    public Transform holder;
    public GameObject prefab;
    public Mesh mesh;
    public CubeCoordinates.LocalSpace.Orientation orientation;
    public Vector2 offsetCoord;
    public Vector2 scaleV2 = Vector2.one;
    public Vector3 rotation;
    public Vector3 offsetWorldSpace;

    public float gameScale = 1;


    private CubeCoordinates.LocalSpace localSpace;
    
    public int radius = 1;
    void Start() {
        var localSpaceId =  CubeCoordinates.NewLocalSpaceId(gameScale, scaleV2, orientation, holder,offsetCoord);
        localSpace = CubeCoordinates.GetLocalSpace(localSpaceId);
        Hex(radius);
    }

    public void Hex(int aRadius) {
        // Create entity prefab from the game object hierarchy once
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        float sum = aRadius * 3;
        
        for (int x = -aRadius; x <= aRadius; x++) {
            for (int y = -aRadius; y <= aRadius; y++) {
                for (int z = -aRadius; z <= aRadius; z++) {
                    if ((x + y + z) == 0) {
                        // Efficiently instantiate a bunch of entities from the already converted entity prefab
                        var instance = entityManager.Instantiate(entity);
                        entityManager.SetComponentData(instance, new CubeCoord_c {Value = new float3(x,y,z)});
                        // Place the instantiated entity in a grid with some noise
                        var position =CubeCoordinates.ConvertPlaneToLocalPosition(new Vector2(x,y),localSpace) ;
                        entityManager.SetComponentData(instance, new Translation {Value = position+ offsetWorldSpace});
                        entityManager.SetComponentData(instance, new Rotation {Value = Quaternion.Euler(rotation)});
                        entityManager.AddComponentData(instance, new Scale {Value = gameScale});
                        var material = new Material(Shader.Find("Standard"));
                        float xa = Mathf.Abs(x);
                        float ya = Mathf.Abs(y);
                        float za = Mathf.Abs(z);
                        material.color = new Color(xa/sum,ya/sum,za/sum);
                        if (y ==0 && z == 0) {
                            material.color = new Color(1,1,1);
                        }
                        entityManager.SetComponentData(instance,
                            new HexMeshRenderer {Mesh = mesh, Material = material});
                            
                    }
                }
            }
        }
    }
}
