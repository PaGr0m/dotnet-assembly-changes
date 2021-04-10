using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AssemblyChanges;
using NUnit.Framework;

namespace TestApplication.tests
{
    public class AssemblyTest
    {
        private readonly KeyValuePair<int, int> _ints = new(8, 3);
        private readonly KeyValuePair<double, double> _doubles = new(5.5, 7.3);
        private readonly KeyValuePair<decimal, decimal> _decimals = new(5m, 6m);

        [Test]
        public void MethodSolve()
        {
            var dllPath = Assembly.GetExecutingAssembly().Location;
            var newBuildDir = Path.GetDirectoryName(dllPath);
            Assert.NotNull(newBuildDir);

            var newDllPath = Path.Combine(newBuildDir, "changedBuild", Path.GetFileName(dllPath));

            // Before change methods
            var assembly = Assembly.LoadFile(dllPath);
            foreach (var type in assembly.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.Name.EndsWith("Int"))
                    {
                        var result = method.Invoke(null, new object[] {_ints.Key, _ints.Value});
                        Assert.AreEqual(_ints.Key + _ints.Value, result);
                    }
                    else if (method.Name.EndsWith("Double"))
                    {
                        var result = method.Invoke(null, new object[] {_doubles.Key, _doubles.Value});
                        Assert.AreEqual(_doubles.Key + _doubles.Value, result);
                    }
                    else if (method.Name.EndsWith("Decimal"))
                    {
                        var result = method.Invoke(null, new object[] {_decimals.Key, _decimals.Value});
                        Assert.AreEqual(_decimals.Key + _decimals.Value, result);
                    }
                }
            }

            // Change
            InternshipTask.Main(new[] {dllPath, newBuildDir});

            // After change methods
            var newAssembly = Assembly.LoadFile(newDllPath);
            foreach (var type in newAssembly.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.Name.EndsWith("Int"))
                    {
                        var result = method.Invoke(null, new object[] {_ints.Key, _ints.Value});
                        Assert.AreEqual(_ints.Key - _ints.Value, result);
                    }
                    else if (method.Name.EndsWith("Double"))
                    {
                        var result = method.Invoke(null, new object[] {_doubles.Key, _doubles.Value});
                        Assert.AreEqual(_doubles.Key - _doubles.Value, result);
                    }
                    else if (method.Name.EndsWith("Decimal"))
                    {
                        var result = method.Invoke(null, new object[] {_decimals.Key, _decimals.Value});
                        Assert.AreEqual(_decimals.Key - _decimals.Value, result);
                    }
                }
            }
        }
        
        [Test]
        public void TestWithIncorrectFilePath()
        {
            var simpleString = "incorrectDllPath";
            Assert.Throws<FileNotFoundException>(
                () => AssemblyUtils.Validation("incorrectDllPath", ref simpleString)
            );
        }

        [Test]
        public void TestWithIncorrectFileExtension()
        {
            var dllPath = Assembly.GetExecutingAssembly().Location;
            var buildDir = Path.GetDirectoryName(dllPath);

            Assert.NotNull(buildDir);
            var pdbPath = Path.Combine(buildDir, Path.GetFileNameWithoutExtension(dllPath) + ".pdb");

            var simpleString = "incorrectDllPath";
            Assert.Throws<ArgumentException>(
                () => AssemblyUtils.Validation(pdbPath, ref simpleString)
            );
        }
    }
}