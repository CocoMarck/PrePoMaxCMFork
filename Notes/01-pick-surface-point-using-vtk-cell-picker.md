# Crear un Cell Picker con `vtkControl.cs`. Usado en PrePoMax.
Para seleccionar un punto en cualquier superficie. No necesitamos que sea nodo ni vértice. Lo típìco es usar un **`CellPicker` de superficie**.

En los modulos donde buscaremos el uso de esto seran
- `vtkControl.cs`: Eventos con mouse.
- `PrePoMax/Controller.cs | PrePoMax/Selection/ItemSet/*.cs`: Selection logic.

- [Documentacion `vtkCellPicker`](https://vtk.org/doc/nightly/html/classvtkCellPicker.html)
- [Documentación `vtkControl`](https://gitlab.com/MatejB/PrePoMax/-/blob/master/README.md?ref_type=heads&plain=0#prepomax-visual-studio-setup)
    *  In Solution Explorer find the vtkControl project and then find the References branch
    *  Right click on References from vtkControl project in the Solution Explorer Window and selected Add Reference	
    *  vtkControl: classes for 3D visualization

Ejemplo:
```csharp
vtkCellPicker picker = vtkCellPicker.New();
picker.SetTolerance(0.0005);

int ok = picker.Pick(mouseX, mouseY, 0, renderer);

if (ok == 1)
{
    double[] p = picker.GetPickPosition(); // punto 3D exacto sobre la superficie
    long cellId = picker.GetCellId();      // cara/elemento/celda tocada
}
```

El punto es usar el cell picker `vtkCellPicker`. De `vtkControl`. 

La idea seria usar `vtkCellPicker`, obtengo coordenada 3D, de lo seleccionado. Guardo eso en memoria, y renderizo.

Guardarlo, algo asi:
```csharp
List<double[]> paintedPoints = new List<double[]>();
paintedPoints.Add(p);
```

---

## `vtkControl.cs` | CellPicker
Tiene como atributo un `vtkCellPicker`, el cual se usa pa muchas cosas. Entre ellas, nuestro objetivo. Seleccionar en cualquier parte del modelo un punto.

Cualquier parte visible/intersectable de la superficie renderizada
```csharp
public partial class vtkControl : UserControl
{
    ...
    private bool _querySelectionInProgress;
    private vtkSelectBy _selectBy;
    private vtkSelectItem _selectItem;
    private vtkPropPicker _propPicker;
    private vtkPointPicker _pointPicker;
    private vtkCellPicker _cellPicker;
    private vtkRenderedAreaPicker _areaPicker;
    private vtkCellPicker _cellPicker;
    ...
    // Coustructor
    public vtkControl(){
        ...
        _cellPicker = vtkCellPicker.New();
        _cellPicker.SetTolerance(0.01); 
        ...
    }
    ...
    private void AddActorGeometry(vtkMaxActor actor, vtkRenderLayer layer)
    {
        ...
        _cellPicker.AddLocation(actor.CellLocator);
        ...
    }
    ...
}
```

---
## Busqueda recursiva en VisualStudio
### `_cellPicker.`
```
_cellPicker.SetTolerance(0.01);	vtkControl\vtkControl.cs	329	13	
        if (actor.CellLocator != null) _cellPicker.AddLocator(actor.CellLocator);	vtkControl\vtkControl.cs	4303	52	
        if (actor.CellLocator != null) _cellPicker.AddLocator(actor.CellLocator);	vtkControl\vtkControl.cs	4315	52	
_cellPicker.RemoveAllLocators();	vtkControl\vtkControl.cs	6663	13	
        if (actor.CellLocator != null) _cellPicker.RemoveLocator(actor.CellLocator);	vtkControl\vtkControl.cs	6698	56	
```

### `_pointPicker`
```
    _pointPicker = vtkPointPicker.New();	vtkControl\vtkControl.cs	325	13	
private vtkPointPicker _pointPicker;	vtkControl\vtkControl.cs	113	32	
    _pointPicker.SetTolerance(0.01);	vtkControl\vtkControl.cs	326	13	
```

**En realidad parece que los atributos privados de `vtkControl` no se usan fuera de el.**

---

## Código de ejemplo, en `vtkControl.cs`
```csharp
        // Surface point picker
        public bool TryPickSurfaceCell( 
            out double[] point, out int globalCellId, out vtkActor pickedActor, out vtkCell pickedCell, out vtkCellLocator pickedCellLocator
        ) 
        {
            point = null;
            globalCellId = -1;
            pickedActor = null;
            pickedCell = null;
            pickedCellLocator = null;

            // Valider estado mínimo de viewport y evitar picking sobre widgets.
            if (_renderer == null) return false;
            else if (_propPicker == null) return false;
            else if (_style != null ) return false;

            // Obtener actor y punto 3D aproximado sobre la superficie seleccionada. Desde la pos 2D del mouse.
            int[] pos = _style.GetInteractor().GetEventPosition();
            point = GetPickPoint(out pickedActor, pos[0], pos[1]);

            // Validar si no hay punto o actor, no se seleccionó una superficie válida.
            if (point == null) return false;
            else if (pickedActor == null) return false;

            // Encontar la celda real más cercana. Se obtiene la FEM real más cercana el punto seleccionado.
            globalCellId = GetGlobalCellIdClosestTo3DPoint(
                ref point, out pickedCell, out pickedCellLocator
            );

            // Validar que el picking realmete encontró una celda válida.
            if (globalCellId < 0) return false;
            else if (pickedCell == null) return false;
            else if (pickedCellLocator == null) return false;

            return true;
        }
        // Surface point picker
```
> Intentar obtener puntos.

**`vtkInteractorStyleControl.cs` es una capa de interacción VTK pura.** `vtkControl.cs` lo tiene de atributo como `_style`.

### Regla simple
Si el método necesita: `renderer, actor, locator, FEM mapping. Pertenece a vtkControl.`

Si solo necesita: `mouse button, keyboard, camera movement.` Pertenece a `vtkInteractorStyleControl`.

### En el `public vtkSelectBy SelectBy`
Tengo que hacer un nuevo case para el `SalectBy`. y el `SelectItem`.

### `vtkSelectBy` `vtkSelectItem`
`vtkSelectBy` tiene MUCHO que ver. MUCHÍSIMO.

Porque literalmente controla TODO el pipeline de selección:
- renderer mode
- picking
- actor filtering
- ids
- geometry
- surfaces
- edge logic
- frustum selection
- angle selection

NO necesitas hacer: `vtkSelectBy.Cell`. Porque YA EXISTE: `vtkSelectBy.Surface`. 

Y también:
- `GeometrySurface`
- `SurfaceAngle`

`vtkSelectItem` probablemente importa MÁS que `vtkSelectBy`.

### Diferencia REAL
- `vtkSelectBy`: controla cómo seleccionas.
    Ejemplos:
    - Node
    - Surface
    - Part
    - EdgeAngle
    - GeometrySurface

    Es más:
    - viewport mode
    - picking mode
    - routing
- `vtkSelectItem`: qué entidad lógica estás manipulando
    Y eso afecta TODO:
    - ids
    - conversiones
    - highlighting
    - annotations
    - sets
    - geometry mapping
    - visibility queries

### `GetVisibleFaceIds()`
`Surface = Face IDs`. Cuando hago `vtkSelectItem.Surface`. Internamente PrePoMax trabajo con: face ids. El problema es que, el sistema de selección de PrePoMax es ID-Based.
- node ids
- element ids
- face ids
- geometry ids
- part ids

No son coordenadas libres.

Lo mejor es hacer un enum nuevo, que prabablemente sera llamado: `vtkSelectItem.SurfacePoint;`. Un `SelectItem` No un `SelectBy`.

De hecho:
```csharp
_controller.SelectBy = vtkSelectBy.Surface;
_controller.Selection.SelectItem = vtkSelectItem.SurfacePoint;
```
> No existe `SurfacePoint`, la misión seria crearlo.

Porque `SelectBy.Surface`, ya da:
- Surface picking mode
- Renderer bahavior
- Filters

Y el conceptual `SeletItem.SurfacePoint`, te dice:
- Que devolver
- Que guardar
- Como interpretar la selección

Esto `if (_selection.SelectItem == vtkSelectItem.Surface)`, esta por todos lados. Y significa `SelectItem`, tipo de identidad lógica.

---
# SOLUCION REAL FINAL
Esta es la solucion real final, lo demas fue puro buscar, prueba y error. Si se quiere entender como implementar `CellPicker`, entender esto de aca es clave.

## Agregar `SurfacePoint`
Los enums de los `SelectBy` y `SelectItem`. Estan en: `CaeGlobals/Selection/Enums`, `vtkSelectItem.cs`, `vtkSelectBy.cs`

Le agregamos un enum al `vtkSelectItem`. Seguimos la equivalencia secuencial.
- `SurfacePoint = 11;`
> El ultimo enum, era el 10.

Agreamos el mismo enum en el `vtkSelectBy.cs`. Pero aca, es sin valor. Solo; `SurfacePoint`.

En el `vtkControl.cs`, en el `public vtkSelectBy SelectBy`, le agregamos un nuevo case.
```csharp
case vtkSelectBy.SurfacePoint:
    _style.SelectionBoxEnabled = false;
    break;
```

En `vtkController.cs` Le agregamos esta var: `private double[] _lastSurfacePoint;`

Y estas funcs.
```csharp
public double[] LastSurfacePoint
{
    get { return _lastSurfacePoint; }
}
private void RenderSurfacePoint(double[] point)
{
    if (point == null) return;

    vtkSphereSource sphere = vtkSphereSource.New();

    sphere.SetCenter( point[0], point[1], point[2] );

    sphere.SetRadius(1);

    vtkPolyDataMapper mapper = vtkPolyDataMapper.New();

    mapper.SetInputConnection( sphere.GetOutputPort() );

    vtkActor actor = vtkActor.New();

    actor.SetMapper(mapper);

    actor.GetProperty().SetColor(1, 0, 0);

    _renderer.AddActor(actor);

    RenderScene();
}
```
> Tema lo que se muestra, creamos una esfera roja, y la mostramos en la pos indicada. Bueno y el lastSurfacePoint, pos un simple get a private var.

Creamos `PickBySurfacePoint`, usara todo para el render y select de puntos.
```csharp
private void PickBySurfacePoint(
    out vtkActor pickedActor, int x, int y
)
{
    Debug.WriteLine("select by Surface. Try to do all things.");
            
    pickedActor = null;

    vtkCellPicker picker = vtkCellPicker.New();
    Debug.WriteLine("Damn, vtkCellPicker, already");

    picker.SetTolerance(0.0005);

    int picked = picker.Pick(x, y, 0, _renderer);

    if (picked != 1) return;

    pickedActor = picker.GetActor();

    if (pickedActor == null) return;

    double[] point = picker.GetPickPosition();

    _lastSurfacePoint = point;

    Debug.WriteLine(
        $"Surface Point: {point[0]}, {point[1]}, {point[2]}"
    );

    RenderSurfacePoint(point);
}
```

En `private void Style_PointPickedOnMouseMoveEvt`. Indicamos el case moment. Junto a los demas case.
```csharp
case vtkSelectBy.SurfacePoint:
    PickBySurfacePoint(out pickedActor, x1, y1);
    break;
```

En el `PrePoMax/Controller.cs`, en la funcion `GetIdsFromSelectionNodeMouse(SelectionNodeMouse selectionNodeMouse)`, ponemos este `else if`:
```csharp
else if (_selection.SelectItem == vtkSelectItem.SurfacePoint)
{
    ids = new int[0];
}
```
> Asi evitamos crash loco, pero puede que no se necesite. Es que esta func busca retornar ids siempre.

### Ejemplo de uso
En PrePoMax Style, como debe ser. Este ejemplo es de un `FrmSurfacePointPicker.cs`. Y funciona de una. Por ahora solo se limita a esto (sin guardado de datos en pmx), pero es un gran avance.
```csharp
private void btnPickPoints_Click(object sender, EventArgs e)
{
    _controller.SelectBy = vtkSelectBy.SurfacePoint;
    _controller.Selection.SelectItem = vtkSelectItem.SurfacePoint;
}
```

## Notas
- `2026-05-18`: 
    - Ok, no esta entrando a `else if (_selectBy == vtkSelectBy.SurfacePoint)` en `PickByArea`. 
    
    - Jajaj... Lo logramos (yo y ChatGPT) we. Yo con el context, y tu con la logic pesada. Bien hecho. Fue algo confuso de hacer, culpa de que no se usar kernels, eso es otro pedo. Descarte código, pero ese no importa. 
    
    - Al final si tube que crear eums, funcs, y etc. Pero lo bueno es que `vtkCellPicker` es buen wrapper y de una jala.

    - Bueno, si quiero usar `RenderSurfacePoint`, todo se resume a ajustarlo, y serializar datos de coords de seleccion, porque dudo que PrePoMax tenga algo listo para guardar eso al `pmx`, de guardado de `xyz` no ids.

    - Existe la función `PickByCell`. Verla para obtener mucha data de como implementar cualquier cosa con relacion a este tema.

    - `Style_PointPickedOnMouseMoveEvt` y `style_PointPickedOnLeftUpEvt`: Son para el estilo de seleccion de puntos, ya sea moviendo el mouse o cuando el click izquierdo se levanta. Se maneja en case, y else if respectivamente, no se muy bien porque uno maneja case y el otro ifs, pero bueno, jala.

    - En busceda recursiva de `PickByCell`, esa func esta ready, pero se usa indiractamente, en `Style_PointPickedOunMouseEvt`, para algo relacionado al `query` tool. `vtkControl.cs line 486`. Conclusión, los metodos de este doc si son necesarios.

### Output
`2026-05-18`
No cleen de except, pero PrePo los cacha bien, y lo chido es que ya todo jala, ahorita te pasa documento. Por hoy termine. A canijo, mucho search, pero otra vez, ya todo esta listo pa usar, solo era conectar todo chido.
```
Damn, vtkCellPicker, already 
Surface Point: 37.6682241162341, 39.498119354248, 163.743460375583
select by Surface. Try to do all things.
Damn, vtkCellPicker, already
Surface Point: 31.6948939100548, 39.498119354248, 172.991437289065 
Exception thrown: 'System.NotSupportedException' in PrePoMax.exe select by Surface.
Try to do all things. Damn, vtkCellPicker, already
Surface Point: 24.2239749698585, 39.498119354248, 170.813487003052
Exception thrown: 'System.NotSupportedException' in PrePoMax.exe The thread 6812 has exited with code 0 (0x0). The thread 5212 has exited with code 0 (0x0). Exception thrown: 'System.NotSupportedException' in PrePoMax.exe
select by Surface. Try to do all things.
Damn, vtkCellPicker, already 
Surface Point: 31.1014512059847, 39.498119354248, 141.
```