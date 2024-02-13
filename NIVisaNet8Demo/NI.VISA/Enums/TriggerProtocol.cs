namespace NIVisaNet8Demo.NI.VISA.Enums;

public enum TriggerProtocol : byte
{
    VI_TRIG_PROT_DEFAULT = 0,
    VI_TRIG_PROT_ON = 1,
    VI_TRIG_PROT_OFF = 2,
    VI_TRIG_PROT_SYNC = 5,
    VI_TRIG_PROT_RESERVE = 6,
    VI_TRIG_PROT_UNRESERVE = 7
}