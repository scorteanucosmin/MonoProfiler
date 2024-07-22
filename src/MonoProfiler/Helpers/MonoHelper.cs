using System;
using System.Runtime.InteropServices;
using MonoProfiler.Common;
using MonoProfiler.Enums;
using MonoProfiler.Handlers;
using MonoProfiler.Models.Mono;

namespace MonoProfiler.Helpers;

public unsafe class MonoHelper : Singleton<MonoHelper>
{
    public string GetMethodFullName(MonoMethod* method)
    {
        if (method->klass == null || method->name == null)
        {
            return "UNKNOWN";
        }
        
        char* namePtr = Interop.GetFullName(&method->klass->_byval_arg, MonoTypeNameFormat.FullName);
        if (namePtr == null)
        {
            return "UNKNOWN";
        }
        
        string? className = Marshal.PtrToStringAnsi((IntPtr)namePtr);
        if (string.IsNullOrEmpty(className))
        {
            return "UNKNOWN";
        }

        string? methodName = Marshal.PtrToStringAnsi((IntPtr)method->name);
        if (string.IsNullOrEmpty(methodName))
        {
            return "UNKNOWN";
        }

        Interop.MonoFree(namePtr);
        //todo: optimize
        return $"{className}::{methodName}";
    }

    public string GetClassName(MonoClass* monoClass)
    {
        if (monoClass == null)
        {
            return "UNKNOWN";
        }
        
        char* namePtr = Interop.GetFullName(&monoClass->_byval_arg, MonoTypeNameFormat.FullName);
        if (namePtr == null)
        {
            return "UNKNOWN";
        }
        
        string? className = Marshal.PtrToStringAnsi((IntPtr)namePtr);
        if (string.IsNullOrEmpty(className))
        {
            return "UNKNOWN";
        }

        Interop.MonoFree(namePtr);
        return className;
    }
    
    public int GetSizeOfMonoObject(MonoObject* monoObject)
    {
        MonoClass* monoClass = monoObject->vtable->klass;
        IntPtr monoObjectPtr = (IntPtr)monoObject;
        
        switch (monoClass->class_kind)
        {
            case MonoTypeKind.Def:
            case MonoTypeKind.Gtd:
            case MonoTypeKind.Ginst:
            {
                if (monoClass != Constants.MonoStringClass)
                {
                    return monoClass->instance_size;
                }
                    
                MonoString* monoString = (MonoString*)monoObjectPtr;
                return monoClass->instance_size + monoString->length * 2;
            }
            case MonoTypeKind.Array:
            {
                MonoArray* monoArray = (MonoArray*)monoObjectPtr;
                return Convert.ToInt32(monoClass->instance_size + monoArray->length * monoClass->sizes.element_size);
            }
            default:
            {
                return 0;
            }
        }
    }

    public bool CanProfileMonoMethod(MonoMethod* monoMethod)
    {
        IntPtr monoMethodPtr = (IntPtr)monoMethod;
        MethodCacheHandler methodCacheHandler = MethodCacheHandler.Instance;
        if (methodCacheHandler.TryGet(monoMethodPtr, out bool cachedResult))
        {
            return cachedResult;
        }

        string? assemblyName = Marshal.PtrToStringAnsi((IntPtr)monoMethod->klass->image->assembly_name);
        bool result = !string.IsNullOrEmpty(assemblyName) && Profiler.Instance.IsAssemblyRegistered(assemblyName);
        if (!result)
        {
            //TODO: re-visit and optimize, look into using GetClassName instead that returns Namespace.ClassName &
            //TODO: add nested classes handler to properly get plugin name
            string monoMethodFullName = GetMethodFullName(monoMethod);
            if (monoMethodFullName.StartsWith(Constants.OxidePluginNamespace))
            {
                string pluginName = ExtractValueFromMethod(Constants.OxidePluginNamespace, monoMethodFullName);
                result = !string.IsNullOrEmpty(pluginName) && Profiler.Instance.IsAssemblyRegistered(pluginName);
            }
        }

        methodCacheHandler.Set(monoMethodPtr, result);
        return result;
    }
    
    public string ExtractValueFromMethod(string separator, string source)
    {
        ReadOnlySpan<char> sourceSpan = source.AsSpan();
        sourceSpan = sourceSpan[separator.Length..];
        int separatorIndex = sourceSpan.IndexOf("::");
        if (separatorIndex <= -1)
        {
            return string.Empty;
        }

        ReadOnlySpan<char> valueSpan = sourceSpan[..separatorIndex];
        return valueSpan.ToString();
    }
    
    public string ExtractValue(string source)
    {
        int lastIndex = source.LastIndexOf('.');
        if (lastIndex == -1)
        {
            return string.Empty;
        }

        ReadOnlySpan<char> valueSpan = source.AsSpan(lastIndex + 1);
        return valueSpan.ToString();
    }
}