using System;
using System.IO;
using DocuTrack.Model;
using DocuTrack.View;

namespace DocuTrack.Controller
{
    public class DocuTrackController
    {
        // Árbol binario que representa la estructura de documentos/carpetas
        private readonly ArbolBinario _arbol;
        // Vista responsable de toda la salida por consola
        private readonly DocuTrackView _vista;

        public DocuTrackController()
        {
            // Crear instancias del modelo (BST) y la vista (consola)
            _arbol = new ArbolBinario();
            _vista = new DocuTrackView();
        }

        private bool EjecutarCasosDesdeArchivo()
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "data", "casos.txt");
                if (!File.Exists(path)) return false;

                _vista.MostrarEncabezado("Ejecución de Casos desde archivo");
                foreach (var raw in File.ReadAllLines(path))
                {
                    var line = raw.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
                    var parts = line.Split(';');
                    var accion = parts[0].Trim().ToUpperInvariant();
                    switch (accion)
                    {
                        case "INSERTAR":
                            if (parts.Length >= 3)
                            {
                                var nombre = parts[1].Trim();
                                var flag = parts[2].Trim().ToLowerInvariant();
                                bool esCarpeta = flag == "true" || flag == "1" || flag == "si" || flag == "sí" || flag == "y" || flag == "yes";
                                var r = _arbol.Insertar(nombre, esCarpeta);
                                _vista.MostrarResultadoInsercion(nombre, r.insertado, r.comparaciones, r.motivo);
                                _vista.MostrarArbolAscii(_arbol.Raiz);
                            }
                            break;
                        case "BUSCAR":
                            if (parts.Length >= 2)
                            {
                                var nombre = parts[1].Trim();
                                var (n, c) = _arbol.Buscar(nombre);
                                _vista.MostrarResultadoBusqueda(nombre, n != null, c);
                            }
                            break;
                        case "ACTUALIZAR":
                            if (parts.Length >= 4)
                            {
                                var antiguo = parts[1].Trim();
                                var nuevo = parts[2].Trim();
                                var flag = parts[3].Trim().ToLowerInvariant();
                                bool esCarpeta = flag == "true" || flag == "1" || flag == "si" || flag == "sí" || flag == "y" || flag == "yes";
                                ActualizarNodo(antiguo, nuevo, esCarpeta);
                                _vista.MostrarArbolAscii(_arbol.Raiz);
                            }
                            break;
                        case "ELIMINAR":
                            if (parts.Length >= 2)
                            {
                                var nombre = parts[1].Trim();
                                var r = _arbol.EliminarConCaso(nombre);
                                _vista.MostrarResultadoEliminacion(nombre, r.eliminado, $"(Caso {r.caso})");
                                _vista.MostrarArbolAscii(_arbol.Raiz);
                            }
                            break;
                        default:
                            _vista.MostrarMensaje($"Acción desconocida: {accion}");
                            break;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Ejecutar()
        {
            // Encabezado inicial
            _vista.MostrarEncabezado("Sistema de Gestión DocuTrack S.A");

            // 1. Construcción del árbol
            ConstruirArbol();

            // Si hay un archivo de casos, se ejecuta y se omite el flujo demo
            if (EjecutarCasosDesdeArchivo())
            {
                // 5. Recorridos de verificación
                RealizarRecorridos();
                // 6. Métricas finales
                MostrarMetricasFinales();
                return;
            }

            // 2. Búsquedas rápidas (flujo demo)
            RealizarBusquedas();

            // 3. Actualizaciones selectivas (flujo demo)
            RealizarActualizaciones();

            // 4. Eliminaciones selectivas (flujo demo)
            RealizarEliminaciones();

            // 5. Recorridos de verificación
            RealizarRecorridos();

            // 6. Métricas finales
            MostrarMetricasFinales();
        }

        private void ConstruirArbol()
        {
            // Carga inicial de elementos: desde archivo si existe, o valores por defecto
            _vista.MostrarEncabezado("Construcción del Árbol");

            var elementos = CargarElementosDesdeArchivo()
                ?? new System.Collections.Generic.List<(string Nombre, bool EsCarpeta)>
                {
                    ("Proyectos", true),
                    ("Documentacion", true),
                    ("Src", true),
                    ("Tests", true),
                    ("Readme.txt", false),
                    ("Program.cs", false),
                    ("Api", true),
                    ("Client", true),
                    ("Models", true),
                    ("Services", true),
                    ("UnitTests", true),
                    ("IntegrationTests", true),
                    ("Design", true),
                    ("App.config", false)
                };

            // Insertar cada elemento en el BST mostrando comparaciones realizadas
            foreach (var e in elementos)
            {
                var (insertado, comps, motivo) = _arbol.Insertar(e.Nombre, e.EsCarpeta);
                _vista.MostrarResultadoInsercion(e.Nombre, insertado, comps, motivo);
            }

            // Pintar el árbol en ASCII y por niveles
            _vista.MostrarArbolAscii(_arbol.Raiz);
            _vista.MostrarArbol(_arbol.Raiz);

            // Casos extra: intentos de inserción duplicada
            _vista.MostrarMensaje("\n--- Inserciones Duplicadas ---");
            var dup1 = _arbol.Insertar("Services", true);
            _vista.MostrarResultadoInsercion("Services", dup1.insertado, dup1.comparaciones, dup1.motivo);
            var dup2 = _arbol.Insertar("Client", true);
            _vista.MostrarResultadoInsercion("Client", dup2.insertado, dup2.comparaciones, dup2.motivo);
        }

        private System.Collections.Generic.List<(string Nombre, bool EsCarpeta)>? CargarElementosDesdeArchivo()
        {
            // Lee data/elementos.txt con formato: Nombre;EsCarpeta (true/false/1/si/yes)
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "data", "elementos.txt");
                if (!File.Exists(path)) return null;

                var lista = new System.Collections.Generic.List<(string Nombre, bool EsCarpeta)>();
                foreach (var raw in File.ReadAllLines(path))
                {
                    var line = raw.Trim();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
                    var parts = line.Split(';');
                    if (parts.Length < 2) continue;
                    var nombre = parts[0].Trim();
                    var flag = parts[1].Trim().ToLowerInvariant();
                    bool esCarpeta = flag == "true" || flag == "1" || flag == "si" || flag == "sí" || flag == "y" || flag == "yes";
                    if (!string.IsNullOrEmpty(nombre))
                    {
                        lista.Add((nombre, esCarpeta));
                    }
                }
                return lista.Count > 0 ? lista : null;
            }
            catch
            {
                // Si hay algún error de lectura/formato devolvemos null para usar los valores por defecto
                return null;
            }
        }

