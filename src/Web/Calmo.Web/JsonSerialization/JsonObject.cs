using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Calmo.Web.JsonSerialization
{
    public sealed class JsonObject : DynamicObject, IDictionary<string, object>, IDictionary
    {
        private Dictionary<string, object> _members;

        public JsonObject()
        {
            _members = new Dictionary<string, object>();
        }

        internal JsonObject(Dictionary<string, object> dictionary)
        {
            _members = dictionary;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if ((binder.Type == typeof(IEnumerable)) ||
                (binder.Type == typeof(IEnumerable<KeyValuePair<string, object>>)) ||
                (binder.Type == typeof(IDictionary<string, object>)) ||
                (binder.Type == typeof(IDictionary)))
            {
                result = this;
                return true;
            }

            result = null;
            return false;
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return _members.Remove(binder.Name);
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            return _members.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_members.ContainsKey(binder.Name))
                _members[binder.Name] = value;
            else
                _members.Add(binder.Name, value);

            return true;
        }

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        #endregion

        #region Implementation of IEnumerable<KeyValuePair<string, object>>

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection

        int ICollection.Count
        {
            get
            {
                return _members.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of ICollection<KeyValuePair<string, object>>

        int ICollection<KeyValuePair<string, object>>.Count
        {
            get
            {
                return _members.Count;
            }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            ((IDictionary<string, object>)_members).Add(item);
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            _members.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)_members).Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IDictionary<string, object>)_members).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return ((IDictionary<string, object>)_members).Remove(item);
        }

        #endregion

        #region Implementation of IDictionary

        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return _members.Keys;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return _members[(string)key];
            }
            set
            {
                _members[(string)key] = value;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return _members.Values;
            }
        }

        void IDictionary.Add(object key, object value)
        {
            _members.Add((string)key, value);
        }

        void IDictionary.Clear()
        {
            _members.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return _members.ContainsKey((string)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (IDictionaryEnumerator)((IEnumerable)this).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            _members.Remove((string)key);
        }

        #endregion

        #region Implementation of IDictionary<string, object>

        ICollection<string> IDictionary<string, object>.Keys
        {
            get
            {
                return _members.Keys;
            }
        }

        object IDictionary<string, object>.this[string key]
        {
            get
            {
                return _members[key];
            }
            set
            {
                _members[key] = value;
            }
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get
            {
                return _members.Values;
            }
        }

        void IDictionary<string, object>.Add(string key, object value)
        {
            _members.Add(key, value);
        }

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return _members.ContainsKey(key);
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return _members.Remove(key);
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return _members.TryGetValue(key, out value);
        }

        #endregion
    }
}
