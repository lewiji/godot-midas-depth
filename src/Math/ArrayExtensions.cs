using System;

namespace GodotMidasDepth.Math; 

public static class ArrayExtensions {
    public static float[] ToFloat32Array(this byte[] byteArray) {
        var floatCount = byteArray.Length / sizeof(float);
        var dest = new float[floatCount];
        Buffer.BlockCopy(byteArray, 0, dest, 0, byteArray.Length);
        return dest;
    }
    
    public static byte[] ToByteArray(this float[] floatArray) {
        var byteCount = floatArray.Length * sizeof(float);
        var dest = new byte[byteCount];
        Buffer.BlockCopy(floatArray, 0, dest, 0, byteCount);
        return dest;
    }
}