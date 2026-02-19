using System;
using System.Drawing;
using System.Windows.Forms;
using MeatProductionApp.Classes;

namespace MeatProductionApp.Forms
{
    public partial class MainForm : Form
    {
        private Panel pnlContent;
        
        public MainForm()
        {
            InitializeComponent();
            LoadDashboard();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form
            this.ClientSize = new Size(1200, 700);
            this.Text = "Система управления производством копченых мясных изделий";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);
            this.WindowState = FormWindowState.Maximized; // ✅ Максимизируем окно
            
            // ✅ КРИТИЧЕСКИ ВАЖНО: ПОРЯДОК ДОБАВЛЕНИЯ КОНТРОЛОВ!
            // Dock-элементы добавляются В ОБРАТНОМ порядке их наложения
            
            // Content Panel - добавляем ПЕРВЫМ, но он будет под остальными
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = Color.FromArgb(236, 240, 241);
            pnlContent.Padding = new Padding(20);
            pnlContent.AutoScroll = true;
            this.Controls.Add(pnlContent);
            
            // Left Menu Panel - добавляем ВТОРЫМ, будет поверх контента слева
            Panel pnlMenu = new Panel();
            pnlMenu.Dock = DockStyle.Left;
            pnlMenu.Width = 220;
            pnlMenu.BackColor = Color.FromArgb(44, 62, 80);
            this.Controls.Add(pnlMenu);
            
            // Menu Buttons
            int yPos = 20;
            
            AddMenuButton(pnlMenu, "📊 Главная", ref yPos, (s, e) => LoadDashboard());
            AddMenuButton(pnlMenu, "📦 Сырье на складе", ref yPos, (s, e) => LoadRawMaterials());
            AddMenuButton(pnlMenu, "🏭 Производство", ref yPos, (s, e) => LoadProduction());
            AddMenuButton(pnlMenu, "🥓 Готовая продукция", ref yPos, (s, e) => LoadProducts());
            AddMenuButton(pnlMenu, "🚚 Отгрузка", ref yPos, (s, e) => LoadShipments());
            AddMenuButton(pnlMenu, "🏪 Магазины", ref yPos, (s, e) => LoadStores());
            AddMenuButton(pnlMenu, "🚛 Поставщики", ref yPos, (s, e) => LoadSuppliers());
            AddMenuButton(pnlMenu, "💰 Финансовый отчёт", ref yPos, (s, e) => LoadFinancialReport());
            
            if (User.CurrentUser.IsAdmin())
            {
                yPos += 20;
                Label lblAdmin = new Label();
                lblAdmin.Text = "АДМИНИСТРИРОВАНИЕ";
                lblAdmin.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                lblAdmin.ForeColor = Color.FromArgb(149, 165, 166);
                lblAdmin.Location = new Point(10, yPos);
                lblAdmin.Size = new Size(200, 20);
                pnlMenu.Controls.Add(lblAdmin);
                yPos += 25;
                
                AddMenuButton(pnlMenu, "👥 Пользователи", ref yPos, (s, e) => LoadUsers());
            }
            
            // Top Panel - добавляем ПОСЛЕДНИМ, будет поверх всех
            Panel pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 60;
            pnlTop.BackColor = Color.FromArgb(52, 73, 94);
            this.Controls.Add(pnlTop);
            
            // Title Label
            Label lblTitle = new Label();
            lblTitle.Text = "🥓 ПРОИЗВОДСТВО КОПЧЕНЫХ ИЗДЕЛИЙ";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;
            pnlTop.Controls.Add(lblTitle);
            
            // Logout Button - СНАЧАЛА кнопка справа
            Button btnLogout = new Button();
            btnLogout.Text = "Выход";
            btnLogout.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnLogout.Size = new Size(90, 35);
            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLogout.Location = new Point(this.ClientSize.Width - 110, 13);
            btnLogout.BackColor = Color.FromArgb(231, 76, 60);
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += BtnLogout_Click;
            pnlTop.Controls.Add(btnLogout);
            
            // User Info Label - СЛЕВА от кнопки
            Label lblUser = new Label();
            lblUser.Text = $"👤 {User.CurrentUser.FullName} ({User.CurrentUser.Role})";
            lblUser.Font = new Font("Segoe UI", 10);
            lblUser.ForeColor = Color.White;
            lblUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUser.AutoSize = true;
            lblUser.Location = new Point(this.ClientSize.Width - 320, 20);
            pnlTop.Controls.Add(lblUser);
            
