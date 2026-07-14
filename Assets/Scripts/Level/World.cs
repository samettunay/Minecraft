using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{

	public GameObject chunk;
	public Chunk[,,] chunks;  //Changed from public GameObject[,,] chunks;
	public int chunkSize = 16;
	public byte[,,] data;

	public GameObject waterChunk;
	public WaterChunk[,,] waterChunks;  //Changed from public GameObject[,,] chunks;
	public int waterChunkSize = 16;
	public byte[,,] waterData;

	public int worldX = 16;
	public int worldY = 16;
	public int worldZ = 16;

	// Use this for initialization
	void Start()
	{

		data = new byte[worldX, worldY, worldZ];
		waterData = new byte[worldX, worldY, worldZ];

		for (int x = 0; x < worldX; x++)
		{
			for (int z = 0; z < worldZ; z++)
			{
				int stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
				stone += PerlinNoise(x, 300, z, 20, 4, 0) + 10;
				int dirt = PerlinNoise(x, 100, z, 50, 3, 0) + 1;

				for (int y = 0; y < worldY; y++)
				{
					if (y <= stone)
					{
						data[x, y, z] = 1;
					}
					else if (y <= dirt + stone)
					{
						data[x, y, z] = 2;
					}

					if (y > 0 && data[x, y, z] == 0 && data[x, y - 1, z] != 0 && y < worldY - 10 && data[x, y - 1, z] != 3)
					{
						if (Random.Range(0, 200) == 50 && data[x, y - 1, z] != 5)
						{
							if ((x > 10 && x < worldX - 10) && (z > 10 && z < worldZ - 10))
							{
								int treeHeight = Random.Range(4, 7);

								for (int i = 0; i <= treeHeight; i++)
								{
									data[x, y + i, z] = 3;
								}

								int leafRadius = 2;
								int leafHeight = 3;
								int leafStartY = y + treeHeight - 1;

								for (int ly = 0; ly < leafHeight; ly++)
								{
									int wy = leafStartY + ly;

									for (int lx = -leafRadius; lx <= leafRadius; lx++)
									{
										for (int lz = -leafRadius; lz <= leafRadius; lz++)
										{
											int wx = x + lx;
											int wz = z + lz;

											if (wx < 0 || wx >= worldX || wy < 0 || wy >= worldY || wz < 0 || wz >= worldZ)
												continue;

											if (data[wx, wy, wz] == 3)
												continue;

											float cornerDist = Mathf.Abs(lx) + Mathf.Abs(lz);
											if (cornerDist > leafRadius + 1)
												continue;

											if (Random.Range(0, 100) < 15)
												continue;

											data[wx, wy, wz] = 5;
										}
									}
								}
							}
						}
					}

					if (data[x, y, z] == 0 && y == 13)
					{
						waterData[x, y, z] = 4;
					}
				}

			}
		}

		chunks = new Chunk[Mathf.FloorToInt(worldX / chunkSize), Mathf.FloorToInt(worldY / chunkSize), Mathf.FloorToInt(worldZ / chunkSize)];
		waterChunks = new WaterChunk[Mathf.FloorToInt(worldX / chunkSize), Mathf.FloorToInt(worldY / chunkSize), Mathf.FloorToInt(worldZ / chunkSize)];

	}

	public void GenColumn(int x, int z)
	{
		for (int y = 0; y < chunks.GetLength(1); y++)
		{

			//Create a temporary Gameobject for the new chunk instead of using chunks[x,y,z]
			GameObject newChunk = Instantiate(chunk, new Vector3(x * chunkSize - 0.5f, y * chunkSize + 0.5f, z * chunkSize - 0.5f), new Quaternion(0, 0, 0, 0)) as GameObject;

			chunks[x, y, z] = newChunk.GetComponent("Chunk") as Chunk;
			chunks[x, y, z].worldGO = gameObject;
			chunks[x, y, z].chunkSize = chunkSize;
			chunks[x, y, z].chunkX = x * chunkSize;
			chunks[x, y, z].chunkY = y * chunkSize;
			chunks[x, y, z].chunkZ = z * chunkSize;

			print(chunks[x, y, z].chunkY);

		}

		for (int y = 0; y < waterChunks.GetLength(1); y++)
		{
			GameObject newWaterChunk = Instantiate(waterChunk, new Vector3(x * chunkSize - 0.5f, y * chunkSize + 0.5f, z * chunkSize - 0.5f), new Quaternion(0, 0, 0, 0)) as GameObject;

			waterChunks[x, y, z] = newWaterChunk.GetComponent("WaterChunk") as WaterChunk;
			waterChunks[x, y, z].worldGO = gameObject;
			waterChunks[x, y, z].chunkSize = chunkSize;
			waterChunks[x, y, z].chunkX = x * chunkSize;
			waterChunks[x, y, z].chunkY = y * chunkSize;
			waterChunks[x, y, z].chunkZ = z * chunkSize;

			print(waterChunks[x, y, z].chunkY);
		}

	}

	public void UnloadColumn(int x, int z)
	{
		for (int y = 0; y < chunks.GetLength(1); y++)
		{
			Object.Destroy(chunks[x, y, z].gameObject);
		}

		for (int y = 0; y < waterChunks.GetLength(1); y++)
		{
			Object.Destroy(waterChunks[x, y, z].gameObject);
		}
	}

	int PerlinNoise(int x, int y, int z, float scale, float height, float power)
	{
		float rValue;
		rValue = Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
		rValue *= height;

		if (power != 0)
		{
			rValue = Mathf.Pow(rValue, power);
		}

		return (int)rValue;
	}

	public byte Block(int x, int y, int z)
	{

		if (x >= worldX || x < 0 || y >= worldY || y < 0 || z >= worldZ || z < 0)
		{
			return (byte)1;
		}

		return data[x, y, z];
	}

	public byte WaterBlock(int x, int y, int z)
	{

		if (x >= worldX || x < 0 || y >= worldY || y < 0 || z >= worldZ || z < 0)
		{
			return (byte)1;
		}

		return waterData[x, y, z];
	}
}