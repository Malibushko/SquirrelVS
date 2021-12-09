using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.DefaultPort;
using Microsoft.VisualStudio.Debugger.Native;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

namespace SquirrelDebugEngine
{
  public class BatchRead
  {
    protected ulong address = 0;
    protected byte[] data = null;

    public BatchRead(ulong address, byte[] data)
    {
      this.address = address;
      this.data = data;
    }

    static public BatchRead Create(DkmProcess process, ulong address, int bytes)
    {
      if (bytes <= 0 || bytes > 8 * 1024 * 1024)
        return null;

      byte[] data = new byte[bytes];

      try
      {
        if (process.ReadMemory(address, DkmReadMemoryFlags.None, data) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      return new BatchRead(address, data);
    }

    public bool TryRead(ulong target, int length, out byte[] result)
    {
      result = null;

      if (target < address)
        return false;

      if (target + (ulong)length > address + (ulong)data.Length)
        return false;

      result = new byte[length];
      Array.Copy(data, (int)(target - address), result, 0, length);
      return true;
    }
  }

  static class Utility
  {
    internal static T GetOrCreateDataItem<T>(
          DkmDataContainer _Container
        ) where T : DkmDataItem, new()
    {
      T DataItem = _Container.GetDataItem<T>();

      if (DataItem != null)
        return DataItem;

      DataItem = new T();

      _Container.SetDataItem<T>(DkmDataCreationDisposition.CreateNew, DataItem);

      return DataItem;
    }

    internal static ulong FindFunctionAddress(
          DkmRuntimeInstance _RuntimeInstance, 
          string             _FunctionName
        )
    {
      foreach (var Module in _RuntimeInstance.GetModuleInstances())
      {
        var FunctionAddress = (Module as DkmNativeModuleInstance)?.FindExportName(_FunctionName, IgnoreDataExports: true);

        if (FunctionAddress != null)
          return FunctionAddress.CPUInstructionPart.InstructionPointer;
      }

      return 0;
    }

    internal static int GetPointerSize(DkmProcess process)
    {
      return 8;
    }

    internal static byte? ReadByteVariable(DkmProcess process, ulong address, BatchRead batch = null)
    {
      byte[] variableAddressData = new byte[1];

      try
      {
        if (batch != null && batch.TryRead(address, variableAddressData.Length, out byte[] batchData))
          variableAddressData = batchData;
        else if (process.ReadMemory(address, DkmReadMemoryFlags.None, variableAddressData) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      return variableAddressData[0];
    }

    internal static short? ReadShortVariable(DkmProcess process, ulong address, BatchRead batch = null)
    {
      byte[] variableAddressData = new byte[2];

      try
      {
        if (batch != null && batch.TryRead(address, variableAddressData.Length, out byte[] batchData))
          variableAddressData = batchData;
        else if (process.ReadMemory(address, DkmReadMemoryFlags.None, variableAddressData) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      return BitConverter.ToInt16(variableAddressData, 0);
    }

    internal static int? ReadIntVariable(DkmProcess process, ulong address, BatchRead batch = null)
    {
      byte[] variableAddressData = new byte[4];

      try
      {
        if (batch != null && batch.TryRead(address, variableAddressData.Length, out byte[] batchData))
          variableAddressData = batchData;
        else if (process.ReadMemory(address, DkmReadMemoryFlags.None, variableAddressData) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      return BitConverter.ToInt32(variableAddressData, 0);
    }

    internal static uint? ReadUintVariable(DkmProcess process, ulong address, BatchRead batch = null)
    {
      byte[] variableAddressData = new byte[4];

      try
      {
        if (batch != null && batch.TryRead(address, variableAddressData.Length, out byte[] batchData))
          variableAddressData = batchData;
        else if (process.ReadMemory(address, DkmReadMemoryFlags.None, variableAddressData) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      return BitConverter.ToUInt32(variableAddressData, 0);
    }

    internal static long? ReadLongVariable(DkmProcess process, ulong address, BatchRead batch = null)
    {
      byte[] variableAddressData = new byte[8];

      try
      {
        if (batch != null && batch.TryRead(address, variableAddressData.Length, out byte[] batchData))
          variableAddressData = batchData;
        else if (process.ReadMemory(address, DkmReadMemoryFlags.None, variableAddressData) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      return BitConverter.ToInt64(variableAddressData, 0);
    }

    internal static ulong? ReadUlongVariable(DkmProcess process, ulong address, BatchRead batch = null)
    {
      byte[] variableAddressData = new byte[8];

      try
      {
        if (batch != null && batch.TryRead(address, variableAddressData.Length, out byte[] batchData))
          variableAddressData = batchData;
        else if (process.ReadMemory(address, DkmReadMemoryFlags.None, variableAddressData) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      return BitConverter.ToUInt64(variableAddressData, 0);
    }

    internal static float? ReadFloatVariable(DkmProcess process, ulong address, BatchRead batch = null)
    {
      byte[] variableAddressData = new byte[4];

      try
      {
        if (batch != null && batch.TryRead(address, variableAddressData.Length, out byte[] batchData))
          variableAddressData = batchData;
        else if (process.ReadMemory(address, DkmReadMemoryFlags.None, variableAddressData) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      return BitConverter.ToSingle(variableAddressData, 0);
    }

    internal static double? ReadDoubleVariable(DkmProcess process, ulong address, BatchRead batch = null)
    {
      byte[] variableAddressData = new byte[8];

      try
      {
        if (batch != null && batch.TryRead(address, variableAddressData.Length, out byte[] batchData))
          variableAddressData = batchData;
        else if (process.ReadMemory(address, DkmReadMemoryFlags.None, variableAddressData) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      return BitConverter.ToDouble(variableAddressData, 0);
    }

    internal static ulong? ReadPointerVariable(DkmProcess process, ulong address, BatchRead batch = null)
    {
      return ReadUlongVariable(process, address, batch);
    }

    internal static byte[] ReadRawStringVariable(DkmProcess process, ulong address, int limit)
    {
      try
      {
        return process.ReadMemoryString(address, DkmReadMemoryFlags.AllowPartialRead, 1, limit);
      }
      catch (DkmException)
      {
      }

      return null;
    }

    internal static string ReadStringVariable(DkmProcess process, ulong address, int limit)
    {
      try
      {
        byte[] nameData = process.ReadMemoryString(address, DkmReadMemoryFlags.AllowPartialRead, 1, limit);

        if (nameData != null && nameData.Length != 0)
          return System.Text.Encoding.UTF8.GetString(nameData, 0, nameData.Length - 1);
      }
      catch (DkmException)
      {
        return null;
      }

      return null;
    }

    internal static ulong? ReadPointerVariable(DkmProcess process, string name)
    {
      var runtimeInstance = process.GetNativeRuntimeInstance();

      if (runtimeInstance != null)
      {
        foreach (var module in runtimeInstance.GetModuleInstances())
        {
          var nativeModule = module as DkmNativeModuleInstance;

          var variableAddress = nativeModule?.FindExportName(name, IgnoreDataExports: false);

          if (variableAddress != null)
            return ReadPointerVariable(process, variableAddress.CPUInstructionPart.InstructionPointer);
        }
      }

      return null;
    }

    internal static ulong? ReadUleb128Variable(DkmProcess process, ulong address, out ulong length, BatchRead batch = null)
    {
      byte[] variableAddressData = new byte[8];

      length = 0;

      try
      {
        if (batch != null && batch.TryRead(address, variableAddressData.Length, out byte[] batchData))
          variableAddressData = batchData;
        else if (process.ReadMemory(address, DkmReadMemoryFlags.None, variableAddressData) == 0)
          return null;
      }
      catch (DkmException)
      {
        return null;
      }

      ulong pos = 0;
      ulong v = variableAddressData[pos++];

      if (v >= 0x80)
      {
        int sh = 0;
        v &= 0x7f;

        do
        {
          sh += 7;
          v |= (variableAddressData[pos] & 0x7fu) << sh;
        }
        while (variableAddressData[pos++] >= 0x80);
      }

      length = pos;
      return v;
    }

    internal static bool TryWriteRawBytes(DkmProcess process, ulong address, byte[] value)
    {
      try
      {
        process.WriteMemory(address, value);
      }
      catch (DkmException)
      {
        return false;
      }

      return true;
    }

    internal static bool TryWriteByteVariable(DkmProcess process, ulong address, byte value)
    {
      try
      {
        process.WriteMemory(address, new byte[1] { value });
      }
      catch (DkmException)
      {
        return false;
      }

      return true;
    }

    internal static bool TryWriteShortVariable(DkmProcess process, ulong address, short value)
    {
      try
      {
        process.WriteMemory(address, BitConverter.GetBytes(value));
      }
      catch (DkmException)
      {
        return false;
      }

      return true;
    }

    internal static bool TryWriteIntVariable(DkmProcess process, ulong address, int value)
    {
      try
      {
        process.WriteMemory(address, BitConverter.GetBytes(value));
      }
      catch (DkmException)
      {
        return false;
      }

      return true;
    }

    internal static bool TryWriteUintVariable(DkmProcess process, ulong address, uint value)
    {
      try
      {
        process.WriteMemory(address, BitConverter.GetBytes(value));
      }
      catch (DkmException)
      {
        return false;
      }

      return true;
    }

    internal static bool TryWriteLongVariable(DkmProcess process, ulong address, long value)
    {
      try
      {
        process.WriteMemory(address, BitConverter.GetBytes(value));
      }
      catch (DkmException)
      {
        return false;
      }

      return true;
    }

    internal static bool TryWriteUlongVariable(DkmProcess process, ulong address, ulong value)
    {
      try
      {
        process.WriteMemory(address, BitConverter.GetBytes(value));
      }
      catch (DkmException)
      {
        return false;
      }

      return true;
    }

    internal static bool TryWriteFloatVariable(DkmProcess process, ulong address, float value)
    {
      try
      {
        process.WriteMemory(address, BitConverter.GetBytes(value));
      }
      catch (DkmException)
      {
        return false;
      }

      return true;
    }

    internal static bool TryWriteDoubleVariable(DkmProcess process, ulong address, double value)
    {
      try
      {
        process.WriteMemory(address, BitConverter.GetBytes(value));
      }
      catch (DkmException)
      {
        return false;
      }

      return true;
    }

    internal static bool TryWritePointerVariable(DkmProcess process, ulong address, ulong value)
    {
      return TryWriteUlongVariable(process, address, value);
    }

    internal static byte? ReadStructByte(DkmProcess process, ref ulong address, BatchRead batch = null)
    {
      var result = ReadByteVariable(process, address, batch);

      address += 1ul;

      return result;
    }

    internal static short? ReadStructShort(DkmProcess process, ref ulong address, BatchRead batch = null)
    {
      address = (address + 1ul) & ~1ul; // Align

      var result = ReadShortVariable(process, address, batch);

      address += 2ul;

      return result;
    }

    internal static int? ReadStructInt(DkmProcess process, ref ulong address, BatchRead batch = null)
    {
      address = (address + 3ul) & ~3ul; // Align

      var result = ReadIntVariable(process, address, batch);

      address += 4ul;

      return result;
    }

    internal static uint? ReadStructUint(DkmProcess process, ref ulong address, BatchRead batch = null)
    {
      address = (address + 3ul) & ~3ul; // Align

      var result = ReadUintVariable(process, address, batch);

      address += 4ul;

      return result;
    }

    internal static long? ReadStructLong(DkmProcess process, ref ulong address, BatchRead batch = null)
    {
      address = (address + 7ul) & ~7ul; // Align

      var result = ReadLongVariable(process, address, batch);

      address += 8ul;

      return result;
    }

    internal static ulong? ReadStructUlong(DkmProcess process, ref ulong address, BatchRead batch = null)
    {
      address = (address + 7ul) & ~7ul; // Align

      var result = ReadUlongVariable(process, address, batch);

      address += 8ul;

      return result;
    }

    internal static ulong? ReadStructPointer(DkmProcess process, ref ulong address, BatchRead batch = null)
    {
      return ReadStructUlong(process, ref address, batch);
    }

    internal static void SkipStructByte(DkmProcess process, ref ulong address)
    {
      address += 1ul;
    }

    internal static void SkipStructShort(DkmProcess process, ref ulong address)
    {
      address = (address + 1ul) & ~1ul; // Align

      address += 2ul;
    }

    internal static void SkipStructInt(DkmProcess process, ref ulong address)
    {
      address = (address + 3ul) & ~3ul; // Align

      address += 4ul;
    }

    internal static void SkipStructUint(DkmProcess process, ref ulong address)
    {
      address = (address + 3ul) & ~3ul; // Align

      address += 4ul;
    }

    internal static void SkipStructLong(DkmProcess process, ref ulong address)
    {
      address = (address + 7ul) & ~7ul; // Align

      address += 8ul;
    }

    internal static void SkipStructUlong(DkmProcess process, ref ulong address)
    {
      address = (address + 7ul) & ~7ul; // Align

      address += 8ul;
    }

    internal static void SkipStructPointer(DkmProcess process, ref ulong address)
    {
      SkipStructUlong(process, ref address);
    }

    // Convert an object to a byte array
    internal static byte[] ObjectToByteArray(Object obj)
    {
      if (obj == null)
        return null;

      BinaryFormatter bf = new BinaryFormatter();
      MemoryStream ms = new MemoryStream();
      bf.Serialize(ms, obj);

      return ms.ToArray();
    }

    // Convert a byte array to an Object
    internal static Object ByteArrayToObject(byte[] arrBytes)
    {
      MemoryStream memStream = new MemoryStream();
      BinaryFormatter binForm = new BinaryFormatter();
      memStream.Write(arrBytes, 0, arrBytes.Length);
      memStream.Seek(0, SeekOrigin.Begin);
      Object obj = (Object)binForm.Deserialize(memStream);

      return obj;
    }
  }
}
