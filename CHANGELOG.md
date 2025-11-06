# Changelog

## v1.0.1 - 2025-11-05

- docs: aclaración de responsabilidad del Model/BST (Camila)
- feat(model): agregar InordenDesc y ContarNodos al BST

## v1.0.0 - 2025-11-03

- Arquitectura MVC completa (Model, View, Controller).
- BST con operaciones: Insertar, Buscar, Eliminar (sucesor), Recorridos y Altura.
- Regla aplicada: "archivo es siempre hoja" (rechazo con motivo en inserción).
- `EliminarConCaso`: informa Hoja / Un Hijo / Dos Hijos / No encontrado.
- Vista mejorada:
  - Diagrama ASCII con conectores (├──, └──, │).
  - Resultados con viñetas y contadores de comparaciones.
- Controlador: flujo de demostración y ejecución opcional desde `data/casos.txt`.
- Configuración por archivo: `data/elementos.txt` y `data/casos.txt`.
- Comentarios en español explicando paso a paso el flujo y la lógica.
- README actualizado (arquitectura, operaciones, personalización, nota sobre Inorden).
- CI/CD: workflow de GitHub Actions (.NET 8) y badge en README.
- Upgrade de framework: .NET 6 → .NET 8 (
  `TargetFramework: net8.0` + `global.json`).
- Limpieza del repo: `.gitignore` para excluir `bin/` y `obj/`.
