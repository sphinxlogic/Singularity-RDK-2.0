// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
//============================================================
//
// Class:  SortedList
//
// Purpose: A sorted dictionary.
//
//===========================================================  
namespace System.Collections
{
    using System;

    // The SortedList class implements a sorted list of keys and values. Entries in
    // a sorted list are sorted by their keys and are accessible both by key and by
    // index. The keys of a sorted list can be ordered either according to a
    // specific IComparer implementation given when the sorted list is
    // instantiated, or according to the IComparable implementation provided
    // by the keys themselves. In either case, a sorted list does not allow entries
    // with duplicate keys.
    //
    // A sorted list internally maintains two arrays that store the keys and
    // values of the entries. The capacity of a sorted list is the allocated
    // length of these internal arrays. As elements are added to a sorted list, the
    // capacity of the sorted list is automatically increased as required by
    // reallocating the internal arrays.  The capacity is never automatically
    // decreased, but users can call either TrimToSize or
    // Capacity explicitly.
    //
    // The GetKeyList and GetValueList methods of a sorted list
    // provides access to the keys and values of the sorted list in the form of
    // List implementations. The List objects returned by these
    // methods are aliases for the underlying sorted list, so modifications
    // made to those lists are directly reflected in the sorted list, and vice
    // versa.
    //
    // The SortedList class provides a convenient way to create a sorted
    // copy of another dictionary, such as a Hashtable. For example:
    //
    // Hashtable h = new Hashtable();
    // h.Add(...);
    // h.Add(...);
    // ...
    // SortedList s = new SortedList(h);
    //
    // The last line above creates a sorted list that contains a copy of the keys
    // and values stored in the hashtable. In this particular example, the keys
    // will be ordered according to the IComparable interface, which they
    // all must implement. To impose a different ordering, SortedList also
    // has a constructor that allows a specific IComparer implementation to
    // be specified.
    //
    // version 1.10
    //| <include path='docs/doc[@for="SortedList"]/*' />
    public class SortedList : IDictionary, ICloneable
    {
        private Object[] keys;
        private Object[] values;
        private int _size;
        private int version;
        private IComparer comparer;
        private KeyList keyList;
        private ValueList valueList;

        private const int _defaultCapacity = 16;

        // Constructs a new sorted list. The sorted list is initially empty and has
        // a capacity of zero. Upon adding the first element to the sorted list the
        // capacity is increased to 16, and then increased in multiples of two as
        // required. The elements of the sorted list are ordered according to the
        // IComparable interface, which must be implemented by the keys of
        // all entries added to the sorted list.
        //| <include path='docs/doc[@for="SortedList.SortedList"]/*' />
        public SortedList() {
            keys = new Object[_defaultCapacity];
            values = new Object[_defaultCapacity];
            comparer = Comparer.Default;
        }

        // Constructs a new sorted list. The sorted list is initially empty and has
        // a capacity of zero. Upon adding the first element to the sorted list the
        // capacity is increased to 16, and then increased in multiples of two as
        // required. The elements of the sorted list are ordered according to the
        // IComparable interface, which must be implemented by the keys of
        // all entries added to the sorted list.
        //
        //| <include path='docs/doc[@for="SortedList.SortedList1"]/*' />
        public SortedList(int initialCapacity) {
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException("initialCapacity", "ArgumentOutOfRange_NeedNonNegNum");
            keys = new Object[initialCapacity];
            values = new Object[initialCapacity];
            comparer = Comparer.Default;
        }

        // Constructs a new sorted list with a given IComparer
        // implementation. The sorted list is initially empty and has a capacity of
        // zero. Upon adding the first element to the sorted list the capacity is
        // increased to 16, and then increased in multiples of two as required. The
        // elements of the sorted list are ordered according to the given
        // IComparer implementation. If comparer is null, the
        // elements are compared to each other using the IComparable
        // interface, which in that case must be implemented by the keys of all
        // entries added to the sorted list.
        //
        //| <include path='docs/doc[@for="SortedList.SortedList2"]/*' />
        public SortedList(IComparer comparer)
            : this() {
            if (comparer != null) this.comparer = comparer;
        }

