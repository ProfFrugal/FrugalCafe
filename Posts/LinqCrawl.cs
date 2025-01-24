using System.Collections.Generic;
using System.Linq;

namespace FrugalCafe.Posts
{
    public enum Department
    {
        IT
    }

    public class Employee
    {
        public string Name;
        public double Salary;
        public Department Department;
    }

    internal class LinqCrawl
    {
        public static void Original(IEnumerable<Employee> employees)
        {
            var highSalaryItNames = new List<string>();

            foreach (var e in employees)
            {
                if ((e.Salary >= 100000) && (e.Department == Department.IT))
                {
                    highSalaryItNames.Add(e.Name);
                }
            }

            var departmentGroups = new Dictionary<Department, List<Employee>>();

            foreach (var e in employees)
            {
                if (!departmentGroups.ContainsKey(e.Department)) 
                {
                    departmentGroups[e.Department] = new List<Employee>();
                }

                departmentGroups[e.Department].Add(e);
            }
        }

        public static void Linq(IEnumerable<Employee> employees)
        {
            List<string> highSalaryItNames = employees
                .Where(e => (e.Salary >= 100000) && (e.Department == Department.IT))
                .Select(e => e.Name).ToList();

            var departmentGroups = employees
                .GroupBy(e => e.Department)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        public static void FrugalCafe(IEnumerable<Employee> employees)
        {
            var highSalaryItNames = new List<string>();
            var departmentGroups = new Dictionary<Department, List<Employee>>();

            foreach (var e in employees)
            {
                if ((e.Salary >= 100000) && (e.Department == Department.IT))
                {
                    highSalaryItNames.Add(e.Name);
                }

                if (!departmentGroups.TryGetValue(e.Department, out var list))
                {
                    departmentGroups[e.Department] = list = new List<Employee>(1);
                }

                list.Add(e);
            }
        }
    }
}
