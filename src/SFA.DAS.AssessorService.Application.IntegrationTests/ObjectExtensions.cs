using System;
using System.Reflection;

namespace SFA.DAS.AssessorService.Application.Api.IntegrationTests
{
    public static class ObjectExtensions
    {
        public static TReturn CallPrivateMethod<TReturn>(
                this object instance,
                string methodName,
                params object[] parameters)
        {
            Type type = instance.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo method = type.GetMethod(methodName, bindingAttr);

            return (TReturn)method.Invoke(instance, parameters);
        }

        public static void CallPrivateMethod(
                this object instance,
                string methodName,
                params object[] parameters)
        {
            Type type = instance.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo method = type.GetMethod(methodName, bindingAttr);

            method.Invoke(instance, parameters);
        }
    }
}
