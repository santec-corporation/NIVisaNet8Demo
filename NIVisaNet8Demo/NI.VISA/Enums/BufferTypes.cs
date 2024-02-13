namespace NIVisaNet8Demo.NI.VISA.Enums;

[Flags]
public enum BufferTypes : byte
{
    VI_READ_BUF = 1,
    VI_WRITE_BUF = 2,
    VI_READ_BUF_DISCARD = 4,
    VI_WRITE_BUF_DISCARD = 8,
    VI_IO_IN_BUF = 16,
    VI_IO_OUT_BUF = 32,
    VI_IO_IN_BUF_DISCARD = 64,
    VI_IO_OUT_BUF_DISCARD = 128
}