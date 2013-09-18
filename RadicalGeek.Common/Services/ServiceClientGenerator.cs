using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using RadicalGeek.Common.Collections;
using Microsoft.CSharp;

namespace RadicalGeek.Common.Services
{
    internal static class ServiceClientGenerator
    {
        private static readonly Dictionary<string, Type> generatedClientsDictionary = new Dictionary<string, Type>();

        private const string ClassPrefix = "Client";

        internal static Type GetClient<TInterface, TServiceClient>()
            where TInterface : class
            where TServiceClient : GenericServiceClientBaseClass<TInterface>
        {
            string serviceClientType = typeof(TServiceClient).Name;
            serviceClientType = serviceClientType.Substring(0, serviceClientType.IndexOf("`"));
            string dictionaryKey = string.Format("{0}.{1}", typeof(TInterface).FullName, serviceClientType);
            if (!generatedClientsDictionary.ContainsKey(dictionaryKey))
            {
                string interfaceName = typeof(TInterface).Name;
                string className = string.Format("{0}_{1}_{2}", ClassPrefix, serviceClientType, interfaceName.Substring(1));
                string code = GenerateCode<TInterface, TServiceClient>(className);
                Type result = Compile(interfaceName, className, code);
                generatedClientsDictionary.Add(dictionaryKey, result);
            }
            return generatedClientsDictionary[dictionaryKey];
        }

        private static Type Compile(string interfaceName, string className, string code)
        {
            CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
            CompilerParameters compilerParameters = new CompilerParameters
                {
                    OutputAssembly = Path.Combine(GeneratedAssembliesFolder, string.Format("{0}.{1}.dll", interfaceName, className))
                };
            string executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (executingDirectory != null)
            {
                string[] dllFiles = Directory.GetFiles(executingDirectory, "*.dll");
                compilerParameters.ReferencedAssemblies.AddRange(dllFiles.Select(f => Path.Combine(executingDirectory, f)).ToArray());
                IEnumerable<string> exeFiles = Directory.GetFiles(executingDirectory, "*.exe").Where(f => !f.Contains(".vshost."));
                compilerParameters.ReferencedAssemblies.AddRange(exeFiles.Select(f => Path.Combine(executingDirectory, f)).ToArray());
            }
            CompilerResults results = cSharpCodeProvider.CompileAssemblyFromSource(compilerParameters, new[] { code });

            if (results.Errors.Count > 0)
            {
                Exception exception = new Exception("Compile error.");
                int n = 0;
                foreach (CompilerError error in results.Errors)
                    exception.Data.Add(++n, error);
                throw exception;
            }

            Type result = results.CompiledAssembly.GetType(className);

            return result;
        }

        private static string generatedAssembliesFolder;

        static readonly string[] candidateAssembliesFolders = new[]
                    {
                        Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Process),
                        Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.Process),
                        Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.User),
                        Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.User),
                        Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine),
                        Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.Machine),
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    };

        private static string GeneratedAssembliesFolder
        {
            get
            {
                return generatedAssembliesFolder ??
                       (generatedAssembliesFolder = candidateAssembliesFolders.First(FolderIsWritable));
            }
        }

        private static bool FolderIsWritable(string folder)
        {
            try
            {
                if (!Directory.Exists(folder)) return false;
                string testFileName = Path.Combine(folder, Path.GetRandomFileName());
                File.WriteAllBytes(testFileName, new byte[0]);
                File.Delete(testFileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static readonly Dictionary<Type, string> typeNames = new Dictionary<Type, string>
            {
                    {typeof(void),"void"}
            };

        private static string GenerateCode<TInterface, TServiceClient>(string className)
            where TInterface : class
            where TServiceClient : GenericServiceClientBaseClass<TInterface>
        {
            Type interfaceType = typeof(TInterface);
            Type serviceClient = typeof(TServiceClient);
            StringBuilder classBuilder = new StringBuilder();

            string genericServiceClientClassName = serviceClient.FullName;
            genericServiceClientClassName = genericServiceClientClassName.Substring(0, genericServiceClientClassName.IndexOf('`'));
            string interfaceFullName = interfaceType.FullName;
            classBuilder.AppendFormat(ClassTemplate, interfaceFullName, className, genericServiceClientClassName, BuildConstructors(className, serviceClient), BuildMethods(interfaceType));

            typeNames.ForEach(kvp => classBuilder.Replace(kvp.Key.FullName, kvp.Value));

            return classBuilder.ToString();
        }

        private static StringBuilder BuildMethods(Type interfaceType)
        {
            StringBuilder methodBuilder = new StringBuilder();
            IEnumerable<MethodInfo> methodInfos =
                    interfaceType.GetMethods().Where(
                        m => m.GetCustomAttributes(typeof(OperationContractAttribute), false).Length > 0);
            foreach (MethodInfo method in methodInfos)
            {
                ParameterInfo[] parameterInfos = method.GetParameters();
                methodBuilder.AppendFormat(MethodTemplate, method.ReturnType.FullName, method.Name,
                                           CreateParameterList(parameterInfos),
                                           method.ReturnType != typeof(void) ? "return " : "", method.Name,
                                           CreateArgumentList(parameterInfos));
            }
            return methodBuilder;
        }

        private const string MethodTemplate = "public {0} {1}({2}){{{3}Service.{4}({5});}}";
        private const string ClassTemplate = "public sealed class {1}:{2}<{0}>,{0}{{{3}{4}}}";
        private const string ConstructorTemplate = "public {0}({1}):base({2}){{}}";

        private static StringBuilder BuildConstructors(string className, Type serviceClient)
        {
            StringBuilder constructorBuilder = new StringBuilder();
            ConstructorInfo[] constructorInfos = serviceClient.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (ConstructorInfo constructorInfo in constructorInfos)
            {
                ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
                constructorBuilder.AppendFormat(ConstructorTemplate, className, CreateParameterList(parameterInfos), CreateArgumentList(parameterInfos));
            }
            return constructorBuilder;
        }

        private static string CreateArgumentList(IEnumerable<ParameterInfo> parameterInfos)
        {
            int i = 0;
            return string.Join(",", parameterInfos.Select(p => string.Format("p{0}", i++)));
        }

        private static string CreateParameterList(IEnumerable<ParameterInfo> parameterInfos)
        {
            int i = 0;
            return string.Join(",", parameterInfos.Select(p => string.Format("{0} p{1}", p.ParameterType.FullName, i++)));
        }
    }
}