        // Constructs a new sorted list with a given IComparer
        // implementation and a given initial capacity. The sorted list is
        // initially empty, but will have room for the given number of elements
        // before any reallocations are required. The elements of the sorted list
        // are ordered according to the given IComparer implementation. If
        // comparer is null, the elements are compared to each other using
        // the IComparable interface, which in that case must be implemented
        // by the keys of all entries added to the sorted list.
        //
        //| <include path='docs/doc[@for="SortedList.SortedList3"]/*' />
        public SortedList(IComparer comparer, int capacity)
            : this(comparer) {
            Capacity = capacity;
        }

        // Constructs a new sorted list containing a copy of the entries in the
        // given dictionary. The elements of the sorted list are ordered according
        // to the IComparable interface, which must be implemented by the
        // keys of all entries in the the given dictionary as well as keys
        // subsequently added to the sorted list.
        //
        //| <include path='docs/doc[@for="SortedList.SortedList4"]/*' />
        public SortedList(IDictionary d)
            : this(d, null) {
        }

        // Constructs a new sorted list containing a copy of the entries in the
        // given dictionary. The elements of the sorted list are ordered according
        // to the given IComparer implementation. If comparer is
        // null, the elements are compared to each other using the
        // IComparable interface, which in that case must be implemented
        // by the keys of all entries in the the given dictionary as well as keys
        // subsequently added to the sorted list.
        //
        //| <include path='docs/doc[@for="SortedList.SortedList5"]/*' />
        public SortedList(IDictionary d, IComparer comparer)
            : this(comparer, (d != null ? d.Count : 0)) {
            if (d == null)
                throw new ArgumentNullException("d", "ArgumentNull_Dictionary");
            d.Keys.CopyTo(keys, 0);
            d.Values.CopyTo(values, 0);
            Array.Sort(keys, values, comparer);
            _size = d.Count;
        }

        // Adds an entry with the given key and value to this sorted list. An
        // ArgumentException is thrown if the key is already present in the sorted list.
        //
        //| <include path='docs/doc[@for="SortedList.Add"]/*' />
        public virtual void Add(Object key, Object value) {
            if (key == null) throw new ArgumentNullException("key", "ArgumentNull_Key");
            int i = Array.BinarySearch(keys, 0, _size, key, comparer);
            if (i >= 0)
                throw new ArgumentException("Argument_AddingDuplicate__" + key);
            Insert(~i, key, value);
        }

        // Returns the capacity of this sorted list. The capacity of a sorted list
        // represents the allocated length of the internal arrays used to store the
        // keys and values of the list, and thus also indicates the maximum number
        // of entries the list can contain before a reallocation of the internal
        // arrays is required.
        //
         //| <include path='docs/doc[@for="SortedList.Capacity"]/*' />
         public virtual int Capacity {
            get {
                return keys.Length;
            }
            set {
                if (value != keys.Length) {
                    if (value < _size) throw new ArgumentOutOfRangeException("value", "ArgumentOutOfRange_SmallCapacity");
                    if (value > 0) {
                        Object[] newKeys = new Object[value];
                        Object[] newValues = new Object[value];
                        if (_size > 0) {
                            Array.Copy(keys, 0, newKeys, 0, _size);
                            Array.Copy(values, 0, newValues, 0, _size);
                        }
                        keys = newKeys;
                        values = newValues;
                    }
                    else {
                        keys = new Object[_defaultCapacity];
                        values = new Object[_defaultCapacity];
                    }
                }
            }
        }

        // Returns the number of entries in this sorted list.
        //
        //| <include path='docs/doc[@for="SortedList.Count"]/*' />
        public virtual int Count {
            get {
                return _size;
            }
        }

