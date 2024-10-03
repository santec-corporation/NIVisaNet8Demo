using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using NIVisaNet8Demo.NI.VISA;
using NIVisaNet8Demo.NI.VISA.Enums;

var status = NativeMethods.viOpenDefaultRM(out var sesn);
const string instrQuery = "GPIB?*INSTR";
const string idnQuery = "*IDN?";

Instrument? TSL = null;
Instrument? MPM = null;

const byte powerUnit = 0; // 0: dBm, 1: mW
const double power = 5D;
const double startWavelength = 1520D;
const double stopWavelength = 1580D;
const double stepWavelength = 1D;
const double sweepSpeed = 50D;
const int delay = 0;
const int cycle = 1;

using var session = sesn;
if (session == null || status < 0) return;
var instruments = QueryDevices(session);
foreach (var instrument in instruments)
    if (instrument.IsTSL)
        TSL = instrument;
    else if (instrument.IsMPM) MPM = instrument;

if (TSL == null || MPM == null)
{
    Console.WriteLine("Instrument not found");
    return;
}

SetupTSL(session, TSL);
SetupMPM(session, MPM);
PrepareTSL(session, TSL);
Start(session, TSL, MPM);

return;


static List<Instrument> QueryDevices(in VisaHandle handle)
{
    var list = new List<Instrument>();
    var status = NativeMethods.viFindRsrc(handle, instrQuery, out var hnd, out var cnt, out var desc);
    if (hnd == null || string.IsNullOrEmpty(desc) || status < 0) return list;

    if (cnt <= 0) return list;
    if (QueryManufacturer(handle, desc, out var instr)) list.Add(instr);

    for (var i = 1; i < cnt; i++)
    {
        status = NativeMethods.viFindNext(hnd, out desc);
        if (string.IsNullOrEmpty(desc) || status < 0) continue;
        if (QueryManufacturer(handle, desc, out instr)) list.Add(instr);
    }

    return list;
}

static bool QueryManufacturer(in VisaHandle handle, in string desc, [NotNullWhen(true)] out Instrument? instrument)
{
    var status = NativeMethods.viOpen(handle, desc, AccessModes.VI_NO_LOCK, 2000, out var hnd);
    instrument = null;
    if (hnd == null || status < 0)
    {
        Console.WriteLine($"Visa status: {status}");
        return false;
    }

    using var h = hnd;
    if (!VisaGetString(h, idnQuery, out var result, 256)) return false;

    var data = result.Split(',');
    instrument = new Instrument(desc, data[0], data[1], data[2], data[3]);
    return true;
}

