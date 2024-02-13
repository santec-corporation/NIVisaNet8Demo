namespace NIVisaNet8Demo.NI.VISA.Enums;

[Flags]
public enum AccessModes : byte
{
    VI_NO_LOCK = 0,
    VI_EXCLUSIVE_LOCK = 1,
    VI_SHARED_LOCK = 2,
    VI_LOAD_CONFIG = 4
}