using System;
using System.Drawing;
using System.Windows.Forms;
using MeatProductionApp.Classes;
using MySql.Data.MySqlClient;

namespace MeatProductionApp.Forms
{
    public partial class RawMaterialsForm : Form
    {
        private DataGridView dgvMaterials;
        private TextBox txtSearch;
        
        public RawMaterialsForm()
        {
            InitializeComponent();
            LoadData();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "УПРАВЛЕНИЕ СЫРЬЕМ НА СКЛАДЕ";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);
            
            // Search Panel
            Panel pnlSearch = new Panel();
            pnlSearch.Location = new Point(20, 70);
            pnlSearch.Size = new Size(900, 50);
            pnlSearch.BackColor = Color.White;
            pnlSearch.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pnlSearch);
            
            Label lblSearch = new Label();
            lblSearch.Text = "🔍 Поиск:";
            lblSearch.Font = new Font("Segoe UI", 10);
            lblSearch.Location = new Point(10, 15);
            lblSearch.AutoSize = true;
            pnlSearch.Controls.Add(lblSearch);
            
            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Location = new Point(90, 12);
            txtSearch.Size = new Size(300, 25);
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlSearch.Controls.Add(txtSearch);
            
            Button btnAdd = new Button();
            btnAdd.Text = "+ Добавить сырье";
            btnAdd.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAdd.Location = new Point(650, 10);
            btnAdd.Size = new Size(160, 30);
            btnAdd.BackColor = Color.FromArgb(46, 204, 113);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += BtnAdd_Click;
            pnlSearch.Controls.Add(btnAdd);
            
            Button btnRefresh = new Button();
            btnRefresh.Text = "🔄";
            btnRefresh.Font = new Font("Segoe UI", 12);
            btnRefresh.Location = new Point(820, 10);
            btnRefresh.Size = new Size(40, 30);
            btnRefresh.BackColor = Color.FromArgb(52, 152, 219);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadData();
            pnlSearch.Controls.Add(btnRefresh);
            
            // DataGridView
            dgvMaterials = new DataGridView();
            dgvMaterials.Location = new Point(20, 140);
            dgvMaterials.Size = new Size(900, 450);
            dgvMaterials.BackgroundColor = Color.White;
            dgvMaterials.BorderStyle = BorderStyle.FixedSingle;
            dgvMaterials.AllowUserToAddRows = false;
            dgvMaterials.AllowUserToDeleteRows = false;
            dgvMaterials.ReadOnly = true;
            dgvMaterials.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaterials.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMaterials.CellDoubleClick += DgvMaterials_CellDoubleClick;
            this.Controls.Add(dgvMaterials);
            
            // Context Menu
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            
            ToolStripMenuItem menuEdit = new ToolStripMenuItem("✏️ Редактировать");
            menuEdit.Click += MenuEdit_Click;
            contextMenu.Items.Add(menuEdit);
            
            ToolStripMenuItem menuAddSupply = new ToolStripMenuItem("📦 Добавить поставку");
            menuAddSupply.Click += MenuAddSupply_Click;
            contextMenu.Items.Add(menuAddSupply);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            
            ToolStripMenuItem menuDelete = new ToolStripMenuItem("🗑️ Удалить");
            menuDelete.Click += MenuDelete_Click;
            contextMenu.Items.Add(menuDelete);
            
            dgvMaterials.ContextMenuStrip = contextMenu;
            
