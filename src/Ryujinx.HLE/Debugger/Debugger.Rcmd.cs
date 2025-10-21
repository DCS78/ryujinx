using Gommon;
using JetBrains.Annotations;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Kernel.Process;
using Ryujinx.HLE.HOS.Kernel.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ryujinx.HLE.Debugger
{
    public partial class Debugger
    {
        static Debugger()
        {
            _rcmdDelegates.Add(["help"], 
                _ => _rcmdDelegates.Keys
                    .Where(x => !x[0].Equals("help"))
                    .Select(x => x.JoinToString('\n'))
                    .JoinToString('\n') + '\n'
                );
            _rcmdDelegates.Add(["get info"], dbgr => dbgr.GetProcessInfo());
            _rcmdDelegates.Add(["backtrace", "bt"], dbgr => dbgr.GetStackTrace());
            _rcmdDelegates.Add(["registers", "reg"], dbgr => dbgr.GetRegisters());
            _rcmdDelegates.Add(["minidump"], dbgr => dbgr.GetMinidump());
        }

        private static readonly Dictionary<string[], Func<Debugger, string>> _rcmdDelegates = new();

        [CanBeNull]
        public static Func<Debugger, string> FindRcmdDelegate(string command)
        {
            Func<Debugger, string> searchResult = null;

            foreach ((string[] names, Func<Debugger, string> dlg) in _rcmdDelegates)
            {
                if (names.ContainsIgnoreCase(command.Trim()))
                {
                    searchResult = dlg;
                    break;
                }
            }

            return searchResult;
        }

        public string GetStackTrace()
        {
            if (GThread == null)
                return "No thread selected\n";

            return Process?.Debugger?.GetGuestStackTrace(DebugProcess.GetThread(GThread.Value)) ?? "No application process found\n"; 
        }

        public string GetRegisters()
        {
            if (GThread == null)
                return "No thread selected\n";

            return Process?.Debugger?.GetCpuRegisterPrintout(DebugProcess.GetThread(GThread.Value)) ?? "No application process found\n";
        }

        public string GetMinidump()
        {
            StringBuilder response = new();
            response.AppendLine("=== Begin Minidump ===\n");
            response.AppendLine(GetProcessInfo());

            foreach (KThread thread in GetThreads())
            {
                response.AppendLine($"=== Thread {thread.ThreadUid} ===");
                try
                {
                    string stackTrace = Process.Debugger.GetGuestStackTrace(thread);
                    response.AppendLine(stackTrace);
                }
                catch (Exception e)
                {
                    response.AppendLine($"[Error getting stack trace: {e.Message}]");
                }

                try
                {
                    string registers = Process.Debugger.GetCpuRegisterPrintout(thread);
                    response.AppendLine(registers);
                }
                catch (Exception e)
                {
                    response.AppendLine($"[Error getting registers: {e.Message}]");
                }
            }

            response.AppendLine("=== End Minidump ===");

            Logger.Info?.Print(LogClass.GdbStub, response.ToString());
            return response.ToString();
        }

        public string GetProcessInfo()
        {
            try
            {
                if (Process is not { } kProcess)
                    return "No application process found\n";

                StringBuilder sb = new();

                sb.AppendLine($"Program Id:  0x{kProcess.TitleId:x16}");
                sb.AppendLine($"Application: {(kProcess.IsApplication ? 1 : 0)}");
                sb.AppendLine("Layout:");
                sb.AppendLine(
                    $"  Alias: 0x{kProcess.MemoryManager.AliasRegionStart:x10} - 0x{kProcess.MemoryManager.AliasRegionEnd - 1:x10}");
                sb.AppendLine(
                    $"  Heap:  0x{kProcess.MemoryManager.HeapRegionStart:x10} - 0x{kProcess.MemoryManager.HeapRegionEnd - 1:x10}");
                sb.AppendLine(
                    $"  Aslr:  0x{kProcess.MemoryManager.AslrRegionStart:x10} - 0x{kProcess.MemoryManager.AslrRegionEnd - 1:x10}");
                sb.AppendLine(
                    $"  Stack: 0x{kProcess.MemoryManager.StackRegionStart:x10} - 0x{kProcess.MemoryManager.StackRegionEnd - 1:x10}");

                sb.AppendLine("Modules:");
                HleProcessDebugger debugger = kProcess.Debugger;
                if (debugger != null)
                {
                    foreach (HleProcessDebugger.Image image in debugger.GetLoadedImages())
                    {
                        ulong endAddress = image.BaseAddress + image.Size - 1;
                        sb.AppendLine($"  0x{image.BaseAddress:x10} - 0x{endAddress:x10} {image.Name}");
                    }
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                Logger.Error?.Print(LogClass.GdbStub, $"Error getting process info: {e.Message}");
                return $"Error getting process info: {e.Message}\n";
            }
        }
    }
}
