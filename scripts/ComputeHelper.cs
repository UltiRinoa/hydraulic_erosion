using System;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using Godot;
using Godot.Collections;
namespace Yonosuke.Helpers;

public class ComputeHelper
{
    public enum BufferType { Storage }
    private static ComputeHelper _instance;
    public static ComputeHelper Instance => _instance ?? new ComputeHelper();

    private RenderingDevice _rd;
    private Rid _shader;
    private bool _isWorking;

    private Array<Rid> _createdRids = new Array<Rid>();
    private Rid _uniformSet;
    private int _bindingCount;
    private Dictionary<uint, Array<RDUniform>> _bindings = new();

    private ComputeHelper() { }

    // public override void _Ready()
    // {
    //     var rd = RenderingServer.CreateLocalRenderingDevice();
    //     // var shaderRid = LoadShader(rd, ShaderFile);
    //     var shaderBytecode = ShaderFile.GetSpirV();
    //     var shader = rd.ShaderCreateFromSpirV(shaderBytecode);

    //     var input = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    //     var inputBytes = new byte[input.Length * sizeof(float)];
    //     Buffer.BlockCopy(input, 0, inputBytes, 0, inputBytes.Length);
    //     var buffer = rd.StorageBufferCreate((uint)inputBytes.Length, inputBytes);

    //     var uniform = new RDUniform
    //     {
    //         UniformType = RenderingDevice.UniformType.StorageBuffer,
    //         Binding = 0
    //     };
    //     uniform.AddId(buffer);
    //     var uniformSet = rd.UniformSetCreate(new Array<RDUniform> { uniform }, shader, 0);

    //     var pipeline = rd.ComputePipelineCreate(shader);
    //     var computeList = rd.ComputeListBegin();
    //     rd.ComputeListBindComputePipeline(computeList, pipeline);
    //     rd.ComputeListBindUniformSet(computeList, uniformSet, 0);
    //     rd.ComputeListDispatch(computeList, 5, 1, 1);
    //     rd.ComputeListEnd();

    //     rd.Submit();
    //     rd.Sync();

    //     var outputBytes = rd.BufferGetData(buffer);
    //     var output = new float[input.Length];
    //     Buffer.BlockCopy(outputBytes, 0, output, 0, outputBytes.Length);
    //     GD.Print("Input: ", string.Join(", ", input));
    //     GD.Print("output: ", string.Join(", ", output));
    // }

    public void InitGpu(RDShaderFile shaderFile)
    {
        _rd = RenderingServer.CreateLocalRenderingDevice();
        _shader = LoadShader(_rd, shaderFile);
    }

    public Rid CreateBuffer(uint length, byte[] bytes, uint set)
    {
        var rid = _rd.StorageBufferCreate(length, bytes);
        _createdRids.Add(rid);
        var uniform = new RDUniform
        {
            UniformType = RenderingDevice.UniformType.StorageBuffer
        };
        uniform.AddId(rid);
        uniform.Binding = _bindingCount++;

        if (_bindings.TryGetValue(set, out var bindings))
        {
            bindings.Add(uniform);
        }
        else
        {
            _bindings.Add(set, new Array<RDUniform>() { uniform });
        }
        return rid;
    }

    public Rid CreateTexture(RDTextureFormat format, Array<byte[]> data)
    {
        var rid = _rd.TextureCreate(format, new RDTextureView(), data);
        _createdRids.Add(rid);
        return rid;
    }

    public Rid CreateUniformSet(Array<RDUniform> uniforms, uint shaderSet)
    {
        _uniformSet = _rd.UniformSetCreate(uniforms, _shader, shaderSet);
        return _uniformSet;
    }

    public void UpdateTexture(Rid textureRid, uint layer, byte[] data)
    {
        _rd.TextureUpdate(textureRid, layer, data);
    }

    public void UpdateBuffer(Rid bufferRid, uint offset, uint size, byte[] data)
    {
        _rd.BufferUpdate(bufferRid, offset, size, data);
    }

    public void Dispatch(uint x, uint y, uint z)
    {
        var pipeline = _rd.ComputePipelineCreate(_shader);
        _createdRids.Add(pipeline);
        var computeList = _rd.ComputeListBegin();
        _rd.ComputeListBindComputePipeline(computeList, pipeline);
        _rd.ComputeListBindUniformSet(computeList, _uniformSet, 0);
        _rd.ComputeListDispatch(computeList, x, y, z);
        _rd.ComputeListEnd();
        _rd.Submit();

    }

    public byte[] GetTextureData(Rid textureRid, uint layer)
    {
        return _rd.TextureGetData(textureRid, layer);
    }

    public byte[] GetBufferData(Rid bufferRid, uint offset, uint size)
    {
        return _rd.BufferGetData(bufferRid, offset, size);
    }

    private Rid LoadShader(RenderingDevice rd, RDShaderFile shaderFile)
    {
        return rd.ShaderCreateFromSpirV(shaderFile.GetSpirV());
    }
}