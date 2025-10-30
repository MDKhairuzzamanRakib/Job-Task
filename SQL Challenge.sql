CREATE DATABASE JobTask
GO

USE JobTask
GO

CREATE TABLE Students (
	StudentID INT,
	Name VARCHAR(100),
	Subject VARCHAR(100),
	Score INT
)

----------------- Question 1 ---------------------

--Use indexes on:
--Frequently filtered/search columns (WHERE, JOIN, ORDER BY)
--Avoid over-indexing (slows down inserts/updates)
--Example:

CREATE INDEX idx_students_subject ON Students(Subject);

----------------- Question 2 ---------------------

SELECT s1.Subject, s1.Name, s1.Score
FROM Students s1
WHERE s1.Score = (
  SELECT MAX(s2.Score)
  FROM Students s2
  WHERE s2.Subject = s1.Subject
);


----------------- Question 3 ---------------------


CREATE TABLE Products (
    product_id INT PRIMARY KEY,
    product_name VARCHAR(100),
    category VARCHAR(50),
    unit_price DECIMAL(10, 2)
);

INSERT INTO Products (product_id, product_name, category, unit_price) VALUES
(101, 'Laptop', 'Electronics', 500.00),
(102, 'Smartphone', 'Electronics', 300.00),
(103, 'Headphones', 'Electronics', 30.00),
(104, 'Keyboard', 'Electronics', 20.00),
(105, 'Mouse', 'Electronics', 15.00);

CREATE TABLE Sales (
    sale_id INT PRIMARY KEY,
    product_id INT,
    quantity_sold INT,
    sale_date DATE,
    total_price DECIMAL(10, 2),
    FOREIGN KEY (product_id) REFERENCES Products(product_id)
);


INSERT INTO Sales (sale_id, product_id, quantity_sold, sale_date, total_price) VALUES
(1, 101, 5, '2024-01-01', 2500.00),
(2, 102, 3, '2024-01-02', 900.00),
(3, 103, 2, '2024-01-02', 60.00),
(4, 104, 4, '2024-01-03', 80.00),
(5, 105, 6, '2024-01-03', 90.00);

----------------- Question 3.1 ---------------------

SELECT SUM(quantity_sold) AS TotalQuantitySold FROM Sales;

----------------- Question 3.2 ---------------------

SELECT TOP 1 product_name, unit_price 
FROM Products
ORDER BY unit_price DESC


----------------- Question 3.3 ---------------------

SELECT p.product_name
FROM Products p
LEFT JOIN Sales s ON p.product_id = s.product_id
WHERE s.product_id IS NULL;


