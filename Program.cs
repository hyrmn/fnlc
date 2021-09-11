using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

if (args.Length == 0)
{
    Console.WriteLine("Usage:\n\tlc \"path\\to\\file.txt\"");
    return;
}

if (!File.Exists(args[0]))
{
    Console.WriteLine($"Could not find {args[0]}. Check the file path.");
    return;
}

const int BufferSize = 512 * 1024;
const byte Rune = (byte)'\n';

using var file = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 1, FileOptions.SequentialScan);

var count = CountLines(file);
Console.WriteLine(count);

static unsafe uint CountLines(FileStream file)
{
    uint count = 0;

    const int vectorSize = 256 / 8; //256 bits, 8 bits in a byte.
    var maskSrc = stackalloc byte[vectorSize];
    
    for (var i = 0; i < vectorSize; i++)
    {
        maskSrc[i] = Rune;
    }

    var runeMask = Avx2.LoadVector256(maskSrc);
    var zero = Vector256<byte>.Zero;
    var accumulator = Vector256<long>.Zero;

    byte* ptr = (byte*)NativeMemory.AlignedAlloc(byteCount: BufferSize, alignment: vectorSize);
    Span<byte> buffer = new Span<byte>(ptr, BufferSize);

    int bytesRead;

    try
    {
        while ((bytesRead = file.Read(buffer)) != 0)
        {
            for (var i = 0; i <= bytesRead - vectorSize; i += vectorSize)
            {
                var v = Avx2.LoadVector256(ptr + i);
                var masked = Avx2.CompareEqual(v, runeMask);
                var result = Avx2.MoveMask(masked);
                count += Popcnt.PopCount((uint)result);
            }
        }
    }
    finally
    {
        NativeMemory.AlignedFree(ptr);
    }

    return count;
}