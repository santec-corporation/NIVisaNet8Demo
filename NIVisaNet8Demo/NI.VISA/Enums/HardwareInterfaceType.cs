namespace NIVisaNet8Demo.NI.VISA.Enums;

public enum HardwareInterfaceType : byte
{
    VI_INTF_GPIB = 1,
    VI_INTF_VXI = 2,
    VI_INTF_GPIB_VXI = 3,
    VI_INTF_ASRL = 4,
    VI_INTF_PXI = 5,
    VI_INTF_TCPIP = 6,
    VI_INTF_USB = 7,
    VI_INTF_FIREWIRE = 9
}