        // Returns a collection representing the keys of this sorted list. This
        // method returns the same object as GetKeyList, but typed as an
        // ICollection instead of an IList.
        //
         //| <include path='docs/doc[@for="SortedList.Keys"]/*' />
         public virtual ICollection Keys {
            get {
               return GetKeyList();
            }
        }

        // Returns a collection representing the values of this sorted list. This
        // method returns the same object as GetValueList, but typed as an
        // ICollection instead of an IList.
        //
         //| <include path='docs/doc[@for="SortedList.Values"]/*' />
         public virtual ICollection Values {
            get {
                return GetValueList();
            }
        }

        // Is this SortedList read-only?
        //| <include path='docs/doc[@for="SortedList.IsReadOnly"]/*' />
        public virtual bool IsReadOnly {
            get { return false; }
        }

        //| <include path='docs/doc[@for="SortedList.IsFixedSize"]/*' />
        public virtual bool IsFixedSize {
            get { return false; }
        }

        // Is this SortedList synchronized (thread-safe)?
        //| <include path='docs/doc[@for="SortedList.IsSynchronized"]/*' />
        public virtual bool IsSynchronized {
            get { return false; }
        }

        // Synchronization root for this object.
        //| <include path='docs/doc[@for="SortedList.SyncRoot"]/*' />
        public virtual Object SyncRoot {
            get { return this; }
        }

        // Removes all entries from this sorted list.
        //| <include path='docs/doc[@for="SortedList.Clear"]/*' />
        public virtual void Clear() {
            version++;
            _size = 0;
            keys = new Object[_defaultCapacity];
            values = new Object[_defaultCapacity];
        }

        // Makes a virtually identical copy of this SortedList.  This is a shallow
        // copy.  IE, the Objects in the SortedList are not cloned - we copy the
        // references to those objects.
        //| <include path='docs/doc[@for="SortedList.Clone"]/*' />
        public virtual Object Clone()
        {
            SortedList sl = new SortedList(_size);
            Array.Copy(keys, 0, sl.keys, 0, _size);
            Array.Copy(values, 0, sl.values, 0, _size);
            sl._size = _size;
            sl.version = version;
            sl.comparer = comparer;
            // Don't copy keyList nor valueList.
            return sl;
        }


        // Checks if this sorted list contains an entry with the given key.
        //
        //| <include path='docs/doc[@for="SortedList.Contains"]/*' />
        public virtual bool Contains(Object key) {
            return IndexOfKey(key) >= 0;
        }

        // Checks if this sorted list contains an entry with the given key.
        //
        //| <include path='docs/doc[@for="SortedList.ContainsKey"]/*' />
        public virtual bool ContainsKey(Object key) {
            // Yes, this is a SPEC'ed duplicate of Contains().
            return IndexOfKey(key) >= 0;
        }

        // Checks if this sorted list contains an entry with the given value. The
        // values of the entries of the sorted list are compared to the given value
        // using the Object.Equals method. This method performs a linear
        // search and is substantially slower than the Contains
        // method.
        //
        //| <include path='docs/doc[@for="SortedList.ContainsValue"]/*' />
        public virtual bool ContainsValue(Object value) {
            return IndexOfValue(value) >= 0;
        }

        // Copies the values in this SortedList to an array.
        //| <include path='docs/doc[@for="SortedList.CopyTo"]/*' />
        public virtual void CopyTo(Array array, int arrayIndex) {
            if (array == null)
            throw new ArgumentNullException("array", "ArgumentNull_Array");
            if (array.Rank != 1)
                throw new ArgumentException("Arg_RankMultiDimNotSupported");
            if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException("arrayIndex", "ArgumentOutOfRange_NeedNonNegNum");
            if (array.Length - arrayIndex < Count)
            throw new ArgumentException("Arg_ArrayPlusOffTooSmall");
            for (int i = 0; i < Count; i++) {
        DictionaryEntry entry = new DictionaryEntry(keys[i],values[i]);
                array.SetValue(entry, i + arrayIndex);
            }
        }

