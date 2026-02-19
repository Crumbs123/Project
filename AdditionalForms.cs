using System;
using System.Drawing;
using System.Windows.Forms;
using MeatProductionApp.Classes;
using MySql.Data.MySqlClient;

namespace MeatProductionApp.Forms
{
    // ========== ФОРМА ГОТОВОЙ ПРОДУКЦИИ ==========
    public class ProductsForm : Form
    {
        private DataGridView dgv;
        
        public ProductsForm()
        {
            InitComponents();
            LoadData();
        }
        
        private void InitComponents()
        {
            Label lblTitle = new Label {
                Text = "ГОТОВАЯ ПРОДУКЦИЯ НА СКЛАДЕ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            
            dgv = new DataGridView {
                Location = new Point(20, 70),
                Size = new Size(900, 520),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgv);
        }
        
        private void LoadData()
        {
            string query = @"SELECT 
                ProductID AS 'ID',
                ProductName AS 'Наименование',
                ProductType AS 'Тип',
                Quantity AS 'На складе (кг)',
                PricePerUnit AS 'Цена за кг',
                Quantity * PricePerUnit AS 'Стоимость',
                SmokingTime AS 'Время копчения (ч)'
            FROM Products ORDER BY ProductName";
            
            dgv.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgv.Columns.Count > 0)
            {
                dgv.Columns["ID"].Visible = false;
                dgv.Columns["Стоимость"].DefaultCellStyle.Format = "N2";
            }
        }
    }
    
    // ========== ФОРМА ОТГРУЗОК ==========
    public class ShipmentsForm : Form
    {
        private DataGridView dgv;
        
        public ShipmentsForm()
        {
            InitComponents();
            LoadData();
        }
        
