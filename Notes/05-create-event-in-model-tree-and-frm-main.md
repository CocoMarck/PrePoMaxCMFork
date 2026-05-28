# Create Event in `ModelTree` and `FrmMain`
## Create option | Al darle click derecho.
**Esto sucede en el `ModelTree.cs`**

El prepomax ya tiene `Create` option contemplado, por eso no es necesario ponerle nombre al boton.

Y recuerda que `Name` es el de tu `TreeNode`.

Adentro de `CanCreate(TreeNode node)` agrega antes del `return false`. Un else if.
```csharp
else if (node.TreeView == cltvCustomTab && node.Name == "customSubNode") return true;
```

### Creamos un pubic event, solo para acceder al nombre nuestro nodo
```csharp
public string WeldingTrajectoriesName { get { return _weldingTrajectoriesName; } }
```

## Ahora en el `FrmMain`
En el `ModelTree_CreateEvent(string nodeName`. Agregamos un `if` o un `else if`. Segun veas. Yo pongo un if de una al final y ya esta:
```csharp
if (nodeName == _modelTree.WeldingTrajectoriesName)
{
    // WeldingTrajectories
    Debug.WriteLine("Create WeldingTrajectorie");
    MessageBox.Show("Create");
}
```