using System.Reflection;
using UnityEngine;
using ILHotfix;
using System.Collections.Generic;

namespace Hotfix
{
    /// <summary>
    /// 热修复类(需要静态方法)
    /// </summary>
    public class Fiaxed
    {
        public const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        /// <summary>
        /// 执行替换初始化
        /// </summary>
        public static void Init()
        {
            //替换方法
            //HotScripts.ReplaceFunc(typeof(TestFix), "Update", typeof(HotHelloWorld).GetMethod("Update", bindingFlags));

            HotScripts.ReplaceField(typeof(TestFix), "__Hotfix_ABC_0", typeof(HotHelloWorld).GetMethod("TestFunc", bindingFlags));

            HotScripts.ReplaceField(typeof(TestFix), "__Hotfix_ABC_1", typeof(HotHelloWorld).GetMethod("TestFunc1", bindingFlags));

            HotScripts.ReplaceField(typeof(TestFix), "__Hotfix_ABC_2", typeof(HotHelloWorld).GetMethod("TestFunc2", bindingFlags));

            HotScripts.ReplaceField(typeof(TestFix), "__Hotfix_ABC_3", typeof(HotHelloWorld).GetMethod("TestFunc3", bindingFlags));

            HotScripts.ReplaceField(typeof(TestFix), "__Hotfix_ABC_4", typeof(HotHelloWorld).GetMethod("TestFunc4", bindingFlags));

            HotScripts.ReplaceField(typeof(TestFix), "__Hotfix_ABC_5", typeof(HotHelloWorld).GetMethod("TestFunc5", bindingFlags));

            HotScripts.ReplaceField(typeof(TestFix), "__Hotfix_ABC_6", typeof(HotHelloWorld).GetMethod("TestFunc6", bindingFlags));

            HotScripts.ReplaceField(typeof(TestFix), "__Hotfix_ABC_7", typeof(HotHelloWorld).GetMethod("TestFunc7", bindingFlags));
        }
    }


    static class HotHelloWorld
    {
        public static void Update(TestFix testFix)
        {

            Debug.Log("热更新");
        }

        public static void TestFunc(TestFix testFix)
        {

            Debug.Log("热补丁");
        }

        public static void TestFunc1(TestFix testFix ,int a)
        {

            Debug.Log("热补丁" + a);
        }
        public static void TestFunc2(TestFix testFix,string b)
        {

            Debug.Log("热补丁" + b);
        }
        public static int TestFunc3(TestFix testFix,int a, int b)
        {
            Debug.Log("热补丁int 返回" + a + "  +  " + b);
            return a + b * 100;
        }
        public static void TestFunc4(TestFix testFix, RefOutParam<int> a, RefOutParam<string> b)
        {
            
            
            a.value = 500;
            b.value = "hello world!!!!!!!!!!!!hotfix";
            Debug.Log("热补丁"+a.value + "  +  " + b.value);
        }

        public static void TestFunc5(TestFix testFix,  int a,  string b)
        {

            Debug.Log("热补丁"+ a + "  +  " + b);
        }
        public static void TestFunc6(TestFix testFix, string a, int b)
        {

            Debug.Log("热补丁"+ a + "  +  " + b);
        }

        public static void TestFunc7(List<object> values)
        {
            TestFix instance = values[0] as TestFix;
            object value = values[1];
            var type = value.GetType();
            object value2 = values[2];
            Debug.Log("热补丁" + type.Name + "  +  " + value2);
        }
    }

    
}
