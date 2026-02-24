using System;
using System.Collections.Generic;

namespace PersonalOrganizer
{
    public class SalaryCalculator
    {
        public enum ExperienceLevel
        {
            Junior = 1,
            Mid = 2,
            Senior = 3,
            Lead = 4
        }

        public enum EducationLevel
        {
            Associate = 1,
            Bachelor = 2,
            Master = 3,
            PhD = 4
        }

        public enum ProjectComplexity
        {
            Low = 1,
            Medium = 2,
            High = 3
        }

        private const double BASE_SALARY = 20000.0; // BMO 2023 asgari ücret temel değeri
        private const double EXPERIENCE_MULTIPLIER = 0.2; // Her seviye için %20 artış
        private const double EDUCATION_MULTIPLIER = 0.15; // Her seviye için %15 artış
        private const double PROJECT_COMPLEXITY_MULTIPLIER = 0.1; // Her seviye için %10 artış

        public double CalculateSalary(
            ExperienceLevel experience,
            EducationLevel education,
            ProjectComplexity complexity,
            bool isPartTime = false)
        {
            double salary = BASE_SALARY;

            // Deneyim hesaplaması
            salary *= (1 + ((int)experience - 1) * EXPERIENCE_MULTIPLIER);

            // Eğitim hesaplaması
            salary *= (1 + ((int)education - 1) * EDUCATION_MULTIPLIER);

            // Proje karmaşıklığı hesaplaması
            salary *= (1 + ((int)complexity - 1) * PROJECT_COMPLEXITY_MULTIPLIER);

            // Part-time çalışma durumu
            if (isPartTime)
            {
                salary *= 0.5;
            }

            return Math.Round(salary, 2);
        }

        public Dictionary<string, double> GetSalaryComponents(
            ExperienceLevel experience,
            EducationLevel education,
            ProjectComplexity complexity)
        {
            var components = new Dictionary<string, double>
            {
                { "Temel Maaş", BASE_SALARY },
                { "Deneyim Katkısı", BASE_SALARY * ((int)experience - 1) * EXPERIENCE_MULTIPLIER },
                { "Eğitim Katkısı", BASE_SALARY * ((int)education - 1) * EDUCATION_MULTIPLIER },
                { "Proje Karmaşıklığı Katkısı", BASE_SALARY * ((int)complexity - 1) * PROJECT_COMPLEXITY_MULTIPLIER }
            };

            return components;
        }
    }
} 