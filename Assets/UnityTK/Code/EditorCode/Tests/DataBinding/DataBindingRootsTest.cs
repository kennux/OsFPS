using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityTK.DataBinding;

namespace UnityTK.Test.DataBinding
{
    public class DataBindingRootsTest
    {
        /// <summary>
        /// Creates a databinding root binding to an instance of <see cref="DataBindingTest"/>.
        /// </summary>
        public static DataBindingRoot CreateRootWithTest(out DataBindingTestExample testBindTarget)
        {
            var rootGo = new GameObject("Root");
            testBindTarget = rootGo.AddComponent<DataBindingTestExample>();
            var root = rootGo.AddComponent<DataBindingRoot>();
            root.target = testBindTarget;
            root.Awake();

            return root;
        }

        [Test]
        public void DataBindingRootTest()
        {
            // Create root
            DataBindingTestExample example;
            var root = CreateRootWithTest(out example);

            Assert.AreEqual(example, root.boundObject);
            Assert.AreEqual(typeof(DataBindingTestExample), root.boundType);
        }
        /// <summary>
        /// Creates a databinding root binding to an instance of <see cref="DataBindingTest"/>.
        /// </summary>
        public static DataBindingScriptedRoot CreateScriptedRootWithTest(out DataBindingTestExample testBindTarget)
        {
            var rootGo = new GameObject("Root");
            testBindTarget = rootGo.AddComponent<DataBindingTestExample>();
            var root = rootGo.AddComponent<DataBindingScriptedRoot>();
			root.bindTargetType = "UnityTK.Test.DataBinding.DataBindingTestExample, UnityTK.Tests";
            root.target = testBindTarget;
            root.Awake();

            return root;
        }

        [Test]
        public void DataBindingScriptedRootTest()
        {
            // Create root
            DataBindingTestExample example;
            var root = CreateScriptedRootWithTest(out example);

            Assert.AreEqual(example, root.boundObject);
            Assert.AreEqual(typeof(DataBindingTestExample), root.boundType);

			bool exceptionFired = false;
			try { root.target = 1; }
			catch (System.ArgumentException ex) { exceptionFired = true; }
			Assert.IsTrue(exceptionFired);
        }
    }
}