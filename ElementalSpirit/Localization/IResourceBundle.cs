namespace ElementalSpirit.Localization
{

    public interface IResourceBundle
    {
        string GetString(string key);
        bool ContainsKey(string key);
    }
}