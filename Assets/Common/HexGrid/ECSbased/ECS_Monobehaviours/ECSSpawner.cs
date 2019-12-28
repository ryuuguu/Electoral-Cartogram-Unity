using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Com.Ryuuguu.HexGridCC;
using TMPro;
using Unity.Rendering;

public class ECSSpawner : MonoBehaviour {
    
    
    public Transform holder;
    public GameObject prefab;
    public Mesh mesh;
    public CubeCoordinates.LocalSpace.Orientation orientation;
    public Vector2 offsetCoord;
    public Vector2 scaleV2 = Vector2.one;
    public Vector3 rotation;
    public Vector3 offsetWorldSpace;

    public float gameScale = 1;


    static CubeCoordinates.LocalSpace localSpace;
    static Entity _entity;
    static EntityManager _entityManager;
    private static Vector3 _offsetWorldSpace;
    private static Vector3 _rotation;
    private static Mesh _mesh;
    private static float _gameScale;
    
    public int radius = 1;

    private static ECSSpawner inst;

    public void Start() {
       // TestInit();
    }

    public void  Awake() {
        inst = this;
        _offsetWorldSpace = offsetWorldSpace;
        _rotation = rotation;
        _mesh = mesh;
        _gameScale = gameScale;
    }
    
    public static  void Init(CubeCoordinates.LocalSpace aLocalSpace) {
        localSpace = aLocalSpace;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        _entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(inst.prefab, settings);
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public static void Hex(Vector3 coord, Color color) {
        var instance = _entityManager.Instantiate(_entity);
        //_entityManager.SetComponentData(instance, new CubeCoord_c {Value = new float3(coord)});
        // Place the instantiated entity in a grid with some noise
        var position =CubeCoordinates.ConvertPlaneToLocalPosition(coord,localSpace) ;
        position *= (_gameScale/ localSpace.gameScale);
        _entityManager.SetComponentData(instance, new Translation {Value = position+ _offsetWorldSpace});
        _entityManager.SetComponentData(instance, new Rotation {Value = Quaternion.Euler(_rotation)});
        _entityManager.AddComponentData(instance, new Scale {Value = _gameScale});
        var material = new Material(Shader.Find("Standard"));
        material.color = color;
        //_entityManager.SetComponentData(instance,
        //    new HexMeshRenderer {Mesh = _mesh, Material = material});

    }
    
    public void TestInit() {
        var localSpaceId =  CubeCoordinates.NewLocalSpaceId(gameScale, scaleV2, orientation, holder,offsetCoord);
        localSpace = CubeCoordinates.GetLocalSpace(localSpaceId);
        MegaHex(radius);
    }
    
    public void MegaHex(int aRadius) {
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
