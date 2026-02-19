using System;
using System.Drawing;
using System.Windows.Forms;
using MeatProductionApp.Classes;

namespace MeatProductionApp.Forms
{
    public partial class LoginForm : Form
    {
        // ВАЖНО: Объявление полей В НАЧАЛЕ класса
        private Captcha captcha;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private PictureBox picCaptcha;
        private TextBox txtCaptcha;
        private Button btnLogin;
        private Panel pnlHeader;
        private Label lblTitle;
        private Panel pnlMain;
        private Label lblIcon;
        private Label lblLogin;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblCaptcha;
        private Button btnRefreshCaptcha;
        private Label lblStatus;
        
        public LoginForm()
        {
            InitializeComponent();
            // Вызываем RefreshCaptcha через событие Load, когда все контролы уже созданы
            this.Load += LoginForm_Load;
        }
        
        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Генерируем капчу после полной загрузки формы
            RefreshCaptcha();
        }

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitle = new Label();
            pnlMain = new Panel();
            lblIcon = new Label();
            lblLogin = new Label();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            lblCaptcha = new Label();
            picCaptcha = new PictureBox();
            btnRefreshCaptcha = new Button();
            txtCaptcha = new TextBox();
            btnLogin = new Button();
            lblStatus = new Label();
            pnlHeader.SuspendLayout();
            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picCaptcha).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(41, 128, 185);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(450, 80);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(450, 80);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "СИСТЕМА УПРАВЛЕНИЯ\nПРОИЗВОДСТВОМ";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.White;
            pnlMain.BorderStyle = BorderStyle.FixedSingle;
            pnlMain.Controls.Add(lblIcon);
            pnlMain.Controls.Add(lblLogin);
            pnlMain.Controls.Add(lblUsername);
            pnlMain.Controls.Add(txtUsername);
            pnlMain.Controls.Add(lblPassword);
            pnlMain.Controls.Add(txtPassword);
            pnlMain.Controls.Add(lblCaptcha);
            pnlMain.Controls.Add(picCaptcha);
            pnlMain.Controls.Add(btnRefreshCaptcha);
            pnlMain.Controls.Add(txtCaptcha);
            pnlMain.Controls.Add(btnLogin);
            pnlMain.Location = new Point(50, 100);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(350, 400);
            pnlMain.TabIndex = 1;
            // 
            // lblIcon
            // 
            lblIcon.Font = new Font("Segoe UI", 36F, FontStyle.Regular, GraphicsUnit.Point);
            lblIcon.Location = new Point(150, 20);
            lblIcon.Name = "lblIcon";
            lblIcon.Size = new Size(80, 50);
            lblIcon.TabIndex = 0;
            lblIcon.Text = "👤";
            // 
            // lblLogin
            // 
            lblLogin.AutoSize = true;
            lblLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblLogin.ForeColor = Color.FromArgb(52, 73, 94);
            lblLogin.Location = new Point(100, 85);
            lblLogin.Name = "lblLogin";
            lblLogin.Size = new Size(129, 21);
            lblLogin.TabIndex = 1;
            lblLogin.Text = "Вход в систему";
            // 
            // lblUsername
            // 
            lblUsername.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            lblUsername.Location = new Point(30, 125);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(100, 25);
            lblUsername.TabIndex = 2;
            lblUsername.Text = "Логин:";
            // 
            // txtUsername
            // 
            txtUsername.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txtUsername.Location = new Point(30, 150);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(290, 27);
            txtUsername.TabIndex = 3;
            // 
            // lblPassword
            // 
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            lblPassword.Location = new Point(30, 190);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(100, 25);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Пароль:";
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txtPassword.Location = new Point(30, 215);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.Size = new Size(290, 27);
            txtPassword.TabIndex = 5;
            txtPassword.KeyPress += TxtPassword_KeyPress;
            // 
            // lblCaptcha
            // 
            lblCaptcha.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            lblCaptcha.Location = new Point(30, 255);
            lblCaptcha.Name = "lblCaptcha";
            lblCaptcha.Size = new Size(200, 25);
            lblCaptcha.TabIndex = 6;
            lblCaptcha.Text = "Введите код с картинки:";
            // 
            // picCaptcha
            // 
            picCaptcha.BackColor = Color.White;
            picCaptcha.BorderStyle = BorderStyle.FixedSingle;
            picCaptcha.Location = new Point(30, 280);
            picCaptcha.Name = "picCaptcha";
            picCaptcha.Size = new Size(200, 50);
            picCaptcha.SizeMode = PictureBoxSizeMode.StretchImage;
            picCaptcha.TabIndex = 7;
            picCaptcha.TabStop = false;
            // 
            // btnRefreshCaptcha
            // 
            btnRefreshCaptcha.Cursor = Cursors.Hand;
            btnRefreshCaptcha.FlatAppearance.BorderColor = Color.FromArgb(41, 128, 185);
            btnRefreshCaptcha.FlatStyle = FlatStyle.Flat;
            btnRefreshCaptcha.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            btnRefreshCaptcha.Location = new Point(240, 280);
            btnRefreshCaptcha.Name = "btnRefreshCaptcha";
            btnRefreshCaptcha.Size = new Size(50, 50);
            btnRefreshCaptcha.TabIndex = 8;
            btnRefreshCaptcha.Text = "🔄";
            btnRefreshCaptcha.Click += BtnRefreshCaptcha_Click;
            // 
            // txtCaptcha
            // 
            txtCaptcha.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txtCaptcha.Location = new Point(30, 340);
            txtCaptcha.MaxLength = 5;
            txtCaptcha.Name = "txtCaptcha";
            txtCaptcha.Size = new Size(140, 27);
            txtCaptcha.TabIndex = 9;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(46, 204, 113);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(180, 340);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(140, 35);
            btnLogin.TabIndex = 10;
            btnLogin.Text = "Войти";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += BtnLogin_Click;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblStatus.ForeColor = Color.Red;
            lblStatus.Location = new Point(50, 510);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(350, 25);
            lblStatus.TabIndex = 2;
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            BackColor = Color.FromArgb(240, 240, 240);
            ClientSize = new Size(450, 550);
            Controls.Add(pnlHeader);
            Controls.Add(pnlMain);
            Controls.Add(lblStatus);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Вход в систему - Управление производством";
            pnlHeader.ResumeLayout(false);
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picCaptcha).EndInit();
            ResumeLayout(false);
        }

        private void RefreshCaptcha()
        {
            try
            {
                // Проверяем что picCaptcha создан
                if (picCaptcha == null)
                {
                    MessageBox.Show("PictureBox не инициализирован!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Освобождаем предыдущее изображение
                if (picCaptcha.Image != null)
                {
                    var oldImage = picCaptcha.Image;
                    picCaptcha.Image = null;
                    oldImage.Dispose();
                }
                
                // Создаем новую капчу
                captcha = new Captcha();
                
                // Генерируем изображение
                Bitmap captchaImage = captcha.GenerateImage(200, 50);
                
                if (captchaImage == null)
                {
                    MessageBox.Show("Не удалось создать изображение капчи!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Устанавливаем изображение
                picCaptcha.Image = captchaImage;
                
                // Принудительно обновляем
                picCaptcha.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации капчи:\n{ex.Message}\n\n{ex.StackTrace}", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                if (lblStatus != null)
                {
                    lblStatus.Text = $"Ошибка капчи: {ex.Message}";
                    lblStatus.ForeColor = Color.Red;
                }
            }
        }
        
        private void BtnRefreshCaptcha_Click(object sender, EventArgs e)
        {
            RefreshCaptcha();
            txtCaptcha.Clear();
            lblStatus.Text = "";
        }
        
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            PerformLogin();
        }
        
        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PerformLogin();
                e.Handled = true;
            }
        }
        
        private void PerformLogin()
        {
            lblStatus.Text = "";
            
            // Проверка заполнения полей
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblStatus.Text = "Введите логин";
                lblStatus.ForeColor = Color.Red;
                txtUsername.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.Text = "Введите пароль";
                lblStatus.ForeColor = Color.Red;
                txtPassword.Focus();
                return;
            }
            
            // Проверка капчи
            if (txtCaptcha.Text.ToUpper() != captcha.Text.ToUpper())
            {
                lblStatus.Text = "Неверный код с картинки";
                lblStatus.ForeColor = Color.Red;
                txtCaptcha.Clear();
                RefreshCaptcha();
                return;
            }
            
            // Аутентификация
            User user = User.Authenticate(txtUsername.Text, txtPassword.Text);
            
            if (user != null)
            {
                User.CurrentUser = user;
                
                // Логируем вход
                DatabaseHelper.LogAction(user.UserID, "Вход в систему", null, null, 
                    $"Пользователь {user.Username} ({user.Role}) вошел в систему");
                
                lblStatus.Text = "Вход выполнен успешно!";
                lblStatus.ForeColor = Color.Green;
                
                // Открываем главную форму
                this.Hide();
                MainForm mainForm = new MainForm();
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                lblStatus.Text = "Неверный логин или пароль";
                lblStatus.ForeColor = Color.Red;
                txtPassword.Clear();
                txtCaptcha.Clear();
                RefreshCaptcha();
            }
        }
    }
}
