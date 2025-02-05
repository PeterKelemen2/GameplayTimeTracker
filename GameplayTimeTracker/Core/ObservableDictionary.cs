
namespace GameplayTimeTracker;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged,
    INotifyPropertyChanged
{
    private readonly Dictionary<TKey, TValue> _dictionary = new();

    public event NotifyCollectionChangedEventHandler CollectionChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
        OnPropertyChanged("Count");
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Keys");
        OnPropertyChanged("Values");
        OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
    }

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public ICollection<TKey> Keys => _dictionary.Keys;

    public bool Remove(TKey key)
    {
        if (_dictionary.TryGetValue(key, out TValue value) && _dictionary.Remove(key))
        {
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnPropertyChanged("Keys");
            OnPropertyChanged("Values");
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
            return true;
        }

        return false;
    }

    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
    public ICollection<TValue> Values => _dictionary.Values;

    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;
                OnPropertyChanged("Item[]");
                OnPropertyChanged("Keys");
                OnPropertyChanged("Values");
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value));
            }
            else
            {
                Add(key, value);
            }
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    public void Clear()
    {
        _dictionary.Clear();
        OnPropertyChanged("Count");
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Keys");
        OnPropertyChanged("Values");
        OnCollectionChanged(NotifyCollectionChangedAction.Reset);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) =>
        _dictionary.ContainsKey(item.Key) && _dictionary[item.Key].Equals(item.Value);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
    public int Count => _dictionary.Count;
    public bool IsReadOnly => false;
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item) =>
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));

    private void OnCollectionChanged(NotifyCollectionChangedAction action) =>
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
}