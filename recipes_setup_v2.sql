-- ================================================
-- НАСТРОЙКА РЕЦЕПТОВ ДЛЯ ПРОИЗВОДСТВА
-- Версия 2.0 - с удалением "Сало украинское"
-- ================================================

USE MeatProductionDB;

-- ================================================
-- ШАГ 1: ОЧИСТКА (удаление старых данных)
-- ================================================

-- Удаляем все записи "Сало украинское"
DELETE FROM Products 
WHERE ProductName LIKE '%Сало%украинское%' 
   OR ProductName LIKE '%украинское%'
   OR ProductName = 'Сало украинское';

-- Очищаем таблицу рецептов (если нужно пересоздать)
-- TRUNCATE TABLE Recipes;

-- ================================================
-- ШАГ 2: СОЗДАНИЕ ТАБЛИЦЫ РЕЦЕПТОВ (если не существует)
-- ================================================

CREATE TABLE IF NOT EXISTS Recipes (
    RecipeID INT AUTO_INCREMENT PRIMARY KEY,
    ProductID INT NOT NULL,
    MaterialID INT NOT NULL,
    RequiredQuantity DECIMAL(10,2) NOT NULL,
    Unit VARCHAR(20) NOT NULL DEFAULT 'кг',
    Notes TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE,
    FOREIGN KEY (MaterialID) REFERENCES RawMaterials(MaterialID) ON DELETE CASCADE,
    UNIQUE KEY unique_recipe (ProductID, MaterialID)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ================================================
-- ШАГ 3: ДОБАВЛЕНИЕ РЕЦЕПТОВ
-- Все рецепты рассчитаны на 100 кг готовой продукции
-- ================================================

-- РЕЦЕПТ 1: Грудинка копченая
-- На 100 кг готовой грудинки требуется:
INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Грудинка копченая' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Свинина%' LIMIT 1),
 120.00, 'кг', 'Основное сырье - грудинка свиная с прослойками жира')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Грудинка копченая' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Соль%' LIMIT 1),
 2.50, 'кг', 'Для посола и консервирования')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Грудинка копченая' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Специи%' LIMIT 1),
 0.50, 'кг', 'Перец черный молотый, чеснок сушеный')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

-- РЕЦЕПТ 2: Окорок копченый
-- На 100 кг готового окорока требуется:
INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Окорок копченый' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Свинина%' LIMIT 1),
 115.00, 'кг', 'Окорок свиной бескостный высшего сорта')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Окорок копченый' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Соль%' LIMIT 1),
 3.00, 'кг', 'Для глубокого посола')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Окорок копченый' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Специи%' LIMIT 1),
 0.80, 'кг', 'Смесь специй для копчения: кориандр, лавровый лист, перец')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

-- РЕЦЕПТ 3: Корейка копченая
-- На 100 кг готовой корейки требуется:
INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Корейка копченая' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Свинина%' LIMIT 1),
 118.00, 'кг', 'Корейка свиная на кости или бескостная')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Корейка копченая' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Соль%' LIMIT 1),
 2.80, 'кг', 'Для равномерного посола')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Корейка копченая' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Специи%' LIMIT 1),
 0.60, 'кг', 'Специи для корейки: перец, чеснок, тмин')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

-- РЕЦЕПТ 4: Ребрышки копченые
-- На 100 кг готовых ребрышек требуется:
INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Ребрышки копченые' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Свинина%' LIMIT 1),
 125.00, 'кг', 'Ребра свиные мясные с хорошей прослойкой')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Ребрышки копченые' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Соль%' LIMIT 1),
 2.00, 'кг', 'Для поверхностного посола')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

INSERT INTO Recipes (ProductID, MaterialID, RequiredQuantity, Unit, Notes) VALUES
((SELECT ProductID FROM Products WHERE ProductName = 'Ребрышки копченые' LIMIT 1),
 (SELECT MaterialID FROM RawMaterials WHERE MaterialName LIKE '%Специи%' LIMIT 1),
 1.00, 'кг', 'Специи для ребрышек: паприка, чили, чеснок, перец')
ON DUPLICATE KEY UPDATE 
    RequiredQuantity = VALUES(RequiredQuantity),
    Notes = VALUES(Notes);

-- ================================================
-- ШАГ 4: ПРОВЕРКА РЕЗУЛЬТАТОВ
-- ================================================

-- Показываем все созданные рецепты
SELECT 
    p.ProductName AS 'Продукт',
    rm.MaterialName AS 'Сырье',
    r.RequiredQuantity AS 'На 100кг',
    r.Unit AS 'Ед.изм.',
    r.Notes AS 'Примечание'
FROM Recipes r
JOIN Products p ON r.ProductID = p.ProductID
JOIN RawMaterials rm ON r.MaterialID = rm.MaterialID
ORDER BY p.ProductName, rm.MaterialName;

-- Статистика по рецептам
SELECT 
    COUNT(DISTINCT ProductID) AS 'Продуктов с рецептами',
    COUNT(*) AS 'Всего ингредиентов',
    SUM(RequiredQuantity) AS 'Общий вес ингредиентов (на 100кг продукта)'
FROM Recipes;

-- Проверяем, что "Сало украинское" удалено
SELECT COUNT(*) AS 'Количество "Сало украинское"' 
FROM Products 
WHERE ProductName LIKE '%Сало%украинское%' 
   OR ProductName LIKE '%украинское%';

-- Должно показать 0!

-- ================================================
-- СПРАВОЧНАЯ ИНФОРМАЦИЯ
-- ================================================

/*
ПОЯСНЕНИЯ К РЕЦЕПТАМ:

1. Все рецепты рассчитаны на 100 кг ГОТОВОЙ продукции
2. Коэффициент выхода учитывает потери при копчении:
   - Усушка: 10-20%
   - Потери при обработке: 2-5%

3. Соотношение основного сырья к готовой продукции:
   - Грудинка: 120 кг → 100 кг (выход 83%)
   - Окорок: 115 кг → 100 кг (выход 87%)
   - Корейка: 118 кг → 100 кг (выход 85%)
   - Ребрышки: 125 кг → 100 кг (выход 80%)

4. При производстве система автоматически:
   - Пересчитывает нужное количество сырья
   - Проверяет наличие на складе
   - Списывает сырье по рецепту
   - Добавляет готовую продукцию

ПРИМЕР РАСЧЕТА:
Если нужно произвести 50 кг грудинки:
- Коэффициент = 50 / 100 = 0.5
- Свинина: 120 * 0.5 = 60 кг
- Соль: 2.5 * 0.5 = 1.25 кг
- Специи: 0.5 * 0.5 = 0.25 кг
*/

-- ================================================
-- КОНЕЦ СКРИПТА
-- ================================================

SELECT '✅ Рецепты успешно установлены!' AS 'Статус';
SELECT '❌ "Сало украинское" удалено из базы' AS 'Очистка';
SELECT '🎯 Система готова к работе!' AS 'Результат';
