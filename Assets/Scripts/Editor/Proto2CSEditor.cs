using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
public class Proto2CSEditor 
{
          [MenuItem("Tools/Proto2CS")]
        public static void AllProto2CS()
        {
            string rootDir = Environment.CurrentDirectory;
            string protoDir = Path.Combine(rootDir, "Proto/");
 
            string protoc;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                protoc = Path.Combine(protoDir, "protoc.exe");
            }
            else
            {
                protoc = Path.Combine(protoDir, "protoc");
            }
 
            string hotfixMessageCodePath = Path.Combine(rootDir, "Assets", "Scripts", "ProtoMessage/");
 
            string argument2 = $"--csharp_out=\"{hotfixMessageCodePath}\" --proto_path=\"{protoDir}\" monitorData.proto";
 
            Run(protoc, argument2, waitExit: true);
 
            UnityEngine.Debug.Log("proto2cs succeed!");
 
            AssetDatabase.Refresh();
        }
 
        public static Process Run(string exe, string arguments, string workingDirectory = ".", bool waitExit = false)
        {
            try
            {
                bool redirectStandardOutput = true;
                bool redirectStandardError = true;
                bool useShellExecute = false;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    redirectStandardOutput = false;
                    redirectStandardError = false;
                    useShellExecute = true;
                }
 
                if (waitExit)
                {
                    redirectStandardOutput = true;
                    redirectStandardError = true;
                    useShellExecute = false;
                }
                
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = useShellExecute,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = redirectStandardOutput,
                    RedirectStandardError = redirectStandardError,
                };
                
                Process process = Process.Start(info);
 
                if (waitExit)
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"{process.StandardOutput.ReadToEnd()} {process.StandardError.ReadToEnd()}");
                    }
                }
 
                return process;
            }
            catch (Exception e)
            {
                throw new Exception($"dir: {Path.GetFullPath(workingDirectory)}, command: {exe} {arguments}", e);
            }
        }
}