        // Ensures that the capacity of this sorted list is at least the given
        // minimum value. If the correct capacity of the list is less than
        // min, the capacity is increased to twice the current capacity or
        // to min, whichever is larger.
        private void EnsureCapacity(int min) {
            int newCapacity = keys.Length == 0? 16: keys.Length * 2;
            if (newCapacity < min) newCapacity = min;
            Capacity = newCapacity;
        }

        // Returns the value of the entry at the given index.
        //
        //| <include path='docs/doc[@for="SortedList.GetByIndex"]/*' />
        public virtual Object GetByIndex(int index) {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
            return values[index];
        }

        // Returns an IEnumerator for this sorted list.  If modifications
        // made to the sorted list while an enumeration is in progress,
        // the MoveNext and Remove methods
        // of the enumerator will throw an exception.
        //
        //| <include path='docs/doc[@for="SortedList.IEnumerable.GetEnumerator"]/*' />
        IEnumerator IEnumerable.GetEnumerator() {
            return new SortedListEnumerator(this, 0, _size, SortedListEnumerator.DictEntry);
        }

        // Returns an IDictionaryEnumerator for this sorted list.  If modifications
        // made to the sorted list while an enumeration is in progress,
        // the MoveNext and Remove methods
        // of the enumerator will throw an exception.
        //
        //| <include path='docs/doc[@for="SortedList.GetEnumerator"]/*' />
        public virtual IDictionaryEnumerator GetEnumerator() {
            return new SortedListEnumerator(this, 0, _size, SortedListEnumerator.DictEntry);
        }

        // Returns the key of the entry at the given index.
        //
        //| <include path='docs/doc[@for="SortedList.GetKey"]/*' />
        public virtual Object GetKey(int index) {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
            return keys[index];
        }

        // Returns an IList representing the keys of this sorted list. The
        // returned list is an alias for the keys of this sorted list, so
        // modifications made to the returned list are directly reflected in the
        // underlying sorted list, and vice versa. The elements of the returned
        // list are ordered in the same way as the elements of the sorted list. The
        // returned list does not support adding, inserting, or modifying elements
        // (the Add, AddRange, Insert, InsertRange,
        // Reverse, Set, SetRange, and Sort methods
        // throw exceptions), but it does allow removal of elements (through the
        // Remove and RemoveRange methods or through an enumerator).
        // Null is an invalid key value.
        //
        //| <include path='docs/doc[@for="SortedList.GetKeyList"]/*' />
        public virtual IList GetKeyList() {
            if (keyList == null) keyList = new KeyList(this);
            return keyList;
        }

        // Returns an IList representing the values of this sorted list. The
        // returned list is an alias for the values of this sorted list, so
        // modifications made to the returned list are directly reflected in the
        // underlying sorted list, and vice versa. The elements of the returned
        // list are ordered in the same way as the elements of the sorted list. The
        // returned list does not support adding or inserting elements (the
        // Add, AddRange, Insert and InsertRange
        // methods throw exceptions), but it does allow modification and removal of
        // elements (through the Remove, RemoveRange, Set and
        // SetRange methods or through an enumerator).
        //
        //| <include path='docs/doc[@for="SortedList.GetValueList"]/*' />
        public virtual IList GetValueList() {
            if (valueList == null) valueList = new ValueList(this);
            return valueList;
        }

        // Returns the value associated with the given key. If an entry with the
        // given key is not found, the returned value is null.
        //
        //| <include path='docs/doc[@for="SortedList.this"]/*' />
        public virtual Object this[Object key] {
            get {
                int i = IndexOfKey(key);
                if (i >= 0) return values[i];
                return null;
            }
            set {
                if (key == null) throw new ArgumentNullException("key", "ArgumentNull_Key");
                int i = Array.BinarySearch(keys, 0, _size, key, comparer);
                if (i >= 0) {
                    values[i] = value;
                    version++;
                    return;
                }
                Insert(~i, key, value);
            }
        }