            this.ResumeLayout(false);
        }
        
        private void LoadData(string searchText = "")
        {
            string query = @"SELECT 
                m.MaterialID AS 'ID',
                m.MaterialName AS 'Наименование',
                s.SupplierName AS 'Поставщик',
                m.Quantity AS 'Количество',
                m.Unit AS 'Ед.изм.',
                m.PricePerUnit AS 'Цена за ед.',
                m.MinStockLevel AS 'Мин.остаток',
                CASE 
                    WHEN m.Quantity < m.MinStockLevel THEN 'Критический'
                    WHEN m.Quantity < m.MinStockLevel * 1.5 THEN 'Низкий'
                    ELSE 'Норма'
                END AS 'Статус',
                DATE_FORMAT(m.LastSupplyDate, '%d.%m.%Y') AS 'Посл.поставка'
            FROM RawMaterials m
            LEFT JOIN Suppliers s ON m.SupplierID = s.SupplierID
            WHERE m.MaterialName LIKE @Search OR s.SupplierName LIKE @Search
            ORDER BY m.MaterialName";
            
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@Search", "%" + searchText + "%")
            };
            
            dgvMaterials.DataSource = DatabaseHelper.ExecuteQuery(query, parameters);
            
            // Скрываем ID
            if (dgvMaterials.Columns.Count > 0)
            {
                dgvMaterials.Columns["ID"].Visible = false;
                
                // Окраска строк по статусу
                foreach (DataGridViewRow row in dgvMaterials.Rows)
                {
                    string status = row.Cells["Статус"].Value?.ToString();
                    if (status == "Критический")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    else if (status == "Низкий")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 220);
                }
            }
        }
        
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text);
        }
        
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            RawMaterialEditForm editForm = new RawMaterialEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }
        
        private void DgvMaterials_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                EditMaterial();
            }
        }
        
        private void MenuEdit_Click(object sender, EventArgs e)
        {
            EditMaterial();
        }
        
        private void MenuAddSupply_Click(object sender, EventArgs e)
        {
            if (dgvMaterials.SelectedRows.Count > 0)
            {
                int materialId = Convert.ToInt32(dgvMaterials.SelectedRows[0].Cells["ID"].Value);
                AddSupplyForm form = new AddSupplyForm(materialId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }
        
        private void EditMaterial()
        {
            if (dgvMaterials.SelectedRows.Count > 0)
            {
                int materialId = Convert.ToInt32(dgvMaterials.SelectedRows[0].Cells["ID"].Value);
                RawMaterialEditForm editForm = new RawMaterialEditForm(materialId);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }
        
        private void MenuDelete_Click(object sender, EventArgs e)
        {
            if (dgvMaterials.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show(
                    "Вы действительно хотите удалить это сырье?", 
                    "Подтверждение удаления", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    int materialId = Convert.ToInt32(dgvMaterials.SelectedRows[0].Cells["ID"].Value);
                    string query = "DELETE FROM RawMaterials WHERE MaterialID = @MaterialID";
                    MySqlParameter[] parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@MaterialID", materialId)
                    };
                    
                    DatabaseHelper.ExecuteNonQuery(query, parameters);
                    DatabaseHelper.LogAction(User.CurrentUser.UserID, "Удаление сырья", 
                        "RawMaterials", materialId, null);
                    
                    MessageBox.Show("Сырье успешно удалено!", "Успех", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
        }
    }
    
    // Форма редактирования сырья
    public class RawMaterialEditForm : Form
    {
        private int? materialId;
        private TextBox txtName, txtQuantity, txtUnit, txtPrice, txtMinStock;
        private ComboBox cmbSupplier;
        
        public RawMaterialEditForm(int? id = null)
        {
            this.materialId = id;
            InitializeComponent();
            LoadSuppliers();
            if (id.HasValue) LoadMaterialData();
        }
        
        private void InitializeComponent()
        {
            this.Text = materialId.HasValue ? "Редактирование сырья" : "Добавление сырья";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            
            int yPos = 20;
            
            AddField("Наименование:", out txtName, ref yPos);
            AddComboField("Поставщик:", out cmbSupplier, ref yPos);
            AddField("Количество:", out txtQuantity, ref yPos);
            AddField("Ед. измерения:", out txtUnit, ref yPos);
            AddField("Цена за ед.:", out txtPrice, ref yPos);
            AddField("Мин. остаток:", out txtMinStock, ref yPos);
            
            Button btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(150, yPos + 20);
            btnSave.Size = new Size(120, 35);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            
            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(280, yPos + 20);
            btnCancel.Size = new Size(120, 35);
            btnCancel.BackColor = Color.FromArgb(149, 165, 166);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);
        }
        
        private void AddField(string label, out TextBox textBox, ref int yPos)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Location = new Point(20, yPos);
            lbl.AutoSize = true;
            this.Controls.Add(lbl);
            
            textBox = new TextBox();
            textBox.Location = new Point(150, yPos);
            textBox.Size = new Size(250, 25);
            this.Controls.Add(textBox);
            
            yPos += 40;
        }
        
        private void AddComboField(string label, out ComboBox comboBox, ref int yPos)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Location = new Point(20, yPos);
            lbl.AutoSize = true;
            this.Controls.Add(lbl);
            
            comboBox = new ComboBox();
            comboBox.Location = new Point(150, yPos);
            comboBox.Size = new Size(250, 25);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(comboBox);
            
            yPos += 40;
        }
        
        private void LoadSuppliers()
        {
            var dt = DatabaseHelper.ExecuteQuery("SELECT SupplierID, SupplierName FROM Suppliers WHERE IsActive = 1");
            cmbSupplier.DisplayMember = "SupplierName";
            cmbSupplier.ValueMember = "SupplierID";
            cmbSupplier.DataSource = dt;
        }
        
        private void LoadMaterialData()
        {
            string query = "SELECT * FROM RawMaterials WHERE MaterialID = @MaterialID";
            var dt = DatabaseHelper.ExecuteQuery(query, new MySqlParameter[] {
                new MySqlParameter("@MaterialID", materialId.Value)
            });
            
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                txtName.Text = row["MaterialName"].ToString();
                txtQuantity.Text = row["Quantity"].ToString();
                txtUnit.Text = row["Unit"].ToString();
                txtPrice.Text = row["PricePerUnit"].ToString();
                txtMinStock.Text = row["MinStockLevel"].ToString();
                cmbSupplier.SelectedValue = row["SupplierID"];
            }
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите наименование!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string query;
            if (materialId.HasValue)
            {
                query = @"UPDATE RawMaterials SET 
                    MaterialName = @Name, SupplierID = @SupplierID, 
                    Unit = @Unit, PricePerUnit = @Price, MinStockLevel = @MinStock
                    WHERE MaterialID = @MaterialID";
            }
            else
            {
                query = @"INSERT INTO RawMaterials 
                    (MaterialName, SupplierID, Quantity, Unit, PricePerUnit, MinStockLevel, LastSupplyDate) 
                    VALUES (@Name, @SupplierID, @Quantity, @Unit, @Price, @MinStock, NOW())";
            }
            
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@Name", txtName.Text),
                new MySqlParameter("@SupplierID", cmbSupplier.SelectedValue),
                new MySqlParameter("@Quantity", string.IsNullOrEmpty(txtQuantity.Text) ? 0 : decimal.Parse(txtQuantity.Text)),
                new MySqlParameter("@Unit", txtUnit.Text),
                new MySqlParameter("@Price", string.IsNullOrEmpty(txtPrice.Text) ? 0 : decimal.Parse(txtPrice.Text)),
                new MySqlParameter("@MinStock", string.IsNullOrEmpty(txtMinStock.Text) ? 0 : decimal.Parse(txtMinStock.Text)),
                new MySqlParameter("@MaterialID", materialId ?? 0)
            };
            
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            DatabaseHelper.LogAction(User.CurrentUser.UserID, 
                materialId.HasValue ? "Редактирование сырья" : "Добавление сырья", 
                "RawMaterials", materialId, txtName.Text);
            
            this.DialogResult = DialogResult.OK;
        }
    }
    
    // Форма добавления поставки
    public class AddSupplyForm : Form
    {
        private int materialId;
        private TextBox txtQuantity;
        private Label lblMaterial;
        
        public AddSupplyForm(int id)
        {
            this.materialId = id;
            InitializeComponent();
            LoadMaterialInfo();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Добавление поставки";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            
            Label lbl1 = new Label();
            lbl1.Text = "Сырье:";
            lbl1.Location = new Point(20, 20);
            lbl1.AutoSize = true;
            this.Controls.Add(lbl1);
            
            lblMaterial = new Label();
            lblMaterial.Location = new Point(20, 45);
            lblMaterial.Size = new Size(340, 40);
            lblMaterial.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblMaterial.ForeColor = Color.FromArgb(52, 73, 94);
            this.Controls.Add(lblMaterial);
            
            Label lbl2 = new Label();
            lbl2.Text = "Количество поставки:";
            lbl2.Location = new Point(20, 100);
            lbl2.AutoSize = true;
            this.Controls.Add(lbl2);
            
            txtQuantity = new TextBox();
            txtQuantity.Location = new Point(20, 125);
            txtQuantity.Size = new Size(200, 25);
            this.Controls.Add(txtQuantity);
            
            Button btnSave = new Button();
            btnSave.Text = "Добавить";
            btnSave.Location = new Point(120, 170);
            btnSave.Size = new Size(100, 30);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            
            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(230, 170);
            btnCancel.Size = new Size(100, 30);
            btnCancel.BackColor = Color.FromArgb(149, 165, 166);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);
        }
        
        private void LoadMaterialInfo()
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT MaterialName, Quantity, Unit FROM RawMaterials WHERE MaterialID = @ID",
                new MySqlParameter[] { new MySqlParameter("@ID", materialId) });
            
            if (dt.Rows.Count > 0)
            {
                lblMaterial.Text = $"{dt.Rows[0]["MaterialName"]} (текущий остаток: {dt.Rows[0]["Quantity"]} {dt.Rows[0]["Unit"]})";
            }
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("Введите количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            decimal quantity = decimal.Parse(txtQuantity.Text);
            string query = @"UPDATE RawMaterials 
                SET Quantity = Quantity + @Quantity, LastSupplyDate = NOW() 
                WHERE MaterialID = @MaterialID";
            
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("@Quantity", quantity),
                new MySqlParameter("@MaterialID", materialId)
            };
            
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            DatabaseHelper.LogAction(User.CurrentUser.UserID, "Добавление поставки", 
                "RawMaterials", materialId, $"Добавлено: {quantity}");
            
            MessageBox.Show("Поставка успешно добавлена!", "Успех", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
        }
    }
}
