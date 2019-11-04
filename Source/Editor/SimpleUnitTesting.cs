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
    public class SimpleUnitTesting : EditorPlugin
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
            Version = new Version(1, 2),
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

        private static bool TryGetAttribute<T>(MemberInfo memberInfo, out T attribute) where T : Attribute
        {
            attribute = memberInfo.GetCustomAttribute<T>();
            return attribute != null;
        }

        private static bool TryGetAttributes<T>(MemberInfo memberInfo, out List<T> attributes) where T : Attribute
        {
            attributes = memberInfo.GetCustomAttributes<T>().ToList();
            return attributes.Count > 0;
        }

        public static void RunTests()
        {
            GatherTests();

            foreach (var suite in _suites)
            {
                var suiteMethods = suite.GetMethods();

                var setup = suiteMethods.Where(m => m.GetCustomAttributes<OneTimeSetUp>().Count() > 0).FirstOrDefault();
                var disposer = suiteMethods.Where(m => m.GetCustomAttributes<OneTimeTearDown>().Count() > 0).FirstOrDefault();
                var beforeEach = suiteMethods.Where(m => m.GetCustomAttributes<SetUp>().Count() > 0).FirstOrDefault();
                var afterEach = suiteMethods.Where(m => m.GetCustomAttributes<TearDown>().Count() > 0).FirstOrDefault();

                var instance = Activator.CreateInstance(suite);

                setup?.Invoke(instance, null);

                foreach (var testMethod in suiteMethods)
                {
                    bool hasTestCases = TryGetAttributes(testMethod, out List<TestCase> testCasesAttribute);
                    bool hasTestCaseSource = TryGetAttribute(testMethod, out TestCaseSource testCaseSourceAttribute);

                    if (TryGetAttribute(testMethod, out Test testAttribute))
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
                            OutputResults(suite, testMethod, failed);
                        }
                    }
                    else if (hasTestCases || hasTestCaseSource)
                    {
                        IEnumerable<TestCaseData> testCases = Enumerable.Empty<TestCaseData>();

                        if (hasTestCases)
                        {
                            testCases = testCases.Concat(testCasesAttribute.Select(a => a.TestCaseData));
                        }
                        if (hasTestCaseSource)
                        {
                            var sourceClassType = testCaseSourceAttribute.SourceClassType ?? suite;
                            object sourceObjectInstance = null;
                            if (sourceClassType == suite || sourceClassType == null)
                            {
                                sourceObjectInstance = instance;
                            }
                            else if (!sourceClassType.IsAbstract)
                            {
                                sourceObjectInstance = Activator.CreateInstance(sourceClassType);
                            }

                            var sourceTestCases = (IEnumerable<TestCaseData>)(
                                sourceClassType
                                    .GetProperty(testCaseSourceAttribute.MemberName, typeof(IEnumerable<TestCaseData>))
                                    ?.GetValue(sourceObjectInstance) ??
                                sourceClassType
                                    .GetField(testCaseSourceAttribute.MemberName)
                                    ?.GetValue(sourceObjectInstance) ??
                               sourceClassType
                                    .GetMethod(testCaseSourceAttribute.MemberName)
                                    ?.Invoke(sourceObjectInstance, null));

                            testCases = testCases.Concat(sourceTestCases);
                        }

                        int testCaseCount = 0;
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
                                testCaseCount++;

                                if (!failed)
                                    successCount++;
                            }
                        }

                        OutputResults(suite, testMethod, successCount, testCaseCount);

                    }
                }

                disposer?.Invoke(instance, null);
            }
        }

        private static void OutputResults(Type suite, MethodInfo testMethod, int successCount, int testCount)
        {
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

        private static void OutputResults(Type suite, MethodInfo testMethod, bool failed)
        {
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
}
