using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicInterop
{
    /// <summary>
    ///     Helper class to dynamically generate the interop assembly.
    /// </summary>
    internal class DynamicInterop
    {
        /// <summary>
        ///     Generate SharpDX.Interop assembly based on signatures.
        /// </summary>
        public static void Generate()
        {
            string name = "Filter.Interop";

            AssemblyName aname = new AssemblyName(name);
            aname.Version = new Version(1, 0, 0, 0);

            // Create Assembly and Module
            AssemblyBuilder asmBldr = AppDomain.CurrentDomain.DefineDynamicAssembly(aname, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder modBldr = asmBldr.DefineDynamicModule(name + ".dll", name + ".dll");

            // Create class SharpDX.Interop
            TypeBuilder tb = modBldr.DefineType(name, TypeAttributes.Public | TypeAttributes.Class);

            // Inherit from System.Object
            Type objType = typeof (object);
            ConstructorInfo objCtor = objType.GetConstructor(new Type[0]);

            // Default constructor private
            Type[] ctorParams = {};
            ConstructorBuilder pointCtor = tb.DefineConstructor(
                MethodAttributes.Private,
                CallingConventions.Standard,
                ctorParams);
            ILGenerator ctorIl = pointCtor.GetILGenerator();
            ctorIl.Emit(OpCodes.Ldarg_0);
            Debug.Assert(objCtor != null, "objCtor != null");
            ctorIl.Emit(OpCodes.Call, objCtor);
            ctorIl.Emit(OpCodes.Ret);

            // Create generic CopyMethod Param

            CreateMemcpy(tb);
            CreateMemset(tb);

            tb.CreateType();

            string fileName = name + ".dll";
            asmBldr.Save(fileName);
        }

        private static void CreateMemcpy(TypeBuilder tb)
        {
            MethodBuilder methodCopyStruct = tb.DefineMethod(
                "memcpy",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                CallingConventions.Standard);

            methodCopyStruct.SetReturnType(typeof (void));
            methodCopyStruct.SetParameters(typeof (void*), typeof (void*), typeof (int));

            methodCopyStruct.DefineParameter(1, ParameterAttributes.None, "pDest");
            methodCopyStruct.DefineParameter(2, ParameterAttributes.None, "pSrc");
            methodCopyStruct.DefineParameter(3, ParameterAttributes.None, "count");

            ILGenerator gen = methodCopyStruct.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Unaligned, (byte)1);
            gen.Emit(OpCodes.Cpblk);
            gen.Emit(OpCodes.Ret);
        }

        private static void CreateMemset(TypeBuilder tb)
        {
            MethodBuilder methodCopyStruct = tb.DefineMethod(
                "memset",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                CallingConventions.Standard);

            methodCopyStruct.SetReturnType(typeof (void));
            methodCopyStruct.SetParameters(typeof (void*), typeof (byte), typeof (int));

            methodCopyStruct.DefineParameter(1, ParameterAttributes.None, "pDest");
            methodCopyStruct.DefineParameter(2, ParameterAttributes.None, "value");
            methodCopyStruct.DefineParameter(3, ParameterAttributes.None, "count");

            ILGenerator gen = methodCopyStruct.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Unaligned, (byte)1);
            gen.Emit(OpCodes.Initblk);
            gen.Emit(OpCodes.Ret);
        }
    }
}