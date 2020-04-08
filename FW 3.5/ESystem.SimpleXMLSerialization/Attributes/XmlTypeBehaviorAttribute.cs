using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace ESystem.SimpleXmlSerialization.Attributes
{
  [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct)]
  public class XmlTypeBehaviorAttribute : Attribute
  {
    /// <summary>
    /// For IList instances, this defines XMLElement under which one item of list is stored in XML file.
    /// </summary>
    public string IListItemXmlElement { get; set; }
    /// <summary>
    /// Type of which the instance will be created. Used when property is typed as interface, so the target instance type cannot be inherited. This type must have public parameterless constructor.
    /// </summary>
    public Type DefaultInstanceType { get; set; }
    /// <summary>
    /// For IList instances, defines type of one item used when item is created. This type must have public parameterless constructor.
    /// </summary>
    public Type DefaultIListItemType { get; set; }

    public XmlTypeBehaviorAttribute()
    {
      this.IListItemXmlElement = "Item";
      this.DefaultInstanceType = null;
      this.DefaultIListItemType = null;
    }

    public void DoValidityTest()
    { }
  }
}
