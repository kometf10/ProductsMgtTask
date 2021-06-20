using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace OA.Domin.Reflection
{
    public static class DynamicModuleLambdaCompiler
    {
        public static Func<T> GenerateFactory<T>() where T : new()
        {
            Expression<Func<T>> expr = () => new T();
            NewExpression newExpr = (NewExpression)expr.Body;

            var method = new DynamicMethod(
                name: "lambda",
                returnType: newExpr.Type,
                parameterTypes: new Type[0],
                m: typeof(DynamicModuleLambdaCompiler).Module,
                skipVisibility: true
                );

            ILGenerator ilGen = method.GetILGenerator();
            if(newExpr.Constructor != null)
            {
                ilGen.Emit(OpCodes.Newobj, newExpr.Constructor);
            }
            else
            {
                LocalBuilder temp = ilGen.DeclareLocal(newExpr.Type);
                ilGen.Emit(OpCodes.Ldloca, temp);
                ilGen.Emit(OpCodes.Initobj, newExpr.Type);
                ilGen.Emit(OpCodes.Ldloc, temp);
            }

            ilGen.Emit(OpCodes.Ret);

            return (Func<T>)method.CreateDelegate(typeof(Func<T>));

        }

        public delegate object PropertyGetDelegate(object obj);
        public static PropertyGetDelegate GetPropertyGetter(string typeName, string propertyName)
        {
            Type t = Type.GetType(typeName);
            PropertyInfo pi = t.GetProperty(propertyName);
            MethodInfo getter = pi.GetGetMethod();

            DynamicMethod dm = new DynamicMethod("GetValue", typeof(object), new Type[] { typeof(object) }, typeof(object), true);
            ILGenerator ilGen = dm.GetILGenerator();

            //emit CIL
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Call, getter);

            if (getter.ReturnType.GetTypeInfo().IsValueType)
                ilGen.Emit(OpCodes.Box, getter.ReturnType);

            ilGen.Emit(OpCodes.Ret);

            return dm.CreateDelegate(typeof(PropertyGetDelegate)) as PropertyGetDelegate;

        }

        public delegate void PropertySetDelegate(object obj, object value);
        public static PropertySetDelegate GetPropertySetter(string typeName, string propName)
        {
            Type t = Type.GetType(typeName);
            PropertyInfo pi = t.GetProperty(propName);
            MethodInfo setter = pi.GetSetMethod();

            DynamicMethod dm = new DynamicMethod("SetValue", typeof(void), new Type[] { typeof(object) }, typeof(object), true);
            ILGenerator ilGen = dm.GetILGenerator();

            //emit CIL
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldarg_1);

            Type parameterType = setter.GetParameters()[0].ParameterType;

            if (parameterType.GetTypeInfo().IsValueType)
                ilGen.Emit(OpCodes.Unbox_Any, parameterType);

            ilGen.Emit(OpCodes.Call, setter);
            ilGen.Emit(OpCodes.Ret);

            return dm.CreateDelegate(typeof(PropertySetDelegate)) as PropertySetDelegate;
        }

    }
}
