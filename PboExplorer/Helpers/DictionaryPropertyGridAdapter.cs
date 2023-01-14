using System.ComponentModel;
using PboExplorer.Interfaces;

namespace PboExplorer.Helpers;

// https://support.xceed.com/portal/en/community/topic/propertygrid-dictionary-not-displaying-values-using-icustomtypedescriptor
[RefreshProperties(RefreshProperties.All)]
public class DictionaryPropertyGridAdapter<T, U> : ICustomTypeDescriptor,
    IMetadata // TODO: Fix this
{
    private readonly IDictionary<T, U> dictionary;

    public DictionaryPropertyGridAdapter(IDictionary<T, U> dictionary)
    {
        this.dictionary = dictionary;
    }

    [Browsable(false)]
    public U this[T key]
    {
        get
        {
            return dictionary[key];
        }
        set
        {
            dictionary[key] = value;
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
        foreach (var e in dictionary)
        {
            properties.Add(new DictionaryPropertyDescriptor(dictionary, e.Key));
        }

        var props = properties.ToArray();

        return new PropertyDescriptorCollection(props);
    }

    public object GetPropertyOwner(PropertyDescriptor pd) => this;

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents() 
        => TypeDescriptor.GetEvents(this, true);

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        => ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);

    public class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        private readonly IDictionary<T, U> dictionary;
        private readonly T key;
       
        internal DictionaryPropertyDescriptor(IDictionary<T, U> dictionary, T key)
            : base(key.ToString(), null)
        {
            this.dictionary = dictionary;
            this.key = key;
        }

        public override Type ComponentType => null;

        public override bool IsReadOnly => false;

        public override Type PropertyType => dictionary[key].GetType();

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component) 
            => dictionary[key];

        public override void ResetValue(object component) { }

        public override void SetValue(object component, object value)
            => dictionary[key] = (U)value;

        public override bool ShouldSerializeValue(object component) => false;
    }
}