void SetupTSL(VisaHandle handle, Instrument tsl)
{
    var visaStatus = NativeMethods.viOpen(handle, tsl.Description, AccessModes.VI_NO_LOCK, 2000, out var hnd);
    if (hnd == null || visaStatus < 0)
    {
        Console.WriteLine($"Visa status: {visaStatus}");
        return;
    }

    using (hnd)
    {
        #region Initialize setting

        VisaSendCommand(hnd, "*CLS");
        VisaSendCommand(hnd, "*RST");
        Console.WriteLine("Checking LD status");
        if (!VisaGetString(hnd, ":POW:STAT?", out var ldStatus, 4)) return;
        if (ldStatus == "0")
        {
            Console.Write("LD is off, turn it on\r");
            if (!VisaSendCommand(hnd, ":POW:STAT 1")) return;
            while (ldStatus == "0")
            {
                if (!VisaGetString(hnd, ":POW:STAT?", out ldStatus, 4)) return;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            Console.WriteLine("LD is on");
        }
        else
        {
            Console.WriteLine("LD is on");
        }

        if (!VisaGetString(hnd, ":POW:SHUT?", out var shutterStatus, 4)) return;
        if (shutterStatus == "0")
            if (!VisaSendCommand(hnd, ":POW:SHUT 1"))
                return;
        if (!VisaSendCommand(hnd, ":POW:ATT:AUT 1")) return;
        if (!VisaSendCommand(hnd, $":POW:UNIT {powerUnit}")) return;
        if (!VisaSendCommand(hnd, $":POW {power}")) return;

        #endregion

        #region Condition setting

        if (!VisaSendCommand(hnd, ":TRIG:OUTP 3")) return;
        if (!VisaSendCommand(hnd, "TRIG:INP:EXT 0")) return;
        if (!VisaSendCommand(hnd, ":WAV:SWE:MOD 1")) return;
        if (!VisaSendCommand(hnd, $":WAV:SWE:STAR {startWavelength}")) return;
        if (!VisaSendCommand(hnd, $":WAV:SWE:STOP {stopWavelength}")) return;
        if (!VisaSendCommand(hnd, $":WAV:SWE:SPE {sweepSpeed}")) return;
        if (!VisaSendCommand(hnd, $":WAV:SWE:DEL {delay}")) return;
        if (!VisaSendCommand(hnd, $":WAV:SWE:CYCL {cycle}")) return;
        if (!VisaSendCommand(hnd, $":TRIG:OUTP:STEP {stepWavelength}")) return;

        #endregion
    }
}

void SetupMPM(VisaHandle handle, Instrument mpm)
{
    var visaStatus = NativeMethods.viOpen(handle, mpm.Description, AccessModes.VI_NO_LOCK, 2000, out var hnd);
    using (hnd)
    {
        if (hnd == null || visaStatus < 0)
        {
            Console.WriteLine($"Visa status: {visaStatus}");
            return;
        }

        Console.Write("Zeroing");
        if (!VisaSendCommand(hnd, "ZERO")) return;
        Thread.Sleep(TimeSpan.FromSeconds(3));
        Console.WriteLine(" finished");
        if (!VisaSendCommand(hnd, $"UNIT {powerUnit}")) return;
        if (!VisaSendCommand(hnd, "LEV 5")) return;
        if (!VisaSendCommand(hnd, "WMOD SWEEP1")) return;
        if (!VisaSendCommand(hnd, $"WSET {startWavelength},{stopWavelength},{stepWavelength}")) return;
        if (!VisaSendCommand(hnd, $"SPE {sweepSpeed}")) return;
        if (!VisaSendCommand(hnd, "TRIG 1")) return;
    }
}

static void PrepareTSL(VisaHandle handle, Instrument tsl)
{
    var status = NativeMethods.viOpen(handle, tsl.Description, AccessModes.VI_NO_LOCK, 2000, out var hnd);
    using (hnd)
    {
        if (hnd == null || status < 0)
        {
            Console.WriteLine($"Visa status: {status}");
            return;
        }

        if (!VisaSendCommand(hnd, ":TRIG:INP:STAN 1")) return;
        if (!VisaSendCommand(hnd, ":WAV:SWE 1")) return;
        if (!VisaGetString(hnd, ":WAV:SWE?", out var sweepStatus, 4)) return;
        while (sweepStatus != "3")
        {
            status = NativeMethods.viRead(hnd, out sweepStatus, 4, out _);
            if (status < 0)
            {
                Console.WriteLine($"Visa status: {status}");
                return;
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }
    }
}

static bool VisaGetString(in VisaHandle handle, in string command, [NotNullWhen(true)] out string? result,
    in int size)
{
    var status = NativeMethods.viWrite(handle, command, out _);
    if (status < 0)
    {
        Console.WriteLine($"Writing [{command}] | Visa status: {status}");
        result = null;
        return false;
    }

    status = NativeMethods.viRead(handle, out result, size, out _);
    if (status < 0)
    {
        Console.WriteLine($"Reading [{command}] | Visa status: {status}");
        result = null;
        return false;
    }

    result ??= string.Empty;

    return true;
}

static bool VisaGetByteArray(in VisaHandle handle, in string command, byte[] result, out int retcnt)
{
    var status = NativeMethods.viWrite(handle, command, out _);
    if (status < 0)
    {
        retcnt = 0;
        Console.WriteLine($"Writing [{command}] | Visa status: {status}");
        return false;
    }

    status = NativeMethods.viRead(handle, result, result.Length, out retcnt);
    if (status < 0)
    {
        Console.WriteLine($"Reading [{command}] | Visa status: {status}");
        return false;
    }

    return true;
}

void Start(VisaHandle handle, Instrument tsl, Instrument mpm)
{
    var retcnt = 0;

    var visaStatus = NativeMethods.viOpen(handle, tsl.Description, AccessModes.VI_NO_LOCK, 2000, out var tslHnd);
    if (tslHnd == null || visaStatus < 0)
    {
        Console.WriteLine($"Visa status for TSL: {visaStatus}");
        return;
    }

    visaStatus = NativeMethods.viOpen(handle, mpm.Description, AccessModes.VI_NO_LOCK, 2000, out var mpmHnd);
    if (mpmHnd == null || visaStatus < 0)
    {
        Console.WriteLine($"Visa status for MPM: {visaStatus}");
        return;
    }

    using var tslHandle = tslHnd;
    using var mpmHandle = mpmHnd;
    Console.WriteLine("Check Shutter status");
    if (!VisaGetString(tslHandle, ":POW:SHUT?", out var shutterStatus, 4)) return;
    if (shutterStatus == "1")
    {
        if (!VisaSendCommand(tslHandle, ":POW:SHUT 0")) return;
        Console.WriteLine("Shutter is off");
    }
    else
    {
        Console.WriteLine("Shutter is off");
    }

    Console.WriteLine("Start measuring");
    if (!VisaSendCommand(mpmHandle, "MEAS"))
    {
        Stop(tslHandle, mpmHandle);
        return;
    }

    if (!VisaSendCommand(tslHandle, ":WAV:SWE:SOFT"))
    {
        Stop(tslHandle, mpmHandle);
        return;
    }

    if (!VisaGetString(mpmHandle, "STAT?", out var mpmStatus, 16))
    {
        Stop(tslHandle, mpmHandle);
        return;
    }

    //if (string.IsNullOrEmpty(mpmStatus))
    //{
    //    VisaSendCommand(mpmHandle, "STOP");
    //    VisaSendCommand(tslHandle, "*CLS");
    //    VisaSendCommand(tslHandle, "*RST");
    //    return;
    //}
    var logPoint = string.Empty;
    var steps = (int)Math.Ceiling((stopWavelength - startWavelength) / stepWavelength);
    var dataLength = (steps + 1) * 4;
    var digits = 1 + (int)Math.Log10(dataLength);
    var length = dataLength + 2 + digits;
    var results = new List<byte[]>(steps);
    while (mpmStatus.StartsWith('0'))
    {
        var point = mpmStatus[2..];
        var buffer = new byte[length];
        if (!VisaGetByteArray(mpmHandle, "LOGG? 0,1", buffer, out _))
        {
            Stop(tslHandle, mpmHandle);
            return;
        }

        if (length < retcnt)
        {
            Console.WriteLine($"Length overflow: {length} < {retcnt}");
            Stop(tslHandle, mpmHandle);
            return;
        }

        if (logPoint != point)
        {
            Console.Write($"Measuring {point}, size: {retcnt}\r");
            results.Add(buffer);
            logPoint = point;
        }

        if (!VisaGetString(mpmHandle, "STAT?", out mpmStatus, 16))
        {
            Stop(tslHandle, mpmHandle);
            return;
        }
    }

    Console.WriteLine();
    if (!VisaSendCommand(mpmHandle, "STOP")) return;
    Console.WriteLine("Measurement finished, turn shutter on");
    VisaSendCommand(tslHandle, ":POW:SHUT 1");

    ProcessData(results);
}

void ProcessData(List<byte[]> results)
{
    if (results.Count == 0) return;
    using var file = File.Create($"result-{DateTimeOffset.Now:yyyyMMddHHmmss}.csv");
    using var writer = new StreamWriter(file);
    var length = 0;
    var digits = 0;
    for (var i = 0; i < results.Count; i++)
    {
        Span<byte> data = results[i];
        if (i == 0)
        {
            if (data[0] != '#') return;
            digits = data[1] - '0';
            var len = 0;
            for (var j = 0; j < digits; j++) len = len * 10 + (data[2 + j] - '0');
            length = len / 4;
        }

        ReadOnlySpan<float> result = MemoryMarshal.Cast<byte, float>(data[(2 + digits)..]);

        Console.Write($"{startWavelength + stepWavelength * i}");
        writer.Write($"{startWavelength + stepWavelength * i}");
        for (var j = 0; j < length; j++)
        {
            if (result[j] == 0) continue;
            Console.Write($" {result[j]}");
            writer.Write($",{result[j]}");
        }

        Console.WriteLine();
        writer.WriteLine();
    }

    writer.Flush();
}

static bool VisaSendCommand(in VisaHandle handle, in string command)
{
    var status = NativeMethods.viWrite(handle, command, out _);
    if (status == VisaStatus.VI_SUCCESS) return true;
    Console.WriteLine($"Writing [{command}] | Visa status: {status}");
    return false;
}

static void Stop(VisaHandle tsl, VisaHandle mpm)
{
    VisaSendCommand(mpm, "STOP");
    VisaSendCommand(tsl, "*CLS");
    VisaSendCommand(tsl, "*RST");
}

internal record Instrument(string Description, string Vendor, string Model, string SerialNumber, string FirmwareVersion)
{
    public bool IsTSL => Model.StartsWith("TSL");
    public bool IsMPM => Model.StartsWith("MPM");
}