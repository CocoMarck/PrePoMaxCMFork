# Documentación resumida: `custom select form`

## Objetivo

Se creó un nuevo formulario llamado `FrmCustomSelect` para probar una selección personalizada de nodos/vértices dentro de PrePoMax.

La idea fue tomar como referencia la lógica de `FrmQuery`, pero hacer una versión más simple enfocada solo en:

- seleccionar nodos/vértices;
- recibir los IDs seleccionados desde `FrmMain.cs`;
- guardar los nodos seleccionados en memoria;
- mostrar información en la ventana de salida;
- dibujar highlight y annotations en el viewport.

---

## 1. Creación del nuevo formulario

Se creó una nueva clase:

```csharp
public partial class FrmCustomSelect : UserControls.PrePoMaxChildForm
````

El formulario está dentro del namespace:

```csharp
namespace PrePoMax.Forms
```

Esto permite que el formulario pertenezca al grupo de ventanas internas de PrePoMax. El comentario principal indica que el formulario no está 100% estructurado como las forms oficiales, pero funciona y recibe los IDs desde `FrmMain.cs`. 

---

## 2. Variables principales del formulario

Dentro de `FrmCustomSelect` se agregaron variables para manejar:

```csharp
private int _numOfNodesToSelect;
private double[][] _coorNodesToDraw;
private double[][] _coorLinesToDraw;
private Controller _controller;
```

Estas variables sirven para:

* definir cuántos nodos se esperan;
* guardar coordenadas para dibujar highlight;
* guardar referencia al `Controller`;
* reutilizar funciones internas de PrePoMax. 

---

## 3. Almacenamiento de nodos seleccionados

Se agregó un diccionario:

```csharp
private Dictionary<int, FeNode> _selectedNodes = new Dictionary<int, FeNode>();
```

La idea es guardar nodos seleccionados con esta relación:

```txt
Node ID -> FeNode
```

Esto funciona como una base inicial para después crear algo tipo `NodeSet` custom o guardar selecciones más formales. 

---

## 4. Constructor del formulario

El constructor arma el formulario manualmente, sin usar `InitializeComponent()`.

Se crean dos botones:

```csharp
btnClose
btnSelectNodes
```

El botón `Close` oculta la ventana, y el botón `Select nodes` activa la selección de nodos. También se define el tamaño, texto, fuente, eventos de cierre y evento `VisibleChanged`. 

---

## 5. Preparación del formulario con `Controller`

Se agregó el método:

```csharp
public void PrepareForm(Controller controller)
{
    _controller = controller;
    _controller.SetSelectByToOff();
}
```

Este método conecta el formulario con el `Controller` principal de PrePoMax.

Con eso, el formulario puede usar:

* selección;
* modelo;
* mesh;
* nodos;
* annotations;
* highlight;
* output;
* viewport. 

---

## 6. Comportamiento al mostrar u ocultar el formulario

Cuando el formulario se muestra, activa el modo query:

```csharp
_controller.SetQuerySelection(true);
```

Cuando se oculta, limpia selección, annotations y desactiva el modo query:

```csharp
_controller.SelectBy = vtkSelectBy.Default;
RemoveMeasureAnnotation();
_controller.ClearSelectionHistoryAndCallSelectionChanged();
_controller.SetQuerySelection(false);
```

Esto hace que `FrmCustomSelect` se comporte parecido a `FrmQuery`: mientras está visible, puede recibir selección desde el viewport. 

---

## 7. Activación de selección de nodos

El botón `Select nodes` activa la selección de nodos con:

```csharp
_controller.SelectBy = vtkSelectBy.Node;
_controller.Selection.SelectItem = vtkSelectItem.Node;
```

Después limpia annotations y selección anterior:

```csharp
RemoveMeasureAnnotation();
_controller.ClearSelectionHistoryAndCallSelectionChanged();
```

Esto configura el viewport para seleccionar nodos/vértices. 

---

## 8. Recepción de IDs seleccionados

El método central del formulario es:

```csharp
public void PickedIds(int[] ids)
```

Este método recibe los IDs que vienen desde el sistema de selección de PrePoMax.

El formulario no hace el picking directamente. `FrmMain.cs` recibe el evento del viewport y luego manda los IDs al formulario custom.

Dentro de `PickedIds`, si llega un solo nodo, se obtiene el `FeNode` real:

```csharp
FeNode node = _controller.Model.Mesh.Nodes[ids[0]];
_selectedNodes.Add(ids[0], node);
OneNodePicked(ids[0]);
```

Después se limpia la selección y se llama a `Highlight()`. 

---

## 9. Procesamiento de un nodo seleccionado

Cuando se selecciona un nodo, se ejecuta:

```csharp
public void OneNodePicked(int nodeId)
```

Ahí se obtiene la unidad de longitud, se obtiene la coordenada base del nodo y se escribe información en la ventana de salida:

```csharp
Vec3D baseV = new Vec3D(_controller.GetNode(nodeId).Coor);
Form_WriteDataToOutput(...);
```

También se imprime el conteo de nodos seleccionados:

```csharp
Form_WriteDataToOutput($"Count of selected nodes: {_selectedNodes.Count}");
```

Finalmente se guarda la coordenada para highlight y se agrega una annotation al nodo:

```csharp
_coorNodesToDraw[0] = baseV.Coor;
_controller.Annotations.AddNodeAnnotation(nodeId);
```



---

## 10. Highlight del nodo seleccionado

El método:

```csharp
public void Highlight()
```

usa funciones internas del `Controller` para dibujar visualmente la selección.

Para un nodo:

```csharp
_controller.HighlightNodes(_coorNodesToDraw);
```

También dejaste preparada lógica para dos nodos o más, usando líneas y flechas, aunque por ahora el caso real es un nodo. 

---

# Cambios en `FrmMain.cs`

## 1. Declaración del formulario

En `FrmMain.cs`, dentro de la región de variables, se agregó:

```csharp
// custom select form
private FrmCustomSelect _frmCustomSelect;
```

Esto declara el formulario custom como variable global dentro de `FrmMain`, igual que las demás forms internas de PrePoMax. 

---

## 2. Creación e integración del formulario

En `FrmMain_Load`, en la sección donde se crean las demás ventanas internas, se agregó:

```csharp
// custom select form
_frmCustomSelect = new FrmCustomSelect();
_frmCustomSelect.Form_WriteDataToOutput = WriteDataToOutput;
_frmCustomSelect.Form_RemoveAnnotations = tsbRemoveAnnotations_Click;
AddFormToAllForms(_frmCustomSelect);
```

Esto hace cuatro cosas:

1. Crea la instancia de `FrmCustomSelect`.
2. Conecta el callback para escribir en la ventana de salida.
3. Conecta el callback para borrar annotations.
4. Registra el formulario en `_allForms`.

Con `AddFormToAllForms(_frmCustomSelect)`, PrePoMax ya puede manejarlo como una ventana interna más. 

---

## 3. Flujo general

El flujo final queda así:

```txt
FrmMain.cs
    ↓
crea FrmCustomSelect
    ↓
le pasa callbacks de Output y Annotations
    ↓
lo registra en AddFormToAllForms
    ↓
FrmCustomSelect usa Controller
    ↓
se activa selección de nodos
    ↓
PickedIds(ids) recibe IDs
    ↓
se guarda FeNode en Dictionary<int, FeNode>
    ↓
se dibuja highlight y annotation
```

---

# Resultado actual

Con estos cambios ya tienes un formulario custom funcional que:

* existe dentro del sistema de forms de PrePoMax;
* puede recibir el `Controller`;
* puede activar selección de nodos;
* puede recibir IDs seleccionados;
* puede obtener objetos `FeNode`;
* puede guardar nodos en memoria;
* puede escribir en Output Window;
* puede agregar annotations;
* puede hacer highlight en el viewport.