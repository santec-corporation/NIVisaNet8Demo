namespace NIVisaNet8Demo.NI.VISA.Enums;

public enum EventMechanism : ushort
{
    VI_QUEUE = 1,
    VI_HNDLR = 2,
    VI_SUSPEND_HNDLR = 4,
    VI_ALL_MECH = 0xFFFF
}