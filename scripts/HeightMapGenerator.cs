using System;
using Godot;

public partial class HeightMapGenerator : Node
{
    [Export] public float Persistance;
    [Export] public float Lacunarity;
    [Export] public float Scale;
    [Export] public int Octave;
    [Export] public int Seed;
    [Export] public bool UseRandomSeed;

    // public float[] GenerateHeightMap(int mapSize)
    // {
    //     Seed = UseRandomSeed ? Random.Shared.Next(-10000, 10000) : Seed;

    //     var prng = new Random(Seed);

    //     var octaveOffsets = new Vector2[Octave];
    //     for (var i = 0; i < octaveOffsets.Length; i++)
    //     {
    //         octaveOffsets[i] = new Vector2(prng.Next(-10000, 10000), prng.Next(-10000, 10000));
    //     }
    // }
}