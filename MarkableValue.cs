namespace NewValues {

    namespace MarkableValue {
        class MarkableValue<T> {
            public MarkableValue(T element, bool marked = false) {
                this.value = element;
                this._marked = marked;
            }

            public void SetMark() {
                this._marked = true;
            }

            public void ResetMark() {
                this._marked = false;
            }

            public void ToggleMark() {
                this._marked = !_marked;
            }

            private bool _marked;
            public bool Marked {
                get {
                    return this._marked;
                }
            }
            public T value;
        };
    }
    
}