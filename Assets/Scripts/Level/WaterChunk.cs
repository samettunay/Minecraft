using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterChunk : MonoBehaviour
{

  public GameObject worldGO;
  private World world;

  private List<Vector3> newVertices = new List<Vector3>();
  private List<int> newTriangles = new List<int>();
  private List<Vector2> newUV = new List<Vector2>();

  private float tUnit = 0.25f;
  private Vector2 tWater = new Vector2(0, 0);

  private Mesh mesh;
  private MeshCollider col;

  private int faceCount;

  public int chunkSize = 16;

  public int chunkX;
  public int chunkY;
  public int chunkZ;
  public bool update;

  // Use this for initialization
  void Start()
  {

    world = worldGO.GetComponent("World") as World;
    mesh = GetComponent<MeshFilter>().mesh;
    col = GetComponent<MeshCollider>();

    GenerateMesh();
  }

  public void GenerateMesh()
  {
    for (int x = 0; x < chunkSize; x++)
    {
      for (int y = 0; y < chunkSize; y++)
      {
        for (int z = 0; z < chunkSize; z++)
        {
          //This code will run for every block in the chunk

          if (Block(x, y, z) != 0)
          {
            if (Block(x, y + 1, z) == 0)
            {
              //Block above is air
              CubeTop(x, y, z);
            }
          }
        }
      }
    }
    UpdateMesh();
  }

  byte Block(int x, int y, int z)
  {
    return world.WaterBlock(x + chunkX, y + chunkY, z + chunkZ);
  }

  void CubeTop(int x, int y, int z)
  {

    newVertices.Add(new Vector3(x, y, z + 1));
    newVertices.Add(new Vector3(x + 1, y, z + 1));
    newVertices.Add(new Vector3(x + 1, y, z));
    newVertices.Add(new Vector3(x, y, z));

    Vector2 texturePos = new Vector2(0, 0);

    if (Block(x, y, z) == 4)
    {
      texturePos = tWater;
    }

    Cube(texturePos);

  }

  void CubeNorth(int x, int y, int z)
  {
    newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
    newVertices.Add(new Vector3(x + 1, y, z + 1));
    newVertices.Add(new Vector3(x, y, z + 1));
    newVertices.Add(new Vector3(x, y - 1, z + 1));

    Vector2 texturePos = new Vector2(0, 0);

    if (Block(x, y, z) == 4)
    {
      texturePos = tWater;
    }

    Cube(texturePos);

  }

  void CubeEast(int x, int y, int z)
  {
    newVertices.Add(new Vector3(x + 1, y - 1, z));
    newVertices.Add(new Vector3(x + 1, y, z));
    newVertices.Add(new Vector3(x + 1, y, z + 1));
    newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

    Vector2 texturePos = new Vector2(0, 0);

    if (Block(x, y, z) == 4)
    {
      texturePos = tWater;
    }

    Cube(texturePos);

  }

  void CubeSouth(int x, int y, int z)
  {
    newVertices.Add(new Vector3(x, y - 1, z));
    newVertices.Add(new Vector3(x, y, z));
    newVertices.Add(new Vector3(x + 1, y, z));
    newVertices.Add(new Vector3(x + 1, y - 1, z));

    Vector2 texturePos = new Vector2(0, 0);

    if (Block(x, y, z) == 4)
    {
      texturePos = tWater;
    }

    Cube(texturePos);

  }

  void CubeWest(int x, int y, int z)
  {
    newVertices.Add(new Vector3(x, y - 1, z + 1));
    newVertices.Add(new Vector3(x, y, z + 1));
    newVertices.Add(new Vector3(x, y, z));
    newVertices.Add(new Vector3(x, y - 1, z));

    Vector2 texturePos = new Vector2(0, 0);

    if (Block(x, y, z) == 4)
    {
      texturePos = tWater;
    }

    Cube(texturePos);
  }



  void CubeBot(int x, int y, int z)
  {
    newVertices.Add(new Vector3(x, y - 1, z));
    newVertices.Add(new Vector3(x + 1, y - 1, z));
    newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
    newVertices.Add(new Vector3(x, y - 1, z + 1));

    Vector2 texturePos = new Vector2(0, 0);

    if (Block(x, y, z) == 4)
    {
      texturePos = tWater;
    }

    Cube(texturePos);
  }

  void Cube(Vector2 texturePos)
  {

    newTriangles.Add(faceCount * 4); //1
    newTriangles.Add(faceCount * 4 + 1); //2
    newTriangles.Add(faceCount * 4 + 2); //3
    newTriangles.Add(faceCount * 4); //1
    newTriangles.Add(faceCount * 4 + 2); //3
    newTriangles.Add(faceCount * 4 + 3); //4

    newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
    newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
    newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
    newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

    faceCount++; // Add this line
  }

  void UpdateMesh()
  {

    mesh.Clear();
    mesh.vertices = newVertices.ToArray();
    mesh.uv = newUV.ToArray();
    mesh.triangles = newTriangles.ToArray();
    mesh.Optimize();
    mesh.RecalculateNormals();

    // col.sharedMesh = null;
    // col.sharedMesh = mesh;

    newVertices.Clear();
    newUV.Clear();
    newTriangles.Clear();
    faceCount = 0;

  }
}