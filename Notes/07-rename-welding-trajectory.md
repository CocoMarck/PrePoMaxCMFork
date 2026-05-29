# Ranaming Welding Trajectory
## Cambios en FeMesh
Primeramente, en el `CaeMesh/FeMesh.cs`. En el método `public string[] GetAllMeshEnityNames()`. Aañado mis `CoordPointSet`. Hijo de `FeGroup`.
```csharp
            names.AddRange(_elementSets.Keys);
            names.AddRange(_surfaces.Keys);
            names.AddRange(_referencePoints.Keys);
            names.AddRange(_coordPointSets.Keys); // CoordPointSets WeldingTrajectories
            return names.ToArray();
        }
```

---
## Cambios en Controller
En el `PrePomax/Controller.cs`. Cree dos métodos para renombrar de una. Inspirados en como jalan `RanemeNodeSet` y `ReplaceNodeSet`.
```csharp
        public void ReplaceCoordPointSet(string oldCoordPointSetName, CoordPointSet coordPointSet, bool feModelUpdate) 
        {
            // Raplace CoordPointSet string name, with new CoordPointSet object
            _model.Mesh.CoordPointSets.Replace(oldCoordPointSetName, coordPointSet.Name, coordPointSet);
            _form.UpdateTreeNode( ViewGeometryModelResults.Model, oldCoordPointSetName, coordPointSet, null, feModelUpdate );
            if (feModelUpdate) FeModelUpdate(UpdateType.Check | UpdateType.RedrawSymbols);
        }
        public void RenameCoordPointSet(string oldCoordPointSetName, string newCoordPointSetName) 
        {
            // Ranaming CoordPointSet jejej Welding Trajectories. Chido. GG.
            CoordPointSet coordPointSet = _model.Mesh.CoordPointSets[oldCoordPointSetName];
            coordPointSet.Name = newCoordPointSetName;
            ReplaceCoordPointSet(oldCoordPointSetName, coordPointSet, true);
        }
```

En el método `public bool CheckItemNode(NamedClass`. Cuando existe el mesh. Pos le puse un `else if` como a los demas.
```csharp
                else if (item is CoordPointSet)
                    // Welding Trajectories
                    return CheckName(item.Name, newName, GetAllMeshEntityNames(), "coord point set");
```
> Ese `GetAllMeshEntityNames`, es medio wrapper al mesh method con el mismo name. Bien monolitico, modo One Dev Logic, Crazy Mode. 
> El CheckMode debe jalar de una, sin cambios, Solo si añadiste los subnodos like PrePoMax Style. Esto se ve en otro documentos anteriores a este.

Ahora en el `public void Raname(Type itemType`. Se añado un momento hardcoding. Pero con facil refactor a `PrePoMaxStyle`.
```csharp
            if (typeof(CoordPointSet).IsAssignableFrom(itemType))
            {
                // WeldingTrajectories Reneme CoordPointSet Tree Node.
                Debug.WriteLine("Fake Renaming jejej. GG");
                RenameCoordPointSet(itemName, newName);
                return;
            }
```

---
## ModelTree
En el `UserControls/ModelTree.cs`, en el constructor `public ModelTree()` se añadio:
```csharp
            // WeldingTab | Raname events
            cltvWelding.IsNodeRenameable = CanRename;
            cltvWelding.CheckNodeName = CheckNodeName;
```

Y en el event que es invocara en para renombrar like `PrePoMax`. `private bool CanRename(TreeNode node)`.
```csharp
            // Welding tab
            else if (node.Tag is CoordPointSet) return true;
            //
```

## Conclusión
Enlazar todo esta chido, pero tambien aunque PrePoMax sea bastante monolitico, aun uno puede enlazar las cosas bastante bien. Pero creo que PrePoMax se beneficiaria de un MVC + Unix Style. Claro es mas lento hacer eso, pero mejor para muchas Features. Like GIMP. Sin miedo al refactor, digo al exito... Bueno a las dos cosas.