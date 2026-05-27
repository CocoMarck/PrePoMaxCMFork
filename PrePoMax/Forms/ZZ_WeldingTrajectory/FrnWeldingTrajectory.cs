// PrePoMax
using CaeGlobals;
using CaeMesh;

// CocoMarck
using PrePoMax.Utils;
using Utils.Text;

// C# Normal && WinForms
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
using static System.Net.Mime.MediaTypeNames;

namespace PrePoMax.Forms
{
    public partial class FrmWeldingTrajectory : UserControls.PrePoMaxChildForm
    {
        /*
        Este formulario es para la seleccion de cualquier punto de la superficie de un modelo.
        */

        // Variables
        private Controller _controller;

        // Widgets
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnExportPoints;
        private System.Windows.Forms.DataGridView dgvPoints;
        private System.Windows.Forms.Label lblPoints;
        private System.Windows.Forms.Label lblPointsNumber;
        private System.Windows.Forms.TextBox tbPointSetName;

        private System.Windows.Forms.Button btnAddCoordWithText;
        private System.Windows.Forms.Button btnAddCoordWithNode;
        private System.Windows.Forms.Button btnAddCoordWithPoint;

        // Callbacks
        public Action<string> Form_WriteDataToOutput;
        public Action<object, EventArgs> Form_RemoveAnnotations;

        // Para guardado en PMX File
        private CoordPointSet _coordPointSets;

        // Constructors
        public FrmWeldingTrajectory(Controller controller)
        {
            _controller = controller;
            _coordPointSets = new CoordPointSet("Welding_01");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Inicializar todos los widgets.
            this.btnClose = new System.Windows.Forms.Button();
            this.btnExportPoints = new System.Windows.Forms.Button();
            this.dgvPoints = new System.Windows.Forms.DataGridView();
            this.lblPoints = new System.Windows.Forms.Label();
            this.lblPointsNumber = new System.Windows.Forms.Label();
            this.tbPointSetName = new TextBox();

            this.btnAddCoordWithText = new System.Windows.Forms.Button();
            this.btnAddCoordWithNode = new System.Windows.Forms.Button();
            this.btnAddCoordWithPoint = new System.Windows.Forms.Button();

            // lblPointSetName
            tbPointSetName.Location = new Point(26, 26);
            tbPointSetName.Size = new Size(200, 23);
            tbPointSetName.Text = _coordPointSets.Name;
            tbPointSetName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbPointSetName.Leave += tbPointSetName_Leave;
            tbPointSetName.KeyDown += tbPointSetName_KeyDown;

            // btnClose
            btnClose.Name = "btnClose";
            btnClose.Text = "Close";
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnClose.Size = new Size(75, 23);
            btnClose.Location = new Point(26, 230);
            btnClose.Click += btnClose_Click;

            // lblPoints
            lblPoints.Text = "Points:";
            lblPoints.Location = new Point(26, 65);
            lblPoints.Size = new Size(75, 23);

            // lblPointsNumber
            lblPointsNumber.Text = "0";
            lblPointsNumber.Location = new Point(100, 65);
            lblPointsNumber.Size = new Size(75, 23);

            // dgvPoints
            dgvPoints.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPoints.Location = new Point(26, 88);
            dgvPoints.Size = new Size(200, 140);
            dgvPoints.AllowUserToAddRows = false;
            dgvPoints.RowHeadersVisible = false;
            dgvPoints.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPoints.Padding = new Padding(4, 0, 4, 0);
            dgvPoints.Columns.Add("id", "ID");
            dgvPoints.Columns.Add("x", "X");
            dgvPoints.Columns.Add("y", "Y");
            dgvPoints.Columns.Add("z", "Z");
            //dgvPoints.CellValidating += dgvPoints_CellValidating;
            //dgvPoints.RowValidating += dgvPoints_RowValidating;

            //// btnAddCoord buttons
            btnAddCoordWithText.Name = "btnAddCoordWithText";
            btnAddCoordWithText.Text = "Text";
            btnAddCoordWithText.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddCoordWithText.Size = new Size(80, 23);
            btnAddCoordWithText.Location = new Point(223, 88);
            btnAddCoordWithText.Click += btnAddCoordWithText_Click;

            btnAddCoordWithNode.Name = "btnAddCoordWithNode";
            btnAddCoordWithNode.Text = "Node";
            btnAddCoordWithNode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddCoordWithNode.Size = new Size(80, 23);
            btnAddCoordWithNode.Location = new Point(223, 111);
            btnAddCoordWithNode.Click += btnAddCoordWithNode_Click;

            btnAddCoordWithPoint.Name = "btnAddCoordWithPoint";
            btnAddCoordWithPoint.Text = "Point";
            btnAddCoordWithPoint.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddCoordWithPoint.Size = new Size(80, 23);
            btnAddCoordWithPoint.Location = new Point(223, 134);
            btnAddCoordWithPoint.Click += btnAddCoordWithPoint_Click;

            // btnExportPoints
            btnExportPoints.Name = "btnExportPoints";
            btnExportPoints.Text = "Export Points";
            btnExportPoints.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExportPoints.Size = new Size(100, 23);
            btnExportPoints.Location = new Point(126, 230);
            btnExportPoints.Click += btnExportPoints_Click;

            // FrmWeldingTrajectory
            this.Text = "Welding Trajectory";
            this.Name = "FrmWeldingTrajectory";
            this.ClientSize = new System.Drawing.Size(320, 256);
            this.CancelButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmWeldingTrajectory_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FrmWeldingTrajectory_VisibleChanged);
            this.SuspendLayout();
            this.ResumeLayout(false);

