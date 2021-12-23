using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine
{
  internal class ResolvedDocumentItem : DkmDataItem
  {
    public ScriptSymbols ScriptData;
  }

  public class ScriptSymbols
  {
    public string SourceName = null;
    public string ScriptContent = null;
    public byte[] SHA1 = null;

    public string ResolvedFilename = null;
  }

  public class SourceSymbols
  {
    public string SourceName       = null;
    public ulong  Address          = 0;
    public string ResolvedFilename = null;
  }

  public class SymbolsVM
  {
    public Dictionary<string, SourceSymbols> Sources        = new Dictionary<string, SourceSymbols>();
    public Dictionary<ulong, string>         FunctionNames  = new Dictionary<ulong, string>();
    public Dictionary<string, ScriptSymbols> Scripts        = new Dictionary<string, ScriptSymbols>();
    
    public SourceSymbols FetchSourceSymbols(
        string _SourceName
      )
    {
      if (Sources.ContainsKey(_SourceName))
        return Sources[_SourceName];

      return null;
    }

    public void AddFunctionName(
        ulong  _Address, 
        string _Name
      )
    {
      if (!FunctionNames.ContainsKey(_Address))
        FunctionNames.Add(_Address, _Name);
      else
        FunctionNames[_Address] = _Name;
    }

    public string FetchFunctionName(
        ulong _Address
      )
    {
      if (FunctionNames.ContainsKey(_Address))
        return FunctionNames[_Address];

      return null;
    }

    public void AddScriptSource(
        string ScriptName, 
        string ScriptContent, 
        byte[] SHA1
      )
    {
      if (!Scripts.ContainsKey(ScriptName))
        Scripts.Add(ScriptName, new ScriptSymbols { SourceName = ScriptName, ScriptContent = ScriptContent, SHA1 = SHA1 });
      else
        Scripts[ScriptName] = new ScriptSymbols { SourceName = ScriptName, ScriptContent = ScriptContent, SHA1 = SHA1 };
    }

    public ScriptSymbols FetchScriptSource(
        string _SourceName
      )
    {
      if (Scripts.ContainsKey(_SourceName))
        return Scripts[_SourceName];

      return null;
    }
  }

  public class SymbolStore
  {
    public Dictionary<ulong, SymbolsVM> SquirrelHandles = new Dictionary<ulong, SymbolsVM>();

    public SymbolsVM FetchOrCreate(
        ulong _StateAddress
      )
    {
      if (!SquirrelHandles.ContainsKey(_StateAddress))
        SquirrelHandles.Add(_StateAddress, new SymbolsVM());

      return SquirrelHandles[_StateAddress];
    }

    public void Remove(
        ulong _StateAddress
      )
    {
      SquirrelHandles.Remove(_StateAddress);
    }

    public SourceSymbols FetchSourceSymbols(
        string _SourceName
      )
    {
      foreach (var State in SquirrelHandles)
      {
        var SourceSymbols = State.Value.FetchSourceSymbols(_SourceName);

        if (SourceSymbols != null)
          return SourceSymbols;
      }

      return null;
    }

    public ScriptSymbols FetchScriptSource(
        string _SourceName
      )
    {
      foreach (var State in SquirrelHandles)
      {
        var ScriptSource = State.Value.FetchScriptSource(_SourceName);

        if (ScriptSource != null)
          return ScriptSource;
      }

      return null;
    }
  }
}
