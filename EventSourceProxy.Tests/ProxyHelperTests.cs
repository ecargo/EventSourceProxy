using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;

namespace EventSourceProxy.Tests
{
    public class ProxyHelperTests
    {
        static TypeBuilder CreateTypeBuilder()
        {
            var assemblyName = new AssemblyName("TestAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            return moduleBuilder.DefineType("TestClass", TypeAttributes.Class | TypeAttributes.Public);
        }

        static object InvokeGeneratedMethod(TypeBuilder typeBuilder, LambdaExpression converter, MethodBuilder result, object[] parameters)
        {
            typeBuilder.CreateType();

            typeBuilder.GetField($"{result.Name}_field", BindingFlags.Static | BindingFlags.Public)
                ?.SetValue(null, converter.Compile());

            return typeBuilder.GetMethod(result.Name, BindingFlags.Static | BindingFlags.Public)
                ?.Invoke(null, parameters);
        }

        [Test]
        public void Given_a_lambda_expression_When_calling_EmitConverterMethod_Then_it_returns_a_valid_method_that_calls_the_lambda_expression()
        {
            var typeBuilder = CreateTypeBuilder();
            Expression<Func<int, int>> converter = _ => 2;

            // Act
            var generatedMethod = ProxyHelper.EmitConverterMethod(typeBuilder, converter);

            // Assert
            var invocationResult = InvokeGeneratedMethod(typeBuilder, converter, generatedMethod, new object[] {1});
            Assert.That(invocationResult, Is.TypeOf<int>());
            Assert.That(invocationResult, Is.EqualTo(2));
        }

        [Test]
        public void Given_a_lambda_expression_with_no_parameters_When_calling_EmitConverterMethod_Then_it_returns_a_valid_method_that_calls_the_lambda_expression()
        {
            var typeBuilder = CreateTypeBuilder();
            Expression<Func<string>> converter = () => "Ugh";

            // Act
            var generatedMethod = ProxyHelper.EmitConverterMethod(typeBuilder, converter);

            // Assert
            var invocationResult = InvokeGeneratedMethod(typeBuilder, converter, generatedMethod, new object[0]);
            Assert.That(invocationResult, Is.TypeOf<string>());
            Assert.That(invocationResult, Is.EqualTo("Ugh"));
        }
    }
}
