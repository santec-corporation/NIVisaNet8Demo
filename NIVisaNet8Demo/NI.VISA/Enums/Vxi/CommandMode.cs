namespace NIVisaNet8Demo.NI.VISA.Enums.Vxi;

public enum CommandMode : short
{
    VI_VXI_CMD16 = 0x0200,
    VI_VXI_CMD16_RESP16 = 0x0202,
    VI_VXI_RESP16 = 0x0002,
    VI_VXI_CMD32 = 0x0400,
    VI_VXI_CMD32_RESP16 = 0x0402,
    VI_VXI_CMD32_RESP32 = 0x0404,
    VI_VXI_RESP32 = 0x0004
}