// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicInterop.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DspSharp.DynamicInterop
{
    /// <summary>
    ///     Helper class to dynamically generate the interop assembly.
    /// </summary>
    internal class DynamicInterop
    {
        public static void Generate()
        {
            const string name = "DspSharp.Interop";
            var aname = new AssemblyName(name) {Version = new Version(1, 0, 0, 0)};

            var asmBldr = AppDomain.CurrentDomain.DefineDynamicAssembly(aname, AssemblyBuilderAccess.Save);
            var modBldr = asmBldr.DefineDynamicModule(name + ".dll", name + ".dll");

            // Create class Filter.Interop
            var tb = modBldr.DefineType(
                name,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);

            CreateMemcpy(tb);
            CreateMemset(tb);

            tb.CreateType();

            var fileName = name + ".dll";
            asmBldr.Save(fileName);
        }

        /// <summary>
        ///     Generates a memcpy-like function based on the cpblk IL instruction.
        /// </summary>
        /// <param name="tb">The typebuilder.</param>
        private static void CreateMemcpy(TypeBuilder tb)
        {
            var mb = tb.DefineMethod(
                "memcpy",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                CallingConventions.Standard);

            mb.SetReturnType(typeof(void));
            mb.SetParameters(typeof(void*), typeof(void*), typeof(int));

            mb.DefineParameter(1, ParameterAttributes.None, "pDest");
            mb.DefineParameter(2, ParameterAttributes.None, "pSrc");
            mb.DefineParameter(3, ParameterAttributes.None, "count");

            var gen = mb.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Unaligned, (byte)1);
            gen.Emit(OpCodes.Cpblk);
            gen.Emit(OpCodes.Ret);
        }

        /// <summary>
        ///     Generates a memset-like function based on the initblk IL instruction.
        /// </summary>
        /// <param name="tb">The typebuilder.</param>
        private static void CreateMemset(TypeBuilder tb)
        {
            var mb = tb.DefineMethod(
                "memset",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                CallingConventions.Standard);

            mb.SetReturnType(typeof(void));
            mb.SetParameters(typeof(void*), typeof(byte), typeof(int));

            mb.DefineParameter(1, ParameterAttributes.None, "pDest");
            mb.DefineParameter(2, ParameterAttributes.None, "value");
            mb.DefineParameter(3, ParameterAttributes.None, "count");

            var gen = mb.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Unaligned, (byte)1);
            gen.Emit(OpCodes.Initblk);
            gen.Emit(OpCodes.Ret);
        }
    }
}