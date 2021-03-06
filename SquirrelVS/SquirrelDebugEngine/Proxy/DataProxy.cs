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
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy {

    /// <summary>
    ///  Represents a proxy for a typed memory location in a process being debugged. 
    /// </summary>
    internal interface IDataProxy : IValueStore {
        DkmProcess Process { get; }
        ulong Address { get; }
        long ObjectSize { get; }
    }

    internal interface IWritableDataProxy : IDataProxy {
        void Write(object value);
    }

    /// <summary>
    ///  Represents a proxy for a typed memory location in a process being debugged. 
    /// </summary>
    internal interface IDataProxy<T> : IDataProxy, IValueStore<T> {
    }

    internal interface IWritableDataProxy<T> : IDataProxy<T>, IWritableDataProxy {
        /// <summary>
        /// Replace the value in the memory location with the one provided.
        /// </summary>
        void Write(T value);
    }

    /// <summary>
    /// Provides various helper extension and static methods for <see cref="IDataProxy"/>.
    /// </summary>
    internal static class DataProxy {
        private static class FactoryBuilder<TProxy> where TProxy : IDataProxy {
            public delegate TProxy FactoryFunc(DkmProcess process, ulong address);

            public static readonly FactoryFunc Factory;

            static FactoryBuilder() {
                FactoryFunc objectsFactory;
                var ctor = typeof(TProxy).GetConstructor(new[] { typeof(DkmProcess), typeof(ulong) });
                if (ctor != null) {
                    var processParam = Expression.Parameter(typeof(DkmProcess));
                    var addressParam = Expression.Parameter(typeof(ulong));
                    objectsFactory = Expression.Lambda<FactoryFunc>(
                        Expression.New(ctor, processParam, addressParam),
                        new[] { processParam, addressParam })
                        .Compile();
                } else {
                    objectsFactory = (process, address) => {
                        Debug.Fail("IDebuggeeReference-derived type " + typeof(TProxy).Name + " does not have a (DkmProcess, ulong) constructor.");
                        throw new NotSupportedException();
                    };
                }

                if (typeof(ISQObject).IsAssignableFrom(typeof(TProxy))) {
                    Factory = (process, address) => {
                        return objectsFactory(process, address);
                    };
                } else {
                    Factory = objectsFactory;
                }
            }
        }

        /// <summary>
        /// Create a new proxy of a given type. This method exists to facilitate generic programming, as a workaround for the lack
        /// of parametrized constructor constraint in CLR generics.
        /// </summary>
        public static TProxy Create<TProxy>(DkmProcess process, ulong address)
            where TProxy : IDataProxy {
            return FactoryBuilder<TProxy>.Factory(process, address);
        }

        /// <summary>
        /// Returns a proxy for an object that is shifted by <paramref name="elementOffset"/> elements (not bytes!) relative to the object represeted
        /// by the current proxy.
        /// </summary>
        /// <remarks>
        /// This is the equivalent of operator+ on pointers in C. Negative values are permitted.
        /// </remarks>
        /// <param name="elementOffset">Number of elements to shift by.</param>
        /// <returns></returns>
        public static TProxy GetAdjacentProxy<TProxy>(this TProxy r, long elementOffset)
            where TProxy : IDataProxy {
            return Create<TProxy>(r.Process, r.Address.OffsetBy(elementOffset * r.ObjectSize));
        }
    }
}
