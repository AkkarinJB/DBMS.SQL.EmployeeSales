using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSalesApp.Controllers
{
    public class EmployeeController : Controller
    {
        
        private readonly string _connectionString = "Server=localhost;Database=Northwind;User Id=sa;Password=Arin3122;";

        // หน้าแรกที่แสดงข้อมูลพนักงาน
        public IActionResult Index()
        {
            List<EmployeeSales> employeeSalesList = new List<EmployeeSales>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // SQL Query 
                    string sqlQuery = @"
                        SELECT e.EmployeeID AS รหัสพนักงาน, 
                               e.TitleOfCourtesy + e.FirstName + ' ' + e.LastName AS ชื่อเต็มพนักงาน, 
                               e.Title AS ตำแหน่ง, 
                               COUNT(o.OrderID) AS จำนวนใบสั่งซื้อ, 
                               CONVERT(decimal(10,2), SUM(od.Quantity * od.UnitPrice * (1 - od.Discount))) AS ยอดเงินรวม
                        FROM Employees e
                        JOIN Orders o ON e.EmployeeID = o.EmployeeID
                        JOIN [Order Details] od ON o.OrderID = od.OrderID
                        GROUP BY e.EmployeeID, e.TitleOfCourtesy, e.FirstName, e.LastName, e.Title;
                    ";

                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employeeSalesList.Add(new EmployeeSales
                        {
                            EmployeeID = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Title = reader.GetString(2),
                            TotalOrders = reader.GetInt32(3),
                            TotalSales = reader.GetDecimal(4)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
               
                ViewBag.ErrorMessage = "เกิดข้อผิดพลาดในการดึงข้อมูล: " + ex.Message;
                return View("Error");
            }

            // ส่งข้อมูลไปยัง View
            return View(employeeSalesList);
        }

        // ดึงข้อมูลใบสั่งซื้อที่เกี่ยวข้องกับพนักงานคนนี้
        public IActionResult OrderList(int employeeID)
        {
            if (employeeID == 0)
            {
                // ถ้าไม่ได้รับ employeeID ส่งกลับไปยังหน้าแสดงผลข้อผิดพลาด
                return RedirectToAction("Index");
            }

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // SQL Query สำหรับดึงข้อมูลใบสั่งซื้อของพนักงาน
                    string sqlQuery = @"
                        SELECT o.OrderID, o.EmployeeID, o.OrderDate, 
                               CONVERT(decimal(10,2), SUM(od.Quantity * od.UnitPrice * (1 - od.Discount))) AS TotalAmount
                        FROM Orders o
                        JOIN [Order Details] od ON o.OrderID = od.OrderID
                        WHERE o.EmployeeID = @EmployeeID
                        GROUP BY o.OrderID, o.EmployeeID, o.OrderDate
                    ";

                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        orderDetailsList.Add(new OrderDetails
                        {
                            OrderID = reader.GetInt32(0),
                            EmployeeID = reader.GetInt32(1),
                            OrderDate = reader.GetDateTime(2),
                            TotalAmount = reader.GetDecimal(3)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // จัดการข้อผิดพลาดในกรณีที่เกิดข้อผิดพลาดใน Query
                ViewBag.ErrorMessage = "เกิดข้อผิดพลาดในการดึงข้อมูลใบสั่งซื้อ: " + ex.Message;
                return View("Error");
            }

            // ส่งข้อมูลใบสั่งซื้อไปยัง View
            return View(orderDetailsList);
        }

        // ดึงข้อมูลพนักงานและยอดขาย
        public IActionResult EmployeeList()
        {
            List<EmployeeSales> employeeSalesList = new List<EmployeeSales>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Query 
                    string sqlQuery = @"
                        SELECT e.EmployeeID AS รหัสพนักงาน, 
                               e.TitleOfCourtesy + e.FirstName + ' ' + e.LastName AS ชื่อเต็มพนักงาน, 
                               e.Title AS ตำแหน่ง, 
                               COUNT(o.OrderID) AS จำนวนใบสั่งซื้อ, 
                               CONVERT(decimal(10,2), SUM(od.Quantity * od.UnitPrice * (1 - od.Discount))) AS ยอดเงินรวม
                        FROM Employees e
                        JOIN Orders o ON e.EmployeeID = o.EmployeeID
                        JOIN [Order Details] od ON o.OrderID = od.OrderID
                        GROUP BY e.EmployeeID, e.TitleOfCourtesy, e.FirstName, e.LastName, e.Title;
                    ";

                    SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        employeeSalesList.Add(new EmployeeSales
                        {
                            EmployeeID = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Title = reader.GetString(2),
                            TotalOrders = reader.GetInt32(3),
                            TotalSales = reader.GetDecimal(4)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                
                ViewBag.ErrorMessage = "เกิดข้อผิดพลาดในการดึงข้อมูลพนักงาน: " + ex.Message;
                return View("Error");
            }

            // ส่งข้อมูลไปยัง View
            return View(employeeSalesList);
        }
    }
}