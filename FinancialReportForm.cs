using System;
using System.Data;
using System.Windows.Forms;
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
using System.Drawing;

namespace MeatProductionApp.Forms
{
    /// <summary>
    /// Форма финансового отчёта — показывает выручку, себестоимость, прибыль
    /// и расходы на сырьё за выбранный период. Поддерживает экспорт в Word.
    /// </summary>
    public class FinancialReportForm : Form
    {
        private DateTimePicker dtpFrom, dtpTo;
        private DataGridView dgvRevenue;     // выручка по продуктам
        private DataGridView dgvCosts;       // затраты на сырьё
        private Label lblTotalRevenue, lblTotalCosts, lblTotalProfit, lblMargin;

        public FinancialReportForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Финансовый отчёт";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // ── Заголовок ──────────────────────────────────────────────────
            Label lblTitle = new Label
            {
                Text = "💰 ФИНАНСОВЫЙ ОТЧЁТ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // ── Панель фильтра ─────────────────────────────────────────────
            Panel pnlFilter = new Panel
            {
                Location = new Point(20, 55),
                Size = new Size(1040, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlFilter);

            pnlFilter.Controls.Add(new Label { Text = "Период с:", Location = new Point(10, 16), AutoSize = true });

            dtpFrom = new DateTimePicker
            {
                Location = new Point(80, 12),
                Size = new Size(140, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-3)
            };
            pnlFilter.Controls.Add(dtpFrom);

            pnlFilter.Controls.Add(new Label { Text = "по:", Location = new Point(232, 16), AutoSize = true });

            dtpTo = new DateTimePicker
            {
                Location = new Point(255, 12),
                Size = new Size(140, 25),
                Format = DateTimePickerFormat.Short
            };
            pnlFilter.Controls.Add(dtpTo);

            Button btnApply = new Button
            {
                Text = "Применить",
                Location = new Point(410, 10),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnApply.Click += (s, e) => LoadData();
            pnlFilter.Controls.Add(btnApply);

            Button btnExport = new Button
            {
                Text = "📄 Отчёт в Word",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(525, 10),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExport.Click += BtnExport_Click;
            pnlFilter.Controls.Add(btnExport);

            // ── Карточки KPI ───────────────────────────────────────────────
            Panel pnlKpi = new Panel
            {
                Location = new Point(20, 120),
                Size = new Size(1040, 90),
                BackColor = Color.Transparent
            };
            this.Controls.Add(pnlKpi);

            lblTotalRevenue = CreateKpiLabel(pnlKpi, "💵 Выручка", 0);
            lblTotalCosts   = CreateKpiLabel(pnlKpi, "📦 Себестоимость", 265);
            lblTotalProfit  = CreateKpiLabel(pnlKpi, "📈 Прибыль", 530);
            lblMargin       = CreateKpiLabel(pnlKpi, "📊 Рентабельность", 795);

            // ── Таблица: Выручка по продукции ──────────────────────────────
            Label lblRev = new Label
            {
                Text = "Выручка по видам продукции",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 225),
                AutoSize = true
            };
            this.Controls.Add(lblRev);

            dgvRevenue = CreateGrid(new Point(20, 250), new Size(505, 380));
            this.Controls.Add(dgvRevenue);

            // ── Таблица: Затраты на сырьё ──────────────────────────────────
            Label lblCosts = new Label
            {
                Text = "Затраты на сырьё",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(555, 225),
                AutoSize = true
            };
            this.Controls.Add(lblCosts);

            dgvCosts = CreateGrid(new Point(555, 250), new Size(505, 380));
            this.Controls.Add(dgvCosts);

            this.ResumeLayout(false);
        }

        // Создаёт карточку KPI и возвращает Label со значением
        private Label CreateKpiLabel(Panel parent, string title, int x)
        {
            Panel card = new Panel
            {
                Location = new Point(x, 0),
                Size = new Size(245, 85),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Panel bar = new Panel { Dock = DockStyle.Top, Height = 4, BackColor = Color.FromArgb(52, 152, 219) };
            card.Controls.Add(bar);

            Label lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(10, 12),
                AutoSize = true
            };
            card.Controls.Add(lbl);

            Label lblValue = new Label
            {
                Text = "—",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(10, 35),
                Size = new Size(225, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblValue);

            parent.Controls.Add(card);
            return lblValue;
        }

        private DataGridView CreateGrid(Point location, Size size)
        {
            return new DataGridView
            {
                Location = location,
                Size = size,
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        // ── Загрузка данных ────────────────────────────────────────────────
        private void LoadData()
        {
            DateTime from = dtpFrom.Value.Date;
            DateTime to   = dtpTo.Value.Date.AddDays(1);

            var pFrom = new MySqlParameter("@From", from);
            var pTo   = new MySqlParameter("@To", to);

            // 1. Выручка по продуктам через отгрузки
            // Таблица Shipments содержит: StoreID, ProductID, Quantity, ShipmentDate, TotalPrice
            var revQuery = @"
                SELECT
                    p.ProductName                        AS 'Продукт',
                    ROUND(SUM(s.Quantity), 2)            AS 'Отгружено (кг)',
                    MAX(p.PricePerUnit)                  AS 'Цена/кг',
                    ROUND(SUM(s.TotalPrice), 2)          AS 'Выручка (руб.)'
                FROM Shipments s
                JOIN Products p ON s.ProductID = p.ProductID
                WHERE s.ShipmentDate BETWEEN @From AND @To
                GROUP BY p.ProductID, p.ProductName
                ORDER BY SUM(s.TotalPrice) DESC";

            dgvRevenue.DataSource = DatabaseHelper.ExecuteQuery(revQuery, new[] { pFrom, pTo });

            // 2. Затраты на сырьё через производство
            var costQuery = @"
                SELECT
                    rm.MaterialName                               AS 'Сырьё',
                    ROUND(SUM(pr.MaterialUsed), 2)               AS 'Использовано (кг)',
                    IFNULL(MAX(rm.PricePerUnit), 0)              AS 'Цена/кг',
                    ROUND(SUM(pr.MaterialUsed * IFNULL(rm.PricePerUnit,0)), 2) AS 'Затраты (руб.)'
                FROM Production pr
                JOIN RawMaterials rm ON pr.MaterialID = rm.MaterialID
                WHERE pr.ProductionDate BETWEEN @From AND @To
                GROUP BY rm.MaterialID, rm.MaterialName
                ORDER BY `Затраты (руб.)` DESC";

            dgvCosts.DataSource = DatabaseHelper.ExecuteQuery(costQuery, new[] { pFrom, pTo });

            // 3. KPI
            var kpiQuery = @"
                SELECT
                    IFNULL((
                        SELECT ROUND(SUM(s2.TotalPrice), 2)
                        FROM Shipments s2
                        WHERE s2.ShipmentDate BETWEEN @From AND @To
                    ), 0) AS Revenue,
                    IFNULL((
                        SELECT ROUND(SUM(pr2.MaterialUsed * IFNULL(rm2.PricePerUnit,0)), 2)
                        FROM Production pr2
                        JOIN RawMaterials rm2 ON pr2.MaterialID = rm2.MaterialID
                        WHERE pr2.ProductionDate BETWEEN @From AND @To
                    ), 0) AS Costs";

            var kpi = DatabaseHelper.ExecuteQuery(kpiQuery, new[] { pFrom, pTo });
            if (kpi.Rows.Count > 0)
            {
                decimal revenue = Convert.ToDecimal(kpi.Rows[0]["Revenue"]);
                decimal costs   = Convert.ToDecimal(kpi.Rows[0]["Costs"]);
                decimal profit  = revenue - costs;
                decimal margin  = revenue > 0 ? Math.Round(profit / revenue * 100, 1) : 0;

                lblTotalRevenue.Text = $"{revenue:N0} руб.";
                lblTotalCosts.Text   = $"{costs:N0} руб.";
                lblTotalProfit.Text  = $"{profit:N0} руб.";
                lblTotalProfit.ForeColor = profit >= 0
                    ? Color.FromArgb(39, 174, 96)
                    : Color.FromArgb(231, 76, 60);
                lblMargin.Text = $"{margin} %";
                lblMargin.ForeColor = margin >= 20
                    ? Color.FromArgb(39, 174, 96)
                    : Color.FromArgb(230, 126, 34);
            }
        }

        // ── Экспорт в Word ─────────────────────────────────────────────────
        private void BtnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Word документ (*.docx)|*.docx",
                FileName = $"Финансовый_отчёт_{DateTime.Now:dd-MM-yyyy}.docx"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
                ExportToWord(sfd.FileName);
        }

        private void ExportToWord(string filename)
        {
            try
            {
                DateTime from = dtpFrom.Value.Date;
                DateTime to   = dtpTo.Value.Date;

                using (var doc = DocX.Create(filename))
                {
                    // ── Заголовок ──────────────────────────────────────────
                    var title = doc.InsertParagraph("ФИНАНСОВЫЙ ОТЧЁТ");
                    title.Alignment = Alignment.center;
                    title.Bold().FontSize(20);
                    title.Color(Color.FromArgb(44, 62, 80));
                    title.SpacingAfter(4);

                    var sub = doc.InsertParagraph(
                        $"Период: {from:dd.MM.yyyy} — {to:dd.MM.yyyy}     " +
                        $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}     " +
                        $"Составил: {User.CurrentUser.FullName}");
                    sub.Alignment = Alignment.center;
                    sub.FontSize(10).Color(Color.Gray);
                    sub.SpacingAfter(16);

                    // ── KPI сводка ────────────────────────────────────────
                    doc.InsertParagraph("Ключевые показатели")
                       .Bold().FontSize(13)
                       .Color(Color.FromArgb(52, 73, 94))
                       .SpacingAfter(6);

                    decimal revenue = ParseKpi(lblTotalRevenue.Text);
                    decimal costs   = ParseKpi(lblTotalCosts.Text);
                    decimal profit  = revenue - costs;
                    decimal margin  = revenue > 0 ? Math.Round(profit / revenue * 100, 1) : 0;

                    string[][] kpiRows =
                    {
                        new[] { "Выручка от реализации",  $"{revenue:N2} руб." },
                        new[] { "Себестоимость продукции", $"{costs:N2} руб." },
                        new[] { "Валовая прибыль",         $"{profit:N2} руб." },
                        new[] { "Рентабельность продаж",   $"{margin} %" },
                    };

                    var kpiTable = doc.AddTable(kpiRows.Length, 2);
                    kpiTable.Design = TableDesign.TableGrid;
                    for (int i = 0; i < kpiRows.Length; i++)
                    {
                        kpiTable.Rows[i].Cells[0].Paragraphs[0].Append(kpiRows[i][0]).Bold().FontSize(10);
                        kpiTable.Rows[i].Cells[1].Paragraphs[0].Append(kpiRows[i][1]).FontSize(10);
                        kpiTable.Rows[i].Cells[0].FillColor = Color.FromArgb(235, 244, 255);
                        kpiTable.Rows[i].Cells[1].Paragraphs[0].Alignment = Alignment.right;
                    }
                    doc.InsertTable(kpiTable);
                    doc.InsertParagraph("").SpacingAfter(10);

                    // ── Выручка по продукции ──────────────────────────────
                    InsertGridSection(doc, "Выручка по видам продукции", dgvRevenue);

                    // ── Затраты на сырьё ──────────────────────────────────
                    InsertGridSection(doc, "Затраты на сырьё", dgvCosts);

                    // ── Аналитическая заметка ─────────────────────────────
                    doc.InsertParagraph("").SpacingAfter(8);
                    doc.InsertParagraph("Аналитическая заметка")
                       .Bold().FontSize(12)
                       .Color(Color.FromArgb(52, 73, 94))
                       .SpacingAfter(4);

                    string note = profit >= 0
                        ? $"За отчётный период предприятие получило прибыль в размере {profit:N2} руб. " +
                          $"Рентабельность продаж составила {margin}%, что является " +
                          (margin >= 20 ? "хорошим показателем." : "удовлетворительным показателем — рекомендуется проработать снижение себестоимости.")
                        : $"За отчётный период зафиксирован убыток в размере {Math.Abs(profit):N2} руб. " +
                          "Рекомендуется провести анализ структуры затрат и ценовой политики.";

                    doc.InsertParagraph(note).FontSize(10).SpacingAfter(6);

                    // ── Подпись ───────────────────────────────────────────
                    doc.InsertParagraph("").SpacingAfter(8);
                    var footer = doc.InsertParagraph(
                        $"Отчёт сформирован системой управления производством   |   {DateTime.Now:dd.MM.yyyy HH:mm}");
                    footer.Alignment = Alignment.center;
                    footer.FontSize(9).Color(Color.Gray);

                    doc.Save();
                }

                MessageBox.Show($"Финансовый отчёт успешно создан!\n{filename}", "Успех",
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

        // Вставляет секцию с заголовком и таблицей из DataGridView
        private void InsertGridSection(DocX doc, string heading, DataGridView grid)
        {
            doc.InsertParagraph(heading)
               .Bold().FontSize(13)
               .Color(Color.FromArgb(52, 73, 94))
               .SpacingAfter(4);

            var visibleCols = new System.Collections.Generic.List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in grid.Columns)
                if (col.Visible) visibleCols.Add(col);

            int colCount = visibleCols.Count;
            if (colCount == 0 || grid.Rows.Count == 0)
            {
                doc.InsertParagraph("Нет данных за выбранный период.").FontSize(10).SpacingAfter(8);
                return;
            }

            var table = doc.AddTable(grid.Rows.Count + 1, colCount);
            table.Design = TableDesign.TableGrid;

            // Заголовок таблицы
            for (int c = 0; c < colCount; c++)
            {
                var cell = table.Rows[0].Cells[c];
                cell.Paragraphs[0].Append(visibleCols[c].HeaderText).Bold().FontSize(9)
                    .Color(Color.White);
                cell.Paragraphs[0].Alignment = Alignment.center;
                cell.FillColor = Color.FromArgb(39, 174, 96);
            }

            // Строки данных
            for (int r = 0; r < grid.Rows.Count; r++)
            {
                bool even = (r % 2 == 0);
                for (int c = 0; c < colCount; c++)
                {
                    var cell = table.Rows[r + 1].Cells[c];
                    string val = grid.Rows[r].Cells[visibleCols[c].Index].Value?.ToString() ?? "";
                    cell.Paragraphs[0].Append(val).FontSize(9);
                    if (even) cell.FillColor = Color.FromArgb(242, 255, 248);
                }
            }

            doc.InsertTable(table);
            doc.InsertParagraph("").SpacingAfter(10);
        }

        // Вытаскивает число из строки вида "12 345 руб." или "15.3 %"
        private decimal ParseKpi(string text)
        {
            text = text.Replace("руб.", "").Replace("%", "").Replace(" ", "").Replace("\u00A0", "").Trim();
            return decimal.TryParse(text, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : 0;
        }
    }
}