        private void InitComponents()
        {
            Label lblTitle = new Label {
                Text = "ОТГРУЗКИ В МАГАЗИНЫ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            
            Button btnAdd = new Button {
                Text = "+ Новая отгрузка",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(720, 15),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);
            
            dgv = new DataGridView {
                Location = new Point(20, 70),
                Size = new Size(900, 520),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgv);
            
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem menuStatus = new ToolStripMenuItem("📦 Изменить статус");
            menuStatus.Click += MenuStatus_Click;
            menu.Items.Add(menuStatus);
            dgv.ContextMenuStrip = menu;
        }
        
        private void LoadData()
        {
            string query = @"SELECT 
                sh.ShipmentID AS 'ID',
                st.StoreName AS 'Магазин',
                p.ProductName AS 'Продукция',
                sh.Quantity AS 'Количество (кг)',
                sh.TotalPrice AS 'Сумма',
                DATE_FORMAT(sh.ShipmentDate, '%d.%m.%Y') AS 'Дата отгрузки',
                sh.Status AS 'Статус',
                u.FullName AS 'Менеджер'
            FROM Shipments sh
            JOIN Stores st ON sh.StoreID = st.StoreID
            JOIN Products p ON sh.ProductID = p.ProductID
            LEFT JOIN Users u ON sh.UserID = u.UserID
            ORDER BY sh.ShipmentDate DESC";
            
            dgv.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgv.Columns.Count > 0)
            {
                dgv.Columns["ID"].Visible = false;
                
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    string status = row.Cells["Статус"].Value?.ToString();
                    if (status == "Доставлено")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
                    else if (status == "Отменено")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                }
            }
        }
        
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShipmentEditForm form = new ShipmentEditForm();
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }
        
        private void MenuStatus_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;
            
            int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["ID"].Value);
            string currentStatus = dgv.SelectedRows[0].Cells["Статус"].Value.ToString();
            
            Form statusForm = new Form {
                Text = "Изменить статус",
                Size = new Size(350, 180),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };
            
            Label lbl = new Label {
                Text = "Новый статус:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            statusForm.Controls.Add(lbl);
            
            ComboBox cmb = new ComboBox {
                Location = new Point(20, 45),
                Size = new Size(290, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmb.Items.AddRange(new[] { "Подготовка", "Отправлено", "Доставлено", "Отменено" });
            cmb.SelectedItem = currentStatus;
            statusForm.Controls.Add(cmb);
            
            Button btnSave = new Button {
                Text = "Сохранить",
                Location = new Point(120, 90),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += (s, ev) => {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Shipments SET Status = @Status WHERE ShipmentID = @ID",
                    new MySqlParameter[] {
                        new MySqlParameter("@Status", cmb.SelectedItem.ToString()),
                        new MySqlParameter("@ID", id)
                    });
                DatabaseHelper.LogAction(User.CurrentUser.UserID, "Изменение статуса отгрузки", 
                    "Shipments", id, $"{currentStatus} → {cmb.SelectedItem}");
                statusForm.DialogResult = DialogResult.OK;
            };
            statusForm.Controls.Add(btnSave);
            
            if (statusForm.ShowDialog() == DialogResult.OK)
                LoadData();
        }
    }
    
    // Форма создания отгрузки
    public class ShipmentEditForm : Form
    {
        private ComboBox cmbStore, cmbProduct;
        private TextBox txtQuantity;
        private DateTimePicker dtpDate;
        
        public ShipmentEditForm()
        {
            this.Text = "Новая отгрузка";
            this.Size = new Size(450, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            
            int y = 20;
            AddCombo("Магазин:", out cmbStore, ref y);
            AddCombo("Продукция:", out cmbProduct, ref y);
            AddText("Количество (кг):", out txtQuantity, ref y);
            AddDate("Дата отгрузки:", out dtpDate, ref y);
            
            LoadStores();
            LoadProducts();
            
            Button btnSave = new Button {
                Text = "Создать",
                Location = new Point(150, y + 20),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }
        
        private void AddCombo(string label, out ComboBox cmb, ref int y)
        {
            Label lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true };
            this.Controls.Add(lbl);
            cmb = new ComboBox {
                Location = new Point(170, y),
                Size = new Size(240, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmb);
            y += 40;
        }
        
        private void AddText(string label, out TextBox txt, ref int y)
        {
            Label lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true };
            this.Controls.Add(lbl);
            txt = new TextBox { Location = new Point(170, y), Size = new Size(240, 25) };
            this.Controls.Add(txt);
            y += 40;
        }
        
        private void AddDate(string label, out DateTimePicker dtp, ref int y)
        {
            Label lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true };
            this.Controls.Add(lbl);
            dtp = new DateTimePicker {
                Location = new Point(170, y),
                Size = new Size(240, 25),
                Format = DateTimePickerFormat.Short
            };
            this.Controls.Add(dtp);
            y += 40;
        }
        
        private void LoadStores()
        {
            var dt = DatabaseHelper.ExecuteQuery("SELECT StoreID, StoreName FROM Stores WHERE IsActive = 1");
            cmbStore.DisplayMember = "StoreName";
            cmbStore.ValueMember = "StoreID";
            cmbStore.DataSource = dt;
        }
        
        private void LoadProducts()
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT ProductID, CONCAT(ProductName, ' (', Quantity, ' кг)') AS DisplayName " +
                "FROM Products WHERE Quantity > 0");
            cmbProduct.DisplayMember = "DisplayName";
            cmbProduct.ValueMember = "ProductID";
            cmbProduct.DataSource = dt;
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbStore.SelectedValue == null || cmbProduct.SelectedValue == null || 
                string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            decimal quantity = decimal.Parse(txtQuantity.Text);
            
            // Проверка наличия
            object availableObj = DatabaseHelper.ExecuteScalar(
                "SELECT Quantity FROM Products WHERE ProductID = @ID",
                new MySqlParameter[] { new MySqlParameter("@ID", cmbProduct.SelectedValue) });
            
            if (availableObj == null || Convert.ToDecimal(availableObj) < quantity)
            {
                MessageBox.Show("Недостаточно продукции на складе!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Получаем цену
            object priceObj = DatabaseHelper.ExecuteScalar(
                "SELECT PricePerUnit FROM Products WHERE ProductID = @ID",
                new MySqlParameter[] { new MySqlParameter("@ID", cmbProduct.SelectedValue) });
            decimal price = Convert.ToDecimal(priceObj);
            
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO Shipments (StoreID, ProductID, Quantity, ShipmentDate, TotalPrice, UserID) 
                  VALUES (@StoreID, @ProductID, @Quantity, @Date, @Total, @UserID)",
                new MySqlParameter[] {
                    new MySqlParameter("@StoreID", cmbStore.SelectedValue),
                    new MySqlParameter("@ProductID", cmbProduct.SelectedValue),
                    new MySqlParameter("@Quantity", quantity),
                    new MySqlParameter("@Date", dtpDate.Value),
                    new MySqlParameter("@Total", quantity * price),
                    new MySqlParameter("@UserID", User.CurrentUser.UserID)
                });
            
            DatabaseHelper.LogAction(User.CurrentUser.UserID, "Создание отгрузки", 
                "Shipments", null, $"{cmbProduct.Text} → {cmbStore.Text}");
            
            MessageBox.Show("Отгрузка создана!", "Успех", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
        }
    }
    
    // ========== ФОРМА МАГАЗИНОВ ==========
    public class StoresForm : Form
    {
        private DataGridView dgv;
        
        public StoresForm()
        {
            InitComponents();
            LoadData();
        }
        
        private void InitComponents()
        {
            Label lblTitle = new Label {
                Text = "МАГАЗИНЫ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            
            Button btnAdd = new Button {
                Text = "+ Добавить магазин",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(720, 15),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => EditStore(null);
            this.Controls.Add(btnAdd);
            
            dgv = new DataGridView {
                Location = new Point(20, 70),
                Size = new Size(900, 520),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                    EditStore(Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["ID"].Value));
            };
            this.Controls.Add(dgv);
        }
        
        private void LoadData()
        {
            string query = @"SELECT 
                StoreID AS 'ID',
                StoreName AS 'Название',
                Address AS 'Адрес',
                ContactPerson AS 'Контактное лицо',
                Phone AS 'Телефон',
                CASE WHEN IsActive = 1 THEN 'Активен' ELSE 'Неактивен' END AS 'Статус'
            FROM Stores ORDER BY StoreName";
            
            dgv.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgv.Columns.Count > 0)
                dgv.Columns["ID"].Visible = false;
        }
        
        private void EditStore(int? id)
        {
            StoreEditForm form = new StoreEditForm(id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }
    }
    
    public class StoreEditForm : Form
    {
        private int? id;
        private TextBox txtName, txtAddress, txtContact, txtPhone, txtEmail;
        private CheckBox chkActive;
        
        public StoreEditForm(int? storeId)
        {
            this.id = storeId;
            this.Text = id.HasValue ? "Редактирование магазина" : "Добавление магазина";
            this.Size = new Size(450, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            
            int y = 20;
            AddText("Название:", out txtName, ref y);
            AddText("Адрес:", out txtAddress, ref y);
            AddText("Контактное лицо:", out txtContact, ref y);
            AddText("Телефон:", out txtPhone, ref y);
            AddText("Email:", out txtEmail, ref y);
            
            chkActive = new CheckBox {
                Text = "Активен",
                Location = new Point(170, y),
                Checked = true
            };
            this.Controls.Add(chkActive);
            y += 30;
            
            if (id.HasValue) LoadData();
            
            Button btnSave = new Button {
                Text = "Сохранить",
                Location = new Point(150, y + 20),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }
        
        private void AddText(string label, out TextBox txt, ref int y)
        {
            Label lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true };
            this.Controls.Add(lbl);
            txt = new TextBox { Location = new Point(170, y), Size = new Size(240, 25) };
            this.Controls.Add(txt);
            y += 40;
        }
        
        private void LoadData()
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM Stores WHERE StoreID = @ID",
                new MySqlParameter[] { new MySqlParameter("@ID", id.Value) });
            
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                txtName.Text = row["StoreName"].ToString();
                txtAddress.Text = row["Address"].ToString();
                txtContact.Text = row["ContactPerson"].ToString();
                txtPhone.Text = row["Phone"].ToString();
                txtEmail.Text = row["Email"].ToString();
                chkActive.Checked = Convert.ToBoolean(row["IsActive"]);
            }
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string query = id.HasValue ?
                @"UPDATE Stores SET StoreName=@Name, Address=@Address, ContactPerson=@Contact, 
                  Phone=@Phone, Email=@Email, IsActive=@Active WHERE StoreID=@ID" :
                @"INSERT INTO Stores (StoreName, Address, ContactPerson, Phone, Email, IsActive) 
                  VALUES (@Name, @Address, @Contact, @Phone, @Email, @Active)";
            
            DatabaseHelper.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@Name", txtName.Text),
                new MySqlParameter("@Address", txtAddress.Text),
                new MySqlParameter("@Contact", txtContact.Text),
                new MySqlParameter("@Phone", txtPhone.Text),
                new MySqlParameter("@Email", txtEmail.Text),
                new MySqlParameter("@Active", chkActive.Checked),
                new MySqlParameter("@ID", id ?? 0)
            });
            
            DatabaseHelper.LogAction(User.CurrentUser.UserID, 
                id.HasValue ? "Редактирование магазина" : "Добавление магазина", 
                "Stores", id, txtName.Text);
            
            this.DialogResult = DialogResult.OK;
        }
    }
    
    // ========== ФОРМА ПОСТАВЩИКОВ ==========
    public class SuppliersForm : Form
    {
        private DataGridView dgv;
        
        public SuppliersForm()
        {
            InitComponents();
            LoadData();
        }
        
        private void InitComponents()
        {
            Label lblTitle = new Label {
                Text = "ПОСТАВЩИКИ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            
            Button btnAdd = new Button {
                Text = "+ Добавить поставщика",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(680, 15),
                Size = new Size(240, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => EditSupplier(null);
            this.Controls.Add(btnAdd);
            
            dgv = new DataGridView {
                Location = new Point(20, 70),
                Size = new Size(900, 520),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                    EditSupplier(Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["ID"].Value));
            };
            this.Controls.Add(dgv);
        }
        
        private void LoadData()
        {
            string query = @"SELECT 
                SupplierID AS 'ID',
                SupplierName AS 'Название',
                ContactPerson AS 'Контактное лицо',
                Phone AS 'Телефон',
                Address AS 'Адрес',
                CASE WHEN IsActive = 1 THEN 'Активен' ELSE 'Неактивен' END AS 'Статус'
            FROM Suppliers ORDER BY SupplierName";
            
            dgv.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgv.Columns.Count > 0)
                dgv.Columns["ID"].Visible = false;
        }
        
        private void EditSupplier(int? id)
        {
            SupplierEditForm form = new SupplierEditForm(id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }
    }
    
    public class SupplierEditForm : Form
    {
        private int? id;
        private TextBox txtName, txtAddress, txtContact, txtPhone, txtEmail;
        private CheckBox chkActive;
        
        public SupplierEditForm(int? supplierId)
        {
            this.id = supplierId;
            this.Text = id.HasValue ? "Редактирование поставщика" : "Добавление поставщика";
            this.Size = new Size(450, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            
            int y = 20;
            AddText("Название:", out txtName, ref y);
            AddText("Адрес:", out txtAddress, ref y);
            AddText("Контактное лицо:", out txtContact, ref y);
            AddText("Телефон:", out txtPhone, ref y);
            AddText("Email:", out txtEmail, ref y);
            
            chkActive = new CheckBox {
                Text = "Активен",
                Location = new Point(170, y),
                Checked = true
            };
            this.Controls.Add(chkActive);
            y += 30;
            
            if (id.HasValue) LoadData();
            
            Button btnSave = new Button {
                Text = "Сохранить",
                Location = new Point(150, y + 20),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }
        
        private void AddText(string label, out TextBox txt, ref int y)
        {
            Label lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true };
            this.Controls.Add(lbl);
            txt = new TextBox { Location = new Point(170, y), Size = new Size(240, 25) };
            this.Controls.Add(txt);
            y += 40;
        }
        
        private void LoadData()
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM Suppliers WHERE SupplierID = @ID",
                new MySqlParameter[] { new MySqlParameter("@ID", id.Value) });
            
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                txtName.Text = row["SupplierName"].ToString();
                txtAddress.Text = row["Address"].ToString();
                txtContact.Text = row["ContactPerson"].ToString();
                txtPhone.Text = row["Phone"].ToString();
                txtEmail.Text = row["Email"].ToString();
                chkActive.Checked = Convert.ToBoolean(row["IsActive"]);
            }
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string query = id.HasValue ?
                @"UPDATE Suppliers SET SupplierName=@Name, Address=@Address, ContactPerson=@Contact, 
                  Phone=@Phone, Email=@Email, IsActive=@Active WHERE SupplierID=@ID" :
                @"INSERT INTO Suppliers (SupplierName, Address, ContactPerson, Phone, Email, IsActive) 
                  VALUES (@Name, @Address, @Contact, @Phone, @Email, @Active)";
            
            DatabaseHelper.ExecuteNonQuery(query, new MySqlParameter[] {
                new MySqlParameter("@Name", txtName.Text),
                new MySqlParameter("@Address", txtAddress.Text),
                new MySqlParameter("@Contact", txtContact.Text),
                new MySqlParameter("@Phone", txtPhone.Text),
                new MySqlParameter("@Email", txtEmail.Text),
                new MySqlParameter("@Active", chkActive.Checked),
                new MySqlParameter("@ID", id ?? 0)
            });
            
            DatabaseHelper.LogAction(User.CurrentUser.UserID, 
                id.HasValue ? "Редактирование поставщика" : "Добавление поставщика", 
                "Suppliers", id, txtName.Text);
            
            this.DialogResult = DialogResult.OK;
        }
    }
    
    // ========== ФОРМА УПРАВЛЕНИЯ ПОЛЬЗОВАТЕЛЯМИ (только для админа) ==========
    public class UsersForm : Form
    {
        private DataGridView dgv;
        
        public UsersForm()
        {
            InitComponents();
            LoadData();
        }
        
        private void InitComponents()
        {
            Label lblTitle = new Label {
                Text = "УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            
            Button btnAdd = new Button {
                Text = "+ Добавить пользователя",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(660, 15),
                Size = new Size(260, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => EditUser(null);
            this.Controls.Add(btnAdd);
            
            dgv = new DataGridView {
                Location = new Point(20, 70),
                Size = new Size(900, 520),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                    EditUser(Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["ID"].Value));
            };
            this.Controls.Add(dgv);
        }
        
        private void LoadData()
        {
            string query = @"SELECT 
                UserID AS 'ID',
                Username AS 'Логин',
                FullName AS 'ФИО',
                Role AS 'Роль',
                CASE WHEN IsActive = 1 THEN 'Активен' ELSE 'Заблокирован' END AS 'Статус',
                DATE_FORMAT(LastLogin, '%d.%m.%Y %H:%i') AS 'Последний вход'
            FROM Users ORDER BY FullName";
            
            dgv.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgv.Columns.Count > 0)
                dgv.Columns["ID"].Visible = false;
        }
        
        private void EditUser(int? id)
        {
            UserEditForm form = new UserEditForm(id);
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }
    }
    
    public class UserEditForm : Form
    {
        private int? id;
        private TextBox txtUsername, txtPassword, txtFullName;
        private ComboBox cmbRole;
        private CheckBox chkActive;
        
        public UserEditForm(int? userId)
        {
            this.id = userId;
            this.Text = id.HasValue ? "Редактирование пользователя" : "Добавление пользователя";
            this.Size = new Size(450, 340);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            
            int y = 20;
            AddText("Логин:", out txtUsername, ref y);
            AddText("Пароль:", out txtPassword, ref y);
            AddText("ФИО:", out txtFullName, ref y);
            
            Label lblRole = new Label { Text = "Роль:", Location = new Point(20, y), AutoSize = true };
            this.Controls.Add(lblRole);
            cmbRole = new ComboBox {
                Location = new Point(170, y),
                Size = new Size(240, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new[] { "Admin", "Manager" });
            cmbRole.SelectedIndex = 1;
            this.Controls.Add(cmbRole);
            y += 40;
            
            chkActive = new CheckBox {
                Text = "Активен",
                Location = new Point(170, y),
                Checked = true
            };
            this.Controls.Add(chkActive);
            y += 30;
            
            if (id.HasValue) LoadData();
            
            Button btnSave = new Button {
                Text = "Сохранить",
                Location = new Point(150, y + 20),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }
        
        private void AddText(string label, out TextBox txt, ref int y)
        {
            Label lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true };
            this.Controls.Add(lbl);
            txt = new TextBox { Location = new Point(170, y), Size = new Size(240, 25) };
            this.Controls.Add(txt);
            y += 40;
        }
        
        private void LoadData()
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM Users WHERE UserID = @ID",
                new MySqlParameter[] { new MySqlParameter("@ID", id.Value) });
            
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                txtUsername.Text = row["Username"].ToString();
                txtFullName.Text = row["FullName"].ToString();
                cmbRole.SelectedItem = row["Role"].ToString();
                chkActive.Checked = Convert.ToBoolean(row["IsActive"]);
                txtPassword.PlaceholderText = "Оставьте пустым, чтобы не менять";
            }
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Заполните обязательные поля!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!id.HasValue && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Введите пароль для нового пользователя!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string query;
            MySqlParameter[] parameters;
            
            if (id.HasValue)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    query = @"UPDATE Users SET Username=@Username, FullName=@FullName, 
                             Role=@Role, IsActive=@Active WHERE UserID=@ID";
                    parameters = new MySqlParameter[] {
                        new MySqlParameter("@Username", txtUsername.Text),
                        new MySqlParameter("@FullName", txtFullName.Text),
                        new MySqlParameter("@Role", cmbRole.SelectedItem.ToString()),
                        new MySqlParameter("@Active", chkActive.Checked),
                        new MySqlParameter("@ID", id.Value)
                    };
                }
                else
                {
                    query = @"UPDATE Users SET Username=@Username, PasswordHash=@Password, 
                             FullName=@FullName, Role=@Role, IsActive=@Active WHERE UserID=@ID";
                    parameters = new MySqlParameter[] {
                        new MySqlParameter("@Username", txtUsername.Text),
                        new MySqlParameter("@Password", User.HashPassword(txtPassword.Text)),
                        new MySqlParameter("@FullName", txtFullName.Text),
                        new MySqlParameter("@Role", cmbRole.SelectedItem.ToString()),
                        new MySqlParameter("@Active", chkActive.Checked),
                        new MySqlParameter("@ID", id.Value)
                    };
                }
            }
            else
            {
                query = @"INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive) 
                         VALUES (@Username, @Password, @FullName, @Role, @Active)";
                parameters = new MySqlParameter[] {
                    new MySqlParameter("@Username", txtUsername.Text),
                    new MySqlParameter("@Password", User.HashPassword(txtPassword.Text)),
                    new MySqlParameter("@FullName", txtFullName.Text),
                    new MySqlParameter("@Role", cmbRole.SelectedItem.ToString()),
                    new MySqlParameter("@Active", chkActive.Checked)
                };
            }
            
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            DatabaseHelper.LogAction(User.CurrentUser.UserID, 
                id.HasValue ? "Редактирование пользователя" : "Добавление пользователя", 
                "Users", id, txtUsername.Text);
            
            this.DialogResult = DialogResult.OK;
        }
    }
}
