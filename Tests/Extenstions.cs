using SqlMigration;

namespace Tests
{
    public static class Extenstions
    {
        public static T OverloadFactory<T>(this T obj)
        {
            //is it a proxy?

            string fullNameKey = typeof(T).FullName;
            
            Factory.Overrides.Remove(fullNameKey);
            Factory.Overrides.Add(fullNameKey, obj);

            return obj;
        }
    }
}
