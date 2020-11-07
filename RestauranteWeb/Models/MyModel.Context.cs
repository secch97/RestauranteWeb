﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    public partial class ProyectoASP_RestauranteEntities : DbContext
    {
        public ProyectoASP_RestauranteEntities()
            : base("name=ProyectoASP_RestauranteEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CategoriasProductos> CategoriasProductos { get; set; }
        public virtual DbSet<CuentasClientes> CuentasClientes { get; set; }
        public virtual DbSet<CuentasEmpleados> CuentasEmpleados { get; set; }
        public virtual DbSet<EstadosProductos> EstadosProductos { get; set; }
        public virtual DbSet<EtapasPedidos> EtapasPedidos { get; set; }
        public virtual DbSet<HistorialPedidos> HistorialPedidos { get; set; }
        public virtual DbSet<OfertasProductos> OfertasProductos { get; set; }
        public virtual DbSet<PedidosClientes> PedidosClientes { get; set; }
        public virtual DbSet<PedidosClientesDetalles> PedidosClientesDetalles { get; set; }
        public virtual DbSet<ProductosRestaurante> ProductosRestaurante { get; set; }
        public virtual DbSet<RolesEmpleados> RolesEmpleados { get; set; }
        public virtual DbSet<TrackeoPedidosClientes> TrackeoPedidosClientes { get; set; }
        public virtual DbSet<Combos> Combos { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<CombosDetalle> CombosDetalle { get; set; }
    }

    public partial class ProyectoASP_RestauranteEntities
    {
        [DbFunction("ProyectoASP_RestauranteModel.Store", "GeneradorIdObjetos")]
        public string GeneradorIdOjetos(string prefijo)
        {
            var objectContext = ((IObjectContextAdapter)this).ObjectContext;

            var parameters = new List<ObjectParameter>();
            parameters.Add(new ObjectParameter("Prefijo", prefijo));

            return objectContext.CreateQuery<string>("ProyectoASP_RestauranteModel.Store.GeneradorIdObjetos(@Prefijo)", parameters.ToArray())
                 .Execute(MergeOption.NoTracking)
                 .FirstOrDefault();
        }
    }
}