        private void RealizarBusquedas()
        {
            // Demostración de búsquedas en diferentes zonas del árbol y elementos inexistentes
            _vista.MostrarEncabezado("Búsquedas Rápidas");

            _vista.MostrarMensaje("\n--- Subárbol Izquierdo ---");
            RealizarBusqueda("Documentacion");
            RealizarBusqueda("Api");

            _vista.MostrarMensaje("\n--- Subárbol Derecho ---");
            RealizarBusqueda("Tests");
            RealizarBusqueda("Client");

            _vista.MostrarMensaje("\n--- Búsquedas Inexistentes ---");
            RealizarBusqueda("NoExisto");
            RealizarBusqueda("ArchivoPerdido");
        }

        private void RealizarBusqueda(string nombre)
        {
            // Busca un nombre y muestra si fue encontrado junto con el número de comparaciones
            var (nodo, comps) = _arbol.Buscar(nombre);
            _vista.MostrarResultadoBusqueda(nombre, nodo != null, comps);
        }

        private void RealizarActualizaciones()
        {
            // Actualiza nombres: hoja, nodo con un hijo y la raíz
            _vista.MostrarEncabezado("Actualizaciones Selectivas");

            _vista.MostrarMensaje("\n--- Actualización de Hoja ---");
            ActualizarNodo("Readme.txt", "LEEME.txt", false);
            _vista.MostrarArbolAscii(_arbol.Raiz);
            _vista.MostrarArbol(_arbol.Raiz);

            _vista.MostrarMensaje("\n--- Actualización de Nodo con Un Hijo ---");
            ActualizarNodo("Documentacion", "Documentation", true);
            _vista.MostrarArbolAscii(_arbol.Raiz);
            _vista.MostrarArbol(_arbol.Raiz);

            _vista.MostrarMensaje("\n--- Actualización de Raíz ---");
            ActualizarNodo("Proyectos", "MainProject", true);
            _vista.MostrarArbolAscii(_arbol.Raiz);
            _vista.MostrarArbol(_arbol.Raiz);

            // Verificaciones: los nombres antiguos ya no deben existir y los nuevos sí
            _vista.MostrarMensaje("\n--- Verificación de Actualizaciones ---");
            var chk1 = _arbol.Buscar("Readme.txt");
            _vista.MostrarResultadoBusqueda("Readme.txt", chk1.nodo != null, chk1.comparaciones);
            var chk2 = _arbol.Buscar("LEEME.txt");
            _vista.MostrarResultadoBusqueda("LEEME.txt", chk2.nodo != null, chk2.comparaciones);
            var chk3 = _arbol.Buscar("Documentacion");
            _vista.MostrarResultadoBusqueda("Documentacion", chk3.nodo != null, chk3.comparaciones);
            var chk4 = _arbol.Buscar("Documentation");
            _vista.MostrarResultadoBusqueda("Documentation", chk4.nodo != null, chk4.comparaciones);
            var chk5 = _arbol.Buscar("Proyectos");
            _vista.MostrarResultadoBusqueda("Proyectos", chk5.nodo != null, chk5.comparaciones);
            var chk6 = _arbol.Buscar("MainProject");
            _vista.MostrarResultadoBusqueda("MainProject", chk6.nodo != null, chk6.comparaciones);
        }

