using System.Runtime.InteropServices;

namespace NIVisaNet8Demo.Extensions;

public static class StringExtensions
{
    public static string? ToUTF8String(this nint buffer, char[]? terminators = null)
    {
        return terminators == null
            ? Marshal.PtrToStringUTF8(buffer)
            : Marshal.PtrToStringUTF8(buffer)?.TrimEnd(terminators);
    }
}