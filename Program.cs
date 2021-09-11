using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

if (args.Length == 0)
{
    Console.WriteLine("Usage:\n\tlc \"path\\to\\file.txt\"");
    return;
}

if(!File.Exists(args[0]))
{
    Console.WriteLine($"Could not find {args[0]}. Check the file path.");
    return;
}

const int BufferSize = 512 * 1024;
const byte Rune = (byte)'\n';

using var file = new FileStream(args[0], FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: BufferSize);

var count = CountLines(file);
Console.WriteLine(count);

static unsafe uint CountLines(FileStream file)
{
    uint count = 0;

    const int vectorSize = 256 / 8; //256 bits, 8 bits in a byte.
    var maskSrc = stackalloc byte[vectorSize];
    var scratch = stackalloc byte[vectorSize];

    for (var i = 0; i < vectorSize; i++)
    {
        maskSrc[i] = Rune;
    }
    
    var runeMask = Avx2.LoadVector256(maskSrc);
    var zero = Vector256<byte>.Zero;
    var accumulator = Vector256<long>.Zero;

    long bytesRead = 0;
    var fileSize = file.Length;

    int read;

    var buffer = new byte[BufferSize];

    while (bytesRead < fileSize)
    {
        read = file.Read(buffer, 0, BufferSize);
        bytesRead += read;
        int i;
        fixed (byte* ptr = buffer)
        {
            for (i = 0; i <= read - vectorSize; i += vectorSize)
            {
                var v = Avx2.LoadVector256(ptr + i);
                var masked = Avx2.CompareEqual(v, runeMask);
                var result = Avx2.MoveMask(masked);
                count += Popcnt.PopCount((uint)result);
            }
        }
    }

    return count;
}