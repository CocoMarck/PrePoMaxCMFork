# `PickBySurfacePoint`
Este es un método en `vtkControl.cs`. Sirve para seleccionar un punto, utilizando `vtkCellPicker`, y se renderiza con `RenderSurfacePoint`, pero mejor con el ya existente `HighLightNodes`. Ya que aunque su nombre diga nodes, renderiza en cualquier `xyz`.

El punto es que se determinara como funcionara la seleccion de puntos y guardado de estos, en `PickBySurfacePoint`. Y el renderizado deberia ser mas fácil.

Los puntos que se seleccionen se guardaran y obtendran del `pmx`. Ya despues se renderiza.

Este documento tiene el contexto del anterior documento. El `15-make-a-vtk-cell-picker`. En realidad todos los documentos deberian trabajar sobre el contexto del anterior. Por lo que si ves cosas que se ven bien raras, es porque te falta contexto.

---

## Guardar data de ids
Se necesitara de un dict, o lista, que se puede serializar asi como en `CaeMesh/FeNodeSet.cs`

Se usaran de nombres de entidades el prefijo `Coord`, pero para obtener `Coordinates`, método `Coor`, para que tenga sentido con `PrePoMax`.

Context [FeGroup](https://gitlab.com/MatejB/PrePoMax/-/raw/master/CaeMesh/FeGroup.cs?ref_type=heads). Lo que quiero guardar son las coords, por lo que sera una list que almacene tuplas/array de double values.

Se crearon los siguientes modulos, estos estan supuestamente listos para el PMX.

La serializacion de datos podria dar crash en old prepomax, pero siendo nueva tipo de data, es posible que no se tenga ese problema.

Almacena coordenadas, no fijas a nadota. `CaeMesh/CoordPoint.cs`
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class CoordPoint
    {
        // Variables
        private int _id;
        private double _x;
        private double _y;
        private double _z;

        // Constructors
        public CoordPoint()
        {
        }

        public CoordPoint(int id, double x, double y, double z)
        {
            _id = id;
            _x = x;
            _y = y;
            _z = z;
        }

        // Methods
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public double[] Coor 
        { 
            get { return new double[] { _x, _y, _z }; }
            set
            {
                if (value == null || value.Length != 3)
                    throw new ArgumentException();

                _x = value[0];
                _y = value[1];
                _z = value[2];
            }
        }
    }
}
```

Almacena `CoordPoint`. `CaeMesh/CoordPointSet.cs`
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

namespace CaeMesh
{
    [Serializable]
    public class CoordPointSet : FeGroup 
    {
        // Variables
        private List<CoordPoint> _points;

        // Constructors
        public CoordPointSet(string name) 
            : base(name, new int[0])
        {
            _points = new List<CoordPoint>();
        }

        // Methods
        public List<CoordPoint> Points
        {
            get { return _points; }
            set { _points = value; }
        }

        public void AddPoint(int id, double x, double y, double z)
        {   
            _points.Add(
                new CoordPoint(id, x, y, z)
            );
        }
    }
}
```

Para el renderizado, seguramente tendre que crear `HighlightCoordPoints`. Metodo que pedira `CoordPointSet`.

### `FeModel`.
Probablemente se tendra que agregar al `FeModel`. El `Controller.cs` lo agrega.
```csharp
protected FeModel _model;
```

Este esta en `CaeModel/FeModel.cs`.

---
## Renderzado
Actualmente `RenderSurfacePoint`, jala, pero deja rastros, mas debug que nada, pero sirve para probar. Este vive en `vtkControl.cs`.

Mientras que `HighlightCoordPoints`, vive en `Controller` y se encargara de dibujar todos los puntos.

Poner este como void, y en `PrePoMax/Controller.cs`:
```csharp
public void HighlightCoordPoints(string pointCoor, bool useSecondaryHighlightColor = false)
    {
        // Code
    }
```