        // Returns the index of the entry with a given key in this sorted list. The
        // key is located through a binary search, and thus the average execution
        // time of this method is proportional to Log2(size), where
        // size is the size of this sorted list. The returned value is -1 if
        // the given key does not occur in this sorted list. Null is an invalid
        // key value.
        //
        //| <include path='docs/doc[@for="SortedList.IndexOfKey"]/*' />
        public virtual int IndexOfKey(Object key) {
            if (key == null)
                throw new ArgumentNullException("key", "ArgumentNull_Key");
            int ret = Array.BinarySearch(keys, 0, _size, key, comparer);
            return ret >=0 ? ret : -1;
        }

        // Returns the index of the first occurrence of an entry with a given value
        // in this sorted list. The entry is located through a linear search, and
        // thus the average execution time of this method is proportional to the
        // size of this sorted list. The elements of the list are compared to the
        // given value using the Object.Equals method.
        //
        //| <include path='docs/doc[@for="SortedList.IndexOfValue"]/*' />
        public virtual int IndexOfValue(Object value) {
            return Array.IndexOf(values, value, 0, _size);
        }

        // Inserts an entry with a given key and value at a given index.
        private void Insert(int index, Object key, Object value) {
            if (_size == keys.Length) EnsureCapacity(_size + 1);
            if (index < _size) {
                Array.Copy(keys, index, keys, index + 1, _size - index);
                Array.Copy(values, index, values, index + 1, _size - index);
            }
            keys[index] = key;
            values[index] = value;
            _size++;
            version++;
        }

        // Removes the entry at the given index. The size of the sorted list is
        // decreased by one.
        //
        //| <include path='docs/doc[@for="SortedList.RemoveAt"]/*' />
        public virtual void RemoveAt(int index) {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
            _size--;
            if (index < _size) {
                Array.Copy(keys, index + 1, keys, index, _size - index);
                Array.Copy(values, index + 1, values, index, _size - index);
            }
            keys[_size] = null;
            values[_size] = null;
            version++;
        }

        // Removes an entry from this sorted list. If an entry with the specified
        // key exists in the sorted list, it is removed. An ArgumentException is
        // thrown if the key is null.
        //
        //| <include path='docs/doc[@for="SortedList.Remove"]/*' />
        public virtual void Remove(Object key) {
            int i = IndexOfKey(key);
            if (i >= 0)
        RemoveAt(i);
        }

        // Sets the value at an index to a given value.  The previous value of
        // the given entry is overwritten.
        //
        //| <include path='docs/doc[@for="SortedList.SetByIndex"]/*' />
        public virtual void SetByIndex(int index, Object value) {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
            values[index] = value;
            version++;
        }

        // Returns a thread-safe SortedList.
        //
        //| <include path='docs/doc[@for="SortedList.Synchronized"]/*' />
        public static SortedList Synchronized(SortedList list) {
            if (list == null)
                throw new ArgumentNullException("list");
            return new SyncSortedList(list);
        }

        // Sets the capacity of this sorted list to the size of the sorted list.
        // This method can be used to minimize a sorted list's memory overhead once
        // it is known that no new elements will be added to the sorted list. To
        // completely clear a sorted list and release all memory referenced by the
        // sorted list, execute the following statements:
        //
        // sortedList.Clear();
        // sortedList.TrimToSize();
        //
        //| <include path='docs/doc[@for="SortedList.TrimToSize"]/*' />
        public virtual void TrimToSize() {
            Capacity = _size;
        }

        private class SyncSortedList : SortedList
        {
            private SortedList _list;
            private Object _root;

            internal SyncSortedList(SortedList list) {
                _list = list;
                _root = list.SyncRoot;
            }

            public override int Count {
                get { lock (_root) { return _list.Count; } }
            }

            public override Object SyncRoot {
                get { return _root; }
            }

            public override bool IsReadOnly {
                get { return _list.IsReadOnly; }
            }

            public override bool IsFixedSize {
                get { return _list.IsFixedSize; }
            }


            public override bool IsSynchronized {
                get { return true; }
            }

