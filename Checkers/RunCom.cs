using System.Reflection;

namespace Kesco.Lib.Win.Document.Checkers
{
    public class RunCom
    {
        public static object Invoke(object target, string name, params object[] args)
        {
            return target.GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, target, args);
        }

        public static object GetProperty(object target, string name, object value)
        {
            return target.GetType().InvokeMember(name, BindingFlags.GetProperty | BindingFlags.IgnoreCase, null, target, new[] {value});
        }

        public static object GetProperty(object target, string name)
        {
            return target.GetType().InvokeMember(name, BindingFlags.GetProperty | BindingFlags.IgnoreCase, null, target, new object[0]);
        }

        public static void SetProperty(object target, string name, object value)
        {
            target.GetType().InvokeMember(name, BindingFlags.SetProperty | BindingFlags.IgnoreCase, null, target, new[] {value});
        }
    }
}