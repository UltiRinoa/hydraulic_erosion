using System;
using System.Runtime.Intrinsics.Arm;
using Godot;
using Godot.Collections;
namespace Yonosuke.Helpers;

public partial class ComputeHelper : Node
{
    [Export(PropertyHint.File)] public RDShaderFile ShaderFile;

    public override void _Ready()
    {
        var rd = RenderingServer.CreateLocalRenderingDevice();
        // var shaderRid = LoadShader(rd, ShaderFile);
        var shaderBytecode = ShaderFile.GetSpirV();
        var shader = rd.ShaderCreateFromSpirV(shaderBytecode);

        var input = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var inputBytes = new byte[input.Length * sizeof(float)];
        Buffer.BlockCopy(input, 0, inputBytes, 0, inputBytes.Length);
        var buffer = rd.StorageBufferCreate((uint)inputBytes.Length, inputBytes);

        var uniform = new RDUniform
        {
            UniformType = RenderingDevice.UniformType.StorageBuffer,
            Binding = 0
        };
        uniform.AddId(buffer);
        var uniformSet = rd.UniformSetCreate(new Array<RDUniform> { uniform }, shader, 0);

        var pipeline = rd.ComputePipelineCreate(shader);
        var computeList = rd.ComputeListBegin();
        rd.ComputeListBindComputePipeline(computeList, pipeline);
        rd.ComputeListBindUniformSet(computeList, uniformSet, 0);
        rd.ComputeListDispatch(computeList, 5, 1, 1);
        rd.ComputeListEnd();

        rd.Submit();
        rd.Sync();

        var outputBytes = rd.BufferGetData(buffer);
        var output = new float[input.Length];
        Buffer.BlockCopy(outputBytes, 0, output, 0, outputBytes.Length);
        GD.Print("Input: ", string.Join(", ", input));
        GD.Print("output: ", string.Join(", ", output));
    }

    // private Rid LoadShader(RenderingDevice rd, RDShaderFile shaderFile)
    // {
    // }
}