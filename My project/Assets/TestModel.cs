using System.Collections.Generic;

namespace Ui.Model
{
    internal class TestModel
    {
        Dictionary<int,bool> _keyValuePairs = new Dictionary<int,bool>();

        internal void Update(bool active, int id)
        {
            if (_keyValuePairs.ContainsKey(id))
            {
                _keyValuePairs[id] = active;
            }
            else
            {
                _keyValuePairs.Add(id, active);
            }

        }
    }
}
