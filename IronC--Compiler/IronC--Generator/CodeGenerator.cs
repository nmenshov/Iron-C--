using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;
using IronC__Common.Syntax;
using IronC__Common.Syntax.Expressions;
using IronC__Common.Trees;

namespace IronC__Generator
{
    public class CodeGenerator
    {
        public const string ProgramName = "Program";
        public const string MainFuncName = "main";

        private readonly ITree _tree;

        public CodeGenerator(ITree tree)
        {
            _tree = tree;
        }

        public bool Generate(string fileName)
        {
            var aName = new AssemblyName(Path.GetFileNameWithoutExtension(fileName));
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(aName.Name, fileName);
            var typeBuilder = moduleBuilder.DefineType(ProgramName);

            var globals = DefineGlobalVariables(typeBuilder);
            InitGlobalsInStaticCtor(globals, typeBuilder);

            var funcs = DefineFunctions(typeBuilder);
            foreach (var function in funcs)
                CreateFunction(function, globals, funcs);

            typeBuilder.CreateType();

            assemblyBuilder.SetEntryPoint(funcs.Single(x => x.Name == MainFuncName).Info);
            assemblyBuilder.Save(fileName);
            return true;
        }

        private void CreateFunction(Function func, List<GlobalVariable> globals, List<Function> allFuncs)
        {
            var il = func.ILGenerator;
            var locals = DeclareAllLocalVariables(func.Body, il);
            var breaks = new Stack<Label>();
            var tmpVar = il.DeclareLocal(typeof (int));

            CodeBlock(func.Body, locals, globals, func.Params, il, breaks, allFuncs, tmpVar);
        }

        #region Statements

        #region Expressions

        private void CodeBinary(BinaryExpression binary, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            CodeExpression(binary.LeftOperand, locals, globals, parameters, il, allFuncs, tmpVar);
            CodeExpression(binary.RightOperand, locals, globals, parameters, il, allFuncs, tmpVar);
            var op = binary.Operator;

            if (!CodeLogicOp(op, il))
                if (!CodeGreatOp(op, il))
                    if (!CodeLessOp(op, il))
                        CodeMathOp(op, il);
        }

