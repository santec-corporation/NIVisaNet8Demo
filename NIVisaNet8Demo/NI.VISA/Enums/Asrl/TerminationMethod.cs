namespace NIVisaNet8Demo.NI.VISA.Enums.Asrl;

public enum TerminationMethod : byte
{
    VI_ASRL_END_NONE = 0,
    VI_ASRL_END_LAST_BIT = 1,
    VI_ASRL_END_TERMCHAR = 2,
    VI_ASRL_END_BREAK = 3
}