            public override Object this[Object key] {
                get {
                    lock (_root) {
                        return _list[key];
                    }
                }
                set {
                    lock (_root) {
                        _list[key] = value;
                    }
                }
            }

            public override void Add(Object key, Object value) {
                lock (_root) {
                    _list.Add(key, value);
                }
            }

            public override int Capacity {
                get{ lock (_root) {  return _list.Capacity; } }
            }

            public override void Clear() {
                lock (_root) {
                    _list.Clear();
                }
            }

            public override Object Clone() {
                lock (_root) {
                    return _list.Clone();
                }
            }

            public override bool Contains(Object key) {
                lock (_root) {
                    return _list.Contains(key);
                }
            }

            public override bool ContainsKey(Object key) {
                lock (_root) {
                    return _list.ContainsKey(key);
                }
            }

            public override bool ContainsValue(Object key) {
                lock (_root) {
                    return _list.ContainsValue(key);
                }
            }

            public override void CopyTo(Array array, int index) {
                lock (_root) {
                    _list.CopyTo(array, index);
                }
            }

            public override Object GetByIndex(int index) {
                lock (_root) {
                    return _list.GetByIndex(index);
                }
            }

            public override IDictionaryEnumerator GetEnumerator() {
                lock (_root) {
                    return _list.GetEnumerator();
                }
            }

            public override Object GetKey(int index) {
                lock (_root) {
                    return _list.GetKey(index);
                }
            }

            public override IList GetKeyList() {
                lock (_root) {
                    return _list.GetKeyList();
                }
            }

            public override IList GetValueList() {
                lock (_root) {
                    return _list.GetValueList();
                }
            }

            public override int IndexOfKey(Object key) {
                lock (_root) {
                    return _list.IndexOfKey(key);
                }
            }

            public override int IndexOfValue(Object value) {
                lock (_root) {
                    return _list.IndexOfValue(value);
                }
            }

            public override void RemoveAt(int index) {
                lock (_root) {
                    _list.RemoveAt(index);
                }
            }

            public override void Remove(Object key) {
                lock (_root) {
                    _list.Remove(key);
                }
            }

            public override void SetByIndex(int index, Object value) {
                lock (_root) {
                    _list.SetByIndex(index, value);
                }
            }

            public override void TrimToSize() {
                lock (_root) {
                    _list.TrimToSize();
                }
            }
        }


        private class SortedListEnumerator : IDictionaryEnumerator, ICloneable
        {
            private SortedList sortedList;
            private Object key;
            private Object value;
            private int index;
            private int startIndex;        // Store for Reset.
            private int endIndex;
            private int version;
            private bool current;       // Is the current element valid?
            private int getObjectRetType;  // What should GetObject return?

            internal const int Keys = 1;
            internal const int Values = 2;
            internal const int DictEntry = 3;

            internal SortedListEnumerator(SortedList sortedList, int index, int count,
                                 int getObjRetType) {
                this.sortedList = sortedList;
                this.index = index;
                startIndex = index;
                endIndex = index + count;
                version = sortedList.version;
                getObjectRetType = getObjRetType;
                current = false;
            }

            public Object Clone()
            {
                return MemberwiseClone();
            }

            public virtual Object Key {
                get {
                    if (version != sortedList.version) throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    if (current == false) throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                    return key;
                }
            }

            public virtual bool MoveNext() {
                if (version != sortedList.version) throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                if (index < endIndex) {
                    key = sortedList.keys[index];
                    value = sortedList.values[index];
                    index++;
                    current = true;
                    return true;
                }
                key = null;
                value = null;
                current = false;
                return false;
            }

            public virtual DictionaryEntry Entry {
                get {
                    if (version != sortedList.version) throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    if (current == false) throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                    return new DictionaryEntry(key, value);
                }
            }

            public virtual Object Current {
                get {
                    if (current == false) throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");

                    if (getObjectRetType == Keys)
                        return key;
                    else if (getObjectRetType == Values)
                        return value;
                    else
                        return new DictionaryEntry(key, value);
                }
            }

