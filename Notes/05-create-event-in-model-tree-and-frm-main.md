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

### De hecho, haciendo todo lo que se vio en este documento, ya de una jala con el doble click.

----

## Agregar evento a subnodo. En evento de Editar.
Primero en `ModelTree`. Existe el metodo `private void tsmiEdit_Click(object sender, EventArgs e)`. La cual invoca a `EditEvent?Invoke((NamedClass)selectedNode.Tag, stepName);`

En esta función, para que veas un message cada edit moment que pasa puedes ponerle esto en el `try`:
```csharp
MessageBox.Show( $"Editing: `{selectedNode.Tag}`" ); // DEBUG
```

Y en el `FrmMain`, en el metodo `private void ModelTree_EditEvent`. Este es lo invocado en el model tree. Agregamos nuestro if, y recuerda aver metido tus subnodos en `AddObjectToNode`, que esta en `TreeNode`.
```csharp
// WeldingTrajectory
if (namedClass is CoordPointSet)
{
    MessageBox.Show( namedClass.Name );
}
``` 
> El message box ya debe ser tu evento. Y si, le envias el name del nodo.

Recordatorio de lo que pide el metodo `AddObjectsToNode`:
```csharp
private void AddObjectsToNode<TKey, TVal>(
    string initialNodeName, TreeNode node, IDictionary<TKey, TVal> dictionary, bool countNodes = true 
)
```