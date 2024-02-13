namespace NIVisaNet8Demo.NI.VISA.Enums.Asrl;

[Flags]
public enum FlowControlTypes : byte
{
    VI_ASRL_FLOW_NONE = 0,
    VI_ASRL_FLOW_XON_XOFF = 1,
    VI_ASRL_FLOW_RTS_CTS = 2,
    VI_ASRL_FLOW_DTR_DSR = 4
}