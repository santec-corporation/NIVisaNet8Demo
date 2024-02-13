namespace NIVisaNet8Demo.NI.VISA.Enums;

public enum AccessPrivilege : byte
{
    VI_DATA_PRIV = 0,
    VI_DATA_NPRIV = 1,
    VI_PROG_PRIV = 2,
    VI_PROG_NPRIV = 3,
    VI_BLCK_PRIV = 4,
    VI_BLCK_NPRIV = 5,
    VI_D64_PRIV = 6,
    VI_D64_NPRIV = 7,
    VI_D64_2EVME = 8,
    VI_D64_SST160 = 9,
    VI_D64_SST267 = 10,
    VI_D64_SST320 = 11
}