
using Microsoft.EntityFrameworkCore;
using CapaEntidadesNT;

namespace CapaAccesoDatosNT
{
    public class clsAutoManagerContextNT : DbContext
    {
        public clsAutoManagerContextNT(
            DbContextOptions<clsAutoManagerContextNT> pObjOpcionesNT)
            : base(pObjOpcionesNT)
        {
        }

        public DbSet<clsOrdenReparacionNT> OrdenesReparacion
            => Set<clsOrdenReparacionNT>();

        protected override void OnModelCreating(
            ModelBuilder pObjModeloNT)
        {
            base.OnModelCreating(pObjModeloNT);

            pObjModeloNT.Entity<clsOrdenReparacionNT>(
                pObjEntidadNT =>
                {
                    pObjEntidadNT.ToTable("OrdenesReparacion");

                    pObjEntidadNT.HasKey(
                        pObjOrdenNT => pObjOrdenNT.IdOrden);

                    pObjEntidadNT.Property(
                        pObjOrdenNT => pObjOrdenNT.VIN)
                        .HasColumnType("varchar(17)");

                    pObjEntidadNT.Property(pObjOrdenNT => pObjOrdenNT.Placa)
                        .HasColumnType("varchar(20)")
                        .IsRequired(false);


                    pObjEntidadNT.Property(
                        pObjOrdenNT => pObjOrdenNT.MotivoIngreso)
                        .HasMaxLength(500);

                    pObjEntidadNT.Property(
                        pObjOrdenNT => pObjOrdenNT.Diagnostico)
                        .HasMaxLength(1000);

                    pObjEntidadNT.Property(
                        pObjOrdenNT => pObjOrdenNT.ObservacionesCierre)
                        .HasMaxLength(1000);
                });
        }
    }
}
