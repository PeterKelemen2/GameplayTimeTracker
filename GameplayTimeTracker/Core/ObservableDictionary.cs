using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GameplayTimeTracker;

public class ObservableDictionary<TKey, TValue> : ObservableCollection<KeyValuePair<TKey, TValue>>
{
    private Dictionary<TKey, TValue> _dictionary;

    public ObservableDictionary()
    {
        _dictionary = new Dictionary<TKey, TValue>();
    }

    public new TValue this[TKey key]
    {
        get => _dictionary[key];
        set
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;
                var kvp = new KeyValuePair<TKey, TValue>(key, value);
                var existingItem = this.FirstOrDefault(i => EqualityComparer<TKey>.Default.Equals(i.Key, key));
                if (existingItem.Key != null)
                {
                    var index = this.IndexOf(existingItem);
                    if (index >= 0)
                    {
                        base[index] = kvp;
                    }
                }
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            }
            else
            {
                Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }
    }

    public new void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
        base.Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public new bool Remove(TKey key)
    {
        if (_dictionary.ContainsKey(key))
        {
            var kvp = new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
            var result = _dictionary.Remove(key);
            if (result)
            {
                base.Remove(kvp);
            }
            return result;
        }
        return false;
    }

    public new bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    public new ICollection<TKey> Keys => _dictionary.Keys;

    public new ICollection<TValue> Values => _dictionary.Values;

    public new int Count => _dictionary.Count;

    public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }
}