        private bool CodeLogicOp(Terminal op, ILGenerator il)
        {
            if (op == Terminals.And)
            {
                var ret = il.DefineLabel();
                il.Emit(OpCodes.Brtrue_S, ret);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_I4_0);
                il.MarkLabel(ret);
                return true;
            }
            if (op == Terminals.Or)
            {
                var ret2 = il.DefineLabel();
                il.Emit(OpCodes.Brfalse_S, ret2);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_I4_1);
                il.MarkLabel(ret2);
                return true;
            }
            if (op == Terminals.Equal)
            {
                il.Emit(OpCodes.Ceq);
                return true;
            }
            if (op == Terminals.NotEqual)
            {
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
            }
            return false;
        }

        private bool CodeGreatOp(Terminal op, ILGenerator il)
        {
            if (op == Terminals.Great)
            {
                il.Emit(OpCodes.Cgt);
                return true;
            }
            if (op == Terminals.GreatOrEqual)
            {
                il.Emit(OpCodes.Clt);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
                return true;
            }
            return false;
        }

        private bool CodeLessOp(Terminal op, ILGenerator il)
        {
            if (op == Terminals.Less)
            {
                il.Emit(OpCodes.Clt);
                return true;
            }
            if (op == Terminals.LessOrEqual)
            {
                il.Emit(OpCodes.Cgt);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
                return true;
            }
            return false;
        }

        private void CodeMathOp(Terminal op, ILGenerator il)
        {
            if (op == Terminals.Minus)
            {
                il.Emit(OpCodes.Sub);
            }
            else if (op == Terminals.Plus)
            {
                il.Emit(OpCodes.Add);
            }
            else if (op == Terminals.Mul)
            {
                il.Emit(OpCodes.Mul);
            }
            else if (op == Terminals.Div)
            {
                il.Emit(OpCodes.Div);
            }
        }

        private void CodeCall(FuncCallExpression call, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            var func = FindFunc(call, allFuncs);
            foreach (var expression in call.Params)
                CodeExpression(expression, locals, globals, parameters, il, allFuncs, tmpVar);
            il.Emit(OpCodes.Call, func.Info);
        }

        private Function FindFunc(FuncCallExpression call, List<Function> allFuncs)
        {
            var func = allFuncs.Where(x => x.Name == call.Func.Value && x.Params.Length == call.Params.Length);
            /*foreach (var function in allFuncs)
            {
                bool f = true;
                for (int i = 0; i < call.Params.Length; i++)
                {
                    if (call.Params[i])
                }
            }*/
            return func.First();
        }

        private void CodeGetArray(GetArrayValueExpression get, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            var vname = get.Variable.Value;
            Type t;
            if (locals.Any(x => x.Name == vname))
            {
                var loc = locals.First(x => x.Name == vname);
                t = loc.Type;
                il.Emit(OpCodes.Ldloc, loc.Info);
            }
            else if (globals.Any(x => x.Name == vname))
            {
                var glo = globals.First(x => x.Name == vname);
                t = glo.Type;
                il.Emit(OpCodes.Ldsfld, glo.FieldInfo);
            }
            else
            {
                var par = parameters.First(x => x.Name == vname);
                t = par.Type;
                il.Emit(OpCodes.Ldarg_S, (byte)par.Index);
            }

            CodeExpression(get.Index, locals, globals, parameters, il, allFuncs, tmpVar);

            if (t == typeof(byte[]))
                il.Emit(OpCodes.Ldelem_I1);
            else
                il.Emit(OpCodes.Ldelem_I4);
        }

        private void CodeGet(GetValueExpression get, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il)
        {
            var vname = get.Variable.Value;
            if (locals.Any(x => x.Name == vname))
            {
                var loc = locals.First(x => x.Name == vname);
                il.Emit(OpCodes.Ldloc, loc.Info);
            }
            else if (globals.Any(x => x.Name == vname))
            {
                var glo = globals.First(x => x.Name == vname);
                il.Emit(OpCodes.Ldsfld, glo.FieldInfo);
            }
            else
            {
                var par = parameters.First(x => x.Name == vname);
                il.Emit(OpCodes.Ldarg_S, (byte)par.Index);
            }
        }

        private void CodeNumber(NumberExpression number, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il)
        {
            switch (number.Num.Value)
            {
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    il.Emit(OpCodes.Ldc_I4, number.Num.Value);
                    break;
            }
        }

        private void CodeSetArray(SetArrayValueExpression set, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            var vname = set.Variable.Value;
            Type t;
            if (locals.Any(x => x.Name == vname))
            {
                var loc = locals.First(x => x.Name == vname);
                t = loc.Type;
                il.Emit(OpCodes.Ldloc, loc.Info);
            }
            else if (globals.Any(x => x.Name == vname))
            {
                var glo = globals.First(x => x.Name == vname);
                t = glo.Type;
                il.Emit(OpCodes.Ldsfld, glo.FieldInfo);
            }
            else
            {
                var par = parameters.First(x => x.Name == vname);
                t = par.Type;
                il.Emit(OpCodes.Ldarg_S, (byte)par.Index);
            }

            CodeExpression(set.Index, locals, globals, parameters, il, allFuncs, tmpVar);
            CodeExpression(set.Value, locals, globals, parameters, il, allFuncs, tmpVar);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Stloc, tmpVar);

            if (t == typeof (byte[]))
                il.Emit(OpCodes.Stelem_I1);
            else
                il.Emit(OpCodes.Stelem_I4);
            il.Emit(OpCodes.Ldloc, tmpVar);
        }

        private void CodeSet(SetValueExpression set, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            CodeExpression(set.Value, locals, globals, parameters, il, allFuncs, tmpVar);
            il.Emit(OpCodes.Dup);
            var vname = set.Variable.Value;
            if (locals.Any(x => x.Name == vname))
            {
                var loc = locals.First(x => x.Name == vname);
                il.Emit(OpCodes.Stloc, loc.Info);
            }
            else if (globals.Any(x => x.Name == vname))
            {
                var glo = globals.First(x => x.Name == vname);
                il.Emit(OpCodes.Stsfld, glo.FieldInfo);
            }
            else
            {
                var par = parameters.First(x => x.Name == vname);
                il.Emit(OpCodes.Starg_S, (byte)par.Index);
            }
        }

        private void CodeUnary(UnaryExpression unary, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            CodeExpression(unary.Expression, locals, globals, parameters, il, allFuncs, tmpVar);
            if (unary.Operator == Terminals.Minus)
                il.Emit(OpCodes.Neg);
            else
            {
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
            }
        }

        private void CodeExpression(Expression expression, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            if (expression is BinaryExpression)
                CodeBinary(expression as BinaryExpression, locals, globals, parameters, il, allFuncs, tmpVar);
            else if (expression is FuncCallExpression)
                CodeCall(expression as FuncCallExpression, locals, globals, parameters, il, allFuncs, tmpVar);
            else if (expression is GetArrayValueExpression)
                CodeGetArray(expression as GetArrayValueExpression, locals, globals, parameters, il, allFuncs, tmpVar);
            else if (expression is GetValueExpression)
                CodeGet(expression as GetValueExpression, locals, globals, parameters, il);
            else if (expression is NumberExpression)
                CodeNumber(expression as NumberExpression, locals, globals, parameters, il);
            else if (expression is SetArrayValueExpression)
                CodeSetArray(expression as SetArrayValueExpression, locals, globals, parameters, il, allFuncs, tmpVar);
            else if (expression is SetValueExpression)
                CodeSet(expression as SetValueExpression, locals, globals, parameters, il, allFuncs, tmpVar);
            else if (expression is UnaryExpression)
                CodeUnary(expression as UnaryExpression, locals, globals, parameters, il, allFuncs, tmpVar);
            else
                throw new InvalidDataException();
        }

        #endregion

        private void CodeBlock(Block block, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il,
            Stack<Label> breaks, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            foreach (var statement in block.Statements)
                CodeStatement(statement, locals, globals, parameters, il, breaks, allFuncs, tmpVar);
        }

        private void CodeBreak(ILGenerator il, Stack<Label> breaks)
        {
            il.Emit(OpCodes.Br_S, breaks.Peek());
        }

        private void CodeIf(IfStatement ifStatement, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il,
            Stack<Label> breaks, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            var trueJmp = il.DefineLabel();
            CodeExpression(ifStatement.Condition, locals, globals, parameters, il, allFuncs, tmpVar);
            il.Emit(OpCodes.Brtrue_S, trueJmp);
            CodeStatement(ifStatement.FalseStatement, locals, globals, parameters, il, breaks, allFuncs, tmpVar);
            il.MarkLabel(trueJmp);
            CodeStatement(ifStatement.TrueStatement, locals, globals, parameters, il, breaks, allFuncs, tmpVar);
        }

        private void CodeRead(ReadStatement readStatement, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il)
        {
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine", new Type[0]));
            il.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToInt32", new Type[]{typeof(string)}));

            var vname = readStatement.Id.Value;
            if (locals.Any(x => x.Name == vname))
            {
                var loc = locals.First(x => x.Name == vname);
                il.Emit(OpCodes.Stloc, loc.Info);
            }
            else if (globals.Any(x => x.Name == vname))
            {
                var glo = globals.First(x => x.Name == vname);
                il.Emit(OpCodes.Stsfld, glo.FieldInfo);
            }
            else
            {
                var par = parameters.First(x => x.Name == vname);
                il.Emit(OpCodes.Starg_S, (byte)par.Index);
            }
        }

        private void CodeReturn(ReturnStatement returnStatement, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            CodeExpression(returnStatement.Expression, locals, globals, parameters, il, allFuncs, tmpVar);
            il.Emit(OpCodes.Ret);
        }

        private void CodeWhile(WhileStatement whileStatement, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il,
            Stack<Label> breaks, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            var endLabel = il.DefineLabel();
            var startLabel = il.DefineLabel();
            breaks.Push(endLabel);

            il.MarkLabel(startLabel);
            CodeExpression(whileStatement.Condition, locals, globals, parameters, il, allFuncs, tmpVar);
            il.Emit(OpCodes.Brfalse_S, endLabel);
            CodeStatement(whileStatement.Statement, locals, globals, parameters, il, breaks, allFuncs, tmpVar);
            il.Emit(OpCodes.Br_S, startLabel);

            il.MarkLabel(breaks.Pop());
        }

        private void CodeWrite(WriteStatement write, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            CodeExpression(write.Expression, locals, globals, parameters, il, allFuncs, tmpVar);
            il.Emit(OpCodes.Call, typeof (Console).GetMethod("Write", new Type[] {typeof (int)}));
        }

        private void CodeWriteln(ILGenerator il)
        {
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[0]));
        }

        private void CodeStatement(Statement statement, List<LocalVariable> locals, List<GlobalVariable> globals, FuncParam[] parameters, ILGenerator il, 
            Stack<Label> breaks, List<Function> allFuncs, LocalBuilder tmpVar)
        {
            if (statement is Block)
                CodeBlock(statement as Block, locals, globals, parameters, il, breaks, allFuncs, tmpVar);
            else if (statement is BreakStatement)
                CodeBreak(il, breaks);
            else if (statement is Expression)
            {
                CodeExpression(statement as Expression, locals, globals, parameters, il, allFuncs, tmpVar);
                il.Emit(OpCodes.Pop);
            }
            else if (statement is IfStatement)
                CodeIf(statement as IfStatement, locals, globals, parameters, il, breaks, allFuncs, tmpVar);
            else if (statement is ReadStatement)
                CodeRead(statement as ReadStatement, locals, globals, parameters, il);
            else if (statement is ReturnStatement)
                CodeReturn(statement as ReturnStatement, locals, globals, parameters, il, allFuncs, tmpVar);
            else if (statement is WhileStatement)
                CodeWhile(statement as WhileStatement, locals, globals, parameters, il, breaks, allFuncs, tmpVar);
            else if (statement is WriteStatement)
                CodeWrite(statement as WriteStatement, locals, globals, parameters, il, allFuncs, tmpVar);
            else if (statement is WritelnStatement)
                CodeWriteln(il);
            else
                throw new InvalidDataException();
        }

        #endregion

        #region Variables

        private List<LocalVariable> DeclareAllLocalVariables(Block block, ILGenerator il)
        {
            var list = new List<LocalVariable>();
            foreach (var varDeclaration in block.VarDeclarations)
            {
                var name = varDeclaration.Id.Value;
                var type = GetVarType(varDeclaration);
                var info = il.DeclareLocal(type);

                if (varDeclaration is ArrayDeclaration)
                {
                    var array = varDeclaration as ArrayDeclaration;
                    il.Emit(OpCodes.Ldc_I4, array.Size.Value);
                    il.Emit(OpCodes.Newarr, type);
                    il.Emit(OpCodes.Stloc, info);
                }

                list.Add(new LocalVariable(name, info, type));
            }
            foreach (var s in block.Statements)
            {
                if (s is Block)
                    list.AddRange(DeclareAllLocalVariables(s as Block, il));
                else if (s is IfStatement)
                {
                    var f = s as IfStatement;
                    if (f.TrueStatement is Block)
                        list.AddRange(DeclareAllLocalVariables(f.TrueStatement as Block, il));
                    if (f.FalseStatement is Block)
                        list.AddRange(DeclareAllLocalVariables(f.FalseStatement as Block, il));
                }
                else if (s is WhileStatement)
                {
                    var w = s as WhileStatement;
                    if (w.Statement is Block)
                        list.AddRange(DeclareAllLocalVariables(w.Statement as Block, il));
                }
            }
            return list;
        }

        private List<Function> DefineFunctions(TypeBuilder typeBuilder)
        {
            var prog = _tree.Root as Program;
            var list = new List<Function>();

            foreach (var declaration in prog.FuncDeclarations)
            {
                var returnType = GetType(declaration.ReturnType);
                var parameters = ProcessFuncParams(declaration.Params);

                var info = typeBuilder.DefineMethod(declaration.Name.Value, MethodAttributes.Static, returnType, parameters.Select(x => x.Type).ToArray());

                list.Add(new Function(declaration.Name.Value, returnType, parameters, declaration.Body, info));
            }

            return list;
        }

        private FuncParam[] ProcessFuncParams(ParamDeclaration[] paramDeclarations)
        {
            var pars = new FuncParam[paramDeclarations.Length];

            for (int i = 0; i < paramDeclarations.Length; i++)
            {
                var type = GetType(paramDeclarations[i].Type, paramDeclarations[i].IsArray);
                pars[i] = new FuncParam(paramDeclarations[i].Id.Value, type, i);
            }

            return pars;
        }

        private void InitGlobalsInStaticCtor(IEnumerable<GlobalVariable> globalVariables, TypeBuilder typeBuilder)
        {
            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, new Type[0]);
            var il = ctor.GetILGenerator();

            foreach (var variable in globalVariables)
                variable.Init(il);

            il.Emit(OpCodes.Ret);
        }

        private List<GlobalVariable> DefineGlobalVariables(TypeBuilder typeBuilder)
        {
            var prog = _tree.Root as Program;
            var list = new List<GlobalVariable>();

            foreach (var declaration in prog.VarDeclarations)
            {
                var name = declaration.Id.Value;
                var type = GetVarType(declaration);
                var info = typeBuilder.DefineField(name, type, FieldAttributes.Static);

                GlobalVariable g;
                if (declaration is ArrayDeclaration)
                {
                    var ar = declaration as ArrayDeclaration;
                    g = new GlobalArrayVariable(name, type, info, ar.Size.Value);
                }
                else
                    g = new GlobalVariable(name, info, type);

                list.Add(g);
            }
            return list;
        }

        private Type GetVarType(VarDeclaration declaration)
        {
            return GetType(declaration.Type, declaration is ArrayDeclaration);
        }

        private Type GetType(Terminal terminal, bool isArray = false)
        {
            if (terminal == Terminals.Int)
                return isArray ? typeof(int[]) : typeof(int);
            return isArray ? typeof(byte[]) : typeof(byte);
        }

        #endregion

        public bool Generate_test(string fileName)
        {
            var aName = new AssemblyName(Path.GetFileNameWithoutExtension(fileName));
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(aName.Name, fileName);
            var tb = moduleBuilder.DefineType("Program");
            //var mb2 = moduleBuilder.DefineGlobalMethod("A", MethodAttributes.Public | MethodAttributes.Static, typeof (int), new Type[] {typeof (int)});
            var f = tb.DefineField("_a", typeof (int), FieldAttributes.Static);

            var mb2 = tb.DefineMethod("A", MethodAttributes.Static, typeof (int), new Type[] {typeof (int)});

            var il2 = mb2.GetILGenerator();
            var v0 = il2.DeclareLocal(typeof (int));
            il2.Emit(OpCodes.Ldarg_0);
            il2.Emit(OpCodes.Stloc_0);
            il2.Emit(OpCodes.Ldloc_0);
            il2.Emit(OpCodes.Ldc_I4_5);
            var labIter = il2.DefineLabel();
            il2.Emit(OpCodes.Bge_S, labIter);
            il2.Emit(OpCodes.Ldloc_0);
            il2.Emit(OpCodes.Ldc_I4_1);
            il2.Emit(OpCodes.Add);
            il2.Emit(OpCodes.Call, mb2);
            il2.Emit(OpCodes.Ret);
            il2.MarkLabel(labIter);
            il2.Emit(OpCodes.Ldloc_0);
            il2.Emit(OpCodes.Ret);

            //MethodBuilder mainBuilder = moduleBuilder.DefineGlobalMethod("Main", MethodAttributes.Static | MethodAttributes.Public, typeof(void), new Type[0]);
            var mainBuilder = tb.DefineMethod("Main", MethodAttributes.Static, typeof (void), new Type[0]);
            var mainIL = mainBuilder.GetILGenerator();
            mainIL.Emit(OpCodes.Ldc_I4_0);
            mainIL.Emit(OpCodes.Stsfld, f);
            mainIL.Emit(OpCodes.Ldsfld, f);
            mainIL.Emit(OpCodes.Call, mb2);
            mainIL.Emit(OpCodes.Call, typeof(System.Console).GetMethod("WriteLine", new Type[] { typeof(int) }));
            mainIL.Emit(OpCodes.Call, typeof(System.Console).GetMethod("ReadKey", new Type[0]));
            mainIL.Emit(OpCodes.Pop);
            mainIL.Emit(OpCodes.Ret);

            //moduleBuilder.CreateGlobalFunctions();
            tb.CreateType();

            assemblyBuilder.SetEntryPoint(mainBuilder);
            assemblyBuilder.Save(fileName);
            return true;
        }
    }
}
