using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace NewCollections { 

    namespace MultiValueDictionary {
        class MultiValueDictionary<Tkey, Tvalue> {
            private Dictionary<Tkey, List<Tvalue>> _dictionary;

            public MultiValueDictionary() {
                _dictionary = new Dictionary<Tkey, List<Tvalue>>();
            }

            public void Add(Tkey key, Tvalue value) {
                if (!_dictionary.ContainsKey(key)) {
                    _dictionary.Add(key, new List<Tvalue>());
                }
                _dictionary[key].Add(value);
            }

            public Dictionary<Tkey, List<Tvalue>>.KeyCollection Keys {
                get {
                    return _dictionary.Keys;
                }
            }

            public int Count {
                get {
                    return _dictionary.Count;
                }
            }

            public List<Tvalue> this[Tkey key] {
                get {
                    return this._dictionary[key];
                }
                set {
                    _dictionary[key] = value;
                }
            }
        }
    } 
    
}