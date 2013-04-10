using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Generator
{
    public class CodeGenerator
    {
        public CodeGenerator()
        {
            
        }

        public void Generate()
        {
            var aName = new AssemblyName("MyApp");
            var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".exe");

            MethodBuilder mainBuilder = mb.DefineGlobalMethod("Main", MethodAttributes.Static|MethodAttributes.Public, typeof (void), new Type[0]);

            var mainIL = mainBuilder.GetILGenerator();
            mainIL.Emit(OpCodes.Ldstr, "Hello, world!");
            mainIL.Emit(OpCodes.Call, typeof(System.Console).GetMethod("WriteLine", new Type[]{typeof(string)}));
            mainIL.Emit(OpCodes.Call, typeof(System.Console).GetMethod("ReadKey", new Type[0]));
            mainIL.Emit(OpCodes.Pop);
            mainIL.Emit(OpCodes.Ret);

            mb.CreateGlobalFunctions();

            ab.SetEntryPoint(mainBuilder.GetBaseDefinition());
            ab.Save(aName.Name + ".exe");
        }
    }
}
