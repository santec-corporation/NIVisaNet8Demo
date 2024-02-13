namespace NIVisaNet8Demo.NI.VISA.Enums.Usb;

public enum PipeState : sbyte
{
    VI_USB_PIPE_STATE_UNKNOWN = -1,
    VI_USB_PIPE_READY = 0,
    VI_USB_PIPE_STALLED = 1
}