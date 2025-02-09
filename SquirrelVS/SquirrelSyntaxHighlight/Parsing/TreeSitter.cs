using System;
using System.Runtime.InteropServices;
using System.Security;
using __CallingConvention = global::System.Runtime.InteropServices.CallingConvention;
using __IntPtr            = global::System.IntPtr;

namespace tree_sitter
{
    public enum TSInputEncoding
    {
        TSInputEncodingUTF8 = 0,
        TSInputEncodingUTF16 = 1
    }

    public enum TSSymbolType
    {
        TSSymbolTypeRegular = 0,
        TSSymbolTypeAnonymous = 1,
        TSSymbolTypeAuxiliary = 2
    }

    public enum TSLogType
    {
        TSLogTypeParse = 0,
        TSLogTypeLex = 1
    }

    public enum TSQuantifier
    {
        TSQuantifierZero = 0,
        TSQuantifierZeroOrOne = 1,
        TSQuantifierZeroOrMore = 2,
        TSQuantifierOne = 3,
        TSQuantifierOneOrMore = 4
    }

    public enum TSQueryPredicateStepType
    {
        TSQueryPredicateStepTypeDone = 0,
        TSQueryPredicateStepTypeCapture = 1,
        TSQueryPredicateStepTypeString = 2
    }

    public enum TSQueryError
    {
        TSQueryErrorNone = 0,
        TSQueryErrorSyntax = 1,
        TSQueryErrorNodeType = 2,
        TSQueryErrorField = 3,
        TSQueryErrorCapture = 4,
        TSQueryErrorStructure = 5,
        TSQueryErrorLanguage = 6
    }

    /// <summary>****************</summary>
    public unsafe partial class TSParser
    {
        public partial struct __Internal
        {
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSParser> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSParser>();

        protected bool __ownsNativeInstance;

        internal static TSParser __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSParser(native.ToPointer(), skipVTables);
        }

