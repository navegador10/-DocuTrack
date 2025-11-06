# DocuTrack

![.NET Build](https://github.com/navegador10/-DocuTrack/actions/workflows/dotnet.yml/badge.svg?branch=main)

## Integrantes

- MARIA CAMILA SAENZ VILLADA
- SERGIO ALONSO ARBOLEDA SANCHEZ
- ADELSON AGUIRRE RODRIGUEZ

## Sistema de Gestion DocuTrack S.A

## Integrantes del Equipo

- **MARIA CAMILA SAENZ VILLADA**: Responsable del Model y Arquitectura BST
- **SERGIO ALONSO ARBOLEDA SANCHEZ**: Responsable del View y Presentación
- **ADELSON AGUIRRE RODRIGUEZ**: Responsable del Controller y Casos de Uso

## Descripcion

Sistema de gestion de documentos basado en Arbol Binario de Busqueda (BST) para la empresa DocuTrack S.A.
Responsable del Model y de la arquitectura BST: MARIA CAMILA SAENZ VILLADA.

## Requisitos para Ejecutar

- .NET 8.0 o superior
- Visual Studio 2022 o VS Code
- Sistema operativo: Windows, Linux o macOS

## Compilacion y Ejecucion

```bash
dotnet build
dotnet run
```

## Ejemplo de salida

```text
DocuTrack iniciado.

=== Sistema de Gestión DocuTrack S.A ===

=== Construcción del Árbol ===
• Insertar 'Proyectos': OK (comparaciones: 1)
• Insertar 'Documentacion': OK (comparaciones: 1)
...

--- Inserciones Duplicadas ---
• Insertar 'Services': YA EXISTE (...)

=== Búsquedas Rápidas ===
• Buscar 'Documentacion': ENCONTRADO (...)
• Buscar 'NoExisto': NO ENCONTRADO (...)

=== Actualizaciones Selectivas ===
✓ Actualizado: 'Readme.txt' → 'LEEME.txt'
...

=== Eliminaciones Selectivas ===
• Eliminar 'App.config': OK (Caso Hoja)
...

=== Recorridos de Verificación ===
Preorden: Services, IntegrationTests, ...
Inorden: Client, Design, ...
Postorden: ...
Por Niveles: ...

=== Métricas Finales ===
Altura: 5

## Arquitectura (MVC)

- **Model**: Contiene `ArbolBinario` (BST) y `Nodo`.
- **View**: `DocuTrackView` imprime todo en consola (encabezados, resultados, recorridos, árbol por niveles).
- **Controller**: `DocuTrackController` orquesta el flujo completo: construcción del árbol, búsquedas, actualizaciones, eliminaciones, recorridos y métricas.

## Operaciones del BST

- **Insertar**: inserta por orden alfabético; evita duplicados; reporta comparaciones.
- **Buscar**: devuelve si un nombre existe y cuántas comparaciones se realizaron.
- **Eliminar**:
  - Hoja: elimina directamente.
  - Un hijo: reemplaza por el hijo existente.
  - Dos hijos: reemplaza por el sucesor inorden (mínimo del subárbol derecho).
- **Recorridos**: Preorden, Inorden, Postorden y Por Niveles.
- **Altura**: profundidad máxima del árbol.

> Nota: el recorrido **Inorden** confirma que el BST mantiene su orden: imprime los nombres en orden alfabético si la estructura es válida.

## Personalización de datos iniciales

- Archivo opcional: `data/elementos.txt`.
- Formato por línea: `Nombre;EsCarpeta`.
- Ejemplo:

  ```text
  # Nombre;EsCarpeta
  Proyectos;true
  Readme.txt;false
  Tests;true
  ```

- Valores válidos para `EsCarpeta`: `true/false`, `1/0`, `si/sí/no`, `yes/y`.
- Si el archivo no existe o está vacío, se usa un conjunto por defecto.

## Casos desde archivo (opcional)

- Archivo: `data/casos.txt`.
- Si existe, el Controller ejecuta estos casos y omite el flujo de demostración.
- Formato:

  ```text
  # Acción;Parámetros
  INSERTAR;Nombre;EsCarpeta
  BUSCAR;Nombre
  ACTUALIZAR;NombreAntiguo;NombreNuevo;EsCarpeta
  ELIMINAR;Nombre
  ```

- Ejemplo:

  ```text
  INSERTAR;Notas.txt;false
  BUSCAR;Documentacion
  ACTUALIZAR;Readme.txt;LEEME.txt;false
  ELIMINAR;Api
  ```

## Personalización de casos de prueba (opcional)

- Se puede añadir un archivo `data/casos.txt` con acciones a ejecutar en orden.
- Formato sugerido:

  ```text
  # Acción;Parámetros
  BUSCAR;Readme.txt
  ACTUALIZAR;Readme.txt;LEEME.txt;false
  INSERTAR;Notas.txt;false
  ELIMINAR;Api
  ```

- Acciones contempladas: `INSERTAR`, `BUSCAR`, `ACTUALIZAR`, `ELIMINAR`.
- Nota: actualmente el ejemplo del Controller incluye un flujo de demostración. Podemos activar la lectura de `casos.txt` si lo deseas.

## Ejecución

- Compilar: `dotnet build`
- Ejecutar: `dotnet run`

## Trabajo en equipo y verificación de commits

- Máximo 3 integrantes: Cumplido (3 personas listadas arriba con responsabilidades asignadas).
- Para verificar que los 3 integrantes han hecho commits en el repositorio público, use:

  ```bash
  # Resumen de autores (nombres y correos configurados en Git)
  git shortlog -sne

  # Commits por autor en un rango de fechas (ajuste fechas según la entrega)
  git log --since="2025-01-01" --until="2025-12-31" --pretty=format:"%an <%ae> %h %ad %s" --date=short

  # Validar que haya commits recientes por cada integrante
  git log -n 50 --pretty=format:"%h %ad %an %s" --date=relative
  ```

- Asegúrese de que el repositorio sea público y compila sin cambios adicionales (`dotnet build`, `dotnet run`).
