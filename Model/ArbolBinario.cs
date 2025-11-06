using System;
using System.Collections.Generic;

namespace DocuTrack.Model
{
    // Árbol Binario de Búsqueda (BST) para organizar nombres de archivos/carpetas
    public class ArbolBinario
    {
        // Raíz del árbol (puede ser nula si el árbol está vacío)
        public Nodo? Raiz { get; private set; }

        // Inserta un nuevo nombre en el BST. Devuelve si se insertó, comparaciones y motivo en caso de rechazo
        public (bool insertado, int comparaciones, string motivo) Insertar(string nombre, bool esCarpeta)
        {
            int comps = 0;
            if (Raiz == null)
            {
                // Caso base: árbol vacío
                Raiz = new Nodo(nombre, esCarpeta);
                return (true, 1, string.Empty);
            }
            // Recorremos comparando hasta encontrar posición libre
            var actual = Raiz;
            while (true)
            {
                comps++;
                int cmp = string.Compare(nombre, actual.Nombre, StringComparison.OrdinalIgnoreCase);
                if (cmp == 0)
                {
                    // Duplicado: no insertamos
                    return (false, comps, "Duplicado");
                }
                else if (cmp < 0)
                {
                    if (actual.Izquierdo == null)
                    {
                        // Enforce: un archivo (no carpeta) no puede tener hijos
                        if (!actual.EsCarpeta)
                        {
                            return (false, comps, "Regla: archivo es hoja");
                        }
                        // Insertamos en el lado izquierdo
                        actual.Izquierdo = new Nodo(nombre, esCarpeta);
                        return (true, comps, string.Empty);
                    }
                    actual = actual.Izquierdo;
                }
                else
                {
                    if (actual.Derecho == null)
                    {
                        // Enforce: un archivo (no carpeta) no puede tener hijos
                        if (!actual.EsCarpeta)
                        {
                            return (false, comps, "Regla: archivo es hoja");
                        }
                        // Insertamos en el lado derecho
                        actual.Derecho = new Nodo(nombre, esCarpeta);
                        return (true, comps, string.Empty);
                    }
                    actual = actual.Derecho;
                }
            }
        }

        // Busca un nombre y devuelve el nodo (o null) junto con comparaciones realizadas
        public (Nodo? nodo, int comparaciones) Buscar(string nombre)
        {
            int comps = 0;
            var actual = Raiz;
            while (actual != null)
            {
                comps++;
                int cmp = string.Compare(nombre, actual.Nombre, StringComparison.OrdinalIgnoreCase);
                if (cmp == 0) return (actual, comps);
                actual = (cmp < 0) ? actual.Izquierdo : actual.Derecho;
            }
            return (null, comps);
        }

        // Elimina un nombre del BST. Devuelve true si se eliminó
        public bool Eliminar(string nombre)
        {
            bool eliminado;
            Raiz = EliminarRec(Raiz, nombre, out eliminado);
            return eliminado;
        }

        // Variante que indica el caso ejecutado: "Hoja", "Un Hijo", "Dos Hijos" o "No encontrado"
        public (bool eliminado, string caso) EliminarConCaso(string nombre)
        {
            bool eliminado;
            string caso;
            Raiz = EliminarRecConCaso(Raiz, nombre, out eliminado, out caso);
            return (eliminado, caso);
        }

        // Eliminación recursiva: maneja casos hoja, un hijo y dos hijos
        private Nodo? EliminarRec(Nodo? raiz, string nombre, out bool eliminado)
        {
            if (raiz == null)
            {
                eliminado = false;
                return null;
            }
            int cmp = string.Compare(nombre, raiz.Nombre, StringComparison.OrdinalIgnoreCase);
            if (cmp < 0)
            {
                // Buscamos en el lado izquierdo
                raiz.Izquierdo = EliminarRec(raiz.Izquierdo, nombre, out eliminado);
                return raiz;
            }
            if (cmp > 0)
            {
                // Buscamos en el lado derecho
                raiz.Derecho = EliminarRec(raiz.Derecho, nombre, out eliminado);
                return raiz;
            }
            // Encontrado
            eliminado = true;
            if (raiz.Izquierdo == null) return raiz.Derecho;
            if (raiz.Derecho == null) return raiz.Izquierdo;
            // Dos hijos: reemplazar por el sucesor en inorden (mínimo del subárbol derecho)
            var sucesor = Min(raiz.Derecho!);
            raiz.Nombre = sucesor.Nombre;
            raiz.EsCarpeta = sucesor.EsCarpeta;
            raiz.Derecho = EliminarRec(raiz.Derecho, sucesor.Nombre, out _);
            return raiz;
        }

        private Nodo? EliminarRecConCaso(Nodo? raiz, string nombre, out bool eliminado, out string caso)
        {
            if (raiz == null)
            {
                eliminado = false;
                caso = "No encontrado";
                return null;
            }
            int cmp = string.Compare(nombre, raiz.Nombre, StringComparison.OrdinalIgnoreCase);
            if (cmp < 0)
            {
                raiz.Izquierdo = EliminarRecConCaso(raiz.Izquierdo, nombre, out eliminado, out caso);
                return raiz;
            }
            if (cmp > 0)
            {
                raiz.Derecho = EliminarRecConCaso(raiz.Derecho, nombre, out eliminado, out caso);
                return raiz;
            }
            // Encontrado
            eliminado = true;
            if (raiz.Izquierdo == null && raiz.Derecho == null)
            {
                caso = "Hoja";
                return null;
            }
            if (raiz.Izquierdo == null)
            {
                caso = "Un Hijo";
                return raiz.Derecho;
            }
            if (raiz.Derecho == null)
            {
                caso = "Un Hijo";
                return raiz.Izquierdo;
            }
            // Dos hijos
            caso = "Dos Hijos";
            var sucesor = Min(raiz.Derecho!);
            raiz.Nombre = sucesor.Nombre;
            raiz.EsCarpeta = sucesor.EsCarpeta;
            raiz.Derecho = EliminarRecConCaso(raiz.Derecho, sucesor.Nombre, out _, out _);
            return raiz;
        }

        // Retorna el nodo con el valor mínimo a partir de n (ir al extremo izquierdo)
        private Nodo Min(Nodo n)
        {
            while (n.Izquierdo != null) n = n.Izquierdo;
            return n;
        }

        // Recorrido en preorden: nodo, izquierdo, derecho
        public IEnumerable<string> Preorden()
        {
            var res = new List<string>();
            PreordenRec(Raiz, res);
            return res;
        }
        private void PreordenRec(Nodo? n, List<string> res)
        {
            if (n == null) return;
            res.Add(n.Nombre);
            PreordenRec(n.Izquierdo, res);
            PreordenRec(n.Derecho, res);
        }

        // Recorrido en inorden: izquierdo, nodo, derecho (entrega orden alfabético)
        public IEnumerable<string> Inorden()
        {
            var res = new List<string>();
            InordenRec(Raiz, res);
            return res;
        }
        private void InordenRec(Nodo? n, List<string> res)
        {
            if (n == null) return;
            InordenRec(n.Izquierdo, res);
            res.Add(n.Nombre);
            InordenRec(n.Derecho, res);
        }

        // Recorrido en inorden descendente: derecho, nodo, izquierdo (orden alfabético descendente)
        public IEnumerable<string> InordenDesc()
        {
            var res = new List<string>();
            InordenDescRec(Raiz, res);
            return res;
        }
        private void InordenDescRec(Nodo? n, List<string> res)
        {
            if (n == null) return;
            InordenDescRec(n.Derecho, res);
            res.Add(n.Nombre);
            InordenDescRec(n.Izquierdo, res);
        }

        // Recorrido en postorden: izquierdo, derecho, nodo
        public IEnumerable<string> Postorden()
        {
            var res = new List<string>();
            PostordenRec(Raiz, res);
            return res;
        }
        private void PostordenRec(Nodo? n, List<string> res)
        {
            if (n == null) return;
            PostordenRec(n.Izquierdo, res);
            PostordenRec(n.Derecho, res);
            res.Add(n.Nombre);
        }

        // Recorrido por niveles (BFS) usando una cola
        public IEnumerable<string> PorNiveles()
        {
            var res = new List<string>();
            if (Raiz == null) return res;
            var q = new Queue<Nodo>();
            q.Enqueue(Raiz);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                res.Add(n.Nombre);
                if (n.Izquierdo != null) q.Enqueue(n.Izquierdo);
                if (n.Derecho != null) q.Enqueue(n.Derecho);
            }
            return res;
        }

        // Altura del árbol (profundidad máxima) calculada recursivamente
        public int CalcularAltura()
        {
            return AlturaRec(Raiz);
        }
        private int AlturaRec(Nodo? n)
        {
            if (n == null) return 0;
            return 1 + Math.Max(AlturaRec(n.Izquierdo), AlturaRec(n.Derecho));
        }

        // Conteo de nodos del árbol
        public int ContarNodos()
        {
            return ContarRec(Raiz);
        }
        private int ContarRec(Nodo? n)
        {
            if (n == null) return 0;
            return 1 + ContarRec(n.Izquierdo) + ContarRec(n.Derecho);
        }
    }
}
