namespace NIVisaNet8Demo.NI.VISA.Enums.Gpib;

public enum AtnMode : byte
{
    VI_GPIB_ATN_DEASSERT = 0,
    VI_GPIB_ATN_ASSERT = 1,
    VI_GPIB_ATN_DEASSERT_HANDSHAKE = 2,
    VI_GPIB_ATN_ASSERT_IMMEDIATE = 3
}