        internal static TSParser __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSParser)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSParser __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSParser(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSParser(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSParser(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }
    }

    public unsafe partial class TSTree
    {
        public partial struct __Internal
        {
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSTree> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSTree>();

        protected bool __ownsNativeInstance;

        internal static TSTree __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSTree(native.ToPointer(), skipVTables);
        }

        internal static TSTree __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSTree)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSTree __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSTree(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSTree(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSTree(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }
    }

    public unsafe partial class TSQuery
    {
        public partial struct __Internal
        {
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQuery> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQuery>();

        protected bool __ownsNativeInstance;

        internal static TSQuery __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSQuery(native.ToPointer(), skipVTables);
        }

        internal static TSQuery __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSQuery)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSQuery __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSQuery(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSQuery(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSQuery(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }
    }

    public unsafe partial class TSQueryCursor
    {
        public partial struct __Internal
        {
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQueryCursor> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQueryCursor>();

        protected bool __ownsNativeInstance;

        internal static TSQueryCursor __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSQueryCursor(native.ToPointer(), skipVTables);
        }

        internal static TSQueryCursor __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSQueryCursor)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSQueryCursor __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSQueryCursor(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSQueryCursor(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSQueryCursor(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }
    }

    public unsafe partial class TSPoint : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 1)]
        public partial struct __Internal
        {
            internal uint row;
            internal uint column;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSPoint@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSPoint> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSPoint>();

        protected bool __ownsNativeInstance;

        internal static TSPoint __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSPoint(native.ToPointer(), skipVTables);
        }

        internal static TSPoint __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSPoint)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSPoint __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSPoint(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSPoint(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSPoint(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSPoint()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSPoint.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSPoint(global::tree_sitter.TSPoint __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSPoint.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSPoint.__Internal*) __Instance) = *((global::tree_sitter.TSPoint.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public uint Row
        {
            get
            {
                return ((__Internal*)__Instance)->row;
            }

            set
            {
                ((__Internal*)__Instance)->row = value;
            }
        }

        public uint Column
        {
            get
            {
                return ((__Internal*)__Instance)->column;
            }

            set
            {
                ((__Internal*)__Instance)->column = value;
            }
        }
    }

    public unsafe partial class TSRange : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 24, Pack = 1)]
        public partial struct __Internal
        {
            internal global::tree_sitter.TSPoint.__Internal start_point;
            internal global::tree_sitter.TSPoint.__Internal end_point;
            internal uint start_byte;
            internal uint end_byte;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSRange@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSRange> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSRange>();

        protected bool __ownsNativeInstance;

        internal static TSRange __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSRange(native.ToPointer(), skipVTables);
        }

        internal static TSRange __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSRange)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSRange __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSRange(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSRange(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSRange(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSRange()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSRange.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSRange(global::tree_sitter.TSRange __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSRange.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSRange.__Internal*) __Instance) = *((global::tree_sitter.TSRange.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public global::tree_sitter.TSPoint StartPoint
        {
            get
            {
                return global::tree_sitter.TSPoint.__CreateInstance(new __IntPtr(&((__Internal*)__Instance)->start_point));
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                ((__Internal*)__Instance)->start_point = *(global::tree_sitter.TSPoint.__Internal*) value.__Instance;
            }
        }

        public global::tree_sitter.TSPoint EndPoint
        {
            get
            {
                return global::tree_sitter.TSPoint.__CreateInstance(new __IntPtr(&((__Internal*)__Instance)->end_point));
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                ((__Internal*)__Instance)->end_point = *(global::tree_sitter.TSPoint.__Internal*) value.__Instance;
            }
        }

        public uint StartByte
        {
            get
            {
                return ((__Internal*)__Instance)->start_byte;
            }

            set
            {
                ((__Internal*)__Instance)->start_byte = value;
            }
        }

        public uint EndByte
        {
            get
            {
                return ((__Internal*)__Instance)->end_byte;
            }

            set
            {
                ((__Internal*)__Instance)->end_byte = value;
            }
        }
    }

    public unsafe partial class TSInput : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 12, Pack = 1)]
        public partial struct __Internal
        {
            internal __IntPtr payload;
            internal __IntPtr read;
            internal global::tree_sitter.TSInputEncoding encoding;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSInput@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSInput> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSInput>();

        protected bool __ownsNativeInstance;

        internal static TSInput __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSInput(native.ToPointer(), skipVTables);
        }

        internal static TSInput __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSInput)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSInput __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSInput(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSInput(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSInput(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSInput()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSInput.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSInput(global::tree_sitter.TSInput __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSInput.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSInput.__Internal*) __Instance) = *((global::tree_sitter.TSInput.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public __IntPtr Payload
        {
            get
            {
                return ((__Internal*)__Instance)->payload;
            }

            set
            {
                ((__Internal*)__Instance)->payload = (__IntPtr) value;
            }
        }

        public global::tree_sitter.Delegates.Func___IntPtr___IntPtr_uint_tree_sitter_TSPoint___Internal_uintPtr Read
        {
            get
            {
                var __ptr0 = ((__Internal*)__Instance)->read;
                return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Func___IntPtr___IntPtr_uint_tree_sitter_TSPoint___Internal_uintPtr) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Func___IntPtr___IntPtr_uint_tree_sitter_TSPoint___Internal_uintPtr));
            }

            set
            {
                ((__Internal*)__Instance)->read = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            }
        }

        public global::tree_sitter.TSInputEncoding Encoding
        {
            get
            {
                return ((__Internal*)__Instance)->encoding;
            }

            set
            {
                ((__Internal*)__Instance)->encoding = value;
            }
        }
    }

    public unsafe partial class TSLogger : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 1)]
        public partial struct __Internal
        {
            internal __IntPtr payload;
            internal __IntPtr log;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSLogger@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLogger> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLogger>();

        protected bool __ownsNativeInstance;

        internal static TSLogger __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSLogger(native.ToPointer(), skipVTables);
        }

        internal static TSLogger __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSLogger)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSLogger __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSLogger(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSLogger(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSLogger(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSLogger()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLogger.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSLogger(global::tree_sitter.TSLogger __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLogger.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSLogger.__Internal*) __Instance) = *((global::tree_sitter.TSLogger.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public __IntPtr Payload
        {
            get
            {
                return ((__Internal*)__Instance)->payload;
            }

            set
            {
                ((__Internal*)__Instance)->payload = (__IntPtr) value;
            }
        }

        public global::tree_sitter.Delegates.Action___IntPtr_tree_sitter_TSLogType_string8 Log
        {
            get
            {
                var __ptr0 = ((__Internal*)__Instance)->log;
                return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Action___IntPtr_tree_sitter_TSLogType_string8) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Action___IntPtr_tree_sitter_TSLogType_string8));
            }

            set
            {
                ((__Internal*)__Instance)->log = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            }
        }
    }

    public unsafe partial class TSInputEdit : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 36, Pack = 1)]
        public partial struct __Internal
        {
            internal uint start_byte;
            internal uint old_end_byte;
            internal uint new_end_byte;
            internal global::tree_sitter.TSPoint.__Internal start_point;
            internal global::tree_sitter.TSPoint.__Internal old_end_point;
            internal global::tree_sitter.TSPoint.__Internal new_end_point;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSInputEdit@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSInputEdit> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSInputEdit>();

        protected bool __ownsNativeInstance;

        internal static TSInputEdit __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSInputEdit(native.ToPointer(), skipVTables);
        }

        internal static TSInputEdit __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSInputEdit)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSInputEdit __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSInputEdit(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSInputEdit(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSInputEdit(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSInputEdit()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSInputEdit.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSInputEdit(global::tree_sitter.TSInputEdit __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSInputEdit.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSInputEdit.__Internal*) __Instance) = *((global::tree_sitter.TSInputEdit.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public uint StartByte
        {
            get
            {
                return ((__Internal*)__Instance)->start_byte;
            }

            set
            {
                ((__Internal*)__Instance)->start_byte = value;
            }
        }

        public uint OldEndByte
        {
            get
            {
                return ((__Internal*)__Instance)->old_end_byte;
            }

            set
            {
                ((__Internal*)__Instance)->old_end_byte = value;
            }
        }

        public uint NewEndByte
        {
            get
            {
                return ((__Internal*)__Instance)->new_end_byte;
            }

            set
            {
                ((__Internal*)__Instance)->new_end_byte = value;
            }
        }

        public global::tree_sitter.TSPoint StartPoint
        {
            get
            {
                return global::tree_sitter.TSPoint.__CreateInstance(new __IntPtr(&((__Internal*)__Instance)->start_point));
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                ((__Internal*)__Instance)->start_point = *(global::tree_sitter.TSPoint.__Internal*) value.__Instance;
            }
        }

        public global::tree_sitter.TSPoint OldEndPoint
        {
            get
            {
                return global::tree_sitter.TSPoint.__CreateInstance(new __IntPtr(&((__Internal*)__Instance)->old_end_point));
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                ((__Internal*)__Instance)->old_end_point = *(global::tree_sitter.TSPoint.__Internal*) value.__Instance;
            }
        }

        public global::tree_sitter.TSPoint NewEndPoint
        {
            get
            {
                return global::tree_sitter.TSPoint.__CreateInstance(new __IntPtr(&((__Internal*)__Instance)->new_end_point));
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                ((__Internal*)__Instance)->new_end_point = *(global::tree_sitter.TSPoint.__Internal*) value.__Instance;
            }
        }
    }

    public unsafe partial class TSNode : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 24, Pack = 1)]
        public partial struct __Internal
        {
            internal fixed uint context[4];
            internal __IntPtr id;
            internal __IntPtr tree;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSNode@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSNode> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSNode>();

        protected bool __ownsNativeInstance;

        internal static TSNode __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSNode(native.ToPointer(), skipVTables);
        }

        internal static TSNode __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSNode)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSNode __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSNode(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSNode(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSNode(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSNode()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSNode.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSNode(global::tree_sitter.TSNode __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSNode.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSNode.__Internal*) __Instance) = *((global::tree_sitter.TSNode.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public uint[] Context
        {
            get
            {
                return CppSharp.Runtime.MarshalUtil.GetArray<uint>(((__Internal*)__Instance)->context, 4);
            }

            set
            {
                if (value != null)
                {
                    for (int i = 0; i < 4; i++)
                        ((__Internal*)__Instance)->context[i] = value[i];
                }
            }
        }

        public __IntPtr Id
        {
            get
            {
                return ((__Internal*)__Instance)->id;
            }
        }

        public global::tree_sitter.TSTree Tree
        {
            get
            {
                var __result0 = global::tree_sitter.TSTree.__GetOrCreateInstance(((__Internal*)__Instance)->tree, false);
                return __result0;
            }
        }
    }

    public unsafe partial class TSTreeCursor : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 16, Pack = 1)]
        public partial struct __Internal
        {
            internal __IntPtr tree;
            internal __IntPtr id;
            internal fixed uint context[2];

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSTreeCursor@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSTreeCursor> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSTreeCursor>();

        protected bool __ownsNativeInstance;

        internal static TSTreeCursor __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSTreeCursor(native.ToPointer(), skipVTables);
        }

        internal static TSTreeCursor __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSTreeCursor)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSTreeCursor __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSTreeCursor(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSTreeCursor(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSTreeCursor(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSTreeCursor()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSTreeCursor.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSTreeCursor(global::tree_sitter.TSTreeCursor __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSTreeCursor.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSTreeCursor.__Internal*) __Instance) = *((global::tree_sitter.TSTreeCursor.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public __IntPtr Tree
        {
            get
            {
                return ((__Internal*)__Instance)->tree;
            }
        }

        public __IntPtr Id
        {
            get
            {
                return ((__Internal*)__Instance)->id;
            }
        }

        public uint[] Context
        {
            get
            {
                return CppSharp.Runtime.MarshalUtil.GetArray<uint>(((__Internal*)__Instance)->context, 2);
            }

            set
            {
                if (value != null)
                {
                    for (int i = 0; i < 2; i++)
                        ((__Internal*)__Instance)->context[i] = value[i];
                }
            }
        }
    }

    public unsafe partial class TSQueryCapture : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 28, Pack = 1)]
        public partial struct __Internal
        {
            internal global::tree_sitter.TSNode.__Internal node;
            internal uint index;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSQueryCapture@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQueryCapture> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQueryCapture>();

        protected bool __ownsNativeInstance;

        internal static TSQueryCapture __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSQueryCapture(native.ToPointer(), skipVTables);
        }

        internal static TSQueryCapture __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSQueryCapture)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSQueryCapture __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSQueryCapture(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSQueryCapture(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSQueryCapture(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSQueryCapture()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSQueryCapture.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSQueryCapture(global::tree_sitter.TSQueryCapture __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSQueryCapture.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSQueryCapture.__Internal*) __Instance) = *((global::tree_sitter.TSQueryCapture.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public global::tree_sitter.TSNode Node
        {
            get
            {
                return global::tree_sitter.TSNode.__CreateInstance(new __IntPtr(&((__Internal*)__Instance)->node));
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                ((__Internal*)__Instance)->node = *(global::tree_sitter.TSNode.__Internal*) value.__Instance;
            }
        }

        public uint Index
        {
            get
            {
                return ((__Internal*)__Instance)->index;
            }

            set
            {
                ((__Internal*)__Instance)->index = value;
            }
        }
    }

    public unsafe partial class TSQueryMatch : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 12, Pack = 1)]
        public partial struct __Internal
        {
            internal uint id;
            internal ushort pattern_index;
            internal ushort capture_count;
            internal __IntPtr captures;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSQueryMatch@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQueryMatch> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQueryMatch>();

        protected bool __ownsNativeInstance;

        internal static TSQueryMatch __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSQueryMatch(native.ToPointer(), skipVTables);
        }

        internal static TSQueryMatch __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSQueryMatch)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSQueryMatch __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSQueryMatch(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSQueryMatch(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSQueryMatch(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSQueryMatch()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSQueryMatch.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSQueryMatch(global::tree_sitter.TSQueryMatch __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSQueryMatch.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSQueryMatch.__Internal*) __Instance) = *((global::tree_sitter.TSQueryMatch.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public uint Id
        {
            get
            {
                return ((__Internal*)__Instance)->id;
            }

            set
            {
                ((__Internal*)__Instance)->id = value;
            }
        }

        public ushort PatternIndex
        {
            get
            {
                return ((__Internal*)__Instance)->pattern_index;
            }

            set
            {
                ((__Internal*)__Instance)->pattern_index = value;
            }
        }

        public ushort CaptureCount
        {
            get
            {
                return ((__Internal*)__Instance)->capture_count;
            }

            set
            {
                ((__Internal*)__Instance)->capture_count = value;
            }
        }

        public global::tree_sitter.TSQueryCapture Captures
        {
            get
            {
                var __result0 = global::tree_sitter.TSQueryCapture.__GetOrCreateInstance(((__Internal*)__Instance)->captures, false);
                return __result0;
            }
        }

      public global::tree_sitter.TSQueryCapture this[int _Index]
      {
        get
        {
          var __result0 = global::tree_sitter.TSQueryCapture.__GetOrCreateInstance(((__Internal*)__Instance)->captures + (sizeof(global::tree_sitter.TSQueryCapture.__Internal) * _Index), false);
          return __result0;
      }
      }
        
    }

    public unsafe partial class TSQueryPredicateStep : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 1)]
        public partial struct __Internal
        {
            internal global::tree_sitter.TSQueryPredicateStepType type;
            internal uint value_id;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSQueryPredicateStep@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQueryPredicateStep> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSQueryPredicateStep>();

        protected bool __ownsNativeInstance;

        internal static TSQueryPredicateStep __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSQueryPredicateStep(native.ToPointer(), skipVTables);
        }

        internal static TSQueryPredicateStep __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSQueryPredicateStep)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSQueryPredicateStep __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSQueryPredicateStep(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSQueryPredicateStep(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSQueryPredicateStep(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSQueryPredicateStep()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSQueryPredicateStep.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSQueryPredicateStep(global::tree_sitter.TSQueryPredicateStep __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSQueryPredicateStep.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSQueryPredicateStep.__Internal*) __Instance) = *((global::tree_sitter.TSQueryPredicateStep.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public global::tree_sitter.TSQueryPredicateStepType Type
        {
            get
            {
                return ((__Internal*)__Instance)->type;
            }

            set
            {
                ((__Internal*)__Instance)->type = value;
            }
        }

        public uint ValueId
        {
            get
            {
                return ((__Internal*)__Instance)->value_id;
            }

            set
            {
                ((__Internal*)__Instance)->value_id = value;
            }
        }
    }

    public unsafe partial class api
    {
        public partial struct __Internal
        {
            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_new", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsParserNew();

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_delete", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsParserDelete(__IntPtr parser);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_set_language", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsParserSetLanguage(__IntPtr self, __IntPtr language);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_language", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsParserLanguage(__IntPtr self);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_set_included_ranges", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsParserSetIncludedRanges(__IntPtr self, __IntPtr ranges, uint length);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_included_ranges", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsParserIncludedRanges(__IntPtr self, uint* length);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_parse", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsParserParse(__IntPtr self, __IntPtr old_tree, global::tree_sitter.TSInput.__Internal input);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_parse_string", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsParserParseString(__IntPtr self, __IntPtr old_tree, string @string, uint length);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_parse_string_encoding", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsParserParseStringEncoding(__IntPtr self, __IntPtr old_tree, string @string, uint length, global::tree_sitter.TSInputEncoding encoding);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_reset", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsParserReset(__IntPtr self);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_set_timeout_micros", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsParserSetTimeoutMicros(__IntPtr self, ulong timeout);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_timeout_micros", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern ulong TsParserTimeoutMicros(__IntPtr self);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_set_cancellation_flag", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsParserSetCancellationFlag(__IntPtr self, uint* flag);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_cancellation_flag", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint* TsParserCancellationFlag(__IntPtr self);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_set_logger", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsParserSetLogger(__IntPtr self, global::tree_sitter.TSLogger.__Internal logger);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_logger", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern global::tree_sitter.TSLogger.__Internal TsParserLogger(__IntPtr self);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_parser_print_dot_graphs", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsParserPrintDotGraphs(__IntPtr self, int file);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_copy", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsTreeCopy(__IntPtr self);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_delete", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsTreeDelete(__IntPtr self);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_root_node", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern global::tree_sitter.TSNode.__Internal TsTreeRootNode(__IntPtr self);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_language", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsTreeLanguage(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_edit", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsTreeEdit(__IntPtr self, __IntPtr edit);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_get_changed_ranges", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsTreeGetChangedRanges(__IntPtr old_tree, __IntPtr new_tree, uint* length);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_print_dot_graph", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsTreePrintDotGraph(__IntPtr _0, __IntPtr _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_type", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsNodeType(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_symbol", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern ushort TsNodeSymbol(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_start_byte", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsNodeStartByte(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_start_point", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern global::tree_sitter.TSPoint.__Internal TsNodeStartPoint(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_end_byte", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsNodeEndByte(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_end_point", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern global::tree_sitter.TSPoint.__Internal TsNodeEndPoint(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_string", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern sbyte* TsNodeString(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_is_null", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsNodeIsNull(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_is_named", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsNodeIsNamed(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_is_missing", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsNodeIsMissing(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_is_extra", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsNodeIsExtra(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_has_changes", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsNodeHasChanges(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_has_error", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsNodeHasError(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_parent", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeParent(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_child", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeChild(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0, uint _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_field_name_for_child", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsNodeFieldNameForChild(global::tree_sitter.TSNode.__Internal _0, uint _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_child_count", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsNodeChildCount(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_named_child", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeNamedChild(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0, uint _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_named_child_count", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsNodeNamedChildCount(global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_child_by_field_name", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeChildByFieldName(__IntPtr @return, global::tree_sitter.TSNode.__Internal self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CppSharp.Runtime.UTF8Marshaller))] string field_name, uint field_name_length);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_child_by_field_id", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeChildByFieldId(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0, ushort _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_next_sibling", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeNextSibling(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_prev_sibling", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodePrevSibling(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_next_named_sibling", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeNextNamedSibling(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_prev_named_sibling", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodePrevNamedSibling(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_first_child_for_byte", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeFirstChildForByte(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0, uint _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_first_named_child_for_byte", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeFirstNamedChildForByte(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0, uint _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_descendant_for_byte_range", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeDescendantForByteRange(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0, uint _1, uint _2);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_descendant_for_point_range", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeDescendantForPointRange(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0, global::tree_sitter.TSPoint.__Internal _1, global::tree_sitter.TSPoint.__Internal _2);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_named_descendant_for_byte_range", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeNamedDescendantForByteRange(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0, uint _1, uint _2);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_named_descendant_for_point_range", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeNamedDescendantForPointRange(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0, global::tree_sitter.TSPoint.__Internal _1, global::tree_sitter.TSPoint.__Internal _2);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_edit", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsNodeEdit(__IntPtr _0, __IntPtr _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_node_eq", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsNodeEq(global::tree_sitter.TSNode.__Internal _0, global::tree_sitter.TSNode.__Internal _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_new", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsTreeCursorNew(__IntPtr @return, global::tree_sitter.TSNode.__Internal _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_delete", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsTreeCursorDelete(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_reset", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsTreeCursorReset(__IntPtr _0, global::tree_sitter.TSNode.__Internal _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_current_node", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsTreeCursorCurrentNode(__IntPtr @return, __IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_current_field_name", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsTreeCursorCurrentFieldName(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_current_field_id", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern ushort TsTreeCursorCurrentFieldId(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_goto_parent", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsTreeCursorGotoParent(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_goto_next_sibling", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsTreeCursorGotoNextSibling(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_goto_first_child", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsTreeCursorGotoFirstChild(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_goto_first_child_for_byte", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern long TsTreeCursorGotoFirstChildForByte(__IntPtr _0, uint _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_goto_first_child_for_point", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern long TsTreeCursorGotoFirstChildForPoint(__IntPtr _0, global::tree_sitter.TSPoint.__Internal _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_tree_cursor_copy", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsTreeCursorCopy(__IntPtr @return, __IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_new", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsQueryNew(__IntPtr language, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CppSharp.Runtime.UTF8Marshaller))] string source, uint source_len, uint* error_offset, global::tree_sitter.TSQueryError* error_type);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_delete", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsQueryDelete(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_pattern_count", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsQueryPatternCount(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_capture_count", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsQueryCaptureCount(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_string_count", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsQueryStringCount(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_start_byte_for_pattern", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsQueryStartByteForPattern(__IntPtr _0, uint _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_predicates_for_pattern", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsQueryPredicatesForPattern(__IntPtr self, uint pattern_index, uint* length);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_is_pattern_guaranteed_at_step", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsQueryIsPatternGuaranteedAtStep(__IntPtr self, uint byte_offset);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_capture_name_for_id", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsQueryCaptureNameForId(__IntPtr _0, uint id, uint* length);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_capture_quantifier_for_id", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern global::tree_sitter.TSQuantifier TsQueryCaptureQuantifierForId(__IntPtr _0, uint pattern_id, uint capture_id);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_string_value_for_id", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsQueryStringValueForId(__IntPtr _0, uint id, uint* length);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_disable_capture", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsQueryDisableCapture(__IntPtr _0, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CppSharp.Runtime.UTF8Marshaller))] string _1, uint _2);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_disable_pattern", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsQueryDisablePattern(__IntPtr _0, uint _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_new", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsQueryCursorNew();

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_delete", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsQueryCursorDelete(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_exec", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsQueryCursorExec(__IntPtr _0, __IntPtr _1, global::tree_sitter.TSNode.__Internal _2);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_did_exceed_match_limit", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsQueryCursorDidExceedMatchLimit(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_match_limit", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsQueryCursorMatchLimit(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_set_match_limit", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsQueryCursorSetMatchLimit(__IntPtr _0, uint _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_set_byte_range", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsQueryCursorSetByteRange(__IntPtr _0, uint _1, uint _2);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_set_point_range", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsQueryCursorSetPointRange(__IntPtr _0, global::tree_sitter.TSPoint.__Internal _1, global::tree_sitter.TSPoint.__Internal _2);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_next_match", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsQueryCursorNextMatch(__IntPtr _0, __IntPtr match);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_remove_match", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsQueryCursorRemoveMatch(__IntPtr _0, uint id);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_query_cursor_next_capture", CallingConvention = __CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.I1)]
            internal static extern bool TsQueryCursorNextCapture(__IntPtr _0, __IntPtr match, uint* capture_index);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_language_symbol_count", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsLanguageSymbolCount(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_language_symbol_name", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsLanguageSymbolName(__IntPtr _0, ushort _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_language_symbol_for_name", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern ushort TsLanguageSymbolForName(__IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CppSharp.Runtime.UTF8Marshaller))] string @string, uint length, bool is_named);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_language_field_count", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsLanguageFieldCount(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_language_field_name_for_id", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TsLanguageFieldNameForId(__IntPtr _0, ushort _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_language_field_id_for_name", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern ushort TsLanguageFieldIdForName(__IntPtr _0, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CppSharp.Runtime.UTF8Marshaller))] string _1, uint _2);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_language_symbol_type", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern global::tree_sitter.TSSymbolType TsLanguageSymbolType(__IntPtr _0, ushort _1);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_language_version", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern uint TsLanguageVersion(__IntPtr _0);

            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "ts_set_allocator", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern void TsSetAllocator(__IntPtr new_malloc, __IntPtr new_calloc, __IntPtr new_realloc, __IntPtr new_free);
        }

        /// <summary>Create a new parser.</summary>
        public static global::tree_sitter.TSParser TsParserNew()
        {
            var __ret = __Internal.TsParserNew();
            var __result0 = global::tree_sitter.TSParser.__GetOrCreateInstance(__ret, false);
            return __result0;
        }

        /// <summary>Delete the parser, freeing all of the memory that it used.</summary>
        public static void TsParserDelete(global::tree_sitter.TSParser parser)
        {
            var __arg0 = parser is null ? __IntPtr.Zero : parser.__Instance;
            __Internal.TsParserDelete(__arg0);
        }

        /// <summary>Set the language that the parser should use for parsing.</summary>
        /// <remarks>
        /// <para>Returns a boolean indicating whether or not the language was successfully</para>
        /// <para>assigned. True means assignment succeeded. False means there was a version</para>
        /// <para>mismatch: the language was generated with an incompatible version of the</para>
        /// <para>Tree-sitter CLI. Check the language's version using `ts_language_version`</para>
        /// <para>and compare it to this library's `TREE_SITTER_LANGUAGE_VERSION` and</para>
        /// <para>`TREE_SITTER_MIN_COMPATIBLE_LANGUAGE_VERSION` constants.</para>
        /// </remarks>
        public static bool TsParserSetLanguage(global::tree_sitter.TSParser self, global::tree_sitter.TSLanguage language)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __arg1 = language is null ? __IntPtr.Zero : language.__Instance;
            var __ret = __Internal.TsParserSetLanguage(__arg0, __arg1);
            return __ret;
        }

        /// <summary>Get the parser's current language.</summary>
        public static global::tree_sitter.TSLanguage TsParserLanguage(global::tree_sitter.TSParser self)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __ret = __Internal.TsParserLanguage(__arg0);
            var __result0 = global::tree_sitter.TSLanguage.__GetOrCreateInstance(__ret, false);
            return __result0;
        }

        /// <summary>Set the ranges of text that the parser should include when parsing.</summary>
        /// <remarks>
        /// <para>By default, the parser will always include entire documents. This function</para>
        /// <para>allows you to parse only a *portion* of a document but still return a syntax</para>
        /// <para>tree whose ranges match up with the document as a whole. You can also pass</para>
        /// <para>multiple disjoint ranges.</para>
        /// <para>The second and third parameters specify the location and length of an array</para>
        /// <para>of ranges. The parser does *not* take ownership of these ranges; it copies</para>
        /// <para>the data, so it doesn't matter how these ranges are allocated.</para>
        /// <para>If `length` is zero, then the entire document will be parsed. Otherwise,</para>
        /// <para>the given ranges must be ordered from earliest to latest in the document,</para>
        /// <para>and they must not overlap. That is, the following must hold for all</para>
        /// <para>`i`&lt;&gt;`length - 1`: ranges[i].end_byte&lt;&gt;= ranges[i + 1].start_byte</para>
        /// <para>If this requirement is not satisfied, the operation will fail, the ranges</para>
        /// <para>will not be assigned, and this function will return `false`. On success,</para>
        /// <para>this function returns `true`</para>
        /// </remarks>
        public static bool TsParserSetIncludedRanges(global::tree_sitter.TSParser self, global::tree_sitter.TSRange ranges, uint length)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __arg1 = ranges is null ? __IntPtr.Zero : ranges.__Instance;
            var __ret = __Internal.TsParserSetIncludedRanges(__arg0, __arg1, length);
            return __ret;
        }

        /// <summary>Get the ranges of text that the parser will include when parsing.</summary>
        /// <remarks>
        /// <para>The returned pointer is owned by the parser. The caller should not free it</para>
        /// <para>or write to it. The length of the array will be written to the given</para>
        /// <para>`length` pointer.</para>
        /// </remarks>
        public static global::tree_sitter.TSRange TsParserIncludedRanges(global::tree_sitter.TSParser self, ref uint length)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            fixed (uint* __length1 = &length)
            {
                var __arg1 = __length1;
                var __ret = __Internal.TsParserIncludedRanges(__arg0, __arg1);
                var __result0 = global::tree_sitter.TSRange.__GetOrCreateInstance(__ret, false);
                return __result0;
            }
        }

        /// <summary>Use the parser to parse some source code and create a syntax tree.</summary>
        /// <remarks>
        /// <para>If you are parsing this document for the first time, pass `NULL` for the</para>
        /// <para>`old_tree` parameter. Otherwise, if you have already parsed an earlier</para>
        /// <para>version of this document and the document has since been edited, pass the</para>
        /// <para>previous syntax tree so that the unchanged parts of it can be reused.</para>
        /// <para>This will save time and memory. For this to work correctly, you must have</para>
        /// <para>already edited the old syntax tree using the `ts_tree_edit` function in a</para>
        /// <para>way that exactly matches the source code changes.</para>
        /// <para>The `TSInput` parameter lets you specify how to read the text. It has the</para>
        /// <para>following three fields:</para>
        /// <para>1. `read`: A function to retrieve a chunk of text at a given byte offset</para>
        /// <para>and (row, column) position. The function should return a pointer to the</para>
        /// <para>text and write its length to the `bytes_read` pointer. The parser does</para>
        /// <para>not take ownership of this buffer; it just borrows it until it has</para>
        /// <para>finished reading it. The function should write a zero value to the</para>
        /// <para>`bytes_read` pointer to indicate the end of the document.</para>
        /// <para>2. `payload`: An arbitrary pointer that will be passed to each invocation</para>
        /// <para>of the `read` function.</para>
        /// <para>3. `encoding`: An indication of how the text is encoded. Either</para>
        /// <para>`TSInputEncodingUTF8` or `TSInputEncodingUTF16`.</para>
        /// <para>This function returns a syntax tree on success, and `NULL` on failure. There</para>
        /// <para>are three possible reasons for failure:</para>
        /// <para>1. The parser does not have a language assigned. Check for this using the</para>
        /// <para>`ts_parser_language` function.</para>
        /// <para>2. Parsing was cancelled due to a timeout that was set by an earlier call to</para>
        /// <para>the `ts_parser_set_timeout_micros` function. You can resume parsing from</para>
        /// <para>where the parser left out by calling `ts_parser_parse` again with the</para>
        /// <para>same arguments. Or you can start parsing from scratch by first calling</para>
        /// <para>`ts_parser_reset`.</para>
        /// <para>3. Parsing was cancelled using a cancellation flag that was set by an</para>
        /// <para>earlier call to `ts_parser_set_cancellation_flag`. You can resume parsing</para>
        /// <para>from where the parser left out by calling `ts_parser_parse` again with</para>
        /// <para>the same arguments.</para>
        /// </remarks>
        public static global::tree_sitter.TSTree TsParserParse(global::tree_sitter.TSParser self, global::tree_sitter.TSTree old_tree, global::tree_sitter.TSInput input)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __arg1 = old_tree is null ? __IntPtr.Zero : old_tree.__Instance;
            if (ReferenceEquals(input, null))
                throw new global::System.ArgumentNullException("input", "Cannot be null because it is passed by value.");
            var __arg2 = input.__Instance;
            var __ret = __Internal.TsParserParse(__arg0, __arg1, *(global::tree_sitter.TSInput.__Internal*) __arg2);
            var __result0 = global::tree_sitter.TSTree.__GetOrCreateInstance(__ret, false);
            return __result0;
        }

        /// <summary>
        /// <para>Use the parser to parse some source code stored in one contiguous buffer.</para>
        /// <para>The first two parameters are the same as in the `ts_parser_parse` function</para>
        /// <para>above. The second two parameters indicate the location of the buffer and its</para>
        /// <para>length in bytes.</para>
        /// </summary>
        public static global::tree_sitter.TSTree TsParserParseString(global::tree_sitter.TSParser self, global::tree_sitter.TSTree old_tree, string @string, uint length)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __arg1 = old_tree is null ? __IntPtr.Zero : old_tree.__Instance;
            var __ret = __Internal.TsParserParseString(__arg0, __arg1, @string, length);
            var __result0 = global::tree_sitter.TSTree.__GetOrCreateInstance(__ret, false);
            return __result0;
        }

        /// <summary>
        /// <para>Use the parser to parse some source code stored in one contiguous buffer with</para>
        /// <para>a given encoding. The first four parameters work the same as in the</para>
        /// <para>`ts_parser_parse_string` method above. The final parameter indicates whether</para>
        /// <para>the text is encoded as UTF8 or UTF16.</para>
        /// </summary>
        public static global::tree_sitter.TSTree TsParserParseStringEncoding(global::tree_sitter.TSParser self, global::tree_sitter.TSTree old_tree, string @string, uint length, global::tree_sitter.TSInputEncoding encoding)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __arg1 = old_tree is null ? __IntPtr.Zero : old_tree.__Instance;
            var __ret = __Internal.TsParserParseStringEncoding(__arg0, __arg1, @string, length, encoding);
            var __result0 = global::tree_sitter.TSTree.__GetOrCreateInstance(__ret, false);
            return __result0;
        }

        /// <summary>Instruct the parser to start the next parse from the beginning.</summary>
        /// <remarks>
        /// <para>If the parser previously failed because of a timeout or a cancellation, then</para>
        /// <para>by default, it will resume where it left off on the next call to</para>
        /// <para>`ts_parser_parse` or other parsing functions. If you don't want to resume,</para>
        /// <para>and instead intend to use this parser to parse some other document, you must</para>
        /// <para>call `ts_parser_reset` first.</para>
        /// </remarks>
        public static void TsParserReset(global::tree_sitter.TSParser self)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            __Internal.TsParserReset(__arg0);
        }

        /// <summary>
        /// <para>Set the maximum duration in microseconds that parsing should be allowed to</para>
        /// <para>take before halting.</para>
        /// </summary>
        /// <remarks>
        /// <para>If parsing takes longer than this, it will halt early, returning NULL.</para>
        /// <para>See `ts_parser_parse` for more information.</para>
        /// </remarks>
        public static void TsParserSetTimeoutMicros(global::tree_sitter.TSParser self, ulong timeout)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            __Internal.TsParserSetTimeoutMicros(__arg0, timeout);
        }

        /// <summary>Get the duration in microseconds that parsing is allowed to take.</summary>
        public static ulong TsParserTimeoutMicros(global::tree_sitter.TSParser self)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __ret = __Internal.TsParserTimeoutMicros(__arg0);
            return __ret;
        }

        /// <summary>Set the parser's current cancellation flag pointer.</summary>
        /// <remarks>
        /// <para>If a non-null pointer is assigned, then the parser will periodically read</para>
        /// <para>from this pointer during parsing. If it reads a non-zero value, it will</para>
        /// <para>halt early, returning NULL. See `ts_parser_parse` for more information.</para>
        /// </remarks>
        public static void TsParserSetCancellationFlag(global::tree_sitter.TSParser self, ref uint flag)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            fixed (uint* __flag1 = &flag)
            {
                var __arg1 = __flag1;
                __Internal.TsParserSetCancellationFlag(__arg0, __arg1);
            }
        }

        /// <summary>Get the parser's current cancellation flag pointer.</summary>
        public static uint* TsParserCancellationFlag(global::tree_sitter.TSParser self)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __ret = __Internal.TsParserCancellationFlag(__arg0);
            return __ret;
        }

        /// <summary>Set the logger that a parser should use during parsing.</summary>
        /// <remarks>
        /// <para>The parser does not take ownership over the logger payload. If a logger was</para>
        /// <para>previously assigned, the caller is responsible for releasing any memory</para>
        /// <para>owned by the previous logger.</para>
        /// </remarks>
        public static void TsParserSetLogger(global::tree_sitter.TSParser self, global::tree_sitter.TSLogger logger)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            if (ReferenceEquals(logger, null))
                throw new global::System.ArgumentNullException("logger", "Cannot be null because it is passed by value.");
            var __arg1 = logger.__Instance;
            __Internal.TsParserSetLogger(__arg0, *(global::tree_sitter.TSLogger.__Internal*) __arg1);
        }

        /// <summary>Get the parser's current logger.</summary>
        public static global::tree_sitter.TSLogger TsParserLogger(global::tree_sitter.TSParser self)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __ret = __Internal.TsParserLogger(__arg0);
            return global::tree_sitter.TSLogger.__CreateInstance(__ret);
        }

        /// <summary>
        /// <para>Set the file descriptor to which the parser should write debugging graphs</para>
        /// <para>during parsing. The graphs are formatted in the DOT language. You may want</para>
        /// <para>to pipe these graphs directly to a `dot(1)` process in order to generate</para>
        /// <para>SVG output. You can turn off this logging by passing a negative number.</para>
        /// </summary>
        public static void TsParserPrintDotGraphs(global::tree_sitter.TSParser self, int file)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            __Internal.TsParserPrintDotGraphs(__arg0, file);
        }

        /// <summary>Create a shallow copy of the syntax tree. This is very fast.</summary>
        /// <remarks>
        /// <para>You need to copy a syntax tree in order to use it on more than one thread at</para>
        /// <para>a time, as syntax trees are not thread safe.</para>
        /// </remarks>
        public static global::tree_sitter.TSTree TsTreeCopy(global::tree_sitter.TSTree self)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __ret = __Internal.TsTreeCopy(__arg0);
            var __result0 = global::tree_sitter.TSTree.__GetOrCreateInstance(__ret, false);
            return __result0;
        }

        /// <summary>Delete the syntax tree, freeing all of the memory that it used.</summary>
        public static void TsTreeDelete(global::tree_sitter.TSTree self)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            __Internal.TsTreeDelete(__arg0);
        }

        /// <summary>Get the root node of the syntax tree.</summary>
        public static global::tree_sitter.TSNode TsTreeRootNode(global::tree_sitter.TSTree self)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __ret = __Internal.TsTreeRootNode(__arg0);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>Get the language that was used to parse the syntax tree.</summary>
        public static global::tree_sitter.TSLanguage TsTreeLanguage(global::tree_sitter.TSTree _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsTreeLanguage(__arg0);
            var __result0 = global::tree_sitter.TSLanguage.__GetOrCreateInstance(__ret, false);
            return __result0;
        }

        /// <summary>
        /// <para>Edit the syntax tree to keep it in sync with source code that has been</para>
        /// <para>edited.</para>
        /// </summary>
        /// <remarks>
        /// <para>You must describe the edit both in terms of byte offsets and in terms of</para>
        /// <para>(row, column) coordinates.</para>
        /// </remarks>
        public static void TsTreeEdit(global::tree_sitter.TSTree self, global::tree_sitter.TSInputEdit edit)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __arg1 = edit is null ? __IntPtr.Zero : edit.__Instance;
            __Internal.TsTreeEdit(__arg0, __arg1);
        }

    /// <summary>
    /// <para>Compare an old edited syntax tree to a new syntax tree representing the same</para>
    /// <para>document, returning an array of ranges whose syntactic structure has changed.</para>
    /// </summary>
    /// <remarks>
    /// <para>For this to work correctly, the old syntax tree must have been edited such</para>
    /// <para>that its ranges match up to the new tree. Generally, you'll want to call</para>
    /// <para>this function right after calling one of the `ts_parser_parse` functions.</para>
    /// <para>You need to pass the old tree that was passed to parse, as well as the new</para>
    /// <para>tree that was returned from that function.</para>
    /// <para>The returned array is allocated using `malloc` and the caller is responsible</para>
    /// <para>for freeing it using `free`. The length of the array will be written to the</para>
    /// <para>given `length` pointer.</para>
    /// </remarks>
    public static global::tree_sitter.TSRange[] TsTreeGetChangedRanges(global::tree_sitter.TSTree old_tree, global::tree_sitter.TSTree new_tree, ref uint length)
    {
      var __arg0 = old_tree is null ? __IntPtr.Zero : old_tree.__Instance;
      var __arg1 = new_tree is null ? __IntPtr.Zero : new_tree.__Instance;
      fixed (uint* __length2 = &length)
      {
        var __arg2 = __length2;
        var __ret = __Internal.TsTreeGetChangedRanges(__arg0, __arg1, __arg2);

        TSRange[] __result = new TSRange[length];

        for (int i = 0; i < length; i++)
          __result[i] = global::tree_sitter.TSRange.__GetOrCreateInstance(new __IntPtr(__ret.ToInt64() + sizeof(TSRange.__Internal) * i), false);

        return __result;
      }
    }

    /// <summary>Write a DOT graph describing the syntax tree to the given file.</summary>
    public static void TsTreePrintDotGraph(global::tree_sitter.TSTree _0, global::System.IntPtr _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            __Internal.TsTreePrintDotGraph(__arg0, _1);
        }

        /// <summary>Get the node's type as a null-terminated string.</summary>
        public static string TsNodeType(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeType(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return CppSharp.Runtime.MarshalUtil.GetString(global::System.Text.Encoding.UTF8, __ret);
        }

        /// <summary>Get the node's type as a numerical id.</summary>
        public static ushort TsNodeSymbol(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeSymbol(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>Get the node's start byte.</summary>
        public static uint TsNodeStartByte(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeStartByte(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>Get the node's start position in terms of rows and columns.</summary>
        public static global::tree_sitter.TSPoint TsNodeStartPoint(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeStartPoint(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return global::tree_sitter.TSPoint.__CreateInstance(__ret);
        }

        /// <summary>Get the node's end byte.</summary>
        public static uint TsNodeEndByte(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeEndByte(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>Get the node's end position in terms of rows and columns.</summary>
        public static global::tree_sitter.TSPoint TsNodeEndPoint(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeEndPoint(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return global::tree_sitter.TSPoint.__CreateInstance(__ret);
        }

        /// <summary>Get an S-expression representing the node as a string.</summary>
        /// <remarks>
        /// <para>This string is allocated with `malloc` and the caller is responsible for</para>
        /// <para>freeing it using `free`.</para>
        /// </remarks>
        public static string TsNodeString(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeString(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return CppSharp.Runtime.MarshalUtil.GetString(global::System.Text.Encoding.UTF8, (__IntPtr)__ret);
        }

        /// <summary>
        /// <para>Check if the node is null. Functions like `ts_node_child` and</para>
        /// <para>`ts_node_next_sibling` will return a null node to indicate that no such node</para>
        /// <para>was found.</para>
        /// </summary>
        public static bool TsNodeIsNull(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeIsNull(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>
        /// <para>Check if the node is *named*. Named nodes correspond to named rules in the</para>
        /// <para>grammar, whereas *anonymous* nodes correspond to string literals in the</para>
        /// <para>grammar.</para>
        /// </summary>
        public static bool TsNodeIsNamed(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeIsNamed(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>
        /// <para>Check if the node is *missing*. Missing nodes are inserted by the parser in</para>
        /// <para>order to recover from certain kinds of syntax errors.</para>
        /// </summary>
        public static bool TsNodeIsMissing(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeIsMissing(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>
        /// <para>Check if the node is *extra*. Extra nodes represent things like comments,</para>
        /// <para>which are not required the grammar, but can appear anywhere.</para>
        /// </summary>
        public static bool TsNodeIsExtra(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeIsExtra(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>Check if a syntax node has been edited.</summary>
        public static bool TsNodeHasChanges(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeHasChanges(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>Check if the node is a syntax error or contains any syntax errors.</summary>
        public static bool TsNodeHasError(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeHasError(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>Get the node's immediate parent.</summary>
        public static global::tree_sitter.TSNode TsNodeParent(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeParent(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>
        /// <para>Get the node's child at the given index, where zero represents the first</para>
        /// <para>child.</para>
        /// </summary>
        public static global::tree_sitter.TSNode TsNodeChild(global::tree_sitter.TSNode _0, uint _1)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeChild(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, _1);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>
        /// <para>Get the field name for node's child at the given index, where zero represents</para>
        /// <para>the first child. Returns NULL, if no field is found.</para>
        /// </summary>
        public static string TsNodeFieldNameForChild(global::tree_sitter.TSNode _0, uint _1)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeFieldNameForChild(*(global::tree_sitter.TSNode.__Internal*) __arg0, _1);
            return CppSharp.Runtime.MarshalUtil.GetString(global::System.Text.Encoding.UTF8, __ret);
        }

        /// <summary>Get the node's number of children.</summary>
        public static uint TsNodeChildCount(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeChildCount(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>Get the node's *named* child at the given index.</summary>
        /// <remarks>See also `ts_node_is_named`.</remarks>
        public static global::tree_sitter.TSNode TsNodeNamedChild(global::tree_sitter.TSNode _0, uint _1)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeNamedChild(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, _1);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>Get the node's number of *named* children.</summary>
        /// <remarks>See also `ts_node_is_named`.</remarks>
        public static uint TsNodeNamedChildCount(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = __Internal.TsNodeNamedChildCount(*(global::tree_sitter.TSNode.__Internal*) __arg0);
            return __ret;
        }

        /// <summary>Get the node's child with the given field name.</summary>
        public static global::tree_sitter.TSNode TsNodeChildByFieldName(global::tree_sitter.TSNode self, string field_name, uint field_name_length)
        {
            if (ReferenceEquals(self, null))
                throw new global::System.ArgumentNullException("self", "Cannot be null because it is passed by value.");
            var __arg0 = self.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeChildByFieldName(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, field_name, field_name_length);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>Get the node's child with the given numerical field id.</summary>
        /// <remarks>
        /// <para>You can convert a field name to an id using the</para>
        /// <para>`ts_language_field_id_for_name` function.</para>
        /// </remarks>
        public static global::tree_sitter.TSNode TsNodeChildByFieldId(global::tree_sitter.TSNode _0, ushort _1)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeChildByFieldId(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, _1);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>Get the node's next / previous sibling.</summary>
        public static global::tree_sitter.TSNode TsNodeNextSibling(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeNextSibling(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        public static global::tree_sitter.TSNode TsNodePrevSibling(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodePrevSibling(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>Get the node's next / previous *named* sibling.</summary>
        public static global::tree_sitter.TSNode TsNodeNextNamedSibling(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeNextNamedSibling(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        public static global::tree_sitter.TSNode TsNodePrevNamedSibling(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodePrevNamedSibling(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>Get the node's first child that extends beyond the given byte offset.</summary>
        public static global::tree_sitter.TSNode TsNodeFirstChildForByte(global::tree_sitter.TSNode _0, uint _1)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeFirstChildForByte(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, _1);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>Get the node's first named child that extends beyond the given byte offset.</summary>
        public static global::tree_sitter.TSNode TsNodeFirstNamedChildForByte(global::tree_sitter.TSNode _0, uint _1)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeFirstNamedChildForByte(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, _1);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>
        /// <para>Get the smallest node within this node that spans the given range of bytes</para>
        /// <para>or (row, column) positions.</para>
        /// </summary>
        public static global::tree_sitter.TSNode TsNodeDescendantForByteRange(global::tree_sitter.TSNode _0, uint _1, uint _2)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeDescendantForByteRange(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, _1, _2);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        public static global::tree_sitter.TSNode TsNodeDescendantForPointRange(global::tree_sitter.TSNode _0, global::tree_sitter.TSPoint _1, global::tree_sitter.TSPoint _2)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            if (ReferenceEquals(_1, null))
                throw new global::System.ArgumentNullException("_1", "Cannot be null because it is passed by value.");
            var __arg1 = _1.__Instance;
            if (ReferenceEquals(_2, null))
                throw new global::System.ArgumentNullException("_2", "Cannot be null because it is passed by value.");
            var __arg2 = _2.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeDescendantForPointRange(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, *(global::tree_sitter.TSPoint.__Internal*) __arg1, *(global::tree_sitter.TSPoint.__Internal*) __arg2);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>
        /// <para>Get the smallest named node within this node that spans the given range of</para>
        /// <para>bytes or (row, column) positions.</para>
        /// </summary>
        public static global::tree_sitter.TSNode TsNodeNamedDescendantForByteRange(global::tree_sitter.TSNode _0, uint _1, uint _2)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeNamedDescendantForByteRange(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, _1, _2);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        public static global::tree_sitter.TSNode TsNodeNamedDescendantForPointRange(global::tree_sitter.TSNode _0, global::tree_sitter.TSPoint _1, global::tree_sitter.TSPoint _2)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            if (ReferenceEquals(_1, null))
                throw new global::System.ArgumentNullException("_1", "Cannot be null because it is passed by value.");
            var __arg1 = _1.__Instance;
            if (ReferenceEquals(_2, null))
                throw new global::System.ArgumentNullException("_2", "Cannot be null because it is passed by value.");
            var __arg2 = _2.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsNodeNamedDescendantForPointRange(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0, *(global::tree_sitter.TSPoint.__Internal*) __arg1, *(global::tree_sitter.TSPoint.__Internal*) __arg2);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>Edit the node to keep it in-sync with source code that has been edited.</summary>
        /// <remarks>
        /// <para>This function is only rarely needed. When you edit a syntax tree with the</para>
        /// <para>`ts_tree_edit` function, all of the nodes that you retrieve from the tree</para>
        /// <para>afterward will already reflect the edit. You only need to use `ts_node_edit`</para>
        /// <para>when you have a `TSNode` instance that you want to keep and continue to use</para>
        /// <para>after an edit.</para>
        /// </remarks>
        public static void TsNodeEdit(global::tree_sitter.TSNode _0, global::tree_sitter.TSInputEdit _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __arg1 = _1 is null ? __IntPtr.Zero : _1.__Instance;
            __Internal.TsNodeEdit(__arg0, __arg1);
        }

        /// <summary>Check if two nodes are identical.</summary>
        public static bool TsNodeEq(global::tree_sitter.TSNode _0, global::tree_sitter.TSNode _1)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            if (ReferenceEquals(_1, null))
                throw new global::System.ArgumentNullException("_1", "Cannot be null because it is passed by value.");
            var __arg1 = _1.__Instance;
            var __ret = __Internal.TsNodeEq(*(global::tree_sitter.TSNode.__Internal*) __arg0, *(global::tree_sitter.TSNode.__Internal*) __arg1);
            return __ret;
        }

        /// <summary>Create a new tree cursor starting from the given node.</summary>
        /// <remarks>
        /// <para>A tree cursor allows you to walk a syntax tree more efficiently than is</para>
        /// <para>possible using the `TSNode` functions. It is a mutable object that is always</para>
        /// <para>on a certain syntax node, and can be moved imperatively to different nodes.</para>
        /// </remarks>
        public static global::tree_sitter.TSTreeCursor TsTreeCursorNew(global::tree_sitter.TSNode _0)
        {
            if (ReferenceEquals(_0, null))
                throw new global::System.ArgumentNullException("_0", "Cannot be null because it is passed by value.");
            var __arg0 = _0.__Instance;
            var __ret = new global::tree_sitter.TSTreeCursor.__Internal();
            __Internal.TsTreeCursorNew(new IntPtr(&__ret), *(global::tree_sitter.TSNode.__Internal*) __arg0);
            return global::tree_sitter.TSTreeCursor.__CreateInstance(__ret);
        }

        /// <summary>Delete a tree cursor, freeing all of the memory that it used.</summary>
        public static void TsTreeCursorDelete(global::tree_sitter.TSTreeCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            __Internal.TsTreeCursorDelete(__arg0);
        }

        /// <summary>Re-initialize a tree cursor to start at a different node.</summary>
        public static void TsTreeCursorReset(global::tree_sitter.TSTreeCursor _0, global::tree_sitter.TSNode _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            if (ReferenceEquals(_1, null))
                throw new global::System.ArgumentNullException("_1", "Cannot be null because it is passed by value.");
            var __arg1 = _1.__Instance;
            __Internal.TsTreeCursorReset(__arg0, *(global::tree_sitter.TSNode.__Internal*) __arg1);
        }

        /// <summary>Get the tree cursor's current node.</summary>
        public static global::tree_sitter.TSNode TsTreeCursorCurrentNode(ref global::tree_sitter.TSTreeCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = new global::tree_sitter.TSNode.__Internal();
            __Internal.TsTreeCursorCurrentNode(new IntPtr(&__ret), __arg0);
            return global::tree_sitter.TSNode.__CreateInstance(__ret);
        }

        /// <summary>Get the field name of the tree cursor's current node.</summary>
        /// <remarks>
        /// <para>This returns `NULL` if the current node doesn't have a field.</para>
        /// <para>See also `ts_node_child_by_field_name`.</para>
        /// </remarks>
        public static string TsTreeCursorCurrentFieldName(global::tree_sitter.TSTreeCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsTreeCursorCurrentFieldName(__arg0);
            return CppSharp.Runtime.MarshalUtil.GetString(global::System.Text.Encoding.UTF8, __ret);
        }

        /// <summary>Get the field id of the tree cursor's current node.</summary>
        /// <remarks>
        /// <para>This returns zero if the current node doesn't have a field.</para>
        /// <para>See also `ts_node_child_by_field_id`, `ts_language_field_id_for_name`.</para>
        /// </remarks>
        public static ushort TsTreeCursorCurrentFieldId(global::tree_sitter.TSTreeCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsTreeCursorCurrentFieldId(__arg0);
            return __ret;
        }

        /// <summary>Move the cursor to the parent of its current node.</summary>
        /// <remarks>
        /// <para>This returns `true` if the cursor successfully moved, and returns `false`</para>
        /// <para>if there was no parent node (the cursor was already on the root node).</para>
        /// </remarks>
        public static bool TsTreeCursorGotoParent(ref global::tree_sitter.TSTreeCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsTreeCursorGotoParent(__arg0);
            return __ret;
        }

        /// <summary>Move the cursor to the next sibling of its current node.</summary>
        /// <remarks>
        /// <para>This returns `true` if the cursor successfully moved, and returns `false`</para>
        /// <para>if there was no next sibling node.</para>
        /// </remarks>
        public static bool TsTreeCursorGotoNextSibling(ref global::tree_sitter.TSTreeCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsTreeCursorGotoNextSibling(__arg0);
            return __ret;
        }

        /// <summary>Move the cursor to the first child of its current node.</summary>
        /// <remarks>
        /// <para>This returns `true` if the cursor successfully moved, and returns `false`</para>
        /// <para>if there were no children.</para>
        /// </remarks>
        public static bool TsTreeCursorGotoFirstChild(ref global::tree_sitter.TSTreeCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsTreeCursorGotoFirstChild(__arg0);
            return __ret;
        }

        /// <summary>
        /// <para>Move the cursor to the first child of its current node that extends beyond</para>
        /// <para>the given byte offset or point.</para>
        /// </summary>
        /// <remarks>
        /// <para>This returns the index of the child node if one was found, and returns -1</para>
        /// <para>if no such child was found.</para>
        /// </remarks>
        public static long TsTreeCursorGotoFirstChildForByte(global::tree_sitter.TSTreeCursor _0, uint _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsTreeCursorGotoFirstChildForByte(__arg0, _1);
            return __ret;
        }

        public static long TsTreeCursorGotoFirstChildForPoint(global::tree_sitter.TSTreeCursor _0, global::tree_sitter.TSPoint _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            if (ReferenceEquals(_1, null))
                throw new global::System.ArgumentNullException("_1", "Cannot be null because it is passed by value.");
            var __arg1 = _1.__Instance;
            var __ret = __Internal.TsTreeCursorGotoFirstChildForPoint(__arg0, *(global::tree_sitter.TSPoint.__Internal*) __arg1);
            return __ret;
        }

        public static global::tree_sitter.TSTreeCursor TsTreeCursorCopy(global::tree_sitter.TSTreeCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = new global::tree_sitter.TSTreeCursor.__Internal();
            __Internal.TsTreeCursorCopy(new IntPtr(&__ret), __arg0);
            return global::tree_sitter.TSTreeCursor.__CreateInstance(__ret);
        }

        /// <summary>
        /// <para>Create a new query from a string containing one or more S-expression</para>
        /// <para>patterns. The query is associated with a particular language, and can</para>
        /// <para>only be run on syntax nodes parsed with that language.</para>
        /// </summary>
        /// <remarks>
        /// <para>If all of the given patterns are valid, this returns a `TSQuery`.</para>
        /// <para>If a pattern is invalid, this returns `NULL`, and provides two pieces</para>
        /// <para>of information about the problem:</para>
        /// <para>1. The byte offset of the error is written to the `error_offset` parameter.</para>
        /// <para>2. The type of error is written to the `error_type` parameter.</para>
        /// </remarks>
        public static global::tree_sitter.TSQuery TsQueryNew(global::tree_sitter.TSLanguage language, string source, uint source_len, ref uint error_offset, ref global::tree_sitter.TSQueryError error_type)
        {
            var __arg0 = language is null ? __IntPtr.Zero : language.__Instance;
            fixed (uint* __error_offset3 = &error_offset)
            {
                var __arg3 = __error_offset3;
                fixed (global::tree_sitter.TSQueryError* __error_type4 = &error_type)
                {
                    var __arg4 = __error_type4;
                    var __ret = __Internal.TsQueryNew(__arg0, source, source_len, __arg3, __arg4);
                    var __result0 = global::tree_sitter.TSQuery.__GetOrCreateInstance(__ret, false);
                    return __result0;
                }
            }
        }

        /// <summary>Delete a query, freeing all of the memory that it used.</summary>
        public static void TsQueryDelete(global::tree_sitter.TSQuery _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            __Internal.TsQueryDelete(__arg0);
        }

        /// <summary>Get the number of patterns, captures, or string literals in the query.</summary>
        public static uint TsQueryPatternCount(global::tree_sitter.TSQuery _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsQueryPatternCount(__arg0);
            return __ret;
        }

        public static uint TsQueryCaptureCount(global::tree_sitter.TSQuery _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsQueryCaptureCount(__arg0);
            return __ret;
        }

        public static uint TsQueryStringCount(global::tree_sitter.TSQuery _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsQueryStringCount(__arg0);
            return __ret;
        }

        /// <summary>Get the byte offset where the given pattern starts in the query's source.</summary>
        /// <remarks>
        /// <para>This can be useful when combining queries by concatenating their source</para>
        /// <para>code strings.</para>
        /// </remarks>
        public static uint TsQueryStartByteForPattern(global::tree_sitter.TSQuery _0, uint _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsQueryStartByteForPattern(__arg0, _1);
            return __ret;
        }

        /// <summary>Get all of the predicates for the given pattern in the query.</summary>
        /// <remarks>
        /// <para>The predicates are represented as a single array of steps. There are three</para>
        /// <para>types of steps in this array, which correspond to the three legal values for</para>
        /// <para>the `type` field:</para>
        /// <para>- `TSQueryPredicateStepTypeCapture` - Steps with this type represent names</para>
        /// <para>of captures. Their `value_id` can be used with the</para>
        /// <para>`ts_query_capture_name_for_id` function to obtain the name of the capture.</para>
        /// <para>- `TSQueryPredicateStepTypeString` - Steps with this type represent literal</para>
        /// <para>strings. Their `value_id` can be used with the</para>
        /// <para>`ts_query_string_value_for_id` function to obtain their string value.</para>
        /// <para>- `TSQueryPredicateStepTypeDone` - Steps with this type are *sentinels*</para>
        /// <para>that represent the end of an individual predicate. If a pattern has two</para>
        /// <para>predicates, then there will be two steps with this `type` in the array.</para>
        /// </remarks>
        public static global::tree_sitter.TSQueryPredicateStep[] TsQueryPredicatesForPattern(global::tree_sitter.TSQuery self, uint pattern_index, ref uint length)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            fixed (uint* __length2 = &length)
            {
                var __arg2 = __length2;
                var __ret = __Internal.TsQueryPredicatesForPattern(__arg0, pattern_index, __arg2);
                var __result0 = new TSQueryPredicateStep[*__arg2];
                
                for (int i = 0; i < *__arg2; i++)
                  __result0[i] = global::tree_sitter.TSQueryPredicateStep.__GetOrCreateInstance(__ret + i * sizeof(TSQueryPredicateStep.__Internal), false);

                return __result0;
            }
        }

        public static bool TsQueryIsPatternGuaranteedAtStep(global::tree_sitter.TSQuery self, uint byte_offset)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __ret = __Internal.TsQueryIsPatternGuaranteedAtStep(__arg0, byte_offset);
            return __ret;
        }

        /// <summary>
        /// <para>Get the name and length of one of the query's captures, or one of the</para>
        /// <para>query's string literals. Each capture and string is associated with a</para>
        /// <para>numeric id based on the order that it appeared in the query's source.</para>
        /// </summary>
        public static string TsQueryCaptureNameForId(global::tree_sitter.TSQuery _0, uint id, ref uint length)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            fixed (uint* __length2 = &length)
            {
                var __arg2 = __length2;
                var __ret = __Internal.TsQueryCaptureNameForId(__arg0, id, __arg2);
                return CppSharp.Runtime.MarshalUtil.GetString(global::System.Text.Encoding.UTF8, __ret);
            }
        }

        /// <summary>
        /// <para>Get the quantifier of the query's captures. Each capture is * associated</para>
        /// <para>with a numeric id based on the order that it appeared in the query's source.</para>
        /// </summary>
        public static global::tree_sitter.TSQuantifier TsQueryCaptureQuantifierForId(global::tree_sitter.TSQuery _0, uint pattern_id, uint capture_id)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsQueryCaptureQuantifierForId(__arg0, pattern_id, capture_id);
            return __ret;
        }

        public static string TsQueryStringValueForId(global::tree_sitter.TSQuery _0, uint id, ref uint length)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            fixed (uint* __length2 = &length)
            {
                var __arg2 = __length2;
                var __ret = __Internal.TsQueryStringValueForId(__arg0, id, __arg2);
                return CppSharp.Runtime.MarshalUtil.GetString(global::System.Text.Encoding.UTF8, __ret);
            }
        }

        /// <summary>Disable a certain capture within a query.</summary>
        /// <remarks>
        /// <para>This prevents the capture from being returned in matches, and also avoids</para>
        /// <para>any resource usage associated with recording the capture. Currently, there</para>
        /// <para>is no way to undo this.</para>
        /// </remarks>
        public static void TsQueryDisableCapture(global::tree_sitter.TSQuery _0, string _1, uint _2)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            __Internal.TsQueryDisableCapture(__arg0, _1, _2);
        }

        /// <summary>Disable a certain pattern within a query.</summary>
        /// <remarks>
        /// <para>This prevents the pattern from matching and removes most of the overhead</para>
        /// <para>associated with the pattern. Currently, there is no way to undo this.</para>
        /// </remarks>
        public static void TsQueryDisablePattern(global::tree_sitter.TSQuery _0, uint _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            __Internal.TsQueryDisablePattern(__arg0, _1);
        }

        /// <summary>Create a new cursor for executing a given query.</summary>
        /// <remarks>
        /// <para>The cursor stores the state that is needed to iteratively search</para>
        /// <para>for matches. To use the query cursor, first call `ts_query_cursor_exec`</para>
        /// <para>to start running a given query on a given syntax node. Then, there are</para>
        /// <para>two options for consuming the results of the query:</para>
        /// <para>1. Repeatedly call `ts_query_cursor_next_match` to iterate over all of the</para>
        /// <para>*matches* in the order that they were found. Each match contains the</para>
        /// <para>index of the pattern that matched, and an array of captures. Because</para>
        /// <para>multiple patterns can match the same set of nodes, one match may contain</para>
        /// <para>captures that appear *before* some of the captures from a previous match.</para>
        /// <para>2. Repeatedly call `ts_query_cursor_next_capture` to iterate over all of the</para>
        /// <para>individual *captures* in the order that they appear. This is useful if</para>
        /// <para>don't care about which pattern matched, and just want a single ordered</para>
        /// <para>sequence of captures.</para>
        /// <para>If you don't care about consuming all of the results, you can stop calling</para>
        /// <para>`ts_query_cursor_next_match` or `ts_query_cursor_next_capture` at any point.</para>
        /// <para>You can then start executing another query on another node by calling</para>
        /// <para>`ts_query_cursor_exec` again.</para>
        /// </remarks>
        public static global::tree_sitter.TSQueryCursor TsQueryCursorNew()
        {
            var __ret = __Internal.TsQueryCursorNew();
            var __result0 = global::tree_sitter.TSQueryCursor.__GetOrCreateInstance(__ret, false);
            return __result0;
        }

        /// <summary>Delete a query cursor, freeing all of the memory that it used.</summary>
        public static void TsQueryCursorDelete(global::tree_sitter.TSQueryCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            __Internal.TsQueryCursorDelete(__arg0);
        }

        /// <summary>Start running a given query on a given node.</summary>
        public static void TsQueryCursorExec(global::tree_sitter.TSQueryCursor _0, global::tree_sitter.TSQuery _1, global::tree_sitter.TSNode _2)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __arg1 = _1 is null ? __IntPtr.Zero : _1.__Instance;
            if (ReferenceEquals(_2, null))
                throw new global::System.ArgumentNullException("_2", "Cannot be null because it is passed by value.");
            var __arg2 = _2.__Instance;
            __Internal.TsQueryCursorExec(__arg0, __arg1, *(global::tree_sitter.TSNode.__Internal*) __arg2);
        }

        /// <summary>
        /// <para>Manage the maximum number of in-progress matches allowed by this query</para>
        /// <para>cursor.</para>
        /// </summary>
        /// <remarks>
        /// <para>Query cursors have an optional maximum capacity for storing lists of</para>
        /// <para>in-progress captures. If this capacity is exceeded, then the</para>
        /// <para>earliest-starting match will silently be dropped to make room for further</para>
        /// <para>matches. This maximum capacity is optional — by default, query cursors allow</para>
        /// <para>any number of pending matches, dynamically allocating new space for them as</para>
        /// <para>needed as the query is executed.</para>
        /// </remarks>
        public static bool TsQueryCursorDidExceedMatchLimit(global::tree_sitter.TSQueryCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsQueryCursorDidExceedMatchLimit(__arg0);
            return __ret;
        }

        public static uint TsQueryCursorMatchLimit(global::tree_sitter.TSQueryCursor _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsQueryCursorMatchLimit(__arg0);
            return __ret;
        }

        public static void TsQueryCursorSetMatchLimit(global::tree_sitter.TSQueryCursor _0, uint _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            __Internal.TsQueryCursorSetMatchLimit(__arg0, _1);
        }

        /// <summary>
        /// <para>Set the range of bytes or (row, column) positions in which the query</para>
        /// <para>will be executed.</para>
        /// </summary>
        public static void TsQueryCursorSetByteRange(global::tree_sitter.TSQueryCursor _0, uint _1, uint _2)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            __Internal.TsQueryCursorSetByteRange(__arg0, _1, _2);
        }

        public static void TsQueryCursorSetPointRange(global::tree_sitter.TSQueryCursor _0, global::tree_sitter.TSPoint _1, global::tree_sitter.TSPoint _2)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            if (ReferenceEquals(_1, null))
                throw new global::System.ArgumentNullException("_1", "Cannot be null because it is passed by value.");
            var __arg1 = _1.__Instance;
            if (ReferenceEquals(_2, null))
                throw new global::System.ArgumentNullException("_2", "Cannot be null because it is passed by value.");
            var __arg2 = _2.__Instance;
            __Internal.TsQueryCursorSetPointRange(__arg0, *(global::tree_sitter.TSPoint.__Internal*) __arg1, *(global::tree_sitter.TSPoint.__Internal*) __arg2);
        }

        /// <summary>Advance to the next match of the currently running query.</summary>
        /// <remarks>
        /// <para>If there is a match, write it to `*match` and return `true`.</para>
        /// <para>Otherwise, return `false`.</para>
        /// </remarks>
        public static bool TsQueryCursorNextMatch(global::tree_sitter.TSQueryCursor _0, global::tree_sitter.TSQueryMatch match)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __arg1 = match is null ? __IntPtr.Zero : match.__Instance;
            var __ret = __Internal.TsQueryCursorNextMatch(__arg0, __arg1);
            return __ret;
        }

        public static void TsQueryCursorRemoveMatch(global::tree_sitter.TSQueryCursor _0, uint id)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            __Internal.TsQueryCursorRemoveMatch(__arg0, id);
        }

        /// <summary>Advance to the next capture of the currently running query.</summary>
        /// <remarks>
        /// <para>If there is a capture, write its match to `*match` and its index within</para>
        /// <para>the matche's capture list to `*capture_index`. Otherwise, return `false`.</para>
        /// </remarks>
        public static bool TsQueryCursorNextCapture(global::tree_sitter.TSQueryCursor _0, global::tree_sitter.TSQueryMatch match, ref uint capture_index)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __arg1 = match is null ? __IntPtr.Zero : match.__Instance;
            fixed (uint* __capture_index2 = &capture_index)
            {
                var __arg2 = __capture_index2;
                var __ret = __Internal.TsQueryCursorNextCapture(__arg0, __arg1, __arg2);
                return __ret;
            }
        }

        /// <summary>Get the number of distinct node types in the language.</summary>
        public static uint TsLanguageSymbolCount(global::tree_sitter.TSLanguage _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsLanguageSymbolCount(__arg0);
            return __ret;
        }

        /// <summary>Get a node type string for the given numerical id.</summary>
        public static string TsLanguageSymbolName(global::tree_sitter.TSLanguage _0, ushort _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsLanguageSymbolName(__arg0, _1);
            return CppSharp.Runtime.MarshalUtil.GetString(global::System.Text.Encoding.UTF8, __ret);
        }

        /// <summary>Get the numerical id for the given node type string.</summary>
        public static ushort TsLanguageSymbolForName(global::tree_sitter.TSLanguage self, string @string, uint length, bool is_named)
        {
            var __arg0 = self is null ? __IntPtr.Zero : self.__Instance;
            var __ret = __Internal.TsLanguageSymbolForName(__arg0, @string, length, is_named);
            return __ret;
        }

        /// <summary>Get the number of distinct field names in the language.</summary>
        public static uint TsLanguageFieldCount(global::tree_sitter.TSLanguage _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsLanguageFieldCount(__arg0);
            return __ret;
        }

        /// <summary>Get the field name string for the given numerical id.</summary>
        public static string TsLanguageFieldNameForId(global::tree_sitter.TSLanguage _0, ushort _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsLanguageFieldNameForId(__arg0, _1);
            return CppSharp.Runtime.MarshalUtil.GetString(global::System.Text.Encoding.UTF8, __ret);
        }

        /// <summary>Get the numerical id for the given field name string.</summary>
        public static ushort TsLanguageFieldIdForName(global::tree_sitter.TSLanguage _0, string _1, uint _2)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsLanguageFieldIdForName(__arg0, _1, _2);
            return __ret;
        }

        /// <summary>
        /// <para>Check whether the given node type id belongs to named nodes, anonymous nodes,</para>
        /// <para>or a hidden nodes.</para>
        /// </summary>
        /// <remarks>See also `ts_node_is_named`. Hidden nodes are never returned from the API.</remarks>
        public static global::tree_sitter.TSSymbolType TsLanguageSymbolType(global::tree_sitter.TSLanguage _0, ushort _1)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsLanguageSymbolType(__arg0, _1);
            return __ret;
        }

        /// <summary>
        /// <para>Get the ABI version number for this language. This version number is used</para>
        /// <para>to ensure that languages were generated by a compatible version of</para>
        /// <para>Tree-sitter.</para>
        /// </summary>
        /// <remarks>See also `ts_parser_set_language`.</remarks>
        public static uint TsLanguageVersion(global::tree_sitter.TSLanguage _0)
        {
            var __arg0 = _0 is null ? __IntPtr.Zero : _0.__Instance;
            var __ret = __Internal.TsLanguageVersion(__arg0);
            return __ret;
        }

        /// <summary>Set the allocation functions used by the library.</summary>
        /// <remarks>
        /// <para>By default, Tree-sitter uses the standard libc allocation functions,</para>
        /// <para>but aborts the process when an allocation fails. This function lets</para>
        /// <para>you supply alternative allocation functions at runtime.</para>
        /// <para>If you pass `NULL` for any parameter, Tree-sitter will switch back to</para>
        /// <para>its default implementation of that function.</para>
        /// <para>If you call this function after the library has already been used, then</para>
        /// <para>you must ensure that either:</para>
        /// <para>1. All the existing objects have been freed.</para>
        /// <para>2. The new allocator shares its state with the old one, so it is capable</para>
        /// <para>of freeing memory that was allocated by the old allocator.</para>
        /// </remarks>
        public static void TsSetAllocator(global::tree_sitter.Delegates.Func___IntPtr_uint new_malloc, global::tree_sitter.Delegates.Func___IntPtr_uint_uint new_calloc, global::tree_sitter.Delegates.Func___IntPtr___IntPtr_uint new_realloc, global::tree_sitter.Delegates.Action___IntPtr new_free)
        {
            var __arg0 = new_malloc == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(new_malloc);
            var __arg1 = new_calloc == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(new_calloc);
            var __arg2 = new_realloc == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(new_realloc);
            var __arg3 = new_free == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(new_free);
            __Internal.TsSetAllocator(__arg0, __arg1, __arg2, __arg3);
        }
    }

    public enum TSParseActionType
    {
        TSParseActionTypeShift = 0,
        TSParseActionTypeReduce = 1,
        TSParseActionTypeAccept = 2,
        TSParseActionTypeRecover = 3
    }

    public unsafe partial class TSFieldMapEntry : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 4)]
        public partial struct __Internal
        {
            internal ushort field_id;
            internal byte child_index;
            internal byte inherited;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSFieldMapEntry@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSFieldMapEntry> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSFieldMapEntry>();

        protected bool __ownsNativeInstance;

        internal static TSFieldMapEntry __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSFieldMapEntry(native.ToPointer(), skipVTables);
        }

        internal static TSFieldMapEntry __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSFieldMapEntry)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSFieldMapEntry __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSFieldMapEntry(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSFieldMapEntry(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSFieldMapEntry(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSFieldMapEntry()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSFieldMapEntry.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSFieldMapEntry(global::tree_sitter.TSFieldMapEntry __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSFieldMapEntry.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSFieldMapEntry.__Internal*) __Instance) = *((global::tree_sitter.TSFieldMapEntry.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public ushort FieldId
        {
            get
            {
                return ((__Internal*)__Instance)->field_id;
            }

            set
            {
                ((__Internal*)__Instance)->field_id = value;
            }
        }

        public byte ChildIndex
        {
            get
            {
                return ((__Internal*)__Instance)->child_index;
            }

            set
            {
                ((__Internal*)__Instance)->child_index = value;
            }
        }

        public bool Inherited
        {
            get
            {
                return ((__Internal*)__Instance)->inherited != 0;
            }

            set
            {
                ((__Internal*)__Instance)->inherited = (byte) (value ? 1 : 0);
            }
        }
    }

    public unsafe partial class TSFieldMapSlice : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 4)]
        public partial struct __Internal
        {
            internal ushort index;
            internal ushort length;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSFieldMapSlice@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSFieldMapSlice> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSFieldMapSlice>();

        protected bool __ownsNativeInstance;

        internal static TSFieldMapSlice __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSFieldMapSlice(native.ToPointer(), skipVTables);
        }

        internal static TSFieldMapSlice __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSFieldMapSlice)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSFieldMapSlice __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSFieldMapSlice(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSFieldMapSlice(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSFieldMapSlice(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSFieldMapSlice()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSFieldMapSlice.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSFieldMapSlice(global::tree_sitter.TSFieldMapSlice __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSFieldMapSlice.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSFieldMapSlice.__Internal*) __Instance) = *((global::tree_sitter.TSFieldMapSlice.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public ushort Index
        {
            get
            {
                return ((__Internal*)__Instance)->index;
            }

            set
            {
                ((__Internal*)__Instance)->index = value;
            }
        }

        public ushort Length
        {
            get
            {
                return ((__Internal*)__Instance)->length;
            }

            set
            {
                ((__Internal*)__Instance)->length = value;
            }
        }
    }

    public unsafe partial class TSSymbolMetadata : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 3)]
        public partial struct __Internal
        {
            internal byte visible;
            internal byte named;
            internal byte supertype;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSSymbolMetadata@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSSymbolMetadata> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSSymbolMetadata>();

        protected bool __ownsNativeInstance;

        internal static TSSymbolMetadata __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSSymbolMetadata(native.ToPointer(), skipVTables);
        }

        internal static TSSymbolMetadata __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSSymbolMetadata)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSSymbolMetadata __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSSymbolMetadata(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSSymbolMetadata(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSSymbolMetadata(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSSymbolMetadata()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSSymbolMetadata.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSSymbolMetadata(global::tree_sitter.TSSymbolMetadata __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSSymbolMetadata.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSSymbolMetadata.__Internal*) __Instance) = *((global::tree_sitter.TSSymbolMetadata.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public bool Visible
        {
            get
            {
                return ((__Internal*)__Instance)->visible != 0;
            }

            set
            {
                ((__Internal*)__Instance)->visible = (byte) (value ? 1 : 0);
            }
        }

        public bool Named
        {
            get
            {
                return ((__Internal*)__Instance)->named != 0;
            }

            set
            {
                ((__Internal*)__Instance)->named = (byte) (value ? 1 : 0);
            }
        }

        public bool Supertype
        {
            get
            {
                return ((__Internal*)__Instance)->supertype != 0;
            }

            set
            {
                ((__Internal*)__Instance)->supertype = (byte) (value ? 1 : 0);
            }
        }
    }

    public unsafe partial class TSLexer : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 28)]
        public partial struct __Internal
        {
            internal int lookahead;
            internal ushort result_symbol;
            internal __IntPtr advance;
            internal __IntPtr mark_end;
            internal __IntPtr get_column;
            internal __IntPtr is_at_included_range_start;
            internal __IntPtr eof;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSLexer@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr _0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLexer> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLexer>();

        protected bool __ownsNativeInstance;

        internal static TSLexer __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSLexer(native.ToPointer(), skipVTables);
        }

        internal static TSLexer __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSLexer)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSLexer __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSLexer(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSLexer(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSLexer(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSLexer()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLexer.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSLexer(global::tree_sitter.TSLexer _0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLexer.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSLexer.__Internal*) __Instance) = *((global::tree_sitter.TSLexer.__Internal*) _0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public int Lookahead
        {
            get
            {
                return ((__Internal*)__Instance)->lookahead;
            }

            set
            {
                ((__Internal*)__Instance)->lookahead = value;
            }
        }

        public ushort ResultSymbol
        {
            get
            {
                return ((__Internal*)__Instance)->result_symbol;
            }

            set
            {
                ((__Internal*)__Instance)->result_symbol = value;
            }
        }

        public global::tree_sitter.Delegates.Action___IntPtr_bool Advance
        {
            get
            {
                var __ptr0 = ((__Internal*)__Instance)->advance;
                return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Action___IntPtr_bool) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Action___IntPtr_bool));
            }

            set
            {
                ((__Internal*)__Instance)->advance = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            }
        }

        public global::tree_sitter.Delegates.Action___IntPtr MarkEnd
        {
            get
            {
                var __ptr0 = ((__Internal*)__Instance)->mark_end;
                return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Action___IntPtr) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Action___IntPtr));
            }

            set
            {
                ((__Internal*)__Instance)->mark_end = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            }
        }

        public global::tree_sitter.Delegates.Func_uint___IntPtr GetColumn
        {
            get
            {
                var __ptr0 = ((__Internal*)__Instance)->get_column;
                return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Func_uint___IntPtr) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Func_uint___IntPtr));
            }

            set
            {
                ((__Internal*)__Instance)->get_column = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            }
        }

        public global::tree_sitter.Delegates.Func_bool___IntPtr IsAtIncludedRangeStart
        {
            get
            {
                var __ptr0 = ((__Internal*)__Instance)->is_at_included_range_start;
                return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Func_bool___IntPtr) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Func_bool___IntPtr));
            }

            set
            {
                ((__Internal*)__Instance)->is_at_included_range_start = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            }
        }

        public global::tree_sitter.Delegates.Func_bool___IntPtr Eof
        {
            get
            {
                var __ptr0 = ((__Internal*)__Instance)->eof;
                return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Func_bool___IntPtr) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Func_bool___IntPtr));
            }

            set
            {
                ((__Internal*)__Instance)->eof = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            }
        }
    }

    public unsafe partial struct TSParseAction
    {
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public partial struct __Internal
        {
            [FieldOffset(0)]
            internal global::tree_sitter.TSParseAction.Shift.__Internal shift;

            [FieldOffset(0)]
            internal global::tree_sitter.TSParseAction.Reduce.__Internal reduce;

            [FieldOffset(0)]
            internal byte type;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSParseAction@@QAE@ABT0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public unsafe partial class Shift : IDisposable
        {
            [StructLayout(LayoutKind.Sequential, Size = 6)]
            public partial struct __Internal
            {
                internal byte type;
                internal ushort state;
                internal byte extra;
                internal byte repetition;

                [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0<unnamed-type-shift>@TSParseAction@@QAE@ABU01@@Z", CallingConvention = __CallingConvention.ThisCall)]
                internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
            }

            public __IntPtr __Instance { get; protected set; }

            internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSParseAction.Shift> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSParseAction.Shift>();

            protected bool __ownsNativeInstance;

            internal static Shift __CreateInstance(__IntPtr native, bool skipVTables = false)
            {
                return new Shift(native.ToPointer(), skipVTables);
            }

            internal static Shift __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
            {
                if (native == __IntPtr.Zero)
                    return null;
                if (NativeToManagedMap.TryGetValue(native, out var managed))
                    return (Shift)managed;
                var result = __CreateInstance(native, skipVTables);
                if (saveInstance)
                    NativeToManagedMap[native] = result;
                return result;
            }

            internal static Shift __CreateInstance(__Internal native, bool skipVTables = false)
            {
                return new Shift(native, skipVTables);
            }

            private static void* __CopyValue(__Internal native)
            {
                var ret = Marshal.AllocHGlobal(sizeof(__Internal));
                *(__Internal*) ret = native;
                return ret.ToPointer();
            }

            private Shift(__Internal native, bool skipVTables = false)
                : this(__CopyValue(native), skipVTables)
            {
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
            }

            protected Shift(void* native, bool skipVTables = false)
            {
                if (native == null)
                    return;
                __Instance = new __IntPtr(native);
            }

            public Shift()
            {
                __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSParseAction.Shift.__Internal));
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
            }

            public Shift(global::tree_sitter.TSParseAction.Shift __0)
            {
                __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSParseAction.Shift.__Internal));
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
                *((global::tree_sitter.TSParseAction.Shift.__Internal*) __Instance) = *((global::tree_sitter.TSParseAction.Shift.__Internal*) __0.__Instance);
            }

            public void Dispose()
            {
                Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
            }

            partial void DisposePartial(bool disposing);

            internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
            {
                if (__Instance == IntPtr.Zero)
                    return;
                NativeToManagedMap.TryRemove(__Instance, out _);
                DisposePartial(disposing);
                if (__ownsNativeInstance)
                    Marshal.FreeHGlobal(__Instance);
                __Instance = IntPtr.Zero;
            }

            public byte Type
            {
                get
                {
                    return ((__Internal*)__Instance)->type;
                }

                set
                {
                    ((__Internal*)__Instance)->type = value;
                }
            }

            public ushort State
            {
                get
                {
                    return ((__Internal*)__Instance)->state;
                }

                set
                {
                    ((__Internal*)__Instance)->state = value;
                }
            }

            public bool Extra
            {
                get
                {
                    return ((__Internal*)__Instance)->extra != 0;
                }

                set
                {
                    ((__Internal*)__Instance)->extra = (byte) (value ? 1 : 0);
                }
            }

            public bool Repetition
            {
                get
                {
                    return ((__Internal*)__Instance)->repetition != 0;
                }

                set
                {
                    ((__Internal*)__Instance)->repetition = (byte) (value ? 1 : 0);
                }
            }
        }

        public unsafe partial class Reduce : IDisposable
        {
            [StructLayout(LayoutKind.Sequential, Size = 8)]
            public partial struct __Internal
            {
                internal byte type;
                internal byte child_count;
                internal ushort symbol;
                internal short dynamic_precedence;
                internal ushort production_id;

                [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0<unnamed-type-reduce>@TSParseAction@@QAE@ABU01@@Z", CallingConvention = __CallingConvention.ThisCall)]
                internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
            }

            public __IntPtr __Instance { get; protected set; }

            internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSParseAction.Reduce> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSParseAction.Reduce>();

            protected bool __ownsNativeInstance;

            internal static Reduce __CreateInstance(__IntPtr native, bool skipVTables = false)
            {
                return new Reduce(native.ToPointer(), skipVTables);
            }

            internal static Reduce __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
            {
                if (native == __IntPtr.Zero)
                    return null;
                if (NativeToManagedMap.TryGetValue(native, out var managed))
                    return (Reduce)managed;
                var result = __CreateInstance(native, skipVTables);
                if (saveInstance)
                    NativeToManagedMap[native] = result;
                return result;
            }

            internal static Reduce __CreateInstance(__Internal native, bool skipVTables = false)
            {
                return new Reduce(native, skipVTables);
            }

            private static void* __CopyValue(__Internal native)
            {
                var ret = Marshal.AllocHGlobal(sizeof(__Internal));
                *(__Internal*) ret = native;
                return ret.ToPointer();
            }

            private Reduce(__Internal native, bool skipVTables = false)
                : this(__CopyValue(native), skipVTables)
            {
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
            }

            protected Reduce(void* native, bool skipVTables = false)
            {
                if (native == null)
                    return;
                __Instance = new __IntPtr(native);
            }

            public Reduce()
            {
                __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSParseAction.Reduce.__Internal));
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
            }

            public Reduce(global::tree_sitter.TSParseAction.Reduce __0)
            {
                __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSParseAction.Reduce.__Internal));
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
                *((global::tree_sitter.TSParseAction.Reduce.__Internal*) __Instance) = *((global::tree_sitter.TSParseAction.Reduce.__Internal*) __0.__Instance);
            }

            public void Dispose()
            {
                Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
            }

            partial void DisposePartial(bool disposing);

            internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
            {
                if (__Instance == IntPtr.Zero)
                    return;
                NativeToManagedMap.TryRemove(__Instance, out _);
                DisposePartial(disposing);
                if (__ownsNativeInstance)
                    Marshal.FreeHGlobal(__Instance);
                __Instance = IntPtr.Zero;
            }

            public byte Type
            {
                get
                {
                    return ((__Internal*)__Instance)->type;
                }

                set
                {
                    ((__Internal*)__Instance)->type = value;
                }
            }

            public byte ChildCount
            {
                get
                {
                    return ((__Internal*)__Instance)->child_count;
                }

                set
                {
                    ((__Internal*)__Instance)->child_count = value;
                }
            }

            public ushort Symbol
            {
                get
                {
                    return ((__Internal*)__Instance)->symbol;
                }

                set
                {
                    ((__Internal*)__Instance)->symbol = value;
                }
            }

            public short DynamicPrecedence
            {
                get
                {
                    return ((__Internal*)__Instance)->dynamic_precedence;
                }

                set
                {
                    ((__Internal*)__Instance)->dynamic_precedence = value;
                }
            }

            public ushort ProductionId
            {
                get
                {
                    return ((__Internal*)__Instance)->production_id;
                }

                set
                {
                    ((__Internal*)__Instance)->production_id = value;
                }
            }
        }

        private TSParseAction.__Internal __instance;
        internal TSParseAction.__Internal __Instance { get { return __instance; } }

        internal static TSParseAction __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSParseAction(native.ToPointer(), skipVTables);
        }

        internal static TSParseAction __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSParseAction(native, skipVTables);
        }

        private TSParseAction(__Internal native, bool skipVTables = false)
            : this()
        {
            __instance = native;
        }

        private TSParseAction(void* native, bool skipVTables = false) : this()
        {
            __instance = *(global::tree_sitter.TSParseAction.__Internal*) native;
        }

        public TSParseAction(global::tree_sitter.TSParseAction __0)
            : this()
        {
            var ____arg0 = __0.__Instance;
            var __arg0 = new __IntPtr(&____arg0);
            fixed (__Internal* __instancePtr = &__instance)
            {
                __Internal.cctor(new __IntPtr(__instancePtr), __arg0);
            }
        }

        public global::tree_sitter.TSParseAction.Shift shift
        {
            get
            {
                return global::tree_sitter.TSParseAction.Shift.__CreateInstance(__instance.shift);
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                __instance.shift = *(global::tree_sitter.TSParseAction.Shift.__Internal*) value.__Instance;
            }
        }

        public global::tree_sitter.TSParseAction.Reduce reduce
        {
            get
            {
                return global::tree_sitter.TSParseAction.Reduce.__CreateInstance(__instance.reduce);
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                __instance.reduce = *(global::tree_sitter.TSParseAction.Reduce.__Internal*) value.__Instance;
            }
        }

        public byte Type
        {
            get
            {
                return __instance.type;
            }

            set
            {
                __instance.type = value;
            }
        }
    }

    public unsafe partial class TSLexMode : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 4)]
        public partial struct __Internal
        {
            internal ushort lex_state;
            internal ushort external_lex_state;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSLexMode@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLexMode> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLexMode>();

        protected bool __ownsNativeInstance;

        internal static TSLexMode __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSLexMode(native.ToPointer(), skipVTables);
        }

        internal static TSLexMode __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSLexMode)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSLexMode __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSLexMode(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSLexMode(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSLexMode(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSLexMode()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLexMode.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSLexMode(global::tree_sitter.TSLexMode __0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLexMode.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSLexMode.__Internal*) __Instance) = *((global::tree_sitter.TSLexMode.__Internal*) __0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public ushort LexState
        {
            get
            {
                return ((__Internal*)__Instance)->lex_state;
            }

            set
            {
                ((__Internal*)__Instance)->lex_state = value;
            }
        }

        public ushort ExternalLexState
        {
            get
            {
                return ((__Internal*)__Instance)->external_lex_state;
            }

            set
            {
                ((__Internal*)__Instance)->external_lex_state = value;
            }
        }
    }

    public unsafe partial struct TSParseActionEntry
    {
        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public partial struct __Internal
        {
            [FieldOffset(0)]
            internal global::tree_sitter.TSParseAction.__Internal action;

            [FieldOffset(0)]
            internal global::tree_sitter.TSParseActionEntry.Entry.__Internal entry;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSParseActionEntry@@QAE@ABT0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
        }

        public unsafe partial class Entry : IDisposable
        {
            [StructLayout(LayoutKind.Sequential, Size = 2)]
            public partial struct __Internal
            {
                internal byte count;
                internal byte reusable;

                [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0<unnamed-type-entry>@TSParseActionEntry@@QAE@ABU01@@Z", CallingConvention = __CallingConvention.ThisCall)]
                internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
            }

            public __IntPtr __Instance { get; protected set; }

            internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSParseActionEntry.Entry> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSParseActionEntry.Entry>();

            protected bool __ownsNativeInstance;

            internal static Entry __CreateInstance(__IntPtr native, bool skipVTables = false)
            {
                return new Entry(native.ToPointer(), skipVTables);
            }

            internal static Entry __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
            {
                if (native == __IntPtr.Zero)
                    return null;
                if (NativeToManagedMap.TryGetValue(native, out var managed))
                    return (Entry)managed;
                var result = __CreateInstance(native, skipVTables);
                if (saveInstance)
                    NativeToManagedMap[native] = result;
                return result;
            }

            internal static Entry __CreateInstance(__Internal native, bool skipVTables = false)
            {
                return new Entry(native, skipVTables);
            }

            private static void* __CopyValue(__Internal native)
            {
                var ret = Marshal.AllocHGlobal(sizeof(__Internal));
                *(__Internal*) ret = native;
                return ret.ToPointer();
            }

            private Entry(__Internal native, bool skipVTables = false)
                : this(__CopyValue(native), skipVTables)
            {
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
            }

            protected Entry(void* native, bool skipVTables = false)
            {
                if (native == null)
                    return;
                __Instance = new __IntPtr(native);
            }

            public Entry()
            {
                __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSParseActionEntry.Entry.__Internal));
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
            }

            public Entry(global::tree_sitter.TSParseActionEntry.Entry __0)
            {
                __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSParseActionEntry.Entry.__Internal));
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
                *((global::tree_sitter.TSParseActionEntry.Entry.__Internal*) __Instance) = *((global::tree_sitter.TSParseActionEntry.Entry.__Internal*) __0.__Instance);
            }

            public void Dispose()
            {
                Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
            }

            partial void DisposePartial(bool disposing);

            internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
            {
                if (__Instance == IntPtr.Zero)
                    return;
                NativeToManagedMap.TryRemove(__Instance, out _);
                DisposePartial(disposing);
                if (__ownsNativeInstance)
                    Marshal.FreeHGlobal(__Instance);
                __Instance = IntPtr.Zero;
            }

            public byte Count
            {
                get
                {
                    return ((__Internal*)__Instance)->count;
                }

                set
                {
                    ((__Internal*)__Instance)->count = value;
                }
            }

            public bool Reusable
            {
                get
                {
                    return ((__Internal*)__Instance)->reusable != 0;
                }

                set
                {
                    ((__Internal*)__Instance)->reusable = (byte) (value ? 1 : 0);
                }
            }
        }

        private TSParseActionEntry.__Internal __instance;
        internal TSParseActionEntry.__Internal __Instance { get { return __instance; } }

        internal static TSParseActionEntry __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSParseActionEntry(native.ToPointer(), skipVTables);
        }

        internal static TSParseActionEntry __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSParseActionEntry(native, skipVTables);
        }

        private TSParseActionEntry(__Internal native, bool skipVTables = false)
            : this()
        {
            __instance = native;
        }

        private TSParseActionEntry(void* native, bool skipVTables = false) : this()
        {
            __instance = *(global::tree_sitter.TSParseActionEntry.__Internal*) native;
        }

        public TSParseActionEntry(global::tree_sitter.TSParseActionEntry __0)
            : this()
        {
            var ____arg0 = __0.__Instance;
            var __arg0 = new __IntPtr(&____arg0);
            fixed (__Internal* __instancePtr = &__instance)
            {
                __Internal.cctor(new __IntPtr(__instancePtr), __arg0);
            }
        }

        public global::tree_sitter.TSParseAction Action;

        public global::tree_sitter.TSParseActionEntry.Entry entry
        {
            get
            {
                return global::tree_sitter.TSParseActionEntry.Entry.__CreateInstance(__instance.entry);
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                __instance.entry = *(global::tree_sitter.TSParseActionEntry.Entry.__Internal*) value.__Instance;
            }
        }
    }

    public unsafe partial class TSLanguage : IDisposable
    {
        [StructLayout(LayoutKind.Sequential, Size = 132)]
        public partial struct __Internal
        {
            internal uint version;
            internal uint symbol_count;
            internal uint alias_count;
            internal uint token_count;
            internal uint external_token_count;
            internal uint state_count;
            internal uint large_state_count;
            internal uint production_id_count;
            internal uint field_count;
            internal ushort max_alias_sequence_length;
            internal __IntPtr parse_table;
            internal __IntPtr small_parse_table;
            internal __IntPtr small_parse_table_map;
            internal __IntPtr parse_actions;
            internal __IntPtr symbol_names;
            internal __IntPtr field_names;
            internal __IntPtr field_map_slices;
            internal __IntPtr field_map_entries;
            internal __IntPtr symbol_metadata;
            internal __IntPtr public_symbol_map;
            internal __IntPtr alias_map;
            internal __IntPtr alias_sequences;
            internal __IntPtr lex_modes;
            internal __IntPtr lex_fn;
            internal __IntPtr keyword_lex_fn;
            internal ushort keyword_capture_token;
            internal global::tree_sitter.TSLanguage.ExternalScanner.__Internal external_scanner;

            [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0TSLanguage@@QAE@ABU0@@Z", CallingConvention = __CallingConvention.ThisCall)]
            internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr _0);
        }

        public unsafe partial class ExternalScanner : IDisposable
        {
            [StructLayout(LayoutKind.Sequential, Size = 28)]
            public partial struct __Internal
            {
                internal __IntPtr states;
                internal __IntPtr symbol_map;
                internal __IntPtr create;
                internal __IntPtr destroy;
                internal __IntPtr scan;
                internal __IntPtr serialize;
                internal __IntPtr deserialize;

                [SuppressUnmanagedCodeSecurity, DllImport("tree_sitter", EntryPoint = "??0<unnamed-type-external_scanner>@TSLanguage@@QAE@ABU01@@Z", CallingConvention = __CallingConvention.ThisCall)]
                internal static extern __IntPtr cctor(__IntPtr __instance, __IntPtr __0);
            }

            public __IntPtr __Instance { get; protected set; }

            internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLanguage.ExternalScanner> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLanguage.ExternalScanner>();

            protected bool __ownsNativeInstance;

            internal static ExternalScanner __CreateInstance(__IntPtr native, bool skipVTables = false)
            {
                return new ExternalScanner(native.ToPointer(), skipVTables);
            }

            internal static ExternalScanner __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
            {
                if (native == __IntPtr.Zero)
                    return null;
                if (NativeToManagedMap.TryGetValue(native, out var managed))
                    return (ExternalScanner)managed;
                var result = __CreateInstance(native, skipVTables);
                if (saveInstance)
                    NativeToManagedMap[native] = result;
                return result;
            }

            internal static ExternalScanner __CreateInstance(__Internal native, bool skipVTables = false)
            {
                return new ExternalScanner(native, skipVTables);
            }

            private static void* __CopyValue(__Internal native)
            {
                var ret = Marshal.AllocHGlobal(sizeof(__Internal));
                *(__Internal*) ret = native;
                return ret.ToPointer();
            }

            private ExternalScanner(__Internal native, bool skipVTables = false)
                : this(__CopyValue(native), skipVTables)
            {
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
            }

            protected ExternalScanner(void* native, bool skipVTables = false)
            {
                if (native == null)
                    return;
                __Instance = new __IntPtr(native);
            }

            public ExternalScanner()
            {
                __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLanguage.ExternalScanner.__Internal));
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
            }

            public ExternalScanner(global::tree_sitter.TSLanguage.ExternalScanner __0)
            {
                __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLanguage.ExternalScanner.__Internal));
                __ownsNativeInstance = true;
                NativeToManagedMap[__Instance] = this;
                *((global::tree_sitter.TSLanguage.ExternalScanner.__Internal*) __Instance) = *((global::tree_sitter.TSLanguage.ExternalScanner.__Internal*) __0.__Instance);
            }

            public void Dispose()
            {
                Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
            }

            partial void DisposePartial(bool disposing);

            internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
            {
                if (__Instance == IntPtr.Zero)
                    return;
                NativeToManagedMap.TryRemove(__Instance, out _);
                DisposePartial(disposing);
                if (__ownsNativeInstance)
                    Marshal.FreeHGlobal(__Instance);
                __Instance = IntPtr.Zero;
            }

            public bool* States
            {
                get
                {
                    return (bool*) ((__Internal*)__Instance)->states;
                }
            }

            public ushort* SymbolMap
            {
                get
                {
                    return (ushort*) ((__Internal*)__Instance)->symbol_map;
                }
            }

            public global::tree_sitter.Delegates.Func___IntPtr Create
            {
                get
                {
                    var __ptr0 = ((__Internal*)__Instance)->create;
                    return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Func___IntPtr) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Func___IntPtr));
                }

                set
                {
                    ((__Internal*)__Instance)->create = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
                }
            }

            public global::tree_sitter.Delegates.Action___IntPtr Destroy
            {
                get
                {
                    var __ptr0 = ((__Internal*)__Instance)->destroy;
                    return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Action___IntPtr) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Action___IntPtr));
                }

                set
                {
                    ((__Internal*)__Instance)->destroy = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
                }
            }

            public global::tree_sitter.Delegates.Func_bool___IntPtr___IntPtr_boolPtr Scan
            {
                get
                {
                    var __ptr0 = ((__Internal*)__Instance)->scan;
                    return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Func_bool___IntPtr___IntPtr_boolPtr) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Func_bool___IntPtr___IntPtr_boolPtr));
                }

                set
                {
                    ((__Internal*)__Instance)->scan = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
                }
            }

            public global::tree_sitter.Delegates.Func_uint___IntPtr_sbytePtr Serialize
            {
                get
                {
                    var __ptr0 = ((__Internal*)__Instance)->serialize;
                    return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Func_uint___IntPtr_sbytePtr) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Func_uint___IntPtr_sbytePtr));
                }

                set
                {
                    ((__Internal*)__Instance)->serialize = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
                }
            }

            public global::tree_sitter.Delegates.Action___IntPtr_string8_uint Deserialize
            {
                get
                {
                    var __ptr0 = ((__Internal*)__Instance)->deserialize;
                    return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Action___IntPtr_string8_uint) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Action___IntPtr_string8_uint));
                }

                set
                {
                    ((__Internal*)__Instance)->deserialize = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
                }
            }
        }

        public __IntPtr __Instance { get; protected set; }

        internal static readonly global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLanguage> NativeToManagedMap = new global::System.Collections.Concurrent.ConcurrentDictionary<IntPtr, global::tree_sitter.TSLanguage>();

        protected bool __ownsNativeInstance;

        internal static TSLanguage __CreateInstance(__IntPtr native, bool skipVTables = false)
        {
            return new TSLanguage(native.ToPointer(), skipVTables);
        }

        internal static TSLanguage __GetOrCreateInstance(__IntPtr native, bool saveInstance = false, bool skipVTables = false)
        {
            if (native == __IntPtr.Zero)
                return null;
            if (NativeToManagedMap.TryGetValue(native, out var managed))
                return (TSLanguage)managed;
            var result = __CreateInstance(native, skipVTables);
            if (saveInstance)
                NativeToManagedMap[native] = result;
            return result;
        }

        internal static TSLanguage __CreateInstance(__Internal native, bool skipVTables = false)
        {
            return new TSLanguage(native, skipVTables);
        }

        private static void* __CopyValue(__Internal native)
        {
            var ret = Marshal.AllocHGlobal(sizeof(__Internal));
            *(__Internal*) ret = native;
            return ret.ToPointer();
        }

        private TSLanguage(__Internal native, bool skipVTables = false)
            : this(__CopyValue(native), skipVTables)
        {
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        protected TSLanguage(void* native, bool skipVTables = false)
        {
            if (native == null)
                return;
            __Instance = new __IntPtr(native);
        }

        public TSLanguage()
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLanguage.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
        }

        public TSLanguage(global::tree_sitter.TSLanguage _0)
        {
            __Instance = Marshal.AllocHGlobal(sizeof(global::tree_sitter.TSLanguage.__Internal));
            __ownsNativeInstance = true;
            NativeToManagedMap[__Instance] = this;
            *((global::tree_sitter.TSLanguage.__Internal*) __Instance) = *((global::tree_sitter.TSLanguage.__Internal*) _0.__Instance);
        }

        public void Dispose()
        {
            Dispose(disposing: true, callNativeDtor : __ownsNativeInstance );
        }

        partial void DisposePartial(bool disposing);

        internal protected virtual void Dispose(bool disposing, bool callNativeDtor )
        {
            if (__Instance == IntPtr.Zero)
                return;
            NativeToManagedMap.TryRemove(__Instance, out _);
            DisposePartial(disposing);
            if (__ownsNativeInstance)
                Marshal.FreeHGlobal(__Instance);
            __Instance = IntPtr.Zero;
        }

        public uint Version
        {
            get
            {
                return ((__Internal*)__Instance)->version;
            }

            set
            {
                ((__Internal*)__Instance)->version = value;
            }
        }

        public uint SymbolCount
        {
            get
            {
                return ((__Internal*)__Instance)->symbol_count;
            }

            set
            {
                ((__Internal*)__Instance)->symbol_count = value;
            }
        }

        public uint AliasCount
        {
            get
            {
                return ((__Internal*)__Instance)->alias_count;
            }

            set
            {
                ((__Internal*)__Instance)->alias_count = value;
            }
        }

        public uint TokenCount
        {
            get
            {
                return ((__Internal*)__Instance)->token_count;
            }

            set
            {
                ((__Internal*)__Instance)->token_count = value;
            }
        }

        public uint ExternalTokenCount
        {
            get
            {
                return ((__Internal*)__Instance)->external_token_count;
            }

            set
            {
                ((__Internal*)__Instance)->external_token_count = value;
            }
        }

        public uint StateCount
        {
            get
            {
                return ((__Internal*)__Instance)->state_count;
            }

            set
            {
                ((__Internal*)__Instance)->state_count = value;
            }
        }

        public uint LargeStateCount
        {
            get
            {
                return ((__Internal*)__Instance)->large_state_count;
            }

            set
            {
                ((__Internal*)__Instance)->large_state_count = value;
            }
        }

        public uint ProductionIdCount
        {
            get
            {
                return ((__Internal*)__Instance)->production_id_count;
            }

            set
            {
                ((__Internal*)__Instance)->production_id_count = value;
            }
        }

        public uint FieldCount
        {
            get
            {
                return ((__Internal*)__Instance)->field_count;
            }

            set
            {
                ((__Internal*)__Instance)->field_count = value;
            }
        }

        public ushort MaxAliasSequenceLength
        {
            get
            {
                return ((__Internal*)__Instance)->max_alias_sequence_length;
            }

            set
            {
                ((__Internal*)__Instance)->max_alias_sequence_length = value;
            }
        }

        public ushort* ParseTable
        {
            get
            {
                return (ushort*) ((__Internal*)__Instance)->parse_table;
            }
        }

        public ushort* SmallParseTable
        {
            get
            {
                return (ushort*) ((__Internal*)__Instance)->small_parse_table;
            }
        }

        public uint* SmallParseTableMap
        {
            get
            {
                return (uint*) ((__Internal*)__Instance)->small_parse_table_map;
            }
        }

        public global::tree_sitter.TSParseActionEntry ParseActions
        {
            get
            {
                var __result0 = ((__Internal*)__Instance)->parse_actions != IntPtr.Zero ? global::tree_sitter.TSParseActionEntry.__CreateInstance(((__Internal*)__Instance)->parse_actions) : default;
                return __result0;
            }
        }

        public sbyte** SymbolNames
        {
            get
            {
                return (sbyte**)((__Internal*)__Instance)->symbol_names;
            }
        }

        public sbyte** FieldNames
        {
            get
            {
                return (sbyte**)((__Internal*)__Instance)->field_names;
            }
        }

        public global::tree_sitter.TSFieldMapSlice FieldMapSlices
        {
            get
            {
                var __result0 = global::tree_sitter.TSFieldMapSlice.__GetOrCreateInstance(((__Internal*)__Instance)->field_map_slices, false);
                return __result0;
            }
        }

        public global::tree_sitter.TSFieldMapEntry FieldMapEntries
        {
            get
            {
                var __result0 = global::tree_sitter.TSFieldMapEntry.__GetOrCreateInstance(((__Internal*)__Instance)->field_map_entries, false);
                return __result0;
            }
        }

        public global::tree_sitter.TSSymbolMetadata SymbolMetadata
        {
            get
            {
                var __result0 = global::tree_sitter.TSSymbolMetadata.__GetOrCreateInstance(((__Internal*)__Instance)->symbol_metadata, false);
                return __result0;
            }
        }

        public ushort* PublicSymbolMap
        {
            get
            {
                return (ushort*) ((__Internal*)__Instance)->public_symbol_map;
            }
        }

        public ushort* AliasMap
        {
            get
            {
                return (ushort*) ((__Internal*)__Instance)->alias_map;
            }
        }

        public ushort* AliasSequences
        {
            get
            {
                return (ushort*) ((__Internal*)__Instance)->alias_sequences;
            }
        }

        public global::tree_sitter.TSLexMode LexModes
        {
            get
            {
                var __result0 = global::tree_sitter.TSLexMode.__GetOrCreateInstance(((__Internal*)__Instance)->lex_modes, false);
                return __result0;
            }
        }

        public global::tree_sitter.Delegates.Func_bool___IntPtr_ushort LexFn
        {
            get
            {
                var __ptr0 = ((__Internal*)__Instance)->lex_fn;
                return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Func_bool___IntPtr_ushort) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Func_bool___IntPtr_ushort));
            }

            set
            {
                ((__Internal*)__Instance)->lex_fn = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            }
        }

        public global::tree_sitter.Delegates.Func_bool___IntPtr_ushort KeywordLexFn
        {
            get
            {
                var __ptr0 = ((__Internal*)__Instance)->keyword_lex_fn;
                return __ptr0 == IntPtr.Zero? null : (global::tree_sitter.Delegates.Func_bool___IntPtr_ushort) Marshal.GetDelegateForFunctionPointer(__ptr0, typeof(global::tree_sitter.Delegates.Func_bool___IntPtr_ushort));
            }

            set
            {
                ((__Internal*)__Instance)->keyword_lex_fn = value == null ? global::System.IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(value);
            }
        }

        public ushort KeywordCaptureToken
        {
            get
            {
                return ((__Internal*)__Instance)->keyword_capture_token;
            }

            set
            {
                ((__Internal*)__Instance)->keyword_capture_token = value;
            }
        }

        public global::tree_sitter.TSLanguage.ExternalScanner external_scanner
        {
            get
            {
                return global::tree_sitter.TSLanguage.ExternalScanner.__CreateInstance(new __IntPtr(&((__Internal*)__Instance)->external_scanner));
            }

            set
            {
                if (ReferenceEquals(value, null))
                    throw new global::System.ArgumentNullException("value", "Cannot be null because it is passed by value.");
                ((__Internal*)__Instance)->external_scanner = *(global::tree_sitter.TSLanguage.ExternalScanner.__Internal*) value.__Instance;
            }
        }
    }

    public unsafe partial class squirrel
    {
        public partial struct __Internal
        {
            [SuppressUnmanagedCodeSecurity, DllImport("tree-sitter", EntryPoint = "tree_sitter_squirrel", CallingConvention = __CallingConvention.Cdecl)]
            internal static extern __IntPtr TreeSitterSquirrel();
        }

        public static global::tree_sitter.TSLanguage TreeSitterSquirrel()
        {
            var __ret = __Internal.TreeSitterSquirrel();
            var __result0 = global::tree_sitter.TSLanguage.__GetOrCreateInstance(__ret, false);
            return __result0;
        }
    }

    namespace Delegates
    {
        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate __IntPtr Func___IntPtr___IntPtr_uint_tree_sitter_TSPoint___Internal_uintPtr(__IntPtr payload, uint byte_index, global::tree_sitter.TSPoint.__Internal position, uint* bytes_read);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate void Action___IntPtr_tree_sitter_TSLogType_string8(__IntPtr payload, global::tree_sitter.TSLogType __0, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CppSharp.Runtime.UTF8Marshaller))] string __1);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate __IntPtr Func___IntPtr_uint(uint __0);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate __IntPtr Func___IntPtr_uint_uint(uint __0, uint __1);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate __IntPtr Func___IntPtr___IntPtr_uint(__IntPtr __0, uint __1);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate void Action___IntPtr(__IntPtr __0);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate void Action___IntPtr_bool(__IntPtr __0, [MarshalAs(UnmanagedType.I1)] bool __1);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate uint Func_uint___IntPtr(__IntPtr __0);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public unsafe delegate bool Func_bool___IntPtr(__IntPtr __0);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate __IntPtr Func___IntPtr();

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public unsafe delegate bool Func_bool___IntPtr___IntPtr_boolPtr(__IntPtr __0, __IntPtr __1, bool* symbol_whitelist);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate uint Func_uint___IntPtr_sbytePtr(__IntPtr __0, sbyte* __1);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        public unsafe delegate void Action___IntPtr_string8_uint(__IntPtr __0, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CppSharp.Runtime.UTF8Marshaller))] string __1, uint __2);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(__CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public unsafe delegate bool Func_bool___IntPtr_ushort(__IntPtr __0, ushort __1);
    }
}
