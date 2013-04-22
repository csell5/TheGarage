namespace MarkNic.JsonSupport.Interfaces
{
    public interface IJsonSerialize
    {
        /// <summary>
        /// Serialize the component's values into a JSON format and return
        /// </summary>
        /// <returns>JSON formatted component state</returns>
        string SerializeJson();
    }
}
