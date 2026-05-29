# Export tsmi node button
Haremos un export, pero que solo jalara para los `CoordPointSet` Tag type.

## `UserControls/ModelTree.Designer.cs`
En el `private void InitializeComponent()`. Inicializamos nuestro nuevo `ToolStripMenuItem`. Junto a los demas.
```csharp
            this.tsmiExport = new System.Windows.Forms.ToolStripMenuItem(); // WeldingTrajectory CoordPointSet
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.ilStatusIcons = new System.Windows.Forms.ImageList(this.components);
            this.tcGeometryModelResults = new System.Windows.Forms.TabControl();
            .....
            this.tsmiExpandAll,
            this.tsmiCollapseAll,
            this.tsmiSpaceDelete,
            this.tsmiDelete});
            this.tsmiDelete,
            this.tsmiExport, // WeldingTrajectory CoordPointSet
            });
            .....
            //
            // tsmiExport WeldingTrajecory CoordPointSet node option. Ye this is good.
            //
            this.tsmiExport.Name = "tsmiExport";
            this.tsmiExport.Size = new System.Drawing.Size(211, 22);
            this.tsmiExport.Text = "Export";
            this.tsmiExport.Click += new System.EventHandler(this.tsmiExport_Click); // Evento que sucede en `ModelTree.cs`
            ....
        ....
        private System.Windows.Forms.ToolStripMenuItem tsmiExport;
        ....
```
Lo agregamos al `this.cmsTree.Items.AddRange`. Establecemos el nombre, text, el size, y su evento que en ModelTree, para invocar el de FrmMain. Y establecemos la variable junto a las demas.

---

## `PrePoMax/Forms/FrmMain.cs`
En `private void FrmMain_Load(object sender`. le indicamos al model tree el export event.
```csharp
_modelTree.ExportEvent += ModelTree_ExportEvent; // Evento de model tree WeldingTrajectory Export
```

Creamos el `private void Model_ExportEvent` referenciado.
```csharp
        private void ModelTree_ExportEvent(NamedClass namedClass, string stepName)
        {
            if (namedClass is CoordPointSet)
            {
                // WeldingTrajectory. Exportar set de coordenadas
                // Modo chido. PrePomax Style. New Button. CoordPointSet.
                //ExportWeldingTrajectory(namedClass.Name);
                MessageBox.Show($"Export CoordPointSet {namedClass.Name}");
            }
        }
```

---

## `UserControls/ModelTree.cs`
En `public struct ContextMenuFields`
```csharp
        public int Export;
```
Lo agregamos al final.

En el `public string[] IntersectSelectionWithList(NamedClass[] list)`. Agregamos la acción de exportar.
```csharp
        public event Action<NamedClass, string> ExportEvent; // WedingTrajectory CoordPointSet Export
```

En `private void PrepareToolStripItem(CodersLabTreeView tree)`. Preparamos el tsmi export.
```csharp
            // Export WeldingTrajectory CoordPointSet. Hacer que se vea o no.
            visible = menuFields.Export == n;
            tsmiExport.Visible = visible;
            oneAboveVisible |= visible;
```

En el `private void AppendMenuFields(TreeNode node, ref ContextMenuFields menuFields)`. Indicamos que nods podran tener la opción export.
```csharp
            // Export WeldingTrajectory
            if (CanExport(node)) menuFields.Export++;
```

Creamos el click referenciado.
```csharp
        private void tsmiExport_Click(object sender, EventArgs e)
        {
            try
            {
                CodersLabTreeView tree = GetActiveTree();
                if (tree.SelectedNodes.Count != 1) return;
                //
                TreeNode selectedNode = tree.SelectedNode;
                //
                if (selectedNode.Tag == null) return;
                //
                if ( CanExport(selectedNode) )
                {
                    string stepName = null;

                    if (
                        selectedNode.Parent != null &&
                        selectedNode.Parent.Parent != null &&
                        selectedNode.Parent.Tag is Step
                    )
                        stepName = selectedNode.Parent.Parent.Name;
                    //
                    ExportEvent?.Invoke((NamedClass)selectedNode.Tag, stepName); // Call Export event
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }
```
> WeldingTrajectory CoordPointSet Export basado en `tsmiEdit_Click`.

Creamos la func bool referenciada:
```csharp
        private bool CanExport(TreeNode node)
        {
            // Export option WeldingTrajectory CoordPointSet. No se necesita pero igual la dejo por trolling.
            if (node.Tag is CoordPointSet) return true;
            else return false;
        }
```