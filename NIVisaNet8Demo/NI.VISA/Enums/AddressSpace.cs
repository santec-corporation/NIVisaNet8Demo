namespace NIVisaNet8Demo.NI.VISA.Enums;

public enum AddressSpace : ushort
{
    VI_LOCAL_SPACE = 0,
    VI_A16_SPACE = 1,
    VI_A24_SPACE = 2,
    VI_A32_SPACE = 3,
    VI_A64_SPACE = 4,
    VI_FIREWIRE_DEFAULT = 5,
    VI_PXI_ALLOC_SPACE = 9,
    VI_PXI_CFG_SPACE = 10,
    VI_PXI_BAR0_SPACE = 11,
    VI_PXI_BAR1_SPACE = 12,
    VI_PXI_BAR2_SPACE = 13,
    VI_PXI_BAR3_SPACE = 14,
    VI_PXI_BAR4_SPACE = 15,
    VI_PXI_BAR5_SPACE = 16,
    VI_OPAQUE_SPACE = 0xFFFF
}