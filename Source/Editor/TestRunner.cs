using FlaxEditor;
using FlaxEditor.GUI;
using FlaxEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FlaxCommunity.UnitTesting;

namespace FlaxCommunity.UnitTesting.Editor
{
    public class TestRunner : EditorPlugin
    {
        private static readonly List<Type> _suites = new List<Type>();
        private MainMenuButton _mmBtn;

        public override PluginDescription Description => new PluginDescription
        {
            Author = "Lukáš Jech",
            AuthorUrl = "https://lukas.jech.me",
            Category = "Unit Testing",
            Description = "Simple unit testing framework",
            IsAlpha = false,
            IsBeta = false,
            Name = "Simple Unit Testing",
            SupportedPlatforms = new PlatformType[] { PlatformType.Windows },
            Version = new Version(1, 1),
            RepositoryUrl = "https://github.com/FlaxCommunityProjects/FlaxUnitTesting"
        };

        public override void InitializeEditor()
        {
            base.InitializeEditor();

            _mmBtn = Editor.UI.MainMenu.AddButton("Unit Tests");
            _mmBtn.ContextMenu.AddButton("Run unit tests").Clicked += RunTests;
            FlaxEditor.Scripting.ScriptsBuilder.ScriptsReloadBegin += ScriptsBuilder_ScriptsReloadBegin;
        }

        private void ScriptsBuilder_ScriptsReloadBegin()
        {
            // Clear type information as per warning https://docs.flaxengine.com/manual/scripting/plugins/index.html
            _suites.Clear();
        }

        public override void Deinitialize()
        {
            base.Deinitialize();
            if (_mmBtn != null)
            {
                _mmBtn.Dispose();
                _mmBtn = null;
            }
        }

        private static void GatherTests()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _suites.Clear();
            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    if (type.GetCustomAttributes<TestFixture>().Count() > 0)
                        _suites.Add(type);
        }

        public static void RunTests()
        {
            GatherTests();

            foreach (var suite in _suites)
            {
                var suiteMethods = suite.GetMethods();

                var tests = suiteMethods.Where(m => m.GetCustomAttributes<Test>().Count() > 0 || m.GetCustomAttributes<TestCase>().Count() > 0).ToArray();
                var setup = suiteMethods.Where(m => m.GetCustomAttributes<OneTimeSetUp>().Count() > 0).FirstOrDefault();
                var disposer = suiteMethods.Where(m => m.GetCustomAttributes<OneTimeTearDown>().Count() > 0).FirstOrDefault();
                var beforeEach = suiteMethods.Where(m => m.GetCustomAttributes<SetUp>().Count() > 0).FirstOrDefault();
                var afterEach = suiteMethods.Where(m => m.GetCustomAttributes<TearDown>().Count() > 0).FirstOrDefault();

                var instance = Activator.CreateInstance(suite);

                setup?.Invoke(instance, null);

                foreach (var testMethod in tests)
                {
                    if (testMethod.GetCustomAttributes<Test>().Count() > 0)
                    {
                        bool failed = false;
                        beforeEach?.Invoke(instance, null);
                        try
                        {
                            testMethod?.Invoke(instance, null);
                        }
                        catch (TargetInvocationException e)
                        {
                            if (!(e.InnerException is SuccessException))
                            {
                                failed = true;
                                Debug.LogException(e.InnerException);
                            }
                        }
                        finally
                        {
                            afterEach?.Invoke(instance, null);
                            string message = $"Test '{suite.Name} {testMethod.Name}' finished with " + (failed ? "Error" : "Success");
                            if (failed)
                            {
                                Debug.LogError(message);
                            }
                            else
                            {
                                Debug.Log(message);
                            }
                        }
                    }
                    else
                    {
                        var testCases = testMethod.GetCustomAttributes<TestCase>();
                        int successCount = 0;
                        foreach (var testCase in testCases)
                        {
                            bool failed = false;
                            beforeEach?.Invoke(instance, null);
                            try
                            {
                                var result = testMethod?.Invoke(instance, testCase.Attributes);
                                if (testCase.ExpectedResult != null)
                                    failed = !testCase.ExpectedResult.Equals(result);
                            }
                            catch (TargetInvocationException e)
                            {
                                if (!(e.InnerException is SuccessException))
                                {
                                    failed = true;
                                }
                            }
                            finally
                            {
                                afterEach?.Invoke(instance, null);

                                if (!failed)
                                    successCount++;
                            }
                        }

                        int testCount = testCases.Count();
                        string message = $"Test '{suite.Name} {testMethod.Name}' finished with {successCount}/{testCount} successfull test cases.";
                        if (successCount < testCount)
                        {
                            Debug.LogError(message);
                        }
                        else
                        {
                            Debug.Log(message);
                        }
                    }
                }

                disposer?.Invoke(instance, null);
            }
        }
    }
}