IMPORTANTE: Como que el `HighLightNodes`, le vale queso si es nodo o no, solo renderiza. Si esto es así, ni hace falta usar `RenderSurfacePoint`.
```csharp
public void HighlightNodes(double[][] nodeCoor, bool useSecondaryHighlightColor = false)
{
    Color color = Color.Red;
    vtkRendererLayer layer = vtkRendererLayer.Selection;
    int nodeSize = _settings.Pre.HighlightNodeSymbolSize;
    DrawNodes("Highlight", nodeCoor, color, layer, nodeSize, false, useSecondaryHighlightColor);
}
```

**UPDATE. Efectivamente, a HighLightNodes, le vale si es nodo, solo pinta puntito, con coordenadas indicadas.** No hae falta, crear un HighlightCoordPoints.

---
## Obtener coords desde controller
La arquitectura REAL de PrePoMax, es alo más tipo Controller-centric architecture. NO MVC. Controller, conoce forms, render, selection, modelo, vtkControl, maneja commands, maneja UI state.

El controller y el frmMain, contienen, `SelectPointOrArea` func.

Para esto agregamos funcs, y logic en `FrmMain.cs`, `Controller.cs`, `vtkControl.cs`. Los tres se conectan, pero todo empieza en `vtkControl.cs`

Para esto usaremos el `vtkControl.cs` mouse event, `private void style_PointPickedOnLeftUpEvt`

Le agregamos este evento. Los comentarios lo dicen todo. Los puse en ingles por los loles, y para practicarlo un poco.
```csharp
// vtkSelectBy.SurfacePoint
else if (_selectBy == vtkSelectBy.SurfacePoint)
{
    vtkActor pickedActor;
    PickBySurfacePoint(out pickedActor, mea.Location.X, mea.Location.Y);

    // Info for using events GG. Good and happy game.
    vtkSelectOperation selectOperation;
    if (Control.ModifierKeys == (Keys.Shift | Keys.Control)) selectOperation = vtkSelectOperation.Intersect;
    else if (Control.ModifierKeys == Keys.Shift) selectOperation = vtkSelectOperation.Add;
    else if (Control.ModifierKeys == Keys.Control) selectOperation = vtkSelectOperation.Subtract;
    else selectOperation = vtkSelectOperation.None;
    bool completelyInside = mea.Location.X > x2;
    double[] pickedPoint = GetPickPoint(out pickedActor, mea.Location.X, mea.Location.Y);
    double[] direction = _renderer.GetActiveCamera().GetDirectionOfProjection();
    if (pickedActor == null) pickedActorNames = new string[0];
    else pickedActorNames = new string[] { GetActorName(pickedActor) };
                
    // This is the action, the very good mouse event
    OnMouseLeftButtonUpSelection?.Invoke(
        pickedPoint, direction, null, completelyInside,
        selectOperation, pickedActorNames
    );
}
```

En `Controller.cs` y en `FrmMain.cs`. Creamos la funcion/metodo `pivate void SelectCoordPoint`, ambas con mismo name. Pero no hacen lo mismo.

`Contoller SelectCoordPoint`
```csharp
        // SelectCoordPoint
        public void SelectCoordPoint(
            double[] pickedPoint, string[] pickedPartNames
        )
        {
            Debug.WriteLine( $"Controller. SelectCoordPoint. Entro: {pickedPoint}" );
            try
            {
                _form.ActivateUserPick();

                if (pickedPoint == null)
                    return;

                foreach (double coord in pickedPoint)
                    Debug.Write($"`{coord}` ");
            }
            catch { }
            finally
            {
                _form.DeactivateUserPick();
            }
        }
        // SelectCoordPoint
```
No hay misterio, agarra la coordenada y ya ta.

