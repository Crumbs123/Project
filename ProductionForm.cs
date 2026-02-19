using System;
using System.Data;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Linq;
using MeatProductionApp.Classes;
using MySql.Data.MySqlClient;
using Xceed.Words.NET;
using Xceed.Document.NET;

// Явные алиасы для устранения конфликта имён с Xceed.Document.NET
using Font        = System.Drawing.Font;
using FontStyle   = System.Drawing.FontStyle;
using Color       = System.Drawing.Color;
using Point       = System.Drawing.Point;
using Size        = System.Drawing.Size;
using BorderStyle = System.Windows.Forms.BorderStyle;

namespace MeatProductionApp.Forms
{
    public partial class ProductionForm : Form
    {
        private DataGridView dgvProduction;
        private DateTimePicker dtpFrom, dtpTo;
        
        public ProductionForm()
        {
            InitializeComponent();
            LoadData();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Управление производством";
            this.Size = new Size(960, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);
            
            Label lblTitle = new Label();
            lblTitle.Text = "УПРАВЛЕНИЕ ПРОИЗВОДСТВОМ";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);
            
            // Filter Panel
            Panel pnlFilter = new Panel();
            pnlFilter.Location = new Point(20, 70);
            pnlFilter.Size = new Size(900, 50);
            pnlFilter.BackColor = Color.White;
            pnlFilter.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pnlFilter);
            
            Label lblFrom = new Label();
            lblFrom.Text = "Период с:";
            lblFrom.Location = new Point(10, 15);
            lblFrom.AutoSize = true;
            pnlFilter.Controls.Add(lblFrom);
            
            dtpFrom = new DateTimePicker();
            dtpFrom.Location = new Point(80, 12);
            dtpFrom.Size = new Size(150, 25);
            dtpFrom.Format = DateTimePickerFormat.Short;
            dtpFrom.Value = DateTime.Now.AddMonths(-1);
            pnlFilter.Controls.Add(dtpFrom);
            
            Label lblTo = new Label();
            lblTo.Text = "по:";
            lblTo.Location = new Point(240, 15);
            lblTo.AutoSize = true;
            pnlFilter.Controls.Add(lblTo);
            
            dtpTo = new DateTimePicker();
            dtpTo.Location = new Point(270, 12);
            dtpTo.Size = new Size(150, 25);
            dtpTo.Format = DateTimePickerFormat.Short;
            pnlFilter.Controls.Add(dtpTo);
            
            Button btnFilter = new Button();
            btnFilter.Text = "Применить";
            btnFilter.Location = new Point(435, 10);
            btnFilter.Size = new Size(100, 30);
            btnFilter.BackColor = Color.FromArgb(52, 152, 219);
            btnFilter.ForeColor = Color.White;
            btnFilter.FlatStyle = FlatStyle.Flat;
            btnFilter.Click += (s, e) => LoadData();
            pnlFilter.Controls.Add(btnFilter);
            
            // Кнопка экспорта в Word
            Button btnExport = new Button();
            btnExport.Text = "📄 Отчёт в Word";
            btnExport.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnExport.Location = new Point(550, 10);
            btnExport.Size = new Size(130, 30);
            btnExport.BackColor = Color.FromArgb(52, 152, 219);
            btnExport.ForeColor = Color.White;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Cursor = Cursors.Hand;
            btnExport.Click += BtnExport_Click;
            pnlFilter.Controls.Add(btnExport);
            
            Button btnAdd = new Button();
            btnAdd.Text = "+ Новое производство";
            btnAdd.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAdd.Location = new Point(690, 10);
            btnAdd.Size = new Size(190, 30);
            btnAdd.BackColor = Color.FromArgb(46, 204, 113);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += BtnAdd_Click;
            pnlFilter.Controls.Add(btnAdd);
            
            // DataGridView
            dgvProduction = new DataGridView();
            dgvProduction.Location = new Point(20, 140);
            dgvProduction.Size = new Size(900, 450);
            dgvProduction.BackgroundColor = Color.White;
            dgvProduction.AllowUserToAddRows = false;
            dgvProduction.AllowUserToDeleteRows = false;
            dgvProduction.ReadOnly = true;
            dgvProduction.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProduction.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(dgvProduction);
            
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            
            ToolStripMenuItem viewItem = new ToolStripMenuItem("Просмотреть детали");
            viewItem.Click += ViewDetails_Click;
            contextMenu.Items.Add(viewItem);
            
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Удалить");
            deleteItem.Click += DeleteItem_Click;
            contextMenu.Items.Add(deleteItem);
            
