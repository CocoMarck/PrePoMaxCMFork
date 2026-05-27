# Agregar nodos a TreeModel con diccionario
Esta funcion esta en `TreeModel.cs`. Y existe para hacerle update a al Tree, y sus Nodos.
```csharp
private void AddObjectsToNode<TKey, TVal>(
    string initialNodeName, TreeNode node, IDictionary<TKey, TVal> dictionary, bool countNodes = true 
)
```
Convierte `dictionary` a lista. Y con ello empieza a actualizar con un `foreach` los nodos del Tree.

Parece que `RegenerateTree` Se encarga de agragar todo, y lo hace usando `AddObjectsToNode`
```csharp
public void RegenerateTree(FeModel model, IDictionary<string, AnalysisJob> jobs, FeResults results,
                           bool remeshing = false)
```

LISTO, lo logre. Bueno eso y usar la busqueda recursiva y saber leer. Si, todo pasa en `RegenerateTree`. En el primer `if (model.Mehs !=Null)`.

Puse:
```csharp
public void RegenerateTree(...)
{
    ...
    if (model.Mesh != null)
    {
        // Mesh Parts
        AddObjectsToNode(_modelPartsName, _modelParts, model.Mesh.Parts);
        // Node sets
        AddObjectsToNode(_modelNodeSetsName, _modelNodeSets, model.Mesh.NodeSets);
        // Element sets
        AddObjectsToNode(_modelElementSetsName, _modelElementSets, model.Mesh.ElementSets);
        // Surfaces
        AddObjectsToNode(_modelSurfacesName, _modelSurfaces, model.Mesh.Surfaces);
        // Reference points
        AddObjectsToNode(_modelReferencePointsName, _modelReferencePoints, model.Mesh.ReferencePoints);
        // Coordinate systems
        AddObjectsToNode(_modelCoordinateSystemsName, _modelCoordinateSystems, model.Mesh.CoordinateSystems);
        // WeldingTrajectories
        AddObjectsToNode(_weldingTrajectoriesName, _weldingTrajectories, model.Mesh.CoordPointSets);
    }
    ...
}
```
> En `WeldingTrajectories`; Agregue mi name de nodo, el nodo `string`, y el `dict` en `FeMesh`.