`FrmMain SelectCoordPoint`
```csharp
// SelectCoordPoint
public void SelectCoordPoint(
    double[] pickedPoint, double[] selectionDirection,
    double[][] planeParameters, bool completelyInside,
    vtkSelectOperation selectOperation, string[] pickedPartNames)
{
    Debug.WriteLine($"FrmMain. SelectCoordPoint. Entro: {pickedPoint}");

    // Relacionado con form
    PushMenuStates();
    SetStateWorking(Globals.SelectionText);

    // Obtener data
    _controller.SelectCoordPoint(pickedPoint, pickedPartNames);
    //List<double> coords = _controller.GetSelectionCoords(); <-- Esta func no existe. Mision para Style PrePoMax.

            // Formulario
    if (_frmSurfacePointPicker != null && _frmSurfacePointPicker.Visible)
    {
        //_frmSurfacePointPicker.PickedCoords(coords); <-- Esta func no existe. Mision para Style PrePoMax.
        _frmSurfacePointPicker.AddSurfacePoint(pickedPoint);
    }

    // Relacionado con form
    SetStateReady(Globals.SelectionText);
    PopMenuStates();
}
// SelectCoordPoint
```
Eventos comunes del form main, y llama al controller.

`FrmMain private void FrmMain_Load`. Aca agregamos el evento al action moment. Asi como el SelectPointOrArea.
```csharp
// vtk
_vtk.OnMouseLeftButtonUpSelection += SelectPointOrArea;
_vtk.OnMouseLeftButtonUpSelection += SelectCoordPoint; // SelectCoordPoint
```

Debug de ejemplo
```
Surface Point: -15.8680344316959, -66.4880627031388, -7.59999990463257
FrmMain. SelectCoordPoint. Entro: System.Double[]
Controller. SelectCoordPoint. Entro: System.Double[]
`-15.8680344316959` `-66.4880627031388` `-7.59999990463257`
```
> Ta bonito la verdad. Jejej.

---

## Updates a modulos relacionados
## Update a `PickBySurfacePoint`
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

    //double[] point = picker.GetPickPosition();
    //Debug.WriteLine(
    //    $"Surface Point: {point[0]}, {point[1]}, {point[2]}"
    //);
    //RenderSurfacePoint(point); // <-- Debug moment, no debe renderizar
}
```
Se elimino atributo `_lastSurfacePoint`, y func `LastSurfacePoint`. El render se dejo, pero no se usa en `PickBySurfacePoint`.

---

## `FeMesh.cs` Guardado de coords. Usando las entidades serializadas antes creadas.
- [Controller raw](https://gitlab.com/MatejB/PrePoMax/-/raw/master/PrePoMax/Controller.cs?ref_type=heads). Tiene a `public FeModel Model`.
- [FeModel raw](https://gitlab.com/MatejB/PrePoMax/-/raw/master/CaeModel/FeModel.cs?ref_type=heads). Tiene a `public FeMesh Mesh`.
- [FeMesh raw](https://gitlab.com/MatejB/PrePoMax/-/raw/master/CaeMesh/FeMesh.cs?ref_type=heads). Tiene a:
    ```csharp
    public OrderedDictionary<string, FeNodeSet> NodeSets
    {
        get { return _nodeSets; }
    }
    ``` 

    Entonces, deberia empezar poniendo en `FeMesh`, mi `CoordPointSet`

    Tambien desde aca se guarda el conteo de selecciones.
    ```
    private int _maxNodeId;     //ISerializable
    private int _maxElementId;  //ISerializable
    ```

### FeMesh

**Referencia de uso**. Guardar
```csharp
// Preparar para el pmx.
_customNodeSet.Labels = uniqueIds.ToArray();
_controller.GetNodesCenterOfGravity(_customNodeSet);

// Agregar al modelo si aún no existe
if (!_controller.Model.Mesh.NodeSets.ContainsKey(_customNodeSet.Name))
{
    _controller.Model.Mesh.NodeSets.Add(
        _customNodeSet.Name, _customNodeSet
    );
}           
```

**Referencia de uso**. Obtener
```csharp
// PMX | Intentar recuperar NodeSet guardado
if (_controller.Model.Mesh.NodeSets.ContainsKey("CustomSelection"))
{
    _customNodeSet =
        _controller.Model.Mesh.NodeSets["CustomSelection"];

    SetDrawDataOfSelectedNodes();
    Highlight();
}
```

---
Primero agregamos las vars necesarias.
```csharp
private OrderedDictionary<string, CoordPointSet> _coordPointSets;
private int _maxPointId;
```

Lo agregamos a las propiedades:
```csharp
// Poperties
...
public OrderedDictionary<string, CoordPointSet> CoordPointSets
{
    get { return _coordPointSets; }
}
...
public int MaxPointId
{
    get { return _maxPointId; }
}
```
De preferencia ordenado, y antes del `public BoundingBox BoundingBox`.

Le creamos estas fucns publicas, para que el formulario las use.
```csharp
// Esta la puse debajo de private int GetNextEdgeNodeId
public int GetNextPointId()
{
    _maxPointId++;
    return _maxPointId;
}
```

---
Usar: `CoordinateSystem`. Como referencia.
- [Coordinate Systen](https://gitlab.com/MatejB/PrePoMax/-/raw/master/CaeMesh/CoordinateSystem.cs?ref_type=heads)
- [FE Reference Point](https://gitlab.com/MatejB/PrePoMax/-/raw/master/CaeMesh/FeReferencePoint.cs?ref_type=heads)

Ya que esta hace algo parecido.

---

### Agregamos la obtecion de datos. Los Init
En `public FeMesh(SerializationInfo info, StreamingContext context)`. Despues del `case "_coordinateSystems"`. Y Luego despues del `if (_coordinateSystems == null)`
```csharp
...
foreach (SerializationEntry entry in info)
{
    switch (entry.Name)
    {
        ...
        case "_coordPointSets":
            _coordPointSets = (OrderedDictionary<string, CoordPointSet>)entry.Value;
            break;
        case "_maxPointId":
            _maxPointId = (int)entry.Value;
            break;
        ...
    }
    ...
    if (_coordPointSets == null)
    {
        _coordPointSets = new OrderedDictionary<string, CoordPointSet>("Coord Point Sets", sc);
    }
    ...
}
```

Aca:
```
public FeMesh(Dictionary<int, FeNode> nodes, Dictionary<int, FeElement> elements, MeshRepresentation representation,
        List<InpElementSet> inpElementTypeSets, string partNamePrefix, bool convertToSecondOrder,
        ImportOptions importOptions)
```
Agregamos:
```csharp
_coordPointSets = new OrderedDictionary<string, CoordPointSet>("Coord Point Sets", sc);
```
> Junto a los demas `OrderedDictionary`. 

En `public FeMesh(FeMesh mesh, string[] partsToKeep)`
Ponemos:
```csharp
_coordPointSets = new OrderedDictionary<string, CoordPointSet>("Coord Point Sets", sc);
_maxPointId = mesh._maxPointId;
```
> Junto a los demas `OrderedDictionary`, y `_max`.


Y en el `public void GetObjectData`, el objeto de mero abajo:
```csharp
info.AddValue("_coordPointSets", _coordPointSets);
info.AddValue("_maxPointId", _maxPointId);
```

LISTO.. Ya lo probe y jala de una. Guardar esto no fue tan dificil.

---

### Update al `FrmSurfacePointPicker.cs`
Asi finalmente se usaria. Ya guarda en el PMX.
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeGlobals;
using CaeMesh;

namespace PrePoMax.Forms
{
    public partial class FrmSurfacePointPicker : UserControls.PrePoMaxChildForm
    {
        /*
        Este formulario es para la seleccion de cualquier punto de la superficie de un modelo.
        */

        // Variables
        private Controller _controller;

        // Widgets
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPickPoints;

        // Callbacks
        public Action<string> Form_WriteDataToOutput;
        public Action<object, EventArgs> Form_RemoveAnnotations;

        // Para guardado en PMX File
        private CoordPointSet _coordPointSet;

        // Constructors
        public FrmSurfacePointPicker()
        {
            // InitializeComponet();
            _coordPointSet = new CoordPointSet("SurfacePoints");

            this.btnClose = new System.Windows.Forms.Button();
            this.btnPickPoints = new System.Windows.Forms.Button();

            // btnClose
            this.btnClose.Name = "btnClose";
            this.btnClose.Text = "Close";
            this.btnClose.Anchor = (
                (System.Windows.Forms.AnchorStyles)
                (
                    (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)
                )
            );
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.Location = new System.Drawing.Point(26, 256 - 26);
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // btnPickPoint
            this.btnPickPoints.Name = "btnPickPoints";
            this.btnPickPoints.Text = "Pick Points";
            this.btnPickPoints.Anchor = (
                (System.Windows.Forms.AnchorStyles)
                (
                    (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                )
            );
            this.btnPickPoints.Size = new System.Drawing.Size(75, 23);
            this.btnPickPoints.Location = new System.Drawing.Point(26, 26);
            this.btnPickPoints.Click += new System.EventHandler(this.btnPickPoints_Click);

            // FrmSurfacePointPicker
            this.Text = "Surface Point Picker";
            this.Name = "FrmSurfacePointPicker";
            this.ClientSize = new System.Drawing.Size(256, 256);
            this.CancelButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSurfacePointPicker_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FrmSurfacePointPicker_VisibleChanged);
            this.SuspendLayout();
            this.ResumeLayout(false);

            // Agregar widget
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPickPoints);
        }

        // Eventos principales
        public void PrepareForm(Controller controller)
        {
            // Controler
            _controller = controller;
            _controller.SetSelectByToOff();

            // PMX | Intentar obtener data.
            if (_controller.Model.Mesh.CoordPointSets.ContainsKey("SurfacePoints"))
            {
                _coordPointSet = _controller.Model.Mesh.CoordPointSets["SurfacePoints"];
                Highlight();
            }
        }

        public void RemoveMeasureAnnotation()
        {
            _controller.Annotations.RemoveCurrentMeasureAnnotation();
        }

        private void FrmSurfacePointPicker_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Esto si no se que es. Parece que es forzar ocultado de form.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void FrmSurfacePointPicker_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                return;
            }
            // Cuando el formulario esta oculto
            else
            {
                return;
            }
        }

        public void PickedCoords(List<double[]> coords)
        {
            // Supongo que esta algun dia se usara. Pero por ahora no hace falta `2026-05-19`.
        }

        // Widgets
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnPickPoints_Click(object sender, EventArgs e)
        {
            _controller.SelectBy = vtkSelectBy.SurfacePoint;
            _controller.Selection.SelectItem = vtkSelectItem.SurfacePoint;
        }

        // Render
        // Mostrar puntos
        public void Highlight()
        {
            if (_coordPointSet == null) return;
            if (_coordPointSet.Points.Count <= 0) return;

            double[][] points = _coordPointSet.Points.
                Select(p => p.Coor).ToArray();

            _controller.HighlightNodes(points);
        }

        public void AddSurfacePoint(double[] point)
        {
            if (point == null) return;

            int id = _controller.Model.Mesh.GetNextPointId();
            _coordPointSet.AddPoint(id, point[0], point[1], point[2]);

            Form_WriteDataToOutput($"Surface point: {point[0]}, {point[1]}, {point[2]}");

            // PMX Agregar al model mesh si aun no existe.
            if (!_controller.Model.Mesh.CoordPointSets.ContainsKey(_coordPointSet.Name))
            {
                _controller.Model.Mesh.CoordPointSets.Add( _coordPointSet.Name, _coordPointSet );
            }

            Highlight();
        }
    }
}
```

---
## Conclusión
El guardado de PMX, fue sencillo. Conectar todo fue mas difcil.