using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
  // create terrain meshes

  public static MeshData GenerateMesh(float[,] heightMap, float tileSize, float heightMultiplier, AnimationCurve heightCurve) {
    int meshSize = heightMap.GetLength(0);
    float halfWidth = .5f * meshSize;
    MeshData meshdata = new MeshData(meshSize);
    int vertexI = 0;

    for(int y = 0; y < meshSize; y++) {
      for(int x = 0; x < meshSize; x++) {
        Vector3 vertexPos = new Vector3((x - halfWidth), heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, (y - halfWidth)) * tileSize;
        Vector2 uv = new Vector2((float)x / meshSize, (float)y / meshSize);
        meshdata.AddVertexAndUv(vertexPos, uv, vertexI);

        if(y > 0 && x > 0) {
          AddTrianglesAtIndex(ref meshdata, meshSize, vertexI);
        }

        vertexI++;
      }
    }

    return meshdata;
  }

  public static MeshData GenerateFlatMesh(int meshSize, float tileSize) {
    float halfWidth = .5f * meshSize;
    MeshData meshdata = new MeshData(meshSize);
    int vertexI = 0;

    for(int y = 0; y < meshSize; y++) {
      for(int x = 0; x < meshSize; x++) {
        Vector3 vertexPos = new Vector3((x - halfWidth), 0, (y - halfWidth)) * tileSize;
        Vector2 uv = new Vector2((float)x / meshSize, (float)y / meshSize);
        meshdata.AddVertexAndUv(vertexPos, uv, vertexI);

        if(y > 0 && x > 0) {
          AddTrianglesAtIndex(ref meshdata, meshSize, vertexI);
        }

        vertexI++;
      }
    }

    return meshdata;
  }

  public static MeshData GenerateWaterMesh(int meshSize, float tileSize, int lodLevel) {
    if(lodLevel < 1 || (meshSize - 1) % lodLevel != 0) {
      Debug.LogWarning($"WaterLayer LOD level {lodLevel} is invalid! using value of 1 for now");
      lodLevel = 1;
    }

    int adjustedMeshSize = Mathf.RoundToInt(((float)(meshSize - 1) / lodLevel) + 1);

    float halfWidth = .5f * meshSize;
    MeshData meshdata = new MeshData(adjustedMeshSize);
    int vertexI = 0;

    for(int y = 0; y < meshSize; y+=lodLevel) {
      for(int x = 0; x < meshSize; x+=lodLevel) {
        Vector3 vertexPos = new Vector3((x - halfWidth), 0, (y - halfWidth)) * tileSize;
        Vector3 normal = Vector3.up;
        Vector2 uv = new Vector2((float)x / meshSize, (float)y / meshSize);
        meshdata.AddVertexAndUv(vertexPos, uv, vertexI);
        meshdata.AddNormal(normal, vertexI);

        if(y > 0 && x > 0) {
          AddTrianglesAtIndex(ref meshdata, adjustedMeshSize, vertexI);
        }

        vertexI++;
      }
    }

    return meshdata;
  }

  static void AddTrianglesAtIndex(ref MeshData meshData, int size, int vertexI) {
    meshData.AddTriangle(
      vertexI - size,
      vertexI - size - 1,
      vertexI - 1
    );

    meshData.AddTriangle(
      vertexI - 1,
      vertexI,
      vertexI - size
    );
  }
}

public struct MeshData {
  Vector3[] vertices;
  Vector3[] normals;
  Vector2[] uvs;
  int[] triangles;

  int triangleI;

  public MeshData(int meshSize) {
    int vertexCount = meshSize * meshSize;
    int triangleCount = (meshSize - 1) * (meshSize - 1) * 6;

    this.vertices = new Vector3[vertexCount];
    this.normals = new Vector3[vertexCount];
    this.uvs = new Vector2[vertexCount];
    this.triangles = new int[triangleCount];
    this.triangleI = 0;
  }

  public void AddVertexAndUv(Vector3 vertex, Vector2 uv, int vertexI) {
    vertices[vertexI] = vertex;
    uvs[vertexI] = uv;
  }

  public void AddNormal(Vector3 normal, int vertexI) {
    normals[vertexI] = normal;
  }

  public void AddTriangle(int pointA, int pointB, int pointC) {
    triangles[triangleI] = pointA;
    triangles[triangleI + 1] = pointB;
    triangles[triangleI + 2] = pointC;

    triangleI += 3;
  }

  public Mesh CreateMesh(bool autoNormals) {
    Mesh mesh = new Mesh();
    mesh.vertices = vertices;
    mesh.uv = uvs;
    mesh.triangles = triangles;

    if(autoNormals) {
      mesh.RecalculateNormals();

    } else {
      mesh.normals = normals;
    }

    return mesh;
  }
}
