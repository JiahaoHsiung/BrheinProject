
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------


namespace WeChat
{

using System;
    using System.Collections.Generic;
    
public partial class iot_gateway
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public iot_gateway()
    {

        this.iot_blank_position = new HashSet<iot_blank_position>();

        this.iot_control_panel = new HashSet<iot_control_panel>();

        this.iot_elebox = new HashSet<iot_elebox>();

        this.iot_scene = new HashSet<iot_scene>();

        this.iot_scene_panel = new HashSet<iot_scene_panel>();

        this.iot_user = new HashSet<iot_user>();

    }


    public int ID { get; set; }

    public string MAC { get; set; }

    public string Name { get; set; }

    public Nullable<System.DateTime> CreateTime { get; set; }

    public Nullable<System.DateTime> EditTIme { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<iot_blank_position> iot_blank_position { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<iot_control_panel> iot_control_panel { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<iot_elebox> iot_elebox { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<iot_scene> iot_scene { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<iot_scene_panel> iot_scene_panel { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<iot_user> iot_user { get; set; }

}

}
