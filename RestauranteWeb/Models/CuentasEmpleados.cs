//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RestauranteWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CuentasEmpleados
    {
        public string Usuario { get; set; }
        public Nullable<int> IdRol { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Clave { get; set; }
    
        public virtual RolesEmpleados RolesEmpleados { get; set; }
    }
}
