namespace RadicalGeek.Common.Patterns.Singleton
{
    /// <summary>
    /// Provides services to allow for a single instance, according to the type T, using a factory and cached list to manage the instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : class,new()
    {
        /// <summary>
        /// Returns the current Singleton instance of this class
        /// </summary>
        public static T Current
        {
            get { return SingletonFactory.Get<T>(); }
        }
    }
}