        private void ActualizarNodo(string nombreAntiguo, string nombreNuevo, bool esCarpeta)
        {
            // Validar existencia del antiguo y ausencia del nuevo
            var (nodoAntiguo, _) = _arbol.Buscar(nombreAntiguo);
            if (nodoAntiguo == null)
            {
                _vista.MostrarMensaje($"✗ No se pudo actualizar: '{nombreAntiguo}' no encontrado");
                return;
            }

            var (nodoNuevo, _) = _arbol.Buscar(nombreNuevo);
            if (nodoNuevo != null)
            {
                _vista.MostrarMensaje($"✗ No se pudo actualizar: '{nombreNuevo}' ya existe");
                return;
            }

            // Estrategia simple de actualización: eliminar e insertar con el nuevo nombre
            _arbol.Eliminar(nombreAntiguo);
            _ = _arbol.Insertar(nombreNuevo, esCarpeta);
            _vista.MostrarMensaje($"✓ Actualizado: '{nombreAntiguo}' → '{nombreNuevo}'");
        }

        private void RealizarEliminaciones()
        {
            // Eliminaciones representativas: hoja, un hijo y raíz con dos hijos
            _vista.MostrarEncabezado("Eliminaciones Selectivas");

            _vista.MostrarMensaje("\n--- Eliminación de Hoja ---");
            var elimHoja = _arbol.EliminarConCaso("App.config");
            _vista.MostrarResultadoEliminacion("App.config", elimHoja.eliminado, $"(Caso {elimHoja.caso})");
            _vista.MostrarArbolAscii(_arbol.Raiz);
            _vista.MostrarArbol(_arbol.Raiz);

            _vista.MostrarMensaje("\n--- Eliminación de Nodo con Un Hijo ---");
            var elimUnHijo = _arbol.EliminarConCaso("Api");
            _vista.MostrarResultadoEliminacion("Api", elimUnHijo.eliminado, $"(Caso {elimUnHijo.caso})");
            _vista.MostrarArbolAscii(_arbol.Raiz);
            _vista.MostrarArbol(_arbol.Raiz);

            _vista.MostrarMensaje("\n--- Eliminación de Raíz ---");
            var elimRaiz = _arbol.EliminarConCaso("MainProject");
            _vista.MostrarResultadoEliminacion("MainProject", elimRaiz.eliminado, $"(Caso {elimRaiz.caso})");
            _vista.MostrarArbolAscii(_arbol.Raiz);
            _vista.MostrarArbol(_arbol.Raiz);

            // Eliminaciones de elementos inexistentes y verificación posterior
            _vista.MostrarMensaje("\n--- Eliminaciones Inexistentes ---");
            var elimNoExiste1 = _arbol.Eliminar("NoExisto");
            _vista.MostrarResultadoEliminacion("NoExisto", elimNoExiste1, "(No existe)");
            var elimNoExiste2 = _arbol.Eliminar("ArchivoPerdido");
            _vista.MostrarResultadoEliminacion("ArchivoPerdido", elimNoExiste2, "(No existe)");

            _vista.MostrarMensaje("\n--- Verificación Tras Eliminaciones ---");
            var v1 = _arbol.Buscar("App.config");
            _vista.MostrarResultadoBusqueda("App.config", v1.nodo != null, v1.comparaciones);
            var v2 = _arbol.Buscar("Api");
            _vista.MostrarResultadoBusqueda("Api", v2.nodo != null, v2.comparaciones);
            var v3 = _arbol.Buscar("MainProject");
            _vista.MostrarResultadoBusqueda("MainProject", v3.nodo != null, v3.comparaciones);
        }

        private void RealizarRecorridos()
        {
            // Muestra los cuatro recorridos comunes y permiten verificar el orden del BST
            _vista.MostrarEncabezado("Recorridos de Verificación");
            _vista.MostrarRecorrido(_arbol.Preorden(), "Preorden");
            _vista.MostrarRecorrido(_arbol.Inorden(), "Inorden");
            _vista.MostrarRecorrido(_arbol.Postorden(), "Postorden");
            _vista.MostrarRecorrido(_arbol.PorNiveles(), "Por Niveles");
        }

        private void MostrarMetricasFinales()
        {
            // Por ahora mostramos solo la altura del árbol (profundidad máxima)
            _vista.MostrarEncabezado("Métricas Finales");
            _vista.MostrarAltura(_arbol.CalcularAltura());
            _vista.MostrarMensaje("\n¡Proceso completado exitosamente!");
        }
    }
}
