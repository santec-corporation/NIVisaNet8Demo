using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using NIVisaNet8Demo.NI.VISA.Enums;
using NIVisaNet8Demo.NI.VISA.Enums.Gpib;
using NIVisaNet8Demo.NI.VISA.Enums.Vxi;
using static NIVisaNet8Demo.Extensions.StringExtensions;

// ReSharper disable InconsistentNaming

namespace NIVisaNet8Demo.NI.VISA;

public delegate int VisaDllEventHandler(nint vi, EventType eventType, nint context, nint userHandle);

public partial class NativeMethods
{
    private const int BufferSize = 512;
    public const string Terminator = "\r\n";
    public static readonly char[] Terminators = Terminator.ToCharArray();

    public static readonly ByteOrder ByteOrder =
        BitConverter.IsLittleEndian ? ByteOrder.VI_LITTLE_ENDIAN : ByteOrder.VI_BIG_ENDIAN;
#pragma warning disable IDE1006
#if Windows
    private const string DllName = "Visa32.Dll";
#elif Linux
        private const string DllName = "libvisa.so";
#endif
    [LibraryImport(DllName, EntryPoint = "viOpenDefaultRM")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _OpenDefaultRM(out nint sesn);

    public static VisaStatus viOpenDefaultRM(out VisaHandle? sesn)
    {
        var num = _OpenDefaultRM(out var sesn2);
        if (num < 0)
            sesn = null;
        else
            sesn = new VisaHandle(sesn2);

        return num;
    }

    [LibraryImport(DllName, EntryPoint = "viOpen", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Open(nint sesn, string rsrcName, AccessModes accessMode, int openTimeout,
        out nint vi);

    public static VisaStatus viOpen(VisaHandle sesn, string rsrcName, AccessModes accessMode, int openTimeout,
        out VisaHandle? vi)
    {
        var num = _Open(sesn.DangerousGetHandle(), rsrcName, accessMode, openTimeout, out var vi2);
        if (num < 0)
            vi = null;
        else
            vi = new VisaHandle(vi2);

        return num;
    }

    [LibraryImport(DllName, EntryPoint = "viClose")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Close(nint vi);

    public static VisaStatus viClose(VisaHandle vi)
    {
        return _Close(vi.DangerousGetHandle());
    }

    public static unsafe VisaStatus viRead(VisaHandle vi, out string? buf, int count, out int retcnt)
    {
        var buffer = new byte[count];
        fixed (byte* ptrBuffer = buffer)
        {
            var num = _Read(vi.DangerousGetHandle(), (nint)ptrBuffer, count, out retcnt);
            if (num < 0)
                buf = null;
            else
                buf = ((nint)ptrBuffer).ToUTF8String();
            return num;
        }
    }

    [LibraryImport(DllName, EntryPoint = "viRead")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Read(nint vi, nint buf, int count, out int retCount);

    public static unsafe VisaStatus viRead(VisaHandle vi, byte[] buf, int count, out int retCount)
    {
        fixed (byte* ptrBuf = buf)
        {
            return _Read(vi.DangerousGetHandle(), (nint)ptrBuf, count, out retCount);
        }
    }

    [LibraryImport(DllName, EntryPoint = "viWrite", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Write(nint vi, string buf, int count, out int retCount);

    public static VisaStatus viWrite(VisaHandle vi, in string buf, out int retCount)
    {
        var str = buf;
        if (!buf.EndsWith(Terminator)) str += Terminator;
        return _Write(vi.DangerousGetHandle(), str, str.Length, out retCount);
    }

    [LibraryImport(DllName, EntryPoint = "viWrite")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Write(nint vi, [In] byte[] buf, int count, out int retCount);

    public static VisaStatus viWrite(VisaHandle vi, byte[] buf, int count, out int retCount)
    {
        return _Write(vi.DangerousGetHandle(), buf, count, out retCount);
    }

    [LibraryImport(DllName, EntryPoint = "viBufWrite", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _BufWrite(nint vi, string buffer, int count, out int retCount);

    public static VisaStatus viBufWrite(VisaHandle vi, string buffer, int count, out int retCount)
    {
        return _BufWrite(vi.DangerousGetHandle(), buffer, count, out retCount);
    }

    [LibraryImport(DllName, EntryPoint = "viBufWrite")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _BufWrite(nint vi, [In] byte[] buffer, int count, out int retCount);

    public static VisaStatus viBufWrite(VisaHandle vi, byte[] buffer, int count, out int retCount)
    {
        return _BufWrite(vi.DangerousGetHandle(), buffer, count, out retCount);
    }


    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, ref int count, [Out] sbyte[] data);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, ref int count, sbyte[] data)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, ref count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, ref int count, nint data);

    public static unsafe VisaStatus viScanf(VisaHandle vi, string scanSpecifier, ref int count, byte[] data)
    {
        fixed (byte* ptrData = data)
        {
            return _viScanf(vi.DangerousGetHandle(), scanSpecifier, ref count, (nint)ptrData);
        }
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, ref int count, [Out] ushort[] data);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, ref int count, ushort[] data)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, ref count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, ref int count, [Out] short[] data);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, ref int count, short[] data)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, ref count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, ref int count, [Out] int[] data);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, ref int count, int[] data)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, ref count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, ref int count, [Out] uint[] data);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, ref int count, uint[] data)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, ref count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, ref int count, [Out] float[] data);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, ref int count, float[] data)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, ref count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, ref int count, [Out] double[] data);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, ref int count, double[] data)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, ref count, data);
    }

    public static unsafe VisaStatus viScanf(VisaHandle vi, string scanSpecifier, ref int count, out string? data)
    {
        var buffer = new byte[BufferSize];
        fixed (byte* ptrBuffer = buffer)
        {
            var num = _viScanf(vi.DangerousGetHandle(), scanSpecifier, ref count, (nint)ptrBuffer);
            if (num < 0)
                data = null;
            else
                data = ((nint)ptrBuffer).ToUTF8String();
            return num;
        }
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier);
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, out int receiver);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, out int receiver)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, out receiver);
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, out double receiver);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, out double receiver)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, out receiver);
    }

    [LibraryImport(DllName, EntryPoint = "viScanf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viScanf(nint vi, string scanSpecifier, out byte receiver);

    public static VisaStatus viScanf(VisaHandle vi, string scanSpecifier, out byte receiver)
    {
        return _viScanf(vi.DangerousGetHandle(), scanSpecifier, out receiver);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, string data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, string data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, data);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, int count, [In] byte[] data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, int count, byte[] data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, int count, [In] sbyte[] data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, int count, sbyte[] data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, int count, [In] short[] data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, int count, short[] data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, int count, [In] ushort[] data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, int count, ushort[] data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, int count, [In] int[] data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, int count, int[] data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, int count, [In] uint[] data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, int count, uint[] data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, int count, [In] float[] data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, int count, float[] data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, int count, [In] double[] data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, int count, double[] data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viPrintf", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _viPrintf(nint vi, string scanSpecifier, int count, nint data);

    public static VisaStatus viPrintf(VisaHandle vi, string scanSpecifier, int count, nint data)
    {
        return _viPrintf(vi.DangerousGetHandle(), scanSpecifier, count, data);
    }

    [LibraryImport(DllName, EntryPoint = "viBufRead")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _BufRead(nint vi, [Out] byte[] buf, int count, out int retCount);

    public static VisaStatus viBufRead(VisaHandle vi, byte[] buf, int count, out int retCount)
    {
        return _BufRead(vi.DangerousGetHandle(), buf, count, out retCount);
    }

    [LibraryImport(DllName, EntryPoint = "viStatusDesc")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _StatusDesc(nint vi, int status, nint buf);

    public static unsafe VisaStatus viStatusDesc(VisaHandle vi, int status, out string? buf)
    {
        var buffer = new byte[BufferSize];
        fixed (byte* ptrBuffer = buffer)
        {
            var num = _StatusDesc(vi.DangerousGetHandle(), status, (nint)ptrBuffer);
            if (num < 0)
                buf = null;
            else
                buf = ((nint)ptrBuffer).ToUTF8String();
            return num;
        }
    }

    [LibraryImport(DllName, EntryPoint = "viGetAttribute")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _GetAttributeUInt(nint vi, AttributeType attribute, out ulong attrState);

    public static VisaStatus GetAttributeUInt(VisaHandle vi, AttributeType attribute, out ulong attrState)
    {
        return _GetAttributeUInt(vi.DangerousGetHandle(), attribute, out attrState);
    }

    public static unsafe VisaStatus GetAttributeString(VisaHandle vi, AttributeType attribute, out string? attrState)
    {
        var buffer = new byte[BufferSize];
        fixed (byte* ptrBuffer = buffer)
        {
            var num = _GetAttributeByteArray(vi.DangerousGetHandle(), attribute, (nint)ptrBuffer);

            if (num < 0)
                attrState = null;
            else
                attrState = ((nint)ptrBuffer).ToUTF8String();
            return num;
        }
    }

    [LibraryImport(DllName, EntryPoint = "viGetAttribute")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _GetAttributeByteArray(nint vi, AttributeType attribute, nint attrState);

    public static unsafe VisaStatus GetAttributeByteArray(VisaHandle vi, AttributeType attribute, out byte[] attrState,
        int bufferSizeToCreate)
    {
        attrState = new byte[bufferSizeToCreate];
        fixed (byte* ptrAttrState = attrState)
        {
            return _GetAttributeByteArray(vi.DangerousGetHandle(), attribute, (nint)ptrAttrState);
        }
    }

    [LibraryImport(DllName, EntryPoint = "viSetAttribute")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _SetAttributeUInt(nint vi, AttributeType attribute, ulong attrState);

    public static VisaStatus SetAttributeUInt(VisaHandle vi, AttributeType attribute, ulong attrState)
    {
        return _SetAttributeUInt(vi.DangerousGetHandle(), attribute, attrState);
    }

    [LibraryImport(DllName, EntryPoint = "viSetAttribute", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _SetAttributeString(nint vi, AttributeType attribute, string attrState);

    public static VisaStatus SetAttributeString(VisaHandle vi, AttributeType attribute, string attrState)
    {
        return _SetAttributeString(vi.DangerousGetHandle(), attribute, attrState);
    }

    [LibraryImport(DllName, EntryPoint = "viClear")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Clear(nint vi);

    public static VisaStatus viClear(VisaHandle vi)
    {
        return _Clear(vi.DangerousGetHandle());
    }

    [LibraryImport(DllName, EntryPoint = "viParseRsrc", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _ParseRsrc(nint sesn, string rsrcName, out HardwareInterfaceType intfType,
        out short intfNum);

    public static VisaStatus viParseRsrc(VisaHandle sesn, string rsrcName, out HardwareInterfaceType intfType,
        out short intfNum)
    {
        return _ParseRsrc(sesn.DangerousGetHandle(), rsrcName, out intfType, out intfNum);
    }

    [LibraryImport(DllName, EntryPoint = "viParseRsrcEx", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _ParseRsrcEx(nint rmSesn, string rsrcName, out HardwareInterfaceType intfType,
        out short intfNum, nint rsrcClass, nint expandedUnaliasedName, nint aliasIfExists);

    public static unsafe VisaStatus viParseRsrcEx(VisaHandle rmSesn, string rsrcName,
        out HardwareInterfaceType intfType, out short intfNum, out string? rsrcClass, out string? expandedUnaliasedName,
        out string? aliasIfExists)
    {
        var rsrcClassBuffer = new byte[BufferSize];
        var expandedUnaliasedNameBuffer = new byte[BufferSize];
        var aliasIfExistsBuffer = new byte[BufferSize];
        fixed (byte* ptrRsrcClassBuffer = rsrcClassBuffer, ptrExpandedUnaliasedNameBuffer =
                   expandedUnaliasedNameBuffer, ptrAliasIfExistsBuffer = aliasIfExistsBuffer)
        {
            var num = _ParseRsrcEx(rmSesn.DangerousGetHandle(), rsrcName, out intfType, out intfNum,
                (nint)ptrRsrcClassBuffer, (nint)ptrExpandedUnaliasedNameBuffer, (nint)ptrAliasIfExistsBuffer);
            if (num < 0)
            {
                rsrcClass = null;
                expandedUnaliasedName = null;
                aliasIfExists = null;
            }
            else
            {
                rsrcClass = ((nint)ptrRsrcClassBuffer).ToUTF8String();
                expandedUnaliasedName = ((nint)ptrExpandedUnaliasedNameBuffer).ToUTF8String();
                aliasIfExists = ((nint)ptrAliasIfExistsBuffer).ToUTF8String();
            }

            return num;
        }
    }

    [LibraryImport(DllName, EntryPoint = "viFindRsrc", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _FindRsrc(nint sesn, string expr, out nint findList, out int retcnt,
        nint instrDesc);

    public static unsafe VisaStatus viFindRsrc(VisaHandle sesn, string expr, out VisaHandle? findList, out int retcnt,
        out string? instrDesc)
    {
        var buffer = new byte[BufferSize];
        fixed (byte* ptrBuffer = buffer)
        {
            var num = _FindRsrc(sesn.DangerousGetHandle(), expr, out var findList2, out retcnt, (nint)ptrBuffer);
            if (num < 0)
            {
                instrDesc = null;
                findList = null;
            }
            else
            {
                instrDesc = ((nint)ptrBuffer).ToUTF8String();
                findList = new VisaHandle(findList2);
            }

            return num;
        }
    }

    [LibraryImport(DllName, EntryPoint = "viFindNext", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _FindNext(nint findList, nint instrDesc);

    public static unsafe VisaStatus viFindNext(VisaHandle findList, out string? instrDesc)
    {
        var buffer = new byte[BufferSize];
        fixed (byte* ptrBuffer = buffer)
        {
            var num = _FindNext(findList.DangerousGetHandle(), (nint)ptrBuffer);
            if (num < 0)
                instrDesc = null;
            else
                instrDesc = ((nint)ptrBuffer).ToUTF8String();
            return num;
        }
    }

    [LibraryImport(DllName, EntryPoint = "viEnableEvent")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _EnableEvent(nint vi, EventType eventType, short mechanism, int context);

    public static VisaStatus viEnableEvent(VisaHandle vi, EventType eventType, short mechanism)
    {
        return _EnableEvent(vi.DangerousGetHandle(), eventType, mechanism, 0);
    }

    [LibraryImport(DllName, EntryPoint = "viDisableEvent")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _DisableEvent(nint vi, EventType eventType, short mechanism);

    public static VisaStatus viDisableEvent(VisaHandle vi, EventType eventType, short mechanism)
    {
        return _DisableEvent(vi.DangerousGetHandle(), eventType, mechanism);
    }

    [LibraryImport(DllName, EntryPoint = "viDiscardEvents")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _DiscardEvents(nint vi, int eventType, short mechanism);

    public static VisaStatus viDiscardEvents(VisaHandle vi, int eventType, short mechanism)
    {
        return _DiscardEvents(vi.DangerousGetHandle(), eventType, mechanism);
    }

    [LibraryImport(DllName, EntryPoint = "viLock", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Lock(nint vi, AccessModes lockType, int timeout, string requestedKey,
        nint accesskey);

    public static unsafe VisaStatus viLock(VisaHandle vi, AccessModes lockType, int timeout, string requestedKey,
        out string? accesskey)
    {
        var buffer = new byte[BufferSize];
        fixed (byte* ptrBuffer = buffer)
        {
            var num = _Lock(vi.DangerousGetHandle(), lockType, timeout, requestedKey, (nint)ptrBuffer);
            if (num < 0)
                accesskey = null;
            else
                accesskey = ((nint)ptrBuffer).ToUTF8String();
            return num;
        }
    }

    [LibraryImport(DllName, EntryPoint = "viUnlock")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Unlock(nint vi);

    public static VisaStatus viUnlock(VisaHandle vi)
    {
        return _Unlock(vi.DangerousGetHandle());
    }

    [LibraryImport(DllName, EntryPoint = "viWaitOnEvent")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _WaitOnEvent(nint vi, EventType inEventType, int timeout, int outEventType,
        out nint outContext);

    public static VisaStatus viWaitOnEvent(VisaHandle vi, EventType inEventType, int timeout,
        out VisaHandle? outContext)
    {
        var num = _WaitOnEvent(vi.DangerousGetHandle(), inEventType, timeout, 0, out var outContext2);
        if (num < 0)
            outContext = null;
        else
            outContext = new VisaHandle(outContext2);

        return num;
    }

    [LibraryImport(DllName, EntryPoint = "viInstallHandler")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _InstallHandler(nint vi, EventType eventType, VisaDllEventHandler handler,
        int userHandle);

    public static VisaStatus viInstallHandler(VisaHandle vi, EventType eventType, VisaDllEventHandler handler)
    {
        return _InstallHandler(vi.DangerousGetHandle(), eventType, handler, 0);
    }

    [LibraryImport(DllName, EntryPoint = "viUninstallHandler")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _UninstallHandler(nint vi, EventType eventType, VisaDllEventHandler handler,
        int userHandle);

    public static VisaStatus viUninstallHandler(VisaHandle vi, EventType eventType, VisaDllEventHandler handler)
    {
        return _UninstallHandler(vi.DangerousGetHandle(), eventType, handler, 0);
    }

    [LibraryImport(DllName, EntryPoint = "viAssertTrigger")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _AssertTrigger(nint vi, short protocol);

    public static VisaStatus viAssertTrigger(VisaHandle vi, short protocol)
    {
        return _AssertTrigger(vi.DangerousGetHandle(), protocol);
    }

    [LibraryImport(DllName, EntryPoint = "viAssertTrigger")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _AssertTrigger(nint vi, TriggerProtocol protocol);

    public static VisaStatus viAssertTrigger(VisaHandle vi, TriggerProtocol protocol)
    {
        return _AssertTrigger(vi.DangerousGetHandle(), protocol);
    }

    [LibraryImport(DllName, EntryPoint = "viReadAsync")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _ReadAsync(nint vi, nint buf, int count, out int jobId);

    public static VisaStatus viReadAsync(VisaHandle vi, nint buf, int count, out int jobId)
    {
        return _ReadAsync(vi.DangerousGetHandle(), buf, count, out jobId);
    }

    [LibraryImport(DllName, EntryPoint = "viReadSTB")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _ReadSTB(nint vi, out short status);

    public static VisaStatus viReadSTB(VisaHandle vi, out short status)
    {
        return _ReadSTB(vi.DangerousGetHandle(), out status);
    }

    [LibraryImport(DllName, EntryPoint = "viReadToFile", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _ReadToFile(nint vi, string fileName, int count, out int retCount);

    public static VisaStatus viReadToFile(VisaHandle vi, string fileName, int count, out int retCount)
    {
        return _ReadToFile(vi.DangerousGetHandle(), fileName, count, out retCount);
    }

    [LibraryImport(DllName, EntryPoint = "viTerminate")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Terminate(nint vi, short degree, int jobId);

    public static VisaStatus viTerminate(VisaHandle vi, int jobId)
    {
        return _Terminate(vi.DangerousGetHandle(), 0, jobId);
    }

    [LibraryImport(DllName, EntryPoint = "viWriteAsync")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _WriteAsync(nint vi, nint buf, int count, out int jobId);

    public static VisaStatus viWriteAsync(VisaHandle vi, nint buf, int count, out int jobId)
    {
        return _WriteAsync(vi.DangerousGetHandle(), buf, count, out jobId);
    }

    [LibraryImport(DllName, EntryPoint = "viWriteFromFile", StringMarshalling = StringMarshalling.Utf8)]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _WriteFromFile(nint vi, string fileName, int count, out int retCount);

    public static VisaStatus viWriteFromFile(VisaHandle vi, string fileName, int count, out int retCount)
    {
        return _WriteFromFile(vi.DangerousGetHandle(), fileName, count, out retCount);
    }

    [LibraryImport(DllName, EntryPoint = "viGpibControlREN")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _GpibControlREN(nint vi, RenMode mode);

    public static VisaStatus viGpibControlREN(VisaHandle vi, RenMode mode)
    {
        return _GpibControlREN(vi.DangerousGetHandle(), mode);
    }

    [LibraryImport(DllName, EntryPoint = "viGpibControlATN")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _GpibControlATN(nint vi, AtnMode mode);

    public static VisaStatus viGpibControlATN(VisaHandle vi, AtnMode mode)
    {
        return _GpibControlATN(vi.DangerousGetHandle(), mode);
    }

    [LibraryImport(DllName, EntryPoint = "viGpibPassControl")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _GpibPassControl(nint vi, short primAddr, short secAddr);

    public static VisaStatus viGpibPassControl(VisaHandle vi, short primAddr, short secAddr)
    {
        return _GpibPassControl(vi.DangerousGetHandle(), primAddr, secAddr);
    }

    [LibraryImport(DllName, EntryPoint = "viGpibCommand")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _GpibCommand(nint vi, [In] byte[] buf, int count, out int retCount);

    public static VisaStatus viGpibCommand(VisaHandle vi, byte[] buf, int count, out int retCount)
    {
        return _GpibCommand(vi.DangerousGetHandle(), buf, count, out retCount);
    }

    [LibraryImport(DllName, EntryPoint = "viGpibSendIFC")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _GpibSendIFC(nint vi);

    public static VisaStatus viGpibSendIFC(VisaHandle vi)
    {
        return _GpibSendIFC(vi.DangerousGetHandle());
    }

    [LibraryImport(DllName, EntryPoint = "viFlush")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Flush(nint vi, BufferTypes mask);

    public static VisaStatus viFlush(VisaHandle vi, BufferTypes mask)
    {
        return _Flush(vi.DangerousGetHandle(), mask);
    }

    [LibraryImport(DllName, EntryPoint = "viFlush")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Flush(nint vi, short mask);

    public static VisaStatus viFlush(VisaHandle vi, short mask)
    {
        return _Flush(vi.DangerousGetHandle(), mask);
    }

    [LibraryImport(DllName, EntryPoint = "viSetBuf")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _SetBuf(nint vi, BufferTypes mask, int size);

    public static VisaStatus viSetBuf(VisaHandle vi, BufferTypes mask, int size)
    {
        return _SetBuf(vi.DangerousGetHandle(), mask, size);
    }

    [LibraryImport(DllName, EntryPoint = "viIn8")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _In8(nint vi, AddressSpace space, uint offset, out byte val8);

    public static VisaStatus viIn8(VisaHandle vi, AddressSpace space, int offset, out byte val8)
    {
        return _In8(vi.DangerousGetHandle(), space, (uint)offset, out val8);
    }

    [LibraryImport(DllName, EntryPoint = "viIn8Ex")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _In8Ex(nint vi, AddressSpace space, long offset, out byte val8);

    public static VisaStatus viIn8Ex(VisaHandle vi, AddressSpace space, long offset, out byte val8)
    {
        return _In8Ex(vi.DangerousGetHandle(), space, offset, out val8);
    }

    [LibraryImport(DllName, EntryPoint = "viIn16")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _In16(nint vi, AddressSpace space, uint offset, out short val16);

    public static VisaStatus viIn16(VisaHandle vi, AddressSpace space, int offset, out short val16)
    {
        return _In16(vi.DangerousGetHandle(), space, (uint)offset, out val16);
    }

    [LibraryImport(DllName, EntryPoint = "viIn16Ex")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _In16Ex(nint vi, AddressSpace space, long offset, out short val16);

    public static VisaStatus viIn16Ex(VisaHandle vi, AddressSpace space, long offset, out short val16)
    {
        return _In16Ex(vi.DangerousGetHandle(), space, offset, out val16);
    }

    [LibraryImport(DllName, EntryPoint = "viIn")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _In(nint vi, AddressSpace space, uint offset, out int val);

    public static VisaStatus viIn(VisaHandle vi, AddressSpace space, int offset, out int val)
    {
        return _In(vi.DangerousGetHandle(), space, (uint)offset, out val);
    }

    [LibraryImport(DllName, EntryPoint = "viInEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _InEx(nint vi, AddressSpace space, long offset, out int val);

    public static VisaStatus viInEx(VisaHandle vi, AddressSpace space, long offset, out int val)
    {
        return _InEx(vi.DangerousGetHandle(), space, offset, out val);
    }

    [LibraryImport(DllName, EntryPoint = "viIn")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _In(nint vi, AddressSpace space, uint offset, out long val);

    public static VisaStatus viIn(VisaHandle vi, AddressSpace space, int offset, out long val)
    {
        return _In(vi.DangerousGetHandle(), space, (uint)offset, out val);
    }

    [LibraryImport(DllName, EntryPoint = "viInEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _InEx(nint vi, AddressSpace space, long offset, out long val);

    public static VisaStatus viInEx(VisaHandle vi, AddressSpace space, long offset, out long val)
    {
        return _InEx(vi.DangerousGetHandle(), space, offset, out val);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveIn8")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus
        _MoveIn8(nint vi, AddressSpace space, uint offset, int length, [Out] byte[] buf8);

    public static VisaStatus viMoveIn8(VisaHandle vi, AddressSpace space, int offset, int length, byte[] buf8)
    {
        return _MoveIn8(vi.DangerousGetHandle(), space, (uint)offset, length, buf8);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveIn8Ex")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveIn8Ex(nint vi, AddressSpace space, long offset, int length,
        [Out] byte[] buf8);

    public static VisaStatus viMoveIn8Ex(VisaHandle vi, AddressSpace space, long offset, int length, byte[] buf8)
    {
        return _MoveIn8Ex(vi.DangerousGetHandle(), space, offset, length, buf8);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveIn16")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveIn16(nint vi, AddressSpace space, uint offset, int length,
        [Out] short[] buf16);

    public static VisaStatus viMoveIn16(VisaHandle vi, AddressSpace space, int offset, int length, short[] buf16)
    {
        return _MoveIn16(vi.DangerousGetHandle(), space, (uint)offset, length, buf16);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveIn16Ex")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveIn16Ex(nint vi, AddressSpace space, long offset, int length,
        [Out] short[] buf16);

    public static VisaStatus viMoveIn16Ex(VisaHandle vi, AddressSpace space, long offset, int length, short[] buf16)
    {
        return _MoveIn16Ex(vi.DangerousGetHandle(), space, offset, length, buf16);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveIn")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveIn(nint vi, AddressSpace space, ulong offset, int length, [Out] int[] buf);

    public static VisaStatus viMoveIn(VisaHandle vi, AddressSpace space, int offset, int length, int[] buf)
    {
        return _MoveIn(vi.DangerousGetHandle(), space, (uint)offset, length, buf);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveInEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveInEx(nint vi, AddressSpace space, long offset, int length, [Out] int[] buf);

    public static VisaStatus viMoveInEx(VisaHandle vi, AddressSpace space, long offset, int length, int[] buf)
    {
        return _MoveInEx(vi.DangerousGetHandle(), space, offset, length, buf);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveIn")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveIn(nint vi, AddressSpace space, uint offset, int length, [Out] long[] buf);

    public static VisaStatus viMoveIn(VisaHandle vi, AddressSpace space, int offset, int length, long[] buf)
    {
        return _MoveIn(vi.DangerousGetHandle(), space, (uint)offset, length, buf);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveInEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus
        _MoveInEx(nint vi, AddressSpace space, long offset, int length, [Out] long[] buf);

    public static VisaStatus viMoveInEx(VisaHandle vi, AddressSpace space, long offset, int length, long[] buf)
    {
        return _MoveInEx(vi.DangerousGetHandle(), space, offset, length, buf);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveOut8")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus
        _MoveOut8(nint vi, AddressSpace space, uint offset, int length, [In] byte[] buf8);

    public static VisaStatus viMoveOut8(VisaHandle vi, AddressSpace space, int offset, int length, byte[] buf8)
    {
        return _MoveOut8(vi.DangerousGetHandle(), space, (uint)offset, length, buf8);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveOut8Ex")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveOut8Ex(nint vi, AddressSpace space, long offset, int length,
        [In] byte[] buf8);

    public static VisaStatus viMoveOut8Ex(VisaHandle vi, AddressSpace space, long offset, int length, byte[] buf8)
    {
        return _MoveOut8Ex(vi.DangerousGetHandle(), space, offset, length, buf8);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveOut16")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveOut16(nint vi, AddressSpace space, uint offset, int length,
        [In] short[] buf16);

    public static VisaStatus viMoveOut16(VisaHandle vi, AddressSpace space, int offset, int length, short[] buf16)
    {
        return _MoveOut16(vi.DangerousGetHandle(), space, (uint)offset, length, buf16);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveOut16Ex")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveOut16Ex(nint vi, AddressSpace space, long offset, int length,
        [In] short[] buf16);

    public static VisaStatus viMoveOut16Ex(VisaHandle vi, AddressSpace space, long offset, int length, short[] buf16)
    {
        return _MoveOut16(vi.DangerousGetHandle(), space, (uint)offset, length, buf16);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveOut")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveOut(nint vi, AddressSpace space, uint offset, int length, [In] int[] buf);

    public static VisaStatus viMoveOut(VisaHandle vi, AddressSpace space, int offset, int length, int[] buf)
    {
        return _MoveOut(vi.DangerousGetHandle(), space, (uint)offset, length, buf);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveOutEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveOutEx(nint vi, AddressSpace space, long offset, int length, [In] int[] buf);

    public static VisaStatus viMoveOutEx(VisaHandle vi, AddressSpace space, long offset, int length, int[] buf)
    {
        return _MoveOutEx(vi.DangerousGetHandle(), space, offset, length, buf);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveOut")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveOut(nint vi, AddressSpace space, uint offset, int length, [In] long[] buf);

    public static VisaStatus viMoveOut(VisaHandle vi, AddressSpace space, int offset, int length, long[] buf)
    {
        return _MoveOut(vi.DangerousGetHandle(), space, (uint)offset, length, buf);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveOutEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus
        _MoveOutEx(nint vi, AddressSpace space, long offset, int length, [In] long[] buf);

    public static VisaStatus viMoveOutEx(VisaHandle vi, AddressSpace space, long offset, int length, long[] buf)
    {
        return _MoveOutEx(vi.DangerousGetHandle(), space, offset, length, buf);
    }

    [LibraryImport(DllName, EntryPoint = "viOut8")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Out8(nint vi, AddressSpace space, uint offset, byte val8);

    public static VisaStatus viOut8(VisaHandle vi, AddressSpace space, int offset, byte val8)
    {
        return _Out8(vi.DangerousGetHandle(), space, (uint)offset, val8);
    }

    [LibraryImport(DllName, EntryPoint = "viOut8Ex")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Out8Ex(nint vi, AddressSpace space, long offset, byte val8);

    public static VisaStatus viOut8Ex(VisaHandle vi, AddressSpace space, long offset, byte val8)
    {
        return _Out8Ex(vi.DangerousGetHandle(), space, offset, val8);
    }

    [LibraryImport(DllName, EntryPoint = "viOut16")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Out16(nint vi, AddressSpace space, uint offset, short val16);

    public static VisaStatus viOut16(VisaHandle vi, AddressSpace space, int offset, short val16)
    {
        return _Out16(vi.DangerousGetHandle(), space, (uint)offset, val16);
    }

    [LibraryImport(DllName, EntryPoint = "viOut16Ex")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Out16Ex(nint vi, AddressSpace space, long offset, short val16);

    public static VisaStatus viOut16Ex(VisaHandle vi, AddressSpace space, long offset, short val16)
    {
        return _Out16Ex(vi.DangerousGetHandle(), space, offset, val16);
    }

    [LibraryImport(DllName, EntryPoint = "viOut")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Out(nint vi, AddressSpace space, uint offset, int val);

    public static VisaStatus viOut(VisaHandle vi, AddressSpace space, int offset, int val)
    {
        return _Out(vi.DangerousGetHandle(), space, (uint)offset, val);
    }

    [LibraryImport(DllName, EntryPoint = "viOutEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _OutEx(nint vi, AddressSpace space, long offset, int val);

    public static VisaStatus viOutEx(VisaHandle vi, AddressSpace space, long offset, int val)
    {
        return _OutEx(vi.DangerousGetHandle(), space, offset, val);
    }

    [LibraryImport(DllName, EntryPoint = "viOut")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Out(nint vi, AddressSpace space, uint offset, long val);

    public static VisaStatus viOut(VisaHandle vi, AddressSpace space, int offset, long val)
    {
        return _Out(vi.DangerousGetHandle(), space, (uint)offset, val);
    }

    [LibraryImport(DllName, EntryPoint = "viOutEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _OutEx(nint vi, AddressSpace space, long offset, long val);

    public static VisaStatus viOutEx(VisaHandle vi, AddressSpace space, long offset, long val)
    {
        return _OutEx(vi.DangerousGetHandle(), space, offset, val);
    }

    [LibraryImport(DllName, EntryPoint = "viMove")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _Move(nint vi, AddressSpace srcSpace, uint srcOffset, DataWidth srcWidth,
        AddressSpace destSpace, uint destOffset, DataWidth destWidth, int length);

    public static VisaStatus viMove(VisaHandle vi, AddressSpace srcSpace, int srcOffset, DataWidth srcWidth,
        AddressSpace destSpace, int destOffset, DataWidth destWidth, int length)
    {
        return _Move(vi.DangerousGetHandle(), srcSpace, (uint)srcOffset, srcWidth, destSpace, (uint)destOffset,
            destWidth, length);
    }

    [LibraryImport(DllName, EntryPoint = "viMoveEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MoveEx(nint vi, AddressSpace srcSpace, long srcOffset, DataWidth srcWidth,
        AddressSpace destSpace, long destOffset, DataWidth destWidth, int length);

    public static VisaStatus viMoveEx(VisaHandle vi, AddressSpace srcSpace, long srcOffset, DataWidth srcWidth,
        AddressSpace destSpace, long destOffset, DataWidth destWidth, int length)
    {
        return _MoveEx(vi.DangerousGetHandle(), srcSpace, srcOffset, srcWidth, destSpace, destOffset, destWidth,
            length);
    }

    [LibraryImport(DllName, EntryPoint = "viVxiCommandQuery")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _VxiCommandQuery(nint vi, CommandMode mode, int cmd, out int response);

    public static VisaStatus viVxiCommandQuery(VisaHandle vi, CommandMode mode, int cmd, out int response)
    {
        return _VxiCommandQuery(vi.DangerousGetHandle(), mode, cmd, out response);
    }

    [LibraryImport(DllName, EntryPoint = "viMemAlloc")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MemAlloc(nint vi, int size, out int offset);

    public static VisaStatus viMemAlloc(VisaHandle vi, int size, out int offset)
    {
        return _MemAlloc(vi.DangerousGetHandle(), size, out offset);
    }

    [LibraryImport(DllName, EntryPoint = "viMemAllocEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MemAllocEx(nint vi, int size, out long offset);

    public static VisaStatus viMemAllocEx(VisaHandle vi, int size, out long offset)
    {
        return _MemAllocEx(vi.DangerousGetHandle(), size, out offset);
    }

    [LibraryImport(DllName, EntryPoint = "viMemFree")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MemFree(nint vi, uint offset);

    public static VisaStatus viMemFree(VisaHandle vi, int offset)
    {
        return _MemFree(vi.DangerousGetHandle(), (uint)offset);
    }

    [LibraryImport(DllName, EntryPoint = "viMemFreeEx")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MemFreeEx(nint vi, long offset);

    public static VisaStatus viMemFreeEx(VisaHandle vi, long offset)
    {
        return _MemFreeEx(vi.DangerousGetHandle(), offset);
    }

    [LibraryImport(DllName, EntryPoint = "viAssertIntrSignal")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _AssertIntrSignal(nint vi, Interrupt mode, int statusID);

    public static VisaStatus viAssertIntrSignal(VisaHandle vi, Interrupt mode, int statusID)
    {
        return _AssertIntrSignal(vi.DangerousGetHandle(), mode, statusID);
    }

    [LibraryImport(DllName, EntryPoint = "viAssertUtilSignal")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _AssertUtilSignal(nint vi, UtilitySignal line);

    public static VisaStatus viAssertUtilSignal(VisaHandle vi, UtilitySignal line)
    {
        return _AssertUtilSignal(vi.DangerousGetHandle(), line);
    }

    [LibraryImport(DllName, EntryPoint = "viMapTrigger")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _MapTrigger(nint vi, TriggerLine trigSrc, TriggerLine trigDest, short mode);

    public static VisaStatus viMapTrigger(VisaHandle vi, TriggerLine trigSrc, TriggerLine trigDest)
    {
        return _MapTrigger(vi.DangerousGetHandle(), trigSrc, trigDest, 0);
    }

    [LibraryImport(DllName, EntryPoint = "viUnmapTrigger")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _UnmapTrigger(nint vi, TriggerLine trigSrc, TriggerLine trigDest);

    public static VisaStatus viUnmapTrigger(VisaHandle vi, TriggerLine trigSrc, TriggerLine trigDest)
    {
        return _UnmapTrigger(vi.DangerousGetHandle(), trigSrc, trigDest);
    }

    [LibraryImport(DllName, EntryPoint = "viUsbControlOut")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _UsbControlOut(nint vi, short bmRequestType, short bRequest, short wValue,
        short wIndex, short wLength, [In] byte[] buf);

    public static VisaStatus viUsbControlOut(VisaHandle vi, short bmRequestType, short bRequest, short wValue,
        short wIndex, short wLength, byte[] buf)
    {
        return _UsbControlOut(vi.DangerousGetHandle(), bmRequestType, bRequest, wValue, wIndex, wLength, buf);
    }

    [LibraryImport(DllName, EntryPoint = "viUsbControlIn")]
#if Windows
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
#endif
    internal static partial VisaStatus _UsbControlIn(nint vi, short bmRequestType, short bRequest, short wValue,
        short wIndex, short wLength, [Out] byte[] buf, out short retCnt);

    public static VisaStatus viUsbControlIn(VisaHandle vi, short bmRequestType, short bRequest, short wValue,
        short wIndex, short wLength, byte[] buf, out short retCnt)
    {
        return _UsbControlIn(vi.DangerousGetHandle(), bmRequestType, bRequest, wValue, wIndex, wLength, buf,
            out retCnt);
    }
#pragma warning restore IDE1006
}

public class VisaHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public VisaHandle(int handle) : base(true)
    {
        SetHandle(new nint(handle));
    }

    public VisaHandle(nint handle) : base(true)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        if (handle != nint.Zero) NativeMethods._Close(handle);

        return true;
    }
}