            this.ResumeLayout(false);
        }
        
        private void AddMenuButton(Panel panel, string text, ref int yPos, EventHandler onClick)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 10);
            btn.ForeColor = Color.White;
            btn.Location = new Point(10, yPos);
            btn.Size = new Size(200, 40);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Cursor = Cursors.Hand;
            btn.Click += onClick;
            
            btn.MouseEnter += (s, e) => {
                btn.BackColor = Color.FromArgb(52, 73, 94);
            };
            btn.MouseLeave += (s, e) => {
                btn.BackColor = Color.Transparent;
            };
            
            panel.Controls.Add(btn);
            yPos += 45;
        }
        
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите выйти из системы?", 
                "Выход", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                DatabaseHelper.LogAction(User.CurrentUser.UserID, "Выход из системы", null, null, 
                    $"Пользователь {User.CurrentUser.Username} вышел из системы");
                
                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.ShowDialog();
                this.Close();
            }
        }
        
        private void ClearContent()
        {
            pnlContent.Controls.Clear();
        }
        
        private void LoadDashboard()
        {
            ClearContent();
            
            // ✅ Используем FlowLayoutPanel для адаптивной компоновки
            FlowLayoutPanel flowPanel = new FlowLayoutPanel();
            flowPanel.Dock = DockStyle.Top;
            flowPanel.AutoSize = true;
            flowPanel.FlowDirection = FlowDirection.TopDown;
            flowPanel.WrapContents = false;
            flowPanel.Padding = new Padding(0);
            pnlContent.Controls.Add(flowPanel);
            
            // Заголовок
            Label lblTitle = new Label();
            lblTitle.Text = "ПАНЕЛЬ УПРАВЛЕНИЯ";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitle.AutoSize = true;
            lblTitle.Margin = new Padding(0, 0, 0, 20);
            flowPanel.Controls.Add(lblTitle);
            
            // ✅ Контейнер для карточек статистики
            FlowLayoutPanel cardsPanel = new FlowLayoutPanel();
            cardsPanel.AutoSize = true;
            cardsPanel.FlowDirection = FlowDirection.LeftToRight;
            cardsPanel.WrapContents = true;
            cardsPanel.Width = pnlContent.Width - 60;
            cardsPanel.Margin = new Padding(0, 0, 0, 20);
            flowPanel.Controls.Add(cardsPanel);
            
            // Статистика в карточках
            var stats = new[] {
                new { Title = "Готовая продукция", Icon = "🥓", Query = "SELECT COUNT(*) FROM Products WHERE Quantity > 0", Color = Color.FromArgb(46, 204, 113) },
                new { Title = "Активные магазины", Icon = "🏪", Query = "SELECT COUNT(*) FROM Stores WHERE IsActive = 1", Color = Color.FromArgb(155, 89, 182) },
                new { Title = "Поставщики", Icon = "🚛", Query = "SELECT COUNT(*) FROM Suppliers WHERE IsActive = 1", Color = Color.FromArgb(230, 126, 34) }
            };
            
            foreach (var stat in stats)
            {
                Panel card = CreateStatCard(stat.Title, stat.Icon, stat.Query, stat.Color);
                card.Margin = new Padding(0, 0, 15, 0);
                cardsPanel.Controls.Add(card);
            }
            
            // ✅ Критические остатки
            Label lblWarning = new Label();
            lblWarning.Text = "⚠️ КРИТИЧЕСКИЕ ОСТАТКИ СЫРЬЯ";
            lblWarning.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblWarning.ForeColor = Color.FromArgb(231, 76, 60);
            lblWarning.AutoSize = true;
            lblWarning.Margin = new Padding(0, 20, 0, 10);
            flowPanel.Controls.Add(lblWarning);
            
            DataGridView dgvWarnings = new DataGridView();
            dgvWarnings.Width = pnlContent.Width - 60;
            dgvWarnings.Height = 200;
            dgvWarnings.BackgroundColor = Color.White;
            dgvWarnings.BorderStyle = BorderStyle.FixedSingle;
            dgvWarnings.AllowUserToAddRows = false;
            dgvWarnings.AllowUserToDeleteRows = false;
            dgvWarnings.ReadOnly = true;
            dgvWarnings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvWarnings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvWarnings.Margin = new Padding(0, 0, 0, 20);
            
            string query = @"SELECT MaterialName AS 'Сырье', Quantity AS 'Остаток', 
                           Unit AS 'Ед.изм.', MinStockLevel AS 'Минимум' 
                           FROM RawMaterials 
                           WHERE Quantity < MinStockLevel 
                           ORDER BY Quantity ASC";
            dgvWarnings.DataSource = DatabaseHelper.ExecuteQuery(query);
            flowPanel.Controls.Add(dgvWarnings);
            
            // ✅ Последняя активность
            Label lblActivity = new Label();
            lblActivity.Text = "📝 ПОСЛЕДНЯЯ АКТИВНОСТЬ";
            lblActivity.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblActivity.ForeColor = Color.FromArgb(52, 73, 94);
            lblActivity.AutoSize = true;
            lblActivity.Margin = new Padding(0, 20, 0, 10);
            flowPanel.Controls.Add(lblActivity);
            
            ListBox lstActivity = new ListBox();
            lstActivity.Width = pnlContent.Width - 60;
            lstActivity.Height = 150;
            lstActivity.Font = new Font("Segoe UI", 9);
            lstActivity.BorderStyle = BorderStyle.FixedSingle;
            
            try
            {
                var activityData = DatabaseHelper.ExecuteQuery(
                    @"SELECT CONCAT(u.FullName, ' - ', al.Action, ' (', DATE_FORMAT(al.CreatedAt, '%d.%m.%Y %H:%i'), ')') AS Activity 
                      FROM AuditLog al 
                      LEFT JOIN Users u ON al.UserID = u.UserID 
                      ORDER BY al.CreatedAt DESC 
                      LIMIT 10");
                
                foreach (System.Data.DataRow row in activityData.Rows)
                {
                    lstActivity.Items.Add(row["Activity"].ToString());
                }
            }
            catch
            {
                lstActivity.Items.Add("Нет данных об активности");
            }
            
            flowPanel.Controls.Add(lstActivity);
        }
        
        private Panel CreateStatCard(string title, string icon, string query, Color color)
        {
            Panel card = new Panel();
            card.Size = new Size(280, 140);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;
            
            // Цветная полоса сверху
            Panel colorBar = new Panel();
            colorBar.Dock = DockStyle.Top;
            colorBar.Height = 5;
            colorBar.BackColor = color;
            card.Controls.Add(colorBar);
            
            // Иконка
            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 40);
            lblIcon.Location = new Point(15, 20);
            lblIcon.Size = new Size(70, 70);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;
            card.Controls.Add(lblIcon);
            
            // Значение
            object count = null;
            try
            {
                count = DatabaseHelper.ExecuteScalar(query);
            }
            catch
            {
                count = "0";
            }
            
            Label lblValue = new Label();
            lblValue.Text = count?.ToString() ?? "0";
            lblValue.Font = new Font("Segoe UI", 32, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Location = new Point(100, 25);
            lblValue.AutoSize = true;
            card.Controls.Add(lblValue);
            
            // Название
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitle.Location = new Point(15, 100);
            lblTitle.Size = new Size(250, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            card.Controls.Add(lblTitle);
            
            return card;
        }
        
        // Остальные методы LoadRawMaterials, LoadProduction и т.д.
        // должны быть реализованы аналогично
        
        private void LoadRawMaterials()
        {
            ClearContent();
            RawMaterialsForm form = new RawMaterialsForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
        
        private void LoadProduction()
        {
            ClearContent();
            ProductionForm form = new ProductionForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
        
        private void LoadProducts()
        {
            ClearContent();
            ProductsForm form = new ProductsForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
        
        private void LoadShipments()
        {
            ClearContent();
            ShipmentsForm form = new ShipmentsForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
        
        private void LoadStores()
        {
            ClearContent();
            StoresForm form = new StoresForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
        
        private void LoadSuppliers()
        {
            ClearContent();
            SuppliersForm form = new SuppliersForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
        
        private void LoadFinancialReport()
        {
            ClearContent();
            FinancialReportForm form = new FinancialReportForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
        
        private void LoadUsers()
        {
            ClearContent();
            UsersForm form = new UsersForm();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
    }
}
