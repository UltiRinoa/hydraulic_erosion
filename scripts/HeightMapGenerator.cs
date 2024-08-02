using System;
using System.Data.SqlTypes;
using Godot;
using Yonosuke.Helpers;

public partial class HeightMapGenerator : Node
{
    [Export] public float Persistance;
    [Export] public float Lacunarity;
    [Export] public float Scale;
    [Export] public int Octave;
    [Export] public uint Seed;
    [Export] public bool UseRandomSeed;
    [Export(PropertyHint.File)] public RDShaderFile _heightMapComputeShader;

    public float[] GenerateHeightMap(int mapSize)
    {
        Seed = UseRandomSeed ? GD.Randi() : Seed;
        GD.Seed(Seed);

        var offsets = new Vector2[Octave];
        for (var i = 0; i < offsets.Length; i++)
        {
            offsets[i] = new Vector2((GD.Randf() * 2 - 1) * 10000, (GD.Randf() * 2 - 1) * 10000);
        }
        var offsetsBytes = new byte[sizeof(float) * 2];
        Buffer.BlockCopy(offsets, 0, offsetsBytes, 0, offsetsBytes.Length);

        ComputeHelper.Instance.CreateBuffer((uint)offsetsBytes.Length, offsetsBytes, 0);

    }

    public override void _Ready()
    {
    }
}