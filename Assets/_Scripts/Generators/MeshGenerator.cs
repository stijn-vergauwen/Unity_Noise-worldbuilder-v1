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
  Vector2[] uvs;
  int[] triangles;

  int triangleI;

  public MeshData(int meshSize) {
    int vertexCount = meshSize * meshSize;
    int triangleCount = (meshSize - 1) * (meshSize - 1) * 6;

    this.vertices = new Vector3[vertexCount];
    this.uvs = new Vector2[vertexCount];
    this.triangles = new int[triangleCount];
    this.triangleI = 0;
  }

  public void AddVertexAndUv(Vector3 vertex, Vector2 uv, int vertexI) {
    vertices[vertexI] = vertex;
    uvs[vertexI] = uv;
  }

  public void AddTriangle(int pointA, int pointB, int pointC) {
    triangles[triangleI] = pointA;
    triangles[triangleI + 1] = pointB;
    triangles[triangleI + 2] = pointC;

    triangleI += 3;
  }

  public Mesh CreateMesh() {
    Mesh mesh = new Mesh();
    mesh.vertices = vertices;
    mesh.uv = uvs;
    mesh.triangles = triangles;

    mesh.RecalculateNormals();

    return mesh;
  }
}
