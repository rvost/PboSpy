using System.ComponentModel;

namespace PboSpy.Modules.Metadata.Utils;

// https://support.xceed.com/portal/en/community/topic/propertygrid-dictionary-not-displaying-values-using-icustomtypedescriptor
[RefreshProperties(RefreshProperties.All)]
class DictionaryPropertyGridAdapter<T, U> : ICustomTypeDescriptor
{
    private readonly IDictionary<T, U> _dictionary;

    public DictionaryPropertyGridAdapter(IDictionary<T, U> dictionary)
    {
        _dictionary = dictionary;
    }

    [Browsable(false)]
    public U this[T key]
    {
        get
        {
            return _dictionary[key];
        }
        set
        {
            _dictionary[key] = value;
        }
    }

    public AttributeCollection GetAttributes()
        => TypeDescriptor.GetAttributes(this, true);

    public string GetClassName()
        => TypeDescriptor.GetClassName(this, true);

    public string GetComponentName()
        => TypeDescriptor.GetComponentName(this, true);

    public TypeConverter GetConverter()
        => TypeDescriptor.GetConverter(this, true);

    public EventDescriptor GetDefaultEvent()
        => TypeDescriptor.GetDefaultEvent(this, true);

    public PropertyDescriptor GetDefaultProperty() => null;

    public object GetEditor(Type editorBaseType)
        => TypeDescriptor.GetEditor(this, editorBaseType, true);

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
        => TypeDescriptor.GetEvents(this, attributes, true);

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
        var properties = new List<DictionaryPropertyDescriptor>();
        foreach (var e in _dictionary)
        {
            properties.Add(new DictionaryPropertyDescriptor(_dictionary, e.Key));
        }

        var props = properties.ToArray();

        return new PropertyDescriptorCollection(props);
    }

    public object GetPropertyOwner(PropertyDescriptor pd) => this;

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        => TypeDescriptor.GetEvents(this, true);

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        => ((ICustomTypeDescriptor)this).GetProperties(Array.Empty<Attribute>());

    private class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        private readonly IDictionary<T, U> _dictionary;
        private readonly T _key;

        internal DictionaryPropertyDescriptor(IDictionary<T, U> dictionary, T key)
            : base(key.ToString(), null)
        {
            _dictionary = dictionary;
            _key = key;
        }

        public override Type ComponentType => null;

        public override bool IsReadOnly => false;

        public override Type PropertyType => _dictionary[_key].GetType();

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component)
            => _dictionary[_key];

        public override void ResetValue(object component) { }

        public override void SetValue(object component, object value)
            => _dictionary[_key] = (U)value;

        public override bool ShouldSerializeValue(object component) => false;
    }
}