            dgvProduction.ContextMenuStrip = contextMenu;
            
            this.ResumeLayout(false);
        }
        
        private void LoadData()
        {
            string query = @"SELECT 
                p.ProductionID AS 'ID',
                pr.ProductName AS 'Продукция',
                rm.MaterialName AS 'Сырье',
                p.MaterialUsed AS 'Использовано',
                p.ProductProduced AS 'Произведено',
                ROUND((p.ProductProduced / p.MaterialUsed) * 100, 2) AS 'Выход (%)',
                p.ProductionDate AS 'Дата',
                u.FullName AS 'Оператор'
            FROM Production p
            LEFT JOIN Products pr ON p.ProductID = pr.ProductID
            LEFT JOIN RawMaterials rm ON p.MaterialID = rm.MaterialID
            LEFT JOIN Users u ON p.UserID = u.UserID
            WHERE p.ProductionDate BETWEEN @From AND @To
            ORDER BY p.ProductionDate DESC";
            
            var parameters = new MySqlParameter[] {
                new MySqlParameter("@From", dtpFrom.Value.Date),
                new MySqlParameter("@To", dtpTo.Value.Date.AddDays(1))
            };
            
            dgvProduction.DataSource = DatabaseHelper.ExecuteQuery(query, parameters);
        }
        
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ProductionEditForm form = new ProductionEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }
        
        private void BtnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Word документ (*.docx)|*.docx";
            sfd.FileName = $"Отчёт_производство_{DateTime.Now:dd-MM-yyyy}.docx";
            
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ExportToWord(sfd.FileName);
            }
        }
        
        private void ExportToWord(string filename)
        {
            try
            {
                using (var doc = DocX.Create(filename))
                {
                    // ── Заголовок отчёта ──────────────────────────────────────────
                    var title = doc.InsertParagraph("ДЕТАЛЬНЫЙ ОТЧЁТ ПО ПРОИЗВОДСТВУ");
                    title.Alignment = Alignment.center;
                    title.Bold();
                    title.FontSize(18);
                    title.Color(Color.FromArgb(44, 62, 80));
                    title.SpacingAfter(6);

                    var subInfo = doc.InsertParagraph(
                        $"Период: с {dtpFrom.Value:dd.MM.yyyy} по {dtpTo.Value:dd.MM.yyyy}     " +
                        $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    subInfo.Alignment = Alignment.center;
                    subInfo.FontSize(10);
                    subInfo.Color(Color.Gray);
                    subInfo.SpacingAfter(14);

                    // ── Раздел: Записи производства ───────────────────────────────
                    var secHeader = doc.InsertParagraph("Записи производства");
                    secHeader.Bold();
                    secHeader.FontSize(13);
                    secHeader.Color(Color.FromArgb(52, 73, 94));
                    secHeader.SpacingAfter(4);

                    // Получаем видимые колонки
                    var visibleCols = new System.Collections.Generic.List<DataGridViewColumn>();
                    foreach (DataGridViewColumn col in dgvProduction.Columns)
                        if (col.Visible) visibleCols.Add(col);

                    int colCount = visibleCols.Count;
                    if (colCount > 0)
                    {
                        var table = doc.AddTable(dgvProduction.Rows.Count + 1, colCount);
                        table.Design = TableDesign.TableGrid;

                        // Заголовочная строка
                        for (int c = 0; c < colCount; c++)
                        {
                            var cell = table.Rows[0].Cells[c];
                            var p = cell.Paragraphs[0];
                            p.Append(visibleCols[c].HeaderText).Bold().FontSize(9);
                            p.Alignment = Alignment.center;
                            cell.FillColor = Color.FromArgb(52, 152, 219);
                            // белый текст через цвет
                            p.Color(Color.White);
                        }

                        // Строки данных
                        int rowIdx = 1;
                        foreach (DataGridViewRow row in dgvProduction.Rows)
                        {
                            if (row.IsNewRow) continue;
                            bool even = (rowIdx % 2 == 0);
                            for (int c = 0; c < colCount; c++)
                            {
                                var cell = table.Rows[rowIdx].Cells[c];
                                string val = row.Cells[visibleCols[c].Index].Value?.ToString() ?? "";
                                cell.Paragraphs[0].Append(val).FontSize(9);
                                if (even)
                                    cell.FillColor = Color.FromArgb(242, 242, 242);
                            }
                            rowIdx++;
                        }

                        doc.InsertTable(table);
                    }

                    doc.InsertParagraph("").SpacingAfter(6);

                    // ── Раздел: Сводная статистика ────────────────────────────────
                    var statsHeader = doc.InsertParagraph("Сводная статистика");
                    statsHeader.Bold();
                    statsHeader.FontSize(13);
                    statsHeader.Color(Color.FromArgb(52, 73, 94));
                    statsHeader.SpacingAfter(4);

                    var stats = DatabaseHelper.ExecuteQuery(@"
                        SELECT 
                            COUNT(*)                                           AS TotalRecords,
                            IFNULL(SUM(MaterialUsed), 0)                       AS TotalMaterial,
                            IFNULL(SUM(ProductProduced), 0)                    AS TotalProduct,
                            IFNULL(ROUND(AVG((ProductProduced/MaterialUsed)*100),2), 0) AS AvgYield
                        FROM Production
                        WHERE ProductionDate BETWEEN @From AND @To",
                        new MySqlParameter[] {
                            new MySqlParameter("@From", dtpFrom.Value.Date),
                            new MySqlParameter("@To", dtpTo.Value.Date.AddDays(1))
                        });

                    if (stats.Rows.Count > 0)
                    {
                        var r = stats.Rows[0];
                        var stTable = doc.AddTable(4, 2);
                        stTable.Design = TableDesign.TableGrid;

                        string[][] rows2 = {
                            new[] { "Всего записей производства",    $"{r["TotalRecords"]}" },
                            new[] { "Использовано сырья (кг)",       $"{Convert.ToDecimal(r["TotalMaterial"]):F2}" },
                            new[] { "Произведено продукции (кг)",    $"{Convert.ToDecimal(r["TotalProduct"]):F2}" },
                            new[] { "Средний выход (%)",             $"{Convert.ToDecimal(r["AvgYield"]):F2}" },
                        };

                        for (int i = 0; i < rows2.Length; i++)
                        {
                            stTable.Rows[i].Cells[0].Paragraphs[0].Append(rows2[i][0]).Bold().FontSize(9);
                            stTable.Rows[i].Cells[1].Paragraphs[0].Append(rows2[i][1]).FontSize(9);
                            stTable.Rows[i].Cells[0].FillColor = Color.FromArgb(235, 244, 255);
                        }

                        doc.InsertTable(stTable);
                    }

                    // ── Подвал ────────────────────────────────────────────────────
                    doc.InsertParagraph("").SpacingAfter(10);
                    var footer = doc.InsertParagraph(
                        $"Отчёт сформирован: {User.CurrentUser.FullName}   |   {DateTime.Now:dd.MM.yyyy HH:mm}");
                    footer.Alignment = Alignment.center;
                    footer.FontSize(9);
                    footer.Color(Color.Gray);

                    doc.Save();
                }

                MessageBox.Show($"Отчёт успешно создан!\n{filename}", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo(filename) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания отчёта: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ViewDetails_Click(object sender, EventArgs e)
        {
            if (dgvProduction.SelectedRows.Count == 0) return;
            
            int id = Convert.ToInt32(dgvProduction.SelectedRows[0].Cells["ID"].Value);
            
            var query = @"SELECT p.*, pr.ProductName, rm.MaterialName, u.FullName
                FROM Production p
                LEFT JOIN Products pr ON p.ProductID = pr.ProductID
                LEFT JOIN RawMaterials rm ON p.MaterialID = rm.MaterialID
                LEFT JOIN Users u ON p.UserID = u.UserID
                WHERE p.ProductionID = @ID";
            
            var dt = DatabaseHelper.ExecuteQuery(query, new MySqlParameter[] {
                new MySqlParameter("@ID", id)
            });
            
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                string details = $"Продукция: {row["ProductName"]}\n" +
                    $"Сырье: {row["MaterialName"]}\n" +
                    $"Использовано: {row["MaterialUsed"]} кг\n" +
                    $"Произведено: {row["ProductProduced"]} кг\n" +
                    $"Дата: {Convert.ToDateTime(row["ProductionDate"]):dd.MM.yyyy HH:mm}\n" +
                    $"Оператор: {row["FullName"]}\n" +
                    $"Примечания: {row["Notes"]}";
                
                MessageBox.Show(details, "Детали производства", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void DeleteItem_Click(object sender, EventArgs e)
        {
            if (dgvProduction.SelectedRows.Count == 0) return;
            
            var result = MessageBox.Show("Удалить выбранную запись производства?", 
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(dgvProduction.SelectedRows[0].Cells["ID"].Value);
                    
                    string query = "DELETE FROM Production WHERE ProductionID = @ID";
                    DatabaseHelper.ExecuteNonQuery(query, new MySqlParameter[] {
                        new MySqlParameter("@ID", id)
                    });
                    
                    LoadData();
                    MessageBox.Show("Запись удалена!", "Успех", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
    
    // ProductionEditForm - форма создания производства
    public class ProductionEditForm : Form
    {
        private ComboBox cmbProduct;
        private NumericUpDown nudQuantity;
        private DateTimePicker dtpDate;
        private TextBox txtNotes;
        private DataGridView dgvIngredients;
        private Label lblStatus;
        
        public ProductionEditForm()
        {
            InitializeComponent();
            LoadProducts();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Новое производство";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            Label lblProduct = new Label();
            lblProduct.Text = "Выберите продукцию:";
            lblProduct.Location = new Point(20, 20);
            lblProduct.Size = new Size(150, 20);
            this.Controls.Add(lblProduct);
            
            cmbProduct = new ComboBox();
            cmbProduct.Location = new Point(20, 45);
            cmbProduct.Size = new Size(300, 25);
            cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            this.Controls.Add(cmbProduct);
            
            Label lblQuantity = new Label();
            lblQuantity.Text = "Количество (кг):";
            lblQuantity.Location = new Point(350, 20);
            lblQuantity.Size = new Size(120, 20);
            this.Controls.Add(lblQuantity);
            
            nudQuantity = new NumericUpDown();
            nudQuantity.Location = new Point(350, 45);
            nudQuantity.Size = new Size(150, 25);
            nudQuantity.Maximum = 10000;
            nudQuantity.DecimalPlaces = 2;
            nudQuantity.ValueChanged += (s, e) => UpdateIngredients();
            this.Controls.Add(nudQuantity);
            
            Label lblDate = new Label();
            lblDate.Text = "Дата производства:";
            lblDate.Location = new Point(20, 85);
            lblDate.Size = new Size(150, 20);
            this.Controls.Add(lblDate);
            
            dtpDate = new DateTimePicker();
            dtpDate.Location = new Point(20, 110);
            dtpDate.Size = new Size(200, 25);
            this.Controls.Add(dtpDate);
            
            Label lblIngredients = new Label();
            lblIngredients.Text = "Требуемые ингредиенты:";
            lblIngredients.Location = new Point(20, 150);
            lblIngredients.Size = new Size(200, 20);
            lblIngredients.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.Controls.Add(lblIngredients);
            
            dgvIngredients = new DataGridView();
            dgvIngredients.Location = new Point(20, 175);
            dgvIngredients.Size = new Size(640, 200);
            dgvIngredients.AllowUserToAddRows = false;
            dgvIngredients.AllowUserToDeleteRows = false;
            dgvIngredients.ReadOnly = true;
            dgvIngredients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(dgvIngredients);
            
            lblStatus = new Label();
            lblStatus.Location = new Point(20, 385);
            lblStatus.Size = new Size(640, 60);
            lblStatus.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.Controls.Add(lblStatus);
            
            Label lblNotes = new Label();
            lblNotes.Text = "Примечания:";
            lblNotes.Location = new Point(20, 455);
            lblNotes.Size = new Size(100, 20);
            this.Controls.Add(lblNotes);
            
            txtNotes = new TextBox();
            txtNotes.Location = new Point(20, 480);
            txtNotes.Size = new Size(640, 20);
            this.Controls.Add(txtNotes);
            
            Button btnSave = new Button();
            btnSave.Text = "Создать производство";
            btnSave.Location = new Point(400, 515);
            btnSave.Size = new Size(180, 35);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            
            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new Point(300, 515);
            btnCancel.Size = new Size(80, 35);
            btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);
        }
        
        private void LoadProducts()
        {
            var query = @"SELECT p.ProductID, p.ProductName 
                FROM Products p
                INNER JOIN Recipes r ON p.ProductID = r.ProductID
                GROUP BY p.ProductID, p.ProductName
                ORDER BY p.ProductName";
            
            var dt = DatabaseHelper.ExecuteQuery(query);
            cmbProduct.DisplayMember = "ProductName";
            cmbProduct.ValueMember = "ProductID";
            cmbProduct.DataSource = dt;
        }
        
        private void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateIngredients();
        }
        
        private void UpdateIngredients()
        {
            if (cmbProduct.SelectedValue == null) return;
            
            int productId = Convert.ToInt32(cmbProduct.SelectedValue);
            decimal quantity = nudQuantity.Value;
            decimal coef = quantity / 100m; // коэффициент от 100 кг
            
            var query = @"SELECT 
                r.MaterialID,
                rm.MaterialName AS 'Ингредиент',
                r.RequiredQuantity AS 'Требуется на 100кг',
                ROUND(r.RequiredQuantity * @Coef, 2) AS 'Требуется',
                rm.Quantity AS 'На складе',
                r.Unit AS 'Ед.изм.'
            FROM Recipes r
            JOIN RawMaterials rm ON r.MaterialID = rm.MaterialID
            WHERE r.ProductID = @ProductID
            ORDER BY r.RequiredQuantity DESC";
            
            var parameters = new MySqlParameter[] {
                new MySqlParameter("@ProductID", productId),
                new MySqlParameter("@Coef", coef)
            };
            
            dgvIngredients.DataSource = DatabaseHelper.ExecuteQuery(query, parameters);
            dgvIngredients.Columns["MaterialID"].Visible = false;
            
            CheckAvailability();
        }
        
        private void CheckAvailability()
        {
            if (dgvIngredients.Rows.Count == 0) return;
            
            bool allAvailable = true;
            StringBuilder warnings = new StringBuilder();
            
            foreach (DataGridViewRow row in dgvIngredients.Rows)
            {
                if (row.IsNewRow) continue;
                
                decimal required = Convert.ToDecimal(row.Cells["Требуется"].Value);
                decimal available = Convert.ToDecimal(row.Cells["На складе"].Value);
                string ingredient = row.Cells["Ингредиент"].Value.ToString();
                
                if (required > available)
                {
                    allAvailable = false;
                    warnings.AppendLine($"⚠️ {ingredient}: требуется {required:F2} кг, доступно {available:F2} кг");
                }
            }
            
            if (allAvailable)
            {
                lblStatus.Text = "✓ Все ингредиенты доступны на складе";
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.Text = "❌ Недостаточно сырья:\n" + warnings.ToString();
                lblStatus.ForeColor = Color.Red;
            }
        }
        
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите продукцию!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (nudQuantity.Value <= 0)
            {
                MessageBox.Show("Укажите количество продукции!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                int productId = Convert.ToInt32(cmbProduct.SelectedValue);
                decimal productQuantity = nudQuantity.Value;
                decimal coef = productQuantity / 100m;
                
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (DataGridViewRow row in dgvIngredients.Rows)
                            {
                                if (row.IsNewRow) continue;
                                
                                int materialId = Convert.ToInt32(row.Cells["MaterialID"].Value);
                                decimal required = Convert.ToDecimal(row.Cells["Требуется"].Value);
                                
                                // Списываем сырье
                                string updateQuery = @"UPDATE RawMaterials 
                                    SET Quantity = Quantity - @Quantity 
                                    WHERE MaterialID = @MaterialID";
                                
                                using (var cmd = new MySqlCommand(updateQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@Quantity", required);
                                    cmd.Parameters.AddWithValue("@MaterialID", materialId);
                                    cmd.ExecuteNonQuery();
                                }
                                
                                // Создаем запись производства
                                string insertQuery = @"INSERT INTO Production 
                                    (ProductID, MaterialID, MaterialUsed, ProductProduced, 
                                     ProductionDate, UserID, Notes)
                                    VALUES (@ProductID, @MaterialID, @MaterialUsed, @ProductProduced,
                                            @Date, @UserID, @Notes)";
                                
                                using (var cmd = new MySqlCommand(insertQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@ProductID", productId);
                                    cmd.Parameters.AddWithValue("@MaterialID", materialId);
                                    cmd.Parameters.AddWithValue("@MaterialUsed", required);
                                    cmd.Parameters.AddWithValue("@ProductProduced", productQuantity);
                                    cmd.Parameters.AddWithValue("@Date", dtpDate.Value);
                                    cmd.Parameters.AddWithValue("@UserID", User.CurrentUser.UserID);
                                    cmd.Parameters.AddWithValue("@Notes", txtNotes.Text);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            
                            // Обновляем количество готовой продукции
                            string updateProductQuery = @"UPDATE Products 
                                SET Quantity = Quantity + @Quantity 
                                WHERE ProductID = @ProductID";
                            
                            using (var cmd = new MySqlCommand(updateProductQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Quantity", productQuantity);
                                cmd.Parameters.AddWithValue("@ProductID", productId);
                                cmd.ExecuteNonQuery();
                            }
                            
                            transaction.Commit();
                            
                            MessageBox.Show($"Производство завершено!\nПроизведено: {productQuantity} кг", 
                                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Ошибка транзакции: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
