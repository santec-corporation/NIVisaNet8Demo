﻿namespace NIVisaNet8Demo.NI.VISA.Enums.Gpib;

public enum RenMode : byte
{
    VI_GPIB_REN_DEASSERT = 0,
    VI_GPIB_REN_ASSERT = 1,
    VI_GPIB_REN_DEASSERT_GTL = 2,
    VI_GPIB_REN_ASSERT_ADDRESS = 3,
    VI_GPIB_REN_ASSERT_LLO = 4,
    VI_GPIB_REN_ASSERT_ADDRESS_LLO = 5,
    VI_GPIB_REN_ADDRESS_GTL = 6
}