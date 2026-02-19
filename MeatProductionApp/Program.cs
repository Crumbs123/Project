using System;
using System.Windows.Forms;
using MeatProductionApp.Forms;

namespace MeatProductionApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Проверка подключения к БД
            if (!MeatProductionApp.Classes.DatabaseHelper.TestConnection())
            {
                MessageBox.Show(
                    "Не удалось подключиться к базе данных!\n\n" +
                    "Убедитесь, что:\n" +
                    "1. MySQL Server запущен\n" +
                    "2. База данных MeatProductionDB создана\n" +
                    "3. Параметры подключения верны\n\n" +
                    "Проверьте настройки в файле DatabaseHelper.cs",
                    "Ошибка подключения к БД",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            
            Application.Run(new LoginForm());
        }
    }
}
