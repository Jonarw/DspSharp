using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicInterop
{
    /// <summary>
    ///     Helper class to dynamically generate the interop assembly. Based on an older implementation of SharpDX.
    /// </summary>
    internal class DynamicInterop
    {
        public static void Generate()
        {
            string name = "Filter.Interop";

            AssemblyName aname = new AssemblyName(name);
            aname.Version = new Version(1, 0, 0, 0);

            // Create Assembly and Module
            AssemblyBuilder asmBldr = AppDomain.CurrentDomain.DefineDynamicAssembly(aname, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder modBldr = asmBldr.DefineDynamicModule(name + ".dll", name + ".dll");

            // Create class Filter.Interop
            TypeBuilder tb = modBldr.DefineType(
                name,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);

            CreateMemcpy(tb);
            CreateMemset(tb);

            tb.CreateType();

            string fileName = name + ".dll";
            asmBldr.Save(fileName);
        }

        /// <summary>
        ///     Generates a memcpy-like function based on the cpblk IL instruction.
        /// </summary>
        /// <param name="tb">The typebuilder.</param>
        private static void CreateMemcpy(TypeBuilder tb)
        {
            MethodBuilder mb = tb.DefineMethod(
                "memcpy",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                CallingConventions.Standard);

            mb.SetReturnType(typeof(void));
            mb.SetParameters(typeof(void*), typeof(void*), typeof(int));

            mb.DefineParameter(1, ParameterAttributes.None, "pDest");
            mb.DefineParameter(2, ParameterAttributes.None, "pSrc");
            mb.DefineParameter(3, ParameterAttributes.None, "count");

            ILGenerator gen = mb.GetILGenerator();

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
            MethodBuilder mb = tb.DefineMethod(
                "memset",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                CallingConventions.Standard);

            mb.SetReturnType(typeof(void));
            mb.SetParameters(typeof(void*), typeof(byte), typeof(int));

            mb.DefineParameter(1, ParameterAttributes.None, "pDest");
            mb.DefineParameter(2, ParameterAttributes.None, "value");
            mb.DefineParameter(3, ParameterAttributes.None, "count");

            ILGenerator gen = mb.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Unaligned, (byte)1);
            gen.Emit(OpCodes.Initblk);
            gen.Emit(OpCodes.Ret);
        }
    }
}