namespace Rosalia.Core.Helpers
{
    using System;

    public static class ValidationExtensions
    {
         public static T EnsureNotNull<T>(this T target, string name)
         {
             if (target == null)
             {
                 throw new Exception(string.Format("Required value {0} is not set", name));
             }

             return target;
         }
    }
}