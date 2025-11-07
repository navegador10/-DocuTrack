using System;
using DocuTrack.Model;

namespace DocuTrack.View
{
    public class DocuTrackView
    {
        public void MostrarEncabezado(string titulo)
        {
            Console.WriteLine($"\n=== {titulo} ===\n");
        }

        public void MostrarMensaje(string mensaje)
        {
            Console.WriteLine(mensaje);
        }

        public void MostrarResultadoInsercion(string nombre, bool insertado, int comparaciones, string motivo)
        {
            var estado = insertado ? "OK" : ($"RECHAZADO ({motivo})");
            Console.WriteLine($"• Insertar '{nombre}': {estado} (comparaciones: {comparaciones})");
        }

        public void MostrarResultadoBusqueda(string nombre, bool encontrado, int comparaciones)
        {
            Console.WriteLine($"• Buscar '{nombre}': {(encontrado ? "ENCONTRADO" : "NO ENCONTRADO")} (comparaciones: {comparaciones})");
        }

        public void MostrarResultadoEliminacion(string nombre, bool eliminado, string caso)
        {
            Console.WriteLine($"• Eliminar '{nombre}': {(eliminado ? "OK" : "NO ENCONTRADO")} {caso}");
        }

        public void MostrarRecorrido(System.Collections.Generic.IEnumerable<string> recorrido, string nombre)
        {
            Console.WriteLine($"\n{nombre}: {string.Join(", ", recorrido)}");
        }

        public void MostrarArbol(Nodo? raiz)
        {
            if (raiz == null)
            {
                Console.WriteLine("[Árbol vacío]");
                return;
            }
            var q = new System.Collections.Generic.Queue<(Nodo node,int level)>();
            q.Enqueue((raiz, 0));
            int current = -1;
            while (q.Count > 0)
            {
                var (n, lvl) = q.Dequeue();
                if (lvl != current)
                {
                    current = lvl;
                    Console.Write($"\nNivel {lvl}: ");
                }
                Console.Write($"[{n.Nombre}] ");
                if (n.Izquierdo != null) q.Enqueue((n.Izquierdo, lvl+1));
                if (n.Derecho != null) q.Enqueue((n.Derecho, lvl+1));
            }
            Console.WriteLine();
        }

        public void MostrarAltura(int altura)
        {
            Console.WriteLine($"\nAltura: {altura}");
        }

        public void MostrarArbolAscii(Nodo? raiz)
        {
            if (raiz == null)
            {
                Console.WriteLine("[Árbol vacío]");
                return;
            }
            Console.WriteLine();
            ImprimirNodoAscii(raiz, prefix: string.Empty, esUltimo: true);
        }

        private void ImprimirNodoAscii(Nodo nodo, string prefix, bool esUltimo)
        {
            var conector = esUltimo ? "└── " : "├── ";
            Console.WriteLine($"{prefix}{conector}{nodo.Nombre}");

            var hijos = new System.Collections.Generic.List<Nodo>();
            if (nodo.Izquierdo != null) hijos.Add(nodo.Izquierdo);
            if (nodo.Derecho != null) hijos.Add(nodo.Derecho);

            for (int i = 0; i < hijos.Count; i++)
            {
                bool ultimoHijo = i == hijos.Count - 1;
                var nuevoPrefijo = prefix + (esUltimo ? "    " : "│   ");
                ImprimirNodoAscii(hijos[i], nuevoPrefijo, ultimoHijo);
            }
        }
    }
}
