using System.Collections.Generic;

namespace CLAP.Interception
{
    public class ArgumentsCollection : ReadOnlyDictionary<string, Value>
    {
        internal ArgumentsCollection(Dictionary<string, string> inputArgs, List<object> values)
        {
            Dict = new Dictionary<string, Value>();

            var index = 0;

            foreach (var kvp in inputArgs)
            {
                Dict.Add(kvp.Key, new Value(kvp.Value, values[index]));

                index++;
            }
        }
    }

    public sealed class Value
    {
        public string StringValue { get; private set; }
        public object ObjectValue { get; private set; }

        public Value(string stringValue, object value)
        {
            StringValue = stringValue;
            ObjectValue = value;
        }
    }
}