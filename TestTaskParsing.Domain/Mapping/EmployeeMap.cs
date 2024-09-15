using CsvHelper.Configuration;

//With the help of CSVHelper set up mapping, using ClassMap
public class EmployeeMap : ClassMap<Employee>
{
    public EmployeeMap()
    {
        Map(m => m.PayrollNumber).Name("Personnel_Records.Payroll_Number");
        Map(m => m.ForeName).Name("Personnel_Records.Forenames");
        Map(m => m.Surname).Name("Personnel_Records.Surname");
        Map(m => m.DateOfBirth).Name("Personnel_Records.Date_of_Birth").TypeConverterOption.Format("dd/M/yyyy");
        Map(m => m.Telephone).Name("Personnel_Records.Telephone");
        Map(m => m.Mobile).Name("Personnel_Records.Mobile");
        Map(m => m.Address).Name("Personnel_Records.Address");
        Map(m => m.City).Name("Personnel_Records.Address_2");
        Map(m => m.PostCode).Name("Personnel_Records.Postcode");
        Map(m => m.EMailHome).Name("Personnel_Records.EMail_Home");
        Map(m => m.StartDate).Name("Personnel_Records.Start_Date").TypeConverterOption.Format("dd/M/yyyy");
    }
}