            public virtual Object Value {
                get {
                    if (version != sortedList.version) throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    if (current == false) throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                    return value;
                }
            }

            public virtual void Reset() {
                if (version != sortedList.version) throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                index = startIndex;
                current = false;
                key = null;
                value = null;
            }
        }

        private class KeyList : IList
        {
            private SortedList sortedList;

            internal KeyList(SortedList sortedList) {
                this.sortedList = sortedList;
            }

            public virtual int Count {
                get { return sortedList._size; }
            }

            public virtual bool IsReadOnly {
                get { return true; }
            }

            public virtual bool IsFixedSize {
                get { return true; }
            }

            public virtual bool IsSynchronized {
                get { return sortedList.IsSynchronized; }
            }

            public virtual Object SyncRoot {
                get { return sortedList.SyncRoot; }
            }

            public virtual int Add(Object key) {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
                //            return 0; // suppress compiler warning
            }

            public virtual void Clear() {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
            }

            public virtual bool Contains(Object key) {
                return sortedList.Contains(key);
            }

            public virtual void CopyTo(Array array, int arrayIndex) {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException("Arg_RankMultiDimNotSupported");

                // defer error checking to Array.Copy
                Array.Copy(sortedList.keys, 0, array, arrayIndex, sortedList.Count);
            }

            public virtual void Insert(int index, Object value) {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
            }

            public virtual Object this[int index] {
                get {
                    return sortedList.GetKey(index);
                }
                set {
                    throw new NotSupportedException("NotSupported_KeyCollectionSet");
                }
            }

            public virtual IEnumerator GetEnumerator() {
                return new SortedListEnumerator(sortedList, 0, sortedList.Count, SortedListEnumerator.Keys);
            }

            public virtual int IndexOf(Object key) {
                if (key == null)
                    throw new ArgumentNullException("key", "ArgumentNull_Key");

                int i = Array.BinarySearch(sortedList.keys, 0,
                                           sortedList.Count, key, sortedList.comparer);
                if (i >= 0) return i;
                return -1;
            }

            public virtual void Remove(Object key) {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
            }

            public virtual void RemoveAt(int index) {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
            }
        }

        private class ValueList : IList
        {
            private SortedList sortedList;

            internal ValueList(SortedList sortedList) {
                this.sortedList = sortedList;
            }

            public virtual int Count {
                get { return sortedList._size; }
            }

            public virtual bool IsReadOnly {
                get { return true; }
            }

            public virtual bool IsFixedSize {
                get { return true; }
            }

            public virtual bool IsSynchronized {
                get { return sortedList.IsSynchronized; }
            }

            public virtual Object SyncRoot {
                get { return sortedList.SyncRoot; }
            }

            public virtual int Add(Object key) {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
            }

            public virtual void Clear() {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
            }

            public virtual bool Contains(Object value) {
                return sortedList.ContainsValue(value);
            }

            public virtual void CopyTo(Array array, int arrayIndex) {
                if (array != null && array.Rank != 1)
                    throw new ArgumentException("Arg_RankMultiDimNotSupported");

                // defer error checking to Array.Copy
                Array.Copy(sortedList.values, 0, array, arrayIndex, sortedList.Count);
            }

            public virtual void Insert(int index, Object value) {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
            }

            public virtual Object this[int index] {
                get {
                    return sortedList.GetByIndex(index);
                }
                set {
                    sortedList.SetByIndex(index,value);
                }
            }

            public virtual IEnumerator GetEnumerator() {
                return new SortedListEnumerator(sortedList, 0, sortedList.Count, SortedListEnumerator.Values);
            }

            public virtual int IndexOf(Object value) {
                return Array.IndexOf(sortedList.values, value, 0, sortedList.Count);
            }

            public virtual void Remove(Object value) {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
            }

            public virtual void RemoveAt(int index) {
                throw new NotSupportedException("NotSupported_SortedListNestedWrite");
            }

        }
    }
}
