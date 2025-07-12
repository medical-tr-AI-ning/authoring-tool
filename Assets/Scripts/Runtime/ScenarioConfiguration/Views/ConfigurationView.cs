namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// A view that supports loading and saving of data with a certain type
    /// </summary>
    /// <typeparam name="T">Type of the data that this view can save or load</typeparam>
    public abstract class ConfigurationView<T> : View
    {
        public abstract T GetConfiguration(string resourcesFolder);

        public abstract void LoadConfiguration(T configuration, string resourcesFolder);
        
    }
}