            // Add controls
            this.Controls.Add(tbPointSetName);
            this.Controls.Add(btnClose);
            this.Controls.Add(btnExportPoints);
            this.Controls.Add(dgvPoints);
            this.Controls.Add(lblPoints);
            this.Controls.Add(lblPointsNumber);
            this.Controls.Add(btnAddCoordWithText);
            this.Controls.Add(btnAddCoordWithNode);
            this.Controls.Add(btnAddCoordWithPoint);
        }

        // Eventos principales
        public void SetWeldingTrajectoryName(string coordPointSetName)
        {
            _coordPointSets.Name = coordPointSetName;
        }
        private void TryToGetAndHighlightWeldingTrajectory()
        {
            // PMX | Intentar obtener data.
            if (_controller.Model.Mesh.CoordPointSets.ContainsKey(_coordPointSets.Name))
            {
                _coordPointSets = _controller.Model.Mesh.CoordPointSets[_coordPointSets.Name];
                Highlight(); // Render
            }
            else
            {
                // Limpia mugrete temp.
                _coordPointSets = new CoordPointSet(_coordPointSets.Name);
                _controller.HighlightNodes(new double[0][]);
            }
            // Actualizar data
            RefreshPointList(); // GUI
            UpdatePointCount(); // GUI
            FocusInLastRow(); // GUI
        }
        public void PrepareForm()
        {
            // Controler
            if (_controller.Model.Mesh.CoordPointSets.Keys.Count > 0)
            {
                foreach (CoordPointSet value in _controller.Model.Mesh.CoordPointSets.Values)
                {
                    _coordPointSets = value;
                    tbPointSetName.Text = _coordPointSets.Name;
                    break;
                }
            }
            _controller.SetSelectByToOff();
            TryToGetAndHighlightWeldingTrajectory();
        }

        public void RemoveMeasureAnnotation()
        {
            _controller.Annotations.RemoveCurrentMeasureAnnotation();
        }

        private void FrmWeldingTrajectory_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Esto si no se que es. Parece que es forzar ocultado de form.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void FrmWeldingTrajectory_VisibleChanged(object sender, EventArgs e)
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
            // Supongo que esta función algun dia se usara. Pero por ahora no hace falta `2026-05-19`.
        }

        // Format Text
        private string FormatCoordPoint(double x, double y, double z)
        {
            return $"Coord point: {x:F2}, {y:F2}, {z:F2}";
        }

        // Widgets
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnAddCoordWithPoint_Click(object sender, EventArgs e)
        {
            _controller.SelectBy = vtkSelectBy.SurfacePoint;
            _controller.Selection.SelectItem = vtkSelectItem.SurfacePoint;
        }
        private void btnAddCoordWithNode_Click(object sender, EventArgs e)
        {
            _controller.SelectBy = vtkSelectBy.Node;
            _controller.Selection.SelectItem = vtkSelectItem.Node;
            RemoveMeasureAnnotation();
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
        }
        private void btnAddCoordWithText_Click(object sender, EventArgs e)
        {
            _controller.SelectBy = vtkSelectBy.Off;
            _controller.Selection.SelectItem = vtkSelectItem.None;

            // Agregar fila bien vacia en tabla.
            dgvPoints.Rows.Add("", "", "", "");

            // Focus en ultima fila hecha en tabla.
            FocusInLastRow();
        }

        // Funciones de tabla
        private void FocusInLastRow()
        {
            if (dgvPoints.Rows.Count == 0) { return; }
            int lastRowIndex = dgvPoints.Rows.Count - 1;
            dgvPoints.CurrentCell = dgvPoints.Rows[lastRowIndex].Cells[3];
        }
        // Validacion de datos en tabla
        private bool IsNumeric(string text)
        {
            double result;
            return double.TryParse(text, out result);
        }
        private bool PointTextPassFilter(string text)
        {
            if (TextFilter.PassTextFilter(text, "-1234567890."))
            {
                return IsNumeric(text);
            }
            return false;
        }
        private void dgvPoints_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Solo validar columnas 1, 2 y 3 (XYZ)
            if (e.ColumnIndex >= 1 && e.ColumnIndex <= 3)
            {
                string text = e.FormattedValue?.ToString();
                if (!PointTextPassFilter(text))
                {
                    dgvPoints.Rows[e.RowIndex].ErrorText = "Only double values";
                    e.Cancel = true; // no deja salir de la celda
                }
                else
                {
                    dgvPoints.Rows[e.RowIndex].ErrorText = string.Empty;
                }
            }
        }
        private void dgvPoints_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            var row = dgvPoints.Rows[e.RowIndex];

            bool ok =
                PointTextPassFilter(Convert.ToString(row.Cells[1].Value)) &&
                PointTextPassFilter(Convert.ToString(row.Cells[2].Value)) &&
                PointTextPassFilter(Convert.ToString(row.Cells[3].Value));

            if (!ok)
            {
                row.ErrorText = "Only double values";
                e.Cancel = true;
            }
            else
            {
                row.ErrorText = string.Empty;
            }
        }


        private void btnExportPoints_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*csv|All files (*.*)|*.*";
                sfd.Title = "Save file CSV";
                sfd.FileName = $"{_coordPointSets.Name}_Tejectories.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (CoordPointExporter.ExportXYZ(_coordPointSets, sfd.FileName))
                    {
                        MessageBox.Show($"Saved: `{sfd.FileName}`");
                    }
                }
            }
        }
        private void tbPointSetName_Leave(object sender, EventArgs e)
        {
            string name = tbPointSetName.Text.Trim();

            if (name == "") return;
            if (_coordPointSets.Name == name) return;

            _coordPointSets.Name = name;
            TryToGetAndHighlightWeldingTrajectory();
        }
        private void tbPointSetName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.ActiveControl = dgvPoints; // fuerza Leave
            }
        }


        private void RefreshPointList()
        {
            dgvPoints.Rows.Clear();
            foreach (var point in _coordPointSets.Points)
            {
                dgvPoints.Rows.Add(point.Id, point.Coor[0], point.Coor[1], point.Coor[2]);
            }
        }

        private void UpdatePointCount()
        {
            lblPointsNumber.Text = $"{dgvPoints.Rows.Count}";
        }

        // Render
        // Mostrar puntos
        public void Highlight()
        {
            if (_coordPointSets == null) return;
            if (_coordPointSets.Points.Count <= 0) return;

            double[][] points = _coordPointSets.Points.Select(p => p.Coor).ToArray();

            _controller.HighlightNodes(points);
        }


        public void AddSurfacePoint(double[] point)
        {
            if (point == null) return;

            int id = _controller.Model.Mesh.GetNextPointId();
            _coordPointSets.AddPoint(id, point[0], point[1], point[2]);

            Form_WriteDataToOutput(FormatCoordPoint(point[0], point[1], point[2]));

            // Modo de renderizar.
            Highlight(); // Render
            RefreshPointList(); // GUI
            UpdatePointCount(); // GUI
            FocusInLastRow(); // GUI

            // PMX Agregar al model mesh si aun no existe.
            if (!_controller.Model.Mesh.CoordPointSets.ContainsKey(_coordPointSets.Name))
            {
                _controller.Model.Mesh.CoordPointSets.Add(_coordPointSets.Name, _coordPointSets);
            }
        }

        // SelectBy
        public bool SelectByPoints()
        {
            return _controller.SelectBy == vtkSelectBy.SurfacePoint && _controller.Selection.SelectItem == vtkSelectItem.SurfacePoint;
        }
        public bool SelectByNodes()
        {
            return _controller.SelectBy == vtkSelectBy.Node && _controller.Selection.SelectItem == vtkSelectItem.Node;
        }
        public bool SelectByText()
        {
            return SelectByNodes() != true && SelectByPoints() != true;
        }
    }
}