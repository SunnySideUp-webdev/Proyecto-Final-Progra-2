"""
Reportes e Integración
"""
import json
import os
from datetime import datetime


class Reportes:
    def __init__(self, gestor_clientes, gestor_ordenes, gestor_inventario, ruta_datos="datos"):
        self.clientes = gestor_clientes
        self.ordenes = gestor_ordenes
        self.inventario = gestor_inventario
        self.ruta_datos = ruta_datos
        os.makedirs(self.ruta_datos, exist_ok=True)

    # REPORTE 1: ÓRDENES ABIERTAS Y CERRADAS
    def reporte_ordenes(self, estado=None):
        todas = self.ordenes.listar_ordenes()
        abiertas = [o for o in todas if str(o.get("estado", "")).lower() == "abierta"]
        cerradas = [o for o in todas if str(o.get("estado", "")).lower() == "cerrada"]

        reporte = {
            "titulo": "REPORTE DE ÓRDENES DE REPARACIÓN",
            "fecha_generacion": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
            "total_ordenes": len(todas),
            "total_abiertas": len(abiertas),
            "total_cerradas": len(cerradas),
        }
        if estado == "abierta":
            reporte["detalle"] = abiertas
        elif estado == "cerrada":
            reporte["detalle"] = cerradas
        else:
            reporte["detalle_abiertas"] = abiertas
            reporte["detalle_cerradas"] = cerradas

        self._imprimir_reporte(reporte)
        return reporte

    # REPORTE 2: REPARACIONES POR VEHÍCULO
    def reporte_reparaciones_por_vehiculo(self, placa_o_vin=None):
        vehiculos = self.clientes.listar_vehiculos()
        ordenes = self.ordenes.listar_ordenes()

        reporte = {
            "titulo": "REPORTE DE REPARACIONES POR VEHÍCULO",
            "fecha_generacion": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
            "vehiculos": []
        }
        for v in vehiculos:
            placa, vin = v.get("placa"), v.get("vin")
            if placa_o_vin and placa_o_vin not in (placa, vin):
                continue
            ordenes_veh = [o for o in ordenes if o.get("placa") == placa or o.get("vin") == vin]
            cliente = self.clientes.buscar_cliente(v.get("id_cliente"))
            
            reporte["vehiculos"].append({
                "placa": placa, "vin": vin,
                "marca": v.get("marca"), "modelo": v.get("modelo"),
                "anio": v.get("anio"),
                "cliente": cliente.get("nombre") if cliente else "N/D",
                "total_reparaciones": len(ordenes_veh),
                "ordenes": [{
                    "id_orden": o.get("id_orden"), "fecha_ingreso": o.get("fecha_ingreso"),
                    "motivo": o.get("motivo"), "diagnostico": o.get("diagnostico"),
                    "estado": o.get("estado"), "fecha_cierre": o.get("fecha_cierre"),
                } for o in ordenes_veh]
            })
        self._imprimir_reporte(reporte)
        return reporte

    # REPORTE 3: REPUESTOS UTILIZADOS
    def reporte_repuestos_utilizados(self, id_orden=None):
        ordenes = self.ordenes.listar_ordenes()
        reporte = {
            "titulo": "REPORTE DE REPUESTOS UTILIZADOS",
            "fecha_generacion": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
            "detalle": [], "totales_por_repuesto": {}, "valor_total_utilizado": 0.0
        }
        totales = {}
        for o in ordenes:
            if id_orden and o.get("id_orden") != id_orden:
                continue
            for r in o.get("repuestos", []):
                codigo = r.get("codigo")
                cantidad = r.get("cantidad", 0)
                precio = r.get("precio_unitario", 0)
                subtotal = cantidad * precio
                totales[codigo] = totales.get(codigo, 0) + cantidad
                reporte["valor_total_utilizado"] += subtotal
                reporte["detalle"].append({
                    "id_orden": o.get("id_orden"), "placa": o.get("placa"),
                    "codigo": codigo, "nombre": r.get("nombre"),
                    "cantidad": cantidad, "precio_unitario": precio, "subtotal": subtotal,
                })
        reporte["totales_por_repuesto"] = totales
        self._imprimir_reporte(reporte)
        return reporte

    # REPORTE 4: EXISTENCIAS DEL INVENTARIO
    def reporte_existencias(self, solo_bajos=False, umbral=5):
        repuestos = self.inventario.listar_repuestos()
        reporte = {
            "titulo": "REPORTE DE EXISTENCIAS DEL INVENTARIO",
            "fecha_generacion": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
            "umbral_stock_bajo": umbral, "total_repuestos": 0,
            "repuestos_stock_bajo": 0, "valor_total_inventario": 0.0, "detalle": []
        }
        for r in repuestos:
            existencia = r.get("existencia", 0)
            precio = r.get("precio_unitario", 0)
            valor = existencia * precio
            if solo_bajos and existencia > umbral:
                continue
            reporte["detalle"].append({
                "codigo": r.get("codigo"), "nombre": r.get("nombre"),
                "existencia": existencia, "stock_bajo": existencia <= umbral,
                "precio_unitario": precio, "valor_total": valor,
            })
            reporte["total_repuestos"] += 1
            reporte["valor_total_inventario"] += valor
            if existencia <= umbral:
                reporte["repuestos_stock_bajo"] += 1
        self._imprimir_reporte(reporte)
        return reporte

    # UTILIDADES
    def _imprimir_reporte(self, reporte):
        print("\n" + "=" * 70)
        print(f" {reporte.get('titulo', 'REPORTE')}")
        print("=" * 70)
        print(f" Fecha: {reporte.get('fecha_generacion')}")
        print("-" * 70)
        for clave, valor in reporte.items():
            if clave in ("titulo", "fecha_generacion", "detalle", "detalle_abiertas", "detalle_cerradas", "vehiculos"):
                continue
            print(f" {clave}: {valor}")
        
        if "detalle" in reporte and isinstance(reporte["detalle"], list):
            print("\n DETALLE:")
            for item in reporte["detalle"]:
                print(f"   - {item}")
        
        for clave in ("detalle_abiertas", "detalle_cerradas"):
            if clave in reporte:
                print(f"\n {clave.upper().replace('_', ' ')}:")
                for item in reporte[clave]:
                    print(f"   - ID: {item.get('id_orden')} | Placa: {item.get('placa')} | Motivo: {item.get('motivo')} | Estado: {item.get('estado')}")
        
        if "vehiculos" in reporte:
            print("\n DETALLE POR VEHÍCULO:")
            for v in reporte["vehiculos"]:
                print(f"   Vehículo: {v['placa']} ({v['marca']} {v['modelo']} {v['anio']})")
                print(f"     Cliente: {v['cliente']}")
                print(f"     Total reparaciones: {v['total_reparaciones']}")
                for o in v["ordenes"]:
                    print(f"       * Orden {o['id_orden']} | Ingreso: {o['fecha_ingreso']} | Estado: {o['estado']}")
        print("=" * 70 + "\n")

    def exportar_reporte_json(self, reporte, nombre_archivo):
        ruta = os.path.join(self.ruta_datos, nombre_archivo)
        with open(ruta, "w", encoding="utf-8") as f:
            json.dump(reporte, f, indent=4, ensure_ascii=False)
        print(f" Reporte exportado a: {ruta}")
        return ruta
