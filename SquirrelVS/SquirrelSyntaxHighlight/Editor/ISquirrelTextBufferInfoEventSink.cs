// Python Tools for Visual Studio
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace SquirrelSyntaxHighlight.Editor {
    /// <summary>
    /// Interface for classes that need to receive events from text buffers.
    /// </summary>
    /// <remarks>
    /// This is an interface rather than an abstract class because there are
    /// IntelliSense features that require using another base class.
    /// Implement no-op handlers by returning <see cref="Task.CompletedTask"/>.
    /// </remarks>
    internal interface ISquirrelTextBufferInfoEventSink {
        Task SquirrelTextBufferEventAsync(
            SquirrelTextBufferInfo          _Sender, 
            SquirrelTextBufferInfoEventArgs _Args
          );
    }

    internal enum SquirrelTextBufferInfoEvents {
        None = 0,
        NewAnalysisEntry,
        NewParseTree,
        NewAnalysis,
        TextContentChanged,
        TextContentChangedLowPriority,
        ContentTypeChanged,
        DocumentEncodingChanged,
        NewTextBufferInfo,
        TextContentChangedOnBackgroundThread,
        AnalyzerExpired,
        ParseTreeChanged,
    }

    internal class SquirrelTextBufferInfoEventArgs : EventArgs {
        public SquirrelTextBufferInfoEventArgs(
            SquirrelTextBufferInfoEvents _EventType
          ) 
        {
          Event = _EventType;
        }

        public SquirrelTextBufferInfoEvents Event { get; }
    }

    internal class SquirrelTextBufferInfoNestedEventArgs : SquirrelTextBufferInfoEventArgs {
        public SquirrelTextBufferInfoNestedEventArgs(
            SquirrelTextBufferInfoEvents _EventType, 
            EventArgs                    _Args
          ) : base(_EventType) 
        {
          NestedEventArgs = _Args;
        }

        public EventArgs NestedEventArgs { get; }
    }

    internal class SquirrelNewTextBufferInfoEventArgs : SquirrelTextBufferInfoEventArgs {
        public SquirrelNewTextBufferInfoEventArgs(
            SquirrelTextBufferInfoEvents _EventType, 
            SquirrelTextBufferInfo       _NewInfo
          ) : base(_EventType) 
        {
          NewTextBufferInfo = _NewInfo;
        }

        public SquirrelTextBufferInfo NewTextBufferInfo { get; }
    }

  internal class SquirrelTreeChanged : SquirrelTextBufferInfoEventArgs
  {
    public SquirrelTreeChanged(
        SquirrelTextBufferInfoEvents _EventType,
        List<SnapshotSpan>           _Spans
      ) : base(_EventType)
    {
      ChangedSpans = _Spans;
    }

    public List<SnapshotSpan> ChangedSpans { get; }
  }
}
