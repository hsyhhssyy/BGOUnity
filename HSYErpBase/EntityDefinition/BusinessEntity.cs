using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HSYErpBase.EntityDefinition
{
    /// <summary>
    /// BusinessEntity����MasterSlaveģʽ�Ĵ���1�Զ����Ե�ʵ��
    /// </summary>
    [DataContract]
    public class BusinessEntity : BasicEntity
    {
        [DataMember]
        public virtual List<BusinessEntityPropertyItem> Properties { get; set; } = new List<BusinessEntityPropertyItem>();

        public new virtual BusinessEntity Clone()
        {
            BusinessEntity obj = (BusinessEntity)this.MemberwiseClone();

            if (Properties != null)
            {
                obj.Properties = new List<BusinessEntityPropertyItem>();
                foreach (var pp in Properties)
                {
                    obj.Properties.Add(pp.Clone());
                }
            }

            return obj;
        }
    }
}