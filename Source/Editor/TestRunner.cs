using FlaxEditor;
using FlaxEditor.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlaxEngine.UnitTesting.Editor
{
    public class TestRunner : EditorPlugin
    {
        private static List<Type> suites = new List<Type>();
        private MainMenuButton mmBtn;

        public override PluginDescription Description => new PluginDescription
        {
            Author = "Lukáš Jech",
            AuthorUrl ="https://lukas.jech.me",
            Category = "Unit Testing",
            Description = "Simple unit testing framework",
            IsAlpha = false,
            IsBeta = false,
            Name = "Simple Unit Testing",
            SupportedPlatforms = new PlatformType[] {PlatformType.Windows},
            Version = new Version(1,0),
            RepositoryUrl = "https://github.com/klukule/flax-ut"
        };

        public override void InitializeEditor()
        {
            base.InitializeEditor();

            mmBtn = Editor.UI.MainMenu.AddButton("Unit Tests");
            mmBtn.ContextMenu.AddButton("Run unit tests").Clicked += RunTests;

        }

        public override void Deinitialize()
        {
            base.Deinitialize();
            if (mmBtn != null)
            {
                mmBtn.Dispose();
                mmBtn = null;
            }
        }

        private static void GatherTests()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            suites.Clear();
            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    if (type.GetCustomAttributes<TestFixture>().Count() > 0)
                        suites.Add(type);
        }

        public static void RunTests()
        {
            GatherTests();

            foreach (var suite in suites)
            {
                var tests = suite.GetMethods().Where(m => m.GetCustomAttributes<TestCase>().Count() > 0 || m.GetCustomAttributes<Test>().Count() > 0).ToArray();
                var setup = suite.GetMethods().Where(m => m.GetCustomAttributes<SetUp>().Count() > 0).FirstOrDefault();
                var disposer = suite.GetMethods().Where(m => m.GetCustomAttributes<TearDown>().Count() > 0).FirstOrDefault();

                var instance = Activator.CreateInstance(suite);

                setup?.Invoke(instance, null);

                foreach (var testMethod in tests)
                {
                    // Mitigates the AttributeNullException
                    foreach (var test in testMethod.GetCustomAttributes<Test>())
                    {
                        bool failed = false;
                        try
                        {
                            testMethod?.Invoke(instance, null);
                        }
                        catch (Exception e)
                        {
                            if(e.GetType() != typeof(SuccessException))
                                failed = true;
                        }
                        finally
                        {
                            Debug.Log($"Test '{suite.Name} {testMethod.Name}' finished with " + (failed ? "Error" : "Success"));
                        }
                    }

                    var testCases = testMethod.GetCustomAttributes<TestCase>();
                    int successCount = 0;
                    foreach (var testCase in testCases)
                    {
                        bool failed = false;
                        try
                        {
                            var result = testMethod?.Invoke(instance, testCase.Attributes);
                            if (testCase.ExpectedResult != null)
                                failed = !testCase.ExpectedResult.Equals(result);
                        }
                        catch (Exception e)
                        {
                            if(e.GetType() != typeof(SuccessException))
                                failed = true;
                        }
                        finally
                        {
                            if (!failed)
                                successCount++;
                        }
                    }

                    if(testCases.Count() > 0)
                        Debug.Log($"Test '{suite.Name} {testMethod.Name}' finished with {successCount}/{testCases.Count()} successfull test cases.");
                }

                disposer?.Invoke(instance, null);
            }
        }
    }
}
