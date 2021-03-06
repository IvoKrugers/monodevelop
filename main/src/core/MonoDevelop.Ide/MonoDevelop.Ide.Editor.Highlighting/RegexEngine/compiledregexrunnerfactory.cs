//------------------------------------------------------------------------------
// <copyright file="CompiledRegexRunnerFactory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

using System.Diagnostics;
using System.Security.Permissions;
using System;

#if !SILVERLIGHT && !FULL_AOT_RUNTIME
using System.Reflection.Emit;

namespace MonoDevelop.Ide.Editor.Highlighting.RegexEngine {

    [Obsolete ("Old editor")]
    internal sealed class CompiledRegexRunnerFactory : RegexRunnerFactory {
        DynamicMethod goMethod;
        DynamicMethod findFirstCharMethod;
        DynamicMethod initTrackCountMethod;

        internal CompiledRegexRunnerFactory (DynamicMethod go, DynamicMethod firstChar, DynamicMethod trackCount) {
            this.goMethod = go;
            this.findFirstCharMethod = firstChar;
            this.initTrackCountMethod = trackCount;
            //Debug.Assert(goMethod != null && findFirstCharMethod != null && initTrackCountMethod != null, "can't be null");
        }
        
        protected internal override RegexRunner CreateInstance() {
            CompiledRegexRunner runner = new CompiledRegexRunner();

#if !DISABLE_CAS_USE
            new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
            runner.SetDelegates((NoParamDelegate)       goMethod.CreateDelegate(typeof(NoParamDelegate)),
                                (FindFirstCharDelegate) findFirstCharMethod.CreateDelegate(typeof(FindFirstCharDelegate)),
                                (NoParamDelegate)       initTrackCountMethod.CreateDelegate(typeof(NoParamDelegate)));

            return runner;
        }
    }

    [Obsolete ("Old editor")]
    internal delegate RegexRunner CreateInstanceDelegate ();
